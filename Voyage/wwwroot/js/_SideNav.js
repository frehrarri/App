import { loadModule } from "/js/__moduleLoader.js";

async function handleClicks(e) {
    const sidenav = document.getElementById('sidenav');
    const isExpanded = sidenav.classList.contains('expanded');

    // check if click came from inside a submenu (leaf node)
    const submenuItem = e.target.closest('.submenu li[data-target]');
    if (submenuItem) {
        const target = submenuItem.dataset.target;
        if (target === "manage-personnel") await loadModule("managePersonnel");
        else if (target === "manage-teams") await loadModule("manageTeams");
        else if (target === "manage-depts") await loadModule("manageDepartments");
        else if (target === "manage-roles") await loadModule("manageRoles");

        document.querySelector('#nav-items .active-page')?.classList.remove('active-page');
        submenuItem.classList.add('active-page');
        return; 
    }

    const rootItem = e.target.closest('#nav-items > li');
    if (!rootItem) return;

    const submenu = rootItem.querySelector(':scope > .submenu');
    if (submenu) {
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
        const target = rootItem.dataset.target;
        if (!target) return;

        if (target === "ticket-view") await loadModule("ticketsControl");

        document.querySelector('#nav-items .active-page')?.classList.remove('active-page');
        rootItem.classList.add('active-page');
    }
}


export async function init() {
    let nav = document.querySelector('.side-nav');
    nav.addEventListener("click", handleClicks);

    //let navtoggle = document.querySelector('#btn-side-nav-toggle');
    //navtoggle.addEventListener("click", () => { 
        

        
    //})
}