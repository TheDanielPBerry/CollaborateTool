

var LastCheckIn = new Date();

$(document).ready(function () {
    $('.ChatSection').scrollTop(50000);
    CheckForMessages();
});

function CheckForMessages() {
    $.get("/Social/RefreshMessages", { time: LastCheckIn.toISOString() }, function (data) {
        if (data === 'True') {
            LastCheckIn = new Date();
            RefreshChatBar();
        }
    });
    setTimeout(CheckForMessages, 2000);
}



function RefreshChatBar() {
    var openArray = [];
    var ChatWindows = $('.ChatBar .ChatWindow');
    for (var i = 0; i < ChatWindows.length; i++) {
        openArray[openArray.length] = $(ChatWindows[i]).css('margin-top');
    }
    $.get("/Social/ChatBar", null, function (data) {
        $('#ChatBarContainer').html(data);

        var ChatWindows = $('.ChatBar .ChatWindow');
        for (var i = 0; i < ChatWindows.length; i++) {
            $(ChatWindows[i]).css('margin-top', openArray[i]);
            $(ChatWindows[i]).children('.ChatSection').scrollTop(50000);
        }
    });
}

function PostMessage() {
    RefreshChatBar();
}


function ToggleConversation(element) {
    var height = $(element.parentNode).css('margin-top');
    var newMargin = (height).substring(0, height.indexOf('px')) ^ (-400);
    $(element.parentNode).animate({ 'margin-top':newMargin });
}

function OpenChat(chatID) {
    $.post("/Social/ChatView", { id: chatID }, function (data) {
        $('.ChatBar').append(data);
    });
}

function CloseChat(chatId, element) {
    $.post("/Social/CloseChat", { id: chatId }, function () {
        $(element.parentNode.parentNode).remove();
    });
}

