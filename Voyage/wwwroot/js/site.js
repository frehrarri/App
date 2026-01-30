
//prevent submitting of form inputs by enter key
document.addEventListener("keydown", (e) => {
    if (
        e.key === "Enter" &&
        e.target instanceof HTMLInputElement &&
        e.target.form
    ) {
        e.preventDefault();
    }
});

function showSuccess(success) {
    const popup = document.getElementById('messagePopup');
    if (!popup) return;

    popup.textContent = success ? 'Saved successfully!' : 'Save failed!';
    popup.classList.remove('hidden');

    setTimeout(() => {
        popup.classList.add('hidden');
    }, 3000);
}

//Attaches a debounced "search-as-you-type" autocomplete behavior to an input.
const searchState = new WeakMap();

function addSearchEventListener({
    input,
    resultsContainerId,
    url,
    displayText,    
    valueField,     
    minLength = 2,
    delay = 300
}) {
    const resultsContainer = document.querySelector(resultsContainerId);
    if (!input || !resultsContainer) return;

    // per-input debounce state
    let state = searchState.get(input);
    if (!state) {
        state = { timer: null };
        searchState.set(input, state);
    }

    const query = input.value.trim();
    clearTimeout(state.timer);

    if (query.length < minLength) {
        resultsContainer.innerHTML = "";
        resultsContainer.classList.remove("show");
        return;
    }

    state.timer = setTimeout(async () => {
        try {
            const response = await axios.get(url, {
                params: { query }
            });

            resultsContainer.innerHTML = "";
            debugger;

            response.data.forEach(item => {
                const li = document.createElement("li");
                li.textContent = displayText(item);
                li.dataset.value = item[valueField];

                li.addEventListener("click", () => {
                    input.value = displayText(item);
                    input.dataset.value = item[valueField];
                    resultsContainer.innerHTML = "";
                    resultsContainer.classList.remove("show");
                });

                resultsContainer.appendChild(li);
            });

            resultsContainer.classList.toggle("show", response.data.length > 0);
        }
        catch (err) {
            console.error("Search failed:", err);
        }
    }, delay);
}


function addUserSearchEventListener(input, resultsContainerId) {
    addSearchEventListener({
        input,
        resultsContainerId,
        url: "/User/Search",
        displayText: u => `${u.displayName} (${u.email})`,
        valueField: "id"
    });
}


async function handlePaste(e) {
    e.preventDefault();
    const text = e.clipboardData.getData('text/plain');
    await document.execCommandAsync('insertText', false, text);
}

function handleEnter(e) {
    if (e.key !== 'Enter') return;

    e.preventDefault();

    const selection = window.getSelection();
    if (!selection.rangeCount) return;

    const range = selection.getRangeAt(0);
    range.deleteContents();

    // Insert a <br> node
    const br = document.createElement('br');
    range.insertNode(br);

    // Move the caret **after the <br>**
    range.setStartAfter(br);
    range.setEndAfter(br);

    selection.removeAllRanges();
    selection.addRange(range);
}


function handleFileDrop(e) {
    e.preventDefault();
    const contentDiv = e.currentTarget;
    if (e.dataTransfer.files.length > 0) {
        contentDiv.dataset.file = e.dataTransfer.files[0];
        contentDiv.innerText = e.dataTransfer.files[0].name;
    }
}

function formatUtc(dateString, includeYear = true) {

    let date = new Date(dateString);
    const months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    const day = String(date.getUTCDate()).padStart(2, "0");
    const month = months[date.getUTCMonth()];
    const hours = String(date.getUTCHours()).padStart(2, "0");
    const minutes = String(date.getUTCMinutes()).padStart(2, "0");

    if (includeYear) {
        return `${day} ${month} ${date.getUTCFullYear()}`;
    }

    return `${day} ${month}`;
}

function addSpacesToSentence(sentence, spaces = 1) {
    if (!sentence || !sentence.trim()) {
        return sentence;
    }

    // Insert space(s) between lowercase → uppercase
    const spaceStr = " ".repeat(spaces);

    let spaced = sentence.replace(/([a-z])([A-Z])/g, `$1${spaceStr}$2`);

    // Replace underscores with spaces
    spaced = spaced.replace(/_/g, spaceStr);

    return spaced;
}

async function getPartial(controller, action) {
    try {
        let route = `/${controller}/${action}`;
        return await axios.get(route);
    }
    catch (error) {
        console.error("error: getManageTicketPartial", error);
    }
}