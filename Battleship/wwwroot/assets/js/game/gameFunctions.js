//////////////////////////////////////////////////////////////////
//                                                              //
//  Game Functions Javascript File						        //
//  Description:  This file contains the scripts required       //
//  to player the game                                          //
//                                                              //
//////////////////////////////////////////////////////////////////

// The game object
var game = {
    xhtmlns: "http://www.w3.org/1999/xhtml",
    svgns: "http://www.w3.org/2000/svg",
    BOARDX: 0, //starting pos of board
    BOARDY: 0,
    BOARDWIDTH: 10, // how many squares across
    BOARDHEIGHT: 10, // how many squares down
    CELLSIZE: 50, // size of cells

    board1Arr: new Array(), // 2d array [row][col] for the board for player 1
    board2Arr: new Array(), // 2d array [row][col] for the board for player 2

    hole1Arr: new Array(), // 2d array [row][col] of holes for player 1
    hole2Arr: new Array(), // 2d array [row][col] of holes for player 2

    // Only display and store the ships for the logged in player
    // Everything else will be done via shots in the DB
    cruiser: null, // The cruiser for the logged in player
    battleship: null, // the battleshipe for the logged in player
    destroyer: null, // the destroyer for the logged in player
    submarine: null, // the submarine for the logged in player
    carrier: null, // the carrier for hte logged in player

    carrierPieces: new Array(), // Pieces associated with the carrier
    battleshipPieces: new Array(), // pieces associated with the battleship
    submarinePieces: new Array(), // pieces associated with the submarine
    cruiserPieces: new Array(), // pieces associated with the cruiser
    destroyerPieces: new Array(), // pieces associated with the destroyer

    gameId: -1,
    complete: false,
    currentPlayerId: -1,
    opponentPlayerId: -1,
    currentPlayerBoardId: -1,
    opponentPlayerBoardId: -1,
    lastShot: -1,
    turn: -1,
    winningBoard: -1,

    // Initialize the game
    init: function () {
        var svgs = document.getElementsByTagName("svg");

        // Create boards for the logged in player and the opponent
        var times = 1;
        while (times < 3) {
            var gEle = document.createElementNS(game.svgns, 'g');
            gEle.setAttributeNS(null, 'transform', 'translate(' + game.BOARDX + ',' + game.BOARDY + ')');
            gEle.setAttributeNS(null, 'id', 'gId_' + game.gameId + '_p' + times);
            gEle.setAttributeNS(null, 'fill', '#b0b8c4');

            // Stick g on board
            svgs[times - 1].insertBefore(gEle, svgs[times - 1].childNodes[5]);
            times += 1;
        }

        // Create the cells and holes on the logged in player's board
        for (var i = 0; i < game.BOARDWIDTH; i++) {
            game.board1Arr[i] = new Array();
            game.hole1Arr[i] = new Array();
            for (j = 0; j < game.BOARDHEIGHT; j++) {
                game.board1Arr[i][j] = new Cell(document.getElementById("gId_" + game.gameId + "_p1"),
                    'cell_' + j + i + '_p1', game.CELLSIZE, j, i);
                game.hole1Arr[i][j] = new Hole(document.getElementById("gId_" + game.gameId + "_p1"),
                    'hole_' + j + "|" + i + '_p1', 20, j, i, (j * 50) + 25, (i * 50) + 25);
            }
        }

        // Create the cells and holes on the opponent's board
        for (var i = 0; i < game.BOARDWIDTH; i++) {
            game.board2Arr[i] = new Array();
            game.hole2Arr[i] = new Array();
            for (j = 0; j < game.BOARDHEIGHT; j++) {
                game.board2Arr[i][j] = new Cell(document.getElementById('gId_' + game.gameId + '_p2'),
                    'cell_' + j + i + '_p2', game.CELLSIZE, j, i);
                game.hole2Arr[i][j] = new Hole(document.getElementById('gId_' + game.gameId + '_p2'),
                    'hole_' + j + "|" + i + '_p2', 20, j, i, (j * 50) + 25, (i * 50) + 25);
            }
        }

        // Load in the ships for the logged in player
        loadShips();

        // Load in the shots for both players
        loadShotsData("currentPlayer");
        loadShotsData("opponent");

        // Put the drop code on the document...
        document.getElementsByTagName('svg')[0].addEventListener('mouseup', drag.releaseMove, false);

        // Put the go() method on the svg doc.
        document.getElementsByTagName('svg')[0].addEventListener('mousemove', drag.go, false);

        initializeRestartGameButton();
    },
    announceWinner: function () {
        var cookie = getTokenFromCookie();
        if (cookie !== "" && cookie !== null) {
            if (validToken(cookie)) {
                // Check to make sure the game has actually been won
                ajax("GET", true, "api/Game/" + gameId + "/" + cookie, null, function (gameData) {
                    if (gameData.complete == 1) {
                        if (game.winningBoard == game.currentPlayerBoardId) {
                            $("#main-container").apend("<h1>Winner: " + $(".currentPlayer-handle").val() + "</h1>");
                        } else if (game.winningBoard == game.opponentPlayerBoardId) {
                            $("#main-container").apend("<h1>Winner: " + $(".opponent-handle").val() + "</h1>");
                        }
                    }
                });       
            }
        }
    }
}

/**
 * Returns a ship length
 * @param int shipType
 */
function getShipLength(shipType) {
    var length = -1;
    ajax("GET", false, "api/ShipType/" + shipType, null, function (shipTypeData) {
        length = shipTypeData.ship_Length;
    });
    return length;
};

/**
 * Checks the DB for the player turn.
 * Will change the turn in the HTML
 * and the game object.
 */
function checkForTurn() {
    var cookie = getTokenFromCookie();
    ajax("GET", true, "api/Game/" + game.gameId + "/" + cookie, null, function(gameData) {
        if (gameData.errMsg != null) {
            sendErrorMessage(gameData);
        } else {
            console.log("It is " + gameData.turn + "'s turn.");
            game.turn = gameData.turn;
            if (game.turn == game.currentPlayerId) {
                $(".opponent-handle").removeClass("player-turn");
                $(".currentPlayer-handle").addClass("player-turn");
            } else if (game.turn == game.opponentPlayerId) {
                $(".currentPlayer-handle").removeClass("player-turn");
                $(".opponent-handle").addClass("player-turn");
            }
        }
    });   
};

/**
 * Checks the DB for new shots.
 * Will updated the HTML with the shots
 * and also modify the game.lastShot.
 */
function checkForShots() {
    console.log("Checking for new shots...");
    ajax("GET", true, "api/Shot/new-shots/" + game.lastShot + "/" + game.currentPlayerBoardId + "/" + game.opponentPlayerBoardId, null, function(shotData) {
        if (shotData.length > 0) {
            for (var x = 0; x < shotData.length; x++) {
                var shot = shotData[x];
                var boardId = shot.board_Id;
                var hole;

                // Get the hole associated with the shot
                if (boardId == game.currentPlayerBoardId) {
                    hole = document.getElementById("hole_" + shot.row + "|" + shot.col + "_p1");
                } else if (boardId == game.opponentPlayerBoardId) {
                    hole = document.getElementById("hole_" + shot.row + "|" + shot.col + "_p2");
                }

                if (shot.is_Hit) {
                    // If the shot was a hit, set the element to display a hit
                    hole.setAttributeNS(null, 'occupied', 'hit');
                    hole.setAttributeNS(null, 'class', 'hole_hit');
                    hole.setAttributeNS(null, 'fill', 'red');

                    // If the shot was a hit, and the board was the logged in player, set the ship to be red as well
                    if (boardId == game.currentPlayerBoardId) {
                        var shipPiece = $("rect[row=" + shot.col + "][col=" + shot.row + "]");
                        shipPiece[0].setAttributeNS(null, "fill", "red");
                    }                   
                } else {
                    hole.setAttributeNS(null, 'occupied', 'miss');
                    hole.setAttributeNS(null, 'class', 'hole_miss');
                    hole.setAttributeNS(null, 'fill', 'white');
                }

                // Set the last shot to be the most recent shot
                game.lastShot = shot.shot_Id;
            }
        }
    });   
};

/**
 * Loads in all of the ships for the game.
 */
function loadShips() {
    ajax("GET", true, "api/Ship/all-by-board/" + game.currentPlayerBoardId, null, function (shipData) {
        if (shipData.length > 0) {
            for (var x = 0; x < shipData.length; x++) {
                var ship = shipData[x];
                var shipType = getShipType(ship.ship_Id);

                if (shipType != null) {
                    switch (shipType) {
                        case "Carrier": // Create the current user's carrier ship
                            game.carrier = new Ship(document.getElementById("gId_" + game.gameId + "_p1"), "Carrier", [], ship.is_Placed, ship.is_Sunk);
                            break;

                        case "Battleship": // Create the current user's battleship ship
                            game.battleship = new Ship(document.getElementById("gId_" + game.gameId + "_p1"), "Battleship", [], ship.is_Placed, ship.is_Sunk);
                            break;

                        case "Submarine": // Create the current user's submarine ship
                            game.submarine = new Ship(document.getElementById("gId_" + game.gameId + "_p1"), "Submarine", [], ship.is_Placed, ship.is_Sunk);
                            break;

                        case "Cruiser": // Create the current user's cruiser ship
                            game.cruiser = new Ship(document.getElementById("gId_" + game.gameId + "_p1"), "Cruiser", [], ship.is_Placed, ship.is_Sunk);
                            break;

                        case "Destroyer": // Create the current user's destroyer ship
                            game.destroyer = new Ship(document.getElementById("gId_" + game.gameId + "_p1"), "Destroyer", [], ship.is_Placed, ship.is_Sunk);
                            break;
                    } // end switch     
                }
            } // end loop through shipData

            if (game.carrier != null &&
                game.battleship != null &&
                game.submarine != null &&
                game.cruiser != null &&
                game.destroyer != null) {
                // Load the ship pieces for the ships
                loadShipPieces();
            } else {
                alert("Error loading ships, please try again.");
            }
        } else {
            alert("No ships found for game.");
        }
    });
};

/**
 * Loads in all of the ship pieces and location for the game.
 */
function loadShipPieces() {
    ajax("GET", true, "api/ShipLocation/board/" + game.currentPlayerBoardId, null, function(shipLocationData) {
        if (shipLocationData.length > 0) {
            var carrierCount = 0;
            var battleshipCount = 0;
            var submarineCount = 0;
            var cruiserCount = 0;
            var destroyerCount = 0;

            for (var x = 0; x < shipLocationData.length; x++) {
                var shipLocation = shipLocationData[x];
                var row = shipLocation.row;
                var col = shipLocation.col;
                var isHit = checkForHit(row, col);
                var shipType = getShipType(shipLocation.ship_Id);

                if (shipType != null) {
                    switch (shipType) {
                        case "Carrier": // Add piece to the carrier array
                            game.carrierPieces[carrierCount] = new ShipPiece(document.getElementById("g_Carrier"), row, col, carrierCount, isHit);
                            carrierCount++;
                            break;

                        case "Battleship": // Add piece to the battleship array
                            game.battleshipPieces[battleshipCount] = new ShipPiece(document.getElementById("g_Battleship"), row, col, battleshipCount, isHit);
                            battleshipCount++;
                            break;

                        case "Submarine": // Add piece to the submarine array
                            game.submarinePieces[submarineCount] = new ShipPiece(document.getElementById("g_Submarine"), row, col, submarineCount, isHit);
                            submarineCount++;
                            break;

                        case "Cruiser": // Add piece to the cruiser array
                            game.cruiserPieces[cruiserCount] = new ShipPiece(document.getElementById("g_Cruiser"), row, col, cruiserCount, isHit);
                            cruiserCount++;
                            break;

                        case "Destroyer": // Add piece to the destroyer array
                            game.destroyerPieces[destroyerCount] = new ShipPiece(document.getElementById("g_Destroyer"), row, col, destroyerCount, isHit);
                            destroyerCount++;
                            break;
                    }
                }
            }

            // Associate the pieces with their ships and place them on the board
            game.carrier.shipPieces = game.carrierPieces;
            game.carrier.initialPlace();

            game.battleship.shipPieces = game.battleshipPieces;
            game.battleship.initialPlace();

            game.submarine.shipPieces = game.submarinePieces;
            game.submarine.initialPlace();

            game.cruiser.shipPieces = game.cruiserPieces;
            game.cruiser.initialPlace();

            game.destroyer.shipPieces = game.destroyerPieces;
            game.destroyer.initialPlace();

            // Remove the spinner
            $("#loading_game").remove();

            // Begin checking for whose turn it is every 2 seconds
            window.setInterval(function () {
                checkForTurn();
            }, 1000);

            // Begin checking for shots every 1.5 seconds
            window.setInterval(function () {
                checkForShots();
            }, 1500);
        }
    });    
};

/**
 * Checks the hole at a specific location
 * to see if it is a hit.
 * @param int row
 * @param int col
 */
function checkForHit(row, col) {
    var hole = document.getElementById("hole_" + col + "|" + row + "_p1");
    if ($(hole).hasClass("hole_hit")) {
        return true;
    }
    return false;
}

/**
 * Returns the shipType name of a ship
 * @param string shipId
 */
function getShipType(shipId) {
    var shipTypeId;
    var shipType;

    ajax("GET", false, "api/Ship/ship/" + shipId, null, function(shipData) {
        shipTypeId = shipData.ship_Type_Id;
    });
    ajax("GET", false, "api/ShipType/" + shipTypeId, null, function(shipTypeData) {
        shipType = shipTypeData.ship_Type;
    });

    return shipType;
};

/**
 * Returns the shots for a specific player.
 * @param string player
 */
function loadShotsData(player) {
    var boardId;
    var board;

    if (player == "opponent") {
        board = "p2";
        boardId = game.opponentPlayerBoardId;
    } else {
        board = "p1";
        boardId = game.currentPlayerBoardId;
    }

    ajax("GET", true, "api/Shot/all-by-board/" + boardId, null, function (shotData) {
        if (shotData.length > 0) {
            for (var x = 0; x < shotData.length; x++) {
                var shot = shotData[x];
                var hole = document.getElementById("hole_" + shot.row + "|" + shot.col + "_" + board);

                if (shot.is_Hit) {
                    hole.setAttributeNS(null, 'occupied', 'hit');
                    hole.setAttributeNS(null, 'class', 'hole_hit');
                    hole.setAttributeNS(null, 'fill', 'red');
                } else {
                    hole.setAttributeNS(null, 'occupied', 'miss');
                    hole.setAttributeNS(null, 'class', 'hole_miss');
                    hole.setAttributeNS(null, 'fill', 'white');
                }

                game.lastShot = shot.shot_Id;
            }
        }
    });   
};

///////////////////////Dragging code/////////////////////////
var drag = {
    ship: null,                 //hold the g element I'm moving
    shipId: '',
    shipX: '',					//hold the position of the last position the first ShipPiece was in.
    shipY: '',					//hold the position of the last position the first ShipPiece was in.
    shipLength: '',
    shipPiecesCount: '',
    g: null,

    setMove: function (type) {
        switch(type) {
            case "Carrier":
                console.log("Carrier selected...");
                drag.ship = document.getElementById("g_Carrier");
                drag.shipId = "g_Carrier";
                drag.g = game.carrier;
                break;

            case "Battleship":
                console.log("Battleship selected...");
                drag.ship = document.getElementById("g_Battleship");
                drag.shipId = "g_Battleship";
                drag.g = game.battleship;
                break;

            case "Submarine":
                console.log("Submarine selected...");
                drag.ship = document.getElementById("g_Submarine");
                drag.shipId = "g_Submarine";
                drag.g = game.submarine;
                break;

            case "Cruiser":
                console.log("Cruiser selected...");
                drag.ship = document.getElementById("g_Cruiser");
                drag.shipId = "g_Cruiser";
                drag.g = game.cruiser;
                break;

            case "Destroyer":
                console.log("Destroyer selected...");
                drag.ship = document.getElementById("g_Destroyer");
                drag.shipId = "g_Destroyer";
                drag.g = game.destroyer;
                break;
        }

        // Get the last position of the ship
        xy = util.getTransform(drag.shipId);
        drag.shipX = xy[0];
        drag.shipY = xy[1];
        drag.shipLength = drag.ship.getAttribute("width");
        drag.shipPiecesCount = drag.shipLength / 50;

        //get the object then re-append it to the document so it is on top!
        drag.g.putOnTop();
    },
    releaseMove: function (evt) {
        if (drag.mover != '') {
            drag.ship = null;
            drag.g = null;
        }
    },
    go: function (evt) {
        if (drag.ship != null) {
            util.setTransform(drag.shipId, evt.layerX, evt.layerY);
        }
    }
}

/**
 * Creates the onclick event handler to
 * handle the restarting of the game.
 */
function initializeRestartGameButton() {
    $("#start_over").on("click",
        function (e) {
            e.preventDefault();
            var cookie = getTokenFromCookie();
            if (cookie !== "" && cookie !== null) {
                if (validToken(cookie)) {
                    // Empty the screen so the user cannot make any more moves
                    $("#currentPlayer-board-container").empty();
                    $("#opponent-board-container").empty();

                    // Set notice that the game is restarting
                    $("#main-container").append("<h1> Restarting game...</h1>");

                    // Restart the game in the DB
                    ajax("POST", true, "api/Game/start-over/" + game.gameId + "/" + cookie, null, function (gameData) {
                        if (gameData.errMsg != null) {
                            sendErrorMessage(gameData);
                        } else {
                            var opponentShips;
                            var currentPlayerShips;

                            // Recreate all of the starter ship's locations and shots
                            ajax("GET", false, "api/Ship/all-by-board/" + game.opponentPlayerBoardId, null, function (shipData) {
                                opponentShips = shipData;
                            });

                            ajax("GET", false, "api/Ship/all-by-board/" + game.currentPlayerBoardId, null, function (shipData) {
                                currentPlayerShips = shipData;
                            });


                            for (var x = 0; x < opponentShips.length; x++) {
                                var ship = opponentShips[x];
                                var shipType = ship.ship_Type_Id;
                                var shipLength = getShipLength(shipType);

                                for (var y = 0; y < shipLength; y++) {
                                    var opponentShipLocation = {
                                        Ship_Location_Id: -1,
                                        Ship_Id: ship.ship_Id,
                                        Board_Id: game.opponentPlayerBoardId,
                                        Row: x,
                                        Col: y
                                    };

                                    ajax("POST", true, "api/ShipLocation/createLocation", opponentShipLocation, function (success) {
                                        if (success) {
                                            //console.log("Location made.");
                                        } else {
                                            console.log("Error making location.");
                                        }
                                    });
                                } // end for shipLength
                            } // end for each opponent ship

                            for (var x = 0; x < currentPlayerShips.length; x++) {
                                var ship = currentPlayerShips[x];
                                var shipType = ship.ship_Type_Id;
                                var shipLength = getShipLength(shipType);

                                for (var y = 0; y < shipLength; y++) {
                                    var currentPlayerShipLocation = {
                                        Ship_Location_Id: -1,
                                        Ship_Id: ship.ship_Id,
                                        Board_Id: game.currentPlayerBoardId,
                                        Row: x,
                                        Col: y
                                    };

                                    ajax("POST", true, "api/ShipLocation/createLocation", currentPlayerShipLocation, function (success) {
                                        if (success) {
                                            console.log("Location made.");
                                        } else {
                                            console.log("Error making location.");
                                        }
                                    });
                                } // end for shipLength
                            } // end for each opponent ship

                            alert("Game has been restarted.");
                            location.reload();
                        } // end else      
                    }); // end original ajax call
                }
            }
        }
    );
};