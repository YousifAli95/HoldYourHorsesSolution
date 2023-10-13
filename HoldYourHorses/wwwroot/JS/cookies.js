const button = document.getElementById("cookie-button");

console.log(button)

// Closes the modal and sets the consent cookie when the user is pressing the button
button.addEventListener("click", () => {
    console.log(document.getElementById("cookie-dialog"))
    document.cookie = button.dataset.cookieString;
    document.getElementById("cookie-dialog").close();
    document.getElementById("custom-backdrop").style.display = "none";
});
