
const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

async function savePermissions() {
    let response;

    let ul = document.getElementById('ul-depts');
    let li = ul.querySelectorAll('li');

    let payload = Array.from(li).map(item => item.innerText);

    try {
        response = await axios.post('/Hr/SavePermissions', payload, {
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


function addPermission() {
    const ul = document.getElementById("ul-privileges");
    const li = document.createElement("li");
    let newPriv = document.getElementById("add-priv");

    li.textContent = `${newPriv.value}`;
    li.addEventListener("click", handleEvents);

    ul.appendChild(li);
    newPriv.value = "";
}


async function handleEvents(e) {
    e.preventDefault();

    if (e.target.tagName == "INPUT")
        return;

    // add 
    if (e.target.id == "add-permission-btn")
        addPermission();

    //save
    if (e.target.id == "permission-save-btn")
        await savePermissions();

    // remove 
    let li = e.target.closest("#ul-permission li");
    if (li) {
        li.remove();
    }
}


export function init() {
    const container = document.getElementById('ul-permissions');
    container.querySelectorAll('li')?.forEach(el => el.addEventListener("click", handleEvents));


}