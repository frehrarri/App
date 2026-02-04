
const autocompleteState = new WeakMap();

async function getGridControlPartial() {
    try {
        const response = await axios.get('/Common/GridControlPartial');
        return response.data;
    } catch (error) {
        console.error("error: getManageTicketPartial", error);
        return false;
    }
}

function addNewRow(newId) {
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
}

function addUserInput(e, newId, controlType, changeTracker) {
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
        input.placeholder = "User Name";
        input.className = `add-${newId}-input`;
        input.value = "";
        input.dataset.uid = uid;

        // Replace span with wrapper, then add input
        target.replaceWith(wrapper);
        wrapper.appendChild(input);

        input.focus();
        
        input.addEventListener("input", () => {
            debounceSearch(input, controlType, uid, changeTracker);
        });

        input.addEventListener("blur", () => {
            const ul = document.querySelector(".autocomplete-list[data-for='" + input.dataset.uid + "']");
            if (ul)
                ul.classList.remove("show");

            const span = document.createElement("span");
            span.className = `add-${newId}-span`;
            span.textContent = input.value || "Click to add user";
            span.dataset.uid = uid;

            wrapper.replaceWith(span); // remove wrapper + input safely
        });

        input.addEventListener("keydown", (ev) => {
            if (ev.key === "Enter")
                input.blur();
        });
    }
}

function handleChangeTrackerParams(controlType, row) {
    let params = {};
    
    switch (controlType) {
        case 1:
            break;
        case 2:
            params = {
                teamKey: row.teamKey ?? row.dataset.teamKey
            }
            break;
        default:
            break;
    }

    return params;
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
            let uid = row.dataset.uid ?? row.dataset.key;
       
            // remove unsaved addition from change tracker
            const existingChange = changeTracker.get(uid);
            if (existingChange && existingChange.dbChangeAction === 1)
                changeTracker.delete(uid)
            
            else {
                let args = handleChangeTrackerParams(controlType, row);
                args.dbChangeAction = 2;
                changeTracker.set(uid, args);
                debugger;
                
            }

            row.remove();
        }
    });

    saveCallback(e, changeTracker);
}

async function hydrateGrid(headerList, newId, rows, key, controlType) {
    const headers = headerList;
    const data = rows;
    
    //load
    let partial = await getGridControlPartial();
    document.getElementById(`${newId}-grid-container`).innerHTML = partial;

    const table = document.getElementById("tbl-newid");
    table.dataset.key = key;
    table.id = `tbl-${newId}`;

    renameIds(newId);
    let eventWrapper = document.getElementById(`dv-${newId}`);
    eventWrapper.dataset.key = key;

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
        debugger;
        let key = crypto.randomUUID();

        const tr = document.createElement('tr');

        handleGetDataAttr(tr, row, controlType);

        tr.classList.add('app-table-row');
        tr.dataset.key = key;

        const tdcbx = document.createElement('td');
        tdcbx.classList.add('app-table-data');

        const cbx = document.createElement('input');
        cbx.type = 'checkbox';

        tdcbx.appendChild(cbx);
        tr.appendChild(tdcbx);

        if (row) {
            handleControlData(controlType, tr, row);

            //const td = document.createElement('td');
            //td.classList.add('app-table-data');
            //td.textContent = row;
            //tr.appendChild(td);
        }

        tbody.appendChild(tr);
    })
}

//ensure these match the data attributes in handleSaveControlTypeDataAttr
function handleGetDataAttr(tr, row, controlType) {
    
    switch (controlType) {
        case 1:
            break;
        case 2:
            tr.dataset.teamKey = row.teamKey;
            break;
        default:
            break;
    }
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

function insertSearchResults(row, controlType, uid, changeTracker) {
    let tr = document.createElement('tr');
    tr.className = 'app-table-row';
    tr.dataset.uid = uid;

    if (row.length == 0)
        return;

    handleSaveControlTypeDataAttr(controlType, tr, row);

    //create checkbox
    let checkbox = document.createElement('td');
    checkbox.className = 'app-table-data'

    let cbx = document.createElement('input');
    cbx.type = 'checkbox'

    checkbox.appendChild(cbx);
    tr.appendChild(checkbox);
 
    handleControlData(controlType, tr, row);

    let wrapper = document.querySelector('.autocomplete-wrapper').parentElement.parentElement;
    wrapper.replaceWith(tr);

    let args = handleChangeTrackerParams(controlType, row);
    args.dbChangeAction = 1; //add
    changeTracker.set(uid, args);
}

function handleControlData(controlType, tr, row) {
    switch (controlType) {
        case 1: //Users control
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
        case 2: //Teams control
            let teamName = document.createElement('td');
            teamName.textContent = row.name ?? row.teamName;
            teamName.className = 'app-table-data';
            teamName.dataset.teamKey = tr.dataset.teamKey;
            tr.appendChild(teamName);
            break;
        default:
            break;
    }
}

function handleSaveControlTypeDataAttr(controlType, tr, row)
{
    switch (controlType) {
        case 1:
            tr.dataset.userid = row.id;
            tr.dataset.employeeid = row.employeeid;
            tr.dataset.roleid = row.roleid;
            break;
        case 2:
            tr.dataset.teamKey = row.teamKey;
            //tr.dataset.userid = row.id;
            //tr.dataset.employeeid = row.employeeid;
            //tr.dataset.roleid = row.roleid;
            break;
        default:
            break;
    }
        

}

function handleHeaders(params) {
    const type = params.controlType;

    if (!type)
        return;

    switch (type) {
        case 1: //User control
            return ["Last Name", "First Name", "Username", "Email"];
        case 2: //Teams control
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
        default:
            break;
    }
}

function formatSearchResults(controlType, r) {
    let result = "";
    switch (controlType) {
        case 1:
            return `${r.lastname}, ${r.firstname} [${r.username}]`;
        case 2:
            return `${r.name}`;
        default:
            break;
    }
}


async function handleSearchInput(e, controlType, uid, changeTracker) {
    const input = e.target;
    const query = input.value.trim();

    if (query.length < 2)
        return;

    //attach auto complete control event listener
    const ul = attachAutoComplete(e);

    let url = handleSearchUrl(controlType);

    const res = await axios.get(url, {
            params: { query }
        });

    ul.innerHTML = "";

    res.data.forEach(r => {
        
        const li = document.createElement("li");
        li.textContent = formatSearchResults(controlType, r);

        li.onclick = () => insertSearchResults(r, controlType, uid, changeTracker);
        ul.appendChild(li);
    });

}

function debounceSearch(input, controlType, uid, changeTracker) {
    let state = autocompleteState.get(input);

    if (!state) {
        state = { timer: null };
        autocompleteState.set(input, state);
    }

    clearTimeout(state.timer);

    state.timer = setTimeout(() => {
        handleSearchInput({ target: input }, controlType, uid, changeTracker);
    }, 300);
}


async function handleEvents(e, newId, saveCallback, controlType, changeTracker) {
    if (e.type === "click") {
        //remove user
        if (e.target.id == `${newId}-remove-btn`) {
            remove(e, newId, saveCallback, changeTracker, controlType);
        }
            

        //add row
        if (e.target.id == `${newId}-add-btn`)
            addNewRow(newId);

        //input control for adding data
        if (e.target.classList.contains(`add-${newId}-span`))
            addUserInput(e, newId, controlType, changeTracker);

        //handle save events 
        if (e.target.id == `${newId}-save-btn`) {
            await saveCallback(e, changeTracker);
        }
            
    }

}

//how to use:
//add element with id `${newId}-grid-container` where we want to render the grid
//call loadModule("gridControl", params) from init function of page

//parameters:
//newId = to be reused in string interpolation for unique ids
//headers = list of headers to be inserted in thead
//rows = list of data to be inserted in tbody
//saveCallback = a callback function required for any saving

export async function init(params) {

    const changeTracker = new Map();
    const key = crypto.randomUUID();

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

    //basic control requires user input headers
    let headers = [];
    if (params.headers && params.headers.length > 0)
        headers = params.headers;

    //default to basic grid control
    if (params.controlType) 
        headers = handleHeaders(params);

    await hydrateGrid(headers, newId, params.rows, key, controlType); 
    
    //event handlers
    const container = document.querySelector(`#dv-${newId}[data-key='${key}']`);
    debugger;
    container?.addEventListener("click", e => handleEvents(e, newId, params.saveCallback, controlType, changeTracker));
    container?.addEventListener("keydown", e => handleEvents(e, newId, null, controlType, changeTracker));
}

