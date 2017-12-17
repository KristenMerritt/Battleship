//////////////////////////////////////////////////////////////////
//                                                              //
//  Initial Load Javascript File						        //
//  Description:  This file contains the scripts required       //
//  to check and initialize a game                              //
//                                                              //
//////////////////////////////////////////////////////////////////

$(document).ready(function () {
    // Retrieve the token from the cookie
    var cookie = getTokenFromCookie();
    if (cookie !== "" && cookie !== null) {

        // Ensure the token is valid
        if (validToken(cookie)) {
            console.log("Valid login credentials detected! Loading game...");

            // Retrieve logged in user ID from the token
            var loggedInUserId = getUserIdFromToken(cookie);
            console.log("Logged in user id for game: " + loggedInUserId);

            // Get the intended players of the game from the URL
            var p1 = getURLParameter("p1");
            var p2 = getURLParameter("p2");
            
            // Load the current game for the players
            if (loggedInUserId == p1) {
                loadCurrentGame(loggedInUserId, p2, cookie);
            } else if (loggedInUserId == p2 ) {
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
    ajax("GET", false, "api/Game/" + loggedInUserId + "/" + p2 + "/" + cookie, null, function(gameData) {
        if (gameData.errMsg != null) {
            sendErrorMessage(gameData);
        } else {
            // Set the hidden gameId input element value
            game.gameId = gameData.game_Id;
            game.turn = gameData.turn;
            game.complete = gameData.complete;
            //$("#gameId").val(gameData.game_Id);

            // Set the hidden board ID elements
            if (loggedInUserId == gameData.player_1_Id) {
                game.currentPlayerId = gameData.player_1_Id;
                game.currentPlayerBoardId = gameData.player_1_Board_Id;
                game.opponentPlayerId = gameData.player_2_Id;
                game.opponentPlayerBoardId = gameData.player_2_Board_Id;
                loadUserHandle(gameData.player_1_Id, "currentPlayer");
                loadUserHandle(gameData.player_2_Id, "opponent");

                //$("#currentPlayer-board-id").val(gameData.player_1_Board_Id);
                //$("#opponent-board-id").val(gameData.player_2_Board_Id);
            } else if (loggedInUserId == gameData.player_2_Id) {
                game.currentPlayerId = gameData.player_2_Id;
                game.currentPlayerBoardId = gameData.player_2_Board_Id;
                game.opponentPlayerId = gameData.player_1_Id;
                game.opponentPlayerBoardId = gameData.player_1_Board_Id;
                loadUserHandle(gameData.player_2_Id, "currentPlayer");
                loadUserHandle(gameData.player_1_Id, "opponent");

                //$("#currentPlayer-board-id").val(gameData.player_2_Board_Id);
                //$("#opponent-board-id").val(gameData.player_1_Board_Id);
            } else {
                alert("Invalid login credentials detected. Rerouting to main site.");
                window.location.href = "/";
            }

            console.log(game);
            // Check the game status to make sure the game is actually active
            if (game.complete) {
                // If the game has already been completed, reroute to the main site
                alert("Game has been completed. Rerouting to main site.");
                window.location.href = "/";
            } else {
                // If the game is still active, set the player turn
                if (game.turn == game.currentPlayerId) {
                    $(".currentPlayer-handle").addClass("player-turn");
                } else if (game.turn == game.opponentPlayerId) {
                    $(".opponent-handle").addClass("player-turn");
                }

                // Initialize the rest of the game
                game.init();
            }
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
    ajax("GET", true, "api/Player/by-id/" + id, null, function(playerData) {
        $("." + player + "-handle").text(playerData.handle);
    });    
};

// HELPER FUNCTION TO GET THE URL PARAMS
// FOUND AT: https://stackoverflow.com/questions/11582512/how-to-get-url-parameters-with-javascript/11582513#11582513
function getURLParameter(name) {
    return decodeURIComponent((new RegExp('[?|&]' + name + '=' + '([^&;]+?)(&|#|;|$)').exec(location.search) || [null, ''])[1].replace(/\+/g, '%20')) || null;
};