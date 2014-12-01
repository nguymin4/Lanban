function recalibrate() {
    var windowHeight = window.innerHeight || document.documentElement.clientHeight;
    var windowWidth = window.innerWidth || document.documentElement.clientWidth;
}

function openAddProjectWindow(index) {
    resetAddProjectWindow();
    $("#projectSupervisor").html("");
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
    $("#projectbrowser, #projectdetail-description, .right-window-content").perfectScrollbar({
        wheelSpeed: 5,
        wheelPropagation: false
    });
    $("#projectbrowser").append($("#projectdetail"));

    $(".txtSearch").on("click", function () {
        this.getElementsByClassName("txtSearchBox")[0].focus();
    });

    // Initialize drag and drop user box
    init_UserDragDrop();

    // Initialize project hub event listener
    init_ProjectHub_Listener();
});

$(window).load(function () {
    unloadPageSpinner();
});

/* A.Business logic */
/* Class Project */
function Project() {
    this.Project_ID = null;
    this.Name = $("#txtProjectName").val();
    this.Description = $("#txtProjectDescription").val().replace(new RegExp('\r?\n', 'g'), '<br />');
    this.Owner = userID;
    this.Start_Date = $("#txtProjectStartDate").val();
}

var projectList;
var userList;

/*1.1.1 Find a project based on its ID */
function findProject(id) {
    for (var i = 0; i < projectList.length; i++) {
        if (projectList[i].Project_ID == id) {
            return projectList[i];
        }
    }
    return null;
}

/*1.1.2 Get a project index based on its ID */
function getProjectIndex(id) {
    for (var i = 0; i < projectList.length; i++) {
        if (projectList[i].Project_ID == id) {
            return i;
        }
    }
}

/*1.2 Find a user based on ID */
function findUser(id) {
    for (var i = 0; i < userList.length; i++) {
        if (userList[i].User_ID == id) {
            return userList[i];
        }
    }
}

/*2.1 View project detail */
function viewProjectDetail(obj, id) {
    // Looking for the project in the projectList based on id
    var project = findProject(id);
    var container = $(".project-container");

    // Insert the project detail box after that last container
    var index = getLastIndexContainer(obj);
    index = (index >= container.length) ? (index - 1) : index;
    $("#projectdetail").insertAfter($(container[index])).attr("data-project-id", id);

    // Open project detail box and load info
    fetchSupervisor(id);
    setTimeout(function () {
        if ($("#projectdetail").hasClass("show"))
            $("#projectdetail").fadeOut("fast", function () {
                loadProjectDetailInfo(project, id);
                $("#projectdetail").fadeIn("fast");
            });
        else {
            $("#projectdetail").addClass("show");
            loadProjectDetailInfo(project, id);
        }
    }, 10);
    scrollProjectBrowser(index);
}

/* 2.1.2 Load project info */
function loadProjectDetailInfo(project, id) {
    $("#screenshot").attr("src", "Uploads/Project_" + project.Project_ID + "/screenshot.jpg");

    $("#btnOpenProject").attr("onclick", "loadPageSpinner();" +
        "__doPostBack('RedirectBoard', '" + id + "$" + project.Name + "');");
    $("#btnShareProject").attr("onclick", "shareProject(" + id + ",'" + project.Name + "')");

    if (project.Owner == userID) {
        $("#btnDeleteProject").css("display", "inline-block").attr("onclick", "deleteProject(" + id + ")");
        $("#btnEditProject").css("display", "inline-block").attr("onclick", "editProject(" + id + ")");
        $("#btnQuitProject").css("display", "none");
    }
    else {
        $("#btnDeleteProject").css("display", "none");
        $("#btnEditProject").css("display", "none");
        $("#btnQuitProject").css("display", "inline-block").attr("onclick", "quitProject(" + id + ")");
    }


    $("#projectdetail-name").html(project.Name);
    $("#projectdetail-description").html(project.Description);
    $("#projectStartDate").html(parseJSONDate(project.Start_Date));

    $("#project-owner .project-data").html(getPersonDisplay(findUser(project.Owner)));
}

/* 2.1.3 Get display of a person */
function getPersonDisplay(person) {
    var result = "<div class='person' title='" + person.Name + "'>" +
        "<img class='person-avatar' src='" + person.Avatar + "' />" +
        "<div class='person-name'>" + person.Name + "</div></div>";
    return result;
}

/* 2.1.4 Fetch supervisor data of a project */
function fetchSupervisor(projectID) {
    var supervisorBox = $("#project-supervisor .project-data");
    supervisorBox.html("<div class='loading-spinner'></div>");
    $.ajax({
        url: "Handler/ProjectHandler.ashx",
        data: {
            action: "fetchSupervisor",
            projectID: projectID
        },
        global: false,
        type: "get",
        success: function (result) {
            supervisorBox.html(result);
        }
    });
}

/* 2.1.5 Close project detail box */
function hideProjectDetail() {
    $("#projectdetail").removeClass("show");
    scrollProjectBrowser(-1);
}

/* Create new project */

var supervisorChange = false;

/*3.1 Create new project */
function addProject() {
    showProcessingDiaglog();
    var project = new Project();
    $.ajax({
        url: "Handler/ProjectHandler.ashx",
        data: {
            action: "createProject",
            project: JSON.stringify(project)
        },
        global: false,
        type: "post",
        success: function (id) {
            // Save list of supervisor to database
            // Close processing diaglog and display success diaglog when everything is ready
            $.when(saveSupervisor(id, true)).done(function () {
                showSuccessDiaglog(0);
            });

            // Push new project to projectList
            project.Project_ID = parseInt(id);
            projectList.push(project);

            // Get visual of the new project
            var objtext = getVisualProject(id, project.Name);
            $("#projectbrowser").prepend(objtext);
            $(".input-project").val("");
        }
    });
}

//3.1 Helper
function getVisualProject(id, name) {
    return "<div id='project" + id + "' class='project-container' onclick='viewProjectDetail(this, " + id + ")'>" +
        "<div class='project-header'>" + id + ". " + name + "</div>" +
        "<div class='project-thumbnail' style=\"background-image:url('/Uploads/Project_" + id + "/screenshot.jpg');\"></div></div>";
}

/*3.2 Link Supervisor ID to Project ID */
function saveSupervisor(id, clear) {
    var deferreds = [];
    var supervisor = $("#projectSupervisor .person");
    for (var i = 0; i < supervisor.length; i++) {
        deferreds.push($.ajax({
            url: "Handler/UserHandler.ashx",
            data: {
                action: "saveSupervisor",
                projectID: id,
                supervisorID: $(supervisor[i]).attr("data-id")
            },
            global: false,
            type: "get"
        }));
    }
    // Whether clear container after save
    if (clear) $("#projectSupervisor .person").remove();
    supervisorChange = false;
    return deferreds;
}

/*4 Using AJAX to search name of user */
// Search user name
var userSearch;
function searchUser(searchBox, role) {
    clearInterval(userSearch);
    if ($(searchBox).val() != "") {
        userSearch = setTimeout(function () {
            $.ajax({
                url: "Handler/UserHandler.ashx",
                data: {
                    action: "searchUser",
                    role: role,
                    name: $(searchBox).val()
                },
                global: false,
                type: "get",
                success: function (result) {
                    var searchContainer = $("#searchContainer");
                    var pos = searchBox.getBoundingClientRect();
                    $(searchContainer).css("top", pos.top + 40).css("left", pos.left);
                    $(searchContainer).html(result).fadeIn("fast");
                    var func = (role == 1) ? "addMember(this)" : "addSupervisor(this)";
                    $("#searchContainer .searchRecord").attr("onclick", func);
                }
            });
        }, 200);
    }
    else clearResult();
}

function addSupervisor(obj) {
    var id = obj.getAttribute("data-id");
    var name = obj.innerHTML;
    var avatar = obj.getAttribute("data-avatar");
    var objtext = "<div class='person' data-id='" + id + "' title='" + name + "' onclick='removeSupervisor(this)' style='cursor: pointer;'>" +
    "<img class='person-avatar' src='" + avatar + "'></img><div class='person-name'>" + name + "</div></div>";
    $("#txtSupervisor").val("").focus();
    var box = $("#projectSupervisor");
    var person = $(objtext);
    person.css("display", "none");
    if (!box.hasClass("expand")) {
        box.addClass("expand").append(person);
        person.fadeIn(500);
    }
    else {
        box.append(person);
        person.fadeIn(500);
    }
    supervisorChange = true;
}

// When click on active user then remove
function removeSupervisor(obj) {
    var parent = obj.parentElement;
    parent.removeChild(obj);
    supervisorChange = true;
}

// Clear search result
function clearResult(obj) {
    setTimeout(function () {
        $(obj).val("");
        $("#searchContainer").html("").fadeOut("fast");
    }, 250);
}

/*5 Update project info */
/*5.1 Open window to update project info */
function editProject(id) {

    //Load data
    var project = findProject(id);
    var supervisorList = $("#project-supervisor .project-data").html();
    $("#txtProjectName").val(project.Name);
    $("#txtProjectDescription").val(project.Description.replace(/<br>/g, '\n'));
    $("#txtProjectStartDate").val(parseJSONDate(project.Start_Date));
    if (supervisorList != "") $("#projectSupervisor").attr("class", "expand").html(supervisorList);
    $("#projectSupervisor .person").on("click", function () { removeUser(this) });
    $("#projectSupervisor .person").css("cursor", "pointer");
    supervisorChange = false;

    var btnSave = $("#btnAddProject");
    btnSave.attr("src", "images/sidebar/update.png");
    btnSave.attr("title", "Save");
    btnSave.attr("onclick", "updateProject(" + id + ")");

    $("#addproject h3").html("Edit project");

    // Open the window
    if ($("#addproject").hasClass("show")) {
        $("#addproject").fadeOut("fast", "swing", function () {
            $("#addproject").fadeIn("fast");
        });
    }
    else {
        var rightContent = document.getElementsByClassName("right-window-content");
        rightContent[0].setAttribute("class", "right-window-content");
        setTimeout(function () {
            rightContent[0].style.display = "none";
            rightContent[1].setAttribute("class", "right-window-content show");
        }, 500);
    }
}

/*5.2 Update Project info */
function updateProject(id) {
    // Initialize
    showProcessingDiaglog();
    var project = new Project();
    project.Project_ID = id;
    var supervisorList = $("#projectSupervisor").html();

    // Update project data
    var saveData = $.ajax({
        url: "Handler/ProjectHandler.ashx",
        data: {
            action: "updateProject",
            projectID: id,
            project: JSON.stringify(project)
        },
        global: false,
        type: "post",
        success: function () {
            projectList[getProjectIndex(id)] = project;

            // Ask other user who share the same project updating project
            proxyUser.invoke("updateProject", id);
        }
    });

    //Update supervisor list
    var saveSupervisor = (supervisorChange == true) ? updateSupervisor(id) : null;

    // When all savings are done
    $.when(saveData, saveSupervisor).done(function () {
        showSuccessDiaglog(1);

        // Update projectbrowser view
        $("#project" + id + " .project-header").html(id + ". " + project.Name);

        // Udate projectdetail view
        $("#projectdetail").fadeOut("fast", function () {
            loadProjectDetailInfo(project, id);
            $("#project-supervisor .project-data").html(supervisorList);
            $("#projectdetail").fadeIn("fast");
        });
    });
}

/*5.3 Update Supervisor list of a Project */
function updateSupervisor(id) {
    $.ajax({
        url: "Handler/UserHandler.ashx",
        data: {
            action: "deleteSupervisor",
            projectID: id
        },
        global: false,
        type: "get",
        success: function () {
            return saveSupervisor(id, false);
        }
    });
}

/*6 Delete project */
function deleteProject(id) {
    var name = findProject(id).Name;
    $.ajax({
        url: "Handler/ProjectHandler.ashx",
        data: {
            action: "deleteProject",
            projectID: id
        },
        global: false,
        type: "get",
        success: function () {
            // Delete project in other clients
            proxyUser.invoke("deleteProject", id, name, userID);
        }
    });
    doAfterDeleteProject(id);
}

/*7 Quit project */
function quitProject(projectID) {
    $.ajax({
        url: "Handler/UserHandler.ashx",
        data: {
            action: "quitProject",
            projectID: projectID
        },
        global: false,
        type: "get",
        success: function () {
            //  In other clients
            proxyUser.invoke("quitProject", projectID, userID);
        }
    });
    doAfterDeleteProject(projectID);
}

function doAfterDeleteProject(id) {
    if ($("#projectdetail").attr("data-project-id") == id)
        hideProjectDetail();

    var project = $("#project" + id);
    project.fadeOut("fast", function () { project.remove() });
    setTimeout(function () {
        $("#projectbrowser").append($("#projectdetail"));
    }, 1000);

    // Remove data in project list
    projectList.splice(getProjectIndex(id), 1);
}

/* Share project */
function shareProject(id, name) {
    showProcessingDiaglog();
    showView(2);
    $("#sharingWindow").attr("data-project-id", id);
    $("#sharingWindow .title-bar").html(name);
    $.ajax({
        url: "Handler/ProjectHandler.ashx",
        data: {
            action: "fetchUser",
            projectID: id
        },
        global: false,
        type: "get",
        success: function (result) {
            $(".diaglog.success").fadeOut(100);
            $("#userList").html(result);
        }
    });
}

/*B. Others*/
// Scroll project browser to the position of project detail box.
function scrollProjectBrowser(index) {
    var container = $(".project-container");

    if (index == -1) {
        var top = $("#projectdetail").css("top").replace("px", "");
        $("#projectbrowser").animate({ scrollTop: top }, '1000', 'swing');
        $("#projectbrowser").perfectScrollbar("update");
    }
    else {
        setTimeout(function () {
            $("#projectbrowser").perfectScrollbar("update");
            var top = container[index].style.top + 200;
            $("#projectbrowser").animate({ scrollTop: top }, '500', 'swing');
        }, 1000);
    }
}

// Find the index of the last container in a row of a project browser
function getLastIndexContainer(obj) {
    var project = $(".project-container");
    var index;
    for (index = 0; index < project.length; index++)
        if (project[index] === obj) break;

    // Calculate number of project container in 1 row
    var boxNum = getNumBoxInRow();

    // Get the index of the last container in the row that the obj belongs to
    index = Math.floor(index / boxNum) + 1;
    index = boxNum * index;
    return index - 1;
}

// Find number of project containers in a row of project browser
function getNumBoxInRow() {
    var boxWidth = $(".project-container:eq(0)").outerWidth(true);
    var projectWidth = $("#projectbrowser").width();
    return parseInt(projectWidth / boxWidth);
}

function resetAddProjectWindow() {
    $("#txtProjectName").val("");
    $("#txtProjectDescription").val("");
    $("#txtProjectStartDate").val("");
    $("#projectSupervisor").attr("class", "").html();

    var btnSave = $("#btnAddProject");
    btnSave.attr("src", "images/sidebar/add_project.png");
    btnSave.attr("title", "Add new project");
    btnSave.attr("onclick", "addProject()");

    $("#addproject h3").html("Add new project");
}


/*C. Real-time communication */
// Add Project hub listeners - Project hub share same connection and proxy with User Hub
function init_ProjectHub_Listener() {
    /* Project User */
    proxyUser.on("addUser", function (projectID, personContainer) {
        var currentProject = $("#sharingWindow").attr("data-project-id");
        var personID = $(personContainer).attr("data-id");

        if (currentProject == projectID && !isInProject(personID)) {
            $("#userList").append(personContainer);
            if (userID != findProject(projectID).Owner) $(".person-remove", personContainer).remove();
        }

    });

    proxyUser.on("removeUser", function (projectID, personID) {
        var currentProject = $("#sharingWindow").attr("data-project-id");
        if (currentProject == projectID && isInProject(personID)) {
            $(".person[data-id='" + personID + "']", $("#sharingWindow")).remove();
            console.log("Removed Person");
        }
    });

    /* Project */
    proxyUser.on("addProject", function (project) {
        // Only added user receive this data
        var objtext = getVisualProject(project.Project_ID, project.Name);
        $("#projectbrowser").prepend(objtext);
        projectList.push(project);
    });

    proxyUser.on("deleteProject", function (projectID) {
        if (findProject(projectID) != null) doAfterDeleteProject(projectID);
        var currentProject = $("#sharingWindow").attr("data-project-id");
        if (currentProject == projectID) {
            showView(0);
            $("#sharingWindow .title-bar").html("Share");
            $("#sharingWindow").attr("data-project-id", "0");
        }
    });

    proxyUser.on("updateProject", function (project) {
        var id = project.Project_ID;
        // Update data in project list
        projectList[getProjectIndex(id)] = project;

        // Update projectbrowser view
        $("#project" + id + " .project-header").html(id + ". " + project.Name);

        // Udate projectdetail view if the current project detail displaying the updated project
        var currentProject = $("#projectdetail").attr("data-project-id");
        if (currentProject == id) {
            $("#projectdetail").fadeOut("fast", function () {
                loadProjectDetailInfo(project, id);
                fetchSupervisor(id);
                $("#projectdetail").fadeIn("fast");
            });
        }
    });
}


/* Add new user to the project */
// Initialize drag and drop user box
function init_UserDragDrop() {
    $(".user-list-connected").sortable({
        connectWith: ".user-list-connected",
        start: function (event, ui) {
            ui.item.id = $(ui.item).attr("data-id");
            ui.item.drop = $(this).is($("#tempUserList")) && !isInProject(ui.item.id);
        },
        receive: function (event, ui) {
            if (ui.item.drop) {
                var person = $(ui.item);
                var projectID = $("#sharingWindow").attr("data-project-id");

                if (userID != findProject(projectID).Owner) $(".person-remove", person).remove();
                else
                    $(".person-remove", person).attr("onclick", "removeMember(this.parentElement," + projectID + ")");

                // Update database
                addMember(ui.item.id, projectID);
            }
            else $(ui.sender).sortable("cancel");
        }
    }).disableSelection();
}

// Based on teh number of parameter
// 1. Add project member to temp list
// 2. Add project member to official list and update data in server
function addMember(arg, projectID) {
    if (arguments.length - 1 == 0) {
        var obj = arg;
        var id = obj.getAttribute("data-id");
        var name = obj.innerHTML;
        var avatar = obj.getAttribute("data-avatar");
        $("#txtSearchUser").val("").focus();
        var objtext = "<div class='person' data-id='" + id + "' title='" + name + "'>" +
        "<img class='person-avatar' src='" + avatar + "'></img><div class='person-name'>" + name + "</div>" +
        "<div class='person-remove' onclick='removeMember(this.parentElement)'></div></div>";
        $("#tempUserList").append(objtext);
    }
    else {
        $.ajax({
            url: "Handler/UserHandler.ashx",
            data: {
                action: "saveAssignee",
                itemID: projectID,
                assigneeID: arg,
                type: "Project"
            },
            global: false,
            type: "get",
            success: function () {
                proxyUser.invoke("addUser", projectID, arg);
            }
        });
    }
}

// Remove project member - based on number of args
// 1. Remove project member in temporary list
// 2. Kick the memeber out of project update database
function removeMember(obj, projectID) {
    if (arguments.length == 2) {
        var uid = $(obj).attr("data-id");
        $.ajax({
            url: "Handler/ProjectHandler.ashx",
            data: {
                action: "kickUser",
                projectID: projectID,
                uid: uid
            },
            global: false,
            type: "get",
            success: function() {
                proxyUser.invoke("removeUser", projectID, uid);
            }
        });
    }

    // Remove visual
    $(obj).remove();
}

// Check whether a person is already in the user list
function isInProject(personID) {
    var members = $("#userList .person");
    for (var i = 0; i < members.length; i++) {
        if ($(members[i]).attr("data-id") == personID) return true;
    }
    return false;
}