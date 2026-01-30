

export async function getAssignDepartmentPartial() {
    try {
        const response = await axios.get('/Hr/AssignDepartmentPartial');
        return response.data;
    } catch (error) {
        alert("Error: getAssignDepartmentPartial")
        console.error("error", error);
        return false;
    }
}

export async function init() {
    //load initial partial
    let partial = await getAssignDepartmentPartial();
    document.getElementById("hr-partial-container").innerHTML = partial;
}