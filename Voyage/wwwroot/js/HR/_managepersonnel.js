import { loadModule } from "/js/__moduleLoader.js";

const changeTracker = new Map();

export async function getManagePersonnelPartial() {
    try {
        const companyId = parseInt(document.getElementById('hdnCompanyId').value);

        const response = await axios.get('/Hr/ManagePersonnelPartial', {
            params: { companyId: companyId }
        });

        return response.data;
    }
    catch (error) {
        console.error("error: getManageTicketPartial", error);
        return false;
    }
} 

function toggleActiveStatus(e) {
    let key = e.target.dataset.key;
    
    let record = changeTracker.get(key);
    if (record) {
        record.isUserActive = e.target.checked;
    }
    else {
        const roleId = document.querySelector(`.sel-assign-role[data-key="${key}"]`).value;
        let changes = {
            isUserActive: e.target.checked ?? false,
            roleId: roleId,
            employeeId: parseInt(e.target.dataset.userid),
            dbSaveAction: 1
        }

        changeTracker.set(key, changes);
    }
}

function assignIndividualRoles(e) {
    
    let key = e.target.dataset.key;
    
    //existing changes
    let record = changeTracker.get(key);
    if (record) {
        record.roleId = e.target.value;
    }
    //new change
    else {
        const isActive = document.querySelector(`.cbx-active[data-key="${key}"]`).checked;
        let changes = {
            isUserActive: isActive ?? false,
            roleId: e.target.value,
            employeeId: parseInt(e.target.dataset.userid),
            dbSaveAction: 1
        }

        changeTracker.set(key, changes);
    }
}

function remove(e) {
    const table = document.querySelectorAll("#manage-personnel tbody tr");

    if (!confirm("Are you sure you want to remove this user?")) {
        return;
    }

    table.forEach(r => {
        
        const row = r.closest("tr");
        if (row) {

            const cbx = row.querySelector('td > input');
            if (cbx.checked) {

                const key = row.dataset.key;
                const employeeId = row.querySelector('#employeeid').textContent.trim();
                row.remove();

                changeTracker.set(key, {
                    employeeId: parseInt(employeeId),
                    DbSaveAction: 2 // Remove
                });
            }

    
        }
    });
}

const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

async function save(e) {
    if (changeTracker.size === 0) {
        return;
    }
    const payload = Array.from(changeTracker.values());

    try {
        const response = await axios.post('/Hr/SavePersonnel', payload, {
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            }
        });

        if (response.data) {
            alert("Success")
            changeTracker.clear();
        } else {
            alert("Error");
        }

        return response.data;
    } catch (error) {
        alert("Error");
        console.error("error", error);
        return false;
    }
}

async function handlePersonnelEvents(e) {
    if (e.type == "click") {
        if (e.target.id === "self-register-button") {
            e.preventDefault();
            await loadModule("registerEmployee");
        }

        if (e.target.id === "save-btn")
            await save(e);

        if (e.target.className === "cbx-active")
            toggleActiveStatus(e);

        if (e.target.id === "btn-delete")
            remove(e);
    }
    else if (e.type == "change") {
        if (e.target.className === "sel-assign-role") {
            assignIndividualRoles(e);
        }
    }
}

function expandSideNavItem() {
    //expand sidenav
    const offcanvas = document.querySelector('.offcanvas.offcanvas-start');
    offcanvas.classList.add('expanded');

    //open submenus

    //move content container with sidenav

    //toggle icon
    
}

function updateBreadCrumb() {
    const ol = document.querySelector('.breadcrumb');

    ol.innerHTML = '';

    const li1 = document.createElement('li');
    li1.classList.add('breadcrumb-item');
    li1.classList.add('active')

    const a1 = document.createElement('a');
    a1.href = "#";
    a1.textContent = 'Human Resources'

    a1.addEventListener("click", expandSideNavItem);

    //add event listener that when clicked on opens side nav and expands Human Resources links

    li1.appendChild(a1);
    ol.appendChild(li1);

    const li2 = document.createElement('li');
    li2.classList.add('breadcrumb-item');
    li2.classList.add('active');
    li2.textContent = 'Manage Personnel';

    ol.appendChild(li2);
}

export async function init() {
    //load initial partial
    let partial = await getManagePersonnelPartial();
    const container = document.querySelector(".main-content");

    if (container) {
        container.innerHTML = partial;

        container.addEventListener("click", handlePersonnelEvents);
        container.addEventListener("change", handlePersonnelEvents);
    }

    updateBreadCrumb();
}