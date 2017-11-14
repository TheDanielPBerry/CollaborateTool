
$(document).ready(function () {
    $(document).mousemove(this, DragCallBack);
    $(document).mouseup(this, DropCard);
    $('.CardContainer').mousedown(this, PickUpCard);
});
var card = null;
var cardClone = null;
var cardOffset = {X:0, Y:0};
var selectedCardId = 0;
var selectedColumnId = 0;
var toggleColumns = true;

function OpenModal(uri, params) {
    $.get(uri, params,
        function (data) {
            $('.ModalContent').html(data);
            $('.modal-window').fadeIn('slow');
        }
    );
}

function CloseModal() {
    $('.modal-window').fadeOut('slow', function () {
        $('.ModalContent').html('');
    });
}

function DragCallBack(e) {
    if (card != null) {
        card.style.left = (e.pageX - cardOffset.X) + 'px';
        card.style.top = (e.pageY - cardOffset.Y) + 'px';
        var columns = $('.BoardColumnContainer');
        for (var i = 0; i < columns.length; i++) {
            //console.log((e.pageY >= $(columns[0]).offset().top) + ",  " + (e.pageY <= $(columns[0]).offset().top + columns[0].offsetHeight) + ",  " + (e.pageX >= $(columns[0]).offset().left) + ",  " + (e.pageX <= $(columns[0]).offset().left + columns[0].offsetWidth));
            if (e.pageY >= $(columns[i]).offset().top && e.pageY <= $(columns[i]).offset().top + columns[i].offsetHeight && e.pageX >= $(columns[i]).offset().left && e.pageX <= $(columns[i]).offset().left + columns[i].offsetWidth && cardClone==null) {
                cardClone = document.createElement('div');
                cardClone.classList += 'CardContainer';
                cardClone.style.minHeight = '100px';
                cardClone.style.backgroundColor = 'rgba(255,255,255,0.3)';
                $(columns[i]).children('span.AddCardButton').before(cardClone);
                selectedColumnId = $(columns[i]).children('input.ColumnId')[0].value;
                break;
            }
            else if(cardClone != null) {
                $(cardClone).remove();
                cardClone = null;
            }
        }
    }
}

function ToggleColumnMove(value) {
    toggleColumns = value;
}

function PickUpCard(e) {
    if (toggleColumns && card==null) {
        card = e.currentTarget;
        card.style.position = 'absolute';
        cardOffset.X = e.pageX - $(card).offset().left;
        cardOffset.Y = e.pageY - $(card).offset().top;
        selectedCardId = $(card).children('input.CardId')[0].value;
    }
}

function DropCard() {
    if (card != null) {
        card.style.position = 'static';
        card.style.left = null;
        card.style.top = null;
        if (cardClone != null) {
            $(card).remove();
            $(cardClone).replaceWith(card);
            $(card).mousedown(this, PickUpCard);
            $.post('/Board/MoveCard',{ cardId:selectedCardId, columnId:selectedColumnId });
        }
        card = null;
    }
    if (cardClone != null) {
        $(cardClone).remove();
    }
    cardClone = null;
}


function AddCard(colId, CardId) {
    var columnId = '#column-' + colId;
    $.get('/Board/CardView', { id: CardId }, function (data) {
        CloseModal();
        $(columnId).children('span.AddCardButton').before(data);
        $(columnId).children('div.CardContainer').mousedown(this, PickUpCard);
    });
}

function UpdateCard(idOfCard) {
    $.get('/Board/CardView', { id: idOfCard }, function (data) {
        CloseModal();
        $('#card-' + idOfCard).replaceWith(data);
        $('#card-' + idOfCard).mousedown(this, PickUpCard);
    });
    
}

function DeleteCard(CardId, icon) {
    if(confirm('Are You Sure?')) {
        $.post('/Board/DeleteCard', { cardId: CardId }, function () {
            $(icon).parent('div').remove();
            card = null;
        });
    }
}


function ColumnSuccess(columnId) {
    $.get('/Board/ColumnView', { id: columnId }, function (data) {
        CloseModal();
        $('#RowInsert').before(data);
    });
}


