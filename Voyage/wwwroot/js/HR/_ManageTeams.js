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

        if (response && response.data) {
            showSuccess(true);
 
            //update dropdown with save
            document.querySelectorAll('.sel-assign-team-member')?.forEach(dropdown => {
                dropdown.replaceChildren();

                response.data.forEach(team => {
                    let option = document.createElement('option');
                    option.value = team.teamId;
                    option.innerText = team.name;
                    dropdown.appendChild(option);
                });
            });
                
            return response.data;
        }
        
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

const teamMembers = [];

async function saveTeamMembers() {
    try {
        response = await axios.post('/Hr/SaveTeamMembers', teamMembers, {
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            }
        });

        teamMembers = [];

        showSuccess(true);

        return response.data;
    } catch (error) {
        showSuccess(false);
        console.error("error", error);
        return false;
    }
}


async function handleEvents(e) {
    e.preventDefault();

    if (e.target.tagName == "INPUT")
        return;

    //save teams
    if (e.target.id == "team-save-btn")
        await saveTeams();

    //remove team
    const li = e.target.closest("#ul-teams li");
    if (li) {
        li.remove();
    }

    //add new team
    if (e.target.id == "add-team-btn")
        addTeam();

    //catch changes to assign team members
    if (e.target.classList.contains('sel-assign-team-member')) {

        if (e.target.dataset.userid && e.target.value) {

            const teamDto = {
                userId: parseInt(e.target.dataset.userid),
                teamId: parseInt(e.target.value)
            }
            teamMembers.push(teamDto);
        }
    }

    //save members to team
    if (e.target.id == "team-member-save-btn")
        await saveTeamMembers();
        
}

export function init() {
    let container = document.getElementById('ul-teams');
    container.querySelectorAll('li')?.forEach(el => el.addEventListener("click", handleEvents));

    document.getElementById('add-team-btn')?.addEventListener("click", handleEvents);
    document.getElementById('team-save-btn')?.addEventListener("click", handleEvents);
    document.getElementById('team-member-save-btn')?.addEventListener("click", handleEvents);

    container = document.getElementById('dv-allocate-personnel');
    container.querySelectorAll('.sel-assign-team-member')?.forEach(el => el.addEventListener("change", handleEvents));
}
