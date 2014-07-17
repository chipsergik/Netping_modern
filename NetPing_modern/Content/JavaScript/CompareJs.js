function CompareJs(table, deviceCount) {
    this._table = $(table);
    this._deviceCount = deviceCount;

    this._table.css('width', 230 * deviceCount);

}

CompareJs.prototype.showDistinct = function () {
    var rows = $('tbody tr', this._table);
    if (rows.length > 0) {
        for (var i = 0; i < rows.length; i++) {
            var row = rows.eq(i);
            var columns = $('td', row);
            var value = null;
            var distinct = false;
            for (var j = 1; j < columns.length; j++) {
                var column = columns.eq(j);
                if (value == null) {
                    value = column.text().trim().toLowerCase();
                }
                else {
                    if (value != column.text().trim().toLowerCase()) {
                        distinct = true;
                        break;
                    }
                }
            }
            if (!distinct) {
                row.fadeOut(500);
            }
        }
    }
};

CompareJs.prototype.showAll = function () {
    $('tbody tr:hidden', this._table).fadeIn(500);
};

$(function () {
    $('.searchIcon').click(function () {

        var img = $($(this).parent().find('img')[0]);
        console.log(img);
        if (img) {
            img = img.clone();
            img.removeClass();
            img.addClass('popupImage');
            $('#popup1').empty();
            $('#popup1').append(["<span class='close'></span>", img]);
            $('#popup1').show();
            $('.overlay').show();
            $('.close').click(function () {
                $('.overlay, .popup').hide();
            });
        }
    });

    $('.popup .close, .overlay').click(function () {
        $('.overlay, .popup').hide();
    });

    $(document).keyup(function (e) {
        if (e.keyCode == 27) {
            $('.overlay, .popup').hide();
        }
    });

    $(".shopItem").on("click", function () {
        addProduct($(this).parent());
    });
})