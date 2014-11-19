$(document).ready(function () {
    $('#datepicker').datetimepicker(
        {
            format: 'd.m.Y',
            inline: true,
            lang: 'ru',
            timepicker: false,
            todayButton: false
        });
    $('.tag').click(function(){

        if ($(this).hasClass('active'))
        {
            var form = $('#tagsForm');
            $('input[value="' + $(this).data('tag-path') + '"]', form).remove();
        }
        else
        {
            var tag = $("<input type='hidden' name='tags'/>");
            tag.val($(this).data('tag-path'));
            $('#tagsForm').append(tag);
        }

        $('#tagsForm').submit();
    });
    $('.tag[data-tag-selected="True"]').addClass('active').each(function(){
        var tag = $("<input type='hidden' name='tags'/>");
        tag.val($(this).data('tag-path'));
        $('#tagsForm').append(tag);
    });
    $('#gSearch input[type="submit"]').click(function(){
        $('input[name="q"]').val('site:netping.ru ' + $('[name="q"]').val());
        $('#gSearch').submit();
        return false;
    });
});