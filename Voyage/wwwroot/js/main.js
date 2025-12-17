const moduleRegistry = {
    sideNav: () => import("./_SideNav.js"),
    manageTicket: () => import("./_ManageTicket.js"),
    tickets: () => import("./_Tickets.js"),
    ticket: () => import("./_Ticket.js")
};

export async function loadModule(name, params) {
    if (!moduleRegistry[name]) {
        console.warn(`Module '${name}' not registered`);
        return;
    }

    const module = await moduleRegistry[name]();

    if (typeof module.init === "function") {
        module.init(params);
    }
}

// Initialize side nav when the page loads
document.addEventListener("DOMContentLoaded", async () => {
    await loadModule("sideNav");
});
