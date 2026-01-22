import { loadModule } from "/js/__moduleLoader.js";

export async function getManagePersonnelPartial() {
    try {
        const companyId = parseInt(document.getElementById('hdnCompanyId').value);

        const response = await axios.get('/Hr/ManagePersonnelPartial', {
            params: { companyId: companyId }
        });

        document.getElementById("hr-view").innerHTML = response.data;
        await loadModule("managePersonnel");

        return response.data;
    } catch (error) {
        console.error("error: getManageTicketPartial", error);
        return false;
    }
} 




async function handleEvents(e) {
    if (e.target.tagName == "INPUT")
        return;

    if (e.target.id == "self-register-button") {
        const module = await loadModule("registerEmployee");
        module.getRegisterEmployeePartial(e);
    }

}


export function init(){
    const container = document.getElementById('manage-personnel-container');
    if (container)
        container.addEventListener("click", handleEvents);
}