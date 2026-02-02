import { loadModule } from "/js/__moduleLoader.js";

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
const changeTracker = new Map();

async function saveTeams(e) {
    e.preventDefault();

    let response;
    let payload = Array.from(changeTracker.values());

    try {
        response = await axios.post('/Hr/SaveTeams', payload, {
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            }
        });

        if (response && response.data) {

            //did not add any records so there is nothing to hyperlink
            if (response.data.length > 0) {
                //hyperlink results
                let team = "";
                let index = 0;

                let addedEntries = [...changeTracker.entries()].filter(([key, value]) => value.dbChangeAction === 1);

                for (let [key, value] of addedEntries) {
                    team = document.querySelector(`.add-team-span[data-key='${key}']`);

                    let anchortag = document.createElement('a');
                    anchortag.href = "#";
                    anchortag.classList.add('goto-assign-team')
                    anchortag.textContent = team.textContent.trim();
                    anchortag.dataset.key = key;

                    let teamKey = response.data[index];
                    anchortag.dataset.teamKey = teamKey;

                    team.replaceWith(anchortag);
                    index++;
                }
            }

            alert("Success");

           /* return response.data;*/
        }
        
    } catch (error) {
        alert("Error");
        console.error("error", error);
    }
}


function addNewTeamRow() {
    const tbody = document.querySelector("#manage-teams > tbody");

    const tr = document.createElement("tr");
    tr.classList.add("app-table-row");

    let key = tr.dataset.key;
    if (!key)
        tr.dataset.key = crypto.randomUUID();

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
    // Only act on spans
    if (e.target.classList.contains("add-team-span") && e.type === "click") {
        const row = e.target.parentElement.parentElement;

        const key = row.dataset.key;
        if (!key)
            key = crypto.randomUUID();
        
        const input = document.createElement("input");
        input.type = "text";
        input.placeholder = "Team Name";
        input.className = "add-team-input";
        input.value = e.target.textContent;

        e.target.replaceWith(input);
        input.focus();
        input.value = "";
        input.dataset.key = key;

        // Save on blur
        input.addEventListener("blur", () => {
            const span = document.createElement("span");
            span.className = "add-team-span";
            span.textContent = input.value || "Click to add team";
            span.dataset.key = key;
           
            input.replaceWith(span);

            changeTracker.set(key, {
                name: span.textContent,
                teamKey: null, //placeholder for a new entry that will be assigned by db
                dbChangeAction: 1 //add
            });
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
    if (!confirm("Remove teams?")) {
        return;
    }

    const checkedBoxes = document.querySelectorAll("#manage-teams tbody input[type='checkbox']:checked");

    //remove row of checked boxes
    checkedBoxes.forEach(cb => {
        const row = cb.closest("tr"); 
        if (row) {
            let key = row.dataset.key;

            // remove unsaved addition from change tracker
            const existingChange = changeTracker.get(key);
            if (existingChange && existingChange.dbChangeAction === 1)
                changeTracker.delete(key)

            // prepare for database deletion
            else if (row.dataset.teamkey != null)
                changeTracker.set(key, {
                    name: row.childNodes[1].textContent,
                    teamKey : row.dataset.teamkey,
                    dbChangeAction: 2 //remove
                });

            row.remove();

            saveTeams(e);
        }
            
    });
}


async function handleEvents(e) {

    //save teams
    if (e.target.id == "team-save-btn") {
        await saveTeams(e);
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

    if (e.type == "click" && e.target.classList.contains("goto-assign-team")) {
        let params = {
            teamkey: e.target.dataset.teamkey,
            teamName: e.target.textContent.trim()
        };
        
        await loadModule("assignTeam", params);
    }
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
