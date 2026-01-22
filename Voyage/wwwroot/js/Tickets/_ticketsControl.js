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
    removeSettingsBtn(e);    
}

function removeSettingsBtn(e) {
    const btn = e.target.closest(".settings-btn");
    if (btn) {
        const container = btn.parentNode;

        if (container) {
            container.classList.add("hidden");
            loadModule("ticketSettings");
        }
    }
}