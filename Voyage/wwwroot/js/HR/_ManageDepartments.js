import { loadModule } from "/js/__moduleLoader.js";

const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

export async function init() {
    //load initial partial
    let partial = await getManageDepartmentsPartial();
    document.getElementById("hr-partial-container").innerHTML = partial;

    //grid
    let depts = await getDepartments();

    const departments = depts.map(list => {
        return {
            name: list.name,
            datakey: list.departmentKey
        }
    });

    let manageDept = {
        headers: ["Department"],
        newId: 'manage-dept',
        rows: departments,
        controlType: 0,
        saveCallback: saveDepartments,
        redirectCallback: redirect
    }
    await loadModule("gridControl", manageDept);
}

export async function getManageDepartmentsPartial() {
    try {
        const response = await axios.get('/Hr/ManageDepartmentPartial');
        return response.data;
    } catch (error) {
        console.error("error: getManageTicketPartial", error);
        return false;
    }
} 

async function getDepartments() {
    try {
        const response = await axios.get('/Hr/GetDepartments');
        return response.data;
    } catch (error) {
        alert("Error: getDepartments")
        console.error("error", error);
        return false;
    }
}

async function saveDepartments(e, changeTracker, newId) {
    e.preventDefault();
    let response;

    let values = Array.from(changeTracker.values());

    let payload = values.map(list => {
        return {
            dbChangeAction: list.dbChangeAction,
            deptKey: list.datakey,
            name: list.name
        }
    })
    
    try {
        response = await axios.post('/Hr/SaveDepartments', payload, {
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            }
        });

        hyperlinkResponse(response, changeTracker, newId, 'goto-assign-dept');

       
    } catch (error) {
        showSuccess(false);
        console.error("error", error);
        return false;
    }
}

async function redirect(data) {
    await loadModule("assignDepartment", data);
}




