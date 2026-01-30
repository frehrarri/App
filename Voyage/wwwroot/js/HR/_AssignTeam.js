

export async function getAssignTeamPartial() {
    try {
        const response = await axios.get('/Hr/AssignTeamPartial');
        return response.data;
    } catch (error) {
        alert("Error: getAssignTeamPartial")
        console.error("error", error);
        return false;
    }
}

function addNewRow() {
    const tbody = document.querySelector("#tbl-allocate-personnel > tbody");

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
    addSpan.classList.add("add-user-span");
    addSpan.textContent = "Click to add user";
    td2.appendChild(addSpan);
    tr.appendChild(td2);

    tbody.appendChild(tr);
}

function addUserInput(e) {
    const target = e.target;

    // Only act on spans
    if (target.classList.contains("add-user-span") && e.type === "click") {
        const input = document.createElement("input");
        input.type = "text";
        input.placeholder = "User Name";
        input.className = "add-user-input";
        input.value = target.textContent;

        addUserSearchEventListener(input, "#userResults");

        target.replaceWith(input);
        input.focus();
        input.value = "";

        

        // Save on blur
        input.addEventListener("blur", () => {
            const span = document.createElement("span");
            span.className = "add-user-span";
            span.textContent = input.value || "Click to add user";
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

function removeUser(e) {
    const checkedBoxes = document.querySelectorAll("#tbl-allocate-personnel tbody input[type='checkbox']:checked");

    //remove row of checked boxes
    checkedBoxes.forEach(cb => {
        const row = cb.closest("tr");
        if (row)
            row.remove();
    });
}


async function handleEvents(e) {

    ////save teams
    //if (e.target.id == "team-save-btn") {
    //    e.preventDefault();
    //    await saveTeams();
    //}

    //remove user
    if (e.target.id == "remove-user-btn")
        removeUser(e);

    //add new user row
    if (e.target.id == "add-user-btn")
        addNewRow();

    //input control for adding team member
    if (e.target.classList.contains("add-user-span")) {
        addUserInput(e);
    }

    ////catch changes to assign team members
    //if (e.target.classList.contains('sel-assign-team-member')) {

    //    if (e.target.dataset.userid && e.target.value) {

    //        const teamDto = {
    //            userId: parseInt(e.target.dataset.userid),
    //            teamId: parseInt(e.target.value)
    //        }
    //        teamMembers.push(teamDto);
    //    }
    //}

    ////save members to team
    //if (e.target.id == "team-member-save-btn")
    //    await saveTeamMembers();
}

export async function init() {
    //load initial partial
    let partial = await getAssignTeamPartial();
    let container = document.getElementById('hr-partial-container');
    container.innerHTML = partial;

    //event handlers
    container.addEventListener("click", handleEvents);
    container.addEventListener("keydown", handleEvents);

    //figure out how to add debounced search for user
    
    //
}