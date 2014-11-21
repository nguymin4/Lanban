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

// Unload page loading spinner
function unloadPageSpinner() {
    $("#overlay").fadeOut(1000, "swing");
    $("#container").fadeOut(1000, "swing", function () {
        $("#container").fadeIn(1000);
    });
}

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


// Support function for both board.js and project.js
// Parse JSonDate to dd.mm.yyyy
function parseJSONDate(jsonDate) {
    if (jsonDate != null) {
        var y = jsonDate.substr(0, 4);
        var m = jsonDate.substr(5, 2);
        var d = jsonDate.substr(8, 2);
        return d + "." + m + "." + y;
    }
    else
        return "";
}

// Format date
function formatDate(date) {
    if (date != "") {
        var data = date.split(".");
        d = (data[0] < 10) ? "0" + data[0] : data[0];
        m = (data[1] < 10) ? "0" + data[1] : data[1];
        return d + "." + m + "." + data[2];
    }
    return date;
}

