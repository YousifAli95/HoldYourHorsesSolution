function clearCart() {
    fetch("/clear-cart",
        {
            method: "DELETE",
        })
        .then(o => {
            const numberofproducts = document.getElementById('number-of-products');
            numberofproducts.attributes[2].value = 0;
        })
}

clearCart();
