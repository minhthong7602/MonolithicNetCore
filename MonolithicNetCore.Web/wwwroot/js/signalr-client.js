"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/hubserver").build();

connection.on("ReceiveMessage", function (message) {
    console.log('message', message);
});

connection.start().then(function () {
    console.log("Connection hub service");
}).catch(function (err) {
    return console.error(err.toString());
});
