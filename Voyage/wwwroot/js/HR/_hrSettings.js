export async function init() {

    //add hr control partial to view
    let partial = await getHrSettingsPartial();
    document.getElementById("hr-control-container").innerHTML = partial;

    //add event handlers
    //const header = document.getElementById('hr-control-header');
    //header.querySelectorAll('.hr-control-tab')?.forEach(el => el.addEventListener("click", handleEvents));
    //header.querySelectorAll('.settings-btn')?.forEach(el => el.addEventListener("click", handleEvents));

    //load default module
    const module = await loadModule("managePersonnel");
    partial = await module.getManagePersonnelPartial();
    document.getElementById("hr-partial-container").innerHTML = partial;

}

export async function getHrSettingsPartial() {
    const response = await axios.get('/Hr/HrSettingsPartial');
    return response.data;
}

async function handleEvents(e) {
    e.preventDefault();

    await handleTabs(e);
}


async function handleTabs(e) {
    let response;
    let partial;

    const companyId = parseInt(document.getElementById('hdnCompanyId').value);
    const header = document.getElementById('hr-control-header');
    let activeTab = header.querySelector('.hr-control-tab.nav-link.active');

    if (e.target.dataset.tab === "ManageTeams") {
        activeTab.classList.remove('active');
        e.target.classList.add('active');
        await init();
    }

}