/*
 * Contains scripts used for creating a new account in Battleship.
 * @author Kristen Merritt
 */

/*
 * Adds an on submit listener to the create-account-form.
 * Retrieves the handle and password used in the login attempt,
 * and passes it to the REST endpoint for validation.
 * If it is a successful creation, set the cookie/token, then
 * redirect the user to the home page.
 */
$(document).ready(function () {
    var body = $("body");

    body.on("submit", "#create-account-form", function (e) { // for some reason it would not accept the event unless done like this
        e.preventDefault();

        var uname = $("input[name='handle']").val();
        var pass = $("input[name='password']").val();

        var newAccount = { // Reflects the db_Player model
            PlayerId: -1,
            Handle: uname,
            Password: pass,
            Salt: "",
            DateCreated: null,
            DateModified: null
        }

        $.ajax({
            type: "POST",
            cache: false,
            dataType: "json",
            url: window.location.protocol + "//" + window.location.host + "/api/CreatePlayer",
            data: newAccount,
            success: function (data) {
                console.log("Successfully created a new player");
                window.location = "/";
            },
            error: function (error) {
                console.log("Error in creating a new player");
                alert("Error, please try again.");
            }
        });
    });

});

