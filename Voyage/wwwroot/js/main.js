import { loadModule } from "/js/__moduleLoader.js";

//Initialize side nav when the page loads
document.addEventListener("DOMContentLoaded", async () => {
    let hamburger = document.getElementById('btn-side-nav-toggle');
    hamburger.classList.remove('hidden');

    await loadModule("sideNav");

    var sidenav = document.getElementById('sidenav');

    //hamburger - expand side nav
    sidenav.addEventListener('show.bs.offcanvas', function () {
        sidenav.classList.add('expanded');
        document.body.classList.add('sidenav-expanded');
    });

    //hamburger - collapse side nav
    sidenav.addEventListener('hide.bs.offcanvas', function () {
        sidenav.classList.remove('expanded');
        document.body.classList.remove('sidenav-expanded');
    });

    //prevent offcanvas focus trap that was causing issue with inputs outside of sidenav
    sidenav.addEventListener('shown.bs.offcanvas', function () {
        const offcanvasInstance = bootstrap.Offcanvas.getInstance(sidenav);
        if (offcanvasInstance) {
            offcanvasInstance._focustrap.deactivate();
        }
    });

    //replace .main-content with main dashboard partial
    await loadModule("mainDashboard");

    const li = document.querySelector('li[data-target="main-dashboard"]');
    li.classList.add('active-page');

    setNavPageTitle();

    const container = document.getElementById('header-nav');
    container.addEventListener("click", handleEvents)
});

function setNavPageTitle(){
    const dvPageTitle = document.querySelector('#dv-navbar-page-title');
    dvPageTitle.innerText = "Home";
}

async function handleEvents(e) {
    const targetId = e.target.closest("#settings-icon").id;
    if (targetId && targetId == "settings-icon") {

    }

    //if (settings === "ticket") await loadModule('ticketSettings');
    //else if (settings === "hr") await loadModule('hrSettings');
}


