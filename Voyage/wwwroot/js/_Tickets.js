import { loadModule } from "./main.js";

export async function getManageTicketPartial(ticketId, sectionTitle) {
    try {
        const response = await axios.get('/Tickets/ManageTicketPartial', {
            params: { ticketId: ticketId }
        });

        document.getElementById("ticket-view").innerHTML = response.data;
        await loadModule("manageTicket", { ticketId, sectionTitle });
        return true;
    } catch (error) {
        console.error("error", error);
        return false;
    }
}

export async function getTicketPartial(ticketId, ticketVersion) {
    try {
        const response = await axios.get('/Tickets/TicketPartial', {
            params: { ticketId: ticketId, ticketVersion: ticketVersion }
        });

        document.getElementById("ticket-view").innerHTML = response.data;
        await loadModule("ticket", { ticketId });
        return true;
    } catch (error) {
        console.error("error", error);
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
        console.error("error", error);
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
            td12.innerHTML = `<button id="btnViewTicket" class="goto-ticket" data-id="${ticket.ticketId}">View</button>`;
            row.appendChild(td12);

            //append the row to the table body
            tableBody.appendChild(row);
        });

        return true;
    } catch (error) {
        console.error("error", error);
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

    //left arrow button
    if (e.target.id == `btn-left-section${sectionTitle}`) {
        targetPage = currentPage - 1;
        pageBtn.classList.remove("selected");
        pageBtn.disabled = false;

        const selectedPageButton = document.querySelector(`.paginate.page[data-section="${sectionTitle}"][data-page="${targetPage}"]`);
        selectedPageButton.classList.add("selected");
        selectedPageButton.disabled = true;
    }
    //right arrow button
    if (e.target.id == `btn-right-section${sectionTitle}`) {
        targetPage = currentPage + 1;
        pageBtn.classList.remove("selected");
        pageBtn.disabled = false;

        const selectedPageButton = document.querySelector(`.paginate.page[data-section="${sectionTitle}"][data-page="${targetPage}"]`);
        selectedPageButton.classList.add("selected");
        selectedPageButton.disabled = true;
    }

    //specific page button
    if (e.target.classList.contains('page')) {
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
}

function handlePriorityLevel(priorityLevel) {
    switch (priorityLevel) {
        case 0: return "Low";
        case 1: return "Medium";
        case 2: return "High";
        default: break;
    }
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
            /*const preChangeResultNum = parseInt(document.querySelector(`select.paginate[data-section="${sectionTitle}"]`).value);*/
            el.addEventListener("change", (e) => updatePaginatedUI(e, sectionTitle));
        }
        //click events for buttons
        else {
            el.addEventListener("click", (e) => updatePaginatedUI(e, sectionTitle))
        }
        
    });
}