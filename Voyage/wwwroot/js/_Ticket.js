import { getTicketsPartial } from "./_Tickets.js";

export async function init() {
    document.getElementById("btnGoToTickets")?.addEventListener("click", async () => {
        await getTicketsPartial();
    });

    //handle events for content editable div
    const noteDiv = document.getElementById('noteContent');
    if (noteDiv) {
        noteDiv.addEventListener('paste', handlePaste);
        noteDiv.addEventListener('keydown', handleEnter);
    }

    await loadTicketNotes();

}

async function loadTicketNotes() {
    const ticketId = document.getElementById('lblTicketId').textContent;
    if (!ticketId) return;

    try {
        const response = await axios.get('/Tickets/GetTicketDetails', {
            params: { ticketId: ticketId }
        });

        const container = document.getElementById("ticket-notes-container");
        container.innerHTML = ''; // Clear existing
        
        response.data.forEach(note => {
            if (note.ticketDetailsId) {
                renderNote(note);
            }
        });

        //default to open add note
        addNote();

    } catch (error) {
        console.error("Error loading notes:", error);
    }
}

function addNote() {
    const container = document.getElementById("ticket-notes-container");

    const noteDiv = document.createElement('div');
    noteDiv.className = 'ticket-note';
    noteDiv.innerHTML = `
        <div class="note-header">
            <span class="note-author">New Note</span>
            <span class="note-date">${new Date().toLocaleString()}</span>
        </div>
        <div class="note-content" contenteditable="true" data-placeholder="Enter notes..."></div>
        <div class="note-actions">
            <button type="button" class="btn-save-note">Save</button>
            <button type="button" class="btn-clear-note">Clear</button>
        </div>
    `;

    //handle events for content editable div
    const contentDiv = noteDiv.querySelector('.note-content');
    contentDiv.addEventListener('dragover', e => e.preventDefault());
    contentDiv.addEventListener('drop', handleFileDrop);
    contentDiv.addEventListener('paste', handlePaste);
    contentDiv.addEventListener('keydown', handleEnter);

    noteDiv.querySelector('.btn-save-note').addEventListener('click', () => saveNote(noteDiv));
    noteDiv.querySelector('.btn-clear-note').addEventListener('click', () => clearNote(noteDiv));

    container.appendChild(noteDiv);
    contentDiv.focus();
    container.scrollTop = container.scrollHeight;
}

async function saveNote(noteDiv) {
    
    const content = noteDiv.querySelector('.note-content').innerHTML.trim()
        .replace(/&ZeroWidthSpace;/g, '')    // remove zero-width spaces
        .replace(/<\/div><div>/g, '<br>')    // convert divs to line breaks
        .replace(/<div>/g, '')               // remove remaining opening divs
        .replace(/<\/div>/g, '');            // remove remaining closing divs

    if (!content) {
        alert('Please enter a note');
        return;
    }
    
    const detailsId = noteDiv.querySelector('.note-header').dataset.detailsid ?? 0;
    const ticketId = document.getElementById('lblTicketId').textContent;
    const ticketVersion = document.getElementById('lblTicketVersion').textContent;
    
    const details = {
        TicketId: parseInt(ticketId),
        TicketVersion: parseInt(ticketVersion),
        TicketDetailsId: parseInt(detailsId),
        Note: content
    };

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    const response = await axios.post('/Tickets/SaveTicketDetails', details, {
        headers: {
            'X-CSRF-TOKEN': token,
            'Content-Type': 'application/json'
        }
    });

    //insert note to container after save
    noteDiv.dataset.detailsid = response.data.ticketDetailsId;
    noteDiv.innerHTML = `<div class="note-header" data-id="${response.data.id}" data-ticketId="${response.data.ticketId}" data-detailsId="${response.data.ticketDetailsId}">
<span class="note-author">${response.data.author}</span>
<span class="note-date">${response.data.createdDate}</span>
${response.data.modifiedDate ? `<i>edited ${response.data.modifiedDate}</i>` : ''}
</div><div class="note-content-display">${response.data.note}</div><button type="button" class="note-edit">Edit</button>`;


    noteDiv.querySelector('.note-edit').addEventListener('click', () => enableEdit(noteDiv));

    //open new add note after saving
    addNote();
}


function renderNote(note) {
    const container = document.getElementById("ticket-notes-container");

    let edited = "";
    if (note.modifiedDate != null && note.modifiedDate != 0) {
        edited = `<i>edited ${note.modifiedDate}</i>`;
    }

    const noteDiv = document.createElement('div');
    noteDiv.className = 'ticket-note saved';

    // Put everything on one line, no indentation around note.note
    noteDiv.innerHTML = `<div class="note-header" data-ticketId="${note.ticketId}" data-detailsId="${note.ticketDetailsId}"><span class="note-author">${note.author}</span><span class="note-date">${note.createdDate}</span>${edited}</div><div class="note-content-display">${note.note}</div><button type="button" class="note-edit">Edit</button>`;

    noteDiv.querySelector('.note-edit').addEventListener('click', function () {
        enableEdit(noteDiv);
    });

    container.appendChild(noteDiv);
}


function enableEdit(noteDiv) {

    //remove add note when editing
    removeAddNote();

    const contentText = noteDiv.querySelector('.note-content-display').innerHTML;
    const ticketid = document.getElementById('hdnTicketId').value;
    const detailsid = noteDiv.querySelector('.note-header').dataset.detailsid;

    noteDiv.innerHTML = `
        <div class="note-header" data-ticketId="${ticketid}" data-detailsId="${detailsid}">
            <span class="note-author">Editing</span>
            <span class="note-date">${new Date().toLocaleString()}</span>
        </div>
        <div class="note-content" contenteditable="true">${contentText}</div>
        <div class="note-actions">
            <button type="button" class="btn-save-note">Save</button>
            <button type="button" class="btn-cancel-note">Cancel</button>
        </div>
    `;

    const saveBtn = noteDiv.querySelector('.btn-save-note');
    saveBtn.addEventListener('click', () => saveNote(noteDiv));

    const cancelBtn = noteDiv.querySelector('.btn-cancel-note');
    cancelBtn.addEventListener('click', () => {
        loadTicketNotes();
    });
}

function removeAddNote() {
    const unsavedNote = document.querySelectorAll('.ticket-note:not(.saved)');
    if (unsavedNote[0]) {
        unsavedNote[0].remove();
    }
}

function clearNote(noteDiv) {
    noteDiv.querySelector('.note-content').innerHTML = "";
}

