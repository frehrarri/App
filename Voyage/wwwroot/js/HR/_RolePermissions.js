
export async function init(data) {
    debugger;
    const partial = await getRolePermissionsPartial(data);
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