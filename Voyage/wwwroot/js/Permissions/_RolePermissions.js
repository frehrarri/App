
const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

export async function init(data) {
    const partial = await getRolePermissionsPartial(data);
    let container = document.querySelector(".main-content");

    if (container)
        container.innerHTML = partial;

    container = document.getElementById('role-permissions-container');
    const changeTracker = new Set();

    container?.addEventListener("click", (e) => handleEvents(e, changeTracker));

    updateBreadCrumb();
}

export async function getRolePermissionsPartial(data) {

    try {
        const response = await axios.get('/Permissions/RolePermissionsPartial', {
            params: {
                name: data.name,
                roleKey: data.datakey
            }
        });
        return response.data;
    } catch (error) {
        console.error("error: getRolePermissionsPartial", error);
        return false;
    }
}

async function handleEvents(e, changeTracker) {
    if (e.target.id == "btn-save-role-permission") 
        await saveRolePermissions(e);

    if (e.target.classList.contains('cbx-permissions'))
        toggleSaveButton(e, changeTracker);
        
}

function toggleSaveButton(e, changeTracker) {
    const existingChange = changeTracker.has(e.target.id);

    //remove existing change
    if (existingChange) {
        changeTracker.delete(e.target.id);

        //if there are no changes in the change tracker disable save
        if (changeTracker.size === 0)
            document.getElementById('btn-save-role-permission').disabled = true;
    }
    //add to change tracker
    else {
        changeTracker.add(e.target.id);
        document.getElementById('btn-save-role-permission').disabled = false;
    }
}

async function saveRolePermissions(e) {
    e.preventDefault();

    try {
        const dto = {
            Permissions: [],
            RoleKey: document.getElementById('hdn-rolekey').value
        }

        const checkboxes = document.querySelectorAll('.cbx-permissions');

        //get checkboxes for each permission
        checkboxes.forEach(c => {
            const guid = c.id.replace('cbx-', "");
            const permissionKey = document.getElementById(`hdn-${guid}`).value;

            dto.Permissions.push({
                PermissionKey: permissionKey,
                IsEnabled: c.checked
            });
        });

        response = await axios.post('/Permissions/SetPermissions', dto, {
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            }
        });

        if (response && response.status === 200) {
            alert("Success");
        }
        else {
            alert("Error");
        }

    } catch (error) {
        showSuccess(false);
        console.error("error", error);
        return false;
    }
}

function updateBreadCrumb() {
    const ol = document.querySelector('.breadcrumb');

    ol.innerHTML = '';

    const li1 = document.createElement('li');
    li1.classList.add('breadcrumb-item');
    li1.classList.add('active')

    const a1 = document.createElement('a');
    a1.href = "#";
    a1.textContent = 'Admin Settings'

    //add event listener that when clicked on opens side nav and expands Human Resources links
    //a1.addEventListener("click", expandSideNavItem);

    li1.appendChild(a1);
    ol.appendChild(li1);

    const li2 = document.createElement('li');
    li2.classList.add('breadcrumb-item');
    li2.classList.add('active');

    const a2 = document.createElement('a');
    a2.href = "#";
    a2.textContent = 'Manage Roles'

    //add event listener that when clicked replaces the partial view
    //a1.addEventListener("click", replacePartial);

    li2.appendChild(a2);
    ol.appendChild(li2);

    const li3 = document.createElement('li');
    li3.classList.add('breadcrumb-item');
    li3.classList.add('active');
    li3.textContent = 'Role Permissions'

    ol.appendChild(li3);
}