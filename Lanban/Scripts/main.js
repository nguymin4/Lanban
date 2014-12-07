/*****************************************************************************/
//Function to change working window
var view;
var currentView;
var viewIndicator
function showView(index) {
    if (index != currentView) {
        //Hide secondary window when it is opening in the Board.aspx
        if (document.getElementById("kanbanWindow") != null) {
            if (!((currentView != 0) && (index != 0))) hideWindow();
        }
            // Reset data-project-id sharingWindow Project.aspx
        else {
            if (currentView == 2) view[currentView].setAttribute("data-project-id", "");
        }

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


var _avatar, _name;
var errorPage = "/Error/error.html";

$(window).ready(function () {
    var profile = $("#profile");
    _avatar = profile.attr("src");
    _name = profile.attr("title");

    /*Make working window draggable*/
    $(".window").draggable({
        cursor: "move",
        handle: ".title-bar"
    });

    $(".btnOK").on("click", function () {
        $(".diaglog.show").removeClass("show").fadeOut(100);
    });

    /*Add function when click the icon to change window*/
    viewIndicator = document.getElementsByClassName("viewIndicator");
    view = document.getElementsByClassName("view");
    currentView = 0;
    $(".viewIndicator").on("click", function () {
        showView(this.getAttribute("data-view-indicator"));
    });

    // Nofication center
    initNotificationCenter();

    // Connect to hub
    init_UserHub();
});


// If error happens with ajax then redirect to error page
$(document).ajaxError(function (event, req, setting) {
    console.log(req.status);
    if (req.status == 401) window.location.href = "Login.aspx";
    else window.location.href = errorPage;
});

/*****************************************************************************/
// Unload page loading spinner
function unloadPageSpinner() {
    $("#overlay").fadeOut(500, "swing");
    $("#container").fadeOut(500, "swing", function () {
        $("#container").fadeIn(500);
        if ($("#notiIndicator").html() != "") $("#notiIndicator").fadeIn(0);
    });
}

// Load page loading spinner
function loadPageSpinner() {
    $("#container").fadeOut(250, "linear", function () {
        $("#overlay").fadeIn(250, "linear");
    });
}


/*****************************************************************************/
// Init Notification center
function initNotificationCenter() {
    $("#notiCenter").perfectScrollbar({
        wheelSpeed: 3,
        wheelPropagation: false,
        suppressScrollX: true,
        includePadding: true
    });

    $("#notiIndicator, #profile").on("click", function (e) {
        e.stopImmediatePropagation();
        $("#notiIndicator").fadeOut("fast").html("");
        if ($("#notiCenter").find($(".noti-msg")).get(0) != null)
            $("#notiCenter").fadeIn("fast").perfectScrollbar("update");
    });

    $("#container").on("click", function () {
        $("#notiCenter").fadeOut("fast");
    });
}


/*****************************************************************************/
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
        d = (data[0] < 10) && (data[0].length == 1) ? "0" + data[0] : data[0];
        m = (data[1] < 10) && (data[1].length == 1) ? "0" + data[1] : data[1];
        return d + "." + m + "." + data[2];
    }
    return date;
}


/*****************************************************************************/
/* Diaglog */
// Show error diaglog  - content taken from an array based on parameter
var errorMsg = ["Cannot drop that item because it is not the same type with the items in column.", "Operation failed"];

function showErrorDialog(i) {
    $(".diaglog.error .content-holder").text(errorMsg[i]);
    $(".diaglog.error").addClass("show").fadeIn(200);
}

// Show success diaglog - content taken from an array based on parameter
var successMsg = ["New item created", "Data updated", "File uploaded", "Deleted"];

function showSuccessDiaglog(i) {
    if (!($(".diaglog.success").hasClass("show")))
        $(".diaglog.success").addClass("show").fadeIn(250);
    else $(".diaglog.success").fadeOut(100, function () {
        $(".diaglog.success").attr("data-diaglog-type", "Success");
        $(".diaglog.success .title-bar").text("Success");
        $(".diaglog.success .content-holder").html(successMsg[i]);
        $(".diaglog.success input").css("display", "block");
        $(".diaglog.success").fadeIn(100);
    })
}

// Show processing message
function showProcessingDiaglog() {
    $(".diaglog.success").attr("data-diaglog-type", "Processing");
    $(".diaglog.success .title-bar").text("Processing");
    $(".diaglog.success .content-holder").html("<div class='loading-spinner'></div>");
    $(".diaglog.success input").css("display", "none");
    $(".diaglog.success").addClass("show").fadeIn(200);
}

// Show confirmation
function showConfirmation(func) {
    var diaglog = $(".diaglog.confirmation");
    $(".btnCancel", diaglog).on("click", function () {
        $(diaglog).fadeOut(100);
    });

    $(".btnSave", diaglog).attr("onclick", func);

    $(diaglog).fadeIn("fast");
}

/***************************/
/* Real-time communication */
var connUser;
var proxyUser;

function init_UserHub() {
    connUser = $.hubConnection();
    proxyUser = connUser.createHubProxy("userHub");

    proxyUser.on("msgAddUser", function (sender, newMem, projectName) {
        if (sender.User_ID == userID) sender.Name = "You";

        if (newMem.User_ID != userID)
            msgAddUser(sender, newMem.Name, projectName);
        else
            msgAddUser(sender, "you", projectName);
    });

    proxyUser.on("msgProject", function (sender, context, pid, projectName) {
        if (sender.User_ID != userID)
            msgProject(sender, context, projectName);
        else
        {
            sender.Name = "You";
            msgProject(sender, context, projectName);
        }

        // If the user in board page then redirect back to project page
        if ((context == "Deleted") && (document.getElementById("kanbanBoard") != null))
            redirectAfterDeleting(pid, sender.Name, "deleted");
    });

    proxyUser.on("msgOwnerKick", function (sender, pid, projectName) {
        msgOwnerKick(sender, projectName);

        // If the user in board page then redirect back to project page
        if (document.getElementById("kanbanBoard") !=  null)
            redirectAfterDeleting(pid, sender.Name, "kicked you out of");
    });

    connUser.start().done(function () {
    });
}

/* Notification center */
// Add new message to notification center
function pushNoti(msg) {
    $("#notiCenter").prepend(msg);
    updateNotiIndicator();
}

// Update notification indicator
function updateNotiIndicator() {
    var msgNum = $("#notiIndicator").html();
    if (msgNum == "") msgNum = 1;
    else msgNum++;
    $("#notiIndicator").html(msgNum).fadeIn("fast");
}

// New member added
function msgAddUser(sender, newMem, project) {
    content = "<div class='noti-msg'><img src='" + sender.Avatar + "' class='noti-msg-avatar' />" +
        "<div class='noti-msg-content'><span class='subject'>" + sender.Name + "</span> added " +
        "<span class='target'>" + newMem + "</span> to <i>" + project + "</i></div>";
    pushNoti(content);
}

// Project updated or deleted
function msgProject(sender, context, project) {
    content = "<div class='noti-msg'><img src='" + sender.Avatar + "' class='noti-msg-avatar' />" +
        "<div class='noti-msg-content'><span class='subject'>" + sender.Name + "</span> " + context +
        " <i>" + project + "</i></div>";
    pushNoti(content);
}

// User is kicked
function msgOwnerKick(sender, project) {
    content = "<div class='noti-msg'><img src='" + sender.Avatar + "' class='noti-msg-avatar' />" +
       "<div class='noti-msg-content'><span class='subject'>" + sender.Name + "</span> kicked <strong> you </strong>" +
       " out of <i>" + project + "</i></div>";
    pushNoti(content);
}