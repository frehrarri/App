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

        const row = document.querySelector(`.app-table-row[data-uid='${key}']`);

        return {
            deptKey: deptKey,
            //teamKey: teamKey,
            //employeeId: parseInt(row.dataset.employeeid),
            dbChangeAction: value.dbChangeAction,
           /* roleId: row.dataset.roleid*/
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

export async function getAssignedDepartmentTeams() {
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
        controlType: 2,
        saveCallback: saveDeptTeams
    }
    await loadModule("gridControl", deptTeam);

    let deptUser = { 
        newId: 'dept-user',
        rows: [],
        controlType: 1,
        saveCallback: saveDeptUsers
    }

    await loadModule("gridControl", deptUser);
}