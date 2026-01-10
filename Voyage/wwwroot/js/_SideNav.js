import { loadModule } from "./__moduleLoader.js";

async function getTicketsPartial() {
    try {
        const response = await axios.get(`/Tickets/TicketsPartial`);
        document.getElementById("ticket-view").innerHTML = response.data;
        await loadModule("tickets");
        return true;
    } catch (error) {
        console.error("error", error);
        return false;
    }
}

export function init() {
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
                await getTicketsPartial();
            }
            // Add other cases as you build them
            // else if (targetId === "chat-view") {
            //     await getChatPartial();
            // }
        });
    });
}