
$("#AccountSummary").click(function () {

    $("#loading-bar-tab").fadeIn();
    AppendUrl(this);
    $.ajax({
        type: "GET",
        url: "/Account/History/THM-payments/1",

        error: function (e) {
            $.Toast("Some Error Occured",
                { 'align': 'center', 'position': 'top', 'width': 300, }
            );
            $("#loading-bar-tab").fadeOut();
        },
        success: function (partialViewData) {
            $("#loading-bar-tab").fadeOut();
            $('#PaymentHistory').html(partialViewData);
        }
    });
});

$("#acDetailsTab").click(function () {

    $("#loading-bar-tab").fadeIn();
    AppendUrl(this);
    $.ajax({
        type: "GET",
        url: "/Account/EditAccountDetail",
        data: { productCode: "THM" },
        error: function (e) {

            $.Toast("Some Error Occured",
                { 'align': 'center', 'position': 'top', 'width': 300 }
            );


            $("#loading-bar-tab").fadeOut();

        },
        success: function (response) {

            $(".home-loading-bar").fadeOut();
            if (response.errorcode == 0) {
                $('#acDetailsContent').html(response.View);
            }
            else {
                $.Toast(response.Message,
                    { 'align': 'center', 'position': 'top', 'width': 300, 'duration': 5000 }
                );
            }

        }
    });
});

$("#autoTopupSettingTab").click(function () {

    $("#loading-bar-tab").fadeIn();
    $('#autoTopupSettingContentError').hide();
    AppendUrl(this);
    $.ajax({
        type: "GET",
        url: "/Account/Settings",
        data: { productCode: "THM" },
        error: function (e) {
            $('#autoTopupSettingContentError').show();
            $('#autoTopupSettingContentError').text("An error occurred");
            $("#loading-bar-tab").fadeOut();
        },
        success: function (response) {
            $("#loading-bar-tab").fadeOut();

            if (response.errorcode == 0) {
                $('#autoTopupSettingContent').html(response.View);
            }
            else if (response.errorcode != 0) {
                $('#autoTopupSettingContentError').show();
                $('#autoTopupSettingContentError').text(response.Message);
            }
        }
    });
});
$('#chkQuickTopUp').change(function () {

    $('#hdnAutoTopUpEnabled').val(this.checked);
});

$(function () {
    $("#loading-bar-tab").fadeIn();

    ActiveTab();
    function ActiveTab() {

        var id = window.location.href.split("#")[1];
        $(".thm-tab").removeClass('active');
        if (typeof id !== "undefined") {
            window.history.pushState("data", "Title", window.location.href.replace("#" + id, ""));
            $("#" + id).addClass('active');
            $("#" + id).trigger('click');
        }
        else {

            $("#AccountSummary").addClass('active');
            $("#AccountSummary").trigger('click');

        }
    }
    //$("#loading-bar-tab").fadeOut();
});
var previousTabId = '';
function AppendUrl(control) {


    window.history.pushState("data", "Title", window.location.href.replace("#" + previousTabId, ""));
    var new_url = window.location.href + "#" + control.id;
    window.history.pushState("data", "Title", new_url);
    previousTabId = control.id;

}

//Payment and Porting  
$("#PaymentMethod").click(function () {

    $("#loading-bar-tab").fadeIn();
    $('.pagesloader').show();

    AppendUrl(this);
    $.ajax({
        type: "GET",
        url: "/Account/paymentmethod",
        error: function (e) {
            $('.pagesloader').hide();
            $("#loading-bar-tab").fadeOut();
            $.Toast("Some Error Occured",
                { 'align': 'center', 'position': 'top', 'width': 300, }
            );
        },
        success: function (partialViewData) {
            $("#loading-bar-tab").fadeOut();
            $('#paymentMethodContent').html(partialViewData);
            // $('.pagesloader').hide();
        }
    });
});
$("#PortingDetail").click(function () {
    $("#loading-bar-tab").fadeIn();
    $('.pagesloader').show();
    
    AppendUrl(this);
    $.ajax({
        type: "GET",
        url: "/Account/portdetails",
        error: function (e) {

            $('.pagesloader').hide();
            $("#loading-bar-tab").fadeOut();
            $("#PortingDetailContent").html('<p class="error">Unable to load data .</p>');
        },
        success: function (partialViewData) {
            if (partialViewData.errorcode != 0) {
                $('.pagesloader').hide();
                $("#loading-bar-tab").fadeOut();
                $("#PortingDetailContent").html('<p class="error">Unable to load data .</p>');
            } else {
                $("#loading-bar-tab").fadeOut();
                $('#PortingDetailContent').html(partialViewData.View);
            }
            
           
        }
    });
});

//show payement history

$("#apaymentHistroy").click(function () {
    $("#PaymentHistory").show();
    $("#apaymentHistroy").addClass('activecallandhistorytab');
    $("#callHistory").hide();
    $("#acallHistory").removeClass('activecallandhistorytab');
});

//Show Call history

$("#acallHistory").click(function () {
    $("#callHistory").show();
    $("#acallHistory").addClass('activecallandhistorytab');
    $("#PaymentHistory").hide();
    $("#apaymentHistroy").removeClass('activecallandhistorytab');
});