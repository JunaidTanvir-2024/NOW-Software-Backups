// JavaScript Document
$(document).ready(function () {

    'use strict';

    /************************************************************************************ SLICK SLIDER STARTS */

    $(document).ready(function () {
        //$('.js-slick').slick({
        //    autoplay: true,
        //    autoplaySpeed: 5000,
        //    dots: true,
        //    draggable: false,
        //    fade: true,
        //    speed: 1000
        //});

        /* $('.js-slick').on('beforeChange', function(event, slick, currentSlide, nextSlide) {
           $(slick.$slides).removeClass('is-animating');
         });
         
         $('.js-slick').on('afterChange', function(event, slick, currentSlide, nextSlide) {
           $(slick.$slides.get(currentSlide)).addClass('is-animating');
         });*/

    });

    /************************************************************************************ SLICK SLIDER ENDS */

    /************************************************************************************ TO TOP STARTS */

    $(window).scroll(function () {
        if ($(this).scrollTop() > 100) {
            $('.scrollup').fadeIn();
        } else {
            $('.scrollup').fadeOut();
        }
    });

    $('.scrollup').on("click", function () {
        $("html, body").animate({
            scrollTop: 0
        }, 600);
        return false;
    });


    /************************************************************************************ TO TOP ENDS */

    $(document).ready(function () {

        //$('.js-slick')
        //    .on('init', function (slick) {
        //        $('.js-slick').css("overflow", "visible");
        //    })
        //    .slick({
        //        autoplay: true,
        //        autoplaySpeed: 5000,
        //        dots: true,
        //        draggable: false,
        //        fade: true,
        //        focusOnSelect: true,
        //        lazyLoad: 'ondemand',
        //        speed: 1000
        //    });



        /*$('.js-slick').slick({
            autoplay: true,
            autoplaySpeed: 5000,
            dots: true,
            draggable: false,
            fade: true,
            speed: 1000
        });*/

        /* $('.js-slick').on('beforeChange', function(event, slick, currentSlide, nextSlide) {
           $(slick.$slides).removeClass('is-animating');
         });
         
         $('.js-slick').on('afterChange', function(event, slick, currentSlide, nextSlide) {
           $(slick.$slides.get(currentSlide)).addClass('is-animating');
         });*/

    });

}); //$(document).ready(function () {
$(document).ready(function () {
    $("header.icons").removeClass('white icons').addClass('header login sisu-style');
});

$("#cardTopup3,#cardTopup4").click(function (e) {
    $("#formCardTopup1").submit();
    e.preventDefault();
});

$('#chkQuickTopUp').change(function () {
    $('#hdnAutoTopUpEnabled').val(this.checked);
});
$('.spnAmountQuickTopUp').on("click", function () {
    $('#hdnFastTopUpId').val($(this).data("id"));
    $('#hdnFastTopUpAmount').val($(this).data("amount"));
});
function amountChange(amount, topUpId) {
    $('#hdnAmount').val(amount);
    $('#hdnTopUpId').val(topUpId);
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

//frmQuickTopUp btnQuickTopUpSubmit
$("#frmQuickTopUp").submit(function (e) {
  
    
    $("#btnGetPackagespinner").show();
    $("span.quickTopUpError").hide();
    e.preventDefault();
    $("#errorpin").hide();
    var PIN = $("#inpMsisdnQuickTopUp").val();
    var Auto;
    if (PIN.length > 0) {
        if (PIN.length == 10 || PIN.length == 11) {
            $("#btnFastRecharge").addClass("loading");
            var modelVerify = {};
            modelVerify.Number = PIN;

            $.ajax({
                url: "/Account/VerifyPIN",
                type: "POST",
                data: modelVerify,
                success: function (response) {
                    if (response.Message == "Success") {

                        var token = $("input[name=__RequestVerificationToken]").val();
                        var modelTopUp = {};
                        modelTopUp.msisdn = PIN;
                        modelTopUp.amount = $("#hdnFastTopUpAmount").val();
                        modelTopUp.AutoTopUpEnabled = $("#hdnAutoTopUpEnabled").val();
                        modelTopUp.TopUpId = $("#hdnFastTopUpId").val();

                        $.ajax({
                            url: "/pay360account/TopUpCheckoutTHCC",
                            type: "POST",
                            data: {
                                __RequestVerificationToken: token,
                                model: modelTopUp
                            },
                            success: function (response) {
                                $("#btnGetPackagespinner").hide();

                                if (response.Message === "Success") {
                                    window.location.href = response.Url;
                                }
                                else {
                                    var element = document.getElementById("pinerror");
                                    element.innerHTML = response.Message;
                                    $("#btnQuickTopUpSubmit").removeClass("loading");
                                }
                            },
                            error: function () {
                                $("#btnGetPackagespinner").hide();
                                $("#btnQuickTopUpSubmit").removeClass("loading");
                            }
                        });
                    }
                    else if (response.Message != "") {
                        $("#btnGetPackagespinner").hide();
                        $("span.quickTopUpError").show();
                        var element = document.getElementById("pinerror");
                        element.innerHTML = response.Message;
                        $("#btnQuickTopUpSubmit").removeClass("loading");
                    }
                    else {
                        $("#btnGetPackagespinner").hide();
                        $("#btnQuickTopUpSubmit").removeClass("loading");
                    }
                },
                error: function () {
                    $("#btnGetPackagespinner").hide();
                    alert("Error");
                    $("#btnQuickTopUpSubmit").removeClass("loading");
                }
            });
        }
        else {
            $("#btnGetPackagespinner").hide();
            document.getElementById("errorpin").innerHTML = "Please Enter a Valid PIN(Min 10,Max 11 Digits)";
            $("#errorpin").show();
        }
    }
    else {
        $("#btnGetPackagespinner").hide();
        return false;
        //document.getElementById("errorpin").innerHTML = "Please Enter a PIN.!";
        //$("#errorpin").css({
        //    fontSize: 15
        //});
        //$("#errorpin").show();
    }
});


//International TopUp



$('#phone').keydown(function () {
    $("#PhoneError").hide();
});

//$("#country-listbox").on("click", "li", function () {
//    alert("dsf");
//});
//GetPackage


var input = document.querySelector("#phone");
if (input !== null) {
    
    output = document.querySelector("output#");
    var iti = window.intlTelInput(input, {
        nationalMode: true,
        allowDropdown: true,
        //hiddenInput: "full_phone",
        //autoHideDialCode: false,
        // autoPlaceholder: "on",
        // dropdownContainer: null,
        //  excludeCountries: ["us"],
        //  formatOnDisplay: false,
        //  geoIpLookup: function(callback) {
        //    $.get("http://ipinfo.io", function() {}, "jsonp").always(function(resp) {
        //      var countryCode = (resp && resp.country) ? resp.country : "";
        //      callback(countryCode);
        //    });
        //  },
        //  hiddenInput: "full_number",
        //   initialCountry: "auto",
        //  localizedCountries: { 'de': 'Deutschland' },
        //  nationalMode: true,
        //  onlyCountries: ['us', 'gb', 'ch', 'ca', 'do'],
        //  placeholderNumberType: "MOBILE",
        //  preferredCountries: ['cn', 'jp'],
        //separateDialCode: true,
        // nationalMode: false,


        utilsScript: "~/CallingCardHomeCSSJS/build/js/utils.js",
    });
   
    var handleChange = function () {
        
        var text = (iti.isValidNumber()) ? iti.getNumber() : "Please enter a valid number";
        var textNode = document.createTextNode(text);
        output.innerHTML = text;
        //output.appendChild(textNode);
    };
    input.addEventListener('change', handleChange);
    input.addEventListener('keyup', handleChange);
    input.addEventListener('focus', handleChange);
}

var Countrycode;
//$(".country-list li").click(function () {
//    $(this).focus();
//    Countrycode = $(this).attr("data-country-code").toUpperCase();
//    alert(Countrycode);
//});

//$(".country-list li").click(function () {

//    Countrycode = $(this).attr("data-country-code").toUpperCase();
//    alert(Countrycode);
//});

//  
$("#btnGetPackage").click(function (e) {
    
    $("#PhoneError").hide();
    $("#GetPackagespinner").show();
    var telephoneNumber = $("#output").text();

    if ($('#phone').val() == '0' || $('#phone').val() == '' || $('#phone').val() == 'undefined' || $('#phone').val() == null) {
        $("#PhoneError").text("Number Is Undefined");
        $("#PhoneError").show();
        $("#GetPackagespinner").hide();
        return false;
        //if (Countrycode == undefined)
        //{
        //    $("#PhoneError").text("Country Code Is Undefined");
        //}
        //else if ($('#phone').val() == '0' || $('#phone').val() == '' || $('#phone').val() == 'undefined' || $('#phone').val() == null)
        //{
        //    $("#PhoneError").text("Number Is Undefined");
        //}
    }
    else {
        if (isNaN(telephoneNumber)) {
            var error = document.getElementById("PhoneError");
            error.innerHTML = telephoneNumber;
            $("#PhoneError").show();
            $("#GetPackagespinner").hide();
            return false;
        }
    }
    //var CountryCode111 = $("data-country-code").html();
    //var CountryCode = $(".selected-dial-code").html();
    // var Code = $(".selected-flag").attr("title");
    //// str.substr(6);
    //// var countrycode = Code.substr(Code.indexOf("+"));
    //$(".invalid-phone").hide();
    //$(".span-progress-1").show();
    //$("#air-time-spinner").show();
    //$("#SelectNumber").fadeOut();
    //$("#air-time-faq-div").fadeOut();
    $("#btnGetPackage").attr("disabled", true);
    $.ajax({
        type: "POST",
        url: "/account/GetTransferPackagesTHCC?telephoneNumber=" + $("#output").text() + "&countryCode=" + Countrycode,
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        success: function (result) {
            
            $("#btnGetPackage").attr("disabled", false);
            var json = $.parseJSON(result);
            if (json.errorCode == 0 && json.payload != null) {
                var tel = telephoneNumber;
                $("#Receiver").text(tel);
                buildPackageList(json.payload);
                setHiddenInputs(json.payload);
                $("#GetPackagespinner").hide();
                //alert("Success");

                $("#package-tab").addClass("nav-link active");
                $("#package").addClass("tab-pane fade show active");

                //$("#location-n-number-tab").removeClass("active");
                $("#location-n-number").removeClass("show active");

                //$("#package-tab").trigger('click');
                //$("#SelectPackage").fadeIn();
                //$("#step-number-2").addClass("step-number-completed");
                //$(".invalid-number").hide();
                //$(".intro-phone-number").hide();
                //$(".intro-select-package").show();
                //$("#air-time-faq-div").fadeIn();
                //$("#air-time-spinner").fadeOut();
                //$("#airfaq2").hide();
            }
            else {
                $("#GetPackagespinner").hide();
                $("#btnGetPackage").attr("disabled", false);
                $("#PhoneError").show();
                $("#PhoneError").text("International top up is unavailable for this number at this time. Please check back soon.");

                //$(".invalid-number").show();
                //$(".span-progress-1").hide();
                //$("#SelectNumber").fadeIn();
                //$("#air-time-faq-div").fadeIn();
                //$("#air-time-spinner").fadeOut();
            }

        },
        error: function () {
            $("#GetPackagespinner").hide();
            $("#btnGetPackage").attr("disabled", false);
            $("#PhoneError").show();
            $("#PhoneError").text("An Error Occured while Processing the Request.");
        }
    });
    //}
    e.preventDefault(); // avoid to execute the actual submit of the form.
});


function buildPackageList(payload) {

    var operator = payload.operators[0];
    $("#location").html("<img src='" + operator.iconUri + "' alt='operator icon'/> " + operator.country + " ");
    $(".OperaterCountry").html("<span class='text-center county-name'><img src='" + operator.iconUri + "' alt='operator icon'/> " + operator.country + " </span>");
    //var packageContainer = $(".transfer-package-container");
    //var operatorDiv = $(packageContainer).find(".transfer-network");
    //var countryDiv = $(packageContainer).find(".transfer-country");

    //var packagesDiv = $(packageContainer).find(".packages-inner");
    var packageListHtml = "";

    for (var ip = 0; ip < operator.products.length; ip++) {
        var selected = false;
        if (ip == (operator.products.length - 1))
            selected = true;
        packageListHtml += buildPackageHtml(operator.products[ip], selected, ip);
    }

    $(".Packagelist").html(packageListHtml);
    //$(operatorDiv).html("<img src='" + operator.iconUri + "' alt='operator icon'/>");

    //$(countryDiv).html(operator.country);
    //$(packagesDiv).html(packageListHtml);


    $(".transfer-package").click(function () {
        
        $('.transfer-package').removeClass("transfer-package-selected");
        $(this).addClass("transfer-package-selected");
        $("#PackageId").val($(this).data("id"));
        $("#Cost").val($(this).data("cost"));
        $("#ProductName").val($(this).data("name"));
        $("#Currency").val($(this).data("currency"));
        $("#TransferAmount").val($(this).data("transferred"));
        $(this).find(".air-radio").prop("checked", true);

        $("#Package").text($(this).data('cost'));
        $("#CostSummery").text($(this).data("name"));

    });

}
function buildPackageHtml(pkg, selected, ip) {

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

        $("#Package").text(pkg.receiverccy + pkg.product);
        $("#CostSummery").text(cost);
    }
    //<li>
    //    <input type="radio" name="optradio" id="1" value="option1">
    //        <label for="1">
    //            <div class="block text-center">
    //                <div class="package-name">LKR 100 <br><span>Package</span></div>
    //                    <div class="price" for="f-option">£0.80</div>
    //                </div>
    //                            </label>
    //                        </li>
    var i = ip;
    var html1 = '<li class="' + selClass + '" data-transferred="' + trans + '" data-id="' + pkg.product + '" data-cost="' + cost + '"data-currency=' + pkg.clientccy + '" data-name="' + pkg.receiverccy + pkg.product + '">' +
        '<input type="radio" class="air-radio" checked="checked" name="optradio" id="' + ip + '" value="option1">' +
        '<label for="' + ip + '">' +
        '<div class="block text-center">' +
        '<div class="package-name">' + pkg.receiverccy + " " + pkg.product +
        '<br><span>Package</span>' +
        '</div>' +
        '<div class="price" for="f-option">' + symbol + cost +
        '</div>' +
        '</div>' +
        '</label>' +
        '</li>';

    //var html1 = '<li>' +
    //    '<div class="block text-center">' +
    //    '<div class="package-name">' + pkg.receiverccy + " " + pkg.product +
    //    '<br><span>Package</span>' +
    //    '<div class="price" for="f-option">' + symbol + cost +
    //    '</div>' +
    //    '</div>' +
    //    '</li>';


    //var html = '<div class="' + selClass + '" data-transferred="' + trans + '" data-id="' + pkg.product + '" data-cost="' + cost + '"data-currency=' + pkg.clientccy + '" data-name="' + pkg.receiverccy + pkg.product + '">' +
    //    '<div class="air-radio-div">' +
    //    '<label class="air-radio-container">' +
    //    '<input type="radio" checked="checked" name="radio" class="air-radio">' +
    //    '<span class="checkmark"></span>' +
    //    '</label>' +
    //    '</div>' +
    //    '<div class="transfer-bundle">' + pkg.receiverccy + " " + pkg.product +
    //    ' Package</div>' +
    //    '<div class="cost">costs you '
    //    + symbol + cost +
    //    '</div>' +
    //    '</div>';

    return html1;

}
function setHiddenInputs(payload) {
    $("#TransactionReference").val(payload.operators[0].nowtelTransactionReference);
    $("#OperatorId").val(payload.operators[0].id);
    $("#ToMsisdn").val($("#phone").val());

}

//$(".transfer-package").click(function () {
//    
//    $('.transfer-package').removeClass("transfer-package-selected");
//    $(this).addClass("transfer-package-selected");
//    $("#PackageId").val($(this).data("id"));
//    $("#Cost").val($(this).data("cost"));
//    $("#ProductName").val($(this).data("name"));
//    $("#Currency").val($(this).data("currency"));
//    $("#TransferAmount").val($(this).data("transferred"));
//    $(this).find(".air-radio").prop("checked", true);

//});

$("#btnPackageSelection").click(function () {
    

    $("#package").removeClass("show active");
    //$("#package-tab").removeClass("active");
    $("#confirm-transfer-tab").addClass("nav-link active");
    $("#confirm-transfer").addClass("tab-pane fade show active");
    //$("#confirm-transfer-tab").trigger('click');
});

$("#btnPackageSelectionBack").click(function () {
    
    $("#package").removeClass("show active");
    $("#package-tab").removeClass("active");
    $("#location-n-number-tab").addClass("nav-link active");
    $("#location-n-number").addClass("tab-pane fade show active");
    //$("#location-n-number-tab").trigger('click');
});

$('#Password').keyup(function () {
    $("#LoginError").hide();
});
$("#btnConfirmTransferBack").click(function () {

    $("#confirm-transfer-tab").removeClass("active");
    $("#confirm-transfer").removeClass("show active");

    $("#package-tab").addClass("nav-link active");
    $("#package").addClass("tab-pane fade show active");

    //$("#package-tab").trigger('click');
});


$("#btnLogin").click(function () {
    
    $("#btnloginspinner").show();
    $("#LoginError").hide();
    var EmailAddress = $("#EmailAddress").val();
    var Password = $("#Password").val();
    var modelLogin = {};
    var token = $("input[name=__RequestVerificationToken]").val();
    modelLogin.EmailAddress = EmailAddress;
    modelLogin.Password = Password;
    if (Password.length >= 8) {
        $("#btnLogin").attr("disabled", true);
        $.ajax({
            url: "/Account/LoginTHCC",
            type: "POST",
            data: {
                __RequestVerificationToken: token,
                model: modelLogin
            },
            success: function (responseModel) {
                
                $("#logindiv").show();
                $('#mRegister').hide();
                if (responseModel.Url != null) {
                    if (responseModel.Url == "/account/myaccount/") {
                        
                        var obj = jQuery.parseJSON(responseModel.Message);
                        var len = obj.ProductCodes.length;
                        $("#EmailAddressText").text(obj.FullName.Email);
                        $("#EmailAddress").val(obj.FullName.Email);
                        for (i = 0; i < len; i++) {
                            if (obj.ProductCodes[i].ProductCode == "THM") {
                                $("#Msisdn").val(obj.ProductCodes[i].Reference);
                                $("#MsisdnText").text(obj.ProductCodes[i].Reference);
                            }
                        }
                        $("#login-signup").removeClass("show active");
                        //$("#login-signup-tab").removeClass("active");
                        $("#location-n-number-tab").addClass("nav-link active");
                        $("#location-n-number").addClass("tab-pane fade show active");
                        //$("#location-n-number-tab").trigger('click');
                    }
                    else {
                        window.location.href = responseModel.Url;
                    }
                }
                else if (responseModel.Message != null) {
                    $("#LoginError").show();
                    var element = document.getElementById("LoginError");
                    element.innerHTML = responseModel.Message;
                }
                $("#btnloginspinner").hide();
                $("#btnLogin").attr("disabled", false);
            },
            error: function (e) {
                $("#btnLogin").attr("disabled", false);
                $("#btnloginspinner").hide();
                $("#btnQuickTopUpSubmit").removeClass("loading");
                $("#LoginError").show();
                var element = document.getElementById("LoginError");
                element.innerHTML = "An Error Occured While Processing the Request ,Please Try Again.";
            }
        });
    }
    else {
        $("#btnloginspinner").hide();
        $("#LoginError").show();
        var element = document.getElementById("LoginError");
        element.innerHTML = "Password Must Consists of 8 Characters.";
    }
});




$("input[name='PaymentMethodOther'], #btnPay360PayWithCardExisting, #btnPay360PayWithCardNew").click(function () {
    
    var provider = $(this).data("provider");
    var pay360Formvalidator = $('#CheckoutFormPay360').validate();

    if (!CheckName(provider)) {
        $("#NameError").show();
    }
    else {
        if (provider == "Pay360") { //Pay360
            var thisButton = $(this);
            if ((thisButton.val() === "Card" || thisButton.val() === "NewCard" || thisButton.val() === "Paypal") && $('#CheckoutFormPay360').valid() !== true) {
                return;
            }
            else if (thisButton.val() == "Paypal") {
                if ($("#addressFields").is(':visible') == false && $("#divChooseAddressPay360").is(':visible') == false) {
                    //$('#lookupLink').trigger("click");
                    //return;
                }
                else if ($('#CheckoutFormPay360').valid() !== true) {
                    return;
                }
            }

            $('.modal').attr('provider', 'Pay360');
            $("#NameErrorPay360").hide();
            if (thisButton.val() === "NewCard" || thisButton.val() === "CreditNew" || thisButton.val() === "PaypalNew") {
                //New Card 
                if ($('#TermsConditionsPay360New').is(':checked')) {
                    $("#TermsErrorPay360New").hide();
                    $("#HiddenPaymentMethod").val($(this).val());
                    pay360Formvalidator.resetForm();
                    pay360Formvalidator.cancelSubmit = true;
                    $(".modal").show();
                }
                else {
                    $('input[name="PaymentMethodCard"]').prop('checked', false);
                    $('input[name="PaymentMethodOther"]').prop('checked', false);
                    $("#TermsErrorPay360New").show();
                }
            }
            else if (thisButton.val() === "Paypal") {
                //Paypal 
                if ($('#TermsConditionsPay360Paypal').is(':checked')) {
                    $("#TermsErrorPay360Paypal").hide();
                    $("#HiddenPaymentMethod").val($(this).val());
                    $(".modal").show();
                }
                else {
                    $('input[name="PaymentMethodCard"]').prop('checked', false);
                    $('input[name="PaymentMethodOther"]').prop('checked', false);
                    $("#TermsErrorPay360Paypal").show();
                }
            }
            else {
                //Existing Card selected
                if ($('#TermsConditionsPay360Existing').is(':checked')) {
                    $("#TermsErrorPay360Existing").hide();
                    $("#HiddenPaymentMethod").val($(this).val());
                    pay360Formvalidator.resetForm();
                    pay360Formvalidator.cancelSubmit = true;
                    $(".modal").show();
                }
                else {
                    $('input[name="PaymentMethodCard"]').prop('checked', false);
                    $('input[name="PaymentMethodOther"]').prop('checked', false);
                    $("#TermsErrorPay360Existing").show();
                }
            }

        }
        else { //MiPay
            $("#NameError").hide();
            if ($('#TermsConditions').is(':checked')) {
                $("#TermsError").hide();
                $("#HiddenPaymentMethod").val($(this).val());
                $(".modal").show();
            }
            else {
                $('input[name="PaymentMethodCard"]').prop('checked', false);
                $('input[name="PaymentMethodOther"]').prop('checked', false);
                $("#TermsError").show();
            }
        }
    }


});

//New Checkout Page js
$("button[name='PaymentMethodOther']").click(function () {

    var provider = $(this).data("provider");
    var pay360Formvalidator = $('#CheckoutFormPay360').validate();

    if (!CheckName(provider)) {
        $("#NameError").show();
    }
    else {
        if (provider == "Pay360") { //Pay360
            var thisButton = $(this);
            if ((thisButton.val() === "Card" || thisButton.val() === "NewCard" || thisButton.val() === "Paypal") && $('#CheckoutFormPay360').valid() !== true) {
                return;
            }
            else if ($("#inputEmailAddress").is(":visible") === true) {
                if ($('#CheckoutFormPay360').valid() !== true) {
                    return;
                }
                //if (isValidEmailAddress($("#inputEmailAddress").val()) == false) {
                //    return;
                //}
            }
            else if (thisButton.val() == "Paypal") {
                if ($("#addressFields").is(':visible') == false && $("#divChooseAddressPay360").is(':visible') == false) {
                    //$('#lookupLink').trigger("click");
                    //return;
                }
                else if ($('#CheckoutFormPay360').valid() !== true) {
                    return;
                }
            }

            $('.modal').attr('provider', 'Pay360');
            $("#NameErrorPay360").hide();
            if (thisButton.val() === "Card" || thisButton.val() === "CreditNew" || thisButton.val() === "PaypalNew") {
                //New Card 
                if ($('#customCheck1').is(':checked') || $('#IsRegistered').val() === "true") {

                    $("#HiddenPaymentMethod").val($(this).val());
                    pay360Formvalidator.resetForm();
                    pay360Formvalidator.cancelSubmit = true;
                    $(".modal").show();
                }
                else {
                    $("#TermsError").show();
                    return;
                }
            }
            else if (thisButton.val() === "Paypal") {
                //Paypal 
                if ($('#TermsConditionsPay360Paypal').is(':checked')) {
                    $("#TermsErrorPay360Paypal").hide();
                    $("#HiddenPaymentMethod").val($(this).val());
                    $(".modal").show();
                }
                else {
                    $('input[name="PaymentMethodCard"]').prop('checked', false);
                    $('input[name="PaymentMethodOther"]').prop('checked', false);
                }
            }
            else {
                //Existing Card selected
                if ($('#TermsConditionsPay360Existing').is(':checked')) {
                    $("#TermsErrorPay360Existing").hide();
                    $("#HiddenPaymentMethod").val($(this).val());
                    pay360Formvalidator.resetForm();
                    pay360Formvalidator.cancelSubmit = true;
                    $(".modal").show();
                }
                else {
                    $('input[name="PaymentMethodCard"]').prop('checked', false);
                    $('input[name="PaymentMethodOther"]').prop('checked', false);
                    //$("#TermsErrorPay360Existing").show();
                }
            }

        }
    }
});

$('#chkQuickTopUp').change(function () {
    $('#hdnAutoTopUpEnabled').val(this.checked);
});

$("#aQuickTopUp").on("click", function () {
    $("#frmQuickTopUp").submit();
});

$("#OKButton").on("click", function () {
    $(".modal").hide();
    var provider = $('.modal').attr('provider');
    if (provider == "Pay360") {
        $("#CheckoutFormPay360").submit();
    }
    else {
        $("#CheckoutForm").submit();
    }
});

function CheckName(provider) {
    if (provider == "Pay360")
        return true;
    if ($("#FirstName").val().trim() == "First Name" || $("#LastName").val().trim() == "Last Name") {
        return false;
    }
    return true;
}

$("#CancelButton,#CloseButton").on("click", function () {
    $('input[name="PaymentMethodCard"]').prop('checked', false);
    $('input[name="PaymentMethodOther"]').prop('checked', false);
    $(".modal").hide();
});

$("input[name='Pay360PaymentMethodExistingCards']").click(function () {
    
    if ($("#new-card").prop("checked")) {
        $("#btnPay360Pay").val("Card");
    }
    else if ($("#aPay360PayWithPaypal").prop("checked")) {
        $("#btnPay360Pay").val("Paypal");
    }
    else {
        $("#btnPay360Pay").val("Card");
    }
    var provider = $(this).data("provider");
    $('#HiddenCardId').val(this.value);
    if (provider == "Pay360" && $(this).attr("id") == "new-card") {
        $("#divPay360CardDetails").show();
        $("#divPay360CVVAndTermsExistingCard").hide();
        $("#divPay360PayByCard").hide();
        $("#inputPay360SecurityCode").removeAttr("required");
        $("#inputPay360SecurityCode").attr("name", "");
        //$("#btnPay360Pay").val("Card");

        $("#inputPay360SecurityCodeNewCard").attr("required", "required");
        $("#inputPay360NameOnCard").attr("required", "required");
        $("#inputPay360CardNumber").attr("required", "required");

        $("#Credit").val("CreditNew");
    }
    else {
        $("#inputPay360SecurityCode").attr("name", "SecurityCode");

        $("#divPay360CardDetails").hide();
        $("#divPay360CVVAndTermsExistingCard").show();
        $("#divPay360PayByCard").show();
        $("#inputPay360SecurityCode").attr("required", "required");
        //$("#divPay360PaywithPaypal").show();

        $("#inputPay360NameOnCard").removeAttr("required");
        $("#inputPay360CardNumber").removeAttr("required");
        $("#inputPay360SecurityCodeNewCard").removeAttr("required");

        $("#Credit").val("CreditExisting");

    }
});

$("input[name='PaymentMethodCard']").click(function () {

    var provider = $(this).data("provider");


    var id = $(this).data("id");
    if (!id)
        id = "0";

    if (provider == "Pay360" && $(this).attr("id") == "new-card") {
        $(".card-details").show();
        return;
    }


    if (!CheckName(provider)) {
        $("#NameError").show();
    }
    else {
        $("#NameError").hide();


        if ($('#TermsConditions').is(':checked')) {
            $("#TermsError").hide();
            $("#HiddenPaymentMethod").val("Card");
            $("#HiddenCardId").val(id);
            $(".modal").show();
        }
        else {
            $('input[name="PaymentMethodCard"]').prop('checked', false);
            $('input[name="PaymentMethodOther"]').prop('checked', false);
            $("#TermsError").show();
        }
    }

});

$("#btnPay360Pay").on("click", function () {
    
    if ($('#IsRegistered').val() === "true") {
        var productType = $("#hdnPay360ProductType").val();
        if (productType === "Bundle" || productType === "Basket") {
            $("#CheckoutFormPay360").prop("action", $("#hdnPay360PurchaseUrl").val());
        }
        else {
            $("#CheckoutFormPay360").prop("action", $("#hdnPay360TopUpUrl").val());
        }
    }

});

$("#aPay360PayWithPaypal").on("click", function () {
    $("#CheckoutFormPay360").prop("action", $("#hdnPayPalStartPaymentpUrl").val());
    if ($("#aPay360PayWithPaypal").prop("checked")) {
        $("#btnPay360Pay").val("PaypalNew");
    }
    else {
        $("#btnPay360Pay").val("Card");
    }
});



$("#aPay360PayWithCard").on("click", function () {
    var a = 1;
    var productType = $("#hdnPay360ProductType").val();
    if (productType === "Bundle" || productType === "Basket") {
        $("#CheckoutFormPay360").prop("action", $("#hdnPay360PurchaseUrl").val());
    }
    else {
        $("#CheckoutFormPay360").prop("action", $("#hdnPay360TopUpUrl").val());
    }
});

$("#btnPayWithCredit").on("click", function () {
    $("#CheckoutFormPay360").prop("action", $("#hdnPay360PurchaseUrl").val());
});
$("#new-card").on("click", function () {

    if ($("#new-card").prop("checked")) {
        $("#btnPay360Pay").val("Card");
        var productType = $("#hdnPay360ProductType").val();
        if (productType === "Bundle" || productType === "Basket") {
            $("#CheckoutFormPay360").prop("action", $("#hdnPay360PurchaseUrl").val());
        }
        else {
            $("#CheckoutFormPay360").prop("action", $("#hdnPay360TopUpUrl").val());
        }
    }
    else {
        $("#btnPay360Pay").val("PaypalNew");
    }
});

$(".imgCallingCardsHome").on("click", function () {
    var productId = $(this).data("productid");
    $("#frmCCPurchaseHome_" + productId).submit();
});



$('.radioCC').change(function () {
    var price = $(this).data("price");
    $("#spnOrdertotal").text(price);
    $("#hdnCCPrice").val(price);
    $("#divCCName").text("Calling Card " + price);
    $("#spnCCPrice").text(price);
    $("#spnTotalModalTHCC").text(price);
});

/**
     * Sets the total displayed in the checkout button
     * 
     */
function setCheckoutTotal() {
    $('.amounts [type=radio]').each(function () {
        if ($(this).is(':checked')) {
            $('.total').text($(this).attr('value'));
            $('input[name=Amount]').val($(this).attr('value'));
        }
    });
}


// On selecting top up amount, update basket property in the cookie
$('.amount-radio').change(function () {
    setCheckoutTotal();
});