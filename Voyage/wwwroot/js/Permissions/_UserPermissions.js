
const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

export async function init(data) {
    const partial = await getRolePermissionsPartial(data);
    document.getElementById('admin-settings-partial-container').innerHTML = partial;

    const container = document.getElementById('user-permissions-container');
    const changeTracker = new Set();

    container?.addEventListener("click", (e) => handleEvents(e, changeTracker));

    updateBreadCrumb();
}

export async function getUserPermissionsPartial(data) {

    try {
        const response = await axios.get('/Permissions/UserPermissionsPartial', {
            params: {
                name: data.name,
                roleKey: data.datakey
            }
        });
        return response.data;
    } catch (error) {
        console.error("error: getUserPermissionsPartial", error);
        return false;
    }
}

function updateBreadCrumb() {
    //const ol = document.querySelector('.breadcrumb');

    //ol.innerHTML = '';

    //const li1 = document.createElement('li');
    //li1.classList.add('breadcrumb-item');
    //li1.classList.add('active')

    //const a1 = document.createElement('a');
    //a1.href = "#";
    //a1.textContent = 'Admin Settings'

    //li1.appendChild(a1);
    //ol.appendChild(li1);

    //const li2 = document.createElement('li');
    //li2.classList.add('breadcrumb-item');
    //li2.classList.add('active');
    //li2.textContent = 'Manage Roles'

    ////const a2 = document.createElement('a');
    ////a2.href = "#";
    ////a2.textContent = 'Manage Roles'

    ////li2.appendChild(a2);
    //ol.appendChild(li2);
}