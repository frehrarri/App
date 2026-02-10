
const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

export async function init(data) {
    const partial = await getDepartmentPermissionsPartial(data);
    document.getElementById('hr-partial-container').innerHTML = partial;

    //const container = document.getElementById('dept-permissions-container');
    //const changeTracker = new Set();

    //container?.addEventListener("click", (e) => handleEvents(e, changeTracker));
}

export async function getDepartmentPermissionsPartial(data) {
    try {
        const response = await axios.get('/Permissions/DeptPermissionsPartial', {
            params: {
                name: data.name,
                roleKey: data.datakey
            }
        });
        return response.data;
    } catch (error) {
        console.error("error: getDepartmentPermissionsPartial", error);
        return false;
    }
}

//async function handleEvents(e, changeTracker) {
//    if (e.target.id == "btn-save-dept-permission")
//        await saveDeptPermissions(e);

//    if (e.target.classList.contains('cbx-permissions'))
//        toggleSaveButton(e, changeTracker);

//}

function toggleSaveButton(e, changeTracker) {
    const existingChange = changeTracker.has(e.target.id);

    //remove existing change
    if (existingChange) {
        changeTracker.delete(e.target.id);

        //if there are no changes in the change tracker disable save
        if (changeTracker.size === 0)
            document.getElementById('btn-save-dept-permission').disabled = true;
    }
    //add to change tracker
    else {
        changeTracker.add(e.target.id);
        document.getElementById('btn-save-dept-permission').disabled = false;
    }
}

//async function saveDeptPermissions(e) {
//    e.preventDefault();

//    try {
//        const dto = {
//            Permissions: [],
//            DeptKey: document.getElementById('hdn-deptkey').value
//        }

//        const checkboxes = document.querySelectorAll('.cbx-permissions');

//        //get checkboxes for each permission
//        checkboxes.forEach(c => {
//            const guid = c.id.replace('cbx-', "");
//            const permissionKey = document.getElementById(`hdn-${guid}`).value;

//            dto.Permissions.push({
//                PermissionKey: permissionKey,
//                IsEnabled: c.checked
//            });
//        });

//        response = await axios.post('/Permissions/SetPermissions', dto, {
//            headers: {
//                'X-CSRF-TOKEN': token,
//                'Content-Type': 'application/json'
//            }
//        });

//        if (response && response.status === 200) {
//            alert("Success");
//        }
//        else {
//            alert("Error");
//        }

//    } catch (error) {
//        showSuccess(false);
//        console.error("error", error);
//        return false;
//    }


}