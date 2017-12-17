//////////////////////////////////////////////////////////////////
//                                                              //
//  Initial Load Javascript File						        //
//  Description:  This file contains the scripts required       //
//  to check and initialize a game                              //
//                                                              //
//////////////////////////////////////////////////////////////////
var loggedInUserId = -1;

$(document).ready(function () {
    console.log("Checking login credentials...");

    // Retrieve the token from the cookie
    var cookie = getTokenFromCookie();
    if (cookie !== "" && cookie !== null) {

        // Ensure the token is valid
        if (validToken(cookie)) {
            console.log("Valid login credentials detected!");

            // Retrieve logged in user ID from the token
            loggedInUserId = getUserIdFromToken(cookie);

            // Get the intended players of the game from the URL
            var p1 = getURLParameter("p1");
            var p2 = getURLParameter("p2");
            
            // Load the current game for the players
            if (loggedInUserId == p1 && loggedInUserId != -1) {
                loadCurrentGame(loggedInUserId, p2, cookie);
            } else if (loggedInUserId == p2 && loggedInUserId != -1) {
                loadCurrentGame(loggedInUserId, p1, cookie);
            } else {
                alert("You are not a player for this game. Rerouting to main site.");
                window.location.href = "/";
            }
        } else {
            alert("Invalid login credentials detected. Rerouting to main site");
            window.location.href = "/";
        }
    } else {
        alert("No login credentials detected - Please login again. Rerouting to main site.");
        window.location.href = "/";
    }
});

/*
 * Loads the current game data.
 * PARAM: int loggedInUserId
 * PARAM: int p2
 * PARAM: string cookie
 */
function loadCurrentGame(loggedInUserId, p2, cookie) {
    console.log("Loading current game...");
    // AJAX call to get the game from the DB
    $.ajax({
        type: "GET",
        cache: false,
        async: false,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/Game/"+loggedInUserId+"/"+p2+"/"+cookie,
        success: function (gameData) {
            if (gameData.errMsg != null) {
                sendErrorMessage(gameData);
            } else {
                // Set the hidden gameId input element value
                $("#gameId").val(gameData.game_Id);

                // Set the hidden board ID elements
                if (loggedInUserId == gameData.player_1_Id) {
                    $("#currentPlayer-board-id").val(gameData.player_1_Board_Id);
                    $("#opponent-board-id").val(gameData.player_2_Board_Id);

                    // Set the user handle names in their HTML elements
                    loadUserHandle(gameData.player_1_Id, "currentPlayer");
                    loadUserHandle(gameData.player_2_Id, "opponent");
                } else if (loggedInUserId == gameData.player_2_Id) {
                    $("#currentPlayer-board-id").val(gameData.player_2_Board_Id);
                    $("#opponent-board-id").val(gameData.player_1_Board_Id);

                    // Set the user handle names in their HTML elements
                    loadUserHandle(gameData.player_2_Id, "currentPlayer");
                    loadUserHandle(gameData.player_1_Id, "opponent");
                } else {
                    alert("Invalid login credentials detected. Rerouting to main site.");
                    window.location.href = "/";
                }

                // Check the game status to make sure the game is actually active
                checkGameStatus(gameData.game_Id, cookie);
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
};

/*
 * Loads the user handles into their HTML elements.
 * PARAM: int id
 * PARAM: string player
 */
function loadUserHandle(id, player) {
    console.log("Loading user handles");
    // AJAX call to get user data from DB
    $.ajax({
        type: "GET",
        cache: false,
        async: true,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/Player/by-id/" + id,
        success: function (playerData) {
            // Set the handles in the html
            $("." + player + "-handle").text(playerData.handle);
        },
        error: function (error) {
            console.log(error);
        }
    });
};

/*
 * Checks to make sure the game is active or not.
 * PARAM: int id
 * PARAM: string cookie
 */
function checkGameStatus(id, cookie) {
    console.log("Checking game status...");
    // AJAX call to get game data
    $.ajax({
        type: "GET",
        cache: false,
        async: true,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/Game/"+id+"/"+cookie,
        success: function (gameData) {
            if (gameData.errMsg != null) {
                sendErrorMessage(gameData);
            } else {
                if (gameData.complete) {
                    // If the game has already been completed, reroute to the main site
                    alert("Game has been completed. Rerouting to main site.");
                    window.location.href = "/";
                } else {
                    // If the game is still active, set the player turn
                    var playerTurn = gameData.turn;

                    console.log("TURN: " + playerTurn);
                    // If the player turn is not -1 (aka the game has not started yet), set the HTML style
                    if (playerTurn == loggedInUserId && playerTurn !== -1) {
                        $(".currentPlayer-handle").addClass("player-turn");
                    } else if (playerTurn !== -1) {
                        $(".opponent-handle").addClass("player-turn");
                    }
                    // Initialize the game
                    game.init();
                }
            }           
        },
        error: function (error) {
            console.log(error);
        }
    });
};

// HELPER FUNCTION TO GET THE URL PARAMS
// FOUND AT: https://stackoverflow.com/questions/11582512/how-to-get-url-parameters-with-javascript/11582513#11582513
function getURLParameter(name) {
    return decodeURIComponent((new RegExp('[?|&]' + name + '=' + '([^&;]+?)(&|#|;|$)').exec(location.search) || [null, ''])[1].replace(/\+/g, '%20')) || null;
};