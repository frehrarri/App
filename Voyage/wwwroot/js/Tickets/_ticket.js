import { loadModule } from "/js/__moduleLoader.js";

const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
const ticketDetailsMap = new Map();

export async function init(ticketid) {
    removeEventListeners();

    let partial = await getTicketPartial(ticketid);
    const container = document.querySelector('.main-content');

    if (!container || !partial)
        return;

    container.innerHTML = partial;

    container.addEventListener("click", handleClicks);
    trackEventListener(container, "click", handleClicks);

    updateBreadcrumb();
    anonymizeTicketDetails();
}

async function handleClicks(e) {
    
    const btn = e.target.closest('button');
    if (!btn)
        return;

    if (btn.id == 'add-note-button')
        await addNote(e);
    else if (btn.classList.contains('edit-note-btn'))
        await editNote(e);
    else if (btn.classList.contains('save-edit-note-btn'))
        await saveEditNote(e);
    else if (btn.id == 'edit-ticket-button') {
        const ticketId = document.getElementById('hdnTicketId').value;

        const module = await loadModule("manageTicket");
        const partial = await module.getManageTicketPartial(ticketId, null);

        if (partial) {
            document.querySelector('.main-content').innerHTML = partial;
        }
    }
    else if (btn.id == 'delete-ticket-button')
        await deleteTicket();
    
}

export async function getTicketPartial(ticketId, ticketVersion) {
    try {
        const response = await axios.get('/Tickets/TicketPartial', {
            params: { ticketId: ticketId, ticketVersion: ticketVersion }
        });

        return response.data;
    } catch (error) {
        console.error("error: getTicketPartial", error);
        return false;
    }
}

function anonymizeTicketDetails() {
    document.querySelectorAll('[data-ticketdetailsid]').forEach(el => {
        const realId = el.dataset.ticketdetailsid;
        const anonId = getAnonymousId(realId);
        el.dataset.ticketdetailsid = anonId;
    });
}

function getRealId(anonId) {
    return [...ticketDetailsMap.entries()].find(([, uuid]) => uuid === anonId)?.[0];
}

function getAnonymousId(realId) {
    if (!ticketDetailsMap.has(realId)) {
        ticketDetailsMap.set(realId, crypto.randomUUID());
    }
    return ticketDetailsMap.get(realId);
}

function updateBreadcrumb() {
    const ol = document.querySelector('.breadcrumb');

    ol.innerHTML = '';

    const li1 = document.createElement('li');
    li1.classList.add('breadcrumb-item');
    li1.classList.add('active');

    const a1 = document.createElement('a');
    a1.innerText = 'Tickets';
    a1.href = "#";

    const loadTickets = async () => {
        await loadModule('tickets');
    };

    a1.addEventListener("click", loadTickets);
    trackEventListener(a1, "click", loadTickets);

    li1.appendChild(a1);
    ol.appendChild(li1);

    const li2 = document.createElement('li');
    li2.innerText = 'Ticket';
    li2.classList.add('breadcrumb-item');
    li2.classList.add('active');

    ol.appendChild(li2);
}

async function addNote() {   
    const noteText = document.getElementById('note-input');
    if (!noteText.innerText.trim())
        return;

    const container = document.getElementById("ticket-notes-container");

    const noteDiv = document.createElement('div');
    noteDiv.className = 'note';
    noteDiv.dataset.ticketdetailsid = crypto.randomUUID();

    const noteHeader = document.createElement('div');
    noteHeader.className = 'note-header';

    //header - initial creation date + author
    const noteCreated = document.createElement('span');
    noteCreated.className = 'note-created';

    const username = document.getElementById('hdnUsername').value;
    noteCreated.innerText = 'Added ' + `${formatUtc(new Date().toUTCString(), true, true)} ` + 'by ' + username;

    noteHeader.appendChild(noteCreated);

    //header - placeholder for layout modified date + editor
    const noteEdited = document.createElement('span');
    noteHeader.appendChild(noteEdited);

    //header - edit note btn
    const editBtn = document.createElement('button');
    editBtn.innerText = 'Edit';
    editBtn.type = 'button';
    editBtn.className = 'edit-note-btn';

    noteHeader.appendChild(editBtn);

    noteDiv.appendChild(noteHeader);

    //note body main content
    const noteContent = document.createElement('div');
    noteContent.className = 'note-content';

    noteContent.innerHTML = noteText.innerHTML;

    noteDiv.appendChild(noteContent);

    container.appendChild(noteDiv);

    //clear input
    document.getElementById('note-input').innerHTML = '';

    await saveNote(noteDiv);
}

async function editNote(e) {
    const note = e.target.closest('.note');
    const noteHeader = note.querySelector('.note-header');
    const contentContainer = note.querySelector('.note-content');
    const editBtn = note.querySelector('.edit-note-btn');
    

    //copy content
    const content = contentContainer.textContent;
  
    //create input
    const editDiv = document.createElement('div');
    editDiv.contentEditable = 'true';
    editDiv.className = 'note-content';
    editDiv.textContent = content;

    //create save button
    const saveBtn = document.createElement('button');
    saveBtn.innerText = 'Save';
    saveBtn.type = 'button';
    saveBtn.classList.add('save-edit-note-btn');

    //replace edit with save btn
    editBtn.replaceWith(saveBtn);

    //replace content container with editable input
    contentContainer.replaceWith(editDiv);

    //add yellow color to notify that the current note is in an editable state
    noteHeader.classList.add('editable-header');
    note.classList.add('editable');
    editDiv.focus();
}

async function saveEditNote(e) {
    const note = e.target.closest('.note');
    const noteHeader = note.querySelector('.note-header');
    const contentContainer = note.querySelector('.note-content');
    const saveBtn = note.querySelector('.save-edit-note-btn');

    //add edited details to header
    const editedHeader = document.createElement('span');
    editedHeader.className = 'note-edited';
    editedHeader.innerText = 'Edited ' + formatUtc(new Date().toUTCString(), true, true) + ' by ' + document.getElementById('hdnUsername').value;
    saveBtn.after(editedHeader);

    //revert styling back to saved note style
    noteHeader.classList.remove('editable-header');
    note.classList.remove('editable');

    //revert input back to non-editable div
    const content = contentContainer.textContent;
    const updatedDiv = document.createElement('div');
    updatedDiv.className = 'note-content';
    updatedDiv.textContent = content;

    contentContainer.replaceWith(updatedDiv);
    
    await saveNote(note);
}

async function saveNote(noteDiv) {
    const content = noteDiv.querySelector('.note-content').innerHTML.trim()
        .replace(/&ZeroWidthSpace;/g, '')    // remove zero-width spaces
        .replace(/<\/div><div>/g, '<br>')    // convert divs to line breaks
        .replace(/<div>/g, '')               // remove remaining opening divs
        .replace(/<\/div>/g, '');            // remove remaining closing divs

    if (!content) {
        return;
    }

    const ticketId = document.getElementById('hdnTicketId').value;

    let ticketDetailsId = getRealId(noteDiv.dataset.ticketdetailsid);
    if (!ticketDetailsId)
        ticketDetailsId = 0;

    //const ticketVersion = document.getElementById('lblTicketVersion').textContent;

    const details = {
        ticketId: parseInt(ticketId),
        //TicketVersion: parseInt(ticketVersion),
        ticketDetailsId: ticketDetailsId,
        note: content,
        author: document.getElementById('hdnUsername').value
    };

    const response = await axios.post('/Tickets/SaveTicketDetails', details, {
        headers: {
            'X-CSRF-TOKEN': token,
            'Content-Type': 'application/json'
        }
    });

    if (response && response.status === 200) {
        const newKey = crypto.randomUUID();

        noteDiv.dataset.ticketdetailsid = newKey;
        ticketDetailsMap.set(response.id, newKey)
    }
    else {
        alert("error");
        noteDiv.remove();
    }
}


async function deleteTicket() {

    if (!confirm("Are you sure you?")) {
        return;
    }

    const ticketId = document.getElementById('hdnTicketId').value;

    const response = await axios.delete('/Tickets/DeleteTicket', {
        params: {
            ticketId: parseInt(ticketId)
        },
        headers: {
            'X-CSRF-TOKEN': token,
            'Content-Type': 'application/json'
        }
    });

    if (response && response.status === 200) {
        setTimeout(500, alert('Success'));

        await loadModule('tickets');
    }
    else {
        alert("error");
        noteDiv.remove();
    }
}