

export async function getAssignDepartmentPartial(params) {
    try {
        const response = await axios.get('/Hr/AssignDepartmentPartial', {
            params: params
        });

        return response.data;
    } catch (error) {
        alert("Error: getAssignDepartmentPartial")
        console.error("error", error);
        return false;
    }
}

export async function init(params) {
    //load initial partial
    let partial = await getAssignDepartmentPartial(params);
    document.getElementById("hr-partial-container").innerHTML = partial;
}