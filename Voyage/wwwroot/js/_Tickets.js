import { loadModule } from './__moduleLoader.js';

export async function getManageTicketPartial(ticketId, sectionTitle) {
    try {
        const response = await axios.get('/Tickets/ManageTicketPartial', {
            params: { ticketId: ticketId }
        });

        document.getElementById("ticket-view").innerHTML = response.data;
        await loadModule("manageTicket");

        return true;
    } catch (error) {
        console.error("error: getManageTicketPartial", error);
        return false;
    }
}

export async function getTicketPartial(ticketId, ticketVersion) {
    try {
        const response = await axios.get('/Tickets/TicketPartial', {
            params: { ticketId: ticketId, ticketVersion: ticketVersion }
        });

        document.getElementById("ticket-view").innerHTML = response.data;
        await loadModule("ticket")
        return true;
    } catch (error) {
        console.error("error: getTicketPartial", error);
        return false;
    }
}

export async function getTicketsPartial() {
    try {
        const response = await axios.get('/Tickets/TicketsPartial');

        document.getElementById("ticket-view").innerHTML = response.data;
        await loadModule("tickets");
        return true;
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

            //event listeners
            document.querySelectorAll(".goto-ticket").forEach(btn =>
                btn.addEventListener("click", (e) => getTicketPartial(btn.dataset.id))
            );

            document.querySelectorAll(".btnAddTicket").forEach(btn =>
                btn.addEventListener("click", () => getManageTicketPartial(null, btn.dataset.section))
            );

            document.querySelectorAll("#btnEditTicket").forEach(btn =>
                btn.addEventListener("click", () => getManageTicketPartial(btn.dataset.id, null))
            );

        });

        toggleEditBtns();

        return true;
    } catch (error) {
        console.error("error: getPaginatedTickets", error);
        return false;
    }
}

function updatePaginatedUI(e, sectionTitle) {
    const sprintId = document.getElementById('hdnSprint').dataset.sprintid;

    const selectList = document.querySelector(`select.paginate[data-section="${sectionTitle}"]`);
    const numResults = parseInt(selectList.value);

    const pageBtn = document.querySelector(`.paginate.page.selected[data-section="${sectionTitle}"]`)
    const currentPage = parseInt(pageBtn.innerText);
    let targetPage = currentPage;
    
    const totalTicketCount = parseInt(document.getElementById(`hdnTotalTicketCount${sectionTitle}`).value);
    let numPages = Math.ceil(totalTicketCount / numResults);
    debugger;

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

            btn.addEventListener("click", (e) => updatePaginatedUI(e, sectionTitle));

            container.appendChild(btn);
        }
    }

    getPaginatedTickets(sprintId, sectionTitle, targetPage, numResults);

    //disable/enable left/right arrows 
    const leftBtn = document.getElementById(`btn-left-section${sectionTitle}`);
    const rightBtn = document.getElementById(`btn-right-section${sectionTitle}`);

    leftBtn.disabled = targetPage === 1;
    rightBtn.disabled = targetPage === numPages;

    toggleEditBtns();

    styleTable();
    
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

function styleTable(container = document) {
    // Style primary buttons
    container.querySelectorAll(".primary-btn").forEach(btn => {
        btn.style.padding = "4px 12px";
        btn.style.fontFamily = "-apple-system, BlinkMacSystemFont, 'Roboto', sans-serif";
        btn.style.borderRadius = "6px";
        btn.style.border = "none";
        btn.style.color = "#fff";
        btn.style.background = "#4B91F7";
        btn.style.backgroundOrigin = "border-box";
        btn.style.boxShadow = "0px 0.5px 1.5px rgba(54, 122, 246, 0.25), inset 0px 0.8px 0px -0.25px rgba(255, 255, 255, 0.2)";
        btn.style.webkitUserSelect = "none";
        btn.style.touchAction = "manipulation";
    });

    // Style secondary buttons
    container.querySelectorAll(".secondary-btn").forEach(btn => {
        btn.style.display = "flex";
        btn.style.flexDirection = "column";
        btn.style.alignItems = "center";
        btn.style.padding = "4px 12px";
        btn.style.fontFamily = "-apple-system, BlinkMacSystemFont, 'Roboto', sans-serif";
        btn.style.borderRadius = "6px";
        btn.style.color = "#3D3D3D";
        btn.style.background = "#fff";
        btn.style.border = "none";
        btn.style.boxShadow = "0px 0.5px 1px rgba(0, 0, 0, 0.5)";
        btn.style.userSelect = "none";
        btn.style.webkitUserSelect = "none";
        btn.style.touchAction = "manipulation";
    });

    // Style table rows (zebra stripes)
    container.querySelectorAll(".app-table-row").forEach((row, i) => {
        row.style.backgroundColor = i % 2 === 0 ? "#86b7fe17" : "#f8f9fa";
    });
}

async function getTicketSettings() {
    const response = await getPartial("Tickets", "SettingsPartial");

    if (response.data) {
        document.getElementById("ticket-view").innerHTML = response.data;
    }

    await loadModule("ticketSettings");
}

export function init() {
    document.querySelectorAll(".btnAddTicket").forEach(btn =>
        btn.addEventListener("click", () => getManageTicketPartial(null, btn.dataset.section))
    );

    document.querySelectorAll("#btnEditTicket").forEach(btn =>
        btn.addEventListener("click", () => getManageTicketPartial(btn.dataset.id, null))
    );

    document.querySelectorAll(".goto-ticket").forEach(btn =>
        btn.addEventListener("click", (e) => getTicketPartial(btn.dataset.id))
    );

    document.querySelectorAll(".paginate").forEach(el => {
        const sectionTitle = el.dataset.section;

        //add change event for select list
        if (el.id.includes(`sel-take-section`)) {
            el.addEventListener("change", (e) => updatePaginatedUI(e, sectionTitle));
        }
        //click events for buttons
        else {
            el.addEventListener("click", (e) => updatePaginatedUI(e, sectionTitle))
        }
    });

    document.querySelector(".settings-btn").addEventListener("click", async (e) => getTicketSettings());

}