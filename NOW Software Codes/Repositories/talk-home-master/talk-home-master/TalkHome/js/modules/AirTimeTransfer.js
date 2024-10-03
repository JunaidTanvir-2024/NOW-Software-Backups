/**
 * Defines the functionalities for the Basket.
 * 
 * @author micheled
 * Created on 15.07.2017
 * 
 */
'use strict';
var AirTimeTransfer = function () {

    function progress(selector) {
        $(this).data("origWidth", $(this).width()).width(0).animate({width: $(this).data("origWidth")}, 1200);
    }

    function validatePhone() {
       var phone = $("#Number").val();
       if (!phone)
           return false;
       return $.isNumeric(phone);
    }

    function validatePackage() {
        var packageId = $("#PackageId").val();
        if (packageId != null && packageId != "")
            return true;
        else
            return false;
    }

  
    function setHiddenInputs(payload) {
        $("#TransactionReference").val(payload.operators[0].nowtelTransactionReference);
        $("#OperatorId").val(payload.operators[0].id);
        $("#ToMsisdn").val($("#Number").val());

    }


    var currency_symbols = {
        'USD': '$', // US Dollar
        'EUR': '€', // Euro
        'CRC': '₡', // Costa Rican Colón
        'GBP': '£', // British Pound Sterling
        'ILS': '₪', // Israeli New Sheqel
        'INR': '₹', // Indian Rupee
        'JPY': '¥', // Japanese Yen
        'KRW': '₩', // South Korean Won
        'NGN': '₦', // Nigerian Naira
        'PHP': '₱', // Philippine Peso
        'PLN': 'zł', // Polish Zloty
        'PYG': '₲', // Paraguayan Guarani
        'THB': '฿', // Thai Baht
        'UAH': '₴', // Ukrainian Hryvnia
        'VND': '₫', // Vietnamese Dong
    };

 
    function buildPackageHtml(pkg,selected) {
       
        var symbol = "&pound;";
        var selClass = "transfer-package";

        /*
        if (currency_symbols[pkg.clientccy] !== undefined) {
            symbol = currency_symbols[pkg.clientccy];
        }
        */

        var buys = "costs you " + pkg.receiverccy + " " + pkg.product;
        var cost = Number(pkg.itemPriceClientccy).toFixed(2);
        var trans = pkg.product + " " + pkg.receiverccy;

        if (selected) {
            selClass += " transfer-package-selected";

            $("#PackageId").val(pkg.product);
            $("#Cost").val(cost);
            $("#ProductName").val(pkg.receiverccy + pkg.product);
            $("#Currency").val(pkg.clientccy);
            $("#TransferAmount").val(trans);
        }


        var html = '<div class="' + selClass + '" data-transferred="' + trans + '" data-id="' + pkg.product + '" data-cost="' + cost + '"data-currency=' + pkg.clientccy + '" data-name="' + pkg.receiverccy + pkg.product + '">' +
        '<div class="air-radio-div">' +
        '<label class="air-radio-container">' +
        '<input type="radio" checked="checked" name="radio" class="air-radio">' +
        '<span class="checkmark"></span>' +
        '</label>' +
        '</div>' +
        '<div class="transfer-bundle">' + pkg.receiverccy + " " + pkg.product +
        ' Package</div>' +
        '<div class="cost">costs you '
          + symbol + cost +
        '</div>' +
        '</div>';

    return html;

    }

    function buildPackageList(payload) {

        var packageContainer = $(".transfer-package-container");
        var operatorDiv = $(packageContainer).find(".transfer-network");
        var countryDiv = $(packageContainer).find(".transfer-country");
        
        var packagesDiv = $(packageContainer).find(".packages-inner");
        var packageListHtml = "";
        var operator = payload.operators[0];
        for (var ip = 0; ip < operator.products.length; ip++) {
            var selected = false;
            if (ip == (operator.products.length - 1))
                selected = true;
            packageListHtml += buildPackageHtml(operator.products[ip],selected);
        }

        $(operatorDiv).html("<img src='" + operator.iconUri + "' alt='operator icon'/>");
      
        $(countryDiv).html(operator.country);
        $(packagesDiv).html(packageListHtml);


        $(".transfer-package").click(function () {
            $('.transfer-package').removeClass("transfer-package-selected");
            $(this).addClass("transfer-package-selected");
            $("#PackageId").val($(this).data("id"));
            $("#Cost").val($(this).data("cost"));
            $("#ProductName").val($(this).data("name"));
            $("#Currency").val($(this).data("currency"));
            $("#TransferAmount").val($(this).data("transferred"));
            $(this).find(".air-radio").prop("checked", true);

        });

    }


    $("#GetPackagesForm").submit(function (e) {

        $(".invalid-phone").hide();
        $(".span-progress-1").show();
        $("#air-time-spinner").show();
        $("#SelectNumber").fadeOut();
        $("#air-time-faq-div").fadeOut();
     
        $.ajax({
            type: "POST",
            url: "/account/gettransferpackages?telephoneNumber=" + $("#Number").val() + "&countryCode=" + $("#transferCountry").val() ,
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=utf-8',
            success: function (result) {
                var json = $.parseJSON(result);
                if (json.errorCode == 0 && json.payload != null) {
                    buildPackageList(json.payload);
                    setHiddenInputs(json.payload);
                    $("#SelectPackage").fadeIn();
                    $("#step-number-2").addClass("step-number-completed");
                    $(".invalid-number").hide();
                    $(".intro-phone-number").hide();
                    $(".intro-select-package").show();
                    $("#air-time-faq-div").fadeIn();
                    $("#air-time-spinner").fadeOut();
                    $("#airfaq2").hide();
                }
                else {
                    $(".invalid-number").show();
                    $(".span-progress-1").hide();
                    $("#SelectNumber").fadeIn();
                    $("#air-time-faq-div").fadeIn();
                    $("#air-time-spinner").fadeOut();
                }

            }
        });

        e.preventDefault(); // avoid to execute the actual submit of the form.
    });


    $("#selectPackageButton").click(function () {

        if (validatePackage()) {
            $(".invalid-package").hide();
            $(".span-progress-2").show();
            $("#step-number-3").addClass("step-number-completed");
            $("#AddressAndMessage").fadeIn();
            $("#SelectPackage").fadeOut();
            $(".intro-messages").show();
            $(".intro-select-package").hide();
            $("#airfaq3").hide();

        }
        else {
            $(".invalid-package").show();
        }

    });

    $("#gotoPayment").click(function () {
        $("#TransferForm").valid();
    });

    $("#backToPackages").click(function () {
        $("#AddressAndMessage").fadeOut();
        $("#SelectPackage").fadeIn();
        $(".span-progress-2").hide();
        $("#step-number-3").removeClass("step-number-completed");
        $(".intro-messages").hide();
        $(".intro-select-package").show();
        $("#airfaq3").show();
    });

    $("#backToNumber").click(function () {
        $("#SelectPackage").fadeOut();
        $("#SelectNumber").fadeIn();
        $(".span-progress-1").hide();
        $("#step-number-2").removeClass("step-number-completed");
        $(".intro-phone-number").show();
        $(".intro-select-package").hide();
        $("#airfaq2").show();
    });

    $("#showLogin").click(function () {
        $("#CreditLogin").fadeIn();
        $("#AddressAndMessage").fadeOut();

    });

    if ($('#transferCountry').length > 0) {
        $('#transferCountry').select2({
            placeholder: 'Select an option',
            closeOnSelect: true
        });
    }
    
    /*
    $('#transferCountry').on('select2:select', function (e) {
        var data = e.params.data;
        $("#country-pic").animate({ left: 300, opacity: "show" }, 1500);
        
    });
    */

};
