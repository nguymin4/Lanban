function recalibrate() {
    var windowHeight = window.innerHeight || document.documentElement.clientHeight;
    var windowWidth = window.innerWidth || document.documentElement.clientWidth;

    var columncount = document.getElementById("kanban").getElementsByTagName("th").length;
    var colgroup = document.getElementsByTagName("colgroup")[0];
    colgroup.innerHTML = "";
    for (var i = 0; i < columncount; i++) {
        colgroup.innerHTML += "<col />";
    }
    colgroup.style.width = parseInt((windowWidth - 86) / columncount) - 2;

    $(".connected").css("height", 0.88 * windowHeight - 35);

    $(".window-content").perfectScrollbar("update");
}

// Show Add backlog/task window
function showWindow(windowName) {
    $("#kanbanWindow").removeClass("show");
    changePageWindow(windowName, 0);
    (windowName == "backlogWindow") ? clearBacklogWindow() : clearTaskWindow();
    setTimeout(function () {
        $("#kanbanWindow").css("display", "none");
        $("#" + windowName).css("display", "block").addClass("show");
        $(viewIndicator[0]).removeClass("show").on("click", function () {
            hideWindow();
        });
    }, 250);
}

// Hide Add backlog/task window and Open the kanban board
function hideWindow() {
    var window = $(".window.show");
    $(window).removeClass("show");
    setTimeout(function () {
        $(window).css("display", "none");
        $("#kanbanWindow").css("display", "block").addClass("show");
        $(viewIndicator[0]).addClass("show");
    }, 250);

    // Leave the current Task Hub group
    try {
        proxyTC.invoke("leaveGroup", $("#btnSubmitComment").attr("data-task-id"));
    }
    catch (e) {
        console.log(e);
    }
}

/* In backlogWindow and taskWindow there are 2 pages: */
//1. For adding or viewing some data
//2. For viewing history or tasks of that backlog, or adding document and comment to a task
function changePageWindow(windowName, index) {
    var page = document.getElementById(windowName).getElementsByClassName("page");
    if (index == 1) {
        page[0].style.left = "-100%";
        page[1].style.display = "block";
        setTimeout(function () {
            page[1].style.left = "0";
        }, 300);
        setTimeout(function () {
            page[0].style.display = "none";
        }, 1000);
    }
    else {
        page[1].style.left = "100%";
        page[0].style.display = "block";
        setTimeout(function () {
            page[0].style.left = "0";
        }, 300);
        setTimeout(function () {
            page[1].style.display = "none";
        }, 1000);
    }
    $(page[index]).scrollTop(0);
}


// Show error diaglog  - content taken from an array based on parameter
var errorMsg = ["Cannot drop that item because it is not the same type with the items in column."];

function showErrorDialog(i) {
    $(".diaglog.error .content-holder").text(errorMsg[i]);
    $(".diaglog.error").addClass("show");
}

// Show success diaglog - content taken from an array based on parameter
var successMsg = ["New item created", "Data updated", "File uploaded"];

function showSuccessDiaglog(i) {
    $(".diaglog.success").attr("data-diaglog-type", "Success");
    $(".diaglog.success .title-bar").text("Success");
    $(".diaglog.success .content-holder").html(successMsg[i]);
    $(".diaglog.success input").css("display", "block");
    if (!($(".diaglog.success").hasClass("show")))
        $(".diaglog.success").addClass("show");
}

// Show processing message
function showProcessingDiaglog() {
    $(".diaglog.success").attr("data-diaglog-type", "Processing");
    $(".diaglog.success .title-bar").text("Processing");
    $(".diaglog.success .content-holder").html("<div class='loading-spinner'></div>");
    $(".diaglog.success input").css("display", "none");
    $(".diaglog.success").addClass("show");
}

/*A. When document is ready*/
$(document).ready(function () {
    /* Take the screenshot*/
    takeScreenshot();

    /*Add customized scroll bar*/
    $(".window-content").perfectScrollbar({
        wheelSpeed: 5,
        wheelPropagation: false,
        minScrollbarLength: 10
    });

    $("#commentBox").perfectScrollbar({
        wheelSpeed: 3,
        wheelPropagation: false,
        suppressScrollX: true,
        includePadding: true
    });

    $("#fileList").perfectScrollbar({
        wheelSpeed: 3,
        wheelPropagation: false,
        suppressScrollX: true,
        includePadding: true
    });

    $(".btnOK").on("click", function () {
        $(".diaglog.show").removeClass("show");
    });
    $("#fileUploadContainer input").val("");

    /*Board drag and drop functionality*/
    $(".connected").sortable({
        connectWith: ".connected",
        receive: function (event, ui) {
            var laneType = this.getAttribute("data-lane-type");
            var laneID = this.getAttribute("data-id");
            if ((laneType == 3) || (ui.item.type == laneType)) {
                // To diferentiate between changing lane and updating position
                ui.item.targetLane = laneID;

                //Update sticky note new swimlane id and position
                changeLane(ui.item.id, ui.item.type, laneID, ui.item.index());

                //Update positon of all old sticky notes in destination lane based on new insertation position
                var index = ui.item.index();
                var note = this.getElementsByClassName("note");
                for (var i = index + 1; i < note.length; i++) {
                    updatePosition($(note[i]).attr("data-id"), i, $(note[i]).attr("data-type"));
                }

                // Update position of sticky notes in the source lane.
                note = ui.item.startLane.getElementsByClassName("note");
                for (var i = ui.item.startPos; i < note.length; i++) {
                    updatePosition($(note[i]).attr("data-id"), i, ui.item.type);
                }

                // Update new position in clients
                var projectID = $("#txtProjectID").val();
                var noteID = $(ui.item).attr("id");
                var swimlanePosition = $(".connected").index(this);
                proxyNote.invoke("changePosition", projectID, noteID, swimlanePosition, index);
            }
            else {
                $(ui.sender).sortable("cancel");
                showErrorDialog(0);
            }
        },
        start: function (event, ui) {
            ui.item.startLane = this;
            ui.item.startPos = ui.item.index();
            ui.item.id = $(ui.item).attr("data-id");
            ui.item.type = $(ui.item).attr("data-type");
        },
        stop: function (event, ui) {

            //If the note didn't move to another swimlane
            if ((ui.item.targetLane == null) && (ui.item.startPos != ui.item.index())) {
                var index = ui.item.index();
                var startPos = ui.item.startPos;
                var note = this.getElementsByClassName("note");
                if (index < startPos) {
                    // If the note move up
                    for (var i = index; i < startPos + 1; i++) {
                        updatePosition($(note[i]).attr("data-id"), i, $(note[i]).attr("data-type"));
                    }
                }
                else {
                    // If the note move down
                    for (var i = startPos; i < index + 1; i++) {
                        updatePosition($(note[i]).attr("data-id"), i, $(note[i]).attr("data-type"));
                    }
                }

                // Update new position in clients
                var projectID = $("#txtProjectID").val();
                var noteID = $(ui.item).attr("id");
                var swimlanePosition = $(".connected").index(this);
                proxyNote.invoke("changePosition", projectID, noteID, swimlanePosition, index);
            }
        }
    }).disableSelection();

    /* Real-time communication */
    init_TaskCommentHub();
    init_NoteHub();

});

/* Class: Backlog */
function Backlog() {
    this.Project_ID = $("#txtProjectID").val();
    this.Swimlane_ID = $("#txtSwimlaneID").val();
    this.Title = $("#txtBacklogTitle").val();
    this.Description = $("#txtBacklogDescription").val();
    this.Complexity = $("#ddlBacklogComplexity").val();
    this.Color = $("#ddlBacklogColor").val();
}

/* Class: Task */
function Task() {
    this.Project_ID = $("#txtProjectID").val();
    this.Swimlane_ID = $("#txtSwimlaneID").val();
    this.Backlog_ID = $("#ddlTaskBacklog").val();
    this.Title = $("#txtTaskTitle").val();
    this.Description = $("#txtTaskDescription").val();
    this.Work_estimation = $("#txtTaskWorkEstimation").val();
    this.Color = $("#ddlTaskColor").val();
    this.Due_date = formatDate($("#txtTaskDueDate").val());
}


/*1. Create new sticky note and save it to database*/
function insertItem(type) {
    var item = (type == "backlog") ? new Backlog() : new Task();

    $.ajax({
        url: "Handler.ashx",
        data: {
            action: "insertItem",
            type: type,
            item: JSON.stringify(item)
        },
        global: false,
        type: "post",
        success: function (result) {
            saveAssignee(result.substring(0, result.indexOf(".")), type);
            var objtext = getVisualNote(result, type, item);
            console.log(objtext);
            var swimlanePosition = parseInt($("#txtSwimlanePosition").val());
            $(objtext).appendTo($(".connected")[swimlanePosition]);
            showSuccessDiaglog(0);
            (type == "backlog") ? clearBacklogWindow() : clearTaskWindow();

            // Send to other clients
            proxyNote.invoke("sendInsertedNote", item.Project_ID, swimlanePosition, objtext);
        }
    });
}

//Create visual note
function getVisualNote(dataID, type, item) {
    var idArray = dataID.split(".");
    var id = idArray[0];
    var backlog_id, typeNum;
    var color = item.Color;
    var status = $("#kanban td[data-id='" + item.Swimlane_ID + "']").attr("data-status");

    if (type == "backlog") {
        typeNum = 1;
    }
    else {
        backlog_id = item.Backlog_ID;
        typeNum = 2;
    }

    var objtext;
    objtext = "<div class='note' data-type='" + typeNum + "' id='" + type + "." + id + "' data-id='" + id + "' " +
    ((type == "task") ? "data-backlog-id='" + backlog_id + "' " : " ");
    objtext += "data-status='" + status + "'>" +
    "<div class='note-header' style='background-color:" + color.substr(0, 7) + ";'><span class='item-id'>" + idArray[1] + "</span>" +
    "<img class='note-button' onclick=\"viewDetailNote(" + id + ",'" + type + "')\" src='images/sidebar/edit_note.png'>" +
    "<img class='note-button' onclick=\"deleteItem(" + id + ",'" + type + "')\" src='images/sidebar/delete_note.png'></div>" +
    "<div class='note-content' style='background-color:" + color.substr(8, 7) + ";'>" + item.Title + "</div>" +
    "<div class='note-footer' style='background-color:" + color.substr(8, 7) + ";'>" +
    ((type == "backlog") ? "<div class='note-stat-button' onmouseover='viewBacklogStat(this, " + id + ")' onmouseout='hideBacklogStat()'></div>" : "&nbsp;");
    return objtext + "</div></div>";
}

// Open insert window
function showInsertWindow(windowName, i, swimlaneID) {
    showWindow(windowName);
    // Don't allow add comment when insert new item
    $("#" + windowName + " .pageRibbon img").css("display", "none");
    if (windowName == "backlogWindow") {
        $("#" + windowName + " .title-bar").html("Add new backlog");
        $("#" + windowName + " .btnSave").val("Add").attr("onclick", "insertItem('backlog')");
    }
    else {
        $("#" + windowName + " .title-bar").html("Add new task");
        $("#" + windowName + " .btnSave").val("Add").attr("onclick", "insertItem('task')");
    }

    // Store swimlame position so that we can add new sticky note into correct column
    $("#txtSwimlanePosition").val(i);
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
        type: "get"
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
                assigneeID: assignee[i].getAttribute("data-id")
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
                    type: type,
                    keyword: $(searchBox).val()
                },
                global: false,
                type: "get",
                success: function (result) {
                    var searchResult = document.getElementById("assigneeSearchResult");
                    searchResult.style.display = "block";
                    var pos = searchBox.getBoundingClientRect();
                    searchResult.style.top = pos.top + 30;
                    searchResult.style.left = pos.left;
                    searchResult.innerHTML = result;
                }
            });
        }, 100);
    }
    else clearResult();
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
function viewDetailNote(itemID, type) {
    showProcessingDiaglog(0);
    var windowName = type + "Window";
    showWindow(windowName);
    $("#" + windowName + " .title-bar").html("Edit " + type + " item");
    var btnSave = $("#" + windowName + " .btnSave");
    $(btnSave).val("Save").attr("onclick", "saveItem(" + itemID + ",'" + type + "')");

    $("#" + windowName + " .pageRibbon img").css("display", "block");

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
            (type == "backlog") ? displayBacklogDetail(result) : displayTaskDetail(result);
        }
    });

    //View all assignee of that item
    viewAssignee(itemID, type);

    if (type == "backlog") loadTaskBacklogTable(itemID);
    else {
        viewTaskComment(itemID);
        viewTaskFile(itemID);
        $("#btnSubmitComment").attr("data-task-id", itemID);
        $("#inputUploadFile").attr("data-task-id", itemID).val("");

        // Connect to Task Hub
        proxyTC.invoke("joinChannel", itemID);
    }
}

/*4.1.1 Display detail backlog */
function displayBacklogDetail(data) {
    $("#txtSwimlaneID").val(data.Swimlane_ID);
    $("#txtBacklogTitle").val(data.Title);
    $("#txtBacklogDescription").val(data.Description);
    $("#ddlBacklogComplexity").val(data.Complexity);
    $("#ddlBacklogColor").val(data.Color);
    $("#txtBacklogStart").val(parseJSONDate(data.Start_date));
    $("#txtBacklogComplete").val(parseJSONDate(data.Completion_date));
}

/*4.1.2 Clear backlog window */
function clearBacklogWindow() {
    $("#backlogWindow .inputbox").val("");
    $("#backlogWindow textarea").val("");
    $("#backlogWindow #ddlBacklogComplexity").val("1");
    $("#backlogWindow .assignee-name-active").remove();
}

/*4.2.1 Display detail task */
function displayTaskDetail(data) {
    $("#txtSwimlaneID").val(data.Swimlane_ID);
    $("#txtTaskTitle").val(data.Title);
    $("#txtTaskDescription").val(data.Description);
    $("#ddlTaskBacklog").val(data.Backlog_ID);
    $("#txtTaskWorkEstimation").val(data.Work_estimation);
    $("#ddlTaskColor").val(data.Color);
    $("#txtTaskDueDate").val(parseJSONDate(data.Due_date));
    $("#txtTaskCompletionDate").val(parseJSONDate(data.Completion_date));
}

/*4.2.2 Clear task Window */
function clearTaskWindow() {
    $("#taskWindow .inputbox").val("");
    $("#taskWindow textarea").val("");
    $("#taskWindow .assignee-name-active").remove();

    //Update list of current backlog items 
    createCurrentBacklogList();
}

/*4.3 Working with comments of a task*/
/*4.3.1 View all comment of a task*/
function viewTaskComment(itemID) {
    $.ajax({
        url: "Handler.ashx",
        data: {
            action: "viewTaskComment",
            itemID: itemID
        },
        global: false,
        type: "get",
        success: function (result) {
            $("#commentBox .comment-box").remove();
            $("#commentBox").prepend(result).perfectScrollbar("update");
        }
    });
}

/*4.3.2 Delete a comment of a task*/
function deleteTaskComment(commentID) {
    $.ajax({
        url: "Handler.ashx",
        data: {
            action: "deleteTaskComment",
            itemID: commentID
        },
        global: false,
        type: "get"
    });
    var obj = document.getElementById("comment." + commentID);
    obj.parentElement.removeChild(obj);

    // Delete comment in other clients' view
    var taskID = $("#btnSubmitComment").attr("data-task-id");
    proxyTC.invoke("deleteComment", taskID, commentID);
}

/*4.3.3 Fetch a comment to input field to edit*/
function fetchTaskComment(commentID) {
    var comment = document.getElementById("comment." + commentID);
    var content = comment.getElementsByClassName("comment-content")[0].innerHTML;
    content = content.replace(/<br>/g, '\n');
    $("#txtTaskComment").val(content);
    $("#btnSubmitComment").val("Save").attr("onclick", "updateTaskComment(" + commentID + ")");
}

/*4.3.4 Update edited comment*/
function updateTaskComment(commentID) {
    $.ajax({
        url: "Handler.ashx",
        data: {
            action: "updateTaskComment",
            itemID: commentID,
            content: $("#txtTaskComment").val()
        },
        global: false,
        type: "post"
    });
    var comment = document.getElementById("comment." + commentID);
    var content = comment.getElementsByClassName("comment-content")[0];
    content.innerHTML = $("#txtTaskComment").val().replace(new RegExp('\n', 'g'), '<br />');
    $("#txtTaskComment").val("");
    $("#btnSubmitComment").val("Send").attr("onclick", "submitTaskComment()");

    // Update comment content in other clients' view
    var taskID = $("#btnSubmitComment").attr("data-task-id");
    proxyTC.invoke("updateComment", taskID, commentID, content.innerHTML);
}

/*4.3.5 Insert new comment for a task */
function submitTaskComment() {
    var taskID = $("#btnSubmitComment").attr("data-task-id");
    $.ajax({
        url: "Handler.ashx",
        data: {
            action: "insertTaskComment",
            taskID: taskID,
            content: $("#txtTaskComment").val()
        },
        global: false,
        type: "post",
        success: function (id) {
            var content = $("#txtTaskComment").val().replace(new RegExp('\r?\n', 'g'), '<br />');
            var objtext = "<div class='comment-box' id='comment." + id + "'><div class='comment-panel'>" +
            "<img class='comment-profile' title='Nguyen Minh Son' src='images/sidebar/profile.png'></div>" +
            "<div class='comment-container'><div class='comment-content'>" + content + "</div><div class='comment-footer'>" +
            "<div class='comment-button' title='Edit comment' onclick='fetchTaskComment(" + id + ")'></div>" +
            "<div class='comment-button' title='Delete comment' onclick='deleteTaskComment(" + id + ")'></div>" +
            "</div></div>";
            $("#commentBox").append(objtext);
            $("#commentBox").scrollTop(document.getElementById("commentBox").scrollHeight);
            $("#txtTaskComment").val("");

            // Send objtext to other client who is also viewing the task
            proxyTC.invoke("sendSubmittedComment", taskID, objtext);
        }
    });
}


/*5 Save changes of the current backlog item to database*/
function saveItem(itemID, type) {
    showProcessingDiaglog();
    var item = (type == "backlog") ? new Backlog() : new Task();

    $.ajax({
        url: "Handler.ashx",
        data: {
            action: "updateItem",
            itemID: itemID,
            type: type,
            item: JSON.stringify(item)
        },
        global: false,
        type: "post",
        success: function (result) {
            $(".diaglog.success").removeClass("show");
            var note = document.getElementById(type + "." + itemID);
            var header = note.getElementsByClassName("note-header")[0];
            header.style.background = item.Color.substr(0, 7);
            var content = note.getElementsByClassName("note-content")[0];
            content.style.background = item.Color.substr(8, 7);
            content.innerHTML = item.Title;
            showSuccessDiaglog(1);
        }
    });

    updateAssignee(itemID, type);

    // Update note in other clients
    proxyNote.invoke("updateNote", item.Project_ID, type + "." + itemID, item.Title, item.Color);
}

/*6. Delete an item*/
function deleteItem(itemID, type) {
    $.ajax({
        url: "Handler.ashx",
        data: {
            action: "deleteItem",
            itemID: itemID,
            type: type
        },
        global: false,
        type: "get"
    });
    var id = type + "." + itemID;
    var note = document.getElementById(id);
    note.parentElement.removeChild(note);

    // Delete in other clients
    proxyNote.invoke("deleteNote", $("#txtProjectID").val(), id);
}

/*7.1 Creat a drop down list contains all current backlog items*/
function createCurrentBacklogList() {
    var note = $(".note[data-type='1']");
    var backloglist = document.getElementById("ddlTaskBacklog");
    backloglist.innerHTML = "";
    for (var i = 0; i < note.length; i++) {
        var id = note[i].getAttribute("data-id");
        var content = note[i].getElementsByClassName("item-id")[0].innerHTML + " - "
        + note[i].getElementsByClassName("note-content")[0].innerHTML;
        backloglist.innerHTML += "<option value='" + id + "'>" + content + "</option>";
    }
}

/*8.1.a Parse multiple file name to box */
function getChosenFileName(obj) {
    var files = obj.files;
    var result = "";
    for (var i = 0; i < files.length - 1; i++) {
        result += files[i].name + ", ";
    }
    result += files[files.length - 1].name;
    $('#inputFileName').html(result);
}

/*8.1.b Calculate total size of all uploading files*/

function getTotalSize(files) {
    totalSize = 0;
    for (var i = 0; i < files.length; i++) {
        totalSize += files[i].size;
    }
    return totalSize;
}

/*8.2.1 Start upload files*/
var files;
var taskID, totalSize, tempSize;
var progress

function startUploadFile() {

    // Init variables
    var input = document.getElementById("inputUploadFile");
    files = input.files;

    if (files.length > 0) {
        taskID = input.getAttribute("data-task-id");
        totalSize = getTotalSize(files);
        tempSize = 0;
        progress = 0;

        // Upload the first file if available
        document.getElementById("uploadProgressContainer").style.opacity = 1;
        uploadFile(0, taskID);
    }
}

/*8.2.2 Upload file at [index] of input file list*/
function uploadFile(i, taskID) {
    var file = files[i];
    var req = new XMLHttpRequest();
    var url = "Handler.ashx?action=uploadFile&fileType=" + file.type + "&taskID=" + taskID;
    var form = new FormData();
    form.append(file.name, file);

    req.upload.addEventListener("progress", function (e) {
        //Tracking progress here
        var done = e.position || e.loaded;
        var tempProgress = Math.round(((tempSize + done) / totalSize) * 100);
        if (progress < tempProgress) {
            progress = tempProgress;
            document.getElementById("uploadProgress").style.width = progress + "%";
            console.log(progress);
        }
    });

    req.onreadystatechange = function () {
        if (req.readyState == 4 && req.status == 200) {
            // Upload completed
            document.getElementById("fileList").innerHTML += req.responseText;
            $("#fileList .file-container").on("mouseover", function () {
                this.getElementsByClassName("file-remove")[0].style.display = "block";
            });
            $("#fileList .file-container").on("mouseout", function () {
                this.getElementsByClassName("file-remove")[0].style.display = "none";
            });

            // Send the visual to other clients
            proxyTC.invoke("sendUploadedFile", taskID, req.responseText);

            // If the queue still has files left then upload them
            if (i < files.length - 1) {
                tempSize += file.size;
                uploadFile(i + 1, taskID);
            }
                // Otherwise reset back to original state
            else {
                document.getElementById("uploadProgressContainer").style.opacity = 0;
                document.getElementById("uploadProgress").style.width = 0;
                document.getElementById("inputFileName").innerHTML = "";
                document.getElementById("inputUploadFile").value = "";
            }
        }
    }
    req.open('post', url, true);
    req.send(form);
}

/*8.3 Get all upload files*/
function viewTaskFile(taskID) {
    $.ajax({
        url: "Handler.ashx",
        data: {
            action: "viewTaskFile",
            taskID: taskID
        },
        type: "get",
        success: function (result) {
            $("#fileList .file-container").remove();
            $("#fileList").prepend(result).perfectScrollbar("update");
            $("#fileList .file-container").on("mouseover", function () {
                this.getElementsByClassName("file-remove")[0].style.display = "block";
            });
            $("#fileList .file-container").on("mouseout", function () {
                this.getElementsByClassName("file-remove")[0].style.display = "none";
            });
        }
    });
}

/*8.4 Delete an attached file of task*/
function deleteFile(fileID) {
    $("#fileList div[data-id='" + fileID + "']").remove();

    // Remove visual of the file in other clients
    var taskID = $("#btnSubmitComment").attr("data-task-id");
    proxyTC.invoke("deleteFile", taskID, fileID)

    $.ajax({
        url: "Handler.ashx",
        data: {
            action: "deleteTaskFile",
            fileID: fileID
        },
        type: "get"
    });
}

/*B.Working with chart*/
var myPie, myBarChart, myLineGraph;

function showChartWindow() {
    fetchPieChartData();
    fetchBarChartData();
    fetchLineGraphData();
}


//Load pie chart data
function fetchPieChartData() {
    $.ajax({
        url: "Handler.ashx",
        data: {
            action: "getPieChart"
        },
        type: "get",
        success: function (pieChartData) {
            var chartPie = document.getElementById("chartPie").getContext("2d");
            if (myPie != null) myPie.destroy();
            myPie = new Chart(chartPie).Pie(pieChartData);
        }
    });
}

//Load bar chart data
function fetchBarChartData() {
    $.ajax({
        url: "Handler.ashx",
        data: {
            action: "getBarChart"
        },
        type: "get",
        success: function (barChartData) {
            var chartBar = document.getElementById("chartBar").getContext("2d");
            if (myBarChart != null) myBarChart.destroy();
            myBarChart = new Chart(chartBar).Bar(barChartData, {
                scaleFontColor: "#FFFFFF",
                scaleGridLineColor: "rgba(128, 128, 128, 0.2)"
            });
        }
    });
}

//Load bar chart data
function fetchLineGraphData() {
    $.ajax({
        url: "Handler.ashx",
        data: {
            action: "getLineGraph"
        },
        type: "get",
        success: function (lineGraphData) {
            var graphLine = document.getElementById("graphLine").getContext("2d");
            if (myLineGraph != null) myLineGraph.destroy();
            myLineGraph = new Chart(graphLine).Line(lineGraphData, {
                bezierCurve: false,
                datasetFill: true,
                scaleFontColor: "#FFFFFF",
                scaleGridLineColor: "rgba(128, 128, 128, 0.2)"
            });
        }
    });
}


/* Backlog statistic */
/* Format tblTaskBacklog and load data */
function loadTaskBacklogTable(backlog_id) {
    var task = $(".note[data-type='2'][data-backlog-id='" + backlog_id + "']");
    var tbody = $("#tblTaskBacklog tbody");
    $(tbody).html("");
    for (var i = 0; i < task.length; i++) {
        var relativeID = task[i].getElementsByClassName("item-id")[0].innerHTML;
        var title = task[i].getElementsByClassName("note-content")[0].innerHTML;
        var objtext = "<tr><td>" + relativeID + "</td><td>" + title + "</td><td>" + task[i].getAttribute("data-status") + "</td>" +
        "<td><img class='note-button' onclick=\"hideWindow(); viewDetailNote(" + task[i].getAttribute("data-id") + ",'task')\" src='images/sidebar/view.png' /></td></tr>";
        $(tbody).append(objtext);
    }
}

// Backlog's tasks statistics
var backlogStat;

// Load and draw chat
function loadBacklogStat(backlog_id) {
    // Create stats
    var task = $(".note[data-type='2'][data-backlog-id='" + backlog_id + "']");

    var data = [0, 0, 0];
    for (var i = 0; i < task.length; i++) {
        switch (task[i].getAttribute("data-status")) {
            case "Standby":
                data[0]++;
                break;
            case "Ongoing":
                data[1]++;
                break;
            case "Done":
                data[2]++;
                break;
        }
    }
    var backlogStatData = {
        labels: ["Standby", "Ongoing", "Done"],
        datasets: [
            {
                fillColor: "rgba(220,220,220,0.5)",
                strokeColor: "rgba(220,220,220,0.8)",
                highlightFill: "rgba(220,220,220,0.75)",
                highlightStroke: "rgba(220,220,220,1)",
                data: data
            }
        ]
    };

    // Draw
    var chartBacklogStat = document.getElementById("chartBacklogStat").getContext("2d");
    if (backlogStat != null) backlogStat.destroy();
    backlogStat = new Chart(chartBacklogStat).Bar(backlogStatData, {
        scaleFontColor: "#FFFFFF",
        scaleGridLineColor: "rgba(128, 128, 128, 0.2)"
    });
}

// Show dialogBacklogStat window
function viewBacklogStat(obj, backlog_id) {
    loadBacklogStat(backlog_id);
    var windowHeight = window.innerHeight || document.documentElement.clientHeight;
    var windowWidth = window.innerWidth || document.documentElement.clientWidth;
    var offset = $(obj).offset();
    var top = (offset.top > windowHeight - 200) ? offset.top - 200 : offset.top + 32;
    var left = (offset.top > windowWidth - 300) ? offset.left - 300 : offset.left;
    $("#diaglogBacklogStat").css("top", top).css("left", left);
    $("#diaglogBacklogStat").css("display", "block").addClass("show");
}

// Hide dialogBacklogStat window
function hideBacklogStat() {
    backlogStat.destroy();
    $("#diaglogBacklogStat").removeClass("show");
    setTimeout(function () {
        $("#diaglogBacklogStat").css("display", "none");
    }, 150)
}


/*C. SignalR Communication */
/*1. Real time comment and document */
var connTC, proxyTC;
function init_TaskCommentHub() {
    connTC = $.hubConnection();
    proxyTC = connTC.createHubProxy("taskHub");

    // When other client send data, we listen by these
    // Receive new comment
    proxyTC.on("receiveSubmittedComment", function (msgObj) {
        $("#commentBox").append(msgObj);
        $("#commentBox").scrollTop(document.getElementById("commentBox").scrollHeight);
    });

    // Delete a comment
    proxyTC.on("deleteComment", function (commentID) {
        var obj = document.getElementById("comment." + commentID);
        obj.parentElement.removeChild(obj);
    });

    // Update content of a comment
    proxyTC.on("updateComment", function (commentID, content) {
        var comment = document.getElementById("comment." + commentID);
        comment.getElementsByClassName("comment-content")[0].innerHTML = content;
    })

    // Receive new document visual
    proxyTC.on("receiveUploadedFile", function (msgObj) {
        document.getElementById("fileList").innerHTML += msgObj;
        $("#fileList .file-container").on("mouseover", function () {
            this.getElementsByClassName("file-remove")[0].style.display = "block";
        });
        $("#fileList .file-container").on("mouseout", function () {
            this.getElementsByClassName("file-remove")[0].style.display = "none";
        });
    });

    // Delete a document
    proxyTC.on("deleteFile", function (fileID) {
        $("#fileList div[data-id='" + fileID + "']").remove();
    });

    // Start connection and join group
    connTC.start().done(function () {

    });
}

/*2. Real time sticky note */
var connNote, proxyNote;
function init_NoteHub() {
    connNote = $.hubConnection();
    proxyNote = connNote.createHubProxy("noteHub");

    // When other client send data, we listen by these
    // Receive new note
    proxyNote.on("receiveInsertedNote", function (swimlanePosition, objtext) {
        $(objtext).appendTo($(".connected")[swimlanePosition]);
    });

    // Delete a note
    proxyNote.on("deleteNote", function (id) {
        var note = document.getElementById(id);
        note.parentElement.removeChild(note);
    });

    // Update a note
    proxyNote.on("updateNote", function (noteID, title, color) {
        var note = document.getElementById(noteID);

        var header = note.getElementsByClassName("note-header")[0];
        header.style.background = color.substr(0, 7);

        var content = note.getElementsByClassName("note-content")[0];
        content.style.background = color.substr(8, 7);
        content.innerHTML = title;
    })

    // Change position
    proxyNote.on("changePosition", function (noteID, swimlanePosition, position) {
        var target = document.getElementById(noteID);
        var lane = document.getElementsByClassName("connected")[swimlanePosition];
        var note = lane.getElementsByClassName("note");
        var i;
        for (i = 0; i < note.length; i++)
            if (note[i].getAttribute("id") == noteID) break;
        if (i > position) $(target).insertBefore($(note[position]));
        else $(target).insertAfter($(note[position]));
    });

    // Change lane
    proxyNote.on("changeLane", function (noteID, swimlanePosition, position) {
        var target = document.getElementById(noteID);
        var lane = document.getElementsByClassName("connected")[swimlanePosition];
        var note = lane.getElementsByClassName("note");
        $(target).insertBefore($(note[position]));
    });

    // Start connection and join group
    connNote.start().done(function () {
        proxyNote.invoke("joinChannel", $("#txtProjectID").val());
    });
}

/*D. Others */
function takeScreenshot() {
    html2canvas($("#container"), {
        onrendered: function (canvas) {
            var screenshot = canvas.toDataURL("image/jpeg", 0.92);
            screenshot = screenshot.replace('data:image/jpeg;base64,', '');
            $.ajax({
                type: "POST",
                url: "Handler.ashx",
                data: {
                    action: "uploadScreenshot",
                    screenshot: screenshot
                }
            });
            unloadPageSpinner();
        }
    });
}
