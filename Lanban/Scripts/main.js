$(document).ready(function() {
    /*Recalibrate based on browser size*/
    recalibrate();
    window.addEventListener('resize', function () {
        recalibrate();
    });

    /*Make working window draggable*/
    $(".window").draggable({
        cursor: "move",
        handle: ".title-bar"
    });

    /*Add function when click the icon to change window*/
    viewIndicator = document.getElementsByClassName("viewIndicator");
    view = document.getElementsByClassName("view");
    currentView = 0;
    $(".viewIndicator").on("click", function () {
        showView(this.getAttribute("data-view-indicator"));
    });
});

//Function to change working window
var view;
var currentView;
var viewIndicator
function showView(index) {
    if (index != currentView) {
        //Hide secondary window when it is opening in the Board.aspx
        if (document.getElementById("kanbanWindow") != null)
            if (!((currentView!=0)&&(index!=0))) hideWindow();

        //Change main window
        view[currentView].setAttribute("class", "window view");
        setTimeout(function () {
            view[currentView].style.display = "none";
            viewIndicator[currentView].setAttribute("class", "viewIndicator");
            view[index].style.display = "block";
            view[index].setAttribute("class", "window view show");
            viewIndicator[index].setAttribute("class", "viewIndicator show")
            currentView = index;
        }, 250);
    }
}
