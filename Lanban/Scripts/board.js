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
function showWindow(windowName, i, swimlaneID) {
    $("#kanbanWindow").removeClass("show");
    setTimeout(function () {
        $("#kanbanWindow").css("display", "none");
        $("#" + windowName).css("display", "block").addClass("show");
        $(viewIndicator[0]).removeClass("show").on("click", function () {
            hideWindow();
        });
    }, 250);
    $("#txtSwimlanePosition").val(i);
    $("#txtNoteIndex").val($(".connected")[i].getElementsByTagName("div").length);
    $("#txtSwimlaneID").val(swimlaneID);
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

    //Testing
    $("#btnRefresh").trigger("click");
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

/*1. Create new sticky note and add it on server */
function insertItem(type) {
    var req = new XMLHttpRequest();
    var url = "Board.aspx?action=insert&type=" + type;
    req.onreadystatechange = function () {
        if ((req.readyState == 4) && (req.status == 200)) {
            console.log(req.responseText);
            $("#btnRefresh").trigger("click");
        }
    }
    req.open("GET", url, true)
    req.send();
}

/*2. Change position of a stickynote*/
/*2.1 In a swimlane, when two items swap positon to each other then
    - call AJAX function to save new position into the database */
function swapPosition(id1, pos1, id2, pos2) {
    console.log("Swap");
}

/*2.2 When an item is moved to another swim lane then 
    call AJAX function to save new position of all items:
    - in the source lane with index <= old index of the moved item
    - in the target lane with index <= new index of the moved item */
function updatePosition(lane, pos) {
    console.log("Update " + lane + " " + pos);
}

/*3. Using AJAX to search name of member to assign to a task or backlog */
function addAssignee(obj, type) {
    var objtext = "<div class='assignee-name-active' onclick='removeAssignee(this)'>" + obj.innerHTML + "</div>";
    var searchBox = document.getElementById(type + "Assign").getElementsByTagName("input")[0];
    $(objtext).insertBefore($(searchBox));
    $(searchBox).val("");
    searchBox.focus();
}

function removeAssignee(obj) {
    var parent = obj.parentElement;
    parent.removeChild(obj);
}

function clearResult(obj) {
    setTimeout(function () {
        $(obj).val("");
        $("#assigneeSearchResult").html("").css("display", "none");
    }, 250);
}

var assigneeSearch;
function searchAssignee(searchBox, type) {
    clearInterval(assigneeSearch);
    assigneeSearch = setTimeout(function () {
        var req = new XMLHttpRequest();
        var url = "Handler.ashx?type="+type+"&keyword=" + $(searchBox).val();
        req.onreadystatechange = function () {
            if (req.readyState == 4 && req.status == 200) {
                var result = document.getElementById("assigneeSearchResult");
                result.style.display = "block";
                result.style.top = searchBox.style.top + 120;
                result.style.left = searchBox.style.left;
                result.innerHTML = req.responseText;
            }
        }
        // start asyncronious data transfer
        req.open("GET", url, true);
        req.send(null);
    }, 250);
}


