import { loadModule } from "/js/__moduleLoader.js";

const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

export async function init(params) {
    //load initial partial
    let partial = await getAssignTeamPartial(params);
    const container = document.querySelector(".main-content");

    if (container && partial) {
        container.innerHTML = partial;
        container.addEventListener("click", handleEvents);
    }

    //load permissions partial
    await loadModule("teamPermissions", params);

    updateBreadCrumb();

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
    if (changeTracker.size === 0) {
        return;
    }

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

function updateBreadCrumb() {
    const ol = document.querySelector('.breadcrumb');

    ol.innerHTML = '';

    const li1 = document.createElement('li');
    li1.classList.add('breadcrumb-item');
    li1.classList.add('active')

    const a1 = document.createElement('a');
    a1.href = "#";
    a1.textContent = 'Human Resouces'

    //add event listener that when clicked on opens side nav and expands Human Resources links
    //a1.addEventListener("click", expandSideNavItem);

    li1.appendChild(a1);
    ol.appendChild(li1);

    const li2 = document.createElement('li');
    li2.classList.add('breadcrumb-item');
    li2.classList.add('active');

    const a2 = document.createElement('a');
    a2.href = "#";
    a2.textContent = 'Manage Teams'

    //add event listener that when clicked replaces the partial view
    //a1.addEventListener("click", replacePartial);

    li2.appendChild(a2);
    ol.appendChild(li2);

    const li3 = document.createElement('li');
    li3.classList.add('breadcrumb-item');
    li3.classList.add('active');
    li3.textContent = 'Assign Teams'

    ol.appendChild(li3);
}

async function handleEvents(e) {
    const id = e.target.id;

    if (id == 'team-permissions-link') {
        const key = document.getElementById('hdn-team-key').value;
        const name = document.getElementById('team-name').textContent;
        debugger;
        const data = {
            name: name,
            datakey: key
        };

        await loadModule("teamPermissions", data);
    }

    handleTabs(e);        
}

function handleTabs(e) {
    if (!e.target.classList.contains('tab'))
        return;

    debugger;
    const activeElements = document.querySelectorAll('.active-element:not(.tab)');

    //update tabs
    const activeTab = document.querySelector('#assign-team-tabs > .active-tab');
    activeTab.classList.remove('active-tab');
    e.target.classList.add('active-tab');

    //hide previous elements
    activeElements.forEach(el => {
        el.classList.add('hidden');
        el.classList.remove('active-element')
    });

    //show current elements
    let elements = null;
    if (e.target.classList.contains('personnel')) {
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