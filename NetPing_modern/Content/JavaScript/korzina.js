(function (window, $, document) {
    var C = jaaulde.utils.cookies;
    //        C.del(true);
    C.setOptions({ expiresAt: 1 });

    var isKorzinaPopupOpened = false;

    function addTovar() {
        var ID = $("#kID")[0].innerHTML.trim();
        var name = $("#kName")[0].innerHTML.trim();
        var price = $("#kPrice")[0].innerHTML.trim();
        var photoURL = $("#kPhotoURL")[0].innerHTML.trim();
        C.set(ID, JSON.stringify({
            name: name,
            count: 1,
            price: price,
            photoURL: photoURL,
            ID: ID
        }));
    }

    $("#shopItem").on("click", function () {
        addTovar();
        showPopup();
    });

    $("#clearCartButton").click(function () {
        $(".hiddenID").each(function () {
            C.del(this.innerHTML);
        });
        hideKorzinPopup();
    });

    var animationTime = 300;
    $("#continueCartButton").click(function () {
        $("#korzinaItems").animate({
            left: "-450px"
        }, animationTime);
        $("#clearCartButton").animate({
            opacity: 0
        }, animationTime, function () {
            $("#clearCartButton, #continueCartButton").hide();
            $("#backCartButton").fadeIn();
            $("#sendDataCartButton").fadeIn();
        });
    });

    $("#backCartButton").click(function () {
        $("#korzinaItems").animate({
            left: "25px"
        }, animationTime);
        $("#clearCartButton").animate({
            opacity: 1,
        }, animationTime, function () {
            $("#backCartButton, #sendDataCartButton").hide();
            $("#clearCartButton").fadeIn();
            $("#continueCartButton").fadeIn();
        });
    });

    function showPopup() {
        isKorzinaPopupOpened = true;
        var korzinaPopup = $('#korzinaItems');
        korzinaPopup.empty();
        var data = getData() || [];
        var itemShablonSH = $("#korizinaItemShablon").clone().removeAttr('id');
        for (var i = 0; i < data.length; i++) {
            var product = data[i];
            var itemShablon = itemShablonSH.clone();
            itemShablon.find(".hiddenID")[0].innerHTML = product.ID;
            var itemImg = itemShablon.find(".shopPopupItemImg")[0];
            itemImg.src = product.photoURL;
            var itemName = itemShablon.find(".shopItemName")[0];
            itemName.innerHTML = product.name;
            var itemPrice = itemShablon.find(".shopItemPrice")[0];
            itemPrice.innerHTML = product.price;
            var itemCount = itemShablon.find(".shopItemCountInput")[0];
            itemCount.value = product.count;
            var itemPriceItogo = itemShablon.find(".shopItemPriceItogo")[0];
            itemPriceItogo.innerHTML = parseInt(product.price) * parseInt(product.count);
            korzinaPopup.append(itemShablon);
        }
        $(".shopItemCountInput").on("blur, change", function () {
            this.value = parseInt(this.value);
            if (!this.value || parseInt(this.value) <= 0) this.value = 1;
            var productContainer = $(this).parent().parent().parent();
            productContainer.find(".shopItemPriceItogo")[0].innerHTML = parseInt(this.value) *
                parseInt(productContainer.find(".shopItemPrice")[0].innerHTML);
            var prID = productContainer.find(".hiddenID")[0].innerHTML;
            updateCount(prID, this.value);
        });
        $(".removeProduct").click(function () {
            var productContainer = $(this).parent().parent().parent();
            var prID = productContainer.find(".hiddenID")[0].innerHTML;
            C.del(prID);
            productContainer.remove();
            if ($("#korzinaPopup").find(".shopPopupItem").length == 0) hideKorzinPopup();
        });
        $('.overlayKorzina, #korzinaPopup').show();

        $('.closeKorzina, .overlayKorzina').click(function () {
            hideKorzinPopup();
        });

        function updateCount(ID, count) {
            var p = C.get(ID);
            p.count = count;
            C.set(ID, p);
        }

        $(document).keyup(function (e) {
            if (e.keyCode == 27) { // ESC
                hideKorzinPopup();
            }
        });
    }
    
    function hideKorzinPopup() {
        $('.overlayKorzina, #korzinaPopup').hide();
        isKorzinaPopupOpened = false;
        $("#korzinaItems").css("left", "25px");
    }

    $("#sendDataCartButton").click(function() {
        sendData();
    });
    function sendData() {
        var data = getData() || [];
        if (data.length == 0) return;
        var fio = $("#FIOK").val();
        var email = $("#emailK").val();
        var address = $("#addressK").val();
        var rekviziti = $("#rekvizitiK").val();
        var pochta = $("input:radio[name=sposobDostavki]:checked").val();
        var requestData = {
            tovari: data,
            fio: fio,
            email: email,
            address: address,
            rekviziti: rekviziti,
            pochta: pochta
        };
        $.ajax({
            type: "POST",
            url: "/cart",
            data: requestData,
            success: function() {
                alert("success");
            },
            complete: function(e) {
                alert(e);
            },
            error:function() {
                alert("compleate");
            },
            dataType: "json"
        });
    }

    function getData() {
        var productArray = [];
        var products = C.get();
        for (var x in products) {
            var p = products[x];
            if (isProduct(p)) {
                productArray.push(p);
            }
        }
        return productArray;
    }

    function isProduct(p) {
        return p.hasOwnProperty("name") &&
            p.hasOwnProperty("price") &&
            p.hasOwnProperty("count") &&
            p.hasOwnProperty("photoURL") &&
            p.hasOwnProperty("ID");
    }

    window.get = C.get;
    
    $('#korzinaPopup').on('DOMMouseScroll mousewheel', function (e) {
        var delta = e.originalEvent.wheelDelta || e.originalEvent.detail * -1 * 60;
        $("#korzinaItems").scrollTop($("#korzinaItems").scrollTop() - delta);
        e.preventDefault();
        e.stopPropagation();
        return false;
    });
    


})(window, jQuery, document);
