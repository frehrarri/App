import { loadModule } from './__moduleLoader.js';

//Initialize side nav when the page loads
document.addEventListener("DOMContentLoaded", async () => {
    await loadModule("sideNav");
});

