function recalibrate() {
    var windowHeight = window.innerHeight || document.documentElement.clientHeight;
    var windowWidth = window.innerWidth || document.documentElement.clientWidth;
    /*Recalibrate sidebar*/
    document.getElementById("sidebar").style.height = windowHeight;
    document.getElementById("panel").style.height = parseInt(windowHeight) - 120;

    /*Recalibrate other elements*/
    document.getElementById("content").style.width = windowWidth - 61;
    document.getElementById("kanbanWindow").style.width = windowWidth - 90;
    var columncount = document.getElementById("kanban").getElementsByTagName("th").length;
    var colgroup = document.getElementsByTagName("colgroup")[0];
    colgroup.innerHTML = "";
    for (var i = 0; i < columncount; i++) {
        colgroup.innerHTML += "<col />";
    }
    colgroup.style.width = parseInt((windowWidth - 86) / columncount) - 2;
}

// Show Add backlog/task window
function showWindow(windowName) {
    $("#kanbanWindow").removeClass("show");
    setTimeout(function () {
        $("#kanbanWindow").css("display", "none");
        $("#" + windowName).css("display", "block").addClass("show");
        $(viewIndicator[0]).removeClass("show").on("click", function () {
            hideWindow();
        });
    }, 250);
}

// Hide Add backlog/task window
// Open the kanban board
function hideWindow() {
    var window = $(".window.show");
    $(window).removeClass("show");
    setTimeout(function () {
        $(window).css("display", "none");
        $("#kanbanWindow").css("display", "block").addClass("show");
        $(viewIndicator[0]).addClass("show");
    }, 250);
}

$(document).ready(function () {
    /*Add customized scroll bar*/
    $(".window-content").perfectScrollbar({
        wheelSpeed: 20,
        wheelPropagation: false,
        minScrollbarLength: 10
    });

    $(".btnOK").on("click", function () {
        $(".diaglog.show").removeClass("show");
    });

    /*Board drag and drop functionality*/
    $(".connected").sortable({
        connectWith: ".connected",
        receive: function (event, ui) {
            var type = this.getAttribute("data-lane-type");
            if ((type == 3) || (ui.item.laneType == type) || (ui.item.laneType == 3)) {
                console.log("receive");
                ui.item.targetLane = this.id;
                updatePosition(this.id, ui.item.index());
                updatePosition(ui.item.startLane, ui.item.startPos);
            }
            else {
                $(ui.sender).sortable("cancel");
                document.getElementsByClassName("diaglog")[0].setAttribute("class", "window diaglog show");
            }
        },
        start: function (event, ui) {
            ui.item.startLane = this.id;
            ui.item.startPos = ui.item.index();
            ui.item.laneType = $(ui.item).attr("data-type");
            console.log("start" + ui.item.laneType);
        },
        stop: function (event, ui) {
            if ((ui.item.targetLane == null) && (ui.item.startPos != ui.item.index())) {
                console.log("stop");
                var startPos = ui.item.startPos;
                var targetItem = this.getElementsByTagName("div")[startPos].getAttribute("id");
                swapPosition(targetItem, startPos, ui.item.id, ui.item.index);
            }
        }
    }).disableSelection();
});

/* In a swimlane, when two items swap positon to each other then
    - call AJAX function to save new position into the database */
function swapPosition(id1, pos1, id2, pos2) {
    console.log("Swap");
}

/* When an item is moved to another swim lane then 
    call AJAX function to save new position of all items:
    - in the source lane with index <= old index of the moved item
    - in the target lane with index <= new index of the moved item */
function updatePosition(lane, pos) {
    console.log("Update " + lane + " " + pos);
}

