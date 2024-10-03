/**
 * Manages UI events and methods for editing text into <input> tags
 *
 * @author micheled
 * Created on 20 Jul 2017
 * 
 */
'use strict';
var billingcountryselected = false;
var checkoutlookup = false;
var isdisabled = false;
var Checkout = function () {

    console.log("checkoutjs 10082018-1 loaded");

    var utils = new Utils,
        emptyAndAddURI = '/basket/api/empty-add-top-up',
        clearUri = '/basket/api/clear-top-up',
        productCode = '',
        Unreg = "0";

    // Private methods

    /**
     * Shows the edit details inputs
     * 
     */
    function editYourDetails() {
        $('[name=PostalCode]').val('');
        $('#editName, .editAddress').show();
        $('.valid-address').hide();
    }

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

    /**
     * Clears any top up
     * 
     */
    function clearTopUp() {
        utils.Post(clearUri, args);
    }

    /**
     * Updates customer's cookie given the default top up
     * 
     * @param {jQueryObject} radio The default product
     * 
     */
    function setTopUp(radio) {

        var id = radio.attr('data-id');
        args.data = { 'Id': id, 'empty': true };

        utils.Post(emptyAndAddURI, args);
    }

    /**
     * Updates UI when the page loads or the user selects a different top up
     * 
     */
    function updateUI() {

        // Find any active product and set the productCode
        $('[name=productCode]').each(function () {
            if ($(this).is(':checked'))
                productCode = $(this).val();
        });

        if (productCode !== '') {
            $('.amounts').hide();
            $('[data-id=' + productCode + '].amounts').show();
            $('.continue').show();
            setTopUp($('[data-id=' + productCode + '].amounts .amount-radio.current'));
        } else {
            clearTopUp();
        }
    }



    /**
     * Works out the credit Id to add to Calling Cards orders and disaplys the correct button and form `action`
     *
     */
    function callingCardCreditOrder() {

        var credit = '';

        function updateUI() {
            if (credit == 0) { // No credit. Process a Calling card mail order
                $('#creditOrder').hide();
                $('#mailOrder').show();
                $('#CCorderForm').attr('action', '/account/mailorder');
            } else {
                $('#creditOrder').show();
                $('#mailOrder').hide();
                $('#CCorderForm').attr('action', '/account/callingcardcreditorder');

                var value = '';

                function getCheckoutValue() {
                    $('#credit option').each(function () {
                        if ($(this).is(':checked')) {
                            value = $(this).text();
                        }
                    });
                };

                getCheckoutValue();

                $('.checkout-total').text(value);
            }
        }

        // Read the selected credit Id from the dropdown
        function readCredit() {
            $('#credit option').each(function () {
                if ($(this).is(':checked')) {
                    credit = $(this).attr('value');
                }
            });

            updateUI();
        }

        // Ui Events
        readCredit();

        $('#credit').change(function () {
            readCredit();
        });
    }

    // UI events

    // On top up page, on selecting product auto-submit the form
    if ($('#topUpPage').length > 0) {
        $('[name=productCode]').change(function () {
            $('#productSelectionForm').submit();
        });
    }

    // On selecting product, update UI
    if ($('#checkoutPage').length > 0) {
        $('[name=productCode]').change(function () {
            updateUI();
        });
    }

    // On selecting top up amount, update basket property in the cookie
    $('.amount-radio').change(function () {
        setTopUp($(this));
        setCheckoutTotal();
    });

    // On checkout page, dynamically set the total on the button
    if ($('#checkoutPage').length > 0)
        setCheckoutTotal();

    // Edit details at checkout
    $('#editYourDetails').click(function () {
        $('[name=Unreg]').val("1"); // Process payment as Unreg customer
        Unreg = "1";
        editYourDetails();
    });

    // Show available cards
    /*
    $('[data-show=cards]').click(function () {
        if ($('.cards li').length > 0 && Unreg === "0")
            $('[data-show-elem=cards]').slideDown(250);
    });

    // Hide available cards
    $('[data-hide=cards]').click(function () {
        if ($('.cards li').length > 0)
            $('[data-show-elem=cards]').hide();
    });
    */

    // Order a Calling card with credit
    if ($('.calling-card-credit').length > 0) {
        callingCardCreditOrder();
    }

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

        //if (!billingcountryselected) {
        //    $("#billingcountryselectederror").show();
        //} else {
        //    $("#billingcountryselectederror").hide();
        //}



        var provider = $(this).data("provider");
        var pay360Formvalidator = $('#CheckoutFormPay360').validate();
        var thisButton = $(this);

        if (thisButton.val() != "PaypalNew" && thisButton.val() != "CreditNew") {
            if (!checkoutlookup && $("input[name='Pay360PaymentMethodExistingCards']:checked").val() == "Card") {
                $("#CheckoutlookupLink").trigger('click');
                return;
            }
        }
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


    $("#inputPay360Security").focus(function () {
        $("#inputPay360Security").attr("required");
        $("#inputPay360Security").attr("name", "SecurityCode");
    })

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
            $("#deliveryAddress").show();
            $("#divPay360CVVAndTermsExistingCard").hide();
            $("#divPay360PayByCard").hide();
            $("#inputPay360SecurityCode").removeAttr("required");
            $("#inputPay360SecurityCode").attr("name", "");

            //Checkout Code OF loggedin user
            $("#inputPay360Security").removeAttr("required");
            $("#inputPay360Security").attr("name", "");


            //$("#btnPay360Pay").val("Card");

            $("#inputPay360SecurityCodeNewCard").attr("required", "required");
            $("#inputPay360SecurityCodeNewCard").attr("name", "SecurityCode");
            $("#inputPay360NameOnCard").attr("required", "required");
            $("#inputPay360CardNumber").attr("required", "required");


            $("#billingcountry").attr("required", "required");
            $("#postalCode").attr("required", "required");
            $("#line1").attr("required", "required");

            $("#Credit").val("CreditNew");
            checkoutlookup = false;

        }
        else {
            $("#inputPay360Security").attr("name", "SecurityCode");
            $("#inputPay360Security").attr("required", "required");

            $("#divPay360CardDetails").hide();
            $("#deliveryAddress").hide();
            $("#divPay360CVVAndTermsExistingCard").show();
            $("#divPay360PayByCard").show();
            //$("#divPay360PaywithPaypal").show();

            $("#inputPay360NameOnCard").removeAttr("required");
            $("#inputPay360CardNumber").removeAttr("required");
            $("#inputPay360SecurityCodeNewCard").removeAttr("required");
            $("#inputPay360SecurityCodeNewCard").attr("name", "");

            $("#billingcountry").removeAttr("required");
            $("#postalCode").removeAttr("required");
            $("#line1").removeAttr("required");
            checkoutlookup = true;

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


    if ($("#SimLandingPage").length > 0) {
        $("#OrderSimForm :input").prop("disabled", true);
    }

    $("#CheckAppNumberForm").submit(function (e) {

        $("#ThaMsisdn").val($("#AppNumber").val());
        $.ajax({
            type: "POST",
            url: "/account/checkappnumber?AppNumber=" + $("#AppNumber").val(),
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=utf-8',
            success: function (result) {
                var json = $.parseJSON(result);
                if (json.errorCode == 0 && json.isApplicable) {
                    $("#OrderSimForm :input").prop("disabled", false);

                    $(".is-applicable").show();
                    $(".not-applicable").hide();
                }
                else {
                    $("#OrderSimForm :input").prop("disabled", true);
                    $(".is-applicable").hide();
                    $(".not-applicable").show();
                }

            }
        });

        e.preventDefault(); // avoid to execute the actual submit of the form.
    });

    $("#inpMsisdnQuickTopUp").on("change", function () {
        $("span.quickTopUpError").hide();
    });

    $("#inpMsisdnQuickTopUp").focusout(function () {
        $("span.quickTopUpError").hide();
    });

    $("#aPay360PayWithCard").on("click", function () {

        var a = 1;
        var productType = $("#hdnPay360ProductType").val();
        if (productType === "Bundle" || productType === "Basket") {
            $("#CheckoutFormPay360").prop("action", $("#hdnPay360PurchaseUrl").val());
        }
        else if (productType === "CreditSimOrder") {
            $("#CheckoutFormPay360").prop("action", $("#hdnPay360CreditSimPaymentUrl").val());
        }
        else {
            $("#CheckoutFormPay360").prop("action", $("#hdnPay360TopUpUrl").val());
        }
        $("#payPalAutoMessage").hide();
    });

    $("#aPay360PayWithPaypal").on("click", function () {

        var IsPay360PayPalStartPayment = $("#IsPay360PayPalPayment").val();
        if (IsPay360PayPalStartPayment == 'true') {
            $("#CheckoutFormPay360").prop("action", "/payPal/pay369PayPalStartPayment");
        } else {
            $("#CheckoutFormPay360").prop("action", $("#hdnPayPalStartPaymentpUrl").val());
        }
        if ($("#aPay360PayWithPaypal").prop("checked")) {
            $("#btnPay360Pay").val("PaypalNew");
        }
        else {
            $("#btnPay360Pay").val("Card");
        }

        if ($("#AutoTopUpEnabled").prop("checked") == true) {
            $("#payPalAutoMessage").show();
            $("#AutoTopUpEnabled").prop("checked", false);
        }
        else if ($("#chkQuickTopUp").prop("checked") == true) {
            $("#payPalAutoMessage").show();
            $("#chkQuickTopUp").prop("checked", false);
            $("#chkQuickTopUp").trigger('change');
        }
        else {
            $("#payPalAutoMessage").hide();
        }
    });

    $("#btnPayWithCredit").on("click", function () {
        $("#CheckoutFormPay360").prop("action", $("#hdnPay360PurchaseUrl").val());
        $("#payPalAutoMessage").hide();
    });

    $("#btnPay360Pay").on("click", function () {

        var Element = document.getElementById('MobileNumber');
        if ((IsValid_MobileNumber == undefined || IsValid_MobileNumber == false) && Element != null && Element.value.length >= 7) {
            $('#CreditModalContent').hide();
            $("#MobileNumber").trigger('keyup');
            $("#btnPay360Pay").attr("disabled", true);
            $('#MobileNumberleLoader').show();
        }
        else if ((IsValid_MobileNumber && Element != null) || Element == null) {

            if ($('#IsRegistered').val() === "true") {
                var productType = $("#hdnPay360ProductType").val();
                if (productType === "Bundle" || productType === "Basket") {
                    $("#CheckoutFormPay360").prop("action", $("#hdnPay360PurchaseUrl").val());
                } else if (productType === "CreditSimOrder") {
                    $("#CheckoutFormPay360").prop("action", $("#hdnPay360CreditSimPaymentUrl").val());
                }
                else {
                    $("#CheckoutFormPay360").prop("action", $("#hdnPay360TopUpUrl").val());
                }
            }

        }
        else if (Element.value.length >= 1) {
            $('#CreditModalContent').hide();
            $('#MobileNumberError').show();
            var Element = document.getElementById('MobileNumberError');
            Element.innerText = "Invalid PIN Number.";
            return false;
        }
        else {
            $('#CreditModalContent').hide();
            $('#MobileNumberError').show();
            var Element = document.getElementById('MobileNumberError');
            Element.innerText = "Please enter Mobile Number.";
            return false;
        }
        $("#payPalAutoMessage").hide();



    });

    $("#MobileNumber").keyup(function () {

        $('#MobileNumberSuccess').hide();
        $('#MobileNumberError').hide();
        $("#btnPay360Pay").attr("disabled", true);
        document.getElementById("btnPay360Pay").style.backgroundColor = "#90922d";

        inputRechargablePindelay(
            function () {
                if ($('#MobileNumber').val().length >= 7) {
                    $.ajax({
                        url: "/Pay360Account/VerifyNumberForCheckOut",
                        type: "POST",
                        data: { msisdn: $('#MobileNumber').val().trim() },
                        beforeSend: function (xhr) {
                            $('#MobileNumberleLoader').fadeIn();
                            $('#MobileNumberleLoader').show();
                            $('#MobileNumberSuccess').hide();
                            $('#MobileNumberError').hide();
                            $("#MobileNumber").prop("disabled", true);

                        },
                        success: function (response) {

                            if (response.errorCode == 0) {
                                $("#FirstUseDate").val(response.Sim_activationdate);
                                IsValid_MobileNumber = true;
                                $('#MobileNumberleLoader').hide();
                                $('#MobileNumberSuccess').show();
                            }

                        },
                        complete: function (xhr, status) {
                            $('#MobileNumberleLoader').hide();
                            $("#MobileNumber").prop("disabled", false);
                            $("#btnPay360Pay").attr("disabled", false);
                            document.getElementById("btnPay360Pay").style.backgroundColor = "#dee228";

                        },
                        error: function (xhr, status, error) {
                            IsValid_MobileNumber = false;
                            $('#MobileNumberError').show();
                            var Element = document.getElementById('MobileNumberError');
                            Element.innerText = "We didn't recognise that number. Please check that it belongs to the correct product.";
                            $('#MobileNumberleLoader').hide();
                            $('#MobileNumberSuccess').hide();
                        }
                    });
                }
                else if ($('#MobileNumber').val().length >= 1) {
                    $('#MobileNumberError').show();
                    var Element = document.getElementById('MobileNumberError');
                    Element.innerText = "Invalid PIN Number.";
                    $('#MobileNumberleLoader').hide();
                    $("#btnPay360Pay").attr("disabled", false);
                    document.getElementById("btnPay360Pay").style.backgroundColor = "#dee228";

                } else {
                    $('#MobileNumberError').show();
                    var Element = document.getElementById('MobileNumberError');
                    Element.innerText = "Please enter Mobile Number.";
                    $('#MobileNumberleLoader').hide();
                    $("#btnPay360Pay").attr("disabled", false);
                    document.getElementById("btnPay360Pay").style.backgroundColor = "#dee228";
                }
            }, 2000);


    });


    $("#new-card").on("click", function () {

        if ($("#new-card").prop("checked")) {
            $("#btnPay360Pay").val("Card");
            var productType = $("#hdnPay360ProductType").val();
            if (productType === "Bundle" || productType === "Basket") {
                $("#CheckoutFormPay360").prop("action", $("#hdnPay360PurchaseUrl").val());
            } else if (productType === "CreditSimOrder") {
                $("#CheckoutFormPay360").prop("action", $("#hdnPay360CreditSimPaymentUrl").val());
            }
            else {
                $("#CheckoutFormPay360").prop("action", $("#hdnPay360TopUpUrl").val());
            }
        }
        else {
            $("#btnPay360Pay").val("PaypalNew");
        }
    });
}();
function isValidEmailAddress(emailAddress) {
    var pattern = new RegExp(/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i);
    return pattern.test(emailAddress);
};

//WorldWideTopUp JS
$('#phone').keydown(function (e) {
    if (e.keyCode == 13) {
        e.preventDefault();
        $('.ph').blur();
        $('#btnGetPackage').trigger('click');
    }
    else {
        $("#PhoneError").hide();
        $("#PhoneCreditLow").hide();
    }
});

var IsValid_MobileNumber;

// restrict space and except 0 index + entry

//$('#phone').keypress(function (e) {
//    
//    if (this.value.length > 0 && e.keyCode == 43 || e.keyCode == 32) {
//        if (this.value.lastIndexOf(String.fromCharCode(e.keyCode)) == -1  && this.selectionStart == 0) {
//            return true;
//        } else {
//            return false;
//        } 
//    }  
//});

//GetPackage

var input = document.querySelector("#phone");
var output;
if (input !== null) {

    output = document.querySelector("#output");
    var iti = window.intlTelInput(input, {
        nationalMode: true,
        allowDropdown: true,
        initialCountry: "auto",
        separateDialCode: true,
        utilsScript: "~/CallingCardHomeCSSJS/build/js/utils.js",
    });
    var handleChange = function (e) {
        var text = (iti.isValidNumber()) ? iti.getNumber() : "Please enter a valid number";
        var textNode = document.createTextNode(text);
        output.innerHTML = text;
        //output.appendChild(textNode);
    };
    input.addEventListener('change', handleChange);
    input.addEventListener('keyup', handleChange);
    input.addEventListener('focus', handleChange);
}

$(".country-list li").click(function (e) {
    $("#phone").val("");
});
$(".selected-flag").click(function (e) {
    $("#phone").val("");
    $("#PhoneError").hide();
    $("#PhoneCreditLow").hide();
});
$(".ph").on('focus', function (e) {
    $("#phone").keyup(function (e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code == 13) {
            $("#phone").val("");
        }
    });
});
var Countrycode;
//  allow only number and Plus sign

var inputEl = document.getElementById('phone');

$(".passworderror").keydown(function () {
    $("#LoginError").hide();
})

//Login js
$("#loginfrm").submit(function (e) {
    if ($("#loginfrm").valid()) {
        $("#btnloginspinner").show();
        $("#LoginError").hide();
        var EmailAddress = $("#EmailAddressLogin").val();
        var Password = $("#Password").val();
        var modelLogin = {};

        var form = $('#loginfrm');
        var token = $('input[name="__RequestVerificationToken"]', form).val();
        //var token = $("input[name=__RequestVerificationToken]").val();
        modelLogin.EmailAddress = EmailAddress;
        modelLogin.Password = Password;
        if (Password.length >= 8) {
            $("#btnLogin").attr("disabled", true);
            $.ajax({
                url: "/Account/LoginInternationalTopUpWidget",
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

                            $("#login-signup").removeClass("show active");
                            $("#location-n-number-tab").addClass("nav-link active");
                            $("#location-n-number").addClass("tab-pane fade show active");
                        }
                        else {
                            window.location.href = responseModel.Url;
                        }
                    }
                    else if (responseModel.Message != null && responseModel.StatusCode == 778) {

                        $("#loginoption").show();
                        $('#loginhide').hide();
                        $("#login-signup-tab").hide();
                        $("#login-signup").removeClass("show active");
                        //$("#location-n-number-tab").hide();
                        $("#addsimcard-tab").removeAttr('hidden');
                        $("#addsimcard-tab").addClass("nav-link active");
                        $("#AddCallingCardDiv").addClass("tab-pane fade show active");
                    }
                    else if (responseModel.Message != null) {
                        $("#LoginError").show();
                        var element = document.getElementById("LoginError");
                        element.innerHTML = responseModel.Message;
                    }
                    $("#btnloginspinner").hide();
                    $("#btnLogin").attr("disabled", false);
                    $("#EmailAddressLogin").prop('disabled', false);
                    $("#Password").prop('disabled', false);
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
    } else {

        return false;
    }
    e.preventDefault();
});
//btn Get Package
$("#btnGetPackage").click(function (e) {

    $("#PhoneCreditLow").hide();
    $("#PhoneError").hide();
    $("#GetPackagespinner").show();
    var telephoneNumber = $("#output").text();
    if ($('#phone').val() == '0' || $('#phone').val() == '' || $('#phone').val() == 'undefined' || $('#phone').val() == null) {
        if ($('#phone').attr('placeholder') == 'Choose a Country') {
            $("#PhoneError").text("Please Choose the Country and Enter the Number.");
            $("#PhoneError").show();
            $("#GetPackagespinner").hide();
        }
        else {
            $("#PhoneError").text("Please Enter the Number");
            $("#PhoneError").show();
            $("#GetPackagespinner").hide();
        }


        return false;
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
    $("#btnGetPackage").attr("disabled", true);
    $.ajax({
        type: "POST",
        url: "/account/GetTransferPackagesInternationalTopUpWidget?telephoneNumber=" + $("#output").text() + "&countryCode=" + Countrycode,
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        success: function (result) {

            if (result.StatusCode == 2) {
                $("#GetPackagespinner").hide();
                $("#btnGetPackage").attr("disabled", false);
                $("#PhoneError").show();
                $("#PhoneError").text("Unfortunately there is not enough credit in your balance to send the phone credit amount. Please top up your account now and try again.");

                $("#PhoneCreditLow").show();
            }
            else {

                $.ajax({
                    url: "/account/GetJsonPayload",
                    success: function (Payload) {

                        if (Payload != null) {
                            var obj = jQuery.parseJSON(Payload);
                            var len = obj.ProductCodes.length;

                            $("#AccountName").text(obj.FullName.FirstName);
                            $("#EmailAddressText").text(obj.FullName.Email);
                            $("#EmailAddress").val(obj.FullName.Email);
                            for (var i = 0; i < len; i++) {
                                if (obj.ProductCodes[i].ProductCode == "THM") {
                                    $("#Msisdn").val(obj.ProductCodes[i].Reference);
                                    $("#MsisdnText").text(obj.ProductCodes[i].Reference);
                                    $("#ToMsisdn").val(output.innerText);
                                }
                            }
                        }
                    },
                    error: function (e) {
                        alert("An Error Occured,Please Refresh The Page.")
                    }
                });

                $("#btnGetPackage").attr("disabled", false);
                var json = $.parseJSON(result);
                if (json.errorCode == 0 && json.payload != null) {
                    var tel = telephoneNumber;
                    $("#Receiver").text(tel);
                    buildPackageList(json.payload);
                    setHiddenInputs(json.payload);
                    $("#GetPackagespinner").hide();
                    $("#package-tab").addClass("nav-link active");
                    $("#package").addClass("tab-pane fade show active");
                    $("#location-n-number").removeClass("show active");
                }
                else {
                    $("#GetPackagespinner").hide();
                    $("#btnGetPackage").attr("disabled", false);
                    $("#PhoneError").show();
                    $("#PhoneError").text("International top up is unavailable for this number at this time. Please check back soon.");
                }
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
    $("#location").html("<img style='display:inline' src='" + operator.iconUri + "' alt='operator icon'/> " + operator.country + " ");
    $(".OperaterCountry").html("<span class='text-center county-name'><img style='display:inline' src='" + operator.iconUri + "' alt='operator icon'/> " + operator.country + " </span>");
    var packageListHtml = "";
    for (var ip = 0; ip < operator.products.length; ip++) {
        var selected = false;
        if (ip == (operator.products.length - 1))
            selected = true;
        packageListHtml += buildPackageHtml(operator.products[ip], selected, ip);
    }
    $(".Packagelist").html(packageListHtml);
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
    return html1;

}

function setHiddenInputs(payload) {
    $("#TransactionReference").val(payload.operators[0].nowtelTransactionReference);
    $("#OperatorId").val(payload.operators[0].id);
}

$("#btnPackageSelection").click(function () {
    $("#package").removeClass("show active");
    $("#confirm-transfer-tab").addClass("nav-link active");
    $("#confirm-transfer").addClass("tab-pane fade show active");

    var EmailAddress = $("#EmailAddress").val();
    var fromMsisdn = $('#MsisdnText').text();
    var ToMsisdn = $("#ToMsisdn").val();
    var Cost = $("#Cost").val();
});

$("#btnPackageSelectionBack").click(function () {
    $("#package").removeClass("show active");
    $("#package-tab").removeClass("active");
    $("#location-n-number-tab").addClass("nav-link active");
    $("#location-n-number").addClass("tab-pane fade show active");
});

$("#btnConfirmTransferBack").click(function () {
    $("#confirm-transfer-tab").removeClass("active");
    $("#confirm-transfer").removeClass("show active");
    $("#package-tab").addClass("nav-link active");
    $("#package").addClass("tab-pane fade show active");
    $('#CheckoutError').hide();
});

function GetPayload() {
    $.ajax({
        url: "/account/GetJsonPayload",
        success: function (Payload) {

            if (Payload != null) {
                var obj = jQuery.parseJSON(Payload);
                var len = obj.ProductCodes.length;

                $("#AccountName").text(obj.FullName.FirstName);
                $("#EmailAddressText").text(obj.FullName.Email);
                $("#EmailAddress").val(obj.FullName.Email);
                for (var i = 0; i < len; i++) {
                    if (obj.ProductCodes[i].ProductCode == "THM") {
                        $("#Msisdn").val(obj.ProductCodes[i].Reference);
                        $("#MsisdnText").text(obj.ProductCodes[i].Reference);
                        $("#ToMsisdn").val(output.innerText);
                    }
                }
            }
        },
        error: function (e) {
            alert("An Error Occured,Please Refresh The Page.")
        }
    });

}

// Add TalkHome Mobile
$("#callingcardform").submit(function (e) {

    $('#btnaddcallingcard').attr('disabled', true);
    $('#btnaddcallingcardspinner').show();
    if ($("#callingcardform").valid()) {
        var CallingCardMode = {};
        CallingCardMode.Code = $('#Code').val();
        CallingCardMode.Number = $("#Number").val();
        CallingCardMode.ProductCode = $("#ProductCode").val();
        $.ajax({
            url: "/Account/AddProductDetailsInternationalTopUp",
            type: "POST",
            data: {
                model: CallingCardMode
            },
            success: function (responseModel) {
                $('#btnaddcallingcardspinner').hide();

                if (responseModel.StatusCode == 1) {
                    $("#SuccessFont").show();

                    setTimeout(
                        function () {
                            $("#AddCallingCardDiv").removeClass("show active");
                            $("#location-n-number-tab").addClass("nav-link active");
                            $("#location-n-number").addClass("tab-pane fade show active");
                            $('#btnaddcallingcard').attr('disabled', false);
                        }, 5000);


                }
                else {
                    $('#btnaddcallingcard').attr('disabled', false);

                    $('#AddcallingcardError').show();
                    var element = document.getElementById('AddcallingcardError');
                    element.innerHTML = responseModel.Message;

                }


            },
            error: function (e) {
                $('#btnaddcallingcardspinner').hide();
                alert("Error");
            }
        });
    }
    else {
        $('#btnaddcallingcardspinner').hide();
        $('#btnaddcallingcard').attr('disabled', false);
        return false;
    }
    e.preventDefault();
});

$("#Code").keyup(function () {
    $('#AddcallingcardError').hide();
});

$("#Number").keyup(function () {
    $('#AddcallingcardError').hide();
});

// Checkout Form 
function OnCheckOutSuccess(responsemodel) {

    $("#CheckoutSpinner").hide();
    $("#gotoCreditPayment").attr('disabled', false);
    $("#btnConfirmTransferBack").attr('disabled', false);
    $('#CheckoutError').hide();
    if (responsemodel.url != null) {
        alert("Unhandled");
        var form = $(document.createElement('form'));
        $(form).attr("action", "account/TransferCheckout");
        $(form).attr("method", "POST");
        $(form).css("display", "none");
        form.appendTo(document.body);
        $(form).submit();
    }
    else if (responsemodel.StatusCode == 1) {

        var model = $.parseJSON(responsemodel.Message);
        (function (i, s, o, g, r, a, m) {
            i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
                (i[r].q = i[r].q || []).push(arguments)
            }, i[r].l = 1 * new Date(); a = s.createElement(o),
                m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
        })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');

        var url = $(location).attr('href').replace(/\/$/, '');
        window.history.pushState("", "", url);

        var form = $(document.createElement('form'));
        $(form).attr("action", "account/TransferCreditSuccess");
        $(form).attr("method", "POST");
        $(form).css("display", "none");
        var TransactionReference = $("<input>")
            .attr("type", "text")
            .attr("name", "TransactionReference")
            .val(model.TransactionReference);
        $(form).append($(TransactionReference));
        var CreditRemaining = $("<input>")
            .attr("type", "text")
            .attr("name", "CreditRemaining")
            .val(model.CreditRemaining);
        $(form).append($(CreditRemaining));
        form.appendTo(document.body);
        $(form).submit();
    }
    else if (responsemodel.Message != null && responsemodel.StatusCode == 0) {
        $('#CheckoutError').show();
        var element = document.getElementById("CheckoutError");
        element.innerHTML = responsemodel.Message;
    }
}

function OnCheckOutFailure(Error) {

    $("#CheckoutSpinner").hide();
    $("#gotoCreditPayment").attr('disabled', false);
    $("#btnConfirmTransferBack").attr('disabled', false);
    alert("error");
}

function OnCheckoutbegin(Error) {

    $("#gotoCreditPayment").attr('disabled', true);
    $("#btnConfirmTransferBack").attr('disabled', true);
    $("#CheckoutSpinner").show();
    $('#CheckoutError').hide();
}

$('.PUK').keypress(function () {

    return (event.charCode === 43 || (event.charCode >= 48 && event.charCode <= 57 || event.charCode == 13));
});

$('.PIN').keypress(function () {

    return (event.charCode === 43 || (event.charCode >= 48 && event.charCode <= 57 || event.charCode == 13));
});

//Bundles 
$('#destination').change(function () {
    var countryCode = $(this).find(':selected').attr('value');
    if (countryCode == "Alldestinations") {
        $('#App-Bundles').find('[data-destination]').fadeIn();
    } else {
        $('#App-Bundles').find('[data-destination]').hide();
        $('#App-Bundles').find('[data-destination=' + countryCode + ']').fadeIn();
    }

    var filteredCountry = impressionArray.filter(function (item) {
        return item[0].countrycode == countryCode || item[0].countrycode == `.recommended-plans${countryCode}`;
    });

    ProductImpression(filteredCountry);
});

$('.CardNum').keypress(function () {

    return (event.charCode === 43 || (event.charCode >= 48 && event.charCode <= 57 || event.charCode == 13));
});

$('.PIN').keypress(function () {

    return (event.charCode === 43 || (event.charCode >= 48 && event.charCode <= 57 || event.charCode == 13));
});

// auto top hide and show section




var numbermsisdn;
var isvaildmsisdn;
var input = document.querySelector("#inpMsisdnQuickTopUp");
var output;
if (input !== null) {

    output = document.querySelector("#msisdnvalid");
    var iti = window.intlTelInput(input, {
        allowDropdown: false,
        onlyCountries: ['gb'],
        utilsScript: "~/CallingCardHomeCSSJS/build/js/utils.js",
    });
    var handleChange = function (e) {

        this.value = this.value.replace(/\s/g, "");
        $('#MobileNumberSuccess').hide();
        $('#autotopupsection').hide();
        $('#MobileNumberError').hide();
        var hdnMsisdn = $('#hdnMsisdn').val();
        if (iti.isValidNumberCustom()) {
            isvaildmsisdn = true;
            numbermsisdn = iti.getNumberCustom();
            numbermsisdn.charAt(0) == "+";
            var numbermsisdn = numbermsisdn.substr(1);
            if (hdnMsisdn != null && hdnMsisdn != undefined) {
                if (hdnMsisdn == numbermsisdn) {
                    $('#MobileNumberSuccess').show();
                    $('#autotopupsection').fadeIn();
                    return false;
                }
            }
        }
        else {
            isvaildmsisdn = false;
        }
    };
    input.addEventListener('change', handleChange);
    input.addEventListener('keyup', handleChange); input.addEventListener('focus', handleChange);
}


$("#frmQuickTopUp").submit(function (e) {
    var number = $("#inpMsisdnQuickTopUp").val();
    if (number.length > 0) {
        if (number.length == 10 || number.length == 11) {
            $("#inpMsisdnQuickTopUp").prop("disabled", true);
            $("#btnQuickTopUpSubmit").addClass("loading");
            var modelVerify = {};
            modelVerify.msisdn = number;
            $.ajax({
                url: "/pay360account/VerifyNumber",
                type: "POST",
                data: modelVerify,
                success: function (data) {
                    $("#inpMsisdnQuickTopUp").prop("disabled", false);
                    if (data.message === "Success" && data.errorCode == 0) {

                        var form = $('#frmQuickTopUp');
                        var token = $('input[name="__RequestVerificationToken"]', form).val();
                        //var token = $("input[name=__RequestVerificationToken]").val();
                        var modelTopUp = {};
                        modelTopUp.msisdn = number;
                        modelTopUp.amount = $("#hdnFastTopUpAmount").val();
                        modelTopUp.AutoTopUpEnabled = $("#hdnAutoTopUpEnabled").val();
                        modelTopUp.TopUpId = $("#hdnFastTopUpId").val();
                        modelTopUp.FirstUseDate = data.sim_activationdate;
                        $.ajax({
                            url: "/pay360account/TopUpCheckout",
                            type: "POST",
                            data: {
                                __RequestVerificationToken: token,
                                model: modelTopUp
                            },
                            success: function (response) {
                                if (response.Message === "Success") {
                                    $("#btnQuickTopUpSubmit").removeClass("loading");
                                    window.location.href = response.Url;

                                }
                                else {
                                    alert("An Error Occured,Please refresh and re-perform.");
                                    $("#btnQuickTopUpSubmit").removeClass("loading");
                                }
                            },
                            error: function () {
                                $("#btnQuickTopUpSubmit").removeClass("loading");
                                $("#btnQuickTopUpSubmit").removeClass("loading");
                            }
                        });
                    }
                    else if (data === "Failure") {
                        $("span.quickTopUpError").show();
                        $("#btnQuickTopUpSubmit").removeClass("loading");
                    }
                    else {
                        $("#btnQuickTopUpSubmit").removeClass("loading");
                    }
                },
                error: function () {
                    alert("An error occured, Please refresh and re-perform.");
                    $("#btnQuickTopUpSubmit").removeClass("loading");
                }
            });
            return false;
        }
        else {
            var Element = document.getElementById('MobileNumberError');
            Element.innerText = "Please enter a valid number.";
            $('#MobileNumberError').show();
            return false;
        }

    } else {
        return false;
    }
});



//VerifyNumber On CheckOut Page
var inputRechargablePindelay = (function () {

    var timer = 0;
    return function (callback, ms) {
        clearTimeout(timer);
        timer = setTimeout(callback, ms);
    };
})();


$('.MobileNumber').keypress(function () {
    return (event.charCode === 43 || (event.charCode >= 48 && event.charCode <= 57 || event.charCode == 13));
});

//App-International-Rates

$("input[type='radio'][name='optradio']").click(function () {

    var CurrencyType = $("input[type='radio'][name='optradio']:checked").val();
    var CountrySelected = $('#appRates').val();

    if (CurrencyType == null) {
        CurrencyType = "GBP";
    }

    GetInternetionalAppRates(CurrencyType, CountrySelected);

});

function DrawInternationalAppRatesSection() {

    var CurrencyType;
    var CountrySelected = "1";
    if (CurrencyType == null) {
        CurrencyType = "GBP";
    }

    GetInternetionalAppRates(CurrencyType, CountrySelected);
}

function Countries(response) {

    var myNode = document.getElementById("appRates");
    myNode.innerHTML = '';
    var length = response.length;
    $('#appRates').append(
        $('<option></option>').val("1").html("Select destination")
    );
    for (var i = 0; i < length; i++) {
        $('#appRates').append(
            $('<option></option>').val(response[i].isoCode).html(response[i].name)
        );
    }
}

function GetInternetionalAppRates(CurrencyType, CountrySelected) {
    $.ajax({
        type: "GET",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        url: "/TalkHomeAppInternationalRatesByCurrency",
        data: {
            CurrencyType: CurrencyType
        },
        beforeSend: function () {
            $('#appRates')
                .children()
                .remove()
                .end()
                .append(
                    $('<option></option>').val("1").html("Loading......")
                );
            $('#appRates').attr("disabled", true);
        },
        success: function (data) {
            $('#appRates').attr("disabled", false);
            $("#RatesServiceUnavailable").hide();
            if (data.payload != undefined) {
                var myNode = document.getElementById("rates");
                myNode.innerHTML = '';

                var htmlImg = '';

                htmlImg += '<ul class="country-rates intal-rate-disp app-rate-disp" data-destination="1">' +
                    '<li class="cr-landline">' +
                    '<img src="/web_assets/images/icon-telephone.svg" alt="Landline Rates">' +
                    '</li>' +
                    '<li class="cr-mobile">' +
                    '<img src="/web_assets/images/smartphone-call.svg" alt="Mobile Rates">' +
                    '</li>' +
                    '<li class="cr-sms">' +
                    ' <img src="/web_assets/images/smartphone-sms.svg" alt="SMS Rates">' +
                    '</li>' +
                    '</ul>';

                $('#rates').append(htmlImg);

                var length = data.payload.countryDestinations.length;
                var countryDestinations = data.payload.countryDestinations;
                Countries(countryDestinations);
                var html = '';
                for (var i = 0; i < length; i++) {
                    html += '<ul class="country-rates intal-rate-disp app-rate-disp" style="display:none" data-destination="' + countryDestinations[i].isoCode + '">';
                    var rateslength = countryDestinations[i].rates.length;
                    var rates = countryDestinations[i].rates;
                    for (var j = 0; j < rateslength; j++) {
                        switch (j) {
                            case 0:
                                html += '<li class="cr-landline">' +
                                    '<img src="/web_assets/images/icon-telephone.svg" alt="Landline Rates">' +
                                    '<strong id="strongFilterMobile" hidden="" style="display:inline;">' +
                                    '<b id = "filterLandlineVal" >' + rates[0].Price + '</b>' +
                                    '<span>/min</span>' +
                                    '</strong>' +
                                    '</li>';
                                break;
                            case 1:
                                html += '<li class="cr-mobile">' +
                                    '<img src="/web_assets/images/smartphone-call.svg" alt="Mobile Rates">' +
                                    '<strong id="strongFilterLandline" hidden="" style="display:inline;">' +
                                    '<b id="filterMobileVal">' + rates[1].Price + '</b>' +
                                    '<span>/min</span>' +
                                    '</strong>' +
                                    '</li>';
                                break;
                            case 2:
                                html += '<li class="cr-sms">' +
                                    '<img src="/web_assets/images/smartphone-sms.svg" alt="SMS Rates">' +
                                    '<strong id="strongFilterSms" hidden="" style="display:inline;">' +
                                    '<b id="filterSmsVal">' + rates[2].Price +
                                    '</b>' +
                                    '<span>/sms</span>' +
                                    '</strong>' +
                                    '</li>';
                                break;
                        }
                    }
                    html += '</ul>';
                }
                $('#rates').append(html);
                $("#appRates").val(CountrySelected).attr("selected", "selected");
                $("#appRates").trigger('change');
            }
            else {
                $("#RatesServiceUnavailable").show();
            }
        }
    });
}

$('.MobileNumber').keypress(function () {
    $('#MobileNumberError').hide();
    $("#quickTopUpErrorSpan").hide();
})
