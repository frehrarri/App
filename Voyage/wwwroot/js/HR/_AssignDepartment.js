import { loadModule } from "/js/__moduleLoader.js";

const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

export async function getAssignDepartmentPartial(params) {
    try {
        const response = await axios.get('/Hr/AssignDepartmentPartial', {
            params: params
        });

        return response.data;
    } catch (error) {
        alert("Error: getAssignDepartmentPartial")
        console.error("error", error);
        return false;
    }
}

async function saveDeptTeams(e, changeTracker) {
    e.preventDefault();
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

        if (response && response.data) {
            return response.data;
        }

    } catch (error) {
        console.error("error", error);
        return false;
    }
}

async function saveDeptUsers(e, changeTracker) {
    e.preventDefault();
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

        if (response && response.data) {
            return response.data;
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
    const departmentKey = params.deptKey;
   
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
    document.getElementById("hr-partial-container").innerHTML = partial;

    let assignedTeams = await getAssignedDepartmentTeams();

    const teamNames = assignedTeams.map(list => {
        return {
            teamName: list.teamName,
            teamKey: list.teamKey
        }
    });

    let deptTeam = {
        newId: 'dept-team',
        rows: teamNames,
        controlType: 3,
        saveCallback: saveDeptTeams
    }
    await loadModule("gridControl", deptTeam);

    let assignedUsers = await getAssignedDepartmentUsers(params);
    const users = assignedUsers.map(list => {
        
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

    let deptUser = { 
        newId: 'dept-user',
        rows: users,
        controlType: 4,
        saveCallback: saveDeptUsers
    }

    await loadModule("gridControl", deptUser);
}