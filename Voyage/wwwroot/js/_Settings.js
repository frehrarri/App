import { loadModule } from "/js/__moduleLoader.js";

export async function init() {
    const partial = await getSettingsPartial();
    let container = document.querySelector('.main-content');

    if (partial && container) {
        container.innerHTML = partial;
        removeEventListeners();
        updateNavHeader();
        updateBreadCrumb();
        resetMainSideNav();

        container = document.querySelector('#settings-container');
        if (container) {
            container.addEventListener("click", handleClicks);
            trackEventListener(container, "click", handleClicks);
        }
        
    }
}

export async function getSettingsPartial() {
    try {
        const response = await axios.get('/Settings/SettingsPartial');
        return response.data;
    } catch (error) {
        console.error("error: getSettingsPartial", error);
        return false;
    }
}

async function handleClicks(e) {
    debugger;
    const btn = e.target.closest('button');
    
    if (!btn)
        return;

    switch (btn.id) {
        case "btn-settings-tickets":
            await loadModule('ticketSettings');
            break;
        default:
            break;
    }
        
}

function updateNavHeader() {
    const page = document.getElementById('dv-navbar-page-title');
    page.innerText = "Settings";
}


async function updateBreadCrumb() {
    const ol = document.querySelector('.breadcrumb');

    ol.innerHTML = '';

    const li1 = document.createElement('li');
    li1.classList.add('breadcrumb-item');
    li1.classList.add('active')
    li1.innerText = 'Settings';
    ol.appendChild(li1);
}

//deselect selected option and close side nav
function resetMainSideNav() {
    const sidenav = document.getElementById('sidenav');
    const offcanvas = bootstrap.Offcanvas.getOrCreateInstance(sidenav);

    // Close the offcanvas
    offcanvas.hide();

    // Remove active-page from any selected items
    document.querySelector('#nav-items .active-page')?.classList.remove('active-page');

    // Close any open submenus and reset their icons
    document.querySelectorAll('#nav-items > li.open').forEach(li => {
        li.classList.remove('open');
        const icon = li.querySelector('.expand-icon > i');
        if (icon) {
            icon.classList.remove('fa-angle-down');
            icon.classList.add('fa-angle-right');
        }
    });
}