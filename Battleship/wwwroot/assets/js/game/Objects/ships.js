//////////////////////////////////////////////////////////////
// Class: Ship  								            //
// Description:  This will create a ship object             // 
// that you can reference from the game.        	        //
// Arguments:										        //
//      parent     - the svg g element (group)              //
//      type       - the type of ship it is                 //
//      shipPieces - the pieces associated with the ship    //
//      isPlaced   - determines whether or not the ship     //
//                      has been placed yet or not          //
//      isSunk     - determines whether or not the ship     //
//                      has sunk or not                     //
//////////////////////////////////////////////////////////////

// ShipPiece constructor
function Ship(parent, type, shipPieces, isPlaced, isSunk) {
    // Create basic variables
    this.parent = parent; 		
    this.type = type;			
    this.isPlaced = isPlaced;	
    this.isSunk = isSunk;       
    this.id = "g_" + this.type;	

    // Create the variables that will determine the location
    this.shipPieces = shipPieces;

    // Create the object
    this.object = new window[type](this);	// Carrier, Cruiser, Battleship, Destroyer, Submarine
    this.g = this.object.g;			        // a shortcut to the actual svg g object
    this.setAtt("id", this.id);				

    document.getElementsByTagName('svg')[0].appendChild(this.g);

    // return this piece object
    return this;
}

Ship.prototype = {
    //change cell (used to move the piece to a new cell and clear the old)
    changeLocation: function () {
        this.firstPiece = this.shipPieces[0];
        this.lastPiece = this.shipPieces[length];
        this.g.setAttributeNS(null, "transform", "translate(" + this.firstPiece.row + "," + this.firstPiece.col + ")");
    },
    //when called, will remove the piece from the document and then re-append it (put it on top!)
    putOnTop: function () {
        console.log("Putting on top");
        document.getElementsByTagName('svg')[0].removeChild(this.g);
        document.getElementsByTagName('svg')[0].appendChild(this.g);
    },
    //will record that I'm now hit
    initialPlace: function () {
        this.firstPiece = this.shipPieces[0];
        this.lastPiece = this.shipPieces[length];
        this.g.setAttributeNS(null, "transform", "translate(" + this.firstPiece.row + "," + this.firstPiece.col + ")");
    },
    sink: function (id) {
        this.g.setAttributeNS(null, "fill", "red");
    },
    // function that allows a quick setting of an attribute of the specific piece object
    setAtt: function (att, val) {
        this.g.setAttributeNS(null, att, val);
    }
}


function Carrier(parent) {
    this.parent = parent;		                        // the board
    this.g = document.createElementNS(game.svgns, "g");	// Each ship is just a g element

    this.shipPieces = this.parent.shipPieces;           // all of the ShipPieces associated with the ship
    this.length = 5;
    this.firstPiece = this.shipPieces[0];
    this.lastPiece = this.shipPieces[length];

    // setting attributes of the g element
    this.g.setAttributeNS(null, "id", "g_Carrier");
    this.g.setAttributeNS(null, "width", (this.length * 50) + 'px');
    this.g.setAttributeNS(null, "height", '50px');
    this.g.setAttributeNS(null, "stroke", "black");
    this.g.setAttribute("type", "Carrier");

    // return this object to be stored in a variable
    return this;
}

function Battleship(parent) {
    this.parent = parent;		
    this.g = document.createElementNS(game.svgns, "g");	

    this.shipPieces = this.parent.shipPieces;
    this.length = 4;
    this.firstPiece = this.shipPieces[0];
    this.lastPiece = this.shipPieces[length];

    // setting attributes of the g element
    this.g.setAttributeNS(null, "id", "g_Battleship");
    this.g.setAttributeNS(null, "width", (this.length * 50) + 'px');
    this.g.setAttributeNS(null, "height", '50px');
    this.g.setAttributeNS(null, "stroke", "black");
    this.g.setAttribute("type", "Battleship");

    // return this object to be stored in a variable
    return this;
}

function Submarine(parent) {
    this.parent = parent;		
    this.g = document.createElementNS(game.svgns, "g");	

    this.shipPieces = this.parent.shipPieces;
    this.length = 3;
    this.firstPiece = this.shipPieces[0];
    this.lastPiece = this.shipPieces[length];

    // setting attributes of the g element
    this.g.setAttributeNS(null, "id", "g_Submarine");
    this.g.setAttributeNS(null, "width", (this.length * 50) + 'px');
    this.g.setAttributeNS(null, "height", '50px');
    this.g.setAttributeNS(null, "stroke", "black");
    this.g.setAttribute("type", "Submarine");

    // return this object to be stored in a variable
    return this;
}

function Cruiser(parent) {
    this.parent = parent;		
    this.g = document.createElementNS(game.svgns, "g");

    this.shipPieces = this.parent.shipPieces;
    this.length = 3;
    this.firstPiece = this.shipPieces[0];
    this.lastPiece = this.shipPieces[length];

    // setting attributes of the g element
    this.g.setAttributeNS(null, "id", "g_Cruiser");
    this.g.setAttributeNS(null, "width", (this.length * 50) + 'px');
    this.g.setAttributeNS(null, "height", '50px');
    this.g.setAttributeNS(null, "stroke", "black");
    this.g.setAttribute("type", "Cruiser");

    // return this object to be stored in a variable
    return this;
}

function Destroyer(parent) {
    this.parent = parent;		
    this.g = document.createElementNS(game.svgns, "g");

    this.shipPieces = this.parent.shipPieces;
    this.length = 2;
    this.firstPiece = this.shipPieces[0];
    this.lastPiece = this.shipPieces[length];

    // setting attributes of the g element
    this.g.setAttributeNS(null, "id", "g_Destroyer");
    this.g.setAttributeNS(null, "width", (this.length * 50) + 'px');
    this.g.setAttributeNS(null, "height", '50px');
    this.g.setAttributeNS(null, "stroke", "black");
    this.g.setAttribute("type", "Destroyer");

    // return this object to be stored in a variable
    return this;
}