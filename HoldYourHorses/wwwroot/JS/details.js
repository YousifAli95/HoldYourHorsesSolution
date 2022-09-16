
function UppdateraVarukorg() {
    const pris = document.getElementById('price').innerHTML
    const varor = document.getElementById('AntalVaror').value
    const artikel = document.getElementById('artikelnr').innerHTML
    const namn = document.getElementById('artikelnamn').innerHTML
    const numberofproducts = document.getElementById('number-of-products')
    fetch(`/uppdateravarukorg/?artikelnr=${artikel}&antalvaror=${varor}&price=${pris}&artikelnamn=${namn}`,
        {
            method: "GET",
        }).then(o => o.text()).then(o => (numberofproducts.attributes[2].value) = o);
}
