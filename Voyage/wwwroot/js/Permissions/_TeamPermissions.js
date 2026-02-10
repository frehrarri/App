
const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

export async function init(data) {
    const partial = await getTeamPermissionsPartial(data);
    document.getElementById('admin-settings-partial-container').innerHTML = partial;

    const container = document.getElementById('team-permissions-container');
    const changeTracker = new Set();

    container?.addEventListener("click", (e) => handleEvents(e, changeTracker));
}

export async function getTeamPermissionsPartial(data) {
    try {
        const response = await axios.get('/Permissions/TeamPermissionsPartial', {
            params: {
                name: data.name,
                roleKey: data.datakey
            }
        });
        return response.data;
    } catch (error) {
        console.error("error: getTeamPermissionsPartial", error);
        return false;
    }
}