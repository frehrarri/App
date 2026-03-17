import { loadModule } from "/js/__moduleLoader.js";

const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
const preChangeValues = new Map();

export async function init(params) {
    removeEventListeners();

    const container = document.querySelector('.main-content');
    const partial = await getManageTicketPartial(params.ticketId);

    if (!container || !partial)
        return;

    container.innerHTML = partial;

    container.addEventListener("click", (e) => handleEvents(e));
    trackEventListener(container, "click", handleEvents);

    container.addEventListener("focusin", (e) => handlePreChange(e));
    trackEventListener(container, "focusin", handlePreChange);

    if (params?.sectionId) {
        const sectionDropdown = document.getElementById('ticketSectionTitle');
        if (sectionDropdown) {
            sectionDropdown.value = params.sectionId;
        }
    }

    const centerHead = document.getElementById('header-center');
    centerHead.innerHTML = "";

    updateBreadcrumb();

    const deleteBtn = document.getElementById('deleteTicket');
    const ticketId = parseInt(document.getElementById('hdnTicketId').value);
    if (ticketId == 0)
        deleteBtn.classList.add('hidden');

    //debounced search
    //let input = document.getElementById("ticketAssignedTo");
    //addUserSearchEventListener(input, "userResults");
}

async function handleEvents(e) {
    
    if (e.target.type != "button")
        return;

    if (e.target.id == "submitTicket") 
        await saveTicket(e);

    if (e.target.id == "deleteTicket")
        await deleteTicket(e);

    if (e.target.id == "undo-button") {
        undo();
    }
        

    //if (e.target.id == "ticketAssignedTo")
    //    assignedTo.value = "";

    //if (e.target.id == "ticketDesc")
    //    handleEnter(e);

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

async function saveTicket(e) {
    e.preventDefault();

    const saveBtn = document.getElementById('submitTicket');
    saveBtn.classList.add('disabled');

    const ticketDTO = {
        TicketId: parseInt(document.getElementById('ticketId').value) || 0,
        SectionTitle: document.getElementById('ticketSectionTitle').selectedOptions[0].text,
        sectionId: document.getElementById('ticketSectionTitle').value,
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

        saveBtn.classList.remove('disabled');

        const ticketId = parseInt(document.getElementById('hdnTicketId').value);

        //edit
        if (ticketId > 0) {
            await loadModule("ticket", ticketId);
        }
        //add new
        else {
            await loadModule("tickets");
        }

    } catch (error) {
        alert(`error: saveTicket`);
    }
}

async function deleteTicket(e) {
    e.preventDefault();

    const deleteBtn = document.getElementById('deleteTicket');
    deleteBtn.classList.add('disabled');

    let response;
    const ticketId = parseInt(document.getElementById('hdnTicketId').value);

    if (!confirm('Are you sure you want to delete this ticket?')) {
        return;
    }

    try {
        response = await axios.delete('/Tickets/DeleteTicket', {
            params: { ticketId: ticketId },
            headers: { 'X-CSRF-TOKEN': token }
        });

        if (response && response.status === 200) {
            alert("success");
            await loadModule("tickets");
        }
        else
            alert("error");

        deleteBtn.classList.remove('disabled');

    } catch (error) {
        alert("error: deleteTicket");
    }

}


function undo() {
    for (const [input, value] of preChangeValues.entries()) {
        if (input.type === "checkbox") {
            input.checked = value;
        } else if (input.isContentEditable) {
            input.innerHTML = value;
        } else {
            input.value = value;
        }
    }
    preChangeValues.clear(); //prevent dupes
}

function handlePreChange(e) {
    const el = e.target;
    if (el.tagName === "INPUT" || el.tagName === "TEXTAREA"
        || el.tagName === "SELECT" || e.target.matches("div[contenteditable='true']"))
    {

        if (el.type === "checkbox") {
            preChangeValues.set(el, el.checked);
        }
        else {
            preChangeValues.set(el, el.value);
        }

    }
    else if (el.isContentEditable) {
        preChangeValues.set(el, el.innerHTML);
    }
}

