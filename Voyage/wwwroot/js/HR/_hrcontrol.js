import { loadModule } from "/js/__moduleLoader.js";

export function init() {
    document.querySelector('.nav-item')?.forEach(el => el.addEventListener("click", handleEvents))
}

async function handleEvents(e) {
    e.preventDefault();

    let response;

    if (e.target.textContent == "Personnel") {
        response = await getPartial("Hr", "ManagePersonnelPartial");
        document.getElementById("hr-partial-container").innerHTML = response.data;
        await loadModule("managePersonnel");
    }
    else if (e.target.textContent == "Teams") {
        response = await getPartial("Hr", "ManageTeamsPartial");
        document.getElementById("hr-partial-container").innerHTML = response.data;
        await loadModule("manageTeams");
    }
    else if (e.target.textContent == "Departments") {
        response = await getPartial("Hr", "ManageDepartmentPartial");
        document.getElementById("hr-partial-container").innerHTML = response.data;
        await loadModule("manageDepartments");
    }
    else if (e.target.textContent == "Permissions") {
        response = await getPartial("Hr", "ManagePersonnelPartial");
        document.getElementById("hr-partial-container").innerHTML = response.data;
        await loadModule("managePermissions");
    }
    else if (e.target.textContent == "Roles") {
        response = await getPartial("Hr", "ManageRolesPartial");
        document.getElementById("hr-partial-container").innerHTML = response.data;
        await loadModule("manageRoles");
    }

}







