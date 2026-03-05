import { loadModule } from "/js/__moduleLoader.js";

export function init() {
    removeEventListeners();

    //load initial partial
    let partial = await getTeamDetailsPartial();
    const container = document.querySelector(".main-content");

    if (!container || !partial) {
        console.log("could not load team details");
        return;
    }

    container.innerHTML = partial;

    //await updateBreadCrumb();
}


export async function getTeamDetailsPartial() {
    try {
        const response = await axios.get('/Hr/TeamDetailsPartial');
        return response.data;
    } catch (error) {
        console.error("error: getTeamDetailsPartial", error);
        return false;
    }
} 