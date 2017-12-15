/*
 * Contains scripts used for logging in to Battleship
 * @author Kristen Merritt
 */

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
    $("#login-form").on("submit", function (e) {
        e.preventDefault();

        var handle = $("input[name='handle']").val();
        var pass = $("input[name='password']").val();

        var playerLoginInfo = { // Reflects the PlayerLoginInfo model
            Handle: handle,
            Password: pass
        }

        $.ajax({
            type: "POST",
            cache: false,
            dataType: "json",
            url: window.location.protocol + "//"+window.location.host+"/api/login",
            data: playerLoginInfo, // PlayerLoginInfo model
            success: function (data) {
                if (data.success) {
                    setTokenCookie(data.token, 1); // this method is in token_validation.js
                    window.location = "/";
                } else {
                    alert("Error with login. \n" + data.errors);
                }
                
            },
            error: function (error) {
                console.log("Error in login attempt");
            }
        });
    });
};

