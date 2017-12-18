//////////////////////////////////////////////////////////////////
//                                                              //
//  Util Javascript File		        	    			    //
//  Description:  This file contains various scripts used       //
//                for the game meant as utility functions       //
//                                                              //
//////////////////////////////////////////////////////////////////

//	use - util.getPiece(), util.getTransform(), etc...
var util={
	getPiece:function(id){
		return game.pieceArr[parseInt(id.substr((id.search(/\_/)+1),1))][parseInt(id.substring((id.search(/\|/)+1),id.length))];
	},
	getTransform:function(id){
		var hold=document.getElementById(id).getAttributeNS(null,'transform');
		var retVal=new Array();
		retVal[0]=hold.substring((hold.search(/\(/) + 1),hold.search(/,/));			//x value
		retVal[1]=hold.substring((hold.search(/,/) + 1),hold.search(/\)/));;		//y value
		return retVal;
	},
    setTransform: function (id, x, y) {
		document.getElementById(id).setAttributeNS(null,'transform','translate('+x+','+y+')');
	},
	changeTurn:function(){
		//turn=Math.abs(turn-1);
		//document.getElementById('output2').firstChild.data='playerId '+playerId+ ' turn '+turn;
		//ajax.changeServerTurnAjax('changeTurn',gameId);
	},
	nytwarning:function(){
		//if(document.getElementById('nyt').getAttributeNS(null,'display') == 'none'){
		//	document.getElementById('nyt').setAttributeNS(null,'display','inline');
		//	setTimeout(util.nytwarning,2000);
		//}else{
		//	document.getElementById('nyt').setAttributeNS(null,'display','none');
		//}
	},
	nypwarning:function(){
		//if(document.getElementById('nyp').getAttributeNS(null,'display') == 'none'){
		//	document.getElementById('nyp').setAttributeNS(null,'display','inline');
		//	setTimeout(util.nypwarning,2000);
		//}else{
		//	document.getElementById('nyp').setAttributeNS(null,'display','none');
		//}
	}
}