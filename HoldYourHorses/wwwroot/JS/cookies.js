window.onload = function () {
    if (document.cookie.indexOf('.AspNet.Consent') == -1) {
            var popup = document.getElementById("popup");
            var overlay = document.getElementById("overlay");
            overlay.style.display = "block";
            popup.style.display = "block";
    }
    try {
        document.getElementById("cookiebutton").onclick = function () {
            var popup = document.getElementById("popup");
            var overlay = document.getElementById("overlay");
            overlay.style.display = "none";
            popup.style.display = "none";
        };
    }
    catch (error) { }
        
    };


