import { loadModule } from "/js/__moduleLoader.js";

export function init() {
    removeEventListeners();

    //load initial partial
    let partial = await getDepartmentDetailsPartial();
    const container = document.querySelector(".main-content");

    if (!container || !partial) {
        console.log("could not load department details");
        return;
    }

    container.innerHTML = partial;

    //await updateBreadCrumb();
}

export async function getDepartmentDetailsPartial() {
    try {
        const response = await axios.get('/Hr/DepartmentDetailsPartial');
        return response.data;
    } catch (error) {
        console.error("error: getDepartmentDetailsPartial", error);
        return false;
    }
} 