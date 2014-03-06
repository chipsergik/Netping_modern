$(function() {
    $("#slider3").responsiveSlides({
        manualControls: '#slider3-pager',
        auto: true,
        speed: 400,
        timeout: 3000,
        nav: true,
        random: false,
        pause: true,
        navContainer: "#slider3-pager-nav",
        maxwidth: 540,
        before: function () {
            $('#slider3-pager').animate({
                scrollLeft: $('#slider3-pager').scrollLeft() + $(".rslides_here").offset().left - $('#slider3-pager').offset().left - 75
            }, 450);
        },
        after: function() {
            $('#slider3-pager').animate({
                scrollLeft: $('#slider3-pager').scrollLeft() + $(".rslides_here").offset().left - $('#slider3-pager').offset().left - 75
            }, 350);
        }
    });
<<<<<<< HEAD
=======

    $('#searchIcon').click(function () {
        var img = $($('.rslides1_on').find('img')[0]);
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

    $('.popup .close, .overlay').click(function() {
        $('.overlay, .popup').hide();
    });
    
    $(document).keyup(function (e) {
        if (e.keyCode == 27) {
            $('.overlay, .popup').hide();
        }
    });
    
>>>>>>> 771ea6a2f60392232546330798e92f08e2358291
});