"use strict";

var connectionLog = new signalR.HubConnectionBuilder().withUrl("/hublog").build();

connectionLog.on("AdminLog", function (message) {
    console.log('AdminLog', message);
});

connectionLog.start().then(function () {
    console.log("Connection hub service");
}).catch(function (err) {
    return console.error(err.toString());
});

function subscribeAdminLog () {
    connectionLog.invoke("SubscribeAdmin");
}