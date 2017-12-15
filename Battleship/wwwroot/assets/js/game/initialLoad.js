$(document).ready(function () {
    console.log("Checking login credentials");
    var loggedInUserId = -1;
    var cookie = getTokenFromCookie();
    if (cookie !== "" && cookie !== null) {
        if (validToken(cookie)) {
            console.log("Valid token detected.");
            loggedInUserId = getUserIdFromToken(cookie);
            var p1 = getURLParameter("p1");
            var p2 = getURLParameter("p2");
            console.log("P1:" + p1);
            console.log("P2: " + p2);
            console.log("Logged in user: " + loggedInUserId);
            if ((loggedInUserId == p1 || loggedInUserId == p2) && loggedInUserId != -1) {
                loadCurrentGame(loggedInUserId, p2, cookie);
            } else {
                alert("Invalid login credentials detected. Rerouting to main site.");
                window.location.href = "/";
            }
        } else {
            alert("Invalid token detected.");
            window.location.href = "/";
        }
    } else {
        alert("No login credentials detected - Please login again.");
    }
   
    //if (userId !== -1) { // the user ID would be set to something different if there was a valid token
    //    window.setInterval(function () {
    //        checkForNewChallenges(userId);
    //    }, 2000);
    //}
});

//https://stackoverflow.com/questions/11582512/how-to-get-url-parameters-with-javascript/11582513#11582513
function getURLParameter(name) {
    return decodeURIComponent((new RegExp('[?|&]' + name + '=' + '([^&;]+?)(&|#|;|$)').exec(location.search) || [null, ''])[1].replace(/\+/g, '%20')) || null;
};

function loadCurrentGame(loggedInUserId, p2, cookie) {
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
                $("#gameId").val(gameData.game_Id);
                console.log("Game id: " + gameData.game_Id);

                if (loggedInUserId == gameData.player_1_Id) {
                    $("#currentPlayer-board-id").val(gameData.player_1_Board_Id);
                    $("#opponent-board-id").val(gameData.player_2_Board_Id);

                    loadUserHandle(gameData.player_1_Id, "currentPlayer");
                    loadUserHandle(gameData.player_2_Id, "opponent");
                } else if (loggedInUserId == gameData.player_2_Id) {
                    $("#currentPlayer-board-id").val(gameData.player_2_Board_Id);
                    $("#opponent-board-id").val(gameData.player_1_Board_Id);

                    loadUserHandle(gameData.player_2_Id, "currentPlayer");
                    loadUserHandle(gameData.player_1_Id, "opponent");
                } else {
                    alert("Invalid login credentials detected.");
                    window.location.href = "/";
                }
                checkGameStatus(gameData.game_Id, cookie);
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
};

function loadUserHandle(id, player) {
    $.ajax({
        type: "GET",
        cache: false,
        async: true,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/Player/by-id/" + id,
        success: function (playerData) {
            console.log(playerData);
            $("." + player + "-handle").text(playerData.handle);
        },
        error: function (error) {
            console.log(error);
        }
    });
};

function checkGameStatus(id, cookie) {
    $.ajax({
        type: "GET",
        cache: false,
        async: true,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/Game/"+id+"/"+cookie,
        success: function (gameData) {
            //console.log(gameData);
            if (gameData.errMsg != null) {
                sendErrorMessage(gameData);
            } else {
                if (gameData.complete) {
                    alert("Game has been completed.");
                    window.location.href = "/";
                } else {
                    var playerTurn = gameData.turn;
                    console.log("Turn: " + playerTurn);
                    if (playerTurn !== -1) {
                        $(".player-" + playerTurn + "-handle").attr("class", "player-turn");
                    }
                    game.init();
                }
            }           
        },
        error: function (error) {
            console.log(error);
        }
    });
};