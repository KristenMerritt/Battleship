/*
 * Contains scripts used for chat in Battleship.
 * @author Kristen Merritt
 */

$(document).ready(function () {
    initializeUserHandle(); // Change the User Handle to reflect who is logged in based off of the token
    initializeSendMessageEvent(); // Add the send message event to the chat form
    getAllChat(); // Retrieve all of the chat data

    window.setInterval(function () {
        getMostRecentChat();
    }, 1000);
});

/*
 * Checks the cookie/token for the user ID and sets the
 * user handle on the page accordinly.
 */
function initializeUserHandle() {
    //console.log("Initializing user handle...");
    var cookie = getTokenFromCookie();
    if (cookie !== "" && cookie !== null) {
        getUserHandleFromToken(cookie);       
    }
};

/*
 * Once the document loads, sets an onsubmit listener to the
 * form used to send messages to the chat.
 * Prevents the form's default action, checks the token,
 * and sends the message information to the server for processing.
 */
function initializeSendMessageEvent() {
    //console.log("Initializing send message event...");
    var cookie = getTokenFromCookie(); // Get the token from the cookie
    $("#chat-form").on("submit",
        function (e) {
            e.preventDefault(); // Prevent the default action of the form
            if (cookie !== "" && cookie !== null) { // Check to make sure the cookie has a value
                if (validToken(cookie)) { // Check to make sure the cookie is valid before sending anything
                    var message = $("input[name='chat-message']").val(); 

                    var chatMessageInfo = { // represents a ChatMessageInfo model
                        Message: message,
                        Token: cookie
                    };

                    $.ajax({
                        type: "POST",
                        cache: true,
                        async: true,
                        dataType: "json",
                        url: window.location.protocol + "//" + window.location.host + "/api/Chat",
                        data: chatMessageInfo,
                        success: function (data) {
                            console.log("Successfully sent chat to server.");
                        },
                        error: function (error) {
                            console.log(error);
                        }
                    });
                } else {
                    alert("Invalid user login - please log in again.");
                    // Log the user out?
                }
            }
        });
};

/*
 * Retrieves all of the chat data from the server.
 * For each chat, calls insertNewChat();
 */
function getAllChat() {
    //console.log("Getting all chat...");
    $.ajax({
        type: "GET",
        cache: false,
        async: true,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/Chat",
        success: function (chatData) {
            $.each(chatData,              
                function (i, val) {
                    insertNewChat(val);
                });
        },
        error: function (error) {
            console.log(error);
        }
    });
};

/*
 * Retrieves all chat after a specified ID.
 * Calls insertNewChat() for every chat recieved.
 */
function getMostRecentChat() {
    //console.log("Getting new chat...");
    var id = $("#last-chat-id").val();    
    $.ajax({
        type: "GET",
        cache: false,
        async: true,
        dataType: "json",
        url: window.location.protocol + "//" + window.location.host + "/api/Chat/"+id,
        success: function (chatData) {
            $.each(chatData,
                function (i, val) {
                    insertNewChat(val);
                });
        },
        error: function (error) {
            console.log(error);
        }
    });
};

/*
 * Inserts new chat into the document
 * PARAM: json val
 */
function insertNewChat(val) {
    console.log("Inserting new chat...");
    var div = document.createElement("div");
    div.setAttribute("class", "chat-item");

    var para = document.createElement("p");
    para.setAttribute("class", "chat-text");
    var message = document.createTextNode(val.message);

    var span = document.createElement("span");
    span.setAttribute("class", "chat-name");

    var handle = getUserHandle(val.player_Id);
    var handleEl = document.createTextNode(handle + ": ");
    span.appendChild(handleEl);

    para.appendChild(span);
    para.appendChild(message);
    div.appendChild(para);
    $("#chat-container").append(div);
    $("#last-chat-id").val(val.chat_Id);               
};