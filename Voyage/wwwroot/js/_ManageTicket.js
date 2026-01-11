import { loadModule } from './__moduleLoader.js';


export function init(params) {

    //set section dropdown based on which table the user clicks the add button for
    if (params?.sectionTitle) {
        const sectionDropdown = document.getElementById('ticketSectionTitle');
        if (sectionDropdown) {
            sectionDropdown.value = params.sectionTitle;
        }
    }

    document.getElementById("submitTicket")?.addEventListener("click", saveTicket);
    document.getElementById("deleteTicket")?.addEventListener("click", deleteTicket);
    document.getElementById("undo-button")?.addEventListener("click", undo);
    document.getElementById("cancel")?.addEventListener("click", cancel);

    //content editable div
    const description = document.getElementById('ticketDesc');
    if (description) {
        description.addEventListener('keydown', handleEnter);
    }

    const assignedTo = document.getElementById('ticketAssignedTo');
    assignedTo.addEventListener("click", (e) => assignedTo.value = "");

    //undo
    addUndoEventListeners();

    //debounced search
    addUserSearchEventListener("ticketAssignedTo", "userResults");
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

        let isEdit = document.getElementsByTagName('h1')[0].textContent.toLowerCase().includes("edit");

        showSuccess(true);
        // Go back to tickets list after successful save
        setTimeout(async () => {

            const module = await loadModule("tickets");

            if (isEdit) {
                let ticketId = parseInt(document.getElementById('ticketId').value);

                await module.getTicketPartial(ticketId);
            }
            else {
                await module.getTicketsPartial();
            }
            
        }, 1500);

        return response.data;
    } catch (error) {
        showSuccess(false);
        console.error("error", error);
        return false;
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


let preChangeValues = new Map();

function undo() {
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

function addUndoEventListeners() {
    const form = document.getElementById('ticket-form');
    if (form) {

        for (const input of form.querySelectorAll("input, textarea, select, div[contenteditable='true']")) {

            if (input.type === "checkbox" || input.type === "radio") {
                preChangeValues.set(input, input.checked);
            } else if (input.isContentEditable) {
                preChangeValues.set(input, input.innerText);
            } else {
                preChangeValues.set(input, input.value);
            }
        }
    }
}

async function cancel() {
    const module = await loadModule("tickets");

    let isEdit = document.getElementsByTagName('h1')[0].textContent.toLowerCase().includes("edit");

    if (isEdit) {
        let ticketId = parseInt(document.getElementById('ticketId').value);

        await module.getTicketPartial(ticketId);
    }
    else {
        await module.getTicketsPartial();
    }
}


