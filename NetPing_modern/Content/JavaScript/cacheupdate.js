$(document).ready(function () {
    var taskNames = [
        "TermsFileTypes",
        "Terms",
        "TermsLabels",
        "TermsCategories",
        "TermsDeviceParameters",
        "TermsDestinations",
        "SiteTexts",
        "DevicesParameters",
        "DevicePhotos",
        "PubFiles",
        "SFiles",
        "Posts",
        "Devices",
        "GenerateYml"
    ];

    $(taskNames).each(function (index, item) {
        $('#update-messages').append('<p id="' + item + '">Task ' + item
                        + ' is processing <img style="margin: 0 10px;" height=16 src="/Content/Images/preloader.gif" /></p>');
        $.ajax({
            type: "POST",
            url: "/innerpages/ucacheasyncwork",
            data: JSON.stringify({ dataName: item }),
            dataType: 'json',
            processData: true,
            traditional: true,
            contentType: 'application/json; charset=utf-8',
            async: true,
            success: function (response) {
                $('#' + item).text('Task ' + item
                + " completed with message: " + response);
            },
            error: function (response) {
                $('#' + item).text('Task ' + item
                              + " completed with message: " + response);
                console.log('Task ' + item + " completed with message: " + response);
            },
        });
    });

})