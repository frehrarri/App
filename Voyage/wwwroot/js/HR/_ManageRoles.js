const changeTracker = new Map();

export async function getManageRolesPartial() {
    try {
        const response = await axios.get('/Hr/ManageRolesPartial');
        return response.data;
    } catch (error) {
        console.error("error: ManageRolesPartial", error);
        return false;
    }
}

function addRoleInput(e) {
    const target = e.target;

    if (target.classList.contains("add-role-span") && e.type === "click") {
        // Get the row and original data BEFORE creating the input
        const row = target.closest("tr");
        const roleId = row.dataset.roleId;
        const trackingKey = row.dataset.trackingKey; // Separate tracking key
        const originalName = target.textContent.trim();

        const input = document.createElement("input");
        input.type = "text";
        input.placeholder = "Role Name";
        input.className = "add-role-input";

        // Store original data on the input element
        input.dataset.roleId = roleId;
        input.dataset.trackingKey = trackingKey;
        input.dataset.originalName = originalName;

        target.replaceWith(input);
        input.focus();

        // Set value after focus (for better UX)
        input.value = originalName === "Click to add role" ? "" : originalName;

        // Save on blur
        input.addEventListener("blur", () => {
            const span = document.createElement("span");
            span.className = "add-role-span";
            const newName = input.value.trim();
            span.textContent = newName || "Click to add role";

            input.replaceWith(span);

            if (newName && newName !== "Click to add role") {
                const isNew = !input.dataset.roleId || input.dataset.roleId === "undefined" || input.dataset.roleId === "0";
                const hasChanged = !isNew && newName !== input.dataset.originalName;

                if (isNew || hasChanged) {
                    // Use existing tracking key or create new one
                    const key = input.dataset.trackingKey;

                    if (isNew) {
                        row.dataset.roleId = 0;
                        row.dataset.trackingKey = key;
                        span.dataset.roleId = 0;
                        span.dataset.trackingKey = key;
                    }

                    changeTracker.set(key, {
                        roleId: parseInt(input.dataset.roleId),
                        name: newName,
                        dbChangeAction: 1 // Save (add or update)
                    });
                }
            }
        });

        // Save on Enter
        input.addEventListener("keydown", (ev) => {
            if (ev.key === "Enter") {
                input.blur();
            }
        });
    }
}

function removeRole() {
    const checkedBoxes = document.querySelectorAll("#manage-roles tbody input[type='checkbox']:checked");

    checkedBoxes.forEach(cb => {
        const row = cb.closest("tr");
        if (row) {
            row.remove();

            const trackingKey = row.dataset.trackingKey;
            const roleName = row.querySelector('.add-role-span').textContent.trim();

            if (roleName && roleName !== "Click to add role") {
                const existingChange = changeTracker.get(trackingKey);

                const roleId = row.dataset.roleId;

                // remove unsaved addition from change tracker
                if (existingChange && existingChange.dbChangeAction === 1 && existingChange.roleId === 0) { 
                    changeTracker.delete(trackingKey);
                }

                // prepare for database deletion
                else if (roleId && roleId !== "undefined" && roleId !== "0") {
                    changeTracker.set(trackingKey, { 
                        roleId: parseInt(roleId),
                        name: roleName,
                        dbChangeAction: 2 // Remove
                    });
                }
            }
        }
    });
}

function addNewRoleRow() {
    const tbody = document.querySelector("#manage-roles > tbody");
    
    const tr = document.createElement("tr");
    tr.classList.add("app-table-row");
    tr.dataset.trackingKey = crypto.randomUUID(); //change tracker index
    tr.dataset.roleId = 0; //placeholder for save to replace

    const td1 = document.createElement("td");
    td1.classList.add("app-table-data");
    const checkbox = document.createElement("input");
    checkbox.type = "checkbox";
    td1.appendChild(checkbox);
    tr.appendChild(td1);

    const td2 = document.createElement("td");
    td2.classList.add("app-table-data");
    const addSpan = document.createElement("span");
    addSpan.classList.add("add-role-span");
    addSpan.textContent = "Click to add role";
    td2.appendChild(addSpan);
    tr.appendChild(td2);

    tbody.appendChild(tr);
}

const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

async function saveRoles() {

    if (changeTracker.size === 0) {
        alert('No changes to save');
        return;
    }

    const payload = Array.from(changeTracker.values());

    try {
        const response = await axios.post('/Hr/SaveRoles', payload, {
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            }
        });
        if (response.data) {
            alert("Success")
            changeTracker.clear();
        } else {
            alert("Error");
        }

        return response.data;
    } catch (error) {
        alert("Error");
        console.error("error", error);
        return false;
    }
}

async function handleEvents(e) {
    if (e.target.id === "role-save-btn") {
        e.preventDefault();
        await saveRoles();
    }

    if (e.target.id === "remove-role-btn") {
        removeRole();
    }

    if (e.target.id === "add-role-btn") {
        addNewRoleRow();
    }

    if (e.target.classList.contains("add-role-span")) {
        addRoleInput(e);
    }
}

export async function init() {
    const partial = await getManageRolesPartial();
    document.getElementById("hr-partial-container").innerHTML = partial;

    const container = document.getElementById('hr-partial-container');
    container.addEventListener("click", handleEvents);
    container.addEventListener("keydown", handleEvents);
}