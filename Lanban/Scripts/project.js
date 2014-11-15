function recalibrate() {
    var windowHeight = window.innerHeight || document.documentElement.clientHeight;
    var windowWidth = window.innerWidth || document.documentElement.clientWidth;
    /*Recalibrate sidebar*/
    document.getElementById("sidebar").style.height = windowHeight;
    document.getElementById("panel").style.height = parseInt(windowHeight) - 120;

    /*Recalibrate other elements*/
    document.getElementById("content").style.width = windowWidth - 61;
    document.getElementsByClassName("view")[0].style.width = 0.9 * (windowWidth - 61);
}

function openAddProjectWindow(index) {
    showView(0);
    var rightContent = document.getElementsByClassName("right-window-content");
    if (index == 1) {
        rightContent[0].setAttribute("class", "right-window-content");
        setTimeout(function () {
            rightContent[0].style.display = "none";
            rightContent[1].setAttribute("class", "right-window-content show");
        }, 500);
    }
    else {
        rightContent[1].setAttribute("class", "right-window-content");
        setTimeout(function () {
            rightContent[0].style.display = "block";
        }, 450);
        setTimeout(function () {
            rightContent[0].setAttribute("class", "right-window-content show");
        }, 500);
    }
}

$(document).ready(function () {
    /*Add customized scroll bar*/
    $("#projectbrowser").perfectScrollbar({
        wheelSpeed: 20,
        wheelPropagation: false,
    });
});

