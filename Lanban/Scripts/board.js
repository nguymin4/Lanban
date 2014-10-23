function recalibrate() {
    var windowHeight = window.innerHeight || document.documentElement.clientHeight;
    var windowWidth = window.innerWidth || document.documentElement.clientWidth;
    /*Recalibrate sidebar*/
    document.getElementById("sidebar").style.height = windowHeight;
    document.getElementById("panel").style.height = parseInt(windowHeight) - 120;

    /*Recalibrate other elements*/
    document.getElementById("content").style.width = windowWidth - 61;
    document.getElementById("kanban").style.width = windowWidth - 90;
    var columncount = document.getElementById("kanban").getElementsByTagName("th").length;
    document.getElementsByTagName("colgroup")[0].style.width = parseInt((windowWidth - 86) / columncount) - 2;
}


$(document).ready(function () {
    /*Add customized scroll bar*/
    $(".window-content").perfectScrollbar({
        wheelSpeed: 20,
        wheelPropagation: false,
        minScrollbarLength: 10
    });

    /*Board drag and drop functionality*/
    $(".connected").sortable({
        connectWith: ".connected",
        receive: function (event, ui) {
            if (ui.item.laneType != this.getAttribute("data-lane-type")) {
                $(ui.sender).sortable("cancel");
                document.getElementsByClassName("diaglog")[0].setAttribute("class", "window diaglog show");
            }
            else {
                console.log("receive");
                ui.item.targetLane = this.id;
                updatePosition(this.id, ui.item.index());
                updatePosition(ui.item.startLane, ui.item.startPos);
            }
        },
        start: function (event, ui) {
            ui.item.startLane = this.id;
            ui.item.startPos = ui.item.index();
            ui.item.laneType = this.getAttribute("data-lane-type");
            console.log("start");
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