
var SetRemoveDefaultCardValues_myPayment;
var SetCardValuesAsDefult_myPayment;
var SecurityCode_Validation_myPayment;
var SetRemoveCardValues_myPayment;

var removeCardModal = $('[data-remodal-id=RemoveCard-popup-myPayment]').remodal();
var removeDefaultCardModal = $('[data-remodal-id=RemoveDefaultCard-popup-myPayment]').remodal();
var setCardModal = $('[data-remodal-id=CV2-popup-myPayment]').remodal();

$(document).ready(function () {
   
    $('.pagesloader').hide();
    ShowMyPaymentMethods_myPayment();
    function ShowMyPaymentMethods_myPayment() {
        $('.pagesloader').show();
        $.ajax({
            type: "GET",
            url: "/account/GetCustomerPaymentMethod",
            success: function (result) {
                
                if (result != null) {
                    if (result.errorCode == 0 && result.status == "Success") {

                        if (result.payload != null && result.payload.paymentMethodResponses.length > 0) {

                            var item = result.payload.paymentMethodResponses.find(item => item.isPrimary === false);
                            if (!jQuery.isEmptyObject(item)) {
                                var cardsHtML = "<div class=\"section-head mb-3\">" +
                                    "<h3>Other Payment Method(s)</h3></div>" +
                                    "<div class=\"row gutters-10 mb-3\">";
                            }

                            $.each(result.payload.paymentMethodResponses, function (key, val) {

                                if (val.isPrimary) {

                                    var defaultcardHtml = "<div class=\"section-head mb-3\"><h3>Default Payment Method</h3></div>" +
                                        "<div class=\"row gutters-10 mb-4\"> <div class=\"col-lg-6 col-md-8\">" +
                                        "<div class=\"quick-card\"> <div class=\"row\"> <div class=\"col-sm-8 mb-sm-0 mb-1\">" +
                                        "<span class=\"cardscheme\">" + val.card.cardScheme.toLowerCase() + "</span> " + "<h3>" + val.card.maskedPan + "</h3></div>" +
                                        "<div class=\"col-sm-4 align-self-center d-flex justify-content-sm-end mt-sm-0 mt-1\">" +
                                        "<button data-token=" + val.card.cardToken + " type=\"button\" onclick=\"SetRemoveDefaultCardValues_myPayment(this)\"   class=\"btn co-btn btn-sm btn-alt\">Remove</button>" +
                                        "</div>" +
                                        "</div></div></div></div>";


                                    $('#MyPayments_DefaultCard_MaskedPan').html(defaultcardHtml);
                                }
                                else {

                                    cardsHtML += " <div class=\"col-lg-4 col-md-6\"> <div class=\"quick-card\">" +
                                        "<span  class=\"cardscheme\">" + val.card.cardScheme.toLowerCase() + "</span> " + "<h3>" + val.card.maskedPan + "</h3>" +
                                        " <div class=\"mt-2\">" +
                                        "<button data-token=" + val.card.cardToken + " type=\"button\" onclick=\"SetRemoveCardValues_myPayment(this)\" class=\"btn co-btn btn-sm btn-alt mr-1\">Remove</button> <button type=\"button\" class=\"btn co-btn btn-sm\" onclick=\"SetCardValuesAsDefult_myPayment(this)\"  data-token=" + val.card.cardToken + ">Make Default</button>" +
                                        "</div>" +
                                        "</div></div>";

                                }
                                $('.pagesloader').hide();
                            })
                            if (!jQuery.isEmptyObject(item)) {
                                cardsHtML += "</div>";
                                $('#MyPayments_OtherCardsContent').html(cardsHtML);
                            }
                            else
                                $('#MyPayments_OtherCardsContent').html('');

                        }
                        else {


                            $('#NoCard').html('<p class="error">No Card Found. Please make one transaction</p>');
                            $('#MyPayments_OtherCardsContent,#MyPayments_DefaultCard_MaskedPan').html('');
                            $('.pagesloader').hide();
                        }
                    }
                    else {
                        $('#NoCard').html('<p class="error">Unable to found record.</p>');
                        $('.pagesloader').hide();
                    }
                }
                else { 
                    $('#NoCard').html('<p class="error">Unable to found record.</p>');
                    $('.pagesloader').hide();
                }
            },
            error: function () {
                $('.pagesloader').hide();
                //$.Toast("Some Error Occured",
                //    { 'align': 'center', 'position': 'top', 'width': 300, }
                //);
                $('#NoCard').html('<p class="error">Unable to found record.</p>');
                $('#MyPayments_OtherCardsContent,#MyPayments_DefaultCard_MaskedPan').html('');
            }
        });
    }

    SetRemoveDefaultCardValues_myPayment = function (control) {

        $('#CardData_myPayment').val($(control).data("token"));

        $('#RemoveDefaultCard-CardNumber-myPayment').html($(control).parent().parent().parent().children()[0].children[0].children[1].innerHTML);

        removeDefaultCardModal.open();

    }

    SetCardValuesAsDefult_myPayment = function (control) {


        $('#CardData_myPayment').val($(control).data("token"));

        $('#CV2-CardNumber-myPayment').html($(control).parent().parent().children()[1].innerHTML);

        ResetForm('SetDefaultCardForm');
        setCardModal.open();

    }

    function ResetForm(formid) {

        $("#" + formid).validate().resetForm();
        $('#' + formid)[0].reset();

        //reset unobtrusive validation summary, if it exists
        $("#" + formid).find("[data-valmsg-summary=true]")
            .removeClass("validation-summary-errors")
            .addClass("validation-summary-valid")
            .find("ul").empty();

        //reset unobtrusive field level, if it exists
        $("#" + formid).find("[data-valmsg-replace]")
            .removeClass("field-validation-error")
            .addClass("field-validation-valid")
            .empty();

        //Remove the Fields border
        $("#" + formid).find('input').removeClass('input-validation-error');
        $("#" + formid).find('select').removeClass('input-validation-error');
    }
    $("#BtnCnfrmDefault_myPayment").on("click", function () {
        if (!$('#SetDefaultCardForm').valid()) {
            return false;
        }
        else {
            
         
            //$('#BtnCnfrmDefault_myPayment,#setDefaultCard').attr('disabled', true);
            $.ajax({
                type: "POST",
                url: "/account/SetDefaultCard",
                data: { defaultCardCV2: $('#SecurityCode').val(), cardToken: $('#CardData_myPayment').val() },
                beforeSend: function (xhr) {

                   

                },
                success: function (response) {

                    if (response != null) {
                        if (response.status == true) {
                            setCardModal.close();
                            ShowMyPaymentMethods_myPayment();
                        }
                        else {
                            $('#CnfrmDefaultFailureMsg_myPayment').text(response.message);
                            $('#CnfrmDefaultFailureAlert_myPayment').slideDown().delay(4000).slideUp();
                        }
                    }
                    else {
                        $('#CnfrmDefaultFailureMsg_myPayment').text('Something went wrong on server.');
                        $('#CnfrmDefaultFailureAlert_myPayment').slideDown().delay(4000).slideUp();
                    }

                },
                complete: function (xhr, status) {

                  //  $('.removeCards').attr('disabled', false);

                   $('#BtnCnfrmDefault_myPayment,#setDefaultCard').attr('disabled', false);

                },
                error: function (xhr, status, error) {
                    $('#CnfrmDefaultFailureMsg_myPayment').text('Something went wrong on server.');
                    $('#CnfrmDefaultFailureAlert_myPayment').slideDown().delay(4000).slideUp();
                }
            });
        }
    });

    $("#BtnRemoveCard_myPayment").on("click", function (e) {
       
       
        $('#BtnRemoveCard_myPayment').attr('disabled', 'disabled');
        $('#RemoveCard').attr('disabled', 'disabled');
        $.ajax({
            type: "POST",
            url: "/account/RemoveCard",
            data: { cardToken: $('#CardData_myPayment').val() },
            beforeSend: function (xhr) {

               
            },
            success: function (response) {
                $('#BtnRemoveCard_myPayment').removeAttr('disabled');
                $('#RemoveCard').removeAttr('disabled');
                if (response != null) {
                    if (response.status == true) {

                        removeCardModal.close();
                        ShowMyPaymentMethods_myPayment();

                        $.Toast(response.message,
                            { 'align': 'center', 'position': 'top', 'width': 300, 'class': 'sl-notificationSuccess' }
                        );
                    }
                    else {

                        $.Toast("Something went wrong on server",
                            { 'align': 'center', 'position': 'top', 'width': 300, }
                        );
                        //$('#RemoveCardFailureMsg_myPayment').text(response.message);
                        //$('#RemoveCardFailureAlert_myPayment').slideDown().delay(4000).slideUp
                    }
                }
                else {
                    $.Toast("Something went wrong on server.",
                        { 'align': 'center', 'position': 'top', 'width': 300, }
                    );
                    //$('#RemoveCardFailureMsg_myPayment').text('Something went wrong on server.');
                    //$('#RemoveCardFailureAlert_myPayment').slideDown().delay(4000).slideUp();;
                }

            },
            complete: function (xhr, status) {
               // $('.removeCards').removeAttr('disabled');
               // $('#BtnRemoveCard_myPayment,#RemoveCard').attr('disabled', false);
                $('#BtnRemoveCard_myPayment').removeAttr('disabled');
                $('#RemoveCard').removeAttr('disabled');
            },
            error: function (xhr, status, error) {
                $('#BtnRemoveCard_myPayment').removeAttr('disabled');
                $('#RemoveCard').removeAttr('disabled');
                //$.Toast("Something went wrong on server",
                //    { 'align': 'center', 'position': 'top', 'width': 300, }
                //);

            }
        });
    });
    $('#BtnRemoveDefaultCard_myPayment').click(function (e) {
        
        $('#BtnRemoveDefaultCard_myPayment').attr('disabled', 'disabled');
        $('#RemoveDefaultCard').attr('disabled', 'disabled');
     
        $.ajax({
            url: "/account/RemoveDefaultCard",
            type: "POST",
            data: { cardToken: $('#CardData_myPayment').val() },
            beforeSend: function (xhr) {

               

            },
            success: function (response) {

                if (response != null) {
                    if (response.status == true) {

                        removeDefaultCardModal.close();
                        //$('#RemoveDefaultCard-popup-myPayment').close();
                        ShowMyPaymentMethods_myPayment();
                        $.Toast(response.message,
                            { 'align': 'center', 'position': 'top', 'width': 300, 'class': 'sl-notificationSuccess' }
                        );
                    }
                    else {
                        $.Toast("Something went wrong on server",
                            { 'align': 'center', 'position': 'top', 'width': 300, }
                        );
                      
                    }
                }
                else {
                    $.Toast("Something went wrong on server",
                        { 'align': 'center', 'position': 'top', 'width': 300, }
                    );
                }

            },
            complete: function (xhr, status) {
             //   $('.removeCards').removeAttr('disabled', 'disabled');
                // $('#BtnRemoveDefaultCard_myPayment,#RemoveDefaultCard').attr('disabled', false);
                $('#BtnRemoveDefaultCard_myPayment').removeAttr('disabled');
                $('#RemoveDefaultCard').removeAttr('disabled');

            },
            error: function (xhr, status, error) {

                //$.Toast("Something went wrong on server",
                //    { 'align': 'center', 'position': 'top', 'width': 300, }
                //);
            }
        });
    });


    $(".remodal-cancel,.remodal-close").on("click", function () {
        var ref = this;
        if (ref.id == "setDefaultCard")
            setCardModal.close();
        else if (ref.id == "RemoveDefaultCard")
            removeDefaultCardModal.close();
        else
            removeCardModal.close();
    });

    SetRemoveCardValues_myPayment = function (control) {
        
        $('#CardData_myPayment').val($(control).data("token"));

        $('#RemoveCard-CardNumber-myPayment').html($(control).parent().parent().children()[1].innerHTML);

        removeCardModal.open();


    }

    SecurityCode_Validation_myPayment = function (evt) {

        var theEvent = evt || window.event;
        var TotalLength = $("#" + evt.currentTarget.id).val().length;


        // Handle paste
        if (theEvent.type === 'paste') {
            key = event.clipboardData.getData('text/plain');
        } else {
            // Handle key press
            var key = theEvent.keyCode || theEvent.which;
            key = String.fromCharCode(key);
        }

        var regex = /[0-9]/;

        if (!regex.test(key)) {
            theEvent.returnValue = false;
            if (theEvent.preventDefault) theEvent.preventDefault();
        }

        // length
        if (TotalLength >= 4) {
            theEvent.returnValue = false;
            theEvent.preventDefault();
        }
    }


});
