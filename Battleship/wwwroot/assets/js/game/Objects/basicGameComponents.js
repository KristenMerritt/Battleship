//////////////////////////////////////////////////////
// Class: Cell										//
// Description:  This will create a cell object		// 
// (board square) that you can reference from the 	//
// game. 											//
// Arguments:										//
//      parent - the svg g element (group)          //
//		id     - the cell's id                      //
//		size   - the object's width & height	    //
//		row    - the row it is located on           //
//		col    - the col it is located on           //
//////////////////////////////////////////////////////


// Cell constructor()
function Cell(parent, id, size, row, col) {
    // Basic variables for the cell
    this.parent = parent;
    this.id = id;
    this.size = size;
    this.row = row;
    this.col = col;

    // Initialize other instance vars
    this.shipPiece = ''; //hold the id of the piece
    this.y = this.size * this.row;
    this.x = this.size * this.col;
    this.droppable = true;

    // Create the cell
    this.object = this.create();
    this.parent.appendChild(this.object);
    this.myBBox = this.getMyBBox();
}


//////////////////////////////////////////////////////
// Cell : Methods									//
// Description:  All of the methods for the			// 
// Cell Class                               		//
//////////////////////////////////////////////////////
Cell.prototype = {
    create: function () {
        var rectEle = document.createElementNS(game.svgns, 'rect'); // Create the svg rect that will represent the cell
        rectEle.setAttributeNS(null, 'x', this.x + 'px');           // Set the x and y variables of the svg element
        rectEle.setAttributeNS(null, 'y', this.y + 'px');
        rectEle.setAttributeNS(null, 'width', this.size + 'px');    // Set the width and height of the svg element
        rectEle.setAttributeNS(null, 'height', this.size + 'px');
        rectEle.setAttributeNS(null, 'id', this.id);                // Set the ID of the svg element
        rectEle.setAttributeNS(null, 'stroke', '#8d9096');          // Set the stroke of the svg element
        rectEle.onclick = function () { alert(this.id); };          // Create an alert for testing purposes
        return rectEle;
    },
    //get my bbox
    getMyBBox: function () {
        return this.object.getBBox();
    },
    //get CenterX
    getCenterX: function () {
        return (game.BOARDX + this.x + (this.size / 2));
    },
    //get CenterY
    getCenterY: function () {
        return (game.BOARDY + this.y + (this.size / 2));
    },
    //set a cell to be occupied by a ShipPiece
    placeShipPiece: function (pieceId) {
        this.shipPiece = pieceId;
    },
    //set cell to empty
    removeShipPiece: function () {
        this.shipPiece = '';
    }
}



//////////////////////////////////////////////////////
// Class: Hole										//
// Description:  This will create a hole object     //
// that represents all of the shots made            //
// Arguments:										//
//      parent - the board                          //
//		id     - the holes's id                     //
//		size   - the object's width & height	    //
//		row    - the row it is located on           //
//		col    - the col it is located on           //
//      cx     - the svg circle's cx                //
//      cy     - the svg circle's cy                //
//////////////////////////////////////////////////////

// Hole constructor()
function Hole(parent, id, size, row, col, cx, cy) {
    // Basic variables for the Hole
    this.parent = parent;       
    this.id = id;               
    this.size = size;
    this.row = row;
    this.col = col;
    this.cy = cx;
    this.cx = cy;

    // Initialize other instance vars
    this.occupied = 'empty'; 
    this.hit = false;         // true = red peg present

    // Create the Hole object
    this.object = this.create();
    this.parent.appendChild(this.object);
    this.myBBox = this.getMyBBox();
}


//////////////////////////////////////////////////////
// Hole : Methods									//
// Description:  All of the methods for the			// 
// Hole Class                               		//
//////////////////////////////////////////////////////
Hole.prototype = {
    create: function () {
        var circleEle = document.createElementNS(game.svgns, 'circle');    // Create the circle svg element
        circleEle.setAttributeNS(null, 'cx', this.cx + 'px');              // Set the cx and cy values for the circle
        circleEle.setAttributeNS(null, 'cy', this.cy + 'px');
        circleEle.setAttributeNS(null, 'r', 10);                           // Set the radius value for the circle
        circleEle.setAttributeNS(null, 'width', this.size + 'px');         // Set the width and height of the circle
        circleEle.setAttributeNS(null, 'height', this.size + 'px');
        circleEle.setAttributeNS(null, 'class', 'hole_' + this.occupied);  // Set the class, which determines the color of the hole
        circleEle.setAttributeNS(null, 'id', this.id);                     // Set the ID of the circle
        circleEle.setAttributeNS(null, 'stroke', 'black');                 // Set the stroke of the circle
        circleEle.setAttribute('row', this.row);                           // Set the row and col of the circle/hole
        circleEle.setAttribute('col', this.col);

        // When a user clicks on a hole, it will create a shot
        circleEle.onclick = function () {
            Hole.prototype.shoot(this); 
        };

        return circleEle;
    },
    //get my bbox
    getMyBBox: function () {
        return this.object.getBBox();
    },
    //get CenterX
    getCenterX: function () {
        return (game.BOARDX + this.x + (this.size / 2));
    },
    //get CenterY
    getCenterY: function () {
        return (game.BOARDY + this.y + (this.size / 2));
    },
    //set a cell to occupied
    shoot: function (hole) {      
        var holeEle = document.getElementById(hole.id);   // Retrieve the hole that was shot at & its row/col
        var holeRow = holeEle.getAttribute('row');
        var holeCol = holeEle.getAttribute('col');
        var hit = false;
        var err = false;

        //alert("Hole: " + hole.id + " Row: " + holeRow + " Col: " + holeCol);
        var split = hole.id.split("_");
        console.log(split);
        var playerBoard = split[2];

        if (game.turn == game.currentPlayerId) {
            if (playerBoard == "p2") {
                // Create a ShipLocation object with the data
                var shipLocation = {
                    Ship_Location_Id: -1,
                    Ship_Id: -1,
                    Board_Id: game.opponentPlayerBoardId,
                    Row: holeRow,
                    Col: holeCol
                };

                // Send the ShipLoation daa to make a shot at the location in the DB
                ajax("POST", false, "api/ShipLocation/checkHit", shipLocation, function(shotData) {
                    if (shotData.err != null) {
                        sendErrorMessage(shotData);
                    } else {
                        hit = shotData.is_Hit;
                        if (hit) {
                            Hole.prototype.hit(hole);
                        } else {
                            Hole.prototype.miss(hole);
                        }
                    }
                });                
            } else {
                alert("Wrong board.");
            }
        } else {
            alert("Not your turn.");
        }
                            
    },
    miss: function(hole) {
        console.log("Shot miss");
        var holeEle = document.getElementById(hole.id);
        holeEle.setAttributeNS(null, 'occupied', 'miss');
        holeEle.setAttributeNS(null, 'class', 'hole_miss');
        holeEle.setAttributeNS(null, 'fill', 'white');
    },
    hit: function (hole) {
        console.log("Shot hit");
        var holeEle = document.getElementById(hole.id);
        holeEle.setAttributeNS(null, 'occupied', 'peg');
        holeEle.setAttributeNS(null, 'class', 'hole_hit');
        holeEle.setAttributeNS(null, 'fill', 'red');
    }
}


//////////////////////////////////////////////////////
// Class: ShipPiece								    //
// Description:  This will create a piece object    // 
// that you can reference from the game.          	//
// Arguments:										//
//      parent - the svg g element (group)          //
//		id     - the cell's id                      //
//		size   - the object's width & height	    //
//		row    - the row it is located on           //
//		col    - the col it is located on           //
//////////////////////////////////////////////////////


// ShipPiece constructor
// creates and initializes each ShipPiece object
function ShipPiece(parent, row, col, num, isHit) {
    // Setting basic variables
    this.parent = parent;		// The g of the ship that the ShipPiece will be part of
    this.row = row; // not sure where i mixed these two up
    this.col = col;
    this.number = num;
    this.isHit = isHit;
    console.log("IS HIT IN ELEMENT: " + this.isHit);
    this.parentType = this.parent.getAttribute("type");
    this.id = this.parentType + "_piece_" + this.number;

    this.piece = document.createElementNS(game.svgns, "rect");
    this.piece.setAttributeNS(null, 'id', this.parentType + '_piece_' + this.number);
    this.piece.setAttributeNS(null, "transform", "translate(" + this.row * 50 + "," + this.col * 50+ ")");
    this.piece.setAttributeNS(null, "height", '50px');
    this.piece.setAttributeNS(null, 'width', '50px');
    if (this.isHit) {
        this.piece.setAttributeNS(null, 'fill', 'red');
    } else {
        this.piece.setAttributeNS(null, 'fill', 'white');
    }   
    this.piece.setAttribute("row", row);
    this.piece.setAttribute("col", col);

    this.piece.addEventListener('mousedown', function () {
        drag.setMove(this.parentElement.getAttribute("type"));
    }, false);	// add a mousedown event listener to your piece so that it can be dragged.

    document.getElementById('g_' + this.parentType).appendChild(this.piece);

    // return this piece object
    return this;
}

ShipPiece.prototype = {
    //change cell (used to move the piece to a new cell and clear the old)
    changeLocation: function (row, col) {
        //this.current_cell.notOccupied();
        //document.getElementById('test_output').firstChild.nodeValue = 'dropped cell: ' + newCell;
        //this.current_cell = game.boardArr[row][col];
        //this.current_cell.isOccupied(this.id);
    },
    //when called, will remove the piece from the document and then re-append it (put it on top!)
    putOnTop: function () {
        document.getElementsByTagName('svg')[0].removeChild(this.piece);
        document.getElementsByTagName('svg')[0].appendChild(this.piece);
    },
    //will record that I'm now hit
    hit: function () {
        this.isHit = true;
        this.setAttributeNS(null, 'fill', 'red');
    },
    // function that allows a quick setting of an attribute of the specific piece object
    setAtt: function (att, val) {
        this.piece.setAttributeNS(null, att, val);
    }
}