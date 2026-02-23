
const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

export async function init(data) {
    const partial = await getTeamPermissionsPartial(data);
    const container = document.querySelector(".main-content");

    const changeTracker = new Set();

    if (container && partial) {
        container.innerHTML = partial;
        //container.addEventListener("click", (e) => handleEvents(e, changeTracker));
    }
}

export async function getTeamPermissionsPartial(data) {
    try {
        const response = await axios.get('/Permissions/TeamPermissionsPartial', {
            params: {
                name: data.name,
                teamkey: data.datakey
            }
        });
        return response.data;
    } catch (error) {
        console.error("error: getTeamPermissionsPartial", error);
        return false;
    }
}