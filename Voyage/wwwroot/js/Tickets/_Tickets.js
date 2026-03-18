import { loadModule } from "/js/__moduleLoader.js";
import { getManageTicketPartial } from "/js/Tickets/_manageticket.js"
/*import { getTicketPartial } from "/js/Tickets/_ticket.js";*/

const sectionMap = new Map();
const ticketMap = new Map();

export async function init() {
    removeEventListeners();

    //load initial partial
    let partial = await getTicketsPartial();
    const container = document.querySelector('.main-content');

    if (!container || !partial)
        return;

    container.innerHTML = partial;

    container.addEventListener("click", handleClick);
    container.addEventListener("change", handleChange);
    trackEventListener(container, "click", handleClick);
    trackEventListener(container, "change", handleChange);

    //appendSectionFilters();
    updateBreadcrumb();
    updateNavHeader();

    anonymizeDataAttributes();

}

function getRealId(map, anonId) {
    return [...map.entries()].find(([, uuid]) => uuid === anonId)?.[0];
}

function getAnonymousId(map, realId) {
    if (!map.has(realId)) {
        map.set(realId, crypto.randomUUID());
    }
    return map.get(realId);
}

function anonymizeDataAttributes() {
    // anonymize sections
    document.querySelectorAll('[data-sectionid]').forEach(el => {
        const realId = el.dataset.sectionid;
        const anonId = getAnonymousId(sectionMap, realId);
        el.dataset.sectionid = anonId;
    });

    // anonymize tickets
    document.querySelectorAll('[data-id]').forEach(el => {
        const realId = el.dataset.id;
        const anonId = getAnonymousId(ticketMap, realId);
        el.dataset.id = anonId;
    });
}




    //document.querySelectorAll(".paginate").forEach(el => {
    //    const sectionTitle = el.dataset.section;

    //    //add change event for select list
    //    if (el.id.includes(`sel-take-section`)) {
    //        el.addEventListener("change", (e) => updatePaginatedUI(e, sectionTitle));
    //    }
    //    //click events for buttons
    //    else {
    //        el.addEventListener("click", (e) => updatePaginatedUI(e, sectionTitle))
    //    }
    //});

    //document.querySelector(".settings-btn")?.addEventListener("click", async (e) => getTicketSettings());

function updateNavHeader() {
    const page = document.getElementById('dv-navbar-page-title');
    page.innerText = 'Tickets';
}

function updateBreadcrumb() {
    const ol = document.querySelector('.breadcrumb');

    ol.innerHTML = '';

    const li1 = document.createElement('li');
    li1.classList.add('breadcrumb-item');
    li1.classList.add('active');
    li1.textContent = 'Tickets';

    ol.appendChild(li1);
}

export async function getTicketsPartial() {
    try {
        const response = await axios.get('/Tickets/TicketsPartial');
        return response.data;
    } catch (error) {
        console.error("error: getTicketsPartial", error);
        return false;
    }
}

async function getPaginatedTickets(sprintId, sectionTitle, targetPage, numResults) {

    try {
        const response = await axios.get('/Tickets/GetPaginatedTickets', {
            params: {
                sprintId: sprintId,
                sectionTitle: sectionTitle,
                pageNumber: targetPage,
                pageSize: numResults
            }
        });

        var tableBody = document.querySelector(`#${sectionTitle}-container > table > tbody`);

        tableBody.innerHTML = ""; //empty current table body

        response.data.tickets.forEach(ticket => {
            //create new row
            const row = document.createElement('tr');
            row.className = 'app-table-row';

            //create new cells
            const td1 = document.createElement('td');
            td1.className = 'app-table-data';
            td1.innerHTML = "<input type='checkbox'>";
            row.appendChild(td1);

            const td2 = document.createElement('td');
            td2.className = 'app-table-data';
            td2.innerHTML = `<a href='#' class='goto-ticket' data-id='${ticket.ticketId}'>${ticket.ticketId}</a>`;
            row.appendChild(td2);

            const td3 = document.createElement('td');
            td3.className = 'app-table-data';
            td3.innerHTML = `<a href="#" class="goto-ticket" data-id="${ticket.ticketId}">${ticket.title}</a>`;
            row.appendChild(td3);

            const td4 = document.createElement('td');
            td4.className = 'app-table-data';
            td4.innerText = `${addSpacesToSentence(ticket.status)}`;
            row.appendChild(td4);

            const td5 = document.createElement('td');
            td5.className = 'app-table-data';
            td5.innerText = `${ticket.assignedTo}`;
            row.appendChild(td5);

            const td6 = document.createElement('td');
            td6.className = 'app-table-data';
            td6.innerText = `${formatUtc(ticket.dueDate, false)}`;
            row.appendChild(td6);

            const td7 = document.createElement('td');
            td7.className = 'app-table-data';
            td7.innerText = `${handlePriorityLevel(ticket.priorityLevel)}`;
            row.appendChild(td7);

            const td8 = document.createElement('td');
            td8.className = 'app-table-data';
            td8.innerText = `${formatUtc(ticket.modifiedDate, false)}`;
            row.appendChild(td8);

            const td9 = document.createElement('td');
            td9.className = 'app-table-data';
            td9.innerText = `${ticket.modifiedBy}`;
            row.appendChild(td9);

            const td10 = document.createElement('td');
            td10.className = 'app-table-data';
            td10.innerText = `${formatUtc(ticket.createdDate, false)}`;
            row.appendChild(td10);

            const td11 = document.createElement('td');
            td11.className = 'app-table-data';
            td11.innerText = `${ticket.createdBy}`;
            row.appendChild(td11);

            const td12 = document.createElement('td');
            td12.className = 'app-table-data';
            td12.innerHTML = `<button id="btnViewTicket" class="goto-ticket primary-btn" data-id="${ticket.ticketId}">View</button><span><button id="btnEditTicket" class="edit-btn secondary-btn" data-id="${ticket.ticketId}" data-author="${ticket.createdBy}">Edit</button></span>`;
            row.appendChild(td12);

            //append the row to the table body
            tableBody.appendChild(row);
        });

        toggleEditBtns();

        return true;
    }
    catch (error) {
        console.error("error: getPaginatedTickets", error);
        return false;
    }
        
     
}


async function updatePaginatedUI(e, sectionTitle) {
        let sprintId = document.getElementById('hdnSprint').dataset.sprintid;
        if (sprintId) {
            sprintId = null
        }

        const selectList = document.querySelector(`select.paginate[data-section="${sectionTitle}"]`);
        const numResults = parseInt(selectList.value);

        const pageBtn = document.querySelector(`.paginate.page.selected[data-section="${sectionTitle}"]`)
        const currentPage = parseInt(pageBtn.innerText);
        let targetPage = currentPage;

        const totalTicketCount = parseInt(document.getElementById(`hdnTotalTicketCount${sectionTitle}`).value);
        let numPages = Math.ceil(totalTicketCount / numResults);

        const selectedBtn = e.target.closest(".paginate");

        //left arrow button
        if (selectedBtn.id == `btn-left-section${sectionTitle}`) {

            targetPage = currentPage - 1;
            pageBtn.classList.remove("selected");
            pageBtn.disabled = false;

            const selectedPageButton = document.querySelector(`.paginate.page[data-section="${sectionTitle}"][data-page="${targetPage}"]`);
            selectedPageButton.classList.add("selected");
            selectedPageButton.disabled = true;
        }
        //right arrow button
        if (selectedBtn.id == `btn-right-section${sectionTitle}`) {
            targetPage = currentPage + 1;
            pageBtn.classList.remove("selected");
            pageBtn.disabled = false;

            const selectedPageButton = document.querySelector(`.paginate.page[data-section="${sectionTitle}"][data-page="${targetPage}"]`);
            selectedPageButton.classList.add("selected");
            selectedPageButton.disabled = true;
        }

        //specific page button
        if (selectedBtn.classList.contains('page')) {
            targetPage = parseInt(e.target.dataset.page);

            //remove selected/disabled from previous page button
            pageBtn.classList.remove("selected");
            pageBtn.disabled = false;

            //update selected/disabled to the target page button
            e.target.classList.add("selected");
            e.target.disabled = true;
        }

        //update num of pages when using result count drop down
        if (e.target.id.includes(`sel-take-section`)) {

            //start at first page on update
            targetPage = 1;

            //update number of pages
            numPages = Math.ceil(totalTicketCount / numResults);

            //empty container of old button layout
            const container = document.getElementById(`page-btn-container-${sectionTitle}`);
            container.innerHTML = "";

            //create new buttons
            for (let i = 1; i <= numPages; i++) {
                const btn = document.createElement("button");
                btn.classList.add("paginate", "page");
                btn.dataset.section = sectionTitle;
                btn.dataset.page = i;
                btn.innerText = i;

                if (i === targetPage) {
                    btn.classList.add("selected");
                    btn.disabled = true;
                }

                /*btn.addEventListener("click", (e) => await updatePaginatedUI(e, sectionTitle));*/

                container.appendChild(btn);
            }
        }

        await getPaginatedTickets(sprintId, sectionTitle, targetPage, numResults);

        //disable/enable left/right arrows 
        const leftBtn = document.getElementById(`btn-left-section${sectionTitle}`);
        const rightBtn = document.getElementById(`btn-right-section${sectionTitle}`);

        leftBtn.disabled = targetPage === 1;
        rightBtn.disabled = targetPage === numPages;

        toggleEditBtns();

        document.getElementById(`${sectionTitle}-container`).focus();
    }

    function handlePriorityLevel(priorityLevel) {
        switch (priorityLevel) {
            case 0: return "Low";
            case 1: return "Medium";
            case 2: return "High";
            default: return;
        }
    }

    function toggleEditBtns() {
        let editBtns = document.querySelectorAll('.edit-btn');
        const user = document.getElementById('hdnUser').value;

        for (const btn of editBtns) {
            if (btn.dataset.author !== user) {
                btn.style.display = 'none';
            }
        }
    }

export async function getTicketSettings() {
    const response = await getPartial("Tickets", "SettingsPartial");

    if (response.data) {
        document.getElementById("ticket-view").innerHTML = response.data;
    }

    await loadModule("setTicketSettings");
}

async function handleClick(e) {
    let partial = null;

    const btn = e.target.closest('button');
    const anchor = e.target.closest('a')

    if (btn && btn.id === 'add-btn') {
        const anonID = e.target.closest('.section-container').dataset.sectionid;
        const sectionID = getRealId(sectionMap, anonID);
        
        const params = {
            sectionId: sectionID,
            ticketId : null
        }
        await loadModule("manageTicket", params);
    }

    //else if (btn && btn.classList.contains('edit-btn')) {
    //    module = await loadModule("manageTicket");
    //    partial = await module.getManageTicketPartial(e.target.dataset.id, null);
    //}

    else if (anchor && anchor.classList.contains('goto-ticket')) {
        const container = e.target.closest('.section-container');
        const sectionId = getRealId(sectionMap, container.dataset.sectionid);

        const ticketId = getRealId(ticketMap, anchor.dataset.id);

        const params = {
            ticketId: ticketId,
            sectionId : sectionId
        }

        loadModule("ticket", params);
    }

    if (partial) {
        document.querySelector('.main-content').innerHTML = partial;
        return;
    }

    if (e.target.matches('.paginate')) {
        const sectionTitle = e.dataset.section;
        //if (e.target.id.includes('sel-take-section') && e.type == "change"){
        //    await updatePaginatedUI(e, sectionTitle);
        //}
        //else if (e.type == "click") {
        //    await updatePaginatedUI(e, sectionTitle);
        //}
    }

}

function handleChange(e) {
    const selectList = e.target.closest('#section-filter');
    
    if (!selectList)
        return;

    //const sectionToDisplay = e.target.innerText.toLowerCase().trim();
    const sections = document.querySelectorAll('.section-container');
    const selectedOption = e.target.value;

    //show all sections for all tab
    if (selectedOption == 0) {
        sections.forEach(s => s.classList.remove('hidden'));
    }
    //show individual section
    else {
        sections.forEach(s => {

            const realId = getRealId(sectionMap, s.dataset.sectionid);

            if (selectedOption != realId) {
                s.classList.add('hidden');
            }
            else {
                s.classList.remove('hidden');
            }
        });
    }
}

