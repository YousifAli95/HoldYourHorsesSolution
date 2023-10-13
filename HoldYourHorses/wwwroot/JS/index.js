const fromSlider = document.querySelector("#fromSlider");
const toSlider = document.querySelector("#toSlider");
const fromInput = document.querySelector("#fromInput");
const toInput = document.querySelector("#toInput");
const fromSliderHK = document.querySelector("#fromSliderHK");
const toSliderHK = document.querySelector("#toSliderHK");
const fromInputHK = document.querySelector("#fromInputHK");
const toInputHK = document.querySelector("#toInputHK");
const allTypes = document.querySelectorAll(".kategori");
const allMaterials = document.querySelectorAll(".material");
const selectElement = document.querySelector("#sort");
const filterElement = document.querySelector(".filter");
const mainElement = document.querySelector(".main-div");
const hamburgerMenuSVG = document.querySelector("#hamburger");

const SVG_COMPARE_FILL_CLASS = "svg-compare-fill"
const HEART_FILL_CLASS = "heart-fill"

var maxPrice = toInput.value;

var minPrice = fromInput.value;
var maxHK = toInputHK.value;
var minHK = fromInputHK.value;
var sortOn = selectElement.value.substr(1);
var isAscending = true;
var types = "-";
var materials = "-";
var numberOfCompares = 0;

// EventListeners
allTypes.forEach((o) => {
    types += " " + o.value;
    o.addEventListener("change", function () {
        if (this.checked) {
            types += " " + this.value;
            getPartialView();
        } else {
            types = types.replace(" " + this.value, "");
            getPartialView();
        }
    });
});
allMaterials.forEach((o) => {
    materials += " " + o.value;
    o.addEventListener("change", function () {
        if (this.checked) {
            materials += " " + this.value;
            getPartialView();
        } else {
            materials = materials.replace(" " + this.value, "");
            getPartialView();
        }
    });
});
const constMaxPrice = maxPrice;
const constMinPrice = minPrice;
const constSortOn = sortOn;
const constMaxHK = maxHK;
const constMinHK = minHK;
const constIsAscending = isAscending;
const constMaterials = materials;
const constTyper = types;

selectElement.addEventListener("change", (event) => {
    sortOn = event.target.value.slice(1);
    isAscending = Boolean(parseInt(event.target.value.substr(0, 1)));
    getPartialView();
});

//price  event listener
fromSlider.addEventListener("change", (event) => {
    minPrice = event.target.value;
    getPartialView();
});

toSlider.addEventListener("change", (event) => {
    maxPrice = event.target.value;
    getPartialView();
});

fromInput.addEventListener("change", (event) => {
    minPrice = event.target.value;
    getPartialView();
});

toInput.addEventListener("change", (event) => {
    maxPrice = event.target.value;
    getPartialView();
});

//Hästkrafter event listener
fromSliderHK.addEventListener("change", (event) => {
    minHK = event.target.value;
    getPartialView();
});

toSliderHK.addEventListener("change", (event) => {
    maxHK = event.target.value;
    getPartialView();
});

fromInputHK.addEventListener("change", (event) => {
    minHK = event.target.value;
    getPartialView();
});

toInputHK.addEventListener("change", (event) => {
    maxHK = event.target.value;
    getPartialView();
});

//functions
async function getPartialView() {
    const superContainer = document.querySelector(".card-container");
    const url = `/IndexPartial/?maxPrice=${maxPrice}&minPrice=${minPrice}&maxHK=${maxHK}&minHK=${minHK}&typer=${types}&materials=${materials}&sortOn=${sortOn}&isAscending=${isAscending}`;
    await fetch(url,
        { method: "GET" }
    )
        .then((result) => result.text())
        .then((html) => {
            superContainer.innerHTML = html;
        })
        .then((o) => {
            getCompare();
            getHearts();
        });
}

async function resetFilter() {
    {
        toSlider.value = constMaxPrice;
        fromSlider.value = constMinPrice;
        toSliderHK.value = constMaxHK;
        fromSliderHK.value = constMinHK;
        toInput.value = constMaxPrice;
        fromInput.value = constMinPrice;
        toInputHK.value = constMaxHK;
        fromInputHK.value = constMinHK;

        controlFromSlider(fromSlider, toSlider, fromInput);
        controlToSlider(fromSlider, toSlider, toInput);
        controlFromInput(fromSlider, fromInput, toInput, toSlider);
        controlToInput(toSlider, fromInput, toInput, toSlider);
        controlFromSlider(fromSliderHK, toSliderHK, fromInputHK);
        controlToSlider(fromSliderHK, toSliderHK, toInputHK);
        controlFromInput(fromSliderHK, fromInputHK, toInputHK, toSliderHK);
        controlToInput(toSliderHK, fromInputHK, toInputHK, toSliderHK);
    }

    allTypes.forEach((o) => {
        o.checked = true;
    });
    allMaterials.forEach((o) => (o.checked = true));

    maxPrice = constMaxPrice;
    minPrice = constMinPrice;
    sortOn = constSortOn;
    maxHK = constMaxHK;
    minHK = constMinHK;
    isAscending = constIsAscending;
    materials = constMaterials;
    types = constTyper;
    getPartialView();
}

function hideProperty(id, minus) {
    if (id.style.height != "0px") {
        id.style.height = "0";
    } else {
        id.style.height = "auto";
    }
    var id = "svg" + minus;
    var minusP = document.getElementById(id);
    if (minusP.innerHTML == '<path d="M0 10h24v4h-24z"></path>') {
        minusP.innerHTML = '<path d="M24 9h-9v-9h-6v9h-9v6h9v9h6v-9h9z"></path>';
    } else if (
        minusP.innerHTML == '<path d="M24 9h-9v-9h-6v9h-9v6h9v9h6v-9h9z"></path>'
    ) {
        minusP.innerHTML = '<path d="M0 10h24v4h-24z"></path>';
    }
}

async function compare(articleNr) {
    let added;
    const url = `/api/add-or-remove-compare/${articleNr}`
    await fetch(url, { method: "GET" })
        .then((response) => response.json())
        .then((data) => added = data.added);
    console.log(added);
    const svg = document.querySelector("#svg-" + articleNr);
    if (added === true) {
        if (numberOfCompares < 4) {
            svg.classList.add(SVG_COMPARE_FILL_CLASS);
            numberOfCompares++;
        } else {
            alert("Du kan inte jämföra fler än fyra käpphästar samtidigt!");
        }
    } else {
        svg.classList.remove(SVG_COMPARE_FILL_CLASS);
        numberOfCompares--;
    }

    ShowOrHideCompareButton();
}
async function getCompare() {
    const url = "/api/get-compare";
    const fetchedArticleList = await fetch(url, { method: "GET" });
    try {
        const data = await fetchedArticleList.json();
        const articleList = data.compareData;
        numberOfCompares = articleList.length;

        for (let index = 0; index < articleList.length; index++) {
            const svg = document.querySelector("#svg-" + articleList[index]);
            if (svg != null) {
                svg.classList.add(SVG_COMPARE_FILL_CLASS);
            }
        }
    } catch (error) {
        numberOfCompares = 0;
    }

    ShowOrHideCompareButton();
}

let isShown = true;

function showHideFilter() {
    const FILTER_CLOSE_CLASS = "filter-closed";
    const MAIN_DIV_CLOSED = "main-div-closed";

    const listItems = filterElement.children;

    for (var i = 1; i < listItems.length; i++) {
        if (isShown) {
            listItems[i].style.display = "none";
        }
        else {
            setTimeout(
                function (a) {
                    a.style.display = "block";
                },
                300,
                listItems[i]
            );
        }
    }

    isShown = !isShown;
    filterElement.classList.toggle(FILTER_CLOSE_CLASS);
    if (isShown) {
        hamburgerMenuSVG.style.transform = "";
        mainElement.classList.add(MAIN_DIV_CLOSED);
    }
    else {
        mainElement.classList.remove(MAIN_DIV_CLOSED);
        hamburgerMenuSVG.style.transform = "rotate(0)"
    }

}

window.onbeforeunload = function (e) {
    getCompare();
    getHearts();
};

async function removeCompare() {
    url = `/api/remove-all-comparisons`;
    await fetch(url, { method: "DELETE" });
    var articles = document.querySelectorAll(".compare-svg");
    articles.forEach((svg) => (svg.classList.remove(SVG_COMPARE_FILL_CLASS)));

    numberOfCompares = 0;
    ShowOrHideCompareButton();
}

function ShowOrHideCompareButton() {
    var button = document.getElementById("compare-btn");
    console.log(numberOfCompares);
    if (numberOfCompares < 2) {
        button.style.display = "none";
    } else {
        button.style.display = "";
    }
}

async function addHeart(svg, artikelNr) {
    let didAddHeart;
    const url = `/api/add-or-remove-favourite/${artikelNr}`
    await fetch(url, { method: "GET" })
        .then((o) => o.json())
        .then((o) => (didAddHeart = o.added));
    console.log(didAddHeart);

    if (didAddHeart === true) {
        svg.classList.add(HEART_FILL_CLASS);
    } else {
        svg.classList.remove(HEART_FILL_CLASS);
    }
}

async function getHearts() {
    const fetchedArticleNumbers = await fetch(`/api/favourites`);
    try {
        articleNumbers = await fetchedArticleNumbers.json();
        console.log(articleNumbers);
        for (let index = 0; index < articleNumbers.length; index++) {
            const svg = document.querySelector("#svg2-" + articleNumbers[index]);
            if (svg != null) {
                svg.classList.add(HEART_FILL_CLASS);
            }
        }
    } catch (error) { }
}

///// Slider Javascript code /////
function controlFromInput(fromSlider, fromInput, toInput, controlSlider) {
    const [from, to] = getParsed(fromInput, toInput);
    fillSlider(fromInput, toInput, "black", "#7b63ad", controlSlider);
    if (from > to) {
        fromSlider.value = to;
        fromInput.value = to;
    } else {
        fromSlider.value = from;
    }
}

function controlToInput(toSlider, fromInput, toInput, controlSlider) {
    const [from, to] = getParsed(fromInput, toInput);
    fillSlider(fromInput, toInput, "black", "#7b63ad", controlSlider);
    setToggleAccessible(toInput);
    if (from <= to) {
        toSlider.value = to;
        toInput.value = to;
    } else {
        toInput.value = from;
    }
}

function controlFromSlider(fromSlider, toSlider, fromInput) {
    const [from, to] = getParsed(fromSlider, toSlider);
    fillSlider(fromSlider, toSlider, "black", "#7b63ad", toSlider);
    if (from > to) {
        fromSlider.value = to;
        fromInput.value = to;
    } else {
        fromInput.value = from;
    }
}

function controlToSlider(fromSlider, toSlider, toInput) {
    const [from, to] = getParsed(fromSlider, toSlider);
    fillSlider(fromSlider, toSlider, "black", "#7b63ad", toSlider);
    setToggleAccessible(toSlider);
    if (from <= to) {
        toSlider.value = to;
        toInput.value = to;
    } else {
        toInput.value = from;
        toSlider.value = from;
    }
}

function getParsed(currentFrom, currentTo) {
    const from = parseInt(currentFrom.value, 10);
    const to = parseInt(currentTo.value, 10);
    return [from, to];
}

function fillSlider(from, to, sliderColor, rangeColor, controlSlider) {
    const rangeDistance = to.max - to.min;
    const fromPosition = from.value - to.min;
    const toPosition = to.value - to.min;
    controlSlider.style.background = `linear-gradient(
      to right,
      ${sliderColor} 0%,
      ${sliderColor} ${(fromPosition / rangeDistance) * 100}%,
      ${rangeColor} ${(fromPosition / rangeDistance) * 100}%,
      ${rangeColor} ${(toPosition / rangeDistance) * 100}%, 
      ${sliderColor} ${(toPosition / rangeDistance) * 100}%, 
      ${sliderColor} 100%)`;
}

function setToggleAccessible(currentTarget) {
    const toSlider = document.querySelector("#toSlider");
    if (Number(currentTarget.value) <= 0) {
        toSlider.style.zIndex = 2;
    } else {
        toSlider.style.zIndex = 0;
    }
}

fillSlider(fromSlider, toSlider, "black", "#7b63ad", toSlider);
setToggleAccessible(toSlider);

fillSlider(fromSliderHK, toSliderHK, "black", "#7b63ad", toSliderHK);
setToggleAccessible(toSliderHK);

fromSlider.oninput = () => controlFromSlider(fromSlider, toSlider, fromInput);
toSlider.oninput = () => controlToSlider(fromSlider, toSlider, toInput);
fromInput.oninput = () =>
    controlFromInput(fromSlider, fromInput, toInput, toSlider);
toInput.oninput = () => controlToInput(toSlider, fromInput, toInput, toSlider);

fromSliderHK.oninput = () =>
    controlFromSlider(fromSliderHK, toSliderHK, fromInputHK);
toSliderHK.oninput = () => controlToSlider(fromSliderHK, toSliderHK, toInputHK);
fromInputHK.oninput = () =>
    controlFromInput(fromSliderHK, fromInputHK, toInputHK, toSliderHK);
toInputHK.oninput = () =>
    controlToInput(toSliderHK, fromInputHK, toInputHK, toSliderHK);

/////// Script starts here ///////
getPartialView();

// Closes the filter sidebar if the site is opened on a screen smaller than 700px.
const windowWidth = window.innerWidth;
console.log(windowWidth);
if (windowWidth < 700)
    showHideFilter();