import { loadModule } from "/js/__moduleLoader.js";

export async function init(params) {
    removeEventListeners();

    let preChangeValues = new Map();

    const container = document.querySelector('.main-content');
    const partial = await getManageTicketPartial(params.ticketId);

    if (!container || !partial)
        return;

    container.innerHTML = partial;

    container.addEventListener("click", (e) => handleEvents(e, preChangeValues));
    trackEventListener(container, "click", handleEvents);

    debugger;
    if (params?.sectionId) {
        const sectionDropdown = document.getElementById('ticketSectionTitle');
        if (sectionDropdown) {
            sectionDropdown.value = params.sectionId;
        }
    }

    const centerHead = document.getElementById('header-center');
    centerHead.innerHTML = "";

    updateBreadcrumb();
    //updateNavHeader(); //need to get verb to implement

    //debounced search
    //let input = document.getElementById("ticketAssignedTo");
    //addUserSearchEventListener(input, "userResults");
}

async function handleEvents(e, preChangeValues) {
    debugger;
    if (e.target.type != "button")
        return;

    if (e.target.id == "submitTicket") 
        await saveTicket();

    if (e.target.id == "deleteTicket")
        await deleteTicket();

    if (e.target.id == "undo-button")
        undo(preChangeValues);

    //if (e.target.id == "ticketAssignedTo")
    //    assignedTo.value = "";

    //if (e.target.id == "ticketDesc")
    //    handleEnter(e);

    handleUndoMap(e, preChangeValues);
}

export async function getManageTicketPartial(ticketId) {
    try {
        const response = await axios.get('/Tickets/ManageTicketPartial', {
            params: { ticketId: ticketId }
        });
        return response.data;
    } catch (error) {
        console.error("error: getManageTicketPartial", error);
        return false;
    }
}

function updateNavHeader() {
    const page = document.getElementById('dv-navbar-page-title');
    const title = document.querySelector('h5').innerText;
    page.innerText = title;
}

function updateBreadcrumb() {
    const ol = document.querySelector('.breadcrumb');

    ol.innerHTML = '';

    const li1 = document.createElement('li');
    li1.classList.add('breadcrumb-item');
    li1.classList.add('active')

    const a1 = document.createElement('a');
    a1.href = "#";
    a1.textContent = 'Tickets'

    const loadTickets = async () => {
        await loadModule('tickets');
    };

    a1.addEventListener("click", loadTickets);
    trackEventListener(a1, "click", loadTickets);

    li1.appendChild(a1);
    ol.appendChild(li1);
    
    const isEdit = document.getElementById('hdnTicketId')?.value > 0 ?? false;

    if (isEdit) {
        const li2 = document.createElement('li');
        li2.classList.add('breadcrumb-item');
        li2.classList.add('active');

        const a2 = document.createElement('a');
        a2.innerText = 'Ticket';

        const gotoTicket = async () => {
            const ticketId = document.getElementById('hdnTicketId').value;
            await loadModule('ticket', parseInt(ticketId));
        };

        a2.addEventListener("click", gotoTicket);
        trackEventListener(a2, "click", gotoTicket);

        li2.appendChild(a2);
        ol.appendChild(li2);

        const li3 = document.createElement('li');
        li3.classList.add('breadcrumb-item');
        li3.classList.add('active');
        li3.textContent = 'Edit Ticket';

        ol.appendChild(li3);
    }
    else {
        const li2 = document.createElement('li');
        li2.classList.add('breadcrumb-item');
        li2.classList.add('active');
        li2.textContent = 'Add Ticket';

        ol.appendChild(li2);
    }
}

async function saveTicket() {
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    const ticketDTO = {
        TicketId: parseInt(document.getElementById('ticketId').value) || 0,
        SectionTitle: document.getElementById('ticketSectionTitle').value,
        Title: document.getElementById('ticketTitle').value,
        Description: document.getElementById('ticketDesc').innerHTML,
        Status: document.getElementById('ticketStatus').value,
        AssignedTo: document.getElementById('ticketAssignedTo').value,
        PriorityLevel: parseInt(document.getElementById('ticketPrioLvl').value),
        DueDate: document.getElementById('ticketDueDate').value ? new Date(document.getElementById('ticketDueDate').value).toISOString() : null,
        ParentTicketId: document.getElementById('ticketParent').value ? parseInt(document.getElementById('ticketParent').value) : null
    }
    let response;
    
    try {
        response = await axios.post('/Tickets/SaveTicket', ticketDTO, {
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            }
        });

        if (response && response.status === 200)
            alert("success");
        else
            alert("error");

        /*let isEdit = document.getElementsByTagName('h1')[0].textContent.toLowerCase().includes("edit");*/

        const isEdit = document.getElementById('ticketId').innerText.trim();

        // Go back to tickets list after successful save
        if (isEdit) {
            let ticketId = parseInt(document.getElementById('ticketId').value);

            await loadModule("ticket", ticketId);
        }
        else {
            await loadModule("tickets");
        }

    } catch (error) {
        alert(`error: saveTicket`);
    }

    return response.data; // bool
}

async function deleteTicket() {
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    let response;
    const ticketId = document.getElementById('ticketId').value;

    if (!confirm('Are you sure you want to delete this ticket?')) {
        return;
    }

    try {
        response = await axios.delete('/Tickets/DeleteTicket', {
            params: { ticketId: ticketId },
            headers: { 'X-CSRF-TOKEN': token }
        });

        showSuccess(true);

        const module = await loadModule("tickets");
        await module.getTicketsPartial();

        return response.data;

    } catch (error) {
        showSuccess(false);
        console.error("error", error);
        return false;
    }

    return response.data; // bool
}




function undo(preChangeValues) {
    for (const [input, value] of preChangeValues.entries()) {
        if (input.type === "checkbox" || input.type === "radio") {
            input.checked = value;
        } else if (input.isContentEditable) {
            input.innerText = value;
        } else {
            input.value = value;
        }
    }
}


function handleUndoMap(e, preChangeValues) {

    if (e.target.tagName == "INPUT" || e.target.tagName == "TEXTAREA"
        || e.target.tagName == "SELECT" || e.target.matches("div[contenteditable='true']"))
    {
        if (e.target.type === "checkbox" /*|| input.target.type === "radio"*/) {
            preChangeValues.set(e.target, e.target.checked);
        } else if (e.target.matches("div[contenteditable='true']")) {
            preChangeValues.set(e.target, e.target.innerText);
        } else {
            preChangeValues.set(e.target, e.target.value);
        }
    }

    //for (const input of form.querySelectorAll("input, textarea, select, div[contenteditable='true']")) {

    //    if (input.type === "checkbox" || input.type === "radio") {
    //        preChangeValues.set(input, input.checked);
    //    } else if (input.isContentEditable) {
    //        preChangeValues.set(input, input.innerText);
    //    } else {
    //        preChangeValues.set(input, input.value);
    //    }
    //}
}



