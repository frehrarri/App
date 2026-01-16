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
                const module = await loadModule("tickets");
                await module.getTicketsPartial();
            }
            else if (targetId === "hr-view") {
                debugger;
                await loadModule("hrControl");
            }
        });
    });
}

export async function init() {
    getSideNavItem();
}