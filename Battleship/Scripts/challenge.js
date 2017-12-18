//////////////////////////////////////////////////////////////////
//                                                              //
//  Challenge Javascript File		        				    //
//  Description:  This file contains various scripts used       //
//                for the challenge system.                     //
//                                                              //
//////////////////////////////////////////////////////////////////

/**
 * Ensures that there is a valid token before attemping to retrieve
 * challenge data.
 * Once validated, collects all pending challenges for the current user
 * who is logged in.
 * Once the initial challenges are loaded in, continues to look for new ones.
 */
$(document).ready(function () {
    initializeChallengeEvent();

    var userId = -1;
    var cookie = getTokenFromCookie();
    if (cookie !== "" && cookie !== null) {
        if (validToken(cookie)) { 
            userId = getUserIdFromToken(cookie);
            // Only check for challenges if the token is set and valid
            checkForChallenges(userId, cookie); 
        }
    }
    if (userId !== -1) { // the user ID would be set to something different if there was a valid token
        window.setInterval(function () {
            checkForNewChallenges(userId, cookie); // Check for new challenges every 2 seconds
        }, 2000);
    }    
});

/**
 * Sets the function on the challenge-request button
 * that will send a new challenge to a player
 */
function initializeChallengeEvent() {
    $("#challenge-request").on("submit",
        function (e) {
            e.preventDefault();
            var cookie = getTokenFromCookie();
            if (cookie !== "" && cookie !== null) {
                if (validToken(cookie)) {
                    // Get the Ids of the players involved in the challenge
                    var player2Handle = $("#challenge-handle").val();
                    var player1Id = getUserIdFromToken(cookie);
                    var player2Id = getUserIdFromHandle(player2Handle, cookie);

                    // Send the challenge to the DB
                    if (player2Id !== -1) {
                        ajax("POST", true, "api/Challenge/new/" + player1Id + "/" + player2Id + "/" + cookie, null, function(challengeData) {
                            if (challengeData.errMsg != null) {
                                sendErrorMessage(challengeData);
                            } else {
                                alert("Challenge sent!");
                            }
                        });
                        
                    }
                } 
            }
        }
    );
};

/**
 * Checks for challenges for the specified user.
 * Inserts any challenges found.
 * @param int userId
 * @param string cookie
 */
function checkForChallenges(userId, cookie) {
    ajax("GET", true, "api/Challenge/" + userId + "/" + cookie, null, function(challengeData) {
        if (challengeData.errMsg != null) {
            sendErrorMessage(challengeData);
        } else {
            $.each(challengeData,function (i, val) {
                insertNewChallenge(val);
            });        
        }
    });    
};

/**
 * Checks for new challenges for the specified user.
 * Inserts any challenges found.
 * @param int userId
 * @param string cookie
 */
function checkForNewChallenges(userId, cookie) {
    console.log("Checking for new challenges...");
    var challengeId = $("#last-challenge-id").val();
    ajax("GET", true, "api/Challenge/checkNew/" + userId + "/" + challengeId + "/" + cookie, null, function(challengeData) {
        if (challengeData.errMsg != null) {
            sendErrorMessage(challengeData);
        } else {
            $.each(challengeData, function (i, val) {
                insertNewChallenge(val);
            });       
        }
    });    
};

/**
 * Inserts new challenge into the HTML.
 * @param {any} val
 */
function insertNewChallenge(val) {
    // Create the div to hold the challenge
    var challenge = document.createElement("div");
    challenge.setAttribute("class", "challenge");
    challenge.setAttribute("id", "challenge_" + val.challenge_Id);

    var challengeInfo = document.createElement("div");
    challengeInfo.setAttribute("class", "challenge-info");

    // Create the p element to hold the challenger names
    var challengerName = document.createElement("p");
    challengerName.setAttribute("class", "challenger-name");

    // Get the actual names of the players involved in the challenge
    var player1Id = val.player_1;
    var player2Id = val.player_2;
    var player1Handle = getUserHandle(player1Id);
    var player2Handle = getUserHandle(player2Id);
    var handleTextNode = document.createTextNode(player1Handle + " vs " + player2Handle);

    // Append the elements
    challengerName.appendChild(handleTextNode);
    challengeInfo.appendChild(challengerName);

    // Create buttons if accepted or not
    if (val.accepted) {
        var viewChallenge = document.createElement("div");
        viewChallenge.setAttribute("class", "challenge-accept-button");
        viewChallenge.setAttribute("onclick", "window.location.href='/game?p1=" + player1Id + "&p2=" + player2Id + "'");

        var viewGame = document.createTextNode("View Game");

        viewChallenge.appendChild(viewGame);
        challenge.appendChild(challengeInfo);
        challenge.appendChild(viewChallenge);
    } else {
        var challengeAccept = document.createElement("div");
        challengeAccept.setAttribute("class", "challenge-accept-button");

        var accept = document.createTextNode("Accept");

        challengeAccept.appendChild(accept);
        challengeAccept.setAttribute("onclick", "respondToChallenge(" + val.challenge_Id + ",true)");

        var challengeDecline = document.createElement("div");
        challengeDecline.setAttribute("class", "challenge-decline-button");

        var decline = document.createTextNode("Decline");

        challengeDecline.appendChild(decline);
        challengeDecline.setAttribute("onclick", "respondToChallenge(" + val.challenge_Id + ",false)");

        challenge.appendChild(challengeInfo);
        challenge.appendChild(challengeAccept);
        challenge.appendChild(challengeDecline);
    }
 
    $("#challenge-container").append(challenge);
    $("#last-challenge-id").val(val.challenge_Id);
};

/**
 * Responds to a challenge in the challenge box.
 * @param int challengeId
 * @param bool status
 */
function respondToChallenge(challengeId, status) {
    var cookie = getTokenFromCookie();
    if (cookie !== "" && cookie !== null) {
        if (validToken(cookie)) {
            ajax("POST", true, "api/Challenge/set-status/" + challengeId + "/" + status + "/" + cookie, null,function(challengeData) {
                    if (challengeData.errMsg != null) {
                        sendErrorMessage(challengeData);
                    } else {
                        if (challengeData.accepted) {
                            // If the challenge was accepted, make the starter game components
                            makeGameComponents(challengeData, cookie);

                            // Remove the old challenge from the challenge section
                            $("#challenge_" + challengeData.challenge_Id).remove();

                            // Create the new challenge that has been accepted in the challenge section
                            insertNewChallenge(challengeData);
                        } else {
                            // if the challenge was declined, just remove it from the challenge section
                            $("#challenge_" + challengeData.challenge_Id).remove();
                        }
                    }
                }
            );
        }
    }
};