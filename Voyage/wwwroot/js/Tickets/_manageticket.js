import { loadModule } from "/js/__moduleLoader.js";


export function init() {

    //set section dropdown based on which table the user clicks the add button for
    //if (params?.sectionTitle) {
    //    const sectionDropdown = document.getElementById('ticketSectionTitle');
    //    if (sectionDropdown) {
    //        sectionDropdown.value = params.sectionTitle;
    //    }
    //}
    debugger;
    const container = document.getElementById('tickets-partial-container');
    container.addEventListener("click", handleEvents);

    //debounced search
    addUserSearchEventListener("ticketAssignedTo", "userResults");
}

async function handleEvents(e) {
    debugger;
    if (e.target.id == "submitTicket")
        await saveTicket();

    if (e.target.id == "deleteTicket")
        await deleteTicket();

    debugger;

    if (e.target.id == "undo-button")
        undo();

    if (e.target.id == "btn-back")
        await back();

    if (e.target.id == "ticketAssignedTo")
        assignedTo.value = "";

    if (e.target.id == "ticketDesc")
        handleEnter(e);

    handleUndoMap(e);
}

export async function getManageTicketPartial(ticketId, sectionTitle) {
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


function handleUndoMap(e) {

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


async function back() {
    await loadModule("ticketsControl")
}


