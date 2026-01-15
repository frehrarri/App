import { loadModule } from "/js/__moduleLoader.js";

//Initialize side nav when the page loads
document.addEventListener("DOMContentLoaded", async () => {
    await loadModule("sideNav");
});

