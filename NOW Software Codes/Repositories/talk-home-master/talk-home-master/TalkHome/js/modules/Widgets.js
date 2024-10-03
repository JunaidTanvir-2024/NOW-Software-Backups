/**
 * 
 * Runtime support for widgets
 * @author micheled
 * Created on 23.07.2017
 * 
 */
'use strict';
var Widgets = function () {

    
    function cV(){
        return ""; //
    }
    var val = Number($("#guageContainer").data("usage"));

    var typw = Number($("#guageContainer").data("usage"));


    var divHeight = $('.static-box').height(); 
    $('.bi-box').css('minHeight', divHeight+'px');


    $(window).resize(function () {
        var divHeight = $('.static-box').height();
        $('.bi-box').css('minHeight', divHeight + 'px');
    });

    /* Full feature box link
        $(".feature-box").on("click", function (e) {
            var t = $(this).data("target");
            window.location.href = t;

        });
    */ 


    /***Transfer support ***/

    $(".show-all-transfers").on("click", function (e) {
        $("#transfer-promotion-widget").slideDown();
        $('html, body').animate({
            scrollTop: $("#transfer-promotion-widget").offset().top
        }, 300);
    });

    $(".promo-box").on("click", function (e) {
        $(this).siblings('.details').show();
        $(this).hide();
    });

    $(".promo-box.details").on("click", function (e) {
        $(this).siblings('.promo-box').show();
        $(this).hide();
    });


    if ($('#promoCountries').length > 0) {
        $('#promoCountries').select2({
            placeholder: 'Select an option',
            closeOnSelect: true
        });
    }
    
    if ($('#promoCategories').length > 0) {
        $('#promoCategories').select2({
            placeholder: 'Select an option',
            closeOnSelect: true
        });
    }
    
    if ($('#plansDropdown').length > 0) {
        $('#plansDropdown').select2({
            placeholder: 'Select an option',
            closeOnSelect: true
        });
    }
  
    $('#plansDropdown').change(function () {
        var cplan = $(this).find(':selected').attr('value');
        var ulist = $(this).find(':selected').data('upgrade');
        var upArray = ulist.split(",");

        $(".plans-full-list").each(function () {
            var cId = $(this).attr("id");
            if (upArray.indexOf(cId) > - 1) {
                $(this).show();
            }
            else
                $(this).hide();
        });
        $(this).select2('close');
    });

    
    $(".transfer-promotion").each(function () {
        if ($(this).data("country") == $("#HiddenTopCountry").val())
            $(this).show();
    });


    $('#promoCountries').change(function () {
        var country = $(this).find(':selected').attr('value');
        var ct = 0;
        $(".transfer-promotion").each(function () {
            if ($(this).data("country") == country){
                $(this).show();
                if (ct == 0) {
                    $("#promo-box-container").html($(this).html());
                    $(".promo-box").on("click", function (e) {
                        $(this).siblings('.details').show();
                        $(this).hide();
                    });

                    $(".promo-box.details").on("click", function (e) {
                        $(this).siblings('.promo-box').show();
                        $(this).hide();
                    });
                }
                ct++;
            }
            else
                $(this).hide();
        });
        $(this).select2('close');
    });


    $('#promoCategories').change(function () {
        var category = $(this).find(':selected').attr('value');
        var country = $("#promoCountries").find(':selected').attr('value');
        var ct = 0;
        $(".transfer-promotion").each(function () {
            if (($(this).data("category") == category || category== "All" ) && $(this).data("country") == country) {
                $(this).show();
                if (ct == 0) {
                    $("#promo-box-container").html($(this).html());
                    $(".promo-box").on("click", function (e) {
                        $(this).siblings('.details').show();
                        $(this).hide();
                    });

                    $(".promo-box.details").on("click", function (e) {
                        $(this).siblings('.promo-box').show();
                        $(this).hide();
                    });
                }
                ct++;
            }
            else
                $(this).hide();
        });
        $(this).select2('close');
    });


    $("#phoneTopupBannerButton").click(function (e) {
        $("#formPhoneTopup").submit();
        e.preventDefault();
    });



    $("#widgetTopUp-THCC,#widgetTopUp-THCC1").click(function (e) {
        $("#formWidgetTopup-THCC").submit();
        e.preventDefault();
    });

    
};