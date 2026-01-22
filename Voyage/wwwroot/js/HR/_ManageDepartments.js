export async function getManageDepartmentsPartial() {
    try {
        const response = await axios.get('/Hr/ManageDepartmentPartial');
        return response.data;
    } catch (error) {
        console.error("error: getManageTicketPartial", error);
        return false;
    }
} 

function addNewDeptRow() {
    const tbody = document.querySelector("#manage-depts > tbody");

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
    addSpan.classList.add("add-dept-span");
    addSpan.textContent = "Click to add deptartment";
    td2.appendChild(addSpan);
    tr.appendChild(td2);

    tbody.appendChild(tr);
}

function addDeptInput(e) {
    const target = e.target;

    // Only act on spans
    if (target.classList.contains("add-dept-span") && e.type === "click") {
        const input = document.createElement("input");
        input.type = "text";
        input.placeholder = "Dept Name";
        input.className = "add-dept-input";
        input.value = target.textContent;

        target.replaceWith(input);
        input.focus();
        input.value = "";

        // Save on blur
        input.addEventListener("blur", () => {
            const span = document.createElement("span");
            span.className = "add-dept-span";
            span.textContent = input.value || "Click to add department";
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

function getDepts() {
    let results = [];
    const teams = document.querySelectorAll(".add-dept-span");

    teams.forEach(t => {

        if (t.textContent.trim() != "Click to add department")
            results.push(t.textContent.trim());
    });

    return results;
}


const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

async function saveDepartments(e) {
    e.preventDefault();
    let response;
    let payload = getDepts();

    try {
        response = await axios.post('/Hr/SaveDepartments', payload, {
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            }
        });

        showSuccess(true);

        return response.data;
    } catch (error) {
        showSuccess(false);
        console.error("error", error);
        return false;
    }
}

function removeDept(e) {
    const checkedBoxes = document.querySelectorAll("#manage-depts tbody input[type='checkbox']:checked");

    //remove row of checked boxes
    checkedBoxes.forEach(cb => {
        const row = cb.closest("tr");
        if (row)
            row.remove();
    });
}



async function handleToggleTabs(e) {
    if (e.target.classList.contains("btn-get-manage-depts")) {
        document.getElementById("dv-allocate-depts").classList.add("hidden");
        document.getElementById("dv-manage-departments").classList.remove("hidden");
    } else if (e.target.classList.contains("btn-get-allocate-depts")) {
        document.getElementById("dv-allocate-depts").classList.remove("hidden");
        document.getElementById("dv-manage-departments").classList.add("hidden");
    }

}

async function handleEvents(e) {

    await handleToggleTabs(e);

    //save teams
    if (e.target.id == "dept-save-btn") {
        await saveDepartments(e);
    }

    //remove team
    if (e.target.id == "remove-dept-btn")
        removeDept(e);

    //add new team row
    if (e.target.id == "add-dept-btn")
        addNewDeptRow();

    //input control for adding team
    if (e.target.classList.contains("add-dept-span")) {
        addDeptInput(e);
    }

    ////save members to team
    //if (e.target.id == "team-member-save-btn")
    //    await saveTeamMembers();

}


export async function init() {
    //load initial partial
    let partial = await getManageDepartmentsPartial();
    document.getElementById("hr-partial-container").innerHTML = partial;

    //event handlers
    let container = document.getElementById('hr-partial-container');
    container.addEventListener("click", handleEvents);
    container.addEventListener("keydown", handleEvents);

   
}