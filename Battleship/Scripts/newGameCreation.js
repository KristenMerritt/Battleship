//////////////////////////////////////////////////////////////////
//                                                              //
//  New Game Creation Javascript File						    //
//  Description:  This file contains various scripts used       //
//                to create a new battleship game.              //
//                                                              //
//////////////////////////////////////////////////////////////////

/**
 * Gets a game from the database based on
 * player IDs provided.
 * @param int p1
 * @param int p2
 * @param string cookie
 */
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

/**
 * Gets a board based off of board ID.
 * @param int boardId
 */
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

/**
 * Initiates the creation of all  
 * the game components required for a
 * new game.
 * @param int challengeData
 * @param string cookie
 */
function makeGameComponents(challengeData, cookie) {
    var player1Id = challengeData.player_1;
    var player2Id = challengeData.player_2;

    // Get the game that was created when the challenge was accepted
    var gameData = getGame(player1Id, player2Id, cookie);

    // Make the starter ships and locations for both boards
    makeStarterShips(gameData.player_1_Board_Id);
    makeStarterShips(gameData.player_2_Board_Id);
};

/**
 * Creates the starter ships for a board
 * @param int boardId
 */
function makeStarterShips(boardId) {
    ajax("POST", true, "api/Ship/starter/" + boardId, null, function(success) {
        if (success) {
            makeStarterShipLocations(boardId);
        } else {
            alert("Error making ships for new board.");
        }
    });
};

/**
 * Creates the starter ship locations for a board
 * @param int boardId
 */
function makeStarterShipLocations(boardId) {
    var ships;

    // Get the ships from the DB
    ajax("GET", false, "api/Ship/all-by-board/" + boardId, null, function(shipData) {
        ships = shipData;
    });

    for (var x = 0; x < ships.length; x++) {
        var ship = ships[x];
        var shipType = ship.ship_Type_Id;
        var shipLength = getShipLength(shipType);

        // Create pieces to achieve the ship length
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
                    //console.log("Location made.");
                } else {
                    console.log("Error making location.");
                }
            });
        }
    }
};

/**
 * Get the length of a ship
 * @param int shipType
 */
function getShipLength(shipType) {
    var length = -1;
    ajax("GET", false, "api/ShipType/" + shipType, null, function(shipTypeData) {
        length = shipTypeData.ship_Length;
    });
    return length;
};