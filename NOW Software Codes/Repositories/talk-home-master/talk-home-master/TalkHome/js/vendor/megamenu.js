/*global $ */
$(document).ready(function () {

    "use strict";

    $('.menu > ul > li:has( > ul)').addClass('menu-dropdown-icon');
    //Checks if li has sub (ul) and adds class for toggle icon - just an UI


    //$('.menu > ul > li > ul:not(:has(ul))').addClass('normal-sub');
    //Checks if drodown menu's li elements have anothere level (ul), if not the dropdown is shown as regular dropdown, not a mega menu (thanks Luka Kladaric)

    $(".menu > ul").before("<a href=\"#\" class=\"menu-mobile\"></a>");



    //Adds menu-mobile class (for mobile toggle menu) before the normal menu
    //Mobile menu is hidden if width is more then 959px, but normal menu is displayed
    //Normal menu is hidden if width is below 959px, and jquery adds mobile menu
    //Done this way so it can be used with wordpress without any trouble

    $(".menu > ul > li").hover(function (e) {
        if ($(window).width() > 943) {
            $(this).children("ul").stop(true, false).fadeToggle(150);
        }
    });

    $(".menu > ul > li").children("ul").hide();

    //If width is more than 943px dropdowns are displayed on hover
    $(".menu > ul > li").click(function () {
        if ($(window).width() < 943) {
            var thisId = $(this).attr("id");
            $(".menu-dropdown-icon-collapse").each(function () {
                var exId = $(this).attr("id");
                if (thisId != exId) {
                    $(this).children("ul").hide();
                    $(this).toggleClass("menu-dropdown-icon-collapse");
                    $(this).toggleClass("menu-dropdown-icon");
                }
            });

            $(this).toggleClass("menu-dropdown-icon-collapse");
            $(this).toggleClass("menu-dropdown-icon");
            $(this).children("ul").fadeToggle(150);

        }
    });

    $('.menu-mobile').click(function () {
        $(this).toggleClass('menu-mobile-dropdown-icon');
    });

    //If width is less or equal to 943px dropdowns are displayed on click (thanks Aman Jain from stackoverflow)

    $(".menu-mobile").click(function (e) {
        $(".menu > ul").toggleClass('show-on-mobile');
        e.preventDefault();
    });
    //when clicked on mobile-menu, normal menu is shown as a list, classic rwd menu story (thanks mwl from stackoverflow)

    $("#phoneTopup,#phoneTopup1,#phoneTopup2,#phoneTopup3,#phoneTopup4").click(function (e) {
        $("#formPhoneTopup").submit();
        e.preventDefault();
    });

    $("#phoneTopup3").click(function (e) {
        $("#formPhoneTopup1").submit();
        e.preventDefault();
    });

    $("#appTopup,#appTopup1").click(function (e) {
        $("#formAppTopup").submit();
        e.preventDefault();
    });

    $("#cardTopup,#cardTopup1,#cardTopup2", "#cardTopup3").click(function (e) {
        $("#formCardTopup").submit();
        e.preventDefault();
    });

    $("#cardTopup3,#cardTopup4").click(function (e) {
        $("#formCardTopup1").submit();
        e.preventDefault();
    });


    $("#userMenuTopup").click(function (e) {
        $("#userFormTopup").submit();
        e.preventDefault();
    });


    $(window).resize(function () {
        if ($(window).width() > 943) {
            $('.menu > ul > li:has( > ul)').addClass('menu-dropdown-icon');
            $(".menu > ul > li").removeClass("menu-dropdown-icon-collapse");
            $(".menu > ul > li > ul").hide();
        }
    });


    $(".menu").show();

});
