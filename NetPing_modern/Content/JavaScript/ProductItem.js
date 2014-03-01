$(document).ready(function () {
    $('.content .tabs-desc .tab').on('click', function () {
        $(this).addClass('active').parent().find('.active').not(this).removeClass('active');
        $('.tab-c[data-tab-id=' + $(this).data('tab-id') + ']').addClass('shown').parent().find('.shown').not('.tab-c[data-tab-id=' + $(this).data('tab-id') + ']').removeClass('shown');
    });

});