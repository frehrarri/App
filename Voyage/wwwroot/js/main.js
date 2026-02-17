import { loadModule } from "/js/__moduleLoader.js";

//Initialize side nav when the page loads
document.addEventListener("DOMContentLoaded", async () => {
    let hamburger = document.getElementById('btn-side-nav-toggle');
    hamburger.classList.remove('hidden');

    await loadModule("sideNav");

    ////default to open
    //const sidenavEl = document.getElementById('sidenav');
    //const sidenav = new bootstrap.Offcanvas(sidenavEl);
    //sidenav.show();

    await loadModule("managePersonnel");

});


