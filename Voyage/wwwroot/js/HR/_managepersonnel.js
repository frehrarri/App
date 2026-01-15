import { loadModule } from "/js/__moduleLoader.js";

export async function getManagePersonnelPartial() {
    try {
        const response = await axios.get('/Hr/ManagePersonnelPartial');

        document.getElementById("hr-view").innerHTML = response.data;
        await loadModule("managePersonnel");

        return true;
    } catch (error) {
        console.error("error: getManageTicketPartial", error);
        return false;
    }
}
