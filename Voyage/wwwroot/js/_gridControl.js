
//how to use:
//add element with id `${newId}-grid-container` where we want to render the grid
//call loadModule("gridControl", params) from init function of page

//parameters:
//newId = to be reused in string interpolation for unique ids
//headers = list of headers to be inserted in thead
//rows = list of data to be inserted in tbody
//saveCallback = a callback function required for any saving
//redirectClass = class for event listener for hyperlinking and redirecting on click

export async function init(params) {

    const changeTracker = new Map();
    const uid = crypto.randomUUID();

    if (!params) {
        console.log("need to pass parameters");
        return;
    }

    if (!params.newId) {
        console.log("need unique text property newId in params");
        return;
    }

    //control type must be 1 or greater OR have headers.length > 1
    let isValid = params.controlType >= 1 || (params.headers && params.headers.length > 0)

    if (!isValid) {
        console.log("must enter headers manually or use non basic control type");
        return;
    }

    let controlType = params.controlType ?? 0;
    let newId = params.newId;
    let redirectClass = params.redirectClass;

    //basic control requires user input headers
    let headers = [];
    if (params.headers && params.headers.length > 0)
        headers = params.headers;

    //default to basic grid control
    if (params.controlType)
        headers = handleHeaders(params);

    await hydrateGrid(headers, newId, params.rows, uid, controlType, redirectClass);

    const container = document.querySelector(`#dv-${newId}[data-uid='${uid}']`);

    // Store state on the container element for easy access
    const autocompleteState = new WeakMap();
    container.autocompleteState = autocompleteState;

    //event handlers
    container?.addEventListener("click", e => handleEvents(e, newId, params.saveCallback, controlType, changeTracker, autocompleteState));
    container?.addEventListener("keydown", e => handleEvents(e, newId, null, controlType, changeTracker, autocompleteState));

    updateSaveButtonState(newId, changeTracker);
}

async function getGridControlPartial() {
    try {
        const response = await axios.get('/Common/GridControlPartial');
        return response.data;
    } catch (error) {
        console.error("error: getManageTicketPartial", error);
        return false;
    }
}

function addNewRow(newId, changeTracker) {
    const tbody = document.querySelector(`#tbl-${newId} > tbody`);

    const tr = document.createElement("tr");
    tr.classList.add("app-table-row");

    let uid = tr.dataset.uid;
    if (!uid)
        tr.dataset.uid = crypto.randomUUID();

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
    addSpan.classList.add(`add-${newId}-span`);
    addSpan.textContent = `Click here`;
    td2.appendChild(addSpan);
    tr.appendChild(td2);

    tbody.appendChild(tr);

    updateSaveButtonState(newId, changeTracker);
}

function addUserInput(e, newId, controlType, changeTracker, autocompleteState) {
    const target = e.target;

    if (target.classList.contains(`add-${newId}-span`) && e.type === "click") {
        const row = e.target.parentElement.parentElement;

        let uid = row.dataset.uid;
        if (!uid)
            uid = crypto.randomUUID();

        //need a wrapper to append children to
        const wrapper = document.createElement("div");
        wrapper.className = "autocomplete-wrapper";

        const input = document.createElement("input");
        input.type = "text";
        input.className = `add-${newId}-input`;
        input.value = "";
        input.dataset.uid = uid;

        // Replace span with wrapper, then add input
        target.replaceWith(wrapper);
        wrapper.appendChild(input);

        input.focus();

        // Disable save button when input is opened
        updateSaveButtonState(newId, changeTracker)
        
        //set up autocomplete
        if (usesAutocomplete(controlType)) {
            input.addEventListener("input", () => {
                debounceSearch(input, controlType, uid, changeTracker, autocompleteState);
            });

            autocompleteState.set(input, { selectionMade: false });

            input.addEventListener("blur", () => {
                // Delay to allow autocomplete click to register
                setTimeout(() => {
                    const ul = document.querySelector(".autocomplete-list[data-for='" + input.dataset.uid + "']");
                    if (ul)
                        ul.classList.remove("show");

                    const state = autocompleteState.get(input);
                    const selectionMade = state?.selectionMade || false;

                    const span = document.createElement("span");
                    span.className = `add-${newId}-span`;

                    // Always revert to placeholder unless selection was made
                    span.textContent = selectionMade ? input.value : "Click here";
                    span.dataset.uid = uid;

                    wrapper.replaceWith(span); // remove wrapper + input safely

                    // Clean up autocomplete state
                    autocompleteState.delete(input);

                    // Re-enable save button after input is closed
                    updateSaveButtonState(newId, changeTracker);
                }, 150);
            });

            input.addEventListener("keydown", (ev) => {
                if (ev.key === "Enter") {
                    // For autocomplete, Enter should not save the query text
                    const state = autocompleteState.get(input);
                    if (state) state.selectionMade = false;
                    input.blur();
                }
                if (ev.key === "Escape") {
                    const state = autocompleteState.get(input);
                    if (state) state.selectionMade = false;
                    input.blur();
                }
            });

        }
        else {
            // For non-autocomplete controls, just save whatever the user types
            input.addEventListener("blur", () => {
                const span = document.createElement("span");
                span.className = `add-${newId}-span`;
                // Keep the user's input or revert to placeholder if empty
                span.textContent = input.value.trim() || "Click here";
                span.dataset.uid = uid;

                wrapper.replaceWith(span);
             
                // If user entered text, prepare it for saving
                if (input.value.trim()) {
                    let args = handleChangeTrackerParams(controlType, input.value.trim());
                    args.dbChangeAction = 1; // add
                    changeTracker.set(uid, args);
                }

                // Re-enable save button after input is closed
                updateSaveButtonState(newId, changeTracker);
            });

            input.addEventListener("keydown", (ev) => {
                if (ev.key === "Enter" || ev.key === "Escape") {
                    input.blur();
                }
            });
        }
    }
}



function renameIds(newId) {
    const eventWrapper = document.getElementById('dv-newid');
    eventWrapper.id = `dv-${newId}`;

    const btnContainer = document.getElementById('newid-btns-container');
    btnContainer.id = `${newId}-btns-container`;

    const addBtn = document.getElementById('newid-add-btn');
    addBtn.id = `${newId}-add-btn`;

    const removeBtn = document.getElementById('newid-remove-btn');
    removeBtn.id = `${newId}-remove-btn`;

    const saveBtn = document.getElementById('newid-save-btn');
    saveBtn.id = `${newId}-save-btn`;
}

function remove(e, newId, saveCallback, changeTracker, controlType) {
    if (!confirm("Are you sure?")) {
        return;
    }
    const checkedBoxes = document.querySelectorAll(`#tbl-${newId} tbody input[type='checkbox']:checked`);

    //remove row of checked boxes
    checkedBoxes.forEach(cb => {
        
        const row = cb.closest("tr");
        if (row) {
            /*let uid = row.dataset.uid ?? row.dataset.key;*/
            let uid = row.dataset.uid
       
            // remove unsaved addition from change tracker
            const existingChange = changeTracker.get(uid);
            if (existingChange && existingChange.dbChangeAction === 1)
                changeTracker.delete(uid)
            
            else {
                let args = handleChangeTrackerParams(controlType, row);
                args.dbChangeAction = 2;
                changeTracker.set(uid, args);
            }

            row.remove();
        }
    });

    // Update save button state after removing rows
    updateSaveButtonState(newId, changeTracker);

    saveCallback(e, changeTracker);
}

async function hydrateGrid(headerList, newId, rows, uid, controlType, redirectClass) {
    const headers = headerList;
    const data = rows;
    
    //load
    let partial = await getGridControlPartial();
    document.getElementById(`${newId}-grid-container`).innerHTML = partial;

    const table = document.getElementById("tbl-newid");
    table.dataset.uid = uid;
    table.id = `tbl-${newId}`;

    renameIds(newId);
    let eventWrapper = document.getElementById(`dv-${newId}`);
    eventWrapper.dataset.uid = uid;

    //add headers
    const headerRow = table.querySelector('thead tr');

    headers.forEach(h => {

        const th = document.createElement("th");
        th.classList.add('app-table-header');
        th.textContent = h;

        headerRow.appendChild(th);
    })

    //add data
    const tbody = table.querySelector('tbody');
    data.forEach(row => {
        let uid = crypto.randomUUID();
        const tr = document.createElement('tr');

        //set attributes on load
        handleDataAttr(tr, row, controlType);

        tr.classList.add('app-table-row');
        tr.dataset.uid = uid;

        const tdcbx = document.createElement('td');
        tdcbx.classList.add('app-table-data');

        const cbx = document.createElement('input');
        cbx.type = 'checkbox';

        tdcbx.appendChild(cbx);
        tr.appendChild(tdcbx);

        //set table data columns
        if (row) {
            handleControlData(controlType, tr, row);
        }

        tbody.appendChild(tr);
    })

    if (controlType == 0)
        hyperlinkNames(redirectClass);
}

function attachAutoComplete(e) {
    const input = e.target;
    if (!input.isConnected) return null;

    // reuse UL if it already exists
    let ul = document.querySelector(".autocomplete-list[data-for='" + input.dataset.uid + "']");

    if (!ul) {
        ul = document.createElement("ul");
        ul.className = "autocomplete-list";
        ul.dataset.for = input.dataset.uid;

        // prevent blur when clicking results
        ul.addEventListener("mousedown", ev => ev.preventDefault());

        document.body.appendChild(ul);
    }

    // position UL under the input
    const rect = input.getBoundingClientRect();
    ul.style.position = "fixed";
    ul.style.top = `${rect.bottom + 4}px`;
    ul.style.left = `${rect.left}px`;
    ul.style.zIndex = 99999;

    ul.classList.add("show");
    return ul;
}

function insertSearchResults(row, controlType, uid, changeTracker, autocompleteState) {
    let tr = document.createElement('tr');
    tr.className = 'app-table-row';
    tr.dataset.uid = uid;

    if (row.length == 0)
        return;

    handleDataAttr(tr, row, controlType);

    //create checkbox
    let checkbox = document.createElement('td');
    checkbox.className = 'app-table-data'

    let cbx = document.createElement('input');
    cbx.type = 'checkbox'

    checkbox.appendChild(cbx);
    tr.appendChild(checkbox);

    //set table data columns
    handleControlData(controlType, tr, row);

    let wrapper = document.querySelector('.autocomplete-wrapper');

    //handle autcomplete
    const input = wrapper?.querySelector('input');
    if (input && autocompleteState) {
        const state = autocompleteState.get(input);
        if (state) state.selectionMade = true;
    }

    let wrapperParent = wrapper.parentElement.parentElement;
    wrapperParent.replaceWith(tr);

    //prepare for save
    let args = handleChangeTrackerParams(controlType, row);
    args.dbChangeAction = 1; //add
    changeTracker.set(uid, args);

    // Extract newId from the input class name (e.g., "add-myGrid-input" -> "myGrid")
    const inputClass = input?.className || '';
    const match = inputClass.match(/add-(.+)-input/);
    if (match) {
        const newId = match[1];
        // Update save button state after inserting result
        updateSaveButtonState(newId);
    }

}

function usesAutocomplete(controlType) {
    if (controlType == 0)
        return false;
    else
        return true;
}

//row.param = on retrieve
//row.dataset.param = on remove
function handleChangeTrackerParams(controlType, row) {
    let params = {};
   
    switch (controlType) {
        case 1: //get all users
        case 4: //get unassigned department users
            params = {
                employeeid: row.employeeid ?? row.dataset.employeeid,
                roleid: row.roleid ?? row.dataset.roleid
            }
            break;
        case 2: //get all teams
        case 3: //get unassigned department teams
            params = {
                teamKey: row.teamKey ?? row.dataset.teamKey
            }
            break;
        case 0:
        default:
            let name = row;
            let datakey = null;

            if (row.dataset) {
                name = row.dataset.name;
                datakey = row.dataset.datakey;
            }

            params = {
                name: name,
                datakey: datakey
            }
            break;
    }

    return params;
}

//apply data attributes for get and remove
function handleDataAttr(tr, row, controlType) {

    switch (controlType) {
        
        case 1: //get all users
        case 4: //get unassigned department users
            tr.dataset.employeeid = row.employeeid;
            tr.dataset.roleid = row.roleid;
            break;
        case 2:   //get all teams
        case 3:   //get unassigned department teams
            tr.dataset.teamKey = row.teamKey;
            break;
        case 0://basic control
        default:
            tr.dataset.name = row.name;
            tr.dataset.datakey = row.datakey;
            break;
    }
}



//grid rows
function handleControlData(controlType, tr, row) {
    switch (controlType) {
        case 1: //get all users
        case 4: //get unassigned department users
            let firstName = document.createElement('td');
            firstName.textContent = row.firstname;
            firstName.className = 'app-table-data';
            tr.appendChild(firstName);

            let lastName = document.createElement('td');
            lastName.textContent = row.lastname;
            lastName.className = 'app-table-data';
            tr.appendChild(lastName);

            let username = document.createElement('td');
            username.textContent = row.username;
            username.className = 'app-table-data';
            tr.appendChild(username);

            let email = document.createElement('td');
            email.textContent = row.email;
            email.className = 'app-table-data';
            tr.appendChild(email);
            break;
        case 2: //get all teams
        case 3: //get unassigned department teams
            let teamName = document.createElement('td');
            teamName.textContent = row.name ?? row.teamName;
            teamName.className = 'app-table-data';
            teamName.dataset.teamKey = tr.dataset.teamKey;
            tr.appendChild(teamName);
            break;
        case 0: //basic control
        default:
            let name = document.createElement('td');
            name.textContent = row.name;
            name.className = 'app-table-data';
            name.dataset.datakey = tr.dataset.datakey;
            tr.appendChild(name);
            break;
    }
}

function handleHeaders(params) {
    const type = params.controlType;

    if (!type)
        return;

    switch (type) {
        case 1: //get all users
        case 4: //get unassigned department users
            return ["Last Name", "First Name", "Username", "Email"];
        case 2: //get all teams
        case 3: //get unassigned department teams
            return ["Teams"]
        default:
            break;
    }
}

function handleSearchUrl(controlType) {
    switch (controlType) {
        case 1:
            return "/SearchService/Users";
        case 2:
            return "/SearchService/Teams";
        case 3:
            return "/SearchService/UnassignedDeptTeams";
        case 4:
            return "/SearchService/UnassignedDeptUsers";
        default:
            break;
    }
}

//params for our debounced search lookups
function handleSearchParams(controlType) {
    switch (controlType) {
        case 1://get all users
        case 2://get all teams
        case 3://get unassigned department teams
            return;
        case 4://get unassigned department users
            return document.getElementById('hdn-dept-key');
        default:
            break;
    }
}

//format autocomplete popup
function formatSearchResults(controlType, r) {
    switch (controlType) {
        case 1: //get all users
        case 4: //get unassigned department users
            return `${r.lastname}, ${r.firstname} [${r.username}]`;
        case 2: //get all teams
        case 3: //get unassigned department teams
            return `${r.name}`;
        default:
            break;
    }
}

async function handleSearchInput(e, controlType, uid, changeTracker, autocompleteState) {
    const input = e.target;
    const query = input.value.trim();

    if (query.length < 2)
        return;

    //attach auto complete control event listener
    const ul = attachAutoComplete(e);

    let url = handleSearchUrl(controlType);
    let searchParam = handleSearchParams(controlType);

    let res = "";
    if (searchParam) {
        res = await axios.get(url, {
            params: {
                query: query,
                parameter: searchParam
            }
        });
    } else {
        res = await axios.get(url, {
            params: { query }
        });
    }
   

    ul.innerHTML = "";

    res.data.forEach(r => {
        
        const li = document.createElement("li");
        li.textContent = formatSearchResults(controlType, r);

        li.onclick = () => insertSearchResults(r, controlType, uid, changeTracker, autocompleteState);
        ul.appendChild(li);
    });

}

function debounceSearch(input, controlType, uid, changeTracker, autocompleteState) {
    let state = autocompleteState.get(input);

    if (!state) {
        state = { timer: null };
        autocompleteState.set(input, state);
    }

    clearTimeout(state.timer);

    state.timer = setTimeout(() => {
        handleSearchInput({ target: input }, controlType, uid, changeTracker, autocompleteState);
    }, 300);
}


async function handleEvents(e, newId, saveCallback, controlType, changeTracker, autocompleteState) {
    if (e.type === "click") {
        //remove
        if (e.target.id == `${newId}-remove-btn`) 
            remove(e, newId, saveCallback, changeTracker, controlType);

        //add row
        if (e.target.id == `${newId}-add-btn`)
            addNewRow(newId, changeTracker);

        //input control for adding data
        if (e.target.classList.contains(`add-${newId}-span`)) {
           
            addUserInput(e, newId, controlType, changeTracker, autocompleteState);
        }
            

        //handle save events 
        if (e.target.id == `${newId}-save-btn`) 
            await saveCallback(e, changeTracker, newId);
    }
}

function updateSaveButtonState(newId, changeTracker) {
    const saveBtn = document.getElementById(`${newId}-save-btn`);
    if (!saveBtn) return;

    // Check if there are any additions in the change tracker (dbChangeAction === 1)
    let hasPendingAdditions = false;
    if (changeTracker) {
        for (let [key, value] of changeTracker.entries()) {
            if (value.dbChangeAction === 1) {
                hasPendingAdditions = true;
                break;
            }
        }
    }

    // Check if there are any open input fields
    const hasOpenInputs = document.querySelector(`.autocomplete-wrapper`) !== null;

    // Enable only if there are pending additions AND no open inputs
    const shouldDisable = !hasPendingAdditions || hasOpenInputs;

    if (shouldDisable) {
        saveBtn.setAttribute('disabled', 'disabled');

    } else {
        saveBtn.removeAttribute('disabled');

    }
}

export function hyperlinkNames(redirectClass) {
    if (!redirectClass)
        return;

    document.querySelectorAll("tbody tr").forEach(tr => {
        const td = tr.querySelector("td:nth-child(2)"); // skip checkbox column
        if (!td) return;

        const link = document.createElement("a");
        link.href = "#"
        link.textContent = td.textContent;
        link.dataset.datakey = tr.dataset.datakey
        /*link.dataset.uid = tr.dataset.uid*/
        link.classList.add(`${redirectClass}`);

        td.textContent = "";
        td.appendChild(link);
    });

}