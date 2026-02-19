import { loadModule } from "/js/__moduleLoader.js";

async function handleClicks(e) {
    const sidenav = document.getElementById('sidenav');
    const isExpanded = sidenav.classList.contains('expanded');

    const rootItem = e.target.closest('#nav-items > li');
    if (!rootItem) return;

    const submenu = rootItem.querySelector(':scope > .submenu');

    if (submenu) {
        // has submenu - expand sidenav if collapsed, then toggle submenu
        if (!isExpanded) {
            sidenav.classList.add('expanded');
            document.body.classList.add('sidenav-expanded');
        }
        rootItem.classList.toggle('open');
        const icon = rootItem.querySelector(':scope > .expand-icon i');
        if (icon) {
            icon.classList.toggle('fa-angle-right');
            icon.classList.toggle('fa-angle-down');
        }
    } else {
        // no submenu - find data-target and load module
        const targetEl = e.target.closest('li[data-target]') ?? rootItem;
        const target = targetEl?.dataset.target;
        if (!target) return;

        if (target === "ticket-view") await loadModule("ticketsControl");
        else if (target === "manage-personnel") await loadModule("managePersonnel");
        else if (target === "manage-teams") await loadModule("manageTeams");
        else if (target === "manage-depts") await loadModule("manageDepartments");
        else if (target === "manage-roles") await loadModule("manageRoles");

        document.querySelector('#nav-items .active-page')?.classList.remove('active-page');
        targetEl.classList.add('active-page');
    }
}


export async function init() {
    let nav = document.querySelector('.side-nav');
    nav.addEventListener("click", handleClicks);
}