/*
 * Contains scripts used in all pages.
 * @author Kristen Merritt
 */

/*
 * Checks to see if a token is valid. Returns true if valid.
 * PARAM: string cookie
 * RETURN: bool valid
 */
function validToken(cookie) {
    //console.log("Validating token...");
    var validToken = false;
    ajax("GET", false, "api/CheckToken/"+cookie, null, function(valid) {
        validToken = valid;
    });
    return validToken;
};

/*
 * Gets the token from the cookie.
 * RETURN: string token
 */
function getTokenFromCookie() {
    //console.log("Getting token from cookie...");
    var name = "bashto=";
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

/*
 * Sets the cookie to the value and expiration specified.
 * PARAM: string cvalue, int exdays
 */
function setTokenCookie(cvalue, exdays) {
    //console.log("Setting token...");
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = "bashto=" + cvalue + ";" + expires + ";path=/";
};

function sendErrorMessage(errorData) {
    console.log(errorData);
    alert(errorData.errMsg);
    console.log(errorData.err);
    if (errorData.invalidToken) {
        document.cookie = 'bashto=;expires=Thu, 01 Jan 1970 00:00:01 GMT;';
        window.location.href = "/";
    }
};

/*
 * Retrieves the user handle from an ID.
 * PARAM: int id
 * RETURN: string handle
 */
function getUserHandle(id) {
    //console.log("Getting user handle from id...");
    var handle = "Unknown";
    ajax("GET", false, "api/Player/by-id/" + id, null, function(userData) {
        handle = userData.handle;
    });   
    return handle;
};

/*
 * Retrieves the user handle from token provided.
 * PARAM: string cookie
 * RETURN: string handle
 */
function getUserHandleFromToken(cookie) {
    //console.log("Getting user handle from token...");
    ajax("GET", true, "api/Player/by-token/" + cookie, null, function(userData) {
        if (userData.err != null) {
            sendErrorMessage(userData);
        } else {
            $("#chat-handle").text(userData.handle);
            console.log(userData.handle);
        }
    });    
};

/*
 * Gets a user ID from the token provided.
 * PARAM: string cookie
 * RETURN: int userId
 */
function getUserIdFromToken(cookie) {
    //console.log("Getting user id from token...");
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

/*
 * Gets a user ID from the handle provided.
 * PARAM: string handle
 * PARAM: string token
 * RETURN: int userId
 */
function getUserIdFromHandle(handle, token) {
    //console.log("Getting user id from handle...");
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

/*
 * Helper function that makes an ajax call
 * PARAM: string type
 * PARAM: bool async
 * PARAM: string url
 * RETURN: json dataReturned
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