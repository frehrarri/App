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

export async function getTicketPartial(ticketId) {
    try {
        const response = await axios.get('/Tickets/TicketPartial', {
            params: { ticketId: ticketId }
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

export function init() {
    document.querySelectorAll(".btnAddTicket").forEach(btn =>
        btn.addEventListener("click", () => {
            getManageTicketPartial(null, btn.dataset.section)
        }));

    document.querySelectorAll("#btnEditTicket").forEach(btn =>
        btn.addEventListener("click", () =>
            getManageTicketPartial(btn.dataset.id, null)
        ));

    document.querySelectorAll(".goto-ticket").forEach(btn =>
        btn.addEventListener("click", (e) => {
            getTicketPartial(btn.dataset.id);
        }
     ));
}