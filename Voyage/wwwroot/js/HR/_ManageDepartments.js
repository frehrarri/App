function addDepartment() {
    const ul = document.getElementById("ul-depts");
    const li = document.createElement("li");
    let newDept = document.getElementById("add-dept");

    li.textContent = `${newDept.value}`;
    li.addEventListener("click", handleEvents);

    ul.appendChild(li);
    newDept.value = "";
}

const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

async function saveDepartments() {
    let response;

    let ul = document.getElementById('ul-depts');
    let li = ul.querySelectorAll('li');

    let payload = Array.from(li).map(item => item.innerText);

    try {
        response = await axios.post('/Hr/SaveDepartments', payload, {
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

    //save
    if (e.target.id == "dept-save-btn")
        await saveDepartments();

    //remove
    const li = e.target.closest("#ul-depts li");
    if (li) {
        li.remove();
    }

    //add
    if (e.target.id == "add-dept-btn")
        addDepartment();
}