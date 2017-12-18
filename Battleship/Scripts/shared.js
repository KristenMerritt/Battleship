//////////////////////////////////////////////////////////////////
//                                                              //
//  Shared Javascript File						                //
//  Description:  This file contains various scripts used       //
//                throughout the entire application.            //
//                                                              //
//////////////////////////////////////////////////////////////////

$(document).ready(function () {
    getBrowser();
});

/**
 * Checks to make sure the browser being used is Chrome.
 * No other browser was tested due to time contraints.
 */
function getBrowser() {
    if (navigator.userAgent.indexOf("Chrome") != -1) {
        // good
    } else if (navigator.userAgent.indexOf("Opera") != -1) {
        alert("Please use chrome.");
        window.location.href = "https://support.google.com/chrome/answer/95346?co=GENIE.Platform%3DDesktop&hl=en";
    } else if (navigator.userAgent.indexOf("MSIE") != -1) {
        alert("Please use chrome.");
        window.location.href = "https://support.google.com/chrome/answer/95346?co=GENIE.Platform%3DDesktop&hl=en";
    } else if (navigator.userAgent.indexOf("Firefox") != -1) {
        alert("Please use chrome.");
        window.location.href = "https://support.google.com/chrome/answer/95346?co=GENIE.Platform%3DDesktop&hl=en";
    } else {
        alert("Please use chrome.");
        window.location.href = "https://support.google.com/chrome/answer/95346?co=GENIE.Platform%3DDesktop&hl=en";
    }
};

/**
 * Checks to see if the token is valid or not.
 * Will return true if valid, false if invalid.
 * @param string cookie
 */
function validToken(cookie) {
    var validToken = false;
    ajax("GET", false, "api/CheckToken/"+cookie, null, function(valid) {
        validToken = valid;
    });
    return validToken;
};

/**
 * Gets the token from the cookie variable.
 * Returns the cookie found.
 * https://stackoverflow.com/questions/10730362/get-cookie-by-name
 */
function getTokenFromCookie() {
    var name = "bashto="; // name of the cookie
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(";");
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == " ") {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
};

/**
 * Sets the cookie to the new token value.
 * @param string cvalue
 * @param int exdays
 */
function setTokenCookie(cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = "bashto=" + cvalue + ";" + expires + ";path=/";
};

/**
 * Helper function. Sends an error message to the screen
 * and console. If the error says that the token is invalid,
 * log the user out by resetting the cookie.
 * @param json errorData
 */
function sendErrorMessage(errorData) {
    alert(errorData.errMsg); // message for the user
    console.log(errorData.err); // message for the dev
    if (errorData.invalidToken) {
        document.cookie = 'bashto=;expires=Thu, 01 Jan 1970 00:00:01 GMT;';
        window.location.href = "/";
    }
};

/**
 * Gets the user handle from a player ID.
 * @param int id
 */
function getUserHandle(id) {
    var handle = "Unknown";
    ajax("GET", false, "api/Player/by-id/" + id, null, function(userData) {
        handle = userData.handle;
    });   
    return handle;
};

/**
 * Gets the user handle from the token.
 * @param {any} cookie
 */
function getUserHandleFromToken(cookie) {
    ajax("GET", true, "api/Player/by-token/" + cookie, null, function(userData) {
        if (userData.err != null) {
            sendErrorMessage(userData);
        } else {
            $("#chat-handle").text(userData.handle);
            console.log(userData.handle);
        }
    });    
};

/**
 * Gets the user ID from the token.
 * @param {any} cookie
 */
function getUserIdFromToken(cookie) {
    var userId = null;
    ajax("GET", false, "api/Player/by-token/" + cookie, null, function(userData) {
        if (userData.err != null) {
            sendErrorMessage(userData);
        } else {
            userId = userData.player_Id;
        }
    });
    return userId;
};

/**
 * Gets the user ID from the handle provided.
 * @param string handle
 * @param string token
 */
function getUserIdFromHandle(handle, token) {
    var userId = null;
    ajax("GET", false, "api/Player/by-handle/" + handle + "/" + token, null, function(playerData) {
        if (playerData.err != null) {
            sendErrorMessage(playerData);
        } else {
            userId = playerData.player_Id;
        }
    });
    return userId;
};

/**
 * Helper function. Makes an ajax call.
 * @param string type
 * @param bool async
 * @param string url
 * @param json data
 * @param function success
 */
function ajax(type, async, url, data, success) {
    if (data != null) {
        $.ajax({
            type: type,
            cache: false,
            async: async,
            dataType: "json",
            url: window.location.protocol + "//" + window.location.host + "/" + url,
            data: data,
            success: success,
            error: function(error) {
                console.log(error);
            }
        });
    } else {
        $.ajax({
            type: type,
            cache: false,
            async: async,
            dataType: "json",
            url: window.location.protocol + "//" + window.location.host + "/" + url,
            success: success,
            error: function (error) {
                console.log(error);
            }
        });
    }
};