import { loadModule } from "/js/__moduleLoader.js";

async function getSideNavItem() {
    const navItems = document.querySelectorAll(".nav-item");
    const views = document.querySelectorAll(".main-content .view");

    navItems.forEach(item => {
        item.addEventListener("click", async function () {
            // Highlight active nav
            navItems.forEach(n => n.classList.remove("active"));
            this.classList.add("active");

            // Hide all views
            views.forEach(view => {
                view.classList.remove("active");
                view.classList.add("hidden");
            });

            const targetId = this.dataset.target;

            // Show target container
            const target = document.getElementById(targetId);
            if (!target) return;

            target.classList.remove("hidden");
            target.classList.add("active");

            // Load partials based on the target
            if (targetId === "ticket-view") {
                await loadModule("ticketsControl");
            }
            else if (targetId === "hr-view") {
                await loadModule("hrControl");
            }
            else if (targetId === "settings-view") {
                await loadModule("adminSettings");
            }
        });
    });
}

async function handleClicks(e) {

    //hamburger toggle
    if (e.target.id == "btn-side-nav-toggle") {
        const sideNav = document.getElementById('sidenav');
        sideNav.classList.toggle('open');
    }

    let li = e.target.closest('li');
    let ul = document.querySelector('#nav-items');

    if(!li || !ul.contains(li)) return;

    // If it has a submenu then toggle
    const submenu = li.querySelector(':scope > .submenu');

    if (submenu) {
        li.classList.toggle('expanded');
        li.classList.toggle('collapsed');

        const icon = li.querySelector(':scope > span i');
        if (icon) {
            icon.classList.toggle('fa-angle-right');
            icon.classList.toggle('fa-angle-down');
        }

        return;
    }


    const target = li.dataset.target;
    if (target) {
        // handle leaf node redirect
        if (target === "ticket-view") {
            await loadModule("ticketsControl");
        }
        //else if (targetId === "hr-view") {
        //    await loadModule("hrControl");
        //}
        //else if (targetId === "settings-view") {
        //    await loadModule("adminSettings");
        //}

        //handle submenu redirects

        //HR
        else if (target === "manage-personnel") {
            await loadModule("managePersonnel");
        }
        else if (target === "manage-teams") {
            await loadModule("manageTeams");
        }
        else if (target === "manage-depts") {
            await loadModule("manageDepartments");
        }
      
        //AdminSettings
        else if (target === "manage-roles") {
            await loadModule("manageRoles");
        }

        let li = document.querySelector('#nav-items .active-page'); 
        if (li)
            li.classList.remove('active-page');

        e.target.classList.add('active-page');
    }

    
}


export async function init() {
    let ul = document.querySelector('#nav-items');
    ul.addEventListener("click", handleClicks);
}