$(document).ready(function () {
    updateCartCount();

    $("#shopItem").on("click", function () {
        addProduct('body');
    });

    $('.cat_item .action .price span').on("click", function () {
        var item = $(this).parents('.cat_item');
        addProduct(item);
    });

    $(".header a.cart").on("click", function () {
        showPopup();
    })

    $("#clearCartButton").click(function () {
        clearCart();
    });

    $("#closeCartButton").click(function () {
        hidePopup();
    });

    $("#continueCartButton").click(function () {
        $("#cartItems").hide();
        $("#cartAbout").show();
        $("#continueCartButton").hide();
        $("#sendDataCartButton").show();
        $("#clearCartButton").hide();
        $("#backCartButton").show();
    })

    $("#backCartButton").click(function () {
        $("#continueCartButton").show();
        $("#sendDataCartButton").hide();
        $("#clearCartButton").show();
        $("#backCartButton").hide();
        $("#cartAbout").hide();
        $("#cartItems").show();
    })

    $("#sendDataCartButton").click(function () {


        var data = getData() || [];
        if (data.length == 0) return;
        var fio = $("#FIOK").val().trim();
        var email = $("#emailK").val().trim();
        var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        var address = $("#addressK").val().trim();
        var isvalid = true;
        if (!regex.test(email)) {
            isvalid = false;
            $("#emailK").addClass('invalid');
        }
        if (fio.length == 0) {
            isvalid = false;
            $("#FIOK").addClass('invalid');
        }
        if (address.length == 0) {
            isvalid = false;
            $("#addressK").addClass('invalid');
        }
        if (!isvalid)
            return;
        var requisites = $("#requisites").val();
        var phone = $("#phone").val().trim();
        var shipping = $("input:radio[name=shipping]:checked").val();
        var requestData = {
            Name: fio,
            EMail: email,
            Address: address,
            Requisites: requisites,
            Shipping: shipping,
            Data: data,
            Phone: phone
        };
        console.log(requestData);
        $.ajax({
            type: "POST",
            url: "/cart/SendCartMail",
            data: requestData,
            success: function () {
                clearCart();
                alert("Ваш заказ отправлен в обработку");
            },
            error: function () {
                alert("Что-то пошло не так");
            },
            dataType: "json"
        });
    })
})

var C = jaaulde.utils.cookies;
//        C.del(true);
C.setOptions({ expiresAt: 1 });

var isCartPopupOpened = false;

function updateCartCount() {
    console.log('upd');
    var data = getData() || [];
    var cartcount = 0;
    for (i = 0; i < data.length; i++) {
        cartcount += parseInt(data[i].count);
    }
    $('.cart_count').text(cartcount);
}

function addProduct(itemcontainer) {
    var ID = $(itemcontainer).find(".ID")[0].innerHTML.trim();
    var name = $(itemcontainer).find(".Name")[0].innerHTML.trim();
    var price = $(itemcontainer).find(".Price")[0].innerHTML.trim();
    var photoURL = $(itemcontainer).find(".PhotoURL")[0].innerHTML.trim();
    var key = $(itemcontainer).find(".Key")[0].innerHTML.trim();
    var data = getData() || [];
    var item = data.filter(function (element) {
        return element.ID == ID;
    });
    if (item.length > 0) {
        item[0].count++
        C.set(ID, JSON.stringify(item[0]));
    }
    else {
        C.set(ID, JSON.stringify({
            name: name,
            count: 1,
            price: price,
            photoURL: photoURL,
            key: key,
            ID: ID
        }));
    }
    updateCartCount();
}

function clearCart() {
    $(".hiddenID").each(function () {
        C.del(this.innerHTML);
    });
    hidePopup();
}


function showPopup() {
    isCartPopupOpened = true;
    var cartPopup = $('#cartItems');
    cartPopup.empty();
    var data = getData() || [];
    var itemTemplateSH = $("#cartItemTemplate").clone().removeAttr('id');
    updateSum(data);

    for (var i = 0; i < data.length; i++) {
        var product = data[i];
        var itemTemplate = itemTemplateSH.clone();
        itemTemplate.find(".hiddenID")[0].innerHTML = product.ID;
        var itemImg = itemTemplate.find(".shopPopupItemImg")[0];
        itemImg.src = product.photoURL;
        var itemName = itemTemplate.find(".shopItemName")[0];
        itemName.innerHTML = '<a href="/product_item.aspx?id=' + product.key + '">' + product.name + '</a>';
        var itemPrice = itemTemplate.find(".shopItemPrice")[0];
        itemPrice.innerHTML = product.price;
        var itemCount = itemTemplate.find(".shopItemCountInput")[0];
        itemCount.value = product.count;
        var itemPriceSum = itemTemplate.find(".shopItemPriceSum")[0];
        itemPriceSum.innerHTML = parseInt(product.price) * parseInt(product.count);
        cartPopup.append(itemTemplate);
    }

    $(".shopItemCountInput").on("blur, change", function () {
        this.value = parseInt(this.value);
        if (!this.value || parseInt(this.value) <= 0) this.value = 1;
        var productContainer = $(this).parents('.shopPopupItem');
        productContainer.find(".shopItemPriceSum")[0].innerHTML = parseInt(this.value) *
            parseInt(productContainer.find(".shopItemPrice")[0].innerHTML);
        var prID = productContainer.find(".hiddenID")[0].innerHTML;
        updateCount(prID, this.value);
        data = getData() || [];
        updateSum(data);
    });
    $(".shopPopupItem .remove").click(function () {
        var productContainer = $(this).parents('.shopPopupItem');
        var prID = productContainer.find(".hiddenID")[0].innerHTML;
        C.del(prID);
        data = getData() || [];
        productContainer.remove();
        if ($("#cartPopup").find(".shopPopupItem").length == 0) hidePopup();
    });
    $('.overlayCart, #cartPopup').show();

    $('.closeCart, .overlayCart').click(function () {
        hidePopup();
    });

    function updateCount(ID, count) {
        var p = C.get(ID);
        p.count = count;
        C.set(ID, p);
    }

    function updateSum(data) {
        var sum = 0;
        for (i = 0; i < data.length; i++) {
            sum += data[i].price * data[i].count;
        }
        $('#cartCostValue')[0].innerHTML = sum;
    }

    $(document).keyup(function (e) {
        if (e.keyCode == 27) { // ESC
            hidePopup();
        }
    });

    $('body').addClass('noscroll');
}

function hidePopup() {
    $('.overlayCart, #cartPopup').hide();
    iscartPopupOpened = false;
    $('body').removeClass('noscroll');
    updateCartCount();
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
        p.hasOwnProperty("ID") &&
        p.hasOwnProperty("key");
}

