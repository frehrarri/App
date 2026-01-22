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
    await handleSettings(e);
}

async function handleSettings(e) {
    let response;
    const companyId = parseInt(document.getElementById('hdnCompanyId').value);

    if (e.target.closest(".settings-btn")) {
        response = await axios.get('/Hr/HrSettingsPartial', {
            params: { companyId: companyId }
        });

        document.getElementById("hr-partial-container").innerHTML = response.data;
        await loadModule("hrSettings");
    }
}

async function handleTabs(e) {
    let response;
    let partial;

    const companyId = parseInt(document.getElementById('hdnCompanyId').value);
    const header = document.getElementById('hr-control-header');
    let activeTab = header.querySelector('.hr-control-tab.nav-link.active');

    if (e.target.dataset.tab === "Personnel") {
        activeTab.classList.remove('active');
        e.target.classList.add('active');
        await init();
    }

    else if (e.target.dataset.tab === "Teams") {
        activeTab.classList.remove('active');
        e.target.classList.add('active');
        await loadModule("manageTeams");
    }

    else if (e.target.dataset.tab === "Departments") {
        activeTab.classList.remove('active');
        e.target.classList.add('active');

        response = await axios.get('/Hr/ManageDepartmentPartial', {
            params: { companyId: companyId }
        });

        document.getElementById("hr-partial-container").innerHTML = response.data;
        await loadModule("manageDepartments");
    }

    else if (e.target.dataset.tab === "Permissions") {
        activeTab.classList.remove('active');
        e.target.classList.add('active');

        response = await getPartial("Hr", "ManagePermissionsPartial");
        document.getElementById("hr-partial-container").innerHTML = response.data;
        await loadModule("managePermissions");
    }

    else if (e.target.dataset.tab === "Roles") {
        activeTab.classList.remove('active');
        e.target.classList.add('active');
        await loadModule("manageRoles");
    }
}









