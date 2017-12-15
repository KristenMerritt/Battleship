﻿/*
 * Contains scripts used for challenge functionality in Battleship.
 * @author Kristen Merritt
 */

/*
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
        if (validToken(cookie)) { // only look for challenges if the token is valid
            userId = getUserIdFromToken(cookie);
            checkForChallenges(userId, cookie);
        }
    }
    if (userId !== -1) { // the user ID would be set to something different if there was a valid token
        window.setInterval(function () {
            checkForNewChallenges(userId, cookie);
        }, 2000);
    }    
});

function initializeChallengeEvent() {
    $("#challenge-request").on("submit",
        function (e) {
            e.preventDefault();
            var cookie = getTokenFromCookie();
            if (cookie !== "" && cookie !== null) {
                if (validToken(cookie)) {
                    var player2Handle = $("#challenge-handle").val();
                    var player1Id = getUserIdFromToken(cookie);
                    var player2Id = getUserIdFromHandle(player2Handle, cookie);

                    if (player2Id !== -1) {
                        $.ajax({
                            type: "POST",
                            cache: false,
                            async: true,
                            dataType: "json",
                            url: window.location.protocol + "//" + window.location.host + "/api/Challenge/new/" + player1Id + "/" + player2Id + "/" + cookie,
                            success: function (challengeData) {
                                if (challengeData.errMsg != null) {
                                    sendErrorMessage(challengeData);
                                } else {
                                    alert("Challenge sent!");
                                }
                            },
                            error: function (error) {
                                alert("Error.");
                                console.log(error);
                            }
                        });
                    }
                }
            }
        }
    );
};

/*
 * Checks for challenges for the specified user.
 * Inserts any challenges found.
 * PARAM: int userId
 */
function checkForChallenges(userId, cookie) {
    //console.log("Checking for pending or accepted challenges for " + userId);
    $.ajax({
        type: "GET",
        cache: false,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/Challenge/"+userId+"/"+cookie,
        success: function (challengeData) {
            if (challengeData.errMsg != null) {
                sendErrorMessage(challengeData);
            } else {
                $.each(challengeData,
                    function (i, val) {
                        insertNewChallenge(val);
                    });
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
};

/*
 * Checks for new challenges for the specified user.
 * Inserts any challenges found.
 * PARAM: int userId
 */
function checkForNewChallenges(userId, cookie) {
    //console.log("Checking for new challenges...");
    var challengeId = $("#last-challenge-id").val();
    $.ajax({
        type: "GET",
        cache: false,
        async: true,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/Challenge/checkNew/"+userId+"/"+challengeId+"/"+cookie,
        success: function (challengeData) {
            if (challengeData.errMsg != null) {
                sendErrorMessage(challengeData);
            } else {
                $.each(challengeData,
                    function(i, val) {
                        insertNewChallenge(val);
                    });
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
};

/*
 * Inserts new challenge into the document.
 * PARAM: json val
 */
function insertNewChallenge(val) {
    console.log("Inserting new challenge...");

    var challenge = document.createElement("div");
    challenge.setAttribute("class", "challenge");
    challenge.setAttribute("id", "challenge_" + val.challenge_Id);

    var challengeInfo = document.createElement("div");
    challengeInfo.setAttribute("class", "challenge-info");

    var challengerName = document.createElement("p");
    challengerName.setAttribute("class", "challenger-name");
    var player1Id = val.player_1;
    var player2Id = val.player_2;
    var player1Handle = getUserHandle(player1Id);
    var player2Handle = getUserHandle(player2Id);
    var handleTextNode = document.createTextNode(player1Handle+" vs "+player2Handle);
    challengerName.appendChild(handleTextNode);

    challengeInfo.appendChild(challengerName);

    if (val.accepted) {
        var viewChallenge = document.createElement("div");
        viewChallenge.setAttribute("class", "challenge-accept-button");
        viewChallenge.setAttribute("onclick", "window.location.href='/game?p1=" + player1Id + "&p2=" + player2Id+"'");
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

function respondToChallenge(challengeId, status) {
    var cookie = getTokenFromCookie();
    if (cookie !== "" && cookie !== null) {
        if (validToken(cookie)) { // only look for challenges if the token is valid
            $.ajax({
                type: "POST",
                cache: false,
                async: true,
                dataType: "json",
                url: window.location.protocol+"//"+window.location.host+"/api/Challenge/set-status/"+challengeId+"/"+status+"/"+cookie,
                success: function (challengeData) {
                    if (challengeData.errMsg != null) {
                        sendErrorMessage(challengeData);
                    } else {
                        if (challengeData.accepted) {
                            console.log("Challenge has been accepted.");
                            makeGameComponents(challengeData, cookie);
                            $("#challenge_" + challengeData.challenge_Id).remove();
                            insertNewChallenge(challengeData);
                        } else {
                            $("#challenge_" + challengeData.challenge_Id).remove();
                        }
                    }                  
                },
                error: function (error) {
                    console.log(error);
                }
            });
        }
    }
};

function getSpecificChallenge(challengeId, cookie) {
    var challenge;
    $.ajax({
        type: "GET",
        cache: false,
        async: true,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/Challenge/get-challenge/"+challengeId+"/"+cookie,
        success: function (challengeData) {
            if (challengeData.errMsg != null) {
                sendErrorMessage(challengeData);
            } else {
                challenge = challengeData;
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
    return challenge;
};