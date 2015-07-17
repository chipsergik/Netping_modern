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

$(document).ready(function () {

    updateInvoke(0);
})

function updateInvoke(itemId) {
    item = taskNames[itemId];
    $('#update-messages').append('<p id="' + item + '">Task ' + item
                       + ' is processing <img style="margin: 0 10px;vertical-align: middle;" height=16 src="/Content/Images/preloader.gif" /></p>');
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
            if (++itemId < taskNames.length)
                updateInvoke(itemId);
            else
            {
                document.title += "(completed)";
                $('#update-messages').append('<h3>Cache updated</h3>');
            }

        },
        error: function (response) {
            $('#' + item).text('Task ' + item
                          + " completed with message: " + response);
            console.log('Task ' + item + " completed with message: ");
            console.log(response);
        },
    });
}