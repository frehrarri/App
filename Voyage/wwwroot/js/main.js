import { loadModule } from "/js/__moduleLoader.js";

//Initialize side nav when the page loads
document.addEventListener("DOMContentLoaded", async () => {
    let hamburger = document.getElementById('btn-side-nav-toggle');
    hamburger.classList.remove('hidden');

    await loadModule("sideNav");

    var sidenav = document.getElementById('sidenav');

    //expand
    sidenav.addEventListener('show.bs.offcanvas', function () {
        sidenav.classList.add('expanded');
        document.body.classList.add('sidenav-expanded');
    });

    //collapse
    sidenav.addEventListener('hide.bs.offcanvas', function () {
        sidenav.classList.remove('expanded');
        document.body.classList.remove('sidenav-expanded');
    });

    ////default to open
    //const sidenavEl = document.getElementById('sidenav');
    //const sidenav = new bootstrap.Offcanvas(sidenavEl);
    //sidenav.show();

    await loadModule("managePersonnel");

});


