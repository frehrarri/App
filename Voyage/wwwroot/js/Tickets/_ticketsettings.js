import { loadModule } from "/js/__moduleLoader.js";

export async function init() {
    document.getElementById('add-section-btn').addEventListener("click", addSection);

    document.getElementById('save-settings-btn')?.addEventListener("click", save);

    document.querySelector("#back-btn")?.addEventListener("click", async (e) => {
        const module = await loadModule("tickets");
        await module.getTicketsPartial();
    });

    document.querySelectorAll("[name='rdo-repeat']")?.forEach(el =>
        el.addEventListener("click", toggleSprintDateControls)
    );

    document.querySelectorAll("[name='rdo-section']")?.forEach(el =>
        el.addEventListener("click", toggleSectionControls)
    );

    document.querySelectorAll(".delete-section")?.forEach(el =>
        el.addEventListener("click", removeSection)
    );

    document.querySelectorAll(".delete")?.forEach(el =>
        el.addEventListener("click", removeSection)
    );

    document.getElementById("undo-section-btn")?.addEventListener("click", undoSection);
}

function addSection(section) {

    let input = document.getElementById('section-title-input').value.trim();
    if (section != null)
        input = section;

    if (!input) return;

    //create tag and add to new container
    let newSection = document.createElement("span")
    newSection.className = "delete-section";
    newSection.innerHTML = `${input} <span class='delete'>x</span>`;

    document.getElementById('section-settings').append(newSection);

    //clear after adding to container
    document.getElementById('section-title-input').value = "";

    newSection.querySelector('.delete-section').addEventListener("click", removeSection);
    newSection.querySelector('.delete').addEventListener("click", removeSection);

    sectionHistory.push({
        type: "add",
        element: newSection
    });

}

const sectionHistory = [];

function undoSection() {
    if (!sectionHistory.length) return;

    const lastAction = sectionHistory.pop();

    if (lastAction.type === "add") {
        // Undo add → remove the element
        lastAction.element.remove();
    } else if (lastAction.type === "remove") {
        // Undo remove → re-insert the element at the original position
        if (lastAction.nextSibling) {
            lastAction.parent.insertBefore(lastAction.element, lastAction.nextSibling);
        } else {
            lastAction.parent.appendChild(lastAction.element);
        }
    }
}

function removeSection(e) {
    let target;
    if (e.target.classList.contains('delete')) {
        target = e.target.parentElement;
    } else {
        target = e.target;
    }        e.target.parentElement.remove();

    sectionHistory.push({
        type: "remove",
        element: target,
        parent: target.parentElement,
        nextSibling: target.nextSibling // for restoring position
    });

    target.remove();
}

async function save(e) {
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

        showSuccess(true);
        return true;
    } catch (error) {
        showSuccess(false);
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

function toggleSectionControls(e) {
    let container = document.getElementById('sections-container');

    //clear any sections that are already created
    document.getElementById('section-settings').innerHTML = "";

    //change display and add sections
    //don't add sections that are required as they are added to everything via Business layer
    if (e.target.id == "rdo-section-custom") {
        container.classList.remove('hidden');
    }
    else {
        container.classList.add('hidden');

        let sections = [];

        if (e.target.id == "rdo-section-development") {
            sections.push("Development");
            sections.push("Review");
            sections.push("QA");
            sections.push("Staging");
            sections.push("UAT");
        }

        for (let section of sections)
            addSection(section);
    }

}