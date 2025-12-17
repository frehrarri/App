import { loadModule } from "./main.js";

async function getManageTicketPartial(ticketId, sectionTitle) {
    try {
        const response = await axios.get(`/Tickets/ManageTicketPartial`, {
            params: { ticketId: ticketId }
        });

        document.getElementById("ticket-view").innerHTML = response.data;
        loadModule("manageTicket", { ticketId, sectionTitle });
        return true;
    } catch (error) {
        console.error("error", error);
        return false;
    }
}

export async function getTicketPartial() {
    try {
        const response = await axios.get(`/Tickets/TicketPartial`);
        document.getElementById("ticket-view").innerHTML = response.data;
        loadModule("ticket");
        return true;
    } catch (error) {
        console.error("error", error);
        return false;
    }
}

export async function getTicketsPartial() {
    try {
        const response = await axios.get(`/Tickets/TicketsPartial`);
        document.getElementById("ticket-view").innerHTML = response.data;
        loadModule("tickets");
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
            debugger;
            getTicketPartial()
        }
        ));
}