import { loadModule } from "/js/__moduleLoader.js";

export function init() {
    document.querySelector('.settings-btn')?.addEventListener("click", async () => getTicketSettings());
}

export async function getTicketSettings() {
    const response = await getPartial("Tickets", "SettingsPartial");

    if (response.data) {
        document.getElementById("ticket-view").innerHTML = response.data;
    }

    await loadModule("ticketSettings");
}
