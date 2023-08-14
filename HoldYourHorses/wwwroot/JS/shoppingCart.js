const numberofproducts = document.getElementById('number-of-products');
const totalSumElement = document.getElementById('total-sum-span');

function deleteItem(articleNr, price, amount) {
    const articleContainer = document.getElementById("article-container");

    fetch(`/remove-from-shopping-cart/${articleNr}`,
        {
            method: "DELETE",
        }).then(() => {

            const article = document.getElementById(`article-${articleNr}`);
            articleContainer.removeChild(article);

            totalSum = getTotalPrice() - price * amount
            totalSumElement.innerHTML = formatPrice(totalSum);

            numberofproducts.attributes[2].value = parseInt(numberofproducts.attributes[2].value) - amount;

            checkIfEmptyShoppingCart();

        });
}

function checkIfEmptyShoppingCart() {
    if (numberofproducts.attributes[2].value == 0) {
        const emptyShoppingCartDiv = document.getElementById("empty-shopping-cart-div");
        const articleContainer = document.getElementById("article-container");

        emptyShoppingCartDiv.classList.toggle("hide");
        articleContainer.classList.toggle("hide");
    };
}

function clearShoppingCart() {
    fetch("/clear-cart",
        {
            method: "DELETE",
        })
        .then(o => {
            numberofproducts.attributes[2].value = 0;
            checkIfEmptyShoppingCart();
        })
}

function getTotalPrice() {
    const totalSum = totalSumElement.innerHTML.replace(/\s+/g, '').replace("kr", "").replace(/&nbsp;/g, '')
    return parseInt(totalSum);
}

function formatPrice(price) {
    const nfi = {
        ...Intl.NumberFormat().resolvedOptions(),
        numberGroupSeparator: " "
    };

    const formattedPrice = price.toLocaleString(undefined, {
        ...nfi,
        style: "currency",
        currency: "SEK",
        minimumFractionDigits: 0,
        maximumFractionDigits: 0
    });

    return formattedPrice;
}