(function (window, $, document) {
    var C = jaaulde.utils.cookies;
    //    C.del(true);
    C.setOptions({ expiresAt: 1 });

    function addTovar() {
        var ID = $("#kID")[0].innerHTML.trim();
        var name = $("#kName")[0].innerHTML.trim();
        var price = $("#kPrice")[0].innerHTML.trim();
        var photoURL = $("#kPhotoURL")[0].innerHTML.trim();
        console.log(ID, name, price, photoURL);
        C.set(ID, JSON.stringify({
            name: name,
            price: price,
            photoURL: photoURL
        }));
    }

    $("#shopItem").on("click", function () {
        addTovar();
        showPopup();
    });

    function changeCount() {

    }


    function clear() {

    }

    function showPopup() {

        var img = $($('.rslides1_on').find('img')[0]);
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
    }

    $('.popupK .close, .overlayKorzina').click(function () {
        $('.overlayKorzina, .popupK').hide();
    });

    $(document).keyup(function (e) {
        if (e.keyCode == 27) {
            $('.overlayKorzina, .popupK').hide();
        }
    });
    function removeItem() {

    }

    function send(successCb, errorCb) {

    }

    function getData() {

    }

    window.get = C.get;
    window.send = send;

})(window, jQuery, document);
