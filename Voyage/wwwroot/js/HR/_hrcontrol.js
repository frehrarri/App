import { loadModule } from "/js/__moduleLoader.js";

export async function init() {
    const header = document.getElementById('hr-control-header');
    header.querySelectorAll('.hr-control-tab')?.forEach(el => el.addEventListener("click", handleEvents));

    let response = await getPartial("Hr", "ManagePersonnelPartial");
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

    const header = document.getElementById('hr-control-header');
    let activeTab = header.querySelector('.hr-control-tab.nav-link.active');

    if (e.target.dataset.tab === "Personnel") {
        activeTab.classList.remove('active');
        e.target.classList.add('active');

        response = await getPartial("Hr", "ManagePersonnelPartial");
        document.getElementById("hr-partial-container").innerHTML = response.data;
        await loadModule("managePersonnel");
    }
    else if (e.target.dataset.tab === "Teams") {
        activeTab.classList.remove('active');
        e.target.classList.add('active');

        response = await getPartial("Hr", "ManageTeamsPartial");
        document.getElementById("hr-partial-container").innerHTML = response.data;
        await loadModule("manageTeams");
    }
    else if (e.target.dataset.tab === "Departments") {
        activeTab.classList.remove('active');
        e.target.classList.add('active');

        response = await getPartial("Hr", "ManageDepartmentPartial");
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

        response = await getPartial("Hr", "ManageRolesPartial");
        document.getElementById("hr-partial-container").innerHTML = response.data;
        await loadModule("manageRoles");
    }

}







