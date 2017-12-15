/*
 * Contains scripts used for retrieving and validating
 * tokens in Battleship.
 * @author Kristen Merritt
 */

/*
 * Checks to see if a token is valid. Returns true if valid.
 * PARAM: string cookie
 * RETURN: bool valid
 */
function validToken(cookie) {
    console.log("Validating token...");
    var valid = false;
    $.ajax({
        type: "GET",
        cache: false,
        async: false,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/CheckToken/" + cookie,
        success: function (bool) {
            valid = bool;
        },
        error: function (error) {
            console.log(error);
        }
    });

    if (valid) {
        console.log("Token is valid.");
    } else {
        console.log("Token is invalid.");
    }

    return valid;
};

/*
 * Gets the token from the cookie.
 * RETURN: string token
 */
function getTokenFromCookie() {
    console.log("Getting token from cookie...");
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
    console.log("Setting token...");
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
    $.ajax({
        type: "GET",
        cache: false,
        async: false,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/Player/by-id/" + id,
        success: function (userData) {
            handle = userData.handle;
        },
        error: function (error) {
            console.log(error);
        }
    });
    return handle;
};

/*
 * Retrieves the user handle from token provided.
 * PARAM: string cookie
 * RETURN: string handle
 */
function getUserHandleFromToken(cookie) {
    console.log("Getting user handle from token...");
    $.ajax({
        type: "GET",
        cache: false,
        async: true,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/Player/by-token/" + cookie,
        success: function (userData) {
            if (userData.err == null) {
                $("#chat-handle").text(userData.handle);
                console.log(userData.handle);
                            
            } else {
                console.log(userData.err);
                alert(userData.err);  
            }
            
        },
        error: function (error) {
            console.log(error);
        }
    });
};

/*
 * Gets a user ID from the token provided.
 * PARAM: string cookie
 * RETURN: int userId
 */
function getUserIdFromToken(cookie) {
    console.log("Getting user id from token...");
    var userId = -1;
    $.ajax({
        type: "GET",
        cache: false,
        async: false,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/Player/by-token/" + cookie,
        success: function (userData) {
            userId = userData.player_Id;
        },
        error: function (error) {
            console.log(error);
        }
    });
    return userId;
};

function getUserIdFromHandle(handle, token) {
    console.log("Getting user id from handle...");
    var playerId = -1;
    $.ajax({
        type: "GET",
        cache: false,
        async: false,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/Player/by-handle/" + handle + "/" + token,
        success: function (playerData) {
            playerId = playerData.player_Id;
        },
        error: function (error) {
            console.log(error);
        }
    });
    return playerId;
}