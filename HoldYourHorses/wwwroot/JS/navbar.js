function searchFunction() {
    searchString = document.getElementById("search-input").value;
    console.log(searchString);
    window.location.href = `/?search=${searchString}`;
}

var input = document.getElementById("search-input");
input.addEventListener("keypress", function (event) {
    if (event.key === "Enter") {
        event.preventDefault();
        document.getElementById("search-btn").click();
    }
});