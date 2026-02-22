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

    //replace .main-content with main dashboard partial
    await loadModule("mainDashboard");

    const li = document.querySelector('li[data-target="main-dashboard"]');
    li.classList.add('active-page');
});


