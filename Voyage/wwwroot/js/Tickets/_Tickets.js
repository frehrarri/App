import { loadModule } from "/js/__moduleLoader.js";

const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
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
    container.addEventListener("keydown", handleEnter);
    trackEventListener(container, "click", handleClick);
    trackEventListener(container, "change", handleChange);
    trackEventListener(container, "keydown", handleEnter);

    //appendSectionFilters();
    updateBreadcrumb();
    updateNavHeader();

    anonymizeDataAttributes();

}

async function handleClick(e) {
    const btn = e.target.closest('button');
    const sectionTitle = btn?.dataset.section;

    const anchor = e.target.closest('a')
    const input = e.target.closest('input');

    //redirect to add ticket (manage ticket)
    if (btn && btn.classList.contains('add-btn')) {
        const anonID = e.target.closest('.section-container').dataset.sectionid;
        const sectionID = getRealId(sectionMap, anonID);

        const params = {
            sectionId: sectionID,
            ticketId: null
        }
        await loadModule("manageTicket", params);
    }

    //redirect to ticket
    else if (anchor && anchor.classList.contains('goto-ticket')) {
        const container = e.target.closest('.section-container');
        const sectionId = getRealId(sectionMap, container.dataset.sectionid);

        const ticketId = getRealId(ticketMap, anchor.dataset.id);

        const params = {
            ticketId: ticketId,
            sectionId: sectionId
        }

        loadModule("ticket", params);
    }

    if (btn && btn.matches('.paginate'))
        await updatePaginatedUI(e, sectionTitle);

    if (input && input.type == "checkbox")
        toggleCompletedDiscontinuedBtns(input);

    if (btn && btn.classList.contains('completed-btn'))
        await markCompleted(e);

    if (btn && btn.classList.contains('discontinue-btn'))
        await discontinue(e);
}

async function handleChange(e) {

    const select = e.target.closest('select.paginate');
    const sectionTitle = select?.dataset.section;

    if (select)
        await updatePaginatedUI(e, sectionTitle);

    toggleSections(e);
}

async function handleEnter(e) {
    if (e.key != 'Enter')
        return;

    const container = e.target.closest('.section-container');
    const input = container.querySelector('.paginate.page-input')
    const sectionTitle = container.id.split('-')[0].trim();

    if (input) {
        updatePaginatedUI(e, sectionTitle);
    }
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

        //skip encryption for already encrypted values
        if ([...sectionMap.values()].includes(realId))
            return;

        const anonId = getAnonymousId(sectionMap, realId);
        el.dataset.sectionid = anonId;
    });

    // anonymize tickets
    document.querySelectorAll('[data-id]').forEach(el => {
        const realId = el.dataset.id;

        //skip encryption for already encrypted values
        if ([...ticketMap.values()].includes(realId))
            return;

        const anonId = getAnonymousId(ticketMap, realId);
        el.dataset.id = anonId;
    });
}

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

        const container = document.querySelector(`#heading-${sectionTitle}`).closest('.section-container');
        const tableBody = container.querySelector('tbody');

        tableBody.innerHTML = ""; //empty current table body
        
        response.data.tickets.forEach(ticket => {
            buildTableRow(tableBody, ticket);
        });

        toggleEditBtns();

        return true;
    }
    catch (error) {
        console.error("error: getPaginatedTickets", error);
        return false;
    }
        
     
}

function buildTableRow(tableBody, ticket) {
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

    //append the row to the table body
    tableBody.appendChild(row);
}

function deleteTableRow(e) {
    const container = e.target.closest('.section-container');
    debugger;
    var rows = getCheckedRows(e)
    if (rows) {
        //decreaseRecordCount(e, rows);
        rows.forEach(r => {
            r.remove();
        });

        //if there are no remaining rows display no results message
        const remaining = container.querySelectorAll('.app-table-data').length;
        if (remaining == 0) {
            const tbody = container.querySelector('tbody');

            const tr = document.createElement('tr');
            tr.classList.add('app-table-row');

            const td = document.createElement('td');
            td.classList.add('no-results', 'app-table-data');
            td.setAttribute('colspan', '11');

            const span = document.createElement('span');
            span.innerText = 'No Results';

            td.append(span);
            tr.appendChild(td);
            tbody.appendChild(tr);
        }
     
    }

}

function updateRecordCount(e, pageNumber, pageSize) {
    const container = e.target.closest('.section-container');

    const low = container.querySelector('.record-low');
    const high = container.querySelector('.record-high');
    const total = container.querySelector('.record-total');


    const start = (pageNumber - 1) * pageSize + 1;
    low.innerText = start;

    const totalTicketCount = document.getElementById('hdnTotalTicketCount').value;
    total.innerText = totalTicketCount;

    const end = Math.min(pageNumber * pageSize, totalTicketCount);
    high.innerText = end;
}


async function updatePaginatedUI(e, sectionTitle) {
    e.preventDefault();

    let sprintId = parseInt(document.getElementById('hdnSprint').dataset.sprintid);
    if (!sprintId)
        return;

    let isManualInput = false;

    const container = e.target.closest('.section-container');
    const selectList = container.querySelector('.select-list');
    const numResults = parseInt(selectList.value);
    
    let currentPage = parseInt(container.querySelector('.paginate.page.selected').innerText);

    const totalTicketCount = parseInt(container.querySelector(`.hdn-total-tickets`).value);
    let numPages = Math.ceil(totalTicketCount / numResults);

    const selectedBtn = e.target.closest(".paginate");
    let selectedPage = parseInt(e.target.dataset.page);

    //left arrow button
    if (selectedBtn.id == `btn-left-section-${sectionTitle}`) {
        selectedBtn.classList.remove("selected");
        selectedBtn.disabled = false;

        currentPage = currentPage - 1;
        handleArrowButtons(e, currentPage, selectedBtn, numPages);
    }
    //right arrow button
    else if (selectedBtn.id == `btn-right-section-${sectionTitle}`) {
        selectedBtn.classList.remove("selected");
        selectedBtn.disabled = false;

        currentPage = currentPage + 1;
        handleArrowButtons(e, currentPage, selectedBtn, numPages);
    }

    //specific page button
    if (selectedBtn.classList.contains('page')) {
        handlePageBtns(e, currentPage, selectedBtn, numPages);
    }

    //manual page input
    if (e.target.classList.contains('page-input')) {
        isManualInput = true;

        selectedPage = parseInt(e.target.value);

        if (selectedPage > numPages)
            selectedPage = numPages;
        else if (selectedPage < 1)
            selectedPage = 1;
        
        handleManualPageInput(container, selectedPage, numPages);
    }

    //update num of pages when using result count drop down
    if (e.target.classList.contains('select-list')) {
        handleResultAmountDropDown(container, numResults, totalTicketCount);
    }

    selectedPage = updateManualPageInput(container, selectedPage, currentPage);

    await getPaginatedTickets(sprintId, sectionTitle, selectedPage, numResults);

    //disable/enable left/right arrows 
    const leftBtn = document.getElementById(`btn-left-section-${sectionTitle}`);
    const rightBtn = document.getElementById(`btn-right-section-${sectionTitle}`);

    //set arrows for non manual input
    if (!isManualInput) {
        leftBtn.disabled = selectedPage === 1;
        rightBtn.disabled = selectedPage === numPages;
    }

    toggleEditBtns();

    updateRecordCount(e, currentPage, numResults, true);

    anonymizeDataAttributes();
}

function handlePageBtns(e, currentPage, pageBtn, numPages) {
    
    currentPage = parseInt(e.target.dataset.page);

    //remove selected/disabled from previous page button
    pageBtn.classList.remove("selected");
    pageBtn.disabled = false;

    //update selected/disabled to the target page button
    e.target.classList.add("selected");
    e.target.disabled = true;

    //handle less than 5 pages - no centering needed for selection
    if (numPages < 5)
        handlePageButtons(e, numPages, pageBtn);

    //only center if there are 2 pages on each side and at least 5 pages total
    if (numPages >= 5 && currentPage - 2 >= 1 && currentPage + 2 <= numPages)
        formatPageButtons(e, currentPage, numPages);

    //handle starting 5 pages
    const startDiff = currentPage - 1;
    if (startDiff == 1 || startDiff == 0) 
        formatPageButtons(e, 3, numPages);
    
    //handle final 5 pages
    const finalDiff = numPages - currentPage;
    if (finalDiff == 1 || finalDiff == 0) 
        formatPageButtons(e, Math.max(3, numPages - 2), numPages)
}

function handlePageButtons(e, numPages, pageBtn) {
    const container = e.target.closest('.section-container');
    let selectedPage = null;

    debugger;
    if (!pageBtn || numPages >=5)
        return;

    if (pageBtn.classList.contains('.page')) {
        selectedPage = parseInt(pageBtn.dataset.page);
    }
    else {
        //selectedPage = parseInt()
    }

    //set button and right arrow to selected disabled
    if (selectedPage >= numPages) {
        //const rightArrow = container.querySelector('.paginate.paginate-arrow.right');
        //rightArrow.disabled = true;

        //pageBtn.
    }
    //set button and left arrow to selected disabled
    else if (selectedPage <= 1) {
        //const leftArrow = container.querySelector('.paginate.paginate-arrow.left');
        //leftArrow.disabled = true;


    }
    //set selected only
    else {

    }

    
}

function formatPageButtons(e, anchor, numPages) {
    const container = e.target.closest('.section-container');

    const clickedPage = parseInt(e.target.dataset.page);
    const selectedPage = anchor ?? clickedPage;

    const pageBtns = Array.from(e.target.closest('.page-btn-container').children)
        .filter(el => el.tagName === 'BUTTON');

    for (let i = 0; i < pageBtns.length; i++) {
        const pageNum = Math.max(1, selectedPage - 2 + i); // never go below 1
        pageBtns[i].dataset.page = pageNum;
        pageBtns[i].innerText = pageNum;
        pageBtns[i].disabled = false;
        pageBtns[i].classList.remove('selected');

        if (pageNum === clickedPage) {
            pageBtns[i].disabled = true;
            pageBtns[i].classList.add('selected');
        }

        applyEllipsis(container, clickedPage, numPages)

    }
}

function handleArrowButtons(e, currentPage, pageBtn, numPages) {
    const sectionContainer = e.target.closest('.section-container');
    const pageBtnContainer = sectionContainer.querySelector('.page-btn-container');
    debugger;

    const pageBtns = Array.from(pageBtnContainer.closest('.page-btn-container').children)
        .filter(el => el.tagName === 'BUTTON');

    // deselect previous
    pageBtn.classList.remove("selected");
    pageBtn.disabled = false;

    const startDiff = currentPage - 1;
    const finalDiff = numPages - currentPage;
    let selectedPage = null;

    //handle less than 5 pages - no centering needed for selection
    if (numPages < 5)
        handlePageButtons(e, numPages, pageBtn);
    else
    {
        //handle starting pages
        if (startDiff <= 1)
            selectedPage = 3;

        //handle final pages
        else if (finalDiff <= 1)
            selectedPage = numPages - 2;

        //only center if there are 2 pages on each side and at least 5 pages total
        else
            selectedPage = currentPage;

        // reformat buttons
        for (let i = 0; i < pageBtns.length; i++) {
            const pageNum = selectedPage - 2 + i;
            pageBtns[i].dataset.page = pageNum;
            pageBtns[i].innerText = pageNum;
            pageBtns[i].disabled = false;
            pageBtns[i].classList.remove('selected');

            //disable selected page button
            if (pageNum === currentPage) {
                pageBtns[i].disabled = true;
                pageBtns[i].classList.add('selected');
            }

            applyEllipsis(sectionContainer, selectedPage, numPages);

        }
    }

    
}

function handleManualPageInput(container, selectedPage, numPages) {

    //enable arrows
    container.querySelector('.paginate.paginate-arrow.left').disabled = false;
    container.querySelector('.paginate.paginate-arrow.right').disabled = false;

    const pageBtns = container.querySelectorAll('.paginate.page');
    let anchor = selectedPage;

    // clamp anchor so buttons don't go below 1 or above numPages
    if (selectedPage - 2 < 1) {
        anchor = 3;
    } else if (selectedPage + 2 > numPages) {
        anchor = Math.max(3, numPages - 2);
    }

    for (let i = 0; i < pageBtns.length; i++) {
        const pageNum = anchor - 2 + i;
        pageBtns[i].dataset.page = pageNum;
        pageBtns[i].innerText = pageNum;
        pageBtns[i].disabled = false;
        pageBtns[i].classList.remove('selected');

        //disable selected button page
        if (pageNum === selectedPage) {
            pageBtns[i].disabled = true;
            pageBtns[i].classList.add('selected');
        }

        //disable arrows
        if (i == 0 && selectedPage <= 1) {
            container.querySelector('.paginate.paginate-arrow.left').disabled = true;
        }
        else if (i == 4 && selectedPage >= numPages) {
            container.querySelector('.paginate.paginate-arrow.right').disabled = true;
        }
    }

    applyEllipsis(container, selectedPage, numPages)
    debugger;
    container.querySelector('.paginate.page-input').value = selectedPage;
}


function applyEllipsis(container, clickedPage, numPages) {
    //hide ellipsis by default
    container.querySelector('.ellipsis-high').classList.add('hidden');
    container.querySelector('.ellipsis-low').classList.add('hidden');

    //appy ellipsis left
    if (numPages >= 6 && clickedPage >= 4) {
        container.querySelector('.ellipsis-low').classList.remove('hidden');
    }

    //apply ellipsis right
    const diff = numPages - clickedPage;
    if (numPages >= 6 && diff >= 3)
        container.querySelector('.ellipsis-high').classList.remove('hidden');
}

function updateManualPageInput(container, selectedPage, currentPage) {
    //update manual page selector
    const btnContainer = container.querySelector('.pagination-buttons');
    const input = btnContainer.querySelector('.page-input');

    if (!selectedPage) {
        selectedPage = currentPage;
    }

    input.value = selectedPage;

    return selectedPage;
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




function toggleCompletedDiscontinuedBtns(input) {

    const container = input.closest('.section-container');
    const checkedBoxes = container.querySelectorAll('input[type="checkbox"]:checked');

    const completedBtn = container.querySelector('.completed-btn');
    const discontinueBtn = container.querySelector('.discontinue-btn');

    //there are no checked boxes - disable the input
    if (checkedBoxes.length == 0) {
        completedBtn.disabled = true;
        completedBtn.classList.add('disabled');

        discontinueBtn.disabled = true;
        discontinueBtn.classList.add('disabled');
        return;
    }

    //display input because there is 1 or more checked boxes
    completedBtn.disabled = false;
    completedBtn.classList.remove('disabled');
    
    discontinueBtn.disabled = false;
    discontinueBtn.classList.remove('disabled');
}



function handleResultAmountDropDown(container, numResults, totalTicketCount) {
    debugger;
    //start at first page on update
    const currentPage = 1;

    const numPages = Math.ceil(totalTicketCount / numResults);

    //empty container of old button layout
    const pageBtnContainer = container.querySelector('.page-btn-container');
    pageBtnContainer.innerHTML = "";

    const sectionTitle = container.id.split('-')[0].trim();

    //create new buttons
    for (let i = 1; i <= Math.min(numPages, 5); i++) {
        const btn = document.createElement("button");
        btn.classList.add("paginate", "page");
        btn.dataset.section = sectionTitle;
        btn.dataset.page = i;
        btn.innerText = i;

        if (i === currentPage) {
            btn.classList.add("selected");
            btn.disabled = true;
        }

        pageBtnContainer.appendChild(btn);
    }
}

function toggleSections(e) {
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

function getCheckedBoxesTicketIds(e){
    const list = [];

    const container = e.target.closest('.section-container');
    const checkboxes = container.querySelectorAll('input[type="checkbox"]');

    for (let i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked) {

            const checkbox = checkboxes[i];
            const row = checkbox.closest('.app-table-row');
            const anonId = row.children[1].querySelector('.goto-ticket').dataset.id;

            const id = getRealId(ticketMap, anonId);

            const dto = {
                ticketId: parseInt(id),
            }

            list.push(dto);
        }
    }

    return list;
}

function getCheckedRows(e) {
    const rows = [];
    const container = e.target.closest('.section-container');
    const checkboxes = container.querySelectorAll('input[type="checkbox"]');

    for (let i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked) {

            const checkbox = checkboxes[i];
            const row = checkbox.closest('.app-table-row');

            rows.push(row);
        }
    }

    return rows;
}



function getCheckedRowData(e) {
    const tickets = [];
    
    const rows = [];
    const container = e.target.closest('.section-container');
    const checkboxes = container.querySelectorAll('input[type="checkbox"]');

    for (let i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked) {

            const checkbox = checkboxes[i];
            const row = checkbox.closest('.app-table-row');

            rows.push(row);
        }
    }

    const tds = rows[0].children;

    let id = tds[1].querySelector('.goto-ticket').dataset.id;
    id = getRealId(ticketMap, id);

    const ticket = {
        ticketId: parseInt(id),
        title: tds[2].innerText.trim(),
        status: tds[3].innerText.trim(),
        assignedTo: tds[4].innerText.trim(),
        dueDate: tds[5].innerText.trim(),
        priorityLevel: tds[6].innerText.trim(),
        modifiedDate: tds[7].innerText.trim(),
        modifiedBy: tds[8].innerText.trim(),
        createdDate: tds[9].innerText.trim(),
        createdBy: tds[10].innerText.trim()
    }

    tickets.push(ticket);

    return tickets;
}

async function markCompleted(e) {
    try {
        const tickets = getCheckedRowData(e);
        const list = getCheckedBoxesTicketIds(e);

        const response = await axios.post('/Tickets/MarkCompleted', list, {
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            }
        });

        if (response && response.status === 200) {
            alert('Success');

            deleteTableRow(e);

            //add rows to new container
            const container = document.querySelector('#Completed-container');
            const tbody = container.querySelector('tbody');

            increaseRecordCount(container, tickets);

            tickets.forEach(ticket => {
                buildTableRow(tbody, ticket);
            });

        }
    } catch (error) {
        console.error("error: markCompleted", error);
        return false;
    }
}

async function discontinue(e) {
    try {
        const tickets = getCheckedRowData(e);
        const list = getCheckedBoxesTicketIds(e);
        
        const response = await axios.post('/Tickets/Discontinue', list, {
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            }
        });

        
        if (response && response.status === 200) {
            alert('Success');

            deleteTableRow(e);

            //add rows to new container
            const container = document.querySelector('#Discontinued-container');
            const tbody = container.querySelector('tbody');

            increaseRecordCount(container, tickets);

            tickets.forEach(ticket => {
                buildTableRow(tbody, ticket);
            });
        }
    }
    catch (error) {
        console.error("error: discontinue", error);
        return false;
    }
}

function decreaseRecordCount(e, rows) {
    const container = e.target.closest('.section-container');
    const numOfRowsRemoved = rows.length;

    const high = container.querySelector('.record-high');
    let value = parseInt(high.innerText) - numOfRowsRemoved;
    high.innerText = `${value}`;

    const total = container.querySelector('.record-total');
    value = parseInt(total.innerText) - numOfRowsRemoved;
    total.innerText = `${value}`;

    const low = container.querySelector('record-low')
    value = parseInt(low.innerText) - numOfRowsRemoved;

    //must have at least 1 record
    //must only remove 1 or 0 records
    if (low.innerText < 1
        && (value == 0 || value == 1))
    {
        low.innerText = `${value}`;
    }
}

function increaseRecordCount(container, rows) {
    const recordLimit = container.querySelector('select').value;
    const numExistingRecords = container.querySelectorAll('tr').length;

    //if the number of records is under our limit we may increment
    rows.forEach(r => {
        if (numExistingRecords <= recordLimit) {
            const high = container.querySelector('.record-high');
            let value = parseInt(high.innerText) + 1;
            high.innerText = `${value}`;
        }
    });
    
    //always increment the total
    const total = container.querySelector('.record-total');
    let value = parseInt(total.innerText) + rows.length;
    total.innerText = `${value}`;
}