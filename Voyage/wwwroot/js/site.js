
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
function addSearchEventListener({
    inputId,               // ID of the input element to attach the search behavior to
    resultsContainerId,     // ID of the container where results will be rendered
    url,                    // Backend endpoint to call (e.g. "/Users/Search")
    renderItem,             // Function that returns a DOM element for a single result
    onSelect,               // Callback invoked when a result is selected
    minLength = 2,          // Minimum characters required before searching
    delay = 300            // Debounce delay (ms)
}) {
    const input = document.getElementById(inputId);
    const resultsContainer = document.getElementById(resultsContainerId);

    // Fail fast if required elements are missing
    if (!input || !resultsContainer) return;

    // Debounce timer is scoped to this instance so multiple searches don't interfere
    let debounceTimer;

    input.addEventListener('input', (e) => {
        const query = e.target.value.trim();

        // Cancel any pending request caused by previous keystrokes
        clearTimeout(debounceTimer);

        // Avoid server calls for short or empty input
        if (query.length < minLength) {
            resultsContainer.innerHTML = '';
            resultsContainer.classList.remove('show');
            return;
        }

        // Delay execution until the user pauses typing
        debounceTimer = setTimeout(async () => {
            try {
                const response = await axios.get(url, {
                    params: { query }
                });

                // Replace existing results with the latest response
                resultsContainer.innerHTML = '';

                response.data.forEach(item => {
                    // Caller controls how each result is rendered
                    const element = renderItem(item);

                    element.addEventListener('click', () => {
                        // Caller controls what "selecting" a result means
                        onSelect(item);
                        resultsContainer.innerHTML = '';
                    });

                    resultsContainer.appendChild(element);
                });

                resultsContainer.classList.toggle('show', response.data.length > 0);

            } catch (err) {
                console.error('Autocomplete search failed:', err);
            }
        }, delay);
    });

}

function addUserSearchEventListener(inputId, resultsContainerId) {
    addSearchEventListener({
        inputId,
        resultsContainerId,
        url: '/User/Search',

        renderItem: (user) => {
            const li = document.createElement('li');
            li.textContent = `${user.displayName} (${user.email})`;
            li.dataset.userId = user.id;
            return li;
        },

        onSelect: (user) => {
            const input = document.getElementById(inputId);
            input.value = user.displayName;
            input.dataset.userId = user.id;

            // hide dropdown on select
            document.getElementById(resultsContainerId).classList.remove('show');
        }
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