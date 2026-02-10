import { loadModule } from "/js/__moduleLoader.js";

export async function init() {

    //add hr control partial to view
    let partial = await getHrControlPartial();
    document.getElementById("hr-partial-container").innerHTML = partial;

    //add event handlers
    const header = document.getElementById('hr-control-header');
    header.querySelectorAll('.hr-control-tab')?.forEach(el => el.addEventListener("click", handleEvents));
    header.querySelectorAll('.settings-btn')?.forEach(el => el.addEventListener("click", handleEvents));

    //load default module
    const module = await loadModule("managePersonnel");
    partial = await module.getManagePersonnelPartial();
    document.getElementById("hr-partial-container").innerHTML = partial;
    
}

export async function getHrControlPartial() {
    const response = await getPartial("Hr", "HrControlPartial");
    return response.data;
}

async function handleEvents(e) {
    e.preventDefault();
    e.stopPropagation();

    await handleTabs(e);
    //await handleSettings(e);
}

async function handleSettings(e) {
    let response;
    const companyId = parseInt(document.getElementById('hdnCompanyId').value);

    if (e.target.closest(".settings-btn")) {
        await loadModule("hrSettings");
    }
}

async function handleTabs(e) {
    let response;

    const tab = e.target.closest('.hr-control-tab');
    if (!tab)
        return;

    const header = document.getElementById('hr-control-header');
    let activeTab = header.querySelector('.hr-control-tab.nav-link.active');

    if (tab.dataset.tab === "Personnel") {
        activeTab.classList.remove('active');
        e.target.classList.add('active');
        await init();
    }

    else if (tab.dataset.tab === "Teams") {
        activeTab.classList.remove('active');
        e.target.classList.add('active');
        await loadModule("manageTeams");
    }

    else if (tab.dataset.tab === "Departments") {
        activeTab.classList.remove('active');
        e.target.classList.add('active');
        await loadModule("manageDepartments");
    }

    else if (tab.dataset.tab === "Permissions") {
        activeTab.classList.remove('active');
        e.target.classList.add('active');

        response = await getPartial("Hr", "ManagePermissionsPartial");
        document.getElementById("hr-partial-container").innerHTML = response.data;
        await loadModule("managePermissions");
    }

    else if (tab.dataset.tab === "Roles") {
        activeTab.classList.remove('active');
        e.target.classList.add('active');
        await loadModule("manageRoles");
    }
}









