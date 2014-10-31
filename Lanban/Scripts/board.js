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
    (windowName == "backlogWindow") ? clearBacklogWindow() : clearTaskWindow();
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

// Show error diaglog  - content taken from an array based on parameter
var errorMsg = ["Cannot drop that item because it is not the same type with the items in column."];
function showErrorDialog(i) {
    $(".diaglog.error .content-holder").text(errorMsg[i]);
    $(".diaglog.error").addClass("show");
}

// Show success diaglog - content taken from an array based on parameter
var successMsg = ["New backlog created"];
function showSuccessDiaglog(i) {
    $(".diaglog.success .title-bar").text("Success");
    $(".diaglog.success .content-holder").text(successMsg[i]);
    $(".diaglog.success input").css("display", "block");
    if (!($(".diaglog.success").hasClass("show")))
        $(".diaglog.success").addClass("show");
}

//Show processing message
function showProcessingDiaglog() {
    $(".diaglog.success .title-bar").text("Processing");
    $(".diaglog.success .content-holder").text("In progress. Please wait.");
    $(".diaglog.success input").css("display", "none");
    $(".diaglog.success").addClass("show");
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
                showErrorDialog(0);
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
                updatePosition(targetId, startPos, type2);
            }
        }
    }).disableSelection();
});

/*1. Create new sticky note and add it on server */
function insertBacklogItem() {
    var backlog = {
        Project_ID: $("#txtProjectID").val(),
        Swimlane_ID: $("#txtSwimlaneID").val(),
        Title: $("#txtBacklogTitle").val(),
        Description: $("#txtBacklogDescription").val(),
        Complexity: $("#ddlBacklogComplexity").val(),
        Color: $("#ddlBacklogColor").val(),
        Position: $("#txtNoteIndex").val()
    };

    $.ajax({
        url: "Handler.ashx",
        data: {
            action: "insertBacklog",
            backlog: JSON.stringify(backlog)
        },
        global: false,
        type: "post",
        success: function (result) {
            saveAssignee(result, "backlog");
            var objtext = "<div class='note' style='background-color:" + backlog.Color + ";' data-type='1' " +
                "data-swimlane-id='" + backlog.Swimlane_ID + "' id='Backlog." + result + "' data-id='" + result + "' " +
                "ondblclick=\"viewDetailNote('backlog'," + result + ")\">" + result + " - " + backlog.Title + "</div>";
            var swimlanePosition = parseInt($("#txtSwimlanePosition").val());
            $(objtext).appendTo($(".connected")[swimlanePosition]);
            showSuccessDiaglog(0);
            clearBacklogWindow();
        }
    });
}

function showInsertWindow(windowName, i, swimlaneID) {
    showWindow(windowName);
    $("#" + windowName + " .btnSave").val("Add").attr("onclick", "insertBacklogItem()");
    $("#txtSwimlanePosition").val(i);
    $("#txtNoteIndex").val($(".connected")[i].getElementsByTagName("div").length);
    $("#txtSwimlaneID").val(swimlaneID);
}

/*2. Change position of a sticky note*/
/*2.1 In a swimlane, when two items swap positon to each other then
    - call AJAX function to save new position into the database */
function updatePosition(itemID, pos, type) {
    var table = (type == 1) ? "Backlog" : "Task";
    $.ajax({
        url: "Handler.ashx",
        data: {
            action: "updatePosition",
            type: table,
            itemID: itemID,
            pos: pos
        },
        global: false,
        type: "get"
    });
}

/*2.2 When an item is moved to another swim lane then 
    call AJAX function to save new position of all items:
    - in the source lane with index <= old index of the moved item
    - in the target lane with index <= new index of the moved item */
function changeLane(itemID, type, swimlane_id, pos) {
    var table = (type == 1) ? "Backlog" : "Task";
    $.ajax({
        url: "Handler.ashx",
        data: {
            action: "changeSwimlane",
            type: table,
            itemID: itemID,
            swimlane: swimlane_id,
            pos: pos
        },
        global: false,
        type: "get",
    });
}

/*3 Working with functionalities involving assignee*/
// View all assignee of an item
function viewAssignee(itemID, type) {
    $.ajax({
        url: "Handler.ashx",
        data: {
            action: "viewAssignee",
            itemID: itemID,
            type: type
        },
        global: false,
        type: "get",
        success: function (result) {
            $("#" + type + "Assign").prepend(result);
        }
    });
}

// Update assignee including delete old records and save new ones
function updateAssignee(itemID, type) {
    //Delete old records
    $.ajax({
        url: "Handler.ashx",
        data: {
            action: "deleteAssignee",
            type: type,
            itemID: itemID
        },
        global: false,
        type: "get",
        success: function () {
            //Save new records
            saveAssignee(itemID, type);
        }
    });
}

// After insert new item and get the item ID, then save the assignee of that item to database
function saveAssignee(itemID, type) {
    var assignee = document.getElementById(type + "Assign").getElementsByTagName("div");
    for (var i = 0; i < assignee.length; i++) {
        $.ajax({
            url: "Handler.ashx",
            data: {
                action: "saveAssignee",
                type: type,
                itemID: itemID,
                assigneeID: assignee[i].getAttribute("data-id"),
                assigneeName: assignee[i].innerHTML
            },
            global: false,
            type: "get"
        });
    }
}

/*3.2 Using AJAX to search name of member to assign to a task or backlog */
// Search assignee name
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

// Add assignee name result
function addAssignee(obj, type) {
    var id = obj.getAttribute("data-id");
    var objtext = "<div class='assignee-name-active' data-id='" + id + "' onclick='removeAssignee(this)'>" + obj.innerHTML + "</div>";
    var searchBox = document.getElementById(type + "Assign").getElementsByTagName("input")[0];
    $(objtext).insertBefore($(searchBox));
    $(searchBox).val("");
    searchBox.focus();
}

// When click on active assignee then it's removed
function removeAssignee(obj) {
    var parent = obj.parentElement;
    parent.removeChild(obj);
}

// Clear search result
function clearResult(obj) {
    setTimeout(function () {
        $(obj).val("");
        $("#assigneeSearchResult").html("").css("display", "none");
    }, 250);
}


/*4. Double click on note to open corresponding window that allow user to edit content*/
function viewDetailNote(type, itemID) {
    showProcessingDiaglog(0);
    showWindow(type + "Window");
    var onclick = (type == "backlog") ? "saveBacklogItem("+itemID+")" : "saveTaskItem("+itemID+")";
    var btnSave = $("#" + type + "Window .btnSave");
    $(btnSave).val("Save").attr("onclick", onclick);

    //Get all data of that item
    $.ajax({
        url: "Handler.ashx",
        data: {
            action: "viewItem",
            itemID: itemID,
            type: type
        },
        global: false,
        type: "get",
        success: function (result) {
            $(".diaglog.success").removeClass("show");
            console.log(result);
            (type == "backlog") ? displayBacklogDetail(result) : displayTaskDetail(result);
        }
    });

    //View all assignee of that item
    viewAssignee(itemID, type);
}

/*4.1.1 Display detail backlog */
function displayBacklogDetail(data) {
    $("#txtSwimlaneID").val(data.Swimlane_ID);
    $("#txtBacklogTitle").val(data.Title);
    $("#txtBacklogDescription").val(data.Description);
    $("#ddlBacklogComplexity").val(data.Complexity);
    $("#ddlBacklogColor").val(data.Color);
}

/*4.1.2 Clear backlog window */
function clearBacklogWindow() {
    $("#backlogWindow .inputbox").val("");
    $("#backlogWindow textarea").val("");
    $("#backlogWindow .assignee-name-active").remove();
}

/*4.2.1 Display detail task */
function displayTaskDetail(data) {

}

/*4.2.2 Clear task Window */
function clearTaskWindow() {
    $("#taskWindow .inputbox").val("");
    $("#taskWindow textarea").val("");
    $("#taskWindow .assignee-name-active").remove();
}


/*5.1 Save changes of the current backlog item to database*/
function saveBacklogItem(itemID) {

}