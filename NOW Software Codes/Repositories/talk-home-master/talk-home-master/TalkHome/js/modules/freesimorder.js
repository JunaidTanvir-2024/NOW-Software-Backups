
function validateemailaddress(Email) {
    var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
    return emailReg.test(Email);
}
var stopkeyupfunction;

$("#BTN_VALIDATESIMORDER").click(function (e) {

    if (!$('#OrderSimForm').valid())
        return false;

    if ($("#lookupLink").is(':visible') === true
        && $("#addressFields").is(':visible') === false
        && $("#destinations").is(':visible') === false) {
        $('#lookupLink').trigger("click");
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/Verifyusersimorder",
        beforeSend: function () {
            $('#email_verificationmessage').hide()
            $('#formsection').addClass("thm-loading");
        },
        data: {
            PostCode: $('#PostalCode').val(),
            Address: $('#AddressLine1').val(),
            EmailAddress: $('#EmailAddress').val()
        },
        success: function (result) {

            if (result.isValid == false) {

                if (result.isValid == false && result.errorcode == 1)
                {
                    $('#email_verificationmessage').html("You have reached the maximum number of SIM orders for this month. Please contact customer services via hello@talkhomemobile.com for further assistance").show();
                }
                if (result.isValid == false && result.errorcode == 2) {
                    $('#email_verificationmessage').html("You have reached the maximum number of SIM orders for this account. Please contact customer services via hello@talkhomemobile.com for further assistance").show();
                }
                if (result.isValid == false && result.errorcode == 3) {
                    $('#email_verificationmessage').html("You have reached the maximum number of SIM orders for this account. Please contact customer services via hello@talkhomemobile.com for further assistance").show();
                }
                if (result.isValid == false && result.errorcode == 4) {
                    $('#email_verificationmessage').html("You have reached the maximum number of SIM orders for this month. Please contact customer services via hello@talkhomemobile.com for further assistance").show();
                }



                //$.Toast("Sim limit Exceeds against this Email.", { 'align': 'center', 'position': 'top', 'width': 300, });
                return;
            }
            if (result.regsitered == false && result.confirmed == false) {

                $('#otp_verification').show();
                //$("#resendsendOTP").trigger("click");
                $('#pswddiv').show();
                $('#SignUpSubmit').show();
                $("#SignUpSubmit").attr("disabled", true);
                $('#BTN_VALIDATESIMORDER').hide();
                $("#EmailAddress").attr("readonly", true);
                $("#PostalCode").attr("readonly", true);
                $("#destinationdiv").attr("readonly", true);
                $("#destinations").attr("disabled", true);
                
                starttimer();
                $("#resendsendOTP").attr("disabled", true);

                

                return;
            }
            if (result.regsitered == true && result.confirmed == false) {

                $("#EmailAddress").attr("readonly", true);
                $("#PostalCode").attr("readonly", true);
                $("#destinationdiv").attr("readonly", true);
                $('#otp_verification').show();
                //$("#resendsendOTP").trigger("click");
                $('#SignUpSubmit').show();
                $("#SignUpSubmit").attr("disabled", true);
                $('#BTN_VALIDATESIMORDER').hide();
                $("#resendsendOTP").attr("disabled", true);
                starttimer();
                $("#resendsendOTP").attr("disabled", true);
                $("#destinations").attr("disabled", true);
                return;

            }
            if (result.regsitered == true && result.confirmed == true && result.isValid == true) {


                $('#BTN_VALIDATESIMORDER').hide();
                $('#SignUpSubmit').show();
                $("#SignUpSubmit").attr("disabled", false);
                $('#OrderSimForm').submit();
                return;

            }
        },
        complete: function() {
            $('#formsection').removeClass("thm-loading");
        },
        error: function (xhr, error, status) {
            $('#formsection').removeClass("thm-loading");
            $('#MobileNumberleLoader').hide();
        }
    });
});

$("#digit-4,#digit-3,#digit-2,#digit-1").keyup(function () {

    var data = {
        digit1: $('#digit-1').val(),
        digit2: $('#digit-2').val(),
        digit3: $('#digit-3').val(),
        digit4: $('#digit-4').val(),
        EmailAddress: $('#EmailAddress').val(),


    };

    if (!data.digit1 || !data.digit2 || !data.digit3 || !data.digit4) {
        return;
    }

    $('#otp_verificationmessage').hide();
    $('#otp_verificationsucessmessage').hide();
    $("#SignUpSubmit").attr("disabled", true);

        $.ajax({
            type: "POST",
            url: "/Verifyuserotp",
            beforeSend: function () {
                $('#formsection').addClass("thm-loading");
                $('#otp_verificationsucessmessage').hide();
                $('#otp_verificationmessage').hide();
            },
            data: data ,
            success: function (result) {

                if (result.response == true) {

                    $('#otp_verification').hide();

                    $('#otp_verificationsucessmessage').html("Verified Sucessfully!").show();
                    if ($('#OrderSimForm').valid()) {
                        $('#formsection').addClass("thm-loading");
                        $('#OrderSimForm').submit();
                    }
                    else {
                        $("#SignUpSubmit").attr("disabled", false);
                    }
                }

                else {

                    $('#otp_verification').show();
                    $('#otp_verificationmessage').html("Invalid PIN.<br>Please Enter valid PIN or Resend OTP.").show();
                    $('#digit-1').val("").focus(),
                        $('#digit-2').val("").attr('disabled', 'disabled'),
                        $('#digit-3').val("").attr('disabled', 'disabled'),
                        $('#digit-4').val("").attr('disabled', 'disabled')

                    $("#SignUpSubmit").attr("disabled", true);
                    
                }

            },
            error: function (xhr, error, status) {
                $('#formsection').removeClass("thm-loading");
            },
            complete: function () {
                $('#formsection').removeClass("thm-loading");
            }
        });
    
});

$("#resendsendOTP").click(function (e) {

    if ($('#resendsendOTP').prop('disabled', true) )
    {
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
function starttimer() {
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
}
