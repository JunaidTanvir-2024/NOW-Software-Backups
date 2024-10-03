$("#autoTopUpSwitch").click(function () {
   
    if (this.checked) {
        $('#autoTopUpSwitch').val("True");
    }
    else {
        $('#autoTopUpSwitch').val("False");
    }

});

$("#btnAutoTopUpSetting").click(function () {
    
    validateAutoTopUp();
});


function autotopup() {
    $("#loading-bar-AutoTopUp").fadeIn();
    $("#btnAutoTopUpSetting").prop("disabled", true);
    $("#btnAutoTopUpSetting").css("background-color", "#a8ab18");
    //AppendUrl(this);
    var AutoTopUpModel = {};
    AutoTopUpModel.ProductCode = "THM";
    AutoTopUpModel.Threshold = $("#Threshold").val();
    AutoTopUpModel.TopUpId = $("#TopUpId").val();
    AutoTopUpModel.AutoTopUp = $("#autoTopUpSwitch").val();
    var token = $('input[name="`__RequestVerificationToken`"]').val();
    $.ajax({
        type: "POST",
        url: "/Account/AutoTopUpSettings",
        data: { model: AutoTopUpModel },
        success: function (response) {
            
            $("#loading-bar-AutoTopUp").fadeOut();
            $("#btnAutoTopUpSetting").prop("disabled", false);
            $("#btnAutoTopUpSetting").css("background-color", "#dee228");

            if (response.errorcode == 0) {
                $.Toast(response.Message,
                    { 'align': 'center', 'position': 'top', 'width': 300, 'class': 'sl-notificationSuccess' }
                );
            }
            else {
                
                $.Toast(response.Message,
                    { 'align': 'center', 'position': 'top', 'width': 300 }
                );
                $("#autoTopupSettingTab").trigger('click');
            }

        },
        error: function (e) {
            $("#loading-bar-AutoTopUp").fadeOut();
            $.Toast("An Error Occured",
                { 'align': 'center', 'position': 'top', 'width': 300, }
            );

        }

    });
}

function validateAutoTopUp() {

    var validator = $("#frmautotopup").validate({
        rules: {
            Threshold: {
                required: true,
            }
        },
        messages: {
            Threshold: {
                required: "Balance threshold should be less than or equal to 30."
            }
        }
    });


    if (validator.form()) {
        autotopup();
    } else {
        return false;
    }
}