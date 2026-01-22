import { loadModule } from "/js/__moduleLoader.js";

export async function getManagePersonnelPartial() {
    try {
        const companyId = parseInt(document.getElementById('hdnCompanyId').value);

        const response = await axios.get('/Hr/ManagePersonnelPartial', {
            params: { companyId: companyId }
        });

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
        e.preventDefault();
        await loadModule("registerEmployee");
    }

}


export async function init() {
    //load initial partial
    let partial = await getManagePersonnelPartial();
    document.getElementById("hr-partial-container").innerHTML = partial;

    //event handlers
    const container = document.getElementById('hr-partial-container');
    if (container)
        container.addEventListener("click", handleEvents);
}