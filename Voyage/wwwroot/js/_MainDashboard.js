export async function init() {
    const partial = await getMainDashboardPartial();
    const container = document.querySelector('.main-content');

    if (container )
        container.innerHTML = partial

    updateBreadCrumb();
}

export async function getMainDashboardPartial() {
    try {
        const response = await axios.get('/App/MainDashboardPartial');
        return response.data;
    }
    catch (error) {
        alert("Error: getMainDashboardPartial")
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
    li1.textContent = 'Main Dashboard'

    ol.appendChild(li1);
}