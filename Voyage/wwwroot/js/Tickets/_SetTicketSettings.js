import { loadModule } from "/js/__moduleLoader.js";

export function init() {
    updateBreadCrumb();
}

export async function getTicketSettings() {
    const response = await getPartial("Tickets", "SettingsPartial");
    return response.data;
}

async function updateBreadCrumb() {
    const ol = document.querySelector('.breadcrumb');

    ol.innerHTML = '';

    const li1 = document.createElement('li');
    li1.classList.add('breadcrumb-item');
    li1.classList.add('active')

    const a1 = document.createElement('a');
    a1.href = "#";
    a1.textContent = 'Tickets'

    const module = await loadModule('sideNav');
    a1.addEventListener("click", module.expandSideNavItem);

    li1.appendChild(a1);
    ol.appendChild(li1);
}
