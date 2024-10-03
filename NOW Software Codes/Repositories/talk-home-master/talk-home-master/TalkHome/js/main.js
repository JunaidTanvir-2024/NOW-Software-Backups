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
    else {
        $('#aFilterCallsTextData').click();
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
        if ($('#takeBackToTransferCredit').length > 0) {
            setTimeout(
                function () {
                    window.location = "/international-topup";
                }, 3000);
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
    new Rates;
    new AirTimeTransfer;
    new Widgets;

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
        if (typeof gtag !== "undefined") {
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


    google.charts.setOnLoadCallback(drawChart);

    function drawChart() {
        $(".bundle-visualizer").each(function () {
            try {
                var id = $(this).attr("id");
                var remaining = Number($(this).data("remaining"));
                var total = Number($(this).data("total"));
                var runit = $(this).data("runit");

                if ($(this).data("unit") && $(this).data("unit") == "gb" && $(this).data("runit") && $(this).data("runit") == "mb") {
                    total = total * 1000;
                }

                var pUsed = ((total - remaining) / total) * 100;
                var pRemaining = (remaining / total) * 100;
                var data = google.visualization.arrayToDataTable([
                    ['Usage', 'Amount'],
                    ['Used', pUsed],
                    ['Left', pRemaining]
                ]);

                var options = {
                    pieHole: 0.5,
                    pieSliceTextStyle: {
                        color: 'black',
                    },
                    colors: ['#dd3912', '#252192']
                };

                var chart = new google.visualization.PieChart(document.getElementById(id));
                chart.draw(data, options);
            }
            catch (e) {
                console.log("unable to render chart" + id);
                $(this).hide();
            }
        });

    }


    $("#SimSwapCheck").on('change', function () {
        var self = $(this);
        if (self.is(":checked")) {
            $("#SimSwap").val("on");
        } else {
            $("#SimSwap").val("off");
        }
    });



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
            'isAutoRenew': isRenew,
            'calling_packageid': self.data("guid"),
            'BundleAmount': self.data("amount"),
            'BundleName': self.data("bundlename")

        };


        var Uuid = $(this).attr('data-guid');
        var sameBundles = $(".auto-renew-checkbox[data-guid=" + Uuid + "]");

        $(sameBundles).prop("checked", $(this).is(':checked'));

        var auto = self.closest('div').find('.autorenewerror');
        $(auto).hide();
        $.ajax({
            type: "POST",
            url: autoRenewUrl,
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(jdata),
            dataType: "json",
            success: function (response) {

                if (response.errorCode == 0) {
                    $.Toast(response.message,
                        { 'align': 'center', 'position': 'top', 'width': 300, 'class': 'sl-notificationSuccess' }
                    );
                }
                else {
                    $.Toast(response.message,
                        { 'align': 'center', 'position': 'top', 'width': 300 }
                    );

                }
            },
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

    //Security CCVCode field should reset when user change/swap the Card Number
    $('.co-radio-btn input[name=Pay360PaymentMethodExistingCards]').on('click', function () {
        $('#inputPay360Security').val('');
    });

    //Account Page Navigation
    $('ul.ac-navs li').click(function () {
        var tab_id = $(this).attr('data-tab');

        $('ul.ac-navs li').removeClass('active');
        $('.tab-content').removeClass('active');

        $(this).addClass('active');
        $("#" + tab_id).addClass('active');
    });

    //to make full width in active plans
    $("div.col-md-6.mb-3 div.active-plan-card").each(function (e) {
        if (!($(this).find('div.isMinutes').next().hasClass('isTexts') && $(this).find('div.isMinutes').next().hasClass('col-sm-6'))) {
            //change Mins to col-12
            $(this).find('div.isMinutes').removeClass('col-sm-6');
            $(this).find('div.isMinutes').addClass('col-sm-12');

            //change Txts to col-12
            $(this).find('div.isTexts').removeClass('col-sm-6');
            $(this).find('div.isTexts').addClass('col-sm-12');
        }
    });





    let mins;
    let secs;
    let interval;

    $(document).ready(function () {
        $("#sendOTP").click(function () {
            mins = 10;
            secs = 0;
            let btn = $(this);
            btn.addClass("disabled");
            interval = setInterval(function () {
                if (mins >= 0 && secs >= 0) {
                    btn.text("Resend OTP in " + pad(mins, 2) + ":" + pad(secs, 2));
                    if (secs > 0) {
                        secs--;
                    } else {
                        secs = 59;
                        mins--;
                    }
                    if (mins < 0) {
                        clearInterval(interval);
                        btn.removeClass("disabled").text("Resend OTP");
                        return true;
                    }
                }
                console.log("Mins: " + mins + ", Secs: " + secs);
            }, 1000);
        });

    });

    function pad(num, size) {
        let s = num + "";
        while (s.length < size) s = "0" + s;
        return s;
    }

    $('.digit-group').find('input').each(function () {
        $(this).attr('maxlength', 1);
        $(this).on('keyup', function (e) {
            var parent = $($(this).parent());

            if (e.keyCode === 8 || e.keyCode === 37) {
                var prev = parent.find('input#' + $(this).data('previous'));

                if (prev.length) {
                    $(prev).select();
                }
            } else if ((e.keyCode >= 48 && e.keyCode <= 57) || (e.keyCode >= 65 && e.keyCode <= 90) || (e.keyCode >= 96 && e.keyCode <= 105) || e.keyCode === 39) {
                var next = parent.find('input#' + $(this).data('next'));

                if (next.length) {
                    $(next).select();
                } else {
                    if (parent.data('autosubmit')) {
                        parent.submit();
                    }
                }
            }
        });
    });

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
    //$("#CookieBanner").hide();
    $("#CookieAlert").hide();
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


$('.ukNumbersOnly').keypress(function () {
    var length = this.value.length;
    //if (event.charCode === 43 || (event.charCode >= 48 && event.charCode <= 57)) {
    //    return false;
    //}
    if (this.value.length > 20) {
        return false;
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

//$('#inpMsisdnQuickTopUp').bind("paste", function () {
//    
//    if($('#frmQuickTopUp').valid()) {
//        return true;
//    }
//});

$('#pBannerPlanForm').on("click", function () {
    var token = $("#pBannerPlanFrmTkn input[type=hidden]").val();
    var html = "<form method=\"Post\" action=\"/pay360account/purchasecheckout\">";
    html += "<input name=\"__RequestVerificationToken\" type=\"hidden\" value=\"" + token + "\" />";
    html += "<input name=\"Id\" type=\"hidden\" value=\"1479\">";
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

$("#aFilterCallsTextData").on("click", function () {
    this.classList.add("active");
    $("#aFilterAll").removeClass("active");
    $("#anchorDataOnly").removeClass("active");
    //$(".dataOnlyPlan").hide();
    //$(".callsPlan").show();
    $(".dataOnlyPlan").hide();
    $(".callsPlan").hide();
    $(".callsPlan").fadeIn(300);
    ProductImpression(impressionArray_CallPlans);
});

$("#aFilterAll").on("click", function () {
    this.classList.add("active");
    $("#anchorDataOnly").removeClass("active");
    $("#aFilterCallsTextData").removeClass("active");
    $(".callsPlan").hide();
    $(".callsPlan").fadeIn(300);
    $(".dataOnlyPlan").fadeIn(300);
    ProductImpression(impressionArray_AllPlans);
    //$(".dataOnlyPlan").show();
    //$(".callsPlan").show();
});

$("#anchorDataOnly").on("click", function () {
    this.classList.add("active");
    $("#aFilterAll").removeClass("active");
    $("#aFilterCallsTextData").removeClass("active");
    //$(".dataOnlyPlan").show();
    //$(".callsPlan").hide();
    $(".callsPlan").hide();
    $(".dataOnlyPlan").hide();
    $(".dataOnlyPlan").fadeIn(500);
    ProductImpression(impressionArray_DataPlans);
});

$(".planCardSubmit").on("click", function () {
    var productId = $(this).data("productid");
    $("#frmPlanCard_" + productId).submit();
});

$(".plan-info label").click(function (event) {
    event.stopPropagation();
});