import { loadModule } from "/js/__moduleLoader.js";

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

async function saveDeptTeams(e) {

}

async function saveDeptUsers(e) {

}



export async function init(params) {
    //load initial partial
    let partial = await getAssignDepartmentPartial(params);
    document.getElementById("hr-partial-container").innerHTML = partial;

    let deptTeam = {
        containerId: 'assign-department-container',
        newId: 'dept-team',
        rows: ["IT"],
        controlType: 2,
        saveCallback: saveDeptTeams
    }
    await loadModule("gridControl", deptTeam);

    let deptUser = { 
        containerId: 'assign-department-container',
        newId: 'dept-user',
        rows: [],
        controlType: 1,
        saveCallback: saveDeptUsers
    }

    await loadModule("gridControl", deptUser);
}