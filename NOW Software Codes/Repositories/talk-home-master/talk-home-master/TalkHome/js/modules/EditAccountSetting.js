
$("#SubscribeMailingList").on("click", function (e) {

    $("#saveSubs").removeAttr('disabled');
    if (this.checked) {
        $(this).attr("value", "true");
    } else {
        $(this).attr("value", "false");
    }
});


$("#btnresetpassword").click(function (e) {
    validatePassword();
});

$("#saveSubs").click(function () {

    validateupdateuser();
});

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

function ResetPassword() {
    
    var resetpasswordmodule = {};
    resetpasswordmodule.EmailAddress = $("#EmailAddress").val();
    resetpasswordmodule.Password = $("#newpassword").val();   // new password
    resetpasswordmodule.ConfirmPassword = $("#confirmpassword").val();  //re-enter new password 
    resetpasswordmodule.ACId = $("#ACId").val();
    resetpasswordmodule.OldEmail = $("#OldEmail").val();
    resetpasswordmodule.Firstname = $("#Firstname").val();
    resetpasswordmodule.Lastname = $("#Lastname").val()
    $.ajax({
        type: "POST",
        url: "/Account/ResetPassword",
        data: { model: resetpasswordmodule },
        success: function (response) {
            $('#loading-bar-ResetPassword').fadeOut();
            if (response.errorcode == 0) {
                $.Toast(response.Message,
                    { 'align': 'center', 'position': 'top', 'width': 300, 'class': 'sl-notificationSuccess' }
                );
                $("#acDetailsTab").trigger('click');
            }
            else {
                $.Toast(response.Message,
                    { 'align': 'center', 'position': 'top', 'width': 300 }
                );

            }

        },
        error: function (e) {
            $('#loading-bar-ResetPassword').fadeOut();
            $.Toast("An error occured.",
                { 'align': 'center', 'position': 'top', 'width': 300 }
            );
        },
    });
}
function updateuser() {
    $('#loading-bar-updateuser').fadeIn();
    var updateusermodel = {};
    updateusermodel.Email = $("#EmailAddress").val();
    updateusermodel.Password = $("#password").val();
    updateusermodel.ConfirmPassword = $("#ConfirmPassword").val();
    updateusermodel.ACId = $("#ACId").val();
    updateusermodel.OldEmail = $("#OldEmail").val();
    updateusermodel.Firstname = $("#Firstname").val();
    updateusermodel.Lastname = $("#Lastname").val();
    updateusermodel.SubscribeMailingList = $("#SubscribeMailingList").val();
    updateusermodel.__RequestVerificationToken = $("input[name=__RequestVerificationToken]").val();


    $.ajax({
        type: "POST",
        url: "/Account/updateuser",
        data: updateusermodel,

        error: function (e) {
            $('#loading-bar-updateuser').fadeOut();
            $.Toast("An error occured.",
                { 'align': 'center', 'position': 'top', 'width': 300 }
            );
        },
        success: function (response) {
            $('#loading-bar-updateuser').fadeOut();
            if (response != null && response.errorcode == 0) {
                $.Toast(response.Message,
                    { 'align': 'center', 'position': 'top', 'width': 300, 'class': 'sl-notificationSuccess' }
                );
            }
            else {
                $.Toast(response.Message,
                    { 'align': 'center', 'position': 'top', 'width': 300 }
                );
            }
        }
    });
}
function validatePassword() {

    var validator = $("#frmaccountpassword").validate({
        rules: {
            newpassword: {
                minlength: 8,
                required: true
            },
            confirmpassword: {
                minlength: 8,
                required: true,
                equalTo: "#newpassword"
            }
        },
        messages: {
            newpassword: {
                minlength: jQuery.format("Enter at least {0} characters"),
                required: "Please enter new password"
            },
            confirmpassword: {
                required: "Please re-enter new password",
                equalTo: "Enter Confirm Password Same as New Password",
                minlength: jQuery.format("Enter at least {0} characters")
            }
        }
    });


    if (validator.form()) {
        ResetPassword();
    } else {
        return false;
    }
}
function validateupdateuser() {

    var validator = $("#frmupdateuser").validate({
        rules: {
            EmailAddress: {
                required: true,
                email: true
            }
        },
        messages: {
            EmailAddress: {
                required: "Please enter password",
                email:"Please enter valid email"
            }
        }
    });


    if (validator.form()) {
        updateuser();
    } else {
        return false;
    }
}