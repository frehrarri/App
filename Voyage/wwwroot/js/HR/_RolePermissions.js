
export async function init(data) {
    const partial = await getRolePermissionsPartial(data);
    document.getElementById('hr-partial-container').innerHTML = partial;


}

export async function getRolePermissionsPartial(data) {

    try {
        const response = await axios.get('/Hr/RolePermissionsPartial', {
            params: {
                name: data.name,
                roleKey: data.datakey
            }
        });
        return response.data;
    } catch (error) {
        console.error("error: AssignRolePermissions", error);
        return false;
    }
}