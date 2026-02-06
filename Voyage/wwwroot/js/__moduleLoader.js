const scriptVersion = new Date().getTime(); //append version to avoid caching issues

//when a partial view is rendered loadModule is called
const moduleRegistry = {

    //shared
    sideNav: {
        js: () => import(`./_sidenav.js?v=${scriptVersion}`),
        css: `/css/_sidenav.css?v=${scriptVersion}`
    },
    register: {
        js: () => import(`./register.js?v=${scriptVersion}`),
        css: `/css/register.css?v=${scriptVersion}`
    },
    gridControl: {
        js: () => import(`./_gridControl.js?v=${scriptVersion}`),
        css: `/css/_gridControl.css?v=${scriptVersion}`
    },

    //hr
    hrControl: {
        js: () => import(`./HR/_hrcontrol.js?v=${scriptVersion}`),
        css: `/css/HR/_hrcontrol.css?v=${scriptVersion}`
    },
    managePersonnel: {
        js: () => import(`./HR/_managePersonnel.js?v=${scriptVersion}`),
        css: `/css/HR/_managePersonnel.css?v=${scriptVersion}`
    },
    manageDepartments: {
        js: () => import(`./HR/_manageDepartments.js?v=${scriptVersion}`),
        css: `/css/HR/_manageDepartments.css?v=${scriptVersion}`
    },
    manageTeams: {
        js: () => import(`./HR/_manageTeams.js?v=${scriptVersion}`),
        css: `/css/HR/_manageTeams.css?v=${scriptVersion}`
    },
    manageRoles: {
        js: () => import(`./HR/_manageRoles.js?v=${scriptVersion}`),
        css: `/css/HR/_manageRoles.css?v=${scriptVersion}`
    },
    managePermissions: {
        js: () => import(`./HR/_managePermissions.js?v=${scriptVersion}`),
        css: `/css/HR/_managePermissions.css?v=${scriptVersion}`
    },
    registerEmployee: {
        js: () => import(`./HR/_registerEmployee.js?v=${scriptVersion}`),
        css: `/css/HR/_registerEmployee.css?v=${scriptVersion}`
    },
    hrSettings: {
        js: () => import(`./HR/_hrSettings.js?v=${scriptVersion}`),
        css: `/css/HR/_hrSettings.css?v=${scriptVersion}`
    },
    assignDepartment: {
        js: () => import(`./HR/_AssignDepartment.js?v=${scriptVersion}`),
        css: `/css/HR/_AssignDepartment.css?v=${scriptVersion}`
    },
    assignTeam: {
        js: () => import(`./HR/_AssignTeam.js?v=${scriptVersion}`),
        css: `/css/HR/_AssignTeam.css?v=${scriptVersion}`
    },
    assignRolePermissions: {
        js: () => import(`./HR/_AssignRolePermissions.js?v=${scriptVersion}`),
        css: `/css/HR/_AssignRolePermissions.css?v=${scriptVersion}`
    },


    //tickets
    ticketsControl: {
        js: () => import(`./Tickets/_ticketsControl.js?v=${scriptVersion}`),
        css: `/css/Tickets/_ticketsControl.css?v=${scriptVersion}`
    },
    manageTicket: {
        js: () => import(`./Tickets/_manageTicket.js?v=${scriptVersion}`),
        css: `/css/Tickets/_manageTicket.css?v=${scriptVersion}`
    },
    tickets: {
        js: () => import(`./Tickets/_tickets.js?v=${scriptVersion}`),
        css: `/css/Tickets/_tickets.css?v=${scriptVersion}`
    },
    ticket: {
        js: () => import(`./Tickets/_ticket.js?v=${scriptVersion}`),
        css: `/css/Tickets/_ticket.css?v=${scriptVersion}`
    },
    ticketSettings: {
        js: () => import(`./Tickets/_ticketsettings.js?v=${scriptVersion}`),
        css: `/css/Tickets/_ticketsettings.css?v=${scriptVersion}`
    }
    //setTicketSettings: {
    //    js: () => import(`./Tickets/_setTicketSettings.js?v=${scriptVersion}`)//,
    //    //css: `/Tickets/css/_ticket.css?v=${scriptVersion}`
    //}
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