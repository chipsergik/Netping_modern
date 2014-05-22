$(function () {
    var banners = window.Banners;
    var slider = document.getElementById("slider1");
    for (var i = 0; i < banners.length; i++) {
        var a = document.createElement('a');
        a.href = banners[i].Url_link;
        var img = document.createElement('img');
        img.src = banners[i].Url;
        a.appendChild(img);
        slider.appendChild(a);
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