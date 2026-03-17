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

    //go to ticket
    //document.getElementById("btnGoToTickets")?.addEventListener("click", async () => {
    //    //adjust container
    //    document.getElementById('ticket-view').style.width = '1400px';

    //    const module = await loadModule("tickets");
    //    await module.getTicketsPartial();
    //});

    //edit
    //document.getElementById("btnEditTicket")?.addEventListener("click", async () => {
    //    const ticketId = document.getElementById('lblTicketId').textContent;
    //    const module = await loadModule("tickets");
    //    await module.getManageTicketPartial(ticketId, null);
    //}); 

    ////versioning
    //const links = document.getElementsByClassName("versionHistoryLink");
    //for (const link of links) {
    //    link.addEventListener("click", async (e) => {
    //        e.preventDefault();

    //        const version = parseInt(e.target.textContent);
    //        const ticketId = parseInt(
    //            document.getElementById("lblTicketId").textContent
    //        );

    //        const module = await loadModule("tickets");
    //        await module.getTicketPartial(ticketId, version);
    //    });
    //}

    //handle events for content editable div
    //const noteDiv = document.getElementById('noteContent');
    //if (noteDiv) {
    //    noteDiv.addEventListener('paste', handlePaste);
    //    noteDiv.addEventListener('keydown', handleEnter);
    //}

    //await loadTicketNotes();
}

async function handleClicks(e) {

    const btn = e.target.closest('button');
    if (!btn)
        return;

    if (btn.id == 'add-note-button')
        await addNote(e);
    //else if (btn.id == 'edit-ticket-button')
    //    editTicket();
    //if (btn.id == 'delete-ticket-button')
    //    deleteTicket();
    
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


//async function loadTicketNotes() {
//    try {
//        const ticketId = document.getElementById('lblTicketId').textContent;
//        if (!ticketId) return;

//        const ticketVersion = document.getElementById('lblTicketVersion').textContent;

//        const response = await axios.get('/Tickets/GetTicket', {
//            params: { ticketId: ticketId, ticketVersion: ticketVersion }
//        });

//        const container = document.getElementById("ticket-notes-container");
//        container.innerHTML = ''; // Clear existing

//        if (response.data.ticketDetails.length > 0) {
//            response.data.ticketDetails.forEach(note => {
//                renderNote(note);
//            });
//        }

//        //default to open add note
//        const isLatest = document.getElementById('hdnIsLatest').value;
//        const section = document.getElementById('lblSection').innerText;
//        const isEditable = isLatest === 'true' && (section !== 'Completed' || section !== 'Discontinued');

//        if (isEditable) {
//            addNote();
//        }

//        hideEditBtns();

//    } catch (error) {
//        console.error("Error loading notes:", error);
//    }
//}



async function saveNote(noteDiv) {
    debugger;

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
        




//    //insert note to container after save
//    noteDiv.dataset.detailsid = response.data.ticketDetailsId;
//    noteDiv.innerHTML = `<div class="note-header" data-id="${response.data.id}" data-ticketId="${response.data.ticketId}" data-detailsId="${response.data.ticketDetailsId}">
//<span class="note-author">${response.data.author}</span>
//${response.data.modifiedDate ? `<span class='modified-date'><i>edited ${formatUtc(response.data.modifiedDate)}</i></span>` : ''}
//<span class="note-date">${formatUtc(response.data.createdDate)}</span>
//</div><div class="note-content-display">${response.data.note}</div><br><button type="button" class="note-edit primary-btn">Edit</button>`;

//    noteDiv.querySelector('.note-edit').addEventListener('click', () =>
//    {
//        enableEdit(noteDiv)
//    });

//    //open new add note after saving
//    const isLatest = document.getElementById('hdnIsLatest').value;
//    const section = document.getElementById('lblSection').innerText;
//    const isEditable = isLatest === 'true' && (section !== 'Completed' || section !== 'Discontinued');

//    if (isEditable) {
//        addNote();
//    }

//    hideEditBtns();
}


//function renderNote(note) {
//    const container = document.getElementById("ticket-notes-container");

//    let edited = "";
//    if (note.modifiedDate != null && note.modifiedDate != 0) {
//        edited = `<div class='modified-date'><i>edited ${formatUtc(note.modifiedDate)}</i></div>`;
//    }

//    const noteDiv = document.createElement('div');
//    noteDiv.className = 'ticket-note saved';

//    // Put everything on one line, no indentation around note.note
//    noteDiv.innerHTML = `<div class="note-header" data-ticketId="${note.ticketId}" data-detailsId="${note.ticketDetailsId}"><span class="note-author">${note.author}</span>${edited}<span class="note-date">${formatUtc(note.createdDate)}</span></div><div class="note-content-display">${note.note}</div><br><button type="button" class="note-edit primary-btn">Edit</button>`;

//    noteDiv.querySelector('.note-edit').addEventListener('click', function () {
//        enableEdit(noteDiv);
//    });

//    container.appendChild(noteDiv);
//}


//function enableEdit(noteDiv) {
//    //remove add note when editing
//    removeAddNote();

//    const contentText = noteDiv.querySelector('.note-content-display').innerHTML;
//    const ticketid = document.getElementById('hdnTicketId').value;
//    const detailsid = noteDiv.querySelector('.note-header').dataset.detailsid;

//    noteDiv.innerHTML = `
//        <div class="note-header" data-ticketId="${ticketid}" data-detailsId="${detailsid}">
//            <span class="note-author">Editing</span>
//            <span class="note-date">${formatUtc(new Date().toUTCString())}</span>
//        </div>
//        <div class="note-content" contenteditable="true">${contentText}</div>
//        <div class="note-actions">
//            <button type="button" class="btn-save-note primary-btn">Save</button>
//            <button type="button" class="btn-cancel-note secondary-btn">Cancel</button>
//        </div>
//    `;

//    const saveBtn = noteDiv.querySelector('.btn-save-note');
//    saveBtn.addEventListener('click', () => saveNote(noteDiv));

//    const cancelBtn = noteDiv.querySelector('.btn-cancel-note');
//    cancelBtn.addEventListener('click', () => {
//        loadTicketNotes();
//    });
//}

//function removeAddNote() {
//    const unsavedNote = document.querySelectorAll('.ticket-note:not(.saved)');
//    if (unsavedNote[0]) {
//        unsavedNote[0].remove();
//    }
//}

//function clearNote(noteDiv) {
//    noteDiv.querySelector('.note-content').innerHTML = "";
//}

//function hideEditBtns() {
//    let user = document.getElementById('hdnUser').value;
//    let isLatest = document.getElementById('hdnIsLatest').value;
//    const section = document.getElementById('lblSection').innerText;

//    //disable header edit button
//    let editBtn = document.getElementById('btnEditTicket');
//    let author = document.getElementById("lblAuthor")?.textContent.trim();

//    let isEditable = isLatest === 'true' && (section !== 'Completed' || section !== 'Discontinued');

//    if (isEditable == false || (author && author !== user)) {
//        if (editBtn) {
//            editBtn.style.display = 'none';
//        }
//    }

//    //disable edit buttons for notes
//    let notes = document.querySelectorAll('.ticket-note.saved');

//    for (let i = 0; i < notes.length; i++){
//        author = notes[i].getElementsByClassName('note-author')[0];

//        if (isEditable == false || (author && author.textContent.trim() !== user)) {

//            editBtn = notes[i].getElementsByClassName('note-edit')[0];

//            if (editBtn) {
//                editBtn.style.display = 'none';
//            }
//        }
     
//    }
//}

