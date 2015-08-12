$(document).ready(function () {
    $('.content .tabs-desc .tab').on('click', function () {
        $(this).addClass('active').parent().find('.active').not(this).removeClass('active');
        $('.tab-c[data-tab-id=' + $(this).data('tab-id') + ']').addClass('shown').parent().find('.shown').not('.tab-c[data-tab-id=' + $(this).data('tab-id') + ']').removeClass('shown');
    });

    checkCompare();

    $('input[type=checkbox][name=compare]').on('click', checkCompare);
});

function checkCompare() {
    if ($('input[type=checkbox][name=compare]:checked').length < 2)
        $('.btn.compare').attr('disabled', 'disabled');
    else
        $('.btn.compare').removeAttr('disabled');
}