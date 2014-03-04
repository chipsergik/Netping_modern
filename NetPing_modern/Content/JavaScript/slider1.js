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
});