import { loadModule } from "/js/__moduleLoader.js";

const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
const changeTracker = new Map();

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
        const row = e.target.parentElement.parentElement;

        let key = row.dataset.key;
        if (!key)
            key = crypto.randomUUID();

        const input = document.createElement("input");
        input.type = "text";
        input.placeholder = "Dept Name";
        input.className = "add-dept-input";
        input.value = target.textContent.trim();
        input.dataset.key = key;

        target.replaceWith(input);
        input.focus();
        input.value = "";

        // Save on blur
        input.addEventListener("blur", () => {
            const span = document.createElement("span");
            span.className = "add-dept-span";
            span.textContent = input.value || "Click to add department";
            span.dataset.key = key;

            input.replaceWith(span);


            changeTracker.set(key, {
                name: span.textContent,
                deptKey: null, //placeholder for a new entry that will be assigned by db
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


async function saveDepartments(e) {
    e.preventDefault();
    let response;
    let payload = Array.from(changeTracker.values());

    debugger;
    
    try {
        response = await axios.post('/Hr/SaveDepartments', payload, {
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            }
        });

        if (response && response.status == 200) {

            //did not add any records so there is nothing to hyperlink
            if (response.data.length > 0) {
                //hyperlink results
                let dept = "";
                let index = 0;

                let addedEntries = [...changeTracker.entries()].filter(([key, value]) => value.dbChangeAction === 1);

                for (let [key, value] of addedEntries) {
                    dept = document.querySelector(`.add-dept-span[data-key='${key}']`);

                    let anchortag = document.createElement('a');
                    anchortag.href = "#";
                    anchortag.classList.add('goto-assign-dept')
                    anchortag.textContent = dept.textContent.trim();
                    anchortag.dataset.key = key;

                    let deptKey = response.data[index];
                    anchortag.dataset.deptKey = deptKey;

                    dept.replaceWith(anchortag);
                    index++;
                }
            }

            alert("Success");
        } else {
            alert("Error saving");
        }
    } catch (error) {
        showSuccess(false);
        console.error("error", error);
        return false;
    }
}

async function removeDept(e) {
    if (!confirm("Remove departments?")) {
        return;
    }

    const checkedBoxes = document.querySelectorAll("#manage-depts tbody input[type='checkbox']:checked");

    //remove row of checked boxes
    checkedBoxes.forEach(cb => {
        debugger;
        const row = cb.closest("tr");
        if (row) {
            let key = row.dataset.key;

            // remove unsaved addition from change tracker
            const existingChange = changeTracker.get(key);
            if (existingChange && existingChange.dbChangeAction === 1)
                changeTracker.delete(key)

            // prepare for database deletion
            else if (row.dataset.deptkey != null)
                changeTracker.set(key, {
                    name: row.childNodes[1].textContent,
                    deptKey: row.dataset.deptkey,
                    dbChangeAction: 2 //remove
                });

            row.remove();
        }
    });

    await saveDepartments(e);
}


async function handleEvents(e) {

    //save teams
    if (e.target.id == "dept-save-btn") {
        await saveDepartments(e);
    }

    //remove team
    if (e.target.id == "remove-dept-btn")
        await removeDept(e);

    //add new team row
    if (e.target.id == "add-dept-btn")
        addNewDeptRow();

    //input control for adding team
    if (e.target.classList.contains("add-dept-span")) {
        addDeptInput(e);
    }

    if (e.type == "click" && e.target.classList.contains("goto-assign-dept")) {
        await loadModule("assignDepartment");
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