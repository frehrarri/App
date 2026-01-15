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

const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

async function savePermissions() {
    let response;

    let ul = document.getElementById('ul-depts');
    let li = ul.querySelectorAll('li');

    let payload = Array.from(li).map(item => item.innerText);

    try {
        response = await axios.post('/Hr/SavePermissions', payload, {
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            }
        });

        showSuccess(true);

        return response.data;
    } catch (error) {
        showSuccess(false);
        console.error("error", error);
        return false;
    }
}

async function saveRoles() {
    let response;
    debugger;
    let ul = document.getElementById('ul-roles');
    let li = ul.querySelectorAll('li');

    let payload = Array.from(li).map(item => item.innerText);

    try {
        response = await axios.post('/Hr/SaveRoles', payload, {
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            }
        });

        showSuccess(true);

        return response.data;
    } catch (error) {
        showSuccess(false);
        console.error("error", error);
        return false;
    }
}

async function saveTeams() {
    let response;

    let ul = document.getElementById('ul-teams');
    let li = ul.querySelectorAll('li');

    let payload = Array.from(li).map(item => item.innerText);

    try {
        response = await axios.post('/Hr/SaveTeams', payload, {
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            }
        });

        showSuccess(true);

        return response.data;
    } catch (error) {
        showSuccess(false);
        console.error("error", error);
        return false;
    }
}

async function saveDepartments() {
    let response;

    let ul = document.getElementById('ul-depts');
    let li = ul.querySelectorAll('li');

    let payload = Array.from(li).map(item => item.innerText);

    try {
        response = await axios.post('/Hr/SaveDepartments', payload, {
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            }
        });

        showSuccess(true);

        return response.data;
    } catch (error) {
        showSuccess(false);
        console.error("error", error);
        return false;
    }
}

async function handleEvents(e) {
    e.preventDefault();

    if (e.target.tagName == "INPUT")
        return;

    // add 
    if (e.target.id == "add-priv-btn") 
        addPermission();

    if (e.target.id == "add-role-btn")
        addRole();

    if (e.target.id == "add-team-btn")
        addTeam();

    if (e.target.id == "add-dept-btn")
        addDepartment();

    // remove 
    let li = e.target.closest("#ul-privileges li");
    if (li) {
        li.remove();
    }

    li = e.target.closest("#ul-roles li");
    if (li) {
        li.remove();
    }

    li = e.target.closest("#ul-teams li");
    if (li) {
        li.remove();
    }

    li = e.target.closest("#ul-depts li");
    if (li) {
        li.remove();
    }

    //save
    if (e.target.id == "priv-save-btn")
        await savePermissions();

    if (e.target.id == "role-save-btn")
        await saveRoles();

    if (e.target.id == "team-save-btn")
        await saveTeams();

    if (e.target.id == "dept-save-btn")
        await saveDepartments();
}

function addDepartment() {
    const ul = document.getElementById("ul-depts");
    const li = document.createElement("li");
    let newDept = document.getElementById("add-dept");

    li.textContent = `${newDept.value}`;
    li.addEventListener("click", handleEvents);

    ul.appendChild(li);
    newDept.value = "";
}

function addPermission() {
    const ul = document.getElementById("ul-privileges");
    const li = document.createElement("li");
    let newPriv = document.getElementById("add-priv");

    li.textContent = `${newPriv.value}`;
    li.addEventListener("click", handleEvents);

    ul.appendChild(li);
    newPriv.value = "";
}

function addRole() {
    const ul = document.getElementById("ul-roles");
    const li = document.createElement("li");
    let newRole = document.getElementById("add-role");

    li.textContent = `${newRole.value}`;
    li.addEventListener("click", handleEvents);

    ul.appendChild(li);
    newRole.value = "";
}

function addTeam() {
    const ul = document.getElementById("ul-teams");
    const li = document.createElement("li");
    let newTeam = document.getElementById("add-team");

    li.textContent = `${newTeam.value}`;
    li.addEventListener("click", handleEvents);

    ul.appendChild(li);
    newTeam.value = "";
}



export function init(){
    const container = document.getElementById('manage-personnel-container');
    if (container)
        container.addEventListener("click", handleEvents);
}