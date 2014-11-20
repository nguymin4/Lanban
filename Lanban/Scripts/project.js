function recalibrate() {
    var windowHeight = window.innerHeight || document.documentElement.clientHeight;
    var windowWidth = window.innerWidth || document.documentElement.clientWidth;
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
    $("#projectbrowser, #projectdetail-description").perfectScrollbar({
        wheelSpeed: 5,
        wheelPropagation: false
    });
});

