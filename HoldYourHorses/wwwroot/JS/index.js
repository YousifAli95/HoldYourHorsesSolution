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

var maxPrice = toInput.value;

var minPrice = fromInput.value;
var maxHK = toInputHK.value;
var minHK = fromInputHK.value;
var sortOn = "Pris";
var isAscending = true;
var typer = "-";
var materials = "-";
var searchString = "";
var numberOfCompares = 0;

//event listeners
allTypes.forEach((o) => {
  typer += " " + o.value;
  o.addEventListener("change", function() {
    if (this.checked) {
      typer += " " + this.value;
      getPartialView();
    } else {
      typer = typer.replace(" " + this.value, "");
      getPartialView();
    }
  });
});
allMaterials.forEach((o) => {
  materials += " " + o.value;
  o.addEventListener("change", function() {
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
const constTyper = typer;
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
  await fetch(
    `/IndexPartial/?maxPrice=${maxPrice}&minPrice=${minPrice}&maxHK=${maxHK}&minHK=${minHK}&typer=${typer}&materials=${materials}&sortOn=${sortOn}&isAscending=${isAscending}`,
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
  typer = constTyper;
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

async function compare(artikelnr, artikelNamn) {
  var didAdd;
  await fetch(`/compareAdd/?artikelnr=${artikelnr}`)
    .then((o) => o.text())
    .then((o) => (didAdd = o));
  console.log(didAdd);
  const svg = document.querySelector("#svg-" + artikelnr);
  if (didAdd == "True") {
    if (numberOfCompares < 4) {
      svg.style.fill = "#7b63ad";
      numberOfCompares++;
    } else {
      alert("Du kan inte jämföra fler än fyra käpphästar samtidigt!");
    }
    ShowOrHideCompareButton();
  } else {
    svg.style.fill = "";
    numberOfCompares--;
    ShowOrHideCompareButton();
  }
}
async function getCompare() {
  var articleList = await fetch("/getCompare");
  try {
    articleList = await articleList.json();
    numberOfCompares = articleList.length;
    for (let index = 0; index < articleList.length; index++) {
      const svg = document.querySelector("#svg-" + articleList[index]);
      if (svg != null) {
        svg.style.fill = "#7b63ad";
      }
    }
  } catch (error) {
    numberOfCompares = 0;
  }
  ShowOrHideCompareButton();
}

var isShown = true;

var filterstyle = document.querySelector(".filter").style;
function showHideFilter() {
  const filter = document.querySelector(".filter");
  const listItems = filter.children;
  const listArray = [...listItems];
  listArray.shift();
  const svg = document.querySelector("#hamburger");
  console.log(svg);
  if (isShown) {
    for (var i = 0; i < listArray.length; i++) {
      listArray[i].style.display = "none";
    }
    isShown = false;
    console.log(filter);
    filter.style.minWidth = "6rem";
    filter.style.border = "0px solid black";
    svg.style.transform = "rotate(0)";
  } else {
    isShown = true;
    filter.style = filterstyle;

    svg.style.transform = "rotate(90deg)";

    for (var i = 0; i < listArray.length; i++) {
      setTimeout(
        function(a) {
          a.style.display = "block";
        },
        300,
        listArray[i]
      );
      console.log(listArray[i]);
    }
  }
}

window.onbeforeunload = function(e) {
  getCompare();
  getHearts();
};

async function removeCompare() {
  await fetch(`/removeCompare`, { method: "GET" });
  var articles = document.querySelectorAll(".compare-svg");
  articles.forEach((e) => (e.style.fill = ""));
  numberOfCompares = 0;
  ShowOrHideCompareButton();
}

function ShowOrHideCompareButton() {
  var button = document.querySelector(".compare-btn");
  console.log(numberOfCompares);
  if (numberOfCompares < 1) {
    button.style.display = "none";
  } else {
    button.style.display = "block";
  }
}

async function addHeart(svg, artikelNr) {
  var didAddHeart;
  await fetch(`/addFavourite/?artikelnr=${artikelNr}`)
    .then((o) => o.text())
    .then((o) => (didAddHeart = o));
  console.log(didAddHeart);
  if (didAddHeart == "True") {
    svg.style.fill = "rgb(248,48,95)";
  } else {
    svg.style.fill = "";
  }
}

async function getHearts() {
  var articleNumbers = await fetch(`/getFavourites`);
  console.log(articleNumbers);
  try {
    articleNumbers = await articleNumbers.json();
    console.log(articleNumbers);
    for (let index = 0; index < articleNumbers.length; index++) {
      const svg = document.querySelector("#svg2-" + articleNumbers[index]);
      if (svg != null) {
        svg.style.fill = "rgb(248,48,95)";
      }
    }
  } catch (error) {}
}

///// Slider JAvascript code /////
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

///script starts here
getPartialView();
