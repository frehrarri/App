import { loadModule } from "/js/__moduleLoader.js";

const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

export async function getAssignTeamPartial(params) {
    params = {
        teamKey: params.datakey,
        teamName: params.name
    };

    try {
        const response = await axios.get('/Hr/AssignTeamPartial', {
            params: params
        });

        return response.data;
    } catch (error) {
        alert("Error: getAssignTeamPartial")
        console.error("error", error);
        return false;
    }
}

async function saveAssignTeamMembers(e, changeTracker, newId) {
    e.preventDefault();
    let response;

    const teamKey = document.getElementById('hdn-team-key').value;

    const payload = Array.from(changeTracker.entries()).map(([key, value]) => {

        return {
            employeeId: value.employeeid,
            teamKey: teamKey,
            dbChangeAction: value.dbChangeAction,
            roleId: value.roleid
        };
    });

    try {
        response = await axios.post('/Hr/SaveAssignTeamMembers', payload, {
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            }
        });

        if (response && response.status === 200) {
            alert("Success");
        }
        else {
            alert("Error");
        }

    } catch (error) {
        console.error("error", error);
        return false;
    }
}

async function getAssignedTeamPersonnel(teamKey) {
    try {
        const response = await axios.get('/Hr/GetAssignedTeamPersonnel', {
            params: { teamKey }
        });

        return response.data;
    } catch (error) {
        alert("Error: getAssignedTeamPersonnel")
        console.error("error", error);
        return false;
    }
}


export async function init(params) {
    //load initial partial
    let partial = await getAssignTeamPartial(params);
    let container = document.getElementById('hr-partial-container');
    container.innerHTML = partial;

    //grid
    let assignedTeams = await getAssignedTeamPersonnel(params.datakey);
    let teamMembers = [];

    if (assignedTeams) {
        teamMembers = assignedTeams.map(list => {
            return {
                teamName: list.teamName,
                teamKey: list.teamKey,
                employeeid: list.employeeId,
                roleid: list.roleId,
                firstname: list.firstName,
                lastname: list.lastName,
                username: list.username,
                email: list.email
            }
        });
    }
   

    let assignTeam = {
        newId: 'assign-team',
        rows: teamMembers,
        controlType: 5,
        saveCallback: saveAssignTeamMembers
    }
    await loadModule("gridControl", assignTeam);
}