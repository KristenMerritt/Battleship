function getGame(p1, p2, cookie) {
    console.log("Retrieving game data...");
    var data;
    ajax("GET", false, "api/Game/" + p1 + "/" + p2 + "/" + cookie, null, function(gameData) {
        if (gameData.errMsg != null) {
            sendErrorMessage(gameData);
        } else {
            data = gameData;
        }
    });   
    return data;
};

function getBoard(boardId) {
    var data;
    ajax("GET", false, "api/Board/" + boardId + "/" + cookie, null, function(boardData) {
        if (boardData.errMsg != null) {
            sendErrorMessage(boardData);
        } else {
            data = boardData;
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
    ajax("POST", true, "api/Ship/starter/" + boardId, null, function(success) {
        if (success) {
            console.log("Ships made for " + boardId);
            makeStarterShipLocations(boardId);
        } else {
            alert("Error making ships for new board.");
        }
    });
};

function makeStarterShipLocations(boardId) {
    console.log("Making starter ship locations...");
    var ships;
    ajax("GET", false, "api/Ship/all-by-board/" + boardId, null, function(shipData) {
        ships = shipData;
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

            ajax("POST", true, "api/ShipLocation/createLocation", shipLocation, function(success) {
                if (success) {
                    console.log("Location made.");
                } else {
                    console.log("Error making location.");
                }
            });
        }
    }
};

function getShipLength(shipType) {
    var length = -1;
    ajax("GET", false, "api/ShipType/" + shipType, null, function(shipTypeData) {
        length = shipTypeData.ship_Length;
    });
    return length;
};