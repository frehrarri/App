const scriptVersion = new Date().getTime(); //append version to avoid caching issues

//when a partial view is rendered loadModule is called
const moduleRegistry = {

    //shared
    sideNav: {
        js: () => import(`./_sidenav.js?v=${scriptVersion}`),
        css: `/css/_sidenav.css?v=${scriptVersion}`
    },

    //tickets
    manageTicket: {
        js: () => import(`./_manageTicket.js?v=${scriptVersion}`),
        css: `/css/_manageTicket.css?v=${scriptVersion}`
    },
    tickets: {
        js: () => import(`./_tickets.js?v=${scriptVersion}`),
        css: `/css/_tickets.css?v=${scriptVersion}`
    },
    ticket: {
        js: () => import(`./_ticket.js?v=${scriptVersion}`),
        css: `/css/_ticket.css?v=${scriptVersion}`
    },
    ticketSettings: {
        js: () => import(`./_ticketsettings.js?v=${scriptVersion}`)//,
        //css: `/css/_ticket.css?v=${scriptVersion}`
    }
};

// Dynamically load CSS if not already loaded
function loadCss(href) {
    return new Promise((resolve, reject) => {
        if (document.querySelector(`link[href="${href}"]`)) {
            resolve(); // already loaded
            return;
        }
        const link = document.createElement("link");
        link.rel = "stylesheet";
        link.href = href;
        link.onload = () => resolve();
        link.onerror = () => reject(`Failed to load CSS: ${href}`);
        document.head.appendChild(link);
    });
}

export async function loadModule(name, params) {
    const mod = moduleRegistry[name];
    if (!mod) {
        console.warn(`Module '${name}' not registered`);
        return;
    }

    // 1️⃣ Load CSS first if exists
    if (mod.css) {
        await loadCss(mod.css);
    }
    // 2️⃣ Load JS module
    const jsModule = await mod.js();

    // 3️⃣ Call init if available
    if (typeof jsModule.init === "function") {
        jsModule.init(params);
    }

    return jsModule;
}