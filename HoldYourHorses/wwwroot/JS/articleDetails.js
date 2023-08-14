function updateShoppingCart() {
    const price = parseFloat(document.getElementById('price').innerHTML);
    const amount = parseInt(document.getElementById('number-of-items').value, 10);
    const articleNr = document.getElementById('article-nr').innerHTML;
    const articleName = encodeURIComponent(document.getElementById('article-name').innerHTML);
    const numberofproducts = document.getElementById('number-of-products');

    const url = `/update-shopping-cart/?articleNr=${articleNr}&amount=${amount}&price=${price}&articleName=${articleName}`;

    fetch(url)
        .then(response => {
            if (!response.ok) {
                console.log(response.text)
                throw new Error('Network response was not ok');
            }
        })
        .then(updatedProductsCount => {
            numberofproducts.attributes[2].value = parseInt(numberofproducts.attributes[2].value) + parseInt(amount);
        })
        .catch(error => {
            console.error('Error updating shopping cart:', error);
        });
}