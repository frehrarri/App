
const moduleEventListeners = [];

function trackEventListener(element, eventType,  handler) {
    if (!element || !eventType || !handler) {
        console.log('could not track event listener, missing params')
        return;
    }

    const listener = {
        element: element,
        eventType: eventType,
        handler: handler
    }

    moduleEventListeners.push(listener)
}

function removeEventListeners() {
    moduleEventListeners.forEach(e => {
        e.element.removeEventListener(e.eventType, e.handler)
    });
    moduleEventListeners.length = 0;
}

///////////////// Debounced search ////////////////////////

//example usage:

//const input = document.getElementById('ticketAssignedTo');
//const results = document.getElementById('search-results');

//input.addEventListener('input', (e) => {
//    handleSearch(e.target.value, 'users', (insert) => {
//        input.value = insert;
//    });
//});

const handleSearch = debounce(async (query, searchName, onSelect) => {
    if (!query.trim()) return;

    let url = null;

    switch (searchName) {
        case 'users':
            url = "/SearchService/Users";
            break;
        case 'teams':
            url = "/SearchService/Teams";
            break;
        default:
            break;
    }

    const response = await axios.get(url, { params: { query } });

    renderResults(response.data, searchName, onSelect);

}, 300);

function renderResults(results, searchName, onSelect) {
    const container = document.getElementById('search-results');

    if (!results.length) {
        container.style.display = 'none';
        return;
    }

    //display popup with list of results
    container.innerHTML = results.map(r => {
        let display = null;
        let insert = null

        switch (searchName) {
            case 'users':
                display = `${r.username} ${r.lastname}, ${r.firstname} - ${r.email}`;
                insert = r.username;
                break;
            case 'teams':
                break;
            default:
                break;
        }

        return `<div class="search-result-item" data-id="${r.id}" data-insert="${insert}">${display}</div>`
    }).join('');

    container.style.display = 'block';

    //on select insert data
    container.querySelectorAll('.search-result-item').forEach(item => {
        item.addEventListener('click', () => {
            onSelect?.(item.dataset.insert, item.dataset.id);
            container.style.display = 'none';
        });
    });
}

//click outside wrapper closes the auto populate popup
document.addEventListener('click', (e) => {
    const container = document.getElementById('search-results');
    if (!container) return;
    if (!e.target.closest('#search-wrapper')) {
        container.style.display = 'none';
    }
});

function debounce(fn, delay = 300) {
    let timer;
    return (...args) => {
        clearTimeout(timer);
        timer = setTimeout(() => fn(...args), delay);
    };
}

//////////////////////////////////////////////////



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
    containerBodyId,
    input,
    resultsContainer,
    url,
    displayText,    
    valueField,     
    minLength = 2,
    delay = 300,
    onSelect
}) {
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

            response.data.forEach(item => {
                const li = document.createElement("li");
                li.textContent = displayText(item);
                li.dataset.value = item[valueField];

                li.addEventListener("click", () => {
                    input.value = displayText(item);
                    input.dataset.value = item[valueField];

                    resultsContainer.innerHTML = "";
                    resultsContainer.classList.remove("show");

                    //callback to handle data selection
                    if (onSelect)
                        onSelect(item);
                });



                resultsContainer.appendChild(li);
            });

            resultsContainer.classList.toggle("show", response.data.length > 0);
            clampResultsToContainer(containerBodyId, resultsContainer, input)
        }
        catch (err) {
            console.error("Search failed:", err);
        }
    }, delay);
}

function clampResultsToContainer(containerBodyId, ul, input) {
    const container = document.getElementById(`${containerBodyId}`);
    if (!container) return;

    const containerRect = container.getBoundingClientRect();
    const inputRect = input.getBoundingClientRect();

    // max width = container right edge - input left
    const maxWidth = containerRect.right - inputRect.left - 8; // small padding
    ul.style.maxWidth = `${maxWidth}px`;
}

function addSearchEventListener(containerBodyId, input, resultsContainer, onSelect, controlType) {
    let url = "";
    let displayText = ""
    switch (controlType) {
        case 1:
            url = "/SearchService/Users";
            displayText = '${u.lastname}, ${u.firstname} [${u.username}] [${u.email}';
            break;
        case 2:
            url = "/SearchService/Teams";
            displayText = '${u.name}';
            break;
        default:
            break;

    }

    input.oninput = async () => {
        const q = input.value.trim();

        if (q.length < 2)
            return;

        const res = await axios.get(url, {
            params: { query: q }
        });

        resultsContainer.innerHTML = "";

        res.data.forEach(u => {
            const li = document.createElement("li");

            li.textContent = displayText

            li.onclick = () => onSelect(u);
            resultsContainer.appendChild(li);
        });
    };
}

function hyperlinkResponse(response, changeTracker, newId, currentVals) {

    //did not add any records so there is nothing to hyperlink
    if (response.data.length > 0) {
        let span = "";

        for (let [key, value] of changeTracker) {

            //skip removals
            if (value.dbChangeAction === 2)
                continue;

            span = document.querySelector(`.add-${newId}-span[data-uid='${key}']`);

            let anchortag = document.createElement('a');
            anchortag.href = "#";
            anchortag.classList.add(`redirect`);
            anchortag.textContent = span.textContent.trim();
            anchortag.dataset.uid = key;

            //set map's redirect value for parsing in grid event
            let entry = currentVals.get(key);
            entry.datakey = response;
         
            span.replaceWith(anchortag);
        }
    }
 
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

function formatUtc(dateString, includeYear = true, includeTime = false) {

    if (!dateString)
        return "";

    let date = new Date(dateString);
    const months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    const day = String(date.getDate()).padStart(2, "0");
    const month = months[date.getMonth()];
    const hours = String(date.getHours()).padStart(2, "0");
    const minutes = String(date.getMinutes()).padStart(2, "0");
    const seconds = String(date.getSeconds()).padStart(2, "0");

    let format = `${day} ${month}`;

    if (includeYear) {
        format = `${format} ${date.getFullYear()}`;
    }

    if (includeTime) {
        format = `${format} ${hours}:${minutes}:${seconds}`;
    }

     return format;
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
        console.error("error: getPartial", error);
    }
}

function expand(li) {
    li.classList.remove('collapsed');
    li.classList.add('expanded');

    const icon = li.querySelector('i');
    icon.classList.remove('fa-angle-right');
    icon.classList.add('fa-angle-down');
}

function collapse(li) {
    li.classList.remove('expanded');
    li.classList.add('collapsed');

    const icon = li.querySelector('i');
    icon.classList.remove('fa-angle-down');
    icon.classList.add('fa-angle-right');
}

