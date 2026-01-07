const version = new Date().getTime(); //append version to avoid caching issues

//when a partial view is rendered loadModule is called
//if the module name is in the module registry it loads the external.js script
const moduleRegistry = {
    sideNav: () => import(`./_sidenav.js?v=${version}`),
    manageTicket: () => loadModuleWithCSS('_manageticket', version),
    tickets: () => import(`./_tickets.js?v=${version}`),
    ticket: () => import(`./_ticket.js?v=${version}`)
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

async function loadModuleWithCSS(moduleName, ver) {
    // Load CSS if not already loaded
    const cssId = `${moduleName}-css`;
    if (!document.getElementById(cssId)) {
        const link = document.createElement('link');
        link.id = cssId;
        link.rel = 'stylesheet';
        link.href = `/css/_${moduleName.toLowerCase()}.css?v=${ver}`;
        document.head.appendChild(link);
    }

    // Load the JS module
    return import(`./${moduleName}.js?v=${ver}`);
}

//Initialize side nav when the page loads
document.addEventListener("DOMContentLoaded", async () => {
    await loadModule("sideNav");
});
