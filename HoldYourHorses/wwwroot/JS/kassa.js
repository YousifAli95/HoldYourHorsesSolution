function deleteItem(artikelnr, pris, antal) {
    fetch(`/deleteItem/?artikelnr=${artikelnr}`,
        {
            method: "GET",
        }).then(o => {
            artikelContainer = document.querySelector(".another-container");
            var artikel = document.getElementById(artikelnr);
            artikelContainer.removeChild(artikel);
            var totalsumma = document.getElementById('totalsumma');
            totalsumma.innerHTML = parseInt(totalsumma.innerHTML.replace(" ", "")) - pris * antal + " kr";
            console.log(totalsumma.innerHTML);
            kundvagn();
            const numberofproducts = document.getElementById('number-of-products');
            numberofproducts.attributes[2].value = parseInt(numberofproducts.attributes[2].value) - antal;
            

        })
}
function kundvagn() {
    var totalsumma = document.getElementById('totalsumma');
    var container = document.querySelector(".artikel-container")
    if (totalsumma.innerHTML == "0 kr") {
        container.innerHTML = `<h1>Din kundvagn är tom</h1><button class="tom-kassa-btn" id="fortsättHandla" onclick="location.href='/'">Bläddra bland våra produkter</button>`
    };
}

function rensakorg() {
    fetch("/rensakorg",
        {
            method: "GET",
        })
        .then(o => {
            var container = document.querySelector(".artikel-container")
            container.innerHTML = `<h1>Din kundvagn är tom</h1><button class="tom-kassa-btn" id="fortsättHandla" onclick="location.href='/'">Bläddra bland våra produkter</button>`
            const numberofproducts = document.getElementById('number-of-products');
            numberofproducts.attributes[2].value = 0;
            console.log(numberofproducts)
        })
}
function checkout() {
    window.location = "/checkout";
}
function ShowOrHideButtons() {
    const numberofproducts = document.getElementById('number-of-products');
    console.log(numberofproducts)
    if (numberofproducts.attributes[2].value == 0) {
        document.querySelector("#rensakorg").style.display = "none";
        document.querySelector("#betalning").style.display = "none";
    }
}

kundvagn();
ShowOrHideButtons();