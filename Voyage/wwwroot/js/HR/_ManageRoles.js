function addRole() {
    const ul = document.getElementById("ul-roles");
    const li = document.createElement("li");
    let newRole = document.getElementById("add-role");

    li.textContent = `${newRole.value}`;
    li.addEventListener("click", handleEvents);

    ul.appendChild(li);
    newRole.value = "";
}

const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

async function saveRoles() {
    let response;
    let ul = document.getElementById('ul-roles');
    let li = ul.querySelectorAll('li');

    let payload = Array.from(li).map(item => item.innerText);

    try {
        response = await axios.post('/Hr/SaveRoles', payload, {
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

async function handleEvents(e) {
    e.preventDefault();

    if (e.target.tagName == "INPUT")
        return;

    //add
    if (e.target.id == "add-role-btn")
        addRole();

    //remove
    li = e.target.closest("#ul-roles li");
    if (li) {
        li.remove();
    }

    //save
    if (e.target.id == "role-save-btn")
        await saveRoles();


}