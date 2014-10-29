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
            var laneID = this.getAttribute("data-id");
            if ((type == 3) || (ui.item.type == type)) {
                ui.item.targetLane = laneID;
                //Update sticky note new swimlane id and position
                changeLane(ui.item.id, type, laneID, ui.item.index());
                $(ui.item).attr("data-swimlane-id", laneID);
                //Update positon of all old sticky notes in that lane based on new insertation position
                var index = ui.item.index;
                var note = this.getElementsByTagName("div");
                for (var i = index; i < note.length; i++) {
                    updatePosition($(note[i]).attr("data-id"), i + 1, type);
                }
            }
            else {
                $(ui.sender).sortable("cancel");
                document.getElementsByClassName("diaglog")[0].setAttribute("class", "window diaglog show");
            }
        },
        start: function (event, ui) {
            ui.item.startLane = this.id;
            ui.item.startPos = ui.item.index();
            ui.item.id = $(ui.item).attr("data-id");
            ui.item.type = $(ui.item).attr("data-type");
        },
        stop: function (event, ui) {
            if ((ui.item.targetLane == null) && (ui.item.startPos != ui.item.index())) {
                var type1 = ui.item.type;
                var startPos = ui.item.startPos;
                var targetId = this.getElementsByTagName("div")[startPos].getAttribute("data-id");
                var type2 = this.getElementsByTagName("div")[startPos].getAttribute("data-type");
                updatePosition(ui.item.id, ui.item.index(), type1);
                console.log(ui.item.id +" " +ui.item.index() +" "+type1)
                updatePosition(targetId, startPos, type2);
                console.log(targetId + " " + startPos + " " + type2)
            }
        }
    }).disableSelection();
});

/*1. Create new sticky note and add it on server */
function insertItem(type) {
    var backlog = {
        Project_ID: $("#txtProjectID").val(),
        Swimlane_ID: $("#txtSwimlaneID").val(),
        Title: $("#txtBacklogTitle").val(),
        Description: $("#txtBacklogDescription").val(),
        Complexity: $("#ddlBacklogComplexity").val(),
        Color: $("#ddlBacklogColor").val(),
        Position: $("#txtNoteIndex").val()
    };
    console.log(JSON.stringify(backlog));
    $.ajax({
        url: "Handler.ashx",
        data: {
            action: "insert" + type,
            backlog: JSON.stringify(backlog)
        },
        global: false,
        type: "post",
        success: function (result) {
            console.log(result);
        }
    });
}

function showInsertWindow(windowName, i, swimlaneID) {
    showWindow(windowName);
    $("#txtSwimlanePosition").val(i);
    $("#txtNoteIndex").val($(".connected")[i].getElementsByTagName("div").length);
    $("#txtSwimlaneID").val(swimlaneID);
}

/*2. Change position of a stickynote*/

/*2.1 In a swimlane, when two items swap positon to each other then
    - call AJAX function to save new position into the database */
function updatePosition(id, pos, type) {
    console.log("Swap");
    var table = (type == 1) ? "Backlog" : "Task";
    $.ajax({
        url: "Handler.ashx",
        data: {
            action: "updatePosition",
            table: table,
            id: id,
            pos: pos
        },
        global: false,
        type: "get",
        success: function (result) {
            console.log(result);
        }
    });
}

/*2.2 When an item is moved to another swim lane then 
    call AJAX function to save new position of all items:
    - in the source lane with index <= old index of the moved item
    - in the target lane with index <= new index of the moved item */
function changeLane(id, type, swimlane_id, pos) {
    var table = (type == 1) ? "Backlog" : "Task";
    $.ajax({
        url: "Handler.ashx",
        data: {
            action: "changeSwimlane",
            table: table,
            id: id,
            swimlane: swimlane_id,
            pos: pos
        },
        global: false,
        type: "get",
        success: function (result) {
            console.log(result);
        }
    });
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
    if ($(searchBox).val() != "") {
        assigneeSearch = setTimeout(function () {
            $.ajax({
                url: "Handler.ashx",
                data: {
                    action: "searchAssignee",
                    projectID: $("#txtProjectID").val(),
                    type: type,
                    keyword: $(searchBox).val()
                },
                global: false,
                type: "get",
                success: function (result) {
                    var searchResult = document.getElementById("assigneeSearchResult");
                    searchResult.style.display = "block";
                    searchResult.style.top = searchBox.style.top + 120;
                    searchResult.style.left = searchBox.style.left;
                    searchResult.innerHTML = result;
                }
            });
        }, 250);
    }
}

/*4. Double click on note to open corresponding window that allow user to edit content*/
function viewDetailNote(windowName, id) {
    showWindow(windowName);
}

