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

const token = document.querySelector('input[name="__RequestVerificationToken"]').value;


async function saveTeams() {

    let response;
    let payload = getTeams();

    try {
        response = await axios.post('/Hr/SaveTeams', payload, {
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            }
        });

        if (response && response.data) {
            showSuccess(true);
 
            //update dropdown with save
            document.querySelectorAll('.sel-assign-team-member')?.forEach(dropdown => {
                dropdown.replaceChildren();

                let option = document.createElement('option');
                option.value = "";
                option.innerText = "Unassigned";
                dropdown.appendChild(option);

                response.data.forEach(team => {
                    option = document.createElement('option');
                    option.value = team.teamId;
                    option.innerText = team.name;
                    dropdown.appendChild(option);
                });
            });
                
            return response.data;
        }
        
    } catch (error) {
        showSuccess(false);
        console.error("error", error);
        return false;
    }
}


function addNewTeamRow() {
    const tbody = document.querySelector("#manage-teams > tbody");

    const tr = document.createElement("tr");
    tr.classList.add("app-table-row");

    //checkbox
    const td1 = document.createElement("td");
    td1.classList.add("app-table-data");
    const checkbox = document.createElement("input");
    checkbox.type = "checkbox";
    td1.appendChild(checkbox);
    tr.appendChild(td1);

    //add
    const td2 = document.createElement("td");
    td2.classList.add("app-table-data");
    const addSpan = document.createElement("span")
    addSpan.classList.add("add-team-span");
    addSpan.textContent = "Click to add team";
    td2.appendChild(addSpan);
    tr.appendChild(td2);

    tbody.appendChild(tr);
}


function addTeamInput(e) {
    const target = e.target;

    // Only act on spans
    if (target.classList.contains("add-team-span") && e.type === "click") {
        const input = document.createElement("input");
        input.type = "text";
        input.placeholder = "Team Name";
        input.className = "add-team-input";
        input.value = target.textContent;

        target.replaceWith(input);
        input.focus();
        input.value = "";

        // Save on blur
        input.addEventListener("blur", () => {
            const span = document.createElement("span");
            span.className = "add-team-span";
            span.textContent = input.value || "Click to add team";
            input.replaceWith(span);
        });

        // Save on Enter
        input.addEventListener("keydown", (ev) => {
            if (ev.key === "Enter") {
                input.blur(); // triggers blur listener
            }
        });
    }

}

function removeTeam(e) {
    const checkedBoxes = document.querySelectorAll("#manage-teams tbody input[type='checkbox']:checked");

    //remove row of checked boxes
    checkedBoxes.forEach(cb => {
        const row = cb.closest("tr"); 
        if (row)
            row.remove();
    });
}

function getTeams() {
    let results = [];
    const teams = document.querySelectorAll(".add-team-span");

    teams.forEach(t => {

        if (t.textContent.trim() != "Click to add team")
            results.push(t.textContent.trim());
    });

    return results;
}


const teamMembers = [];

async function saveTeamMembers() {
    try {
        response = await axios.post('/Hr/SaveTeamMembers', teamMembers, {
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            }
        });

        teamMembers = [];

        showSuccess(true);

        return response.data;
    } catch (error) {
        showSuccess(false);
        console.error("error", error);
        return false;
    }
}

async function handleToggleTabs(e) {
    
    if (e.target.classList.contains("btn-get-manage-teams")) {
        document.getElementById("dv-allocate-personnel").classList.add("hidden");
        document.getElementById("dv-manage-teams").classList.remove("hidden");
    } else if (e.target.classList.contains("btn-get-allocate-teams")) {
        document.getElementById("dv-allocate-personnel").classList.remove("hidden");
        document.getElementById("dv-manage-teams").classList.add("hidden");
    }
        
}

async function handleEvents(e) {

    await handleToggleTabs(e);

    //save teams
    if (e.target.id == "team-save-btn") {
        e.preventDefault();
        await saveTeams();
    }

    //remove team
    if (e.target.id == "remove-team-btn")
        removeTeam(e);

    //add new team row
    if (e.target.id == "add-team-btn")
        addNewTeamRow();

    //input control for adding team
    if (e.target.classList.contains("add-team-span")) {
        addTeamInput(e);
    }

    //catch changes to assign team members
    if (e.target.classList.contains('sel-assign-team-member')) {

        if (e.target.dataset.userid && e.target.value) {

            const teamDto = {
                userId: parseInt(e.target.dataset.userid),
                teamId: parseInt(e.target.value)
            }
            teamMembers.push(teamDto);
        }
    }

    //save members to team
    if (e.target.id == "team-member-save-btn")
        await saveTeamMembers();
        
}


export async function init() {
    //load partial
    let partial = await getManageTeamsPartial();
    document.getElementById("hr-partial-container").innerHTML = partial;

    //event handlers
    let container = document.getElementById('hr-partial-container');
    container.addEventListener("click", handleEvents);
    container.addEventListener("keydown", handleEvents);
}
