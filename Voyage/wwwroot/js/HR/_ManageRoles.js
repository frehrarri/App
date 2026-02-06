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
    const partial = await getManageRolesPartial();
    document.getElementById("hr-partial-container").innerHTML = partial;

    //grid
    let roles = await getRoles();
    let roleNames = [];

    if (roles) {
        roleNames = roles.map(list => {
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

async function redirect() {

}