export async function getManageRolesPartial() {
    try {
        const response = await axios.get('/Hr/ManageRolesPartial');
        return response.data;
    } catch (error) {
        console.error("error: ManageRolesPartial", error);
        return false;
    }
} 

function getRoles() {
    let results = [];
    const teams = document.querySelectorAll(".add-role-span");

    teams.forEach(t => {

        if (t.textContent.trim() != "Click to add role")
            results.push(t.textContent.trim());
    });

    return results;
}

function addRoleInput(e) {
    const target = e.target;

    // Only act on spans
    if (target.classList.contains("add-role-span") && e.type === "click") {
        const input = document.createElement("input");
        input.type = "text";
        input.placeholder = "Role Name";
        input.className = "add-role-input";
        input.value = target.textContent;

        target.replaceWith(input);
        input.focus();
        input.value = "";

        // Save on blur
        input.addEventListener("blur", () => {
            const span = document.createElement("span");
            span.className = "add-role-span";
            span.textContent = input.value || "Click to add role";
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

function removeRole() {
    const checkedBoxes = document.querySelectorAll("#manage-roles tbody input[type='checkbox']:checked");

    //remove row of checked boxes
    checkedBoxes.forEach(cb => {
        const row = cb.closest("tr");
        if (row)
            row.remove();
    });
}

function addNewRoleRow() {
    const tbody = document.querySelector("#manage-roles > tbody");

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
    addSpan.classList.add("add-role-span");
    addSpan.textContent = "Click to add role";
    td2.appendChild(addSpan);
    tr.appendChild(td2);

    tbody.appendChild(tr);
}

const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

async function saveRoles() {
    let response;
    let payload = getRoles();

    try {
        response = await axios.post('/Hr/SaveRoles', payload, {
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

async function handleEvents(e) {
    if (e.target.id == "role-save-btn") {
        e.preventDefault();
        await saveRoles();
    }

    if (e.target.id == "remove-role-btn")
        removeRole(e);

    if (e.target.id == "add-role-btn")
        addNewRoleRow();

    if (e.target.classList.contains("add-role-span")) {
        addRoleInput(e);
    }

}

export async function init() {
    //load initial partial
    let partial = await getManageRolesPartial();
    document.getElementById("hr-partial-container").innerHTML = partial;

    //const container = document.getElementById('ul-roles');
    //container.querySelectorAll('li')?.forEach(el => el.addEventListener("click", handleEvents));

    let container = document.getElementById('hr-partial-container');
    container.addEventListener("click", handleEvents);
    container.addEventListener("keydown", handleEvents);

    //document.getElementById('add-role-btn')?.addEventListener("click", handleEvents);
    //document.getElementById('role-save-btn')?.addEventListener("click", handleEvents);
}