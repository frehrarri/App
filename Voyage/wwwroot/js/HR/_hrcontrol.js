import { loadModule } from "/js/__moduleLoader.js";

export function init() {
    debugger;
    const header = document.getElementById('hr-control-header');
    header.querySelectorAll('.hr-control-tab')?.forEach(el => el.addEventListener("click", handleEvents));
}

async function handleEvents(e) {
    e.preventDefault();
    e.stopPropagation();

    let response;

    if (e.target.dataset.tab === "Personnel") {
        response = await getPartial("Hr", "ManagePersonnelPartial");
        document.getElementById("hr-partial-container").innerHTML = response.data;
        await loadModule("managePersonnel");
    }
    else if (e.target.dataset.tab === "Teams") {
        response = await getPartial("Hr", "ManageTeamsPartial");
        document.getElementById("hr-partial-container").innerHTML = response.data;
        await loadModule("manageTeams");
    }
    else if (e.target.dataset.tab === "Departments") {
        response = await getPartial("Hr", "ManageDepartmentPartial");
        document.getElementById("hr-partial-container").innerHTML = response.data;
        await loadModule("manageDepartments");
    }
    else if (e.target.dataset.tab === "Permissions") {
        response = await getPartial("Hr", "ManagePersonnelPartial");
        document.getElementById("hr-partial-container").innerHTML = response.data;
        await loadModule("managePermissions");
    }
    else if (e.target.dataset.tab === "Roles") {
        response = await getPartial("Hr", "ManageRolesPartial");
        document.getElementById("hr-partial-container").innerHTML = response.data;
        await loadModule("manageRoles");
    }

}







