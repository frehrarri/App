import { loadModule } from "/js/__moduleLoader.js";

//Initialize side nav when the page loads
document.addEventListener("DOMContentLoaded", async () => {
    await loadModule("sideNav");
    await loadModule("hrControl");

    const container = document.querySelector(".main-content.container");
    container.addEventListener("click", handleSettings);
    
});

async function handleSettings(e) {

    const btn = e.target.closest(".settings-btn");

    if (btn) {
        e.stopPropagation();
        const module = await loadModule("setTicketSettings");
        await module.getTicketSettings();
    }
       

        

}

    

