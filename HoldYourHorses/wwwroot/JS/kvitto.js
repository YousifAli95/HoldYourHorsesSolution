function rensakorg() {
    fetch("/rensakorg",
        {
            method: "GET",
        })
        .then(o => {
            const numberofproducts = document.getElementById('number-of-products');
            numberofproducts.attributes[2].value = 0;
        })
}
rensakorg();
