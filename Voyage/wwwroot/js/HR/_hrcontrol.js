import { loadModule } from "/js/__moduleLoader.js";

export async function init() {
    const header = document.getElementById('hr-control-header');
    header.querySelectorAll('.hr-control-tab')?.forEach(el => el.addEventListener("click", handleEvents));

    await getManagePersonnelPartial();
}

async function getManagePersonnelPartial() {
    const companyId = parseInt(document.getElementById('hdnCompanyId').value);

    const response = await axios.get('/Hr/ManagePersonnelPartial', {
        params: { companyId: companyId }
    });

    if (response) {
        document.getElementById("hr-partial-container").innerHTML = response.data;
        await loadModule("managePersonnel");
    }
}
   

export async function getHrControlPartial() {
    const response = await getPartial("Hr", "HrControlPartial");
    if (response) {
        document.getElementById("hr-partial-container").innerHTML = response.data;
        await loadModule("hrControl");
    }
}

async function handleEvents(e) {
    e.preventDefault();
    e.stopPropagation();

    let response;

    const companyId = parseInt(document.getElementById('hdnCompanyId').value);
    const header = document.getElementById('hr-control-header');
    let activeTab = header.querySelector('.hr-control-tab.nav-link.active');

    if (e.target.dataset.tab === "Personnel") {
        activeTab.classList.remove('active');
        e.target.classList.add('active');

        await getManagePersonnelPartial(companyId);
    }
    else if (e.target.dataset.tab === "Teams") {
        activeTab.classList.remove('active');
        e.target.classList.add('active');

        response = await axios.get('/Hr/ManageTeamsPartial', {
            params: { companyId: companyId }
        });

        document.getElementById("hr-partial-container").innerHTML = response.data;
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

        response = await axios.get('/Hr/ManageRolesPartial', {
            params: { companyId: companyId }
        });

        document.getElementById("hr-partial-container").innerHTML = response.data;
        await loadModule("manageRoles");
    }

}







