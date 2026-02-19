import { loadModule } from "/js/__moduleLoader.js";

const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

export async function getManageTeamsPartial() {
    try {
        const response = await getPartial("Hr", "ManageTeamsPartial");
        document.getElementById("hr-partial-container").innerHTML = response.data;

        return response.data;
    } catch (error) {
        console.error("error: getManageTicketPartial", error);
        return false;
    }
}

async function saveTeams(e, changeTracker, newId, currentVals) {
    if (changeTracker.size === 0) {
        return;
    }

    let response;
    let values = Array.from(changeTracker.values());
    
    let payload = values.map(list => {
        return {
            dbChangeAction: list.dbChangeAction,
            teamKey: list.datakey,
            name: list.name
        }
    })

    try {
        response = await axios.post('/Hr/SaveTeams', payload, {
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
    }
}


async function redirect(data) {
    await loadModule("assignTeam", data);
}

async function getTeams() {
    try {
        const response = await axios.get('/Hr/GetTeams');
        return response.data;
    } catch (error) {
        alert("Error: getTeams")
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
    a1.textContent = 'Human Resources'

    //add event listener that when clicked on opens side nav and expands Human Resources links

    li1.appendChild(a1);
    ol.appendChild(li1);

    const li2 = document.createElement('li');
    li2.classList.add('breadcrumb-item');
    li2.classList.add('active');

    const a2 = document.createElement('a');
    a2.href = "#";
    a2.textContent = 'Manage Teams'

    li2.appendChild(a2);
    ol.appendChild(li2);
}


export async function init() {
    //load partial
    let partial = await getManageTeamsPartial();
    document.getElementById("hr-partial-container").innerHTML = partial;

    //grid
    let teams = await getTeams();
    let teamNames = [];

    if (teams) {
        teamNames = teams.map(list => {
            return {
                name: list.name,
                datakey: list.teamKey
            }
        });
    }

    let manageTeam = {
        headers: ["Team"],
        newId: 'manage-team',
        rows: teamNames,
        controlType: 0,
        saveCallback: saveTeams,
        redirectCallback: redirect
    }
    await loadModule("gridControl", manageTeam);

    updateBreadCrumb();
}
