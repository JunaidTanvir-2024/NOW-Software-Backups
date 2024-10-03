var stopkeyupfunction;
var IsCheckedEmail = false;
function BuyPlan_home(current) {
    $('#BundleId').val(current.dataset.id);
    $('#bundleAmount').val(current.dataset.price);
    
    //$("#btnConfirmOrderPlan").show();
    $("#step_2").hide();
    $("#step_1").hide();
    //show credit sim user entry fileds
    $("#creditsimordrform").slideDown(2000, function () {
        $("#plans_section").fadeOut();
    });
    if ($('#step_1').is(":visible") == false && $("#destinations").is(':visible') === false) {
        $("#btnConfirmOrder").show();
    }
    else {
        $("#btnConfirmOrder").hide();
    }
}


$("#btnConfirmOrder").click(function () {
    if (!$('#creditsimordrform').valid())
        return false;
    var Email = $('#EmailAddresscreditsim').val();

    if (!validateemail(Email))
        return false;

    if ($("#lookupLink").is(':visible') === true
        && $("#addressFields").is(':visible') === false
        && $("#destinations").is(':visible') === false) {
        $('#lookupLink').trigger("click");
        return false;
    }

    ValidateUserOrder();
});

//Check Userexists or not
function validateemail(Email) {
    var valid = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/.test(Email);
    if (!valid) {
        $("#emailaddresserrror").show();
        $("#emailaddresserrror").text("Please enter valid email");
        return false;
    }
    return true;


}
function ValidateUserOrder() {

    $("#SignUp").val("off");

    $("#MobileNumberSuccess").hide();
    
        $.ajax({
            url: "Verifyusersimorder",
            type: 'POST',
            dataType: 'json',
            beforeSend: function () {
                $('#creditsim_formsection').addClass("thm-loading");
                $("#BTN_VALIDATESIMORDER").attr("disabled", true);
                $("#EmailAddresscreditsim").attr("readonly", true);
            },
            data: {
                PostCode: $('#PostalCode').val(),
                Address: $('#AddressLine1').val(),
                EmailAddress: $('#EmailAddresscreditsim').val()
            },
            success: function (result) {

                if (result.isValid == false) {

                    if (result.isValid == false && result.errorcode == 1) {
                        $('#crs_email_verificationmessage').html("You have reached the maximum number of SIM orders for this month. Please contact customer services via hello@talkhomemobile.com for further assistance").show();
                    }
                    if (result.isValid == false && result.errorcode == 2) {
                        $('#crs_email_verificationmessage').html("You have reached the maximum number of SIM orders for this account. Please contact customer services via hello@talkhomemobile.com for further assistance").show();
                    }
                    if (result.isValid == false && result.errorcode == 3) {
                        $('#crs_email_verificationmessage').html("You have reached the maximum number of SIM orders for this account. Please contact customer services via hello@talkhomemobile.com for further assistance").show();
                    }
                    if (result.isValid == false && result.errorcode == 4) {
                        $('#crs_email_verificationmessage').html("You have reached the maximum number of SIM orders for this month. Please contact customer services via hello@talkhomemobile.com for further assistance").show();
                    }
                    $("#EmailAddresscreditsim").attr("readonly", false);
                    $("#btnConfirmUserOrder").attr("disabled", false);
                    $("#destinations").attr("disabled", false);


                    return;
                }

                if (result.regsitered == false && result.confirmed == false) {
                    $("#btnConfirmOrderbtn").show();
                    $('#crs_email_verificationmessage').hide();
                    $("#divnewpassword").show();
                    $("#divconfirmpassword").show();
                    $("#creditsim_otp_verification").show();
                    $("#divSubscribeSignUp").show();
                    $("#SignUp").val("on");

                    $('#btnConfirmOrderPlan').show();
                    $("#btnConfirmOrderPlan").attr("disabled", true);

                    $("#btnConfirmOrder").hide();
                    $("#btnConfirmOrder").attr("disabled", false);

                    $("#EmailAddress").attr("readonly", true);
                    $("#PostalCode").attr("readonly", true);
                    $("#destinations").attr("disabled", true);

                    starttimer();
                    $("#sendOTP").attr("disabled", true);
                    

                    return;

                }

                if (result.regsitered == true && result.confirmed == false) {

                    $('#creditsim_otp_verification').show();
                    //$("#resendsendOTP").trigger("click");
                    $('#btnConfirmOrderPlan').show();
                    $("#btnConfirmOrderPlan").attr("disabled", true);
                    starttimer();
                    $("#btnConfirmOrder").hide();
                    $("#btnConfirmOrder").attr("disabled", false);
                    $("#sendOTP").attr("disabled", true);
                    return;

                }
                if (result.regsitered == true && result.confirmed == true && result.isValid == true) {
                    $("#btnConfirmOrderbtn").show();
                    $('#crs_email_verificationmessage').hide();
                    $('#btnConfirmOrderPlan').show();
                    $("#btnConfirmOrderPlan").attr("disabled", false);

                    $("#btnConfirmOrder").hide();
                    $("#btnConfirmOrder").attr("disabled", false);


                    if ($('#step_1').is(":visible") == true) {
                        $("#gotostep2").trigger('click');
                        return;
                    }

                    else {
                        $('#creditsimordrform').submit();
                        $('#creditsim_formsection').addClass("thm-loading");
                        return;
                    }
                    

                }

            },
            complete: function () {
                $('#creditsim_formsection').removeClass("thm-loading");
            }
        });
}


//$("#EmailAddresscreditsim").keyup(function () {

//    if (this.value == "")
//        return false;
//    $("#emailaddresserrror").hide();
//    $("#MobileNumberSuccess").hide();
//    var Email = this.value;
//    stopCreditSimUserExists();
//    validateemail(Email)
//});

//Check Userexists or not
//function validateemail(Email) {
//    var valid = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/.test(Email);
//    if (valid) {
//        CreditSimUserExists();
//    } else {
//        $("#emailaddresserrror").show();
//        $("#emailaddresserrror").text("Please enter valid email");
//        return false;
//    }
//}
//function CreditSimUserExists() {

//    $("#SignUp").val("off");
//    $('#MobileNumberleLoader').show();
//    $("#MobileNumberSuccess").hide();
//    stopkeyupfunction = setTimeout(function () {

//        $("#btnConfirmOrder").attr("disabled", true);
//        $("#btnConfirmOrder").css('background-color', '#9ea125');
//        $.ajax({
//            url: "CreditSimUserExists",
//            type: 'GET',
//            dataType: 'json',
//            data: { "EmailAddress": $("#EmailAddresscreditsim").val() },
//            beforeSend: function () {
//                $('#MobileNumberleLoader').show();
//                $("#EmailAddresscreditsim").attr("disabled", true);
//            },
//            success: function (res) {

//                $('#MobileNumberleLoader').hide();
//                $("#EmailAddresscreditsim").attr("disabled", false);

//                if (res.status == true && res.errorCode == 0) {

//                    IsCheckedEmail = true;
//                    $("#btnConfirmOrder").attr("disabled", false);
//                    $("#btnConfirmOrder").css('background-color', '#dee228');
//                    $("#MobileNumberSuccess").show();
//                    if (!res.data.UserExists) {
//                        $("#divnewpassword").show();
//                        $("#divconfirmpassword").show(); 
//                        $("#creditsim_otp_verification").show();
//                        $("#resendsendOTP").trigger("click");
//                        $("#divSubscribeSignUp").show();
//                        $("#SignUp").val("on");
//                    } else {
//                        $("#userId").val(res.data.UserId);
//                        $("#divnewpassword").hide();
//                        $("label[for='Password']").hide()
//                        $("#divconfirmpassword").hide();
//                        $("label[for='confirmpassword']").hide()
//                        $("#divSubscribeSignUp").hide();
//                    }

//                } else {
//                    IsCheckedEmail = false;
//                    $("#divnewpassword").hide();
//                    $("label[for='Password']").hide()
//                    $("#divconfirmpassword").hide();
//                    $("label[for='confirmpassword']").hide()
//                    $("#divSubscribeSignUp").hide();

//                    $("#emailaddresserrror").show();
//                    $("#emailaddresserrror").text(res.message);
//                    $("#btnConfirmOrder").attr("disabled", true);
//                    $("#btnConfirmOrder").css('background-color', '#9ea125');
//                }

//            }
//        });

//    }, 3000);
//}

$("#cdigit-4,#digit-3,#digit-2,#digit-1").keyup(function () {

    var data = {
        digit1: $('#digit-1').val(),
        digit2: $('#digit-2').val(),
        digit3: $('#digit-3').val(),
        digit4: $('#cdigit-4').val(),
        EmailAddress: $("#EmailAddresscreditsim").val(),
    };

    if (!data.digit1 || !data.digit2 || !data.digit3 || !data.digit4) {
        return;
    }

    $('#otp_verificationmessage').hide();
    $('#otp_verificationsucessmessage').hide();
    $("#btnConfirmOrderPlan").attr("disabled", true);
    if (this.value == "")
        return false;
    else {
        $.ajax({
            type: "POST",
            url: "/Verifyuserotp",
            data: data,
            beforeSend: function () {
                $('#creditsim_formsection').addClass("thm-loading");
            },

            success: function (response) {

                if (response.response == true) {

                    $('#otp_verificationsucessmessage').show();
                    $('#creditsim_otp_verification').hide();

                    if ($('#creditsimordrform').valid())
                    {
                        $('#creditsim_formsection').addClass("thm-loading");


                        if ($('#step_1').is(":visible") == true) {
                            $("#gotostep2").trigger('click');
                            return;

                        }
                        $('#btnConfirmUserOrder').trigger("click");
                        $("#btnConfirmUserOrder").attr("disabled", false);
                    }

                    else {
                        $("#btnConfirmUserOrder").attr("disabled", false);
                    }
                 
                    
                }

                else {
                    $('#creditsim_otp_verification').show();
                    $('#otp_verificationmessage').html("Invalid PIN.<br>Please Enter valid PIN or Resend OTP.").show();
                    $('#digit-1').val("").focus(),
                        $('#digit-2').val("").attr('disabled', 'disabled'),
                        $('#digit-3').val("").attr('disabled', 'disabled'),
                        $('#cdigit-4').val("").attr('disabled', 'disabled'),
                    $("#btnConfirmUserOrder").attr("disabled", true);


                }

            },

            error: function (xhr, error, status) {
                $('#creditsim_formsection').removeClass("thm-loading");
            },
            complete: function () {
                $('#creditsim_formsection').removeClass("thm-loading");
            }
        });
    }
});

$("#sendOTP").click(function (e) {
    if ($('#sendOTP').prop('disabled', true)) {
        return;
    }
    $.ajax({
        type: "POST",
        url: "/Send_otp_to_user",
        data: {
            email: $('#EmailAddress').val(),
            firstname: $('#FirstName').val(),
        },
        success: function (result) {

            starttimer();
            return;

        },

        error: function (xhr, error, status) {
        }
    });

});

function stopCreditSimUserExists() {
    clearTimeout(stopkeyupfunction);
}

//$("#btnConfirmOrder").click(function () {

//    var signup = $("#SignUp").val();
//    validateCreditSimOrderForm(this);

//})

$("#btnConfirmUserOrder").click(function () {
    var signup = $("#SignUp").val();
    validateCreditSimOrderForm(this);

})
//Validate Form
function validateCreditSimOrderForm(element) {

    jQuery.validator.addMethod("lettersonly", function (value, element, param) {
        return this.optional(element) || ! /[-+]?([0-9]*.[0-9]+|[0-9]+)/g.test(value);
    });

    var validator = $("#creditsimordrform").validate({
        rules: {
            Password: {
                minlength: 8,
                required: true
            },
            confirmpassword: {
                minlength: 8,
                required: true,
                equalTo: "#newpassword"
            },
            FirstName: {
                required: true,
                lettersonly: true,
            }, LastName: {
                required: true,
                lettersonly: true,
            },
            EmailAddress: {
                required: true,
                email: true
            },
            PostalCode: {
                required: true
            }
        },
        messages: {
            Password: {
                minlength: jQuery.format("Enter at least {0} characters"),
                required: "Please enter new password"
            },
            confirmpassword: {
                required: "Please re-enter new password",
                equalTo: "Enter Confirm Password Same as New Password",
                minlength: jQuery.format("Enter at least {0} characters")
            },
            FirstName: {
                required: "Please enter firstname",
                lettersonly: "Only alphabetical characters are allowed"
            },
            LastName: {
                required: "Please enter lastname",
                lettersonly: "Only alphabetical characters are allowed"
            }, EmailAddress: {
                required: "Please enter email",
                email: "Please enter valid email"
            },
            PostalCode: {
                required: "Please enter postal code",
            }
        }
    });
    if (validator.form()) {
        //if ($("#lookupLink").is(':visible') === true && $("#addressFields").is(':visible') === false && $("#destinations").is(':visible') === false)
        //{

            
        //    $('#lookupLink').trigger("click");
        //    return false;
        //}

        if ($("#destinations").is(':visible') === true && checkoutlookup == false) {
            $('#lookupLink').trigger("click");
            return false;
        }

        

        if ($('#step_1').is(":visible") == true) {
            $("#gotostep2").trigger('click');
            return;

        }
        else {
            

            var bundleId = $("#BundleId").val();
            var TopupId = $('.amount-radio:input:checked').val();
            if (bundleId != 0 && bundleId != "" && bundleId != undefined) {

              
                $("#creditsimordrform").submit();
            }
            
        }


        //CreditSimOrderForm();
    } else {
        return false;
    }
}
//CreditSimOrder
function CreditSimOrderConfirmForm() {

    var basket;
    var creditsimOrdermodel = {};
    var amount = $('.amount-radio:input:checked').val();
    var bundleId = $("#BundleId").val();
    var bundleAmount = $("#bundleAmount").val();
    if (bundleId != null && bundleId != "" && bundleId != undefined && amount != null && amount != undefined && amount != "") {
        creditsimOrdermodel.creditSimType = 3;
        basket = [{
            "amount": bundleAmount,
            "bundleId": bundleId
        },
        {
            "amount": amount,
            "bundleId": ""
        }];

    } else if (bundleId != null && bundleId != "" && bundleId != undefined && (amount == null || amount == undefined || amount == "")) {
        creditsimOrdermodel.creditSimType = 2;

        basket = [{
            "amount": bundleAmount,
            "bundleId": bundleId
        }];


    } else if (amount != null && amount != undefined && amount != "" && (bundleId == null || bundleId == "" || bundleId == undefined)) {
        creditsimOrdermodel.creditSimType = 1;

        basket = [{
            "amount": amount,
            "bundleId": ""
        }];
    } else {
        $.Toast("Either select pland or Top-up amount, or click No, I just want my Free SIM delivered",
            { 'align': 'center', 'position': 'top', 'width': 300, 'class': 'sl-notificationSuccess' }
        );
        return false;
    }

    creditsimOrdermodel.firstName = $("#FirstName").val();
    creditsimOrdermodel.lastName = $("#LastName").val();   // new password
    creditsimOrdermodel.email = $("#EmailAddresscreditsim").val();  //re-enter new password 
    creditsimOrdermodel.addressL1 = $("#AddressLine1").val();
    creditsimOrdermodel.addressL2 = $("#AddressLine2").val();
    creditsimOrdermodel.city = $("#City").val();
    creditsimOrdermodel.county = $("#CountyOrProvince").val();
    creditsimOrdermodel.country = $("#CountryCode").val();
    creditsimOrdermodel.postCode = $("#PostalCode").val();
    creditsimOrdermodel.userId = $("#userId").val();
    creditsimOrdermodel.basket = basket;

    $.ajax({
        type: "POST",
        url: "/creditsimorderconfirm",
        data: { model: creditsimOrdermodel },
        success: function (res) {

            if (res.status = true && res.errorCode == 0) {
                //go to step 2
                //$("#gotostep2").trigger('click');
                if (res.userId != null) {
                    $("#userId").val(res.userId);

                    var creditsimcheckoutmodel = {};
                    creditsimcheckoutmodel.amount = amount;
                    creditsimcheckoutmodel.BundleId = $("#BundleId").val();
                    creditsimcheckoutmodel.ProductCode = "THM";
                    $.ajax({
                        type: "POST",
                        url: "/creditsimcheckout",
                        data: { model: creditsimcheckoutmodel },
                        success: function (res) {

                            if (res.status == true && res.errorCode == 0 && res.data != null) {
                                window.location.href = res.data;
                            } else {
                                $.Toast("An internal server error occured.",
                                    { 'align': 'center', 'position': 'top', 'width': 300, 'class': 'sl-notificationSuccess' }
                                );
                                return false;
                            }
                        },
                        error: function () {

                            $.Toast("An error occured,Please try again.",
                                { 'align': 'center', 'position': 'top', 'width': 300, 'class': 'sl-notificationSuccess' }
                            );
                            return false;
                        }
                    })
                }

            } else if (res.status = false && res.errorCode != 0) {
                if (res.message != null) {
                    alert(res.message);
                }
            }

        },
        error: function (e) {

        },
    });
}
//free sim
$("#btnfreesimorder").click(function (e) {

    $('.pagesloader').show();
    $("#creditsimordrform").attr('action', '/MailOrderForExistingUser')
    $("#creditsimordrform").submit();
    e.preventDefault();
    //CreditSimOrderForm();
})
function CreditSimOrderForm() {

    var creditsimOrdermodel = {};
    creditsimOrdermodel.FirstName = $("#FirstName").val();
    creditsimOrdermodel.LastName = $("#LastName").val();   // new password
    creditsimOrdermodel.EmailAddress = $("#EmailAddresscreditsim").val();  //re-enter new password 
    creditsimOrdermodel.Password = $("#newpassword").val();
    creditsimOrdermodel.ConfirmPassword = $("#confirmpassword").val();
    creditsimOrdermodel.AddressLine1 = $("#AddressLine1").val();
    creditsimOrdermodel.AddressLine2 = $("#AddressLine2").val()
    creditsimOrdermodel.City = $("#City").val()
    creditsimOrdermodel.CountyOrProvince = $("#CountyOrProvince").val()
    creditsimOrdermodel.CountryCode = $("#CountryCode").val()
    creditsimOrdermodel.PostalCode = $("#PostalCode").val()
    creditsimOrdermodel.MailOrderProduct = $("#MailOrderProduct").val()
    creditsimOrdermodel.SignUp = $("#SignUp").val()

    $.ajax({
        type: "POST",
        url: "/MailOrderForExistingUser",
        data: { model: creditsimOrdermodel },
        success: function (res) {

            if (res.status == true && res.errorCode == 0) {
                window.location.href = "/order-a-sim/success";
                if (res.data != null) {
                    $("#userId").val(res.data.payload);
                }

            } else if (res.status == false && res.errorCode != 0) {
                if (res.message != null) {
                    $.Toast(res.message,
                        { 'align': 'center', 'position': 'top', 'width': 300 }
                    );
                }
            }

        },
        error: function (e) {
            $.Toast("An error occurred.",
                { 'align': 'center', 'position': 'top', 'width': 300 }
            );
        },
    });
}


// Post CreditSim Form for Payment and other calls

$("#btnconfirmordersim").click(function () {

    $('#btnconfirmordersimSpinner').show();

    var bundleId = $("#BundleId").val();
    var TopupId = $('.amount-radio:input:checked').val();
    if ((bundleId != 0 && bundleId != "" && bundleId != undefined) || (TopupId != 0 && TopupId != "" && TopupId != undefined)) {
        $('#topiderror').hide();
        $('#btnconfirmordersimSpinner').show();
        $("#btnconfirmordersim").attr("disabled", true);
        $("#btnconfirmordersim").css('background-color', '#9ea125');
        $("#creditsimordrform").submit();
    }
    else {
        $('#topiderror').show();
        $('#btnconfirmordersimSpinner').hide();
    }
})
//show and hide password
$('.pwd-hs-btn').click(function () {
    var next = $(this).next();
    if ('password' == next.attr('type')) {
        this.textContent = "Hide";
        next.prop('type', 'text');
    } else {
        this.textContent = "Show";
        next.prop('type', 'password');
    }
});

// Checkbox of Offer and services subscriptions

$("#SubscribeSignUp").on("click", function (e) {
    if (this.checked) {
        $(this).attr("value", "true");
    } else {
        $(this).attr("value", "false");
    }
});



//Credit sim new journey

$("#creditsim_arrow").click(function () {
    $("#arrow_down").fadeToggle('slow');
    $("#arrow_up").fadeToggle().css('display', 'inline');

    $("#creditsimordrform").slideToggle();
    $("#plans_section").fadeToggle();



    //$("#creditsimordrform").slideToggle(2000, function () {
    //    $("#plans_section").fadeToggle();
    //});
})


$("#PostalCode").keyup(function () {
    checkoutlookup = false;
})

function starttimer() {
    mins = 10;
    secs = 0;
    let btn = $("#sendOTP");
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
}