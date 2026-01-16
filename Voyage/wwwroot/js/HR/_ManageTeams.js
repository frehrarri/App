export async function getManageTeamsPartial() {
    try {
        const response = await getPartial("Hr", "ManageTeamsPartial");
        document.getElementById("hr-partial-container").innerHTML = response.data;
        await loadModule("manageTeams");

        return true;
    } catch (error) {
        console.error("error: getManageTicketPartial", error);
        return false;
    }
}


const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

async function saveTeams() {
    let response;

    let ul = document.getElementById('ul-teams');
    let li = ul.querySelectorAll('li');

    let payload = Array.from(li).map(item => item.innerText);

    try {
        response = await axios.post('/Hr/SaveTeams', payload, {
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            }
        });

        showSuccess(true);

        return response.data;
    } catch (error) {
        showSuccess(false);
        console.error("error", error);
        return false;
    }
}

function addTeam() {
    const ul = document.getElementById("ul-teams");
    const li = document.createElement("li");
    let newTeam = document.getElementById("add-team");

    li.textContent = `${newTeam.value}`;
    li.addEventListener("click", handleEvents);

    ul.appendChild(li);
    newTeam.value = "";

}

async function handleEvents(e) {
    e.preventDefault();

    if (e.target.tagName == "INPUT")
        return;

    //save
    if (e.target.id == "team-save-btn")
        await saveTeams();

    //remove
    const li = e.target.closest("#ul-teams li");
    if (li) {
        li.remove();
    }

    //add
    if (e.target.id == "add-team-btn")
        addTeam();

}

export function init() {
    const container = document.getElementById('ul-teams');
    container.querySelectorAll('li')?.forEach(el => el.addEventListener("click", handleEvents));
}
