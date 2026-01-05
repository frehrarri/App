const version = new Date().getTime(); //append version to avoid caching issues

//when a partial view is rendered loadModule is called
//if the module name is in the module registry it loads the external.js script
const moduleRegistry = {
    sideNav: () => import(`./_SideNav.js?v=${version}`),
    manageTicket: () => import(`./_ManageTicket.js?v=${version}`),
    tickets: () => import(`./_Tickets.js?v=${version}`),
    ticket: () => import(`./_Ticket.js?v=${version}`)
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

//Initialize side nav when the page loads
document.addEventListener("DOMContentLoaded", async () => {
    await loadModule("sideNav");
});
