import { getTicketsPartial } from "./_Tickets.js";

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
    document.getElementById("cancel")?.addEventListener("click", () => getTicketsPartial());
}

async function saveTicket() {
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    const ticket = {
        TicketId: parseInt(document.getElementById('ticketId').value) || 0,
        SectionTitle: document.getElementById('ticketSectionTitle').value,
        Title: document.getElementById('ticketTitle').value,
        Description: document.getElementById('ticketDesc').value,
        Status: document.getElementById('ticketStatus').value,
        AssignedTo: document.getElementById('ticketAssignedTo').value,
        PriorityLevel: document.getElementById('ticketPrioLvl').value,
        DueDate: document.getElementById('ticketDueDate').value ? new Date(document.getElementById('ticketDueDate').value).toISOString() : null,
        ParentTicketId: document.getElementById('ticketParent').value ? parseInt(document.getElementById('ticketParent').value) : null
    }
    let response;
    
    try {
        response = await axios.post('Tickets/SaveTicket', ticket, {
            headers: { 'X-CSRF-TOKEN': token },
            'Content-Type': 'application/json'
        });
        showSuccess(true);
        // Go back to tickets list after successful save
        setTimeout(async () => {
            await getTicketsPartial();
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
        await getTicketsPartial();
        return response.data;

    } catch (error) {
        showSuccess(false);
        console.error("error", error);
        return false;
    }

    return response.data; // bool
}