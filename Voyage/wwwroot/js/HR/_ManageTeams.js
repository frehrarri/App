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

async function saveTeams(e, changeTracker, newId) {
    e.preventDefault();
    let response;

    debugger;

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

        hyperlinkResponse(response, changeTracker, newId, 'goto-assign-team');
       
    } catch (error) {
        alert("Error");
        console.error("error", error);
    }
}

async function handleEvents(e) {
    if (e.type == "click" && e.target.classList.contains("goto-assign-team")) {
        let params = {
            teamkey: e.target.dataset.datakey,
            teamName: e.target.textContent.trim()
        };
        
        await loadModule("assignTeam", params);
    }
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


export async function init() {
    //load partial
    let partial = await getManageTeamsPartial();
    document.getElementById("hr-partial-container").innerHTML = partial;

    //grid
    let teams = await getTeams();

    const teamNames = teams.map(list => {
        return {
            name: list.name,
            datakey: list.teamKey
        }
    });

    let manageTeam = {
        headers: ["Team"],
        newId: 'manage-team',
        rows: teamNames,
        controlType: 0,
        saveCallback: saveTeams,
        redirectClass: "goto-assign-team"
    }
    await loadModule("gridControl", manageTeam);

    //event handlers
    let container = document.getElementById('hr-partial-container');
    container.addEventListener("click", handleEvents);
}
