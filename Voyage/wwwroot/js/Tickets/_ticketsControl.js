import { loadModule } from "/js/__moduleLoader.js";

export async function init() {

    //add ticket control partial to view
    let partial = await getTicketsControlPartial();
    document.getElementById("ticket-view").innerHTML = partial;

    //add event handlers
    const container = document.getElementById('tickets-control-container');
    container.addEventListener("click", handleEvents);

    //load default module
    const module = await loadModule("tickets");
}

export async function getTicketsControlPartial() {
    const response = await getPartial("Tickets", "TicketsControlPartial");
    return response.data;
}

async function handleEvents(e) {
    e.preventDefault();
    e.stopPropagation();

    await handleSettings(e);

}

async function handleSettings(e) {
    let response;
    const companyId = parseInt(document.getElementById('hdnCompanyId').value);

    if (e.target.closest(".settings-btn")) {
        response = await axios.get('/Tickets/SettingsPartial', {
            params: { companyId: companyId }
        });

        document.getElementById("tickets-partial-container").innerHTML = response.data;
        await loadModule("ticketSettings");
    }
}