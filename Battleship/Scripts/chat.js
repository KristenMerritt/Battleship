//////////////////////////////////////////////////////////////////
//                                                              //
//  Chat Javascript File		        				        //
//  Description:  This file contains various scripts used       //
//                for chat in the application.                  //
//                                                              //
//////////////////////////////////////////////////////////////////

$(document).ready(function () {
    initializeUserHandle();         // Change the User Handle to reflect who is logged in based off of the token
    initializeSendMessageEvent();   // Add the send message event to the chat form
    getAllChat();                   // Retrieve all of the chat data

    window.setInterval(function () {// Get new chat every second
        getMostRecentChat();
    }, 1000);
});

/**
 * Checks the cookie/token for the user ID and sets the
 * user handle on the page accordinly.
 */
function initializeUserHandle() {
    var cookie = getTokenFromCookie();
    if (cookie !== "" && cookie !== null) {
        getUserHandleFromToken(cookie);       
    }
};

/**
 * Once the document loads, sets an onsubmit listener to the
 * form used to send messages to the chat.
 * Prevents the form's default action, checks the token,
 * and sends the message information to the server for processing.
 */
function initializeSendMessageEvent() {
    var cookie = getTokenFromCookie(); 

    $("#chat-form").on("submit",
        function (e) {
            e.preventDefault(); 
            if (cookie !== "" && cookie !== null) { 
                if (validToken(cookie)) { 
                    // Get the message the user wants to send
                    var message = $("input[name='chat-message']").val(); 

                    var chatMessageInfo = { 
                        Message: message,
                        Token: cookie
                    };

                    // Send the message to the database
                    ajax("POST", true, "api/Chat", chatMessageInfo, function(data) {
                        //console.log("Successfully sent chat to server.");
                    });
                } else {
                    alert("Invalid user login - please log in again.");
                }
            }
        });
};

/**
 * Retrieves all of the chat data from the server.
 * For each chat, calls insertNewChat();
 */
function getAllChat() {
    ajax("GET", true, "api/Chat", null, function(chatData) {
        $.each(chatData, function (i, val) {
            insertNewChat(val);
        });       
    });
};

/**
 * Retrieves all chat after a specified ID.
 * Calls insertNewChat() for every chat recieved.
 */
function getMostRecentChat() {
    var id = $("#last-chat-id").val();
    ajax("GET", true, "api/Chat/" + id, null, function(chatData) {
        $.each(chatData, function (i, val) {
            insertNewChat(val);
        });       
    });
};

/**
 * Inserts new chat into the HTML
 * @param json val
 */
function insertNewChat(val) {
    // Create the div element to contain the chat message
    var div = document.createElement("div");
    div.setAttribute("class", "chat-item");

    // Create the p element to hold the text
    var para = document.createElement("p");
    para.setAttribute("class", "chat-text");
    var message = document.createTextNode(val.message);

    // Create the span that will hold the handle
    var span = document.createElement("span");
    span.setAttribute("class", "chat-name");

    // Set the handle
    var handle = getUserHandle(val.player_Id);
    var handleEl = document.createTextNode(handle + ": ");
    span.appendChild(handleEl);

    // Append all of the elments
    para.appendChild(span);
    para.appendChild(message);
    div.appendChild(para);
    $("#chat-container").append(div);
    $("#last-chat-id").val(val.chat_Id);               
};