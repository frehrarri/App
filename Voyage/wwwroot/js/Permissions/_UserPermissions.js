
const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

export async function init(data) {
    const partial = await getRolePermissionsPartial(data);
    document.getElementById('admin-settings-partial-container').innerHTML = partial;

    const container = document.getElementById('user-permissions-container');
    const changeTracker = new Set();

    container?.addEventListener("click", (e) => handleEvents(e, changeTracker));
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