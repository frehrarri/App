import { loadModule } from "/js/__moduleLoader.js";



function getPreviousContainer(e) {
    const containers = document.querySelectorAll(".form-container");

    if (e.target.id == "btn-back-user") {
        containers[1].classList.add("hidden");
        containers[0].classList.remove("hidden");
    }
}

function getNextContainer(e) {
    const containers = document.querySelectorAll(".form-container");

    if (e.target.id == "btn-next-account-details") {
        containers[0].classList.add("hidden");
        containers[1].classList.remove("hidden");
    }
}

export async function getRegisterEmployeePartial() {
        const companyId = parseInt(document.getElementById('hdnCompanyId').value);

        const response = await axios.get('/Hr/RegisterEmployeePartial', {
            params: { companyId: companyId }
        });

    return response.data;
}

const register = async (e) => {
    e.preventDefault();

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    let companyData = {
        companyId: parseInt(document.getElementById("hdnCompanyId").value)
    };

    const registrationData = {
        isCompanyRegistration: (document.getElementById('hdnIsCompanyRegistration')?.value == "true" ?? false),
        userName: document.getElementById("userName").value,
        firstname: document.getElementById("firstName").value,
        middlename: document.getElementById("middleName").value,
        lastname: document.getElementById("lastName").value,
        email: document.getElementById("email").value,
        password: document.getElementById("password").value,
        phoneCountryCode: parseInt(document.getElementById("phoneCountryCode").value),
        phoneAreaCode: parseInt(document.getElementById("phoneAreaCode").value),
        phone: parseInt(document.getElementById("phone").value),
        streetAddress: document.getElementById("streetAddress").value,
        unitNumber: parseInt(document.getElementById("unitNumber").value) || 0,
        city: document.getElementById("city").value,
        state: document.getElementById("state").value,
        country: document.getElementById("country").value,
        zipCode: parseInt(document.getElementById("zipCode").value),
        company: companyData
    };

    const isValid = await validate(registrationData);
    if (isValid) {
        try {
            const response = await axios.post('/User/Register', registrationData, {
                headers: {
                    'Content-Type': 'application/json',
                    'X-CSRF-TOKEN': token
                }
            });

            if (response.data.redirectURL) {
                window.location.href = response.data.redirectURL;
            }

        } catch (error) {
            if (error.response) {
                alert('Registration failed: ' + error.response.data);
            } else if (error.request) {
                alert('No response from server.');
            } else {
                alert('Error: ' + error.message);
            }
        }
    }
}

async function validate(data) {

    hideErrors();

    let input = null;

    await validateUsername(input, data);
    await validateName(data);
    await validatePassword(input, data);
    await validatePhone(input, data);
    await validateEmail(data);
    await validateAddress(input, data);

    //look for visible errors
    const errorsExist = Array.from(document.querySelectorAll('.text-error')).some(el =>
        el.offsetParent !== null
    );

    if (errorsExist) {
        return false;
    }

    return true;
}

async function validateUsername(input, data) {

    input = document.getElementById('userName');

    if (!data.userName) {
        document.getElementById('error-username-req').style.display = 'block';
        input.classList.add('border-error');
    }

    if (data.userName.length < 5) {
        document.getElementById('error-username-min').style.display = 'block';
        input.classList.add('border-error');
    }

    if (data.userName.length > 20) {
        document.getElementById('error-username-max').style.display = 'block';
        input.classList.add('border-error');
    }

    if (await CheckUsernameExists(data) === true) {
        document.getElementById('error-username-exists').style.display = 'block';
        input.classList.add('border-error');
    }

}

async function CheckUsernameExists(data) {
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    let response;

    try {
        response = await axios.get('/User/CheckUsernameExists', {
            params: { username: data.userName },
            headers: { 'X-CSRF-TOKEN': token }
        });
    } catch (error) {
        console.error("error", error)
        return false;
    }

    return response.data;
}

async function validateName(data) {

    if (!data.firstname) {
        document.getElementById('error-firstname-req').style.display = 'block';
        document.getElementById('firstName').classList.add('border-error');
    }

    if (!data.lastname) {
        document.getElementById('error-lastname-req').style.display = 'block';
        document.getElementById('lastName').classList.add('border-error');
    }
}

async function validatePassword(input, data) {

    input = document.getElementById('password');

    //TODO: regex to check if the password has special characters, letter casing, and numbers

    if (!data.password || data.password.length < 8) {
        document.getElementById('error-password-min').style.display = 'block';
        input.classList.add('border-error');
    }

    if (data.password && data.password.length > 20) {
        document.getElementById('error-password-max').style.display = 'block';
        input.classList.add('border-error');
    }
}

async function validatePhone(input, data) {

    //phone area code
    input = document.getElementById('phoneAreaCode');

    if (!data.phoneAreaCode) {
        document.getElementById('error-phoneareacode-req').style.display = 'block';
        input.classList.add('border-error');
    }

    if (data.phoneAreaCode && data.phoneAreaCode.toString().length > 5) {
        document.getElementById('error-phoneareacode-max').style.display = 'block';
        input.classList.add('border-error');
    }

    //phone
    input = document.getElementById('phone');

    if (!data.phone) {
        document.getElementById('error-phone-req').style.display = 'block';
        input.classList.add('border-error');
    }


    if (data.phone && data.phone.toString().length > 10) {
        document.getElementById('error-phone-max').style.display = 'block';
        input.classList.add('border-error');
    }

    if (data.phone && data.phone.toString().length < 3) {
        document.getElementById('error-phone-min').style.display = 'block';
        input.classList.add('border-error');
    }

    //phone country code validation not required - will be determined by the users country regardless
}

async function validateEmail(data) {

    if (!data.email || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(data.email)) {
        document.getElementById('error-email-req').style.display = 'block';
        document.getElementById('email').classList.add('border-error');
    }
}

async function validateAddress(input, data) {
    if (!data.streetAddress) {
        document.getElementById('error-streetaddress-req').style.display = 'block';
        document.getElementById('streetAddress').classList.add('border-error');
    }

    if (!data.city) {
        document.getElementById('error-city-req').style.display = 'block';
        document.getElementById('city').classList.add('border-error');
    }

    if (!data.state) {
        document.getElementById('error-state-req').style.display = 'block';
        document.getElementById('state').classList.add('border-error');
    }

    if (!data.country) {
        document.getElementById('error-country-req').style.display = 'block';
        document.getElementById('country').classList.add('border-error');
    }

    //post code
    input = document.getElementById('zipCode');

    if (!data.zipCode) {
        document.getElementById('error-postal-req').style.display = 'block';
        input.classList.add('border-error');
    }

    if (data.zipCode && data.zipCode.toString().length < 3) {
        document.getElementById('error-postal-min').style.display = 'block';
        input.classList.add('border-error');
    }

    if (data.zipCode && data.zipCode.toString().length > 10) {
        document.getElementById('error-postal-max').style.display = 'block';
        input.classList.add('border-error');
    }
}


function hideErrors() {
    Array.from(document.getElementsByClassName('text-error')).forEach(el => el.style.display = 'none'); //replace with classlist hidden
    Array.from(document.querySelectorAll('.form-control')).forEach(el => el.classList.remove('border-error'));
}


export async function init() {
    //load initial partial
    debugger;
    let partial = await getRegisterEmployeePartial();
    document.getElementById("hr-partial-container").innerHTML = partial;

    hideErrors();


    document.querySelectorAll('.primary-btn')?.forEach(el => {
        if (el.id == "register-btn")
            el.addEventListener("click", register);
        else if (el.classList.contains("next-btn"))
            el.addEventListener("click", getNextContainer);
    });

    document.querySelectorAll('.secondary-btn')?.forEach(el => {
        el.addEventListener("click", getPreviousContainer);
    });
}