//////////////////////////////////////////////////////////////////
//                                                              //
//  Game Functions Javascript File						        //
//  Description:  This file contains the scripts required       //
//  to player the game                                          //
//                                                              //
//////////////////////////////////////////////////////////////////


var lastShot; // the last shot ID
var turn;     // the current player's turn

// The game object
var game = {
    xhtmlns: "http://www.w3.org/1999/xhtml",
    svgns: "http://www.w3.org/2000/svg",
    BOARDX: 0,              //starting pos of board
    BOARDY: 0,
    BOARDWIDTH: 10,         // how many squares across
    BOARDHEIGHT: 10,        // how many squares down
    CELLSIZE: 50,           // size of cells

    board1Arr: new Array(), // 2d array [row][col] for the board for player 1
    board2Arr: new Array(), // 2d array [row][col] for the board for player 2

    hole1Arr: new Array(),  // 2d array [row][col] of holes for player 1
    hole2Arr: new Array(),  // 2d array [row][col] of holes for player 2

    // Only display and store the ships for the logged in player
    // Everything else will be done via shots in the DB
    cruiser: null,          // The cruiser for the logged in player
    battleship: null,       // the battleshipe for the logged in player
    destroyer: null,        // the destroyer for the logged in player
    submarine: null,        // the submarine for the logged in player
    carrier: null,          // the carrier for hte logged in player

    carrierPieces: new Array(),     // Pieces associated with the carrier
    battleshipPieces: new Array(),  // pieces associated with the battleship
    submarinePieces: new Array(),   // pieces associated with the submarine
    cruiserPieces: new Array(),     // pieces associated with the cruiser
    destroyerPieces: new Array(),   // pieces associated with the destroyer
    gameId: -1,

    // Initialize the game
    init: function () {       
        var svgs = document.getElementsByTagName("svg");
        game.gameId = $("#gameId").val(); 

        // Create boards for the logged in player and the opponent
        var times = 1;
        while (times < 3) {
            var gEle = document.createElementNS(game.svgns, 'g');
            gEle.setAttributeNS(null, 'transform', 'translate(' + game.BOARDX + ',' + game.BOARDY + ')');
            gEle.setAttributeNS(null, 'id', 'gId_' + gameId + '_p' + times);
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
                game.board1Arr[i][j] = new Cell(document.getElementById("gId_" + gameId + "_p1"), 'cell_' + j + i + '_p1', game.CELLSIZE, j, i);
                game.hole1Arr[i][j] = new Hole(document.getElementById("gId_" + gameId + "_p1"), 'hole_' + j + "|" + i + '_p1', 20, j, i, (j * 50) + 25, (i * 50) + 25);
            }
        }

        // Create the cells and holes on the opponent's board
        for (var i = 0; i < game.BOARDWIDTH; i++) {
            game.board2Arr[i] = new Array();
            game.hole2Arr[i] = new Array();
            for (j = 0; j < game.BOARDHEIGHT; j++) {
                game.board2Arr[i][j] = new Cell(document.getElementById('gId_' + gameId + '_p2'), 'cell_' + j + i + '_p2', game.CELLSIZE, j, i);
                game.hole2Arr[i][j] = new Hole(document.getElementById('gId_' + gameId + '_p2'), 'hole_' + j + "|" + i + '_p2', 20, j, i, (j * 50) + 25, (i * 50) + 25);
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

        // Begin checking for whose turn it is every 2 seconds
        window.setInterval(function () {
            checkForTurn();
        }, 1000);

        // Begin checking for shots every 1.5 seconds
        window.setInterval(function () {
            checkForShots();
        }, 1500);
    }
};

/*
 * Checks the DB for the player turn.
 */
function checkForTurn() {
    console.log("Checking turn...");
    var cookie = getTokenFromCookie();
    $.ajax({
        type: "GET",
        cache: false,
        async: true,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/Game/" + game.gameId + "/" + cookie,
        success: function (gameData) {
            if (gameData.errMsg != null) {
                sendErrorMessage(gameData);
            } else {
                if (gameData.turn == loggedInUserId) {
                    $(".opponent-handle").removeClass("player-turn");
                    $(".currentPlayer-handle").addClass("player-turn");           
                } else {
                    $(".currentPlayer-handle").removeClass("player-turn");
                    $(".opponent-handle").addClass("player-turn");
                }
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
};

function checkForShots() {
    console.log("Checking for new shots...");
    $.ajax({
        type: "GET",
        cache: false,
        async: true,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/Shot/new-shots/" + lastShot + "/" + $("#currentPlayer-board-id").val() + "/" + $("#opponent-board-id").val(),
        success: function (shotData) {
            if (shotData.length > 0) {
                console.log("New Shot data: " + shotData);
                for (var x = 0; x < shotData.length; x++) {
                    var shot = shotData[x];
                    var boardId = shot.board_Id;
                    var hole;

                    if (boardId == $("#currentPlayer-board-id").val()) {
                        hole = document.getElementById("hole_" + shot.row + "|" + shot.col + "_p1");
                    } else {
                        hole = document.getElementById("hole_" + shot.row + "|" + shot.col + "_p2");
                    }

                    if (shot.is_Hit) {
                        hole.setAttributeNS(null, 'occupied', 'hit');
                        hole.setAttributeNS(null, 'class', 'hole_hit');
                        hole.setAttributeNS(null, 'fill', 'red');
                    } else {
                        hole.setAttributeNS(null, 'occupied', 'miss');
                        hole.setAttributeNS(null, 'class', 'hole_miss');
                        hole.setAttributeNS(null, 'fill', 'white');
                    }

                    lastShot = shot.shot_Id;
                }
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
};

function loadShips() {
    console.log("Loading ships...");
    $.ajax({
        type: "GET",
        cache: false,
        async: true,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/Ship/all-by-board/" + $("#currentPlayer-board-id").val(),
        success: function (shipData) {
            if (shipData.length > 0) {
                for (var x = 0; x < shipData.length; x++) {
                    var ship = shipData[x];
                    var shipType = getShipType(ship.ship_Id);

                    if (shipType != null) {
                        switch (shipType) {
                        case "Carrier":
                            game.carrier = new Ship(document.getElementById("gId_" + $("#gameId").val() + "_p1"), "Carrier", [], ship.is_Placed, ship.is_Sunk);
                            break;

                        case "Battleship":
                            game.battleship = new Ship(document.getElementById("gId_" + $("#gameId").val() + "_p1"), "Battleship", [], ship.is_Placed, ship.is_Sunk);
                            break;

                        case "Submarine":
                            game.submarine = new Ship(document.getElementById("gId_" + $("#gameId").val() + "_p1"), "Submarine", [], ship.is_Placed, ship.is_Sunk);
                            break;

                        case "Cruiser":
                            game.cruiser = new Ship(document.getElementById("gId_" + $("#gameId").val() + "_p1"), "Cruiser", [], ship.is_Placed, ship.is_Sunk);
                            break;

                        case "Destroyer":
                            game.destroyer = new Ship(document.getElementById("gId_" + $("#gameId").val() + "_p1"), "Destroyer", [], ship.is_Placed, ship.is_Sunk);
                            break;
                        }    // end switch     
                    }                          
                } // end loop through shipData

                if (game.carrier != null &&
                    game.battleship != null &&
                    game.submarine != null &&
                    game.cruiser != null &&
                    game.destroyer != null) {

                    loadShipPieces();
                } else {
                    alert("Error loading ships, please try again.");
                }                               
            } else {
                alert("No ships found for game.");
            }
        }, // end success
        error: function (error) {
            console.log(error);
        }
    });
};

function loadShipPieces() {
    console.log("Loading ship locations and pieces... ");
    $.ajax({
        type: "GET",
        cache: false,
        async: true,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/ShipLocation/board/" + $("#currentPlayer-board-id").val(),
        success: function (shipLocationData) {
            if (shipLocationData.length > 0) {
                console.log("Ship location data length: " + shipLocationData.length);
                var carrierCount = 0;
                var battleshipCount = 0;
                var submarineCount = 0;
                var cruiserCount = 0;
                var destroyerCount = 0;

                for (var x = 0; x < shipLocationData.length; x++) {
                    var shipLocation = shipLocationData[x];
                    console.log(shipLocation);
                    var shipId = shipLocation.ship_Id;
                    var row = shipLocation.row;
                    var col = shipLocation.col;

                    var shipType = getShipType(shipId);
                    console.log(shipType);
                    if (shipType != null) {
                        switch (shipType) {
                        case "Carrier":
                            game.carrierPieces[carrierCount] = new ShipPiece(document.getElementById("g_Carrier"), row, col, carrierCount);
                            carrierCount++;
                            break;

                        case "Battleship":
                            game.battleshipPieces[battleshipCount] = new ShipPiece(document.getElementById("g_Battleship"), row, col, battleshipCount);
                            battleshipCount++;
                            break;

                        case "Submarine":
                            game.submarinePieces[submarineCount] = new ShipPiece(document.getElementById("g_Submarine"), row, col, submarineCount);
                            submarineCount++;
                            break;

                        case "Cruiser":
                            game.cruiserPieces[cruiserCount] = new ShipPiece(document.getElementById("g_Cruiser"), row, col, cruiserCount);
                            cruiserCount++;
                            break;

                        case "Destroyer":
                            game.destroyerPieces[destroyerCount] = new ShipPiece(document.getElementById("g_Destroyer"), row, col, destroyerCount);
                            destroyerCount++;
                            break;
                        }
                    }                  
                }

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

                $("#loading_game").remove();
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
};

function getShipType(shipId) {
    var shipTypeId;
    var shipTypeName;

    $.ajax({
        type: "GET",
        cache: false,
        async: false,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/Ship/ship/" + shipId,
        success: function (shipData) {
            shipTypeId = shipData.ship_Type_Id;
        },
        error: function (error) {
            console.log(error);
        }
    });

    if (shipTypeId != null) {
        $.ajax({
            type: "GET",
            cache: false,
            async: false,
            dataType: "json",
            url: window.location.protocol + "//" + window.location.host + "/api/ShipType/" + shipTypeId,
            success: function (shipTypeData) {
                shipTypeName = shipTypeData.ship_Type;
            },
            error: function (error) {
                console.log(error);
            }
        });
        return shipTypeName;
    }

    return null;
};

function loadShotsData(player) {
    console.log("Loading shots for: " + player);
    var board;
    if (player == "opponent") {
        board = "p2";
    } else {
        board = "p1";
    }
    $.ajax({
        type: "GET",
        cache: false,
        async: true,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/Shot/all-by-board/" + $("#"+player+"-board-id").val(),
        success: function (shotData) {
            if (shotData.length > 0) {
                console.log("Shot data: " + shotData);
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

                    lastShot = shot.shot_Id;
                }
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
};

///////////////////////Dragging code/////////////////////////
var drag = {
    //the problem of dragging....
    ship: null,                 //hold the g element I'm moving
    shipId: '',
    shipX: '',					//hold the position of the last position the first ShipPiece was in.
    shipY: '',					//hold the position of the last position the first ShipPiece was in.
    shipLength: '',
    shipPiecesCount: '',
    g: null,

    //  set the id of the thing I'm moving...
    setMove: function (type) {
        switch(type) {
            case "Carrier":
                console.log("Carrier selected for drag.");
                drag.ship = document.getElementById("g_Carrier");
                drag.shipId = "g_Carrier";
                drag.g = game.carrier;
                break;

            case "Battleship":
                console.log("Battleship selected for drag.");
                drag.ship = document.getElementById("g_Battleship");
                drag.shipId = "g_Battleship";
                drag.g = game.battleship;
                break;

            case "Submarine":
                console.log("Submarine selected for drag.");
                drag.ship = document.getElementById("g_Submarine");
                drag.shipId = "g_Submarine";
                drag.g = game.submarine;
                break;

            case "Cruiser":
                console.log("Cruiser selected for drag.");
                drag.ship = document.getElementById("g_Cruiser");
                drag.shipId = "g_Cruiser";
                drag.g = game.cruiser;
                break;

            case "Destroyer":
                console.log("Destroyer selected for drag.");
                drag.ship = document.getElementById("g_Destroyer");
                drag.shipId = "g_Destroyer";
                drag.g = game.destroyer;
                break;
        }

        console.log("Drag variables:");
        console.log("Drag ship: " + drag.ship);
        console.log("Drag shipId: " + drag.shipId);
        console.log("Drag g: " + drag.g);

        //get the last position of the thing... (NOW through the transform=translate(x,y))
        xy = util.getTransform(drag.shipId);
        drag.shipX = xy[0];
        drag.shipY = xy[1];
        drag.shipLength = drag.ship.getAttribute("width");
        drag.shipPiecesCount = drag.shipLength / 50;

        console.log("Drag shipX: " + drag.shipX);
        console.log("Drag shipY: " + drag.shipY);
        console.log("Drag shipLength: " + drag.shipLength);
        console.log("Drag shipPiecesCount: " + drag.shipPiecesCount);

        //get the object then re-append it to the document so it is on top!
        drag.g.putOnTop();
    },

    ////releaseMove/////
    //	clear the id of the thing I'm moving...
    ////////////////
    releaseMove: function (evt) {
        if (drag.mover != '') {
            //is it YOUR turn?
            //if (turn == playerId) {
            //    var hit = drag.checkHit(evt.layerX, evt.layerY, drag.mover);
            //} else {
            //    var hit = false;
            //    util.nytwarning();
            //}
            //if (hit == true) {
            //    //I'm on the square...
            //    //send the move to the server!!!
            //} else {
            //    //move back
            //    util.setTransform(drag.mover, drag.myX, drag.myY)
            //}
            drag.ship = null;
            drag.g = null;
        }
    },

    ////go/////
    //	move the thing I'm moving...
    ////////////////
    go: function (evt) {
        if (drag.ship != null) {
            util.setTransform(drag.shipId, evt.layerX, evt.layerY);
        }
    },

    ////checkHit/////
    //	did I land on anything important...
    ////////////////
    checkHit: function (x, y, id) {
        //lets change the x and y coords (mouse) to match the transform
        x = x - game.BOARDX;
        y = y - game.BOARDY;
        //go through ALL of the board
        for (i = 0; i < game.BOARDWIDTH; i++) {
            for (j = 0; j < game.BOARDHEIGHT; j++) {
                var drop = game.boardArr[i][j].myBBox;
                //document.getElementById('output2').firstChild.nodeValue+=x +":"+drop.x+"|";
                if (x > drop.x && x < (drop.x + drop.width) && y > drop.y && y < (drop.y + drop.height) && game.boardArr[i][j].droppable && game.boardArr[i][j].occupied == '') {
                    //NEED - check is it a legal move???
                    //if it is - then
                    //put me to the center....
                    util.setTransform(id, game.boardArr[i][j].getCenterX(), game.boardArr[i][j].getCenterY());
                    //fill the new cell
                    //alert(parseInt(which.substring((which.search(/\|/)+1),which.length)));
                    util.getPiece(id).changeCell(game.boardArr[i][j].id, i, j);
                    //////////////////
                    //change the board in the database for the other person to know




                    //change who's turn it is
                    util.changeTurn();
                    return true;
                }
            }
        }
        return false;
    }
}