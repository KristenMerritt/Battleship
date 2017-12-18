//////////////////////////////////////////////////////////////////
//                                                              //
//  Login Javascript File						                //
//  Description:  This file contains the scripts required       //
//                to log a player into the application.         //
//                                                              //
//////////////////////////////////////////////////////////////////

$(document).ready(function () {
    initializeLoginEvent();
});

/*
 * Adds an on submit listener to the login-form.
 * Retrieves the handle and password used in the login attempt,
 * and passes it to the REST endpoint for validation.
 * If it is a successful login, set the cookie/token, then
 * redirect the user to the home page.
 */
function initializeLoginEvent() {
    $("#login-form").on("submit",
        function(e) {
            e.preventDefault();

            // Get the login data
            var handle = $("input[name='handle']").val();
            var pass = $("input[name='password']").val();

            // Create the login data object
            var playerLoginInfo = { // Reflects the PlayerLoginInfo model
                Handle: handle,
                Password: pass
            }

            // Send data to the database for verification
            ajax("POST", true, "api/login", playerLoginInfo, function(data) {
                if (data.success) {
                    setTokenCookie(data.token, 1); 
                    window.location = "/";
                } else {
                    alert("Error with login. \n" + data.errors);
                }
            });        
        });
};