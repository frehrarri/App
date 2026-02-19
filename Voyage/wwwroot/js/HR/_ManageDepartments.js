import { loadModule } from "/js/__moduleLoader.js";

const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

export async function init() {
    //load initial partial
    let partial = await getManageDepartmentsPartial();
    const container = document.querySelector(".main-content");

    if (container) 
        container.innerHTML = partial;

    //grid
    let depts = await getDepartments();
    let departments = [];

    if (depts) {
        departments = depts.map(list => {
            return {
                name: list.name,
                datakey: list.departmentKey
            }
        });
    }

    let manageDept = {
        headers: ["Department"],
        newId: 'manage-dept',
        rows: departments,
        controlType: 0,
        saveCallback: saveDepartments,
        redirectCallback: redirect
    }
    await loadModule("gridControl", manageDept);

    updateBreadCrumb();
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

async function saveDepartments(e, changeTracker, newId, currentVals) {
    if (changeTracker.size === 0) {
        return;
    }

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

        if (response && response.status === 200) {
            alert("Success");
            hyperlinkResponse(response, changeTracker, newId, currentVals);
            changeTracker.clear();
        }
        else {
            alert("Error");
        }

       
    } catch (error) {
        showSuccess(false);
        console.error("error", error);
        return false;
    }
}

async function redirect(data) {
    await loadModule("assignDepartment", data);
}


function updateBreadCrumb() {
    const ol = document.querySelector('.breadcrumb');

    ol.innerHTML = '';

    const li1 = document.createElement('li');
    li1.classList.add('breadcrumb-item');
    li1.classList.add('active')

    const a1 = document.createElement('a');
    a1.href = "#";
    a1.textContent = 'Human Resources'

    //add event listener that when clicked on opens side nav and expands Human Resources links

    li1.appendChild(a1);
    ol.appendChild(li1);

    const li2 = document.createElement('li');
    li2.classList.add('breadcrumb-item');
    li2.classList.add('active');

    const a2 = document.createElement('a');
    a2.href = "#";
    a2.textContent = 'Manage Departments'

    li2.appendChild(a2);
    ol.appendChild(li2);
}

