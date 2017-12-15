function getGame(p1, p2, cookie) {
    console.log("Retrieving game data...");
    var data;
    $.ajax({
        type: "GET",
        cache: false,
        async: false,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/Game/" + p1 + "/" + p2 + "/" + cookie,
        success: function (gameData) {
            if (gameData.errMsg != null) {
                sendErrorMessage(gameData);
            } else {
                data = gameData;
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
    return data;
};

function getBoard(boardId) {
    var data;
    $.ajax({
        type: "GET",
        cache: false,
        async: false,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/Board/" + boardId + "/" + cookie,
        success: function (boardData) {
            if (boardData.errMsg != null) {
                sendErrorMessage(boardData);
            } else {
                data = boardData;
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
    return data;
};

function makeGameComponents(challengeData, cookie) {
    console.log("Making new game components.");
    var player1Id = challengeData.player_1;
    var player2Id = challengeData.player_2;
    console.log("Player 1 ID: " + player1Id);
    console.log("Player 2 ID: " + player2Id);

    var gameData = getGame(player1Id, player2Id, cookie);
    console.log("Game: " + gameData);

    makeStarterShips(gameData.player_1_Board_Id);
    makeStarterShips(gameData.player_2_Board_Id);
};

function makeStarterShips(boardId) {
    console.log("Making starter ships for board " + boardId);
    $.ajax({
        type: "POST",
        cache: false,
        async: true,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/Ship/starter/" + boardId,
        success: function (success) {
            if (success) {
                console.log("Ships made for " + boardId);
                makeStarterShipLocations(boardId);
            } else {
                alert("Error making ships for new board.");
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
};

function makeStarterShipLocations(boardId) {
    console.log("Making starter ship locations...");
    var ships;

    $.ajax({
        type: "GET",
        cache: false,
        async: false,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/Ship/all-by-board/" + boardId,
        success: function (shipData) {
            ships = shipData;
        },
        error: function (error) {
            console.log(error);
        }
    });

    for (var x = 0; x < ships.length; x++) {
        var ship = ships[x];
        var shipType = ship.ship_Type_Id;
        var shipLength = getShipLength(shipType);

        for (var y = 0; y < shipLength; y++) {
            var shipLocation = {
                Ship_Location_Id: -1,
                Ship_Id: ship.ship_Id,
                Board_Id: boardId,
                Row: x,
                Col: y
            };

            $.ajax({
                type: "POST",
                cache: false,
                async: true,
                dataType: "json",
                url: window.location.protocol + "//" + window.location.host + "/api/ShipLocation/createLocation",
                data: shipLocation,
                success: function (success) {
                    if (success) {
                        console.log("Location made.");
                    } else {
                        console.log("Error making location.");
                    }
                },
                error: function (error) {
                    console.log(error);
                }
            });
        }
    }
};

function getShipLength(shipType) {
    var length = -1;

    $.ajax({
        type: "GET",
        cache: false,
        async: false,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/ShipType/" + shipType,
        success: function (shipTypeData) {
            length = shipTypeData.ship_Length;
        },
        error: function (error) {
            console.log(error);
        }
    });

    return length;
};