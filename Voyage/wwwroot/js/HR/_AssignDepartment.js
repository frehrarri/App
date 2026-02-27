import { loadModule } from "/js/__moduleLoader.js";

const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

export async function getAssignDepartmentPartial(params) {
    try {
        const payload = {
            deptKey : params.datakey,
            deptName : params.name
        }

        const response = await axios.get('/Hr/AssignDepartmentPartial', {
            params: payload
        });

        return response.data;
    } catch (error) {
        alert("Error: getAssignDepartmentPartial")
        console.error("error", error);
        return false;
    }
}

async function saveDeptTeams(e, changeTracker) {
    if (changeTracker.size === 0) {
        return;
    }

    let response;
    
    const deptKey = document.getElementById('hdn-dept-key').value;
    const payload = Array.from(changeTracker.entries()).map(([key, value]) => {
        return {
            departmentKey: deptKey,
            teamKey: value.teamKey,
            dbChangeAction: value.dbChangeAction,
        };
    });

    try {
        response = await axios.post('/Hr/SaveAssignDepartmentTeams', payload, {
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            }
        });

        if (response && response.status === 200) {
            alert("Success");
            changeTracker.clear();
        }
        else {
            alert("Error");
        }

    } catch (error) {
        console.error("error", error);
        return false;
    }
}

async function saveDeptUsers(e, changeTracker) {
    if (changeTracker.size === 0) {
        return;
    }

    let response;
    const deptKey = document.getElementById('hdn-dept-key').value;

    const payload = Array.from(changeTracker.entries()).map(([key, value]) => {

        return {
            departmentKey: deptKey,
            employeeId: parseInt(value.employeeid),
            roleId: value.roleid,
            dbChangeAction: value.dbChangeAction,
           
        };
    });

    try {
        response = await axios.post('/Hr/SaveAssignDepartmentUsers', payload, {
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            }
        });

        if (response && response.status === 200) {
            alert("Success");
            changeTracker.clear();
        }
        else {
            alert("Error");
        }

    } catch (error) {
        console.error("error", error);
        return false;
    }
}

async function getAssignedDepartmentTeams() {
    const deptKey = document.getElementById('hdn-dept-key').value;
    try {
        const response = await axios.get('/Hr/GetAssignedDepartmentTeams', {
            params: { deptKey }
        });

        return response.data;
    } catch (error) {
        alert("Error: getAssignedDepartmentTeams")
        console.error("error", error);
        return false;
    }
}

async function getAssignedDepartmentUsers(params) {
    const departmentKey = document.getElementById('hdn-dept-key').value;
    try {
        const response = await axios.get('/Hr/GetAssignedDepartmentUsers', {
            params: { departmentKey }
        });

        return response.data;
    } catch (error) {
        alert("Error: getAssignedDepartmentUsers")
        console.error("error", error);
        return false;
    }
}


export async function init(params) {
    //load initial partial
    let partial = await getAssignDepartmentPartial(params);
    const container = document.querySelector(".main-content");

    if (container)
        container.innerHTML = partial;

    //load permissions partial
    await loadModule("deptPermissions", params);

    const tabs = document.getElementById('assign-dept-tabs');
    if (tabs)
        tabs.addEventListener("click", handleTabs);

    await updateBreadCrumb();

    const centerHead = document.getElementById('header-center');
    centerHead.innerHTML = "";

    let assignedTeams = await getAssignedDepartmentTeams();
    let teamNames = [];

    if (assignedTeams) {
        teamNames = assignedTeams.map(list => {
            return {
                teamName: list.teamName,
                teamKey: list.teamKey
            }
        });
    }

    let deptTeam = {
        newId: 'dept-team',
        rows: teamNames,
        controlType: 3,
        saveCallback: saveDeptTeams
    }
    await loadModule("gridControl", deptTeam);

    let assignedUsers = await getAssignedDepartmentUsers(params);
    let users = [];

    if (assignedUsers) {
        users = assignedUsers.map(list => {
            return {
                deptKey: list.departmentKey,
                employeeid: list.employeeId,
                roleid: list.roleId,
                firstname: list.firstName,
                lastname: list.lastName,
                username: list.username,
                email: list.email
            }
        });
    }
   

    let deptUser = { 
        newId: 'dept-user',
        rows: users,
        controlType: 4,
        saveCallback: saveDeptUsers
    }

    await loadModule("gridControl", deptUser);

    
}

async function updateBreadCrumb() {
    const ol = document.querySelector('.breadcrumb');

    ol.innerHTML = '';

    const li1 = document.createElement('li');
    li1.classList.add('breadcrumb-item');
    li1.classList.add('active')

    const a1 = document.createElement('a');
    a1.href = "#";
    a1.textContent = 'Human Resouces'

    const module = await loadModule('sideNav');
    a1.addEventListener("click", module.expandSideNavItem);

    li1.appendChild(a1);
    ol.appendChild(li1);

    const li2 = document.createElement('li');
    li2.classList.add('breadcrumb-item');
    li2.classList.add('active');

    const a2 = document.createElement('a');
    a2.href = "#";
    a2.textContent = 'Manage Departments'

    a2.addEventListener("click", async () => await loadModule('manageDepartments'));

    li2.appendChild(a2);
    ol.appendChild(li2);

    const li3 = document.createElement('li');
    li3.classList.add('breadcrumb-item');
    li3.classList.add('active');
    li3.textContent = 'Department'

    ol.appendChild(li3);
}



function handleTabs(e) {
    if (!e.target.classList.contains('tab'))
        return;

    const activeElements = document.querySelectorAll('.active-element:not(.tab)');

    //update tabs
    const activeTab = document.querySelector('#assign-dept-tabs > .active-tab');
    activeTab.classList.remove('active-tab');
    e.target.classList.add('active-tab');

    //hide previous elements
    activeElements.forEach(el => {
        el.classList.add('hidden');
        el.classList.remove('active-element')
    });

    //show current elements
    let elements = null;
    if (e.target.classList.contains('teams')) {
        elements = document.querySelectorAll('.teams');
    }
    else if (e.target.classList.contains('personnel')) {
        elements = document.querySelectorAll('.personnel');
    }
    else if (e.target.classList.contains('permissions')) {
        elements = document.querySelectorAll('.permissions');
    }

    if (elements)
        elements.forEach(el => {
            el.classList.add('active-element');
            el.classList.remove('hidden');
        });
}