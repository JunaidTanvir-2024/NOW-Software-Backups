/**
 * JS entry point for the website.
 * Initialises JS AMD modules based on a comma-separated list of module names found on every HTML tag with the attribute 
 * `data-js-module`.
 * 
 * @author micheled
 * Created on 12.07.2017
 * 
 */
'use strict';
$(document).ready(function () {
    SelectDefaultQuickTopUpAmount();
    CheckBannerCookie();

    var NavigateToDataPlan = $('#NavigateToDataPlan').val();
    if (NavigateToDataPlan === "True") {
        $('#anchorDataOnly').click();
    }

    var modules = '',
        breakPoint = 960,
        width = $(document).width();

    function InitialiseJSModulesOnPage() {
        $('[data-js-module]').each(function () {
            modules += ($(this).attr('data-js-module').split(',')) + ','; // Add to the comma-separated string
        });

        var modulesArr = modules.split(','); // Create the array of modules name

        for (var i = 0, max = modulesArr.length; i < max; i++) {
            var module = modulesArr[i];
            window[module]; // Create a new global object using the module name (aka initiliase it)
        }
    }

    InitialiseJSModulesOnPage(); // Initialise JS modules found by scanning [data-js-modules] attributes

    // Most basic url parser
    // All Credits to https://gist.github.com/jlong/2428561
    // parser.protocol; => "http:"
    // parser.hostname; => "example.com"
    // parser.port;     => "3000"
    // parser.pathname; => "/pathname/"
    // parser.search;   => "?search=test"
    // parser.hash;     => "#hash"
    // parser.host;     => "example.com:30
    //var parser = document.createElement('a');
    //parser.href = window.location.href;
    //var paths = parser.pathname.split("/"); // close enough to C# fragments
    var parser = document.createElement('a');
    parser.href = window.location.href;
    var hrefLocationPath = parser.pathname;

    if (hrefLocationPath.indexOf("/") > 0) {
        hrefLocationPath = "/" + hrefLocationPath;
    }
    var paths = hrefLocationPath.split("/"); // close enough to C# fragments

    console.log(paths);

    console.log("main js 09082018-1 loaded");

    // Initialise Rates

    //$("#AcceptCookie").on("click", function(){
    //    document.cookie = "TalkHomeAcceptCookie=true; path=/; expires=Fri, 31 Dec 2038 23:59:59 GMT";
    //    //localStorage.setItem("TalkHomeAcceptCookie", "TalkHomeAcceptCookie=true; path=/; expires=Fri, 31 Dec 2038 23:59:59 GMT");
    //    $("#CookieBanner").hide();
    //});

    $("#DDO__Body__Banners--left").on("click", function (e) {

        $("#BundleId").val("1308");
        $("#DoubleDataForm").submit();
    });

    $("#DDO__Body__Banners--right").on("click", function (e) {

        $("#BundleId").val("1459");
        $("#DoubleDataForm").submit();
    });



    $("#Email").on("input", function (e) {
        $("#saveSubs").removeAttr('disabled');
    });

    $("#SubscribeMailingList").on("click", function (e) {
        $("#saveSubs").removeAttr('disabled');
        if (this.checked) {
            $(this).attr("value", "true");
        } else {
            $(this).attr("value", "false");
        }
    });

    //$("#btnSignUpSubmit").on("click", function (e) {
    //    if ($('#TermsConditions').is(":checked") == false) {
    //    }
    //    else {
    //    }
    //});

    $("#SubscribeSignUp").on("click", function (e) {
        if (this.checked) {
            $(this).attr("value", "true");
        } else {
            $(this).attr("value", "false");
        }
    });


    //widget support //
    var player = null;
    $(".app-video-link").on("click", function (e) {

        e.preventDefault();
        $("#vimeo-container").css("height", $(window).height());
        $("#vimeo-container").css("width", $(window).width());
        $("#vimeo-container").show();


        var vid = Number($(this).data("id"));
        var options = {
            id: vid,
            loop: true,
            autoplay: true,
            maxwidth: $(window).width(),
            maxheight: $(window).height()
        };


        if (player == null) {
            player = new Vimeo.Player('vimeo-container', options);

            player.setVolume(20);

            player.on('play', function () {
                console.log('played the video!');
            });
        }
        else {
            player.play();
        }


    });


    $(".vimeo-close").on("click", function (e) {
        player.pause();
        $("#vimeo-container").hide();
    });

    // Smooth scrolling
    $('a[href^="#"]').on('click', function (e) {
        if ($(this).data("scroll") === false) {
            return;
        }
        var target = $(this.getAttribute('href'));
        if (target.length) {
            e.preventDefault();
            $('html, body').stop().animate({
                scrollTop: target.offset().top
            }, 1000);
        }
    });

    // Drop down divs
    $('[data-dropdown=true]').click(function () {
        if ($(this).hasClass('active') == true) {
            $('[data-dropdown=true]').removeClass('active').find('.answer.active').slideUp(500);
            $('[data-dropdown=true]').removeClass('active').next('.answer.active').slideUp(500);
        } else {
            $('[data-dropdown=true]').removeClass('active').find('.answer.active').slideUp(0);
            $('[data-dropdown=true]').removeClass('active').next('.answer.active').slideUp(0);
            $(this).addClass('active').find('.answer').addClass('active').slideDown(500);
            $(this).addClass('active').next('.answer').addClass('active').slideDown(500);
        }
    });
    $('#showMore').click(function () {
        $('[data-show-more=true]').slideDown(250);
    });

    // My Account mobile dropdown product select
    $('#myAccountSelect').change(function () {
        $(this).find('option').each(function () {
            if ($(this).is(':selected'))
                window.location.href = $(this).attr('data-href');
        });
    });

    // Register product selection form
    if ($('#registerProduct').length > 0) {
        $('[name=productCode]').change(function () {
            $('#selectionForm').submit();
        });
    }

    $(".overlay").on("click", function () {
        $(this).hide();
    });

    if ($(window).width() < 943) {
        $("#desktop-overlay-content").hide();
        $("#mobile-overlay-content").show();
    }
    else {
        $("#desktop-overlay-content").show();
        $("#mobile-overlay-content").hide();
    }

    function hidePopUps() {
        $('.bundle-description').each(function () {
            $(this).slideUp();
        });

        $('.sms-help-pop').each(function () {
            $(this).slideUp();

        });
    }

    //Help pop ups
    $(".more-info").on("click", function (e) {
        hidePopUps();

        var p = $(this).parent().parent().find(".bundle-description");

        if ($(window).width() > 943) {
            if ($("#TopProducts").length == 0) {
                p.css({ left: 400 });
                p.css({ top: -140 });
            }
            else {
                p.css({ left: 110 });
                p.css({ top: 20 });
            }
        }

        p.fadeIn();

        $(this).parent().parent().parent().find(".close-description").on("click", function (event) {
            $(this).closest(".bundle-description").slideUp();
            event.stopPropagation();
        });
    });

    $(".purchase-sms-code").on("click", function (event) {
        hidePopUps();
        var p = $(this).parent().parent().find(".sms-help-pop");

        if ($(window).width() > 943) {
            if ($("#TopProducts").length == 0) {
                p.css({ left: 790 });
                p.css({ top: -140 });
            }
            else {
                p.css({ left: 110 });
                p.css({ top: 190 });
            }
        }

        p.fadeIn();

        $(this).parent().parent().parent().find(".sms-close-description").on("click", function (event) {
            $(this).closest(".sms-help-pop").slideUp();
            event.stopPropagation();
        });
    });


    //Google tags

    $("#SimOrderButton").on("click", function () {
        var lbl = "sim_order: " + $("#EmailAddress").val();
        gtag('event', 'purchase', {
            'event_category': 'ecommerce',
            'event_label': lbl
        });
    });

    if ($("#notification .alert").length > 0) {
        var lbl = "sim_order_validation: " + $("#EmailAddress").val();
        gtag('event', 'purchase', {
            'event_category': 'ecommerce',
            'event_label': lbl
        });
        var vl = $("#notification .alert").text();

        gtag('event', 'exception', {
            'event_category': 'description',
            'event_label': vl
        });
    }



    $(".cta-container a").on("click", function (e) {
        var l = $(this).attr("href");
        gtag('event', 'cta', {
            'origin': 'button',
            'event_label': l
        });
    });

    $("#mainMenu a").on("click", function (e) {
        var l = $(this).attr("href");
        gtag('event', 'cta', {
            'origin': 'menu',
            'event_label': l
        });
    });



    $(".destinations").on("change", function () {
        var d = $(this).val();
        gtag('event', 'select_content', {
            'origin': 'destination',
            'event_label': d
        });
    })

    $(".iphone-app").on("click", function (e) {
        var l = $(this).attr("href");
        gtag('event', 'cta', {
            'origin': 'button',
            'event_label': 'friendzone iphone'
        });
    });


    $(".google-play-app").on("click", function (e) {
        var l = $(this).attr("href");
        gtag('event', 'cta', {
            'origin': 'button',
            'event_label': 'friendzone google play'
        });
    });


    $(".window-app").on("click", function (e) {
        var l = $(this).attr("href");
        gtag('event', 'cta', {
            'origin': 'button',
            'event_label': 'friendzone windows app'
        });
    });


    window.onbeforeunload = function (e) {
        $("overlay").hide();
    }

    window.onbeforeunload = function (e) {
        $("overlay").hide();
    }


    // Clear out the Order SIM form 
    if ($("#OrderSimForm").length > 0) {

        $("form").get(0).reset();
    }
    



    $(".auto-renew-checkbox").on('change', function () {
        var isRenew = false;
        var self = $(this);
        if (self.is(":checked")) {
            isRenew = true;
        }

        var jdata = {
            'msisdn': self.data("msidn"),
            'productCode': self.data("productcode"),
            'autoTopUp': isRenew,
            'calling_packageid': self.data("guid")
        };
        var auto = self.closest('div').find('.autorenewerror');
        $(auto).hide();
        $.ajax({
            type: "POST",
            url: autoRenewUrl,
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(jdata),
            dataType: "json",
            success: function (recData) { },
            error: function () {
                $(auto).show();
                $(self).prop("checked", false);
            }
        });
    });

    // Plans tickets flip
    // Icon top right flip
    $('.tk-flip-trigger, .tk-flip-trigger-b').each(function () {
        $(this).on("click", function () {
            $(this).closest('.tk-sub-container').toggleClass('flipped');
            $(".sms-code").hide();
            $(".plan-info").show();
        });
    });

    // Plan SMS code flip
    $('.linkl').each(function () {
        $(this).on("click", function () {
            $(this).parent().parent().parent().find('.tk-sub-container').toggleClass('flipped');
            $(".sms-code").show();
            $(".plan-info").hide();
        });
    });

    // Plan B table strips li background
    $("ul.bundles  li.table-plans:visible:odd").addClass("oddItem");
    $("ul.bundles  li.table-plans:visible:even").addClass("evenItem");

    // ===== Scroll to Top button ==== 
    $(window).scroll(function () {
        if ($(this).scrollTop() >= 50) {        // If page is scrolled more than 50px
            $('#return-to-top').fadeIn(200);    // Fade in the arrow
        } else {
            $('#return-to-top').fadeOut(200);   // Else fade out the arrow
        }
    });
    $('#return-to-top').click(function () {      // When arrow is clicked
        $('body,html').animate({
            scrollTop: 0                       // Scroll to top of body
        }, 500);
    });


    // Change tab class and display content
    $('.tabs-nav a').on('click', function (event) {
        event.preventDefault();

        $('.tab-active').removeClass('tab-active');
        $(this).parent().addClass('tab-active');
        $('.tabs-stage > div').hide();
        $($(this).attr('href')).show();
    });

    $('.tabs-nav a:first').trigger('click'); // Default


    //show password
    //$('.pwd-hs-btn').on('click', function () {
    //    
    //    var $x = $(this).parent('.pwd-hs').find('.pwd-hs-input');

    //    if ($x.attr('type') === 'password') {
    //        $x.attr('type', 'text');
    //        $(this).text('Hide').css({ 'background-color': '#59c9e4', 'color': '#ffffff' });
    //    } else {
    //        $x.attr('type', 'password');
    //        $(this).text('show').css({ 'background-color': '#ffffff', 'color': '#59c9e4' });
    //    }
    //});

    //view more cc
    $('#cardViewMore').on('click', function () {
        $('.main-calling-cards .col-6:last-child').css('display', 'block');
        $(this).remove();
    })

});

function SelectDefaultQuickTopUpAmount() {
    $('.defaultFastTopUp').prop('checked', true);
}

function CheckBannerCookie() {
    if (getCookie("TalkHomeAcceptCookie") == "") {
        var url = '/Account/CookieAccept';
        $.get(url, function (result) {
            var cookieDiv = $('#divCookieBannerLayout');
            cookieDiv.replaceWith(result);
        });
    }
}
//Get Cookie by name

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function OnCookieAccept() {
    document.cookie = "TalkHomeAcceptCookie=true; path=/; expires=Fri, 31 Dec 2038 23:59:59 GMT";
    //localStorage.setItem("TalkHomeAcceptCookie", "TalkHomeAcceptCookie=true; path=/; expires=Fri, 31 Dec 2038 23:59:59 GMT");
    $("#CookieBanner").hide();
}

$('.ukNumbersOnly').keydown(function () {
    $("span.quickTopUpError").hide();
});

$('.ukNumbersOnly').keypress(function () {
    var length = this.value.length;

    if (this.value.length > 0) {
        var firstCharacter = this.value.charAt(0);
        if ((firstCharacter === '0' && length > 10) || (firstCharacter !== '0' && length > 9)) {
            return false;
        }
    }
    return (event.charCode === 43 || (event.charCode >= 48 && event.charCode <= 57));
});

$('.CCNumbersOnly').keypress(function () {
    var length = this.value.length;
    //if (event.charCode === 43 || (event.charCode >= 48 && event.charCode <= 57)) {
    //    return false;
    //}
    if (this.value.length > 20) {
        return false;
    }

    return (event.charCode === 43 || (event.charCode >= 48 && event.charCode <= 57));
});

$('.securityCode').keypress(function () {
    var length = this.value.length;

    if (this.value.length >= 3) {
        return false;
    }
    return (event.charCode === 43 || (event.charCode >= 48 && event.charCode <= 57));
});

$('.securityCode').bind("paste", function (e) {
    e.preventDefault();
});

//$('#inpMsisdnQuickTopUp').bind("paste", function (e) {
//    e.preventDefault();
//});

$('#pUK20').on("click", function () {
    var token = $("#pUKPlanFrmTkn input[type=hidden]").val();
    var html = "<form method=\"Post\" action=\"/pay360account/purchasecheckout\">";
    html += "<input name=\"__RequestVerificationToken\" type=\"hidden\" value=\"" + token + "\" />";
    html += "<input name=\"Id\" type=\"hidden\" value=\"1459\">";
    html += "<input name=\"ProductCode\" type=\"hidden\" value=\"THM\">";
    html += "</form>";

    $(html).appendTo('body').submit();

});

$('.spnAmountQuickTopUp').on("click", function () {
    $('#hdnFastTopUpId').val($(this).data("id"));
    $('#hdnFastTopUpAmount').val($(this).data("amount"));
});

$("#OrderSimForm").on("submit", function () {
    
    if ($("#lookupLink").is(':visible') === true && $("#addressFields").is(':visible') === false && $("#destinations").is(':visible') === false) {
        $('#lookupLink').trigger("click");
        return false;
    }
    else {
        return true;
    }
});