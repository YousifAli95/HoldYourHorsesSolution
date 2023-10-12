function openOrCloseOrder(svgDiv, orderId) {
    const HIDE_CLASS = "hide";
    // Array with the plus and minus sign
    const paths = svgDiv.querySelectorAll("path");

    // Change the minus sign to plus sign and vice versa
    for (const path of paths) {
        path.classList.toggle(HIDE_CLASS);
    }

    // Hides and shows the article container
    const elementId = "articles-container-" + orderId;
    const articleContainer = document.getElementById(elementId);
    articleContainer.classList.toggle(HIDE_CLASS);



}
