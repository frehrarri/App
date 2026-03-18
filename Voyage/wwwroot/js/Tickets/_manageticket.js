import { loadModule } from "/js/__moduleLoader.js";

const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
const preChangeValues = new Map();

export async function init(params) {
    removeEventListeners();

    const container = document.querySelector('.main-content');
    const partial = await getManageTicketPartial(params.ticketId);

    if (!container || !partial)
        return;

    container.innerHTML = partial;

    container.addEventListener("click", handleClicks);
    trackEventListener(container, "click", handleClicks);

    container.addEventListener("focusin", handlePreChange);
    trackEventListener(container, "focusin", handlePreChange);

    //hide errors
    container.addEventListener("change", hideErrors);

    if (params?.sectionId) {
        const sectionDropdown = document.getElementById('ticketSectionTitle');
        if (sectionDropdown) {
            sectionDropdown.value = params.sectionId;
        }
    }

    const centerHead = document.getElementById('header-center');
    centerHead.innerHTML = "";

    updateBreadcrumb();

    const deleteBtn = document.getElementById('deleteTicket');
    const ticketId = parseInt(document.getElementById('hdnTicketId').value);
    if (ticketId == 0)
        deleteBtn.classList.add('hidden');

    attachSearchHandler();//attach debounced search
}

async function handleClicks(e) {
    debugger;
    const btn = e.target.closest('button');

    if (btn?.id == "submitTicket") 
        await saveTicket(e);

    if (btn?.id == "deleteTicket")
        await deleteTicket(e);

    if (btn?.id == "undo-button") 
        undo();
    
    //clear autocomplete field on click
    if (btn?.className == "btn-delete-circle") {

        document.getElementById('ticketAssignedTo').value = ''; //reset to blank value
        btn.remove();
        e.stopPropagation(); //prevent event from triggering search handler
    }
        
}

export async function getManageTicketPartial(ticketId) {
    try {
        const response = await axios.get('/Tickets/ManageTicketPartial', {
            params: { ticketId: ticketId }
        });
        return response.data;
    } catch (error) {
        console.error("error: getManageTicketPartial", error);
        return false;
    }
}

function hideErrors() {
    document.querySelectorAll('.text-error').forEach(err => err.classList.add('hidden'));
}

function updateBreadcrumb() {
    const ol = document.querySelector('.breadcrumb');

    ol.innerHTML = '';

    const li1 = document.createElement('li');
    li1.classList.add('breadcrumb-item');
    li1.classList.add('active')

    const a1 = document.createElement('a');
    a1.href = "#";
    a1.textContent = 'Tickets'

    const loadTickets = async () => {
        await loadModule('tickets');
    };

    a1.addEventListener("click", loadTickets);
    trackEventListener(a1, "click", loadTickets);

    li1.appendChild(a1);
    ol.appendChild(li1);
    
    const isEdit = document.getElementById('hdnTicketId')?.value > 0 ?? false;

    if (isEdit) {
        const li2 = document.createElement('li');
        li2.classList.add('breadcrumb-item');
        li2.classList.add('active');

        const a2 = document.createElement('a');
        a2.innerText = 'Ticket';

        const gotoTicket = async () => {
            const ticketId = document.getElementById('hdnTicketId').value;

            const params = {
                ticketId: parseInt(ticketId)
            }
            await loadModule('ticket', params);
        };

        a2.addEventListener("click", gotoTicket);
        trackEventListener(a2, "click", gotoTicket);

        li2.appendChild(a2);
        ol.appendChild(li2);

        const li3 = document.createElement('li');
        li3.classList.add('breadcrumb-item');
        li3.classList.add('active');
        li3.textContent = 'Edit Ticket';

        ol.appendChild(li3);
    }
    else {
        const li2 = document.createElement('li');
        li2.classList.add('breadcrumb-item');
        li2.classList.add('active');
        li2.textContent = 'Add Ticket';

        ol.appendChild(li2);
    }
}

async function saveTicket(e) {
    e.preventDefault();

    const saveBtn = document.getElementById('submitTicket');
    saveBtn.classList.add('disabled');
    saveBtn.disabled = true;

    const isValid = validate();
    if (!isValid) {
        saveBtn.disabled = false;
        return;
    }

    const ticketDTO = {
        TicketId: parseInt(document.getElementById('ticketId').value) || 0,
        SectionTitle: document.getElementById('ticketSectionTitle').selectedOptions[0].text,
        sectionId: document.getElementById('ticketSectionTitle').value,
        Title: document.getElementById('ticketTitle').value,
        Description: document.getElementById('ticketDesc').innerHTML,
        Status: document.getElementById('ticketStatus').value,
        AssignedTo: document.getElementById('ticketAssignedTo').value,
        PriorityLevel: parseInt(document.getElementById('ticketPrioLvl').value),
        DueDate: document.getElementById('ticketDueDate').value ? new Date(document.getElementById('ticketDueDate').value).toISOString() : null,
        ParentTicketId: document.getElementById('ticketParent').value ? parseInt(document.getElementById('ticketParent').value) : null
    }
    let response;
    
    try {
        response = await axios.post('/Tickets/SaveTicket', ticketDTO, {
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            }
        });

        if (response && response.status === 200)
            alert("success");
        else
            alert("error");

        saveBtn.classList.remove('disabled');

        const ticketId = parseInt(document.getElementById('hdnTicketId').value);

        //edit
        if (ticketId > 0) {
            await loadModule("ticket", ticketId);
        }
        //add new
        else {
            await loadModule("tickets");
        }

    } catch (error) {
        saveBtn.disabled = false;
        alert(`error: saveTicket`);
    }
}

async function deleteTicket(e) {
    e.preventDefault();

    const deleteBtn = document.getElementById('deleteTicket');
    deleteBtn.classList.add('disabled');

    let response;
    const ticketId = parseInt(document.getElementById('hdnTicketId').value);

    if (!confirm('Are you sure you want to delete this ticket?')) {
        return;
    }

    try {
        response = await axios.delete('/Tickets/DeleteTicket', {
            params: { ticketId: ticketId },
            headers: { 'X-CSRF-TOKEN': token }
        });

        if (response && response.status === 200) {
            alert("success");
            await loadModule("tickets");
        }
        else
            alert("error");

        deleteBtn.classList.remove('disabled');

    } catch (error) {
        alert("error: deleteTicket");
    }

}

function undo() {
    for (const [input, value] of preChangeValues.entries()) {
        if (input.type === "checkbox") {
            input.checked = value;
        } else if (input.isContentEditable) {
            input.innerHTML = value;
        } else {
            input.value = value;
        }
    }
    preChangeValues.clear(); //prevent dupes
}

function handlePreChange(e) {
    const el = e.target;
    if (el.tagName === "INPUT" || el.tagName === "TEXTAREA"
        || el.tagName === "SELECT" || e.target.matches("div[contenteditable='true']"))
    {

        if (el.type === "checkbox") {
            preChangeValues.set(el, el.checked);
        }
        else {
            preChangeValues.set(el, el.value);
        }

    }
    else if (el.isContentEditable) {
        preChangeValues.set(el, el.innerHTML);
    }
}

function attachSearchHandler() {
    const input = document.getElementById('ticketAssignedTo');

    input.addEventListener('input', (e) => {
        handleSearch(e.target.value, 'users', (insert) => {

            //insert value
            input.value = insert; 

            //hide search list
            const resultList = document.getElementById('search-results'); 
            resultList.style = 'display:none';

            //apply delete button to input
            const btn = document.createElement('button');
            btn.className = 'btn-delete-circle';

            const icon = document.createElement('i');
            icon.classList.add('fa-regular');
            icon.classList.add('fa-circle-xmark');

            btn.appendChild(icon);

            const wrapper = document.getElementById('search-wrapper');
            const results = document.getElementById('search-results');

            wrapper.insertBefore(btn, results);
        });
    });

    trackEventListener(input, 'input', handleSearch);
}

function validate() {
    let isValid = true;

    const title = document.getElementById('ticketTitle').value.trim();
    const description = document.getElementById('ticketDesc').textContent;

    if (!title) {
        document.getElementById('title-error').classList.remove('hidden');
        isValid = false;
    }

    if (!description) {
        document.getElementById('desc-error').classList.remove('hidden');
        isValid = false;
    }

    return isValid;
}