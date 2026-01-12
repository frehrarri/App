import { loadModule } from './__moduleLoader.js';

export async function init() {
    document.getElementById('add-section-btn').addEventListener("click", (e) => addSection());

    document.getElementById('save-settings-btn')?.addEventListener("click", (e) => save(e));

    document.querySelector("#back-btn")?.addEventListener("click", async (e) => {
        const module = await loadModule("tickets");
        await module.getTicketsPartial();
    });
}


        

function addSection() {
    const input = document.getElementById('section-title-input').value.trim();
    if (!input) return;

    //create tag and add to new container
    let newSection = document.createElement("span")
    newSection.className = "delete-section";
    newSection.innerHTML = `${input} <span class='delete'>x</span>`;

    document.getElementById('section-settings').append(newSection);

    //clear after adding to container
    document.getElementById('section-title-input').value = "";

    document.querySelectorAll('.delete-section').forEach(el => {
        el.removeEventListener("click", removeSection);
        el.addEventListener("click", removeSection);
    });

    document.querySelectorAll('.delete').forEach(el => {
        el.removeEventListener("click", removeSection);
        el.addEventListener("click", removeSection);
    });
}

function removeSection(e) {
    if (e.target.classList.contains('delete')) {
        e.target.parentElement.remove();
    }
    else {
        e.target.remove();
    }
}

async function save(e) {
    e.preventDefault();

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    const repeatOption = document.querySelector("[name='rdo-repeat']:checked").value;
    const sprintStart = document.getElementById('start-date')?.value;
    const sprintEnd = document.getElementById('end-date')?.value;

    const sections = Array.from(document.querySelectorAll('.delete-section')).map((s, i) => {
        const clone = s.cloneNode(true);
        clone.querySelector('.delete')?.remove();
        return {
            Title: clone.textContent.trim(),
            SectionOrder: i
        };
    });

    const dto = {
        RepeatSprintOption: parseInt(repeatOption),
        SprintStart: sprintStart,
        SprintEnd: sprintEnd,
        Sections: sections
    }

    let response;

    try {
        response = await axios.post('/Tickets/SaveSettings', dto, {
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            }
        });

        showSuccess(true);
        return true;
    } catch (error) {
        showSuccess(false);
        console.error("error: /Tickets/SaveSettings", error);
        return false;
    }

    return response.data; // bool

}