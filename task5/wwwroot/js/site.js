document.addEventListener('DOMContentLoaded', function () {
    $('#loader').hide();
    loadUsersCurrentSettings();

    setupErrorInput();

    let seed = document.getElementById('seedInput');
    seed.oninput = handleSeedInput;

    addInfiniteScroll();
    
}, false);

function addInfiniteScroll() {
    let tableWrapper = document.getElementById('infiniteTable');
    tableWrapper.addEventListener("scroll", handleTableScroll);
}

function setupErrorInput() {
    let slider = document.getElementById("errorRange");
    let sliderValue = document.getElementById("errorRangeValue");
    let errorInput = document.getElementById('errorInput');

    sliderValue.innerHTML = slider.value;
    slider.oninput = function() {
        sliderValue.innerHTML = this.value;
        errorInput.value = this.value;
        errorInput.dispatchEvent(new Event('input'));
    };

    errorInput.oninput = handleErrorInput;
}

function reloadUsersNewLocale(locale, country) {
    let regionBtn = document.getElementById('regionBtn');
    regionBtn.innerText = country;
    regionBtn.setAttribute('locale', locale)

    reloadUsers();
}

function reloadUsers() {
    let body = document.getElementById('tBody');
    body.innerHTML = "";

    currentPage = 1;
    loadUsersCurrentSettings();
}

let page = {
    currentPage: 1,
    pageSize: 10
};

function nextPage() {
    page.currentPage++;
    loadUsers(
        document.getElementById('usersTable').getAttribute('sendUrl'),
        'tBody',
        document.getElementById('regionBtn').getAttribute('locale'),
        document.getElementById('errorInput').value,
        document.getElementById('seedInput').value,
        page.currentPage,
        page.pageSize);
}

function loadUsersCurrentSettings() {
    loadUsers(
        document.getElementById('usersTable').getAttribute('sendUrl'),
        'tBody',
        document.getElementById('regionBtn').getAttribute('locale'),
        document.getElementById('errorInput').value,
        document.getElementById('seedInput').value)
}

function loadUsers(url, elementId, locale, errors, seed, page = 1, pageSize = 20) {
    $.ajax({
        beforeSend: () => $('#loader').show(),
        complete: () => $('#loader').hide(),
        url: `${url}`,
        type: 'GET',
        cache: false,
        async: true,
        dataType: 'html',
        data: {
            "locale": locale,
            "errors": errors,
            "seed": seed,
            "page": page,
            "pageSize": pageSize
        }
    }).done(result => {
        addRows(elementId, result);
    });
}

function addRows(elementId, result) {
    let body = document.getElementById(elementId);
    body.innerHTML += result;
}

const handleInfiniteScroll = () => {
    throttle(() => {
        const endOfPage =
            window.innerHeight + window.pageYOffset >= document.body.offsetHeight;
        if (endOfPage) {
            nextPage();
        }
    }, 500, tableScrollTimer);
};

function randomSeed() {
    let seedInput = document.getElementById('seedInput');
    seedInput.value = Math.floor(Math.random() * 100_000_000);
}

let errorFieldTimer;
function handleErrorInput() {
    clearTimeout(errorFieldTimer);
    errorFieldTimer = setTimeout(function () {
        let errorInput = document.getElementById('errorInput');
        let slider = document.getElementById("errorRange");
        let sliderValue = document.getElementById("errorRangeValue");

        if (+errorInput.value > +errorInput.getAttribute('max')) {
            errorInput.value = errorInput.getAttribute('max')
        }
        errorInput.value = Math.abs(+errorInput.value);
        slider.value = errorInput.value;
        sliderValue.innerText = slider.value;

        reloadUsers();
    }, 1000); 
}

let seedFieldTimer;
function handleSeedInput() {
    clearTimeout(seedFieldTimer);
    seedFieldTimer = setTimeout(function () {
        let seedInput = document.getElementById('seedInput');
        seedInput.value = Math.floor(Math.abs(+seedInput.value));

        reloadUsers();

    }, 1000);
}

let tableScrollTimer;
function handleTableScroll() {
    clearTimeout(tableScrollTimer);
    tableScrollTimer = setTimeout(function () {
        let tableWrapper = document.getElementById('infiniteTable');
        if (Math.abs(tableWrapper.scrollHeight - tableWrapper.clientHeight - tableWrapper.scrollTop) < 50 && tableWrapper.scrollTop != 0) {
            nextPage();
        }

    }, 500);
}

