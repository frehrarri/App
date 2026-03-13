import { getTicketsPartial } from "/js/Tickets/_Tickets.js";
import { loadModule } from "/js/__moduleLoader.js";

export async function init() {
    removeEventListeners();

    const sectionHistory = [];
    const partial = await getTicketSettingsPartial();
    const container = document.querySelector("#settings-content");

    if (partial && container) {
        container.innerHTML = partial;

        container.addEventListener("click", (e) => handleClicks(e, sectionHistory));
        container.addEventListener("change", (e) => handleChange(e, sectionHistory));
        trackEventListener(container, "click", handleClicks);
        trackEventListener(container, "change", handleChange);

        updateBreadCrumb();
    }
}

async function getTicketSettingsPartial() {
    try {
        const response = await axios.get('/Tickets/SettingsPartial');
        return response.data;
    }
    catch (error) {
        console.log(`Error getTicketSettingsPartial: ${error}`)
    }
}

async function handleClicks(e, sectionHistory) {
    const btn = e.target.closest("button");
    
    if (btn) {
        if (btn.id == "add-section-btn")
            addSection(null, sectionHistory);

        else if (btn.id == 'save-settings-btn')
            await save(e);

        else if (btn.id == "undo-section-btn")
            undoSection(sectionHistory);
    }
    else
    {
        const sectionTag = e.target.closest(".delete-section");
        if (!sectionTag || sectionTag.hasAttribute('disabled'))
            return;
        //else if (sectionTag.disabled == true)
        //    return;
        else if (sectionTag)
            removeSection(e, sectionHistory);
    }
}

async function handleChange(e, sectionHistory){
    if (e.target.matches("[name='rdo-repeat']"))
        toggleSprintDateControls(e);

    else if (e.target.matches("[name='rdo-section']"))
        toggleSectionControls(e, sectionHistory);
}


async function updateBreadCrumb() {
    const ol = document.querySelector('.breadcrumb');

    ol.innerHTML = '';

    const li1 = document.createElement('li');
    li1.classList.add('breadcrumb-item');
    li1.classList.add('active');
    li1.innerText = 'Settings';

    ol.appendChild(li1);

    const li2 = document.createElement('li');
    li2.classList.add('breadcrumb-item');
    li2.classList.add('active');
    li2.textContent = 'Ticket Settings';

    ol.appendChild(li2);
}

function addSection(section, sectionHistory) {
    let input = document.getElementById('section-title-input').value.trim();
    if (section != null)
        input = section;

    if (!input) return;

    //create tag and add to new container
    let newSection = document.createElement("span")
    newSection.className = "delete-section";
    newSection.innerHTML = `${input} <span class='delete'>x</span>`;

    document.getElementById('section-settings')?.append(newSection);

    //clear after adding to container
    document.getElementById('section-title-input').value = "";

    sectionHistory.push({
        type: "add",
        element: newSection
    });

}



function undoSection(sectionHistory) {
    if (!sectionHistory.length) return;

    const lastAction = sectionHistory.pop();

    if (lastAction.type === "add") {
        lastAction.element.remove();
    } else if (lastAction.type === "remove") {
        if (lastAction.nextSibling) {
            lastAction.parent.insertBefore(lastAction.element, lastAction.nextSibling);
        } else {
            lastAction.parent.appendChild(lastAction.element);
        }
    }
}

function removeSection(e, sectionHistory) {
    let target;
    if (e.target.classList.contains('delete')) {
        target = e.target.parentElement;
    } else {
        target = e.target;
    }       /* e.target.parentElement.remove();*/

    sectionHistory.push({
        type: "remove",
        element: target,
        parent: target.parentElement,
        nextSibling: target.nextSibling // for restoring position
    });

    target.remove();
}

async function save(e) {
    debugger;
    e.stopPropagation();
    e.preventDefault();

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    const repeatOption = document.querySelector("[name='rdo-repeat']:checked").value;
    const sprintStart = document.getElementById('start-date')?.value;
    const sprintEnd = document.getElementById('end-date')?.value;

    const sectionSetting = document.querySelector("[name='rdo-section']:checked")?.value;

    const sections = Array.from(document.querySelectorAll('.delete-section')).map((s, i) => {
        const clone = s.cloneNode(true);
        clone.querySelector('.delete')?.remove();
        return {
            Title: clone.textContent.trim(),
            SectionOrder: i
        };
    });

    const dto = {
        RepeatSprintOption: parseInt(repeatOption),
        SprintStart: sprintStart,
        SprintEnd: sprintEnd || null,
        SectionSetting: parseInt(sectionSetting),
        Sections: sections ?? []
    }

    let response;

    try {
        response = await axios.post('/Tickets/SaveSettings', dto, {
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            }
        });

        if (response && response.status === 200) {
            alert("Success");
            //hyperlinkResponse(response, changeTracker, newId, currentVals);
            //changeTracker.clear();
        }
        else {
            alert("Error");
        }

        return true;
    } catch (error) {
        console.error("error: /Tickets/SaveSettings", error);
        return false;
    }

    return response.data; // bool

}

function toggleSprintDateControls(e) {
    let container = document.getElementById('sprint-dates-container');
    if (e.target.id == "rdo-repeat-never") {
        container.classList.add('hidden');
        return;
    }

    container.classList.remove('hidden');

    let endDate = document.getElementById('dv-end-date');
    if (e.target.id == "rdo-repeat-custom") {
        endDate.classList.remove('hidden');
    }
    else {
        endDate.classList.add('hidden');
    }
}

function toggleSectionControls(e, sectionHistory) {
    let container = document.getElementById('sections-container');

    //clear any sections that are already created
    document.getElementById('section-settings').innerHTML = "";

    //change display and add sections
    //don't add sections that are required as they are added to everything via Business layer
    if (e.target.id == "rdo-section-custom") {
        container.classList.remove('hidden');

        //required sections
        const required = getRequiredSections();

        for (let r of required)
            addRequiredSection(r);

    }
    else if (e.target.id == "rdo-section-development")
    {
        container.classList.add('hidden');

        //required sections
        const required = getRequiredSections();

        for (let r of required)
            addRequiredSection(r);

        //custom sections
        let sections = [];

        if (e.target.id == "rdo-section-development") {
            sections.push("Development");
            sections.push("Review");
            sections.push("QA");
            sections.push("Staging");
            sections.push("UAT");
            sections.push("Backlog");
        }

        for (let section of sections)
            addSection(section, sectionHistory);

      
    }

}

function addRequiredSection(section) {
    let input = document.getElementById('section-title-input').value.trim();
    if (section != null)
        input = section;

    if (!input) return;

    //create tag and add to new container
    let newSection = document.createElement("span")
    newSection.className = "delete-section";
    newSection.innerHTML = `${input}`;
    newSection.setAttribute('disabled', 'true');

    document.getElementById('section-settings')?.append(newSection);

    //clear after adding to container
    document.getElementById('section-title-input').value = "";

    //sectionHistory.push({
    //    type: "add",
    //    element: newSection
    //});

}

function getRequiredSections() {
    const sections = [];
    sections.push('Completed');
    sections.push('Discontinued');
    return sections;
}