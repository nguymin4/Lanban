function recalibrate() {
    var windowHeight = window.innerHeight || document.documentElement.clientHeight;
    var windowWidth = window.innerWidth || document.documentElement.clientWidth;
}

function openAddProjectWindow(index) {
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
    $("#projectbrowser, #projectdetail-description").perfectScrollbar({
        wheelSpeed: 5,
        wheelPropagation: false
    });
});

/* Business logic */
var projectList;
var userList;
/* Find a project based on its ID */
function findProject(id) {
    for (var i = 0; i < projectList.length; i++) {
        if (projectList[i].Project_ID == id) {
            return projectList[i];
        }
    }
}

/* Find a user based on ID */
function findUser(id) {
    for (var i = 0; i < userList.length; i++) {
        if (userList[i].User_ID == id) {
            return userList[i];
        }
    }
}

/* View project detail */
function viewProjectDetail(obj, id) {
    // Looking for the project in the projectList based on id
    var project = findProject(id);
    var container = $(".project-container");

    // Insert the project detail box after that last container
    var index = getLastIndexContainer(obj);
    index = (index >= container.length) ? (index - 1) : index;
    $("#projectdetail").insertAfter($(container[index]));

    // Open project detail box and load info
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

// Load info
function loadProjectDetailInfo(project, id) {
    $("#screenshot").attr("src", "Uploads/Project_" + project.Project_ID + "/screenshot.jpg");
    $("#btnOpenProject").attr("onclick", "__doPostBack('RedirectBoard', '" + id + "$" + project.Name + "')");
    $("#btnDeleteProject").attr("onclick", "");
    $("#btnEditProject").attr("onclick", "");

    $("#projectdetail-name").html(project.Name);
    $("#projectdetail-description").html(project.Description);
    $("#txtProjectStartDate").html(parseJSONDate(project.Start_Date));

    $("#project-owner .project-data").html(getPersonDisplay(findUser(project.Owner)));
}

// Close project detail box
function hideProjectDetail() {
    $("#projectdetail").removeClass("show");
    scrollProjectBrowser(-1);
}

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

// Get display of a person
function getPersonDisplay(person) {
    var result = "<div class='person'>"+
        "<img class='person-avatar' src='"+person.Avatar+"' />" +
        "<div class='person-name'>"+person.Name+"</div></div>";
    return result;
}
