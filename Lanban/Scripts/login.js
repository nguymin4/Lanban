var checkName;

$(document).ready(function () {
    $("#register table input").val("").attr("autocomplete", "off");
});

// Check registered username
function checkName() {
    clearInterval(checkName);
    var status = $("#checkUsername");
    var username = $("#txtRUsername").val();

    if (!/[^a-zA-Z0-9\.\-\_]/.test($("#txtRUsername").val())) {
        displayMsg(1, "");
        if (username.length > 7) {
            userSearch = setTimeout(function () {
                $.ajax({
                    url: "Handler/UserHandler.ashx",
                    data: {
                        action: "checkUsername",
                        username: username
                    },
                    success: function (result) {
                        if (result == "Existed") displayMsg(1, result);
                    }
                });
            }, 200);
        }
        else displayMsg(1, "At least 8 characters");
    }
    else displayMsg(1, "Only 0-9, a-Z, . , - , and _");
}

// Open login form or register form
function openRegister(register) {
    if (register) {
        $("#login").addClass("hidden");
        $("#register").removeClass("hidden");
    }
    else {
        $("#login").removeClass("hidden");
        $("#register").addClass("hidden");
        $("#register table input").val("");
        $("#register .validator").css("display", "none");
    }
}

// Register user
function registerUser() {
    var v = $("#register .validator");
    v.css("display", "none").html("");
    var f = $("#register table input");

    // Fullname field validator
    if (f[0].value.length < 1) displayMsg(0, "Required");

    // Username field validator
    var regex = /[^a-zA-Z0-9\.\-\_]/;
    if (f[1].value.length < 7) displayMsg(1, "At least 8 characters");
    else
        if (regex.test(f[1].value)) displayMsg(1, "Only 0-9, a-Z, . , - , and _");

    // Password field validator
    if (f[2].value.length < 7) displayMsg(2, "At least 8 characters");

    // Repeat password validator
    if (f[3].value != f[2].value) displayMsg(3, "Mismatch");

    var ready = true;
    for (var i = 0; i < v.length; i++) {
        if ($(v[i]).css("display") == "block") {
            ready = false;
            break;
        }
    }

    // If every fields is validated then post back
    if (ready) __doPostBack("Register", "");
}

function displayMsg(i, content) {
    var v = $("#register .validator");
    $(v[i]).css("display", "block").html(content);
}