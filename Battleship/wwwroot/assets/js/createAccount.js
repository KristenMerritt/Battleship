//////////////////////////////////////////////////////////////////
//                                                              //
//  Create Account Javascript File						        //
//  Description:  This file contains various scripts used       //
//                to create a new account in the application.   //
//                                                              //
//////////////////////////////////////////////////////////////////


/**
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

        var newAccount = {
            PlayerId: -1,
            Handle: uname,
            Password: pass,
            Salt: "",
            DateCreated: null,
            DateModified: null
        }

        ajax("POST", false, "api/Player", newAccount, function(data) {
            console.log("Successfully created a new player");
            window.location = "/";
        });
    });
});

