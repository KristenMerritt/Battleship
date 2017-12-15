var game = {
    xhtmlns: "http://www.w3.org/1999/xhtml",
    svgns: "http://www.w3.org/2000/svg",
    BOARDX: 0, //starting pos of board
    BOARDY: 0,
    BOARDWIDTH: 10, // how many squares across
    BOARDHEIGHT: 10, // how many squares down
    CELLSIZE: 50, // size of cells

    board1Arr: new Array(), // 2d array [row][col] for the board for player 1
    board2Arr: new Array(), // 2d array [row][col] for the board for player 1

    hole1Arr: new Array(), // 2d array [row][col] of holes for player 1
    hole2Arr: new Array(), // 2d array [row][col] of holes for player 2

    cruiser: null,
    battleship: null,
    destroyer: null,
    submarine: null,
    carrier: null,

    carrierPieces: new Array(),
    battleshipPieces: new Array(),
    submarinePieces: new Array(),
    cruiserPieces: new Array(),
    destroyerPieces: new Array(),

    init: function() {
        var times = 1;
        var svgs = document.getElementsByTagName("svg");
        var gameId = $("#gameId").val();

        //create a parent to stick board in...
        while (times < 3) {
            console.log(times);
            var gEle = document.createElementNS(game.svgns, 'g');
            gEle.setAttributeNS(null, 'transform', 'translate(' + game.BOARDX + ',' + game.BOARDY + ')');
            gEle.setAttributeNS(null, 'id', 'gId_' + gameId + '_p' + times);
            gEle.setAttributeNS(null, 'fill', '#b0b8c4');

            //stick g on board
            svgs[times - 1].insertBefore(gEle, svgs[times - 1].childNodes[5]);
            times += 1;
        }

        //create the board...
        //var x = new Cell(document.getElementById('someIDsetByTheServer'),'cell_00',CELLSIZE,0,0);       
        for (var i = 0; i < game.BOARDWIDTH; i++) {
            game.board1Arr[i] = new Array();
            game.hole1Arr[i] = new Array();
            for (j = 0; j < game.BOARDHEIGHT; j++) {
                game.board1Arr[i][j] = new Cell(document.getElementById("gId_" + gameId + "_p1"),
                    'cell_' + j + i + '_p1',
                    game.CELLSIZE,
                    j,
                    i);
                game.hole1Arr[i][j] = new Hole(document.getElementById("gId_" + gameId + "_p1"),
                    'hole_' + j + i + '_p1',
                    20,
                    j,
                    i,
                    (j * 50) + 25,
                    (i * 50) + 25);
            }
        }

        for (var i = 0; i < game.BOARDWIDTH; i++) {
            game.board2Arr[i] = new Array();
            game.hole2Arr[i] = new Array();
            for (j = 0; j < game.BOARDHEIGHT; j++) {
                game.board2Arr[i][j] = new Cell(document.getElementById('gId_' + gameId + '_p2'),
                    'cell_' + j + i + '_p2',
                    game.CELLSIZE,
                    j,
                    i);
                game.hole2Arr[i][j] = new Hole(document.getElementById('gId_' + gameId + '_p2'),
                    'hole_' + j + i + '_p2',
                    20,
                    j,
                    i,
                    (j * 50) + 25,
                    (i * 50) + 25);
            }
        }

        loadShips();
        loadShotsData("currentPlayer");
        loadShotsData("opponent");

        //put the drop code on the document...
        //document.getElementsByTagName('svg')[0].addEventListener('mouseup', drag.releaseMove, false);

        //put the go() method on the svg doc.
        //document.getElementsByTagName('svg')[0].addEventListener('mousemove', drag.go, false);

        //put the player in the text
        //document.getElementById('youPlayer').firstChild.data += player;
        //document.getElementById('opponentPlayer').firstChild.data += player2;
    }
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
                            carrierCount++;
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
    $.ajax({
        type: "GET",
        cache: false,
        async: true,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/Shot/all-by-board/" + $("#player-"+player+"-board-id").val(),
        success: function (shotData) {
            if (shotData.length > 0) {
                console.log("Shot data: " + shotData);
                for (var x = 0; x < shotData.length; x++) {
                    var shot = shotData[x];
                    var hole = document.getElementById("hole_" + player + "_" + shot.row + shot.col);

                    if (shot.is_Hit) {
                        hole.setAttributeNS(null, 'occupied', 'hit');
                        hole.setAttributeNS(null, 'class', 'hole_hit');
                        hole.setAttributeNS(null, 'fill', 'red');
                    } else {
                        hole.setAttributeNS(null, 'occupied', 'miss');
                        hole.setAttributeNS(null, 'class', 'hole_miss');
                        hole.setAttributeNS(null, 'fill', 'white');
                    }
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
    myX: '',						//hold my last pos.
    myY: '',					//hold my last pos.
    mover: '',					//hold the id of the thing I'm moving
    ////setMove/////
    //	set the id of the thing I'm moving...
    ////////////////
    setMove: function (which) {
        drag.mover = which;
        //get the last position of the thing... (NOW through the transform=translate(x,y))
        xy = util.getTransform(which);

        drag.myX = xy[0];
        drag.myY = xy[1];
        //get the object then re-append it to the document so it is on top!
        util.getPiece(which).putOnTop(which);
    },

    ////releaseMove/////
    //	clear the id of the thing I'm moving...
    ////////////////
    releaseMove: function (evt) {
        if (drag.mover != '') {
            //is it YOUR turn?
            if (turn == playerId) {
                var hit = drag.checkHit(evt.layerX, evt.layerY, drag.mover);
            } else {
                var hit = false;
                util.nytwarning();
            }
            if (hit == true) {
                //I'm on the square...
                //send the move to the server!!!
            } else {
                //move back
                util.setTransform(drag.mover, drag.myX, drag.myY)
            }
            drag.mover = '';
        }
    },

    ////go/////
    //	move the thing I'm moving...
    ////////////////
    go: function (evt) {
        if (drag.mover != '') {
            util.setTransform(drag.mover, evt.layerX, evt.layerY);
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