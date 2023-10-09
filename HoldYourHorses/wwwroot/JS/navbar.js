let searchString = "";
const searchInputs = document.getElementsByClassName("search-input");

Array.from(searchInputs).forEach(input => {
    // Sync all the search input fields with each other so that they have the same value
    input.addEventListener("input", (event) => {
        searchString = event.target.value;
    Array.from(searchInputs).forEach(input => input.value = searchString);
    })

    //Add "Enter" eventListener so you can press the search button by pressing the enter key
    input.addEventListener("keypress", function (event) {
    if (event.key === "Enter") {
        event.preventDefault();
        document.getElementById("search-btn").click();
    }
});

})


function searchFunction() {
    window.location.href = `/?search=${searchString}`;
}

