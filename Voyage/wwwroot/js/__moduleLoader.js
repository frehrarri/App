const scriptVersion = new Date().getTime(); //append version to avoid caching issues

//when a partial view is rendered loadModule is called
//if the module name is in the module registry it loads the external.js script
const moduleRegistry = {
    sideNav: () => import(`./_sidenav.js?v=${scriptVersion}`),
    manageTicket: () => import(`./_manageTicket.js?v=${scriptVersion}`),
    tickets: () => import(`./_tickets.js?v=${scriptVersion}`),
    ticket: () => import(`./_ticket.js?v=${scriptVersion}`)
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

    return module;
}