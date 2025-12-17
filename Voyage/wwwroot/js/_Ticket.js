import { getTicketsPartial } from "./_Tickets.js";

export async function init() {
    document.getElementById("btnTicketAddNote")?.addEventListener("click", addNote);
    document.getElementById("btnGoToTickets")?.addEventListener("click", async () => {
        await getTicketsPartial();
    });
    /*await loadTicketNotes();*/
}

//async function loadTicketNotes() {
//    const ticketId = document.getElementById("lblTicketId")?.textContent;
//    if (!ticketId) return;

//    try {
//        const response = await axios.get('/Tickets/GetTicketNotes', {
//            params: { ticketId: ticketId }
//        });

//        const container = document.getElementById("ticket-notes-container");
//        container.innerHTML = ''; // Clear existing

//        // Remove await here - renderNote doesn't need to be async
//        response.data.forEach(note => {
//            renderNote(note);
//        });
//    } catch (error) {
//        console.error("Error loading notes:", error);
//    }
//}

function addNote() {

    /*const input = document.getElementById("ticket-notes").value;*/
    const container = document.getElementById("ticket-notes-container");

    // Create new note element
    const noteDiv = document.createElement('div');
    noteDiv.className = 'ticket-note';
    noteDiv.innerHTML = `
        <div class="note-header">
            <span class="note-author">New Note</span>
            <span class="note-date">${new Date().toLocaleString()}</span>
        </div>
        <textarea class="note-content" placeholder="Enter your note..."></textarea>
        <div class="note-actions">
            <button type="button" class="btn-save-note">Save</button>
            <button type="button" class="btn-cancel-note">Cancel</button>
        </div>
    `;

    container.appendChild(noteDiv);

    // Add event listeners for the new note
    noteDiv.querySelector('.btn-save-note').addEventListener('click', () => saveNote(noteDiv));
    noteDiv.querySelector('.btn-cancel-note').addEventListener('click', () => noteDiv.remove());

    // Focus the textarea
    noteDiv.querySelector('.note-content').focus();

    // Scroll to bottom to show new note
    container.scrollTop = container.scrollHeight;
}

//async function saveNote(noteDiv) {
//    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
//    const ticketId = document.getElementById("lblTicketId")?.textContent;
//    const content = noteDiv.querySelector('.note-content').value;

//    if (!content.trim()) {
//        alert('Please enter a note');
//        return;
//    }

//    const note = {
//        TicketId: parseInt(ticketId),
//        Content: content,
//        CreatedDate: new Date().toISOString()
//    };

//    try {
//        const response = await axios.post('/Tickets/SaveTicketNote', note, {
//            headers: {
//                'RequestVerificationToken': token,
//                'Content-Type': 'application/json'
//            }
//        });

//        // Replace the editable note with a saved note display
//        noteDiv.innerHTML = `
//            <div class="note-header">
//                <span class="note-author">${response.data.createdBy}</span>
//                <span class="note-date">${new Date(response.data.createdDate).toLocaleString()}</span>
//            </div>
//            <div class="note-content-display">${response.data.content}</div>
//        `;
//        noteDiv.classList.add('saved');
//    } catch (error) {
//        console.error("Error saving note:", error);
//        alert('Failed to save note');
//    }
//}

//function renderNote(note) {
//    const container = document.getElementById("ticket-notes-container");

//    const noteDiv = document.createElement('div');
//    noteDiv.className = 'ticket-note saved';
//    noteDiv.innerHTML = `
//        <div class="note-header">
//            <span class="note-author">${note.createdBy || 'Unknown'}</span>
//            <span class="note-date">${new Date(note.createdDate).toLocaleString()}</span>
//        </div>
//        <div class="note-content-display">${note.content}</div>
//    `;

//    // FIXED: appendChild (lowercase 'a') and removed second parameter
//    container.appendChild(noteDiv);
//}