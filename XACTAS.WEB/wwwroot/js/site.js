var butmenu_settings = document.getElementById("buttons_menu");
window.onload = function () {
    if (localStorage.getItem("theme") == null) {
        localStorage.setItem("theme", "dark");
    } else if (localStorage.getItem("theme") === "light") {
        document.documentElement.style.setProperty("--background", "white");
        document.documentElement.style.setProperty("--color", "black");
        document.documentElement.style.setProperty("--toggler_style", 'url("../images/toggle/toggle-light.webp")');
    }
}
function Click_ButTheme() {
    if (localStorage.getItem("theme") === "dark") {
        document.documentElement.style.setProperty("--background", "white");
        document.documentElement.style.setProperty("--color", "black");
        document.documentElement.style.setProperty("--toggler_style", 'url("../images/toggle/toggle-light.webp")');
        localStorage.setItem("theme", "light");
    } else if (localStorage.getItem("theme") === "light") {
        document.documentElement.style.setProperty("--background", "black");
        document.documentElement.style.setProperty("--color", "white");
        document.documentElement.style.setProperty("--toggler_style", 'url("../images/toggle/toggle-dark.webp")');
        localStorage.setItem("theme", "dark");
    }
}

window.popoverInit = () => {
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'))
    var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl)
    })
};