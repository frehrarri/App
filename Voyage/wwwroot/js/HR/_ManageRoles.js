import { loadModule } from "/js/__moduleLoader.js";

const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

export async function getManageRolesPartial() {
    try {
        const response = await axios.get('/Hr/ManageRolesPartial');
        return response.data;
    } catch (error) {
        console.error("error: ManageRolesPartial", error);
        return false;
    }
}

async function saveRoles(e, changeTracker, newId, currentVals) {

    if (changeTracker.size === 0) {
        return;
    }

    const values = Array.from(changeTracker.values());

    let payload = values.map(list => {
        return {
            dbChangeAction: list.dbChangeAction,
            roleKey: list.datakey,
            name: list.name
        }
    })

    try {
        const response = await axios.post('/Hr/SaveRoles', payload, {
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            }
        });

        if (response && response.status === 200) {
            alert("Success");
            hyperlinkResponse(response, changeTracker, newId, currentVals);
            changeTracker.clear();
        }
        else {
            alert("Error");
        }

    } catch (error) {
        alert("Error");
        console.error("error", error);
        return false;
    }
}

async function getRoles() {
    try {
        const response = await axios.get('/Hr/GetRoles');
        return response.data;
    } catch (error) {
        alert("Error: getRoles")
        console.error("error", error);
        return false;
    }
}

export async function init() {
    //load initial partial
    const partial = await getManageRolesPartial();
    const container = document.querySelector(".main-content");

    if (container) 
        container.innerHTML = partial;

    updateBreadCrumb();

    //grid
    let roles = await getRoles();

    //remove principal
    const filteredRoles = roles.filter(r => r.roleId !== -1);

    let roleNames = [];

    if (roles) {
        roleNames = filteredRoles.map(list => {
            return {
                name: list.name,
                datakey: list.roleKey
            }
        });
    }

    let manageRoles = {
        headers: ["Roles"],
        newId: 'manage-roles',
        rows: roleNames,
        controlType: 0,
        saveCallback: saveRoles,
        redirectCallback: redirect
    }
    await loadModule("gridControl", manageRoles);
}

async function redirect(data) {
    await loadModule("rolePermissions", data);
}

function updateBreadCrumb() {
    const ol = document.querySelector('.breadcrumb');

    ol.innerHTML = '';

    const li1 = document.createElement('li');
    li1.classList.add('breadcrumb-item');
    li1.classList.add('active')

    const a1 = document.createElement('a');
    a1.href = "#";
    a1.textContent = 'Admin Settings'

    //add event listener that when clicked on opens side nav and expands Human Resources links

    li1.appendChild(a1);
    ol.appendChild(li1);

    const li2 = document.createElement('li');
    li2.classList.add('breadcrumb-item');
    li2.classList.add('active');
    li2.textContent = 'Manage Roles';

    //const a2 = document.createElement('a');
    //a2.href = "#";
    //a2.textContent = 'Manage Roles'

    /*li2.appendChild(a2);*/
    ol.appendChild(li2);
}
