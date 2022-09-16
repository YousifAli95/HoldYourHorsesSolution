//****variables****/
var kategori;
var density;
var horsepowers;
var wood;
var land
var counter = 0;
var timeFactor = 5
//****Get selectors and add animation
const goal = document.querySelector(".goal")
var numberOfHorses = document.querySelector(".numberOfHorses").innerHTML
numberOfHorses = parseInt(numberOfHorses);
goal.style.height = (160 * numberOfHorses) + "px";
let horses = document.querySelectorAll(".horse");
horses.forEach((e) => {
    intKategori = getKategori(e.dataset.kategori);
    density = e.dataset.density;
    horsepowers = e.dataset.horsepowers;
    wood = e.dataset.wood;
    intLand = getCountry(e.dataset.land);
    let t = (intKategori * density* intLand * timeFactor) / horsepowers;
    let child = Array.from(e.children);
    child.forEach((c) => { //adderar animationen till bild, namn och cirkel
        c.style.animationTimingFunction = getBezier(wood);
        c.style.animationDuration = t + "s";
        c.style.animationFillMode = "forwards";
    });
});
let circles = Array.from(document.querySelectorAll(".circle")).sort((a, b) => {
    let x = a.parentElement.dataset;
    let y = b.parentElement.dataset;
    let rtn =
        (getKategori(x.kategori) * x.density * getCountry(x.Country) * timeFactor) / x.horsepowers -
        (getKategori(y.kategori) * y.density * getCountry(y.Country) * timeFactor) / y.horsepowers 
    if (rtn === 0) {
        x.equal = "true";
    }
    return rtn;
});
circles.forEach((e) => {
    if (e.parentElement.dataset.equal == "true") {
        counter;
    } else {
        counter++;
    }
    e.innerHTML = counter + ":a";
});

//****listeners****/

//**** functions*****/

function getBezier(material) {
    console.log(material);
    if (material == "Ek") {
        return "cubic-bezier(0.27, 0.56, 0, 1.02)"
    }
    else if (material == "Furu") {

        return "cubic-bezier(0.74, 0.22, 0.6, 0.86"
    }
    else if (material == "Mahogony") {
        return "cubic-bezier(.45, .45, .75, .75)"
       
    }
    else if (material == "Gran") {
        return "cubic-bezier(.97, -1.01, 0, .12)"
        console.log("´gran")
    }
    else {
        return "cubic-bezier(0, 0.45, 0.51, 0.82)"
    }
}

function getKategori(kategori) {
    if (kategori == "Sport") {
        return 0.7;

    }
    else if (kategori == "Fritid") {
        return 1
    }
    else if (kategori == "Barn") {
        return 1.3
    }
    else { return (1.1) }
}
function getCountry(Country) {
    if (kategori == "Sverige") {
        return 0.7;

    }
    else if (kategori == "Norge") {
        return 1
    }
    else if (kategori == "Danmark") {
        return 1.3
    }
    else { return (1.1) }
}