$(function () {
    var banners = window.Banners;
    var slider = document.getElementById("slider1");
    for (var i = 0; i < banners.length; i++) {
        var li = document.createElement('li');
        var img = document.createElement('img');
        img.src = banners[i];
        li.appendChild(img);
        slider.appendChild(li);
    }
    $("#slider1").responsiveSlides({
        auto: true,
        speed: 500,
        timeout: 3000,
        nav: true,
        random: false,
        pause: true,
        navContainer: "",
        manualControls: "",
        namespace: "callbacks"
    });
    $(".callbacks_nav").hide();
    $("#wrapper").hover(function () {
        $(".callbacks_nav").show();
    }, function () {
        $(".callbacks_nav").hide();
    });
});