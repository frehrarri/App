

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

    if (target.classList.contains("add-user-span") && e.type === "click") {

        //need a wrapper to append children to
        const wrapper = document.createElement("div");
        wrapper.className = "autocomplete-wrapper";

        const input = document.createElement("input");
        input.type = "text";
        input.placeholder = "User Name";
        input.className = "add-user-input";
        input.value = "";
        input.dataset.uid = crypto.randomUUID();

        // Replace span with wrapper, then add input
        target.replaceWith(wrapper);
        wrapper.appendChild(input);

        input.focus();

        input.addEventListener("blur", () => {
            const ul = document.querySelector(".autocomplete-list[data-for='" + input.dataset.uid + "']");
            if (ul) ul.classList.remove("show");

            //const span = document.createElement("span");
            //span.className = "add-user-span";
            //span.textContent = input.value || "Click to add user";
            //wrapper.replaceWith(span); // remove wrapper + input safely
        });

        input.addEventListener("keydown", (ev) => {
            if (ev.key === "Enter") input.blur();
        });
    }
}

function removeUser(e) {
    const checkedBoxes = document.querySelectorAll("#tbl-allocate-personnel tbody input[type='checkbox']:checked");

    //remove row of checked boxes
    checkedBoxes.forEach(cb => {
        const row = cb.closest("tr");

        if (row) {
            changeTracker.push({
                id: row.dataset.userid,
                saveaction: 2 //remove
            });

            row.remove();
        }
            
    });

   
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

function insertSearchResults(user) {
    let tr = document.createElement('tr');
    tr.className = 'app-table-row';
    tr.dataset.userid = user.id;

    let checkbox = document.createElement('td');
    checkbox.className = 'app-table-data'

    let cbx = document.createElement('input');
    cbx.type = 'checkbox'

    checkbox.appendChild(cbx);
    tr.appendChild(checkbox);

    let firstName = document.createElement('td');
    firstName.textContent = user.firstname;
    firstName.className = 'app-table-data';
    tr.appendChild(firstName);

    let lastName = document.createElement('td');
    lastName.textContent = user.lastname;
    lastName.className = 'app-table-data';
    tr.appendChild(lastName);

    let username = document.createElement('td');
    username.textContent = user.username;
    username.className = 'app-table-data';
    tr.appendChild(username);

    let email = document.createElement('td');
    email.textContent = user.email;
    email.className = 'app-table-data';
    tr.appendChild(email);

    let row = document.querySelector('.autocomplete-wrapper').parentElement.parentElement;
    row.replaceWith(tr);

    changeTracker.push({
        id: user.id,
        saveaction: 1 //add
    });
}

function saveTeamMembers() {
    
    let teamKey = document.getElementById('hdn-team-key');
}

const changeTracker = [{

}];

async function handleEvents(e) {

    ////save teams
    //if (e.target.id == "team-save-btn") {
    //    e.preventDefault();
    //    await saveTeams();
    //}

    if (e.type === "click") {
        //remove user
        if (e.target.id == "remove-user-btn")
            removeUser(e);

        //add new user row
        if (e.target.id == "add-user-btn")
            addNewRow();

        //input control for adding team member
        if (e.target.classList.contains("add-user-span"))
            addUserInput(e);

    }
    else if (e.type === "input") {
        if (e.target.classList.contains("add-user-input")) {
            const resultsContainer = attachAutoComplete(e);
            addUserSearchEventListener("dv-allocate-personnel", e.target, resultsContainer, (user) => insertSearchResults(user));
        }
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
    container.addEventListener("input", handleEvents);

    //let tbody = container.querySelector('#tbl-allocate-personnel > tbody');
    //tbody.addEventListener("change", handleEvents);
}