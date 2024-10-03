var msisdnCheck;
var paccheck;
var InfoModal = $('[data-remodal-id=BtnSwitchingInfo]').remodal();
var PortOutModal = $('[data-remodal-id=PortOutButton]').remodal();
var PortInModal = $('[data-remodal-id=PortInButton]').remodal();
var CancelModal = $('[data-remodal-id=CancelConfirmModal]').remodal();
var PortModalDisabled = $('[data-remodal-id=disabledPortModal]').remodal();


var Porting = function () {

   
    $('#PortInButtonDisabled').on('click', function () {
        PortModalDisabled.open();

    });
    $('#PortOutButtonDisabled').on('click', function () {

        PortModalDisabled.open();
    });
    GetUserPortDetails();
    // Variable use in disabled date field checks

    $('#PortInButton').show();
    $('#PortOutButton').show();
    // Initialise the datepicker
    $('#PortDate').dcalendarpicker({ format: 'yyyy-mm-dd' });

    // Initialise the datepicker
    $('#PortOutDate').dcalendarpicker({ format: 'yyyy-mm-dd' });

    var portOutMethod = "portout";

    $("#BtnSwitchingInfo").click(function (e) {

        InfoModal.open();

    });

    $("#PortOutButton").click(function (e) {

        PortOutModal.open();
    });

    $("#PortInButton").click(function (e) {
       
      
        ResetForm('portInForm');
        $('#PortDate').val(new Date().toISOString().split("T")[0]);
        msisdnCheck = false;
        paccheck = false;
        $("#PortDate").attr('disabled', true);
        $("#PortDate").css('background', '#dddddd');
        PortInModal.open();


    });
    
    $(".closezpop").on('click', function () {

        $('#PortingConfirmModal').removeAttr('checked');
        $('#PortingOutConfirmModal').removeAttr('checked');
        $('#CancelConfirmModal').removeAttr('checked');
        
    });


    $("#PortIn").on('click', function () {

        if (!$("#portInForm").valid()) {
            return;
        }
        $('#PortingConfirmModal').attr('checked', 'checked');

        $('#spnPortInNo').text($('#PortInMsisdn').val());

    })


    $("#PortInButtn").click(function (e) {
      
        e.preventDefault();
        var id = $(this).attr("id");

   
        $("#PortingResult").html("");

        //Confirm Cancel Scenario
        //$('#ConfirmCancel-Section').hide();
        //$('#CancelOK').hide();

        //Confirm Portout Scenario
        //$('#ConfirmPortOut-Section').hide();
        //$('#ConfirmPortIn-Section').hide();
        //$('#PortOutOK').hide();
        //$('#PortInComfirm').hide();

        //disable Date Field
        msisdnCheck = false;
        paccheck = false;
        $("#PortDate").attr('disabled', true);
        $("#PortDate").css('background', '#dddddd');

        switch (id) {
            case "PortInButton":
                $(".portOut-section").hide();
                $('#Switchinginfo-Section').hide();
                $('#PortOutButtonDisabledSection').hide();
                $('#portingHeading').html('Port-In Number');
                ResetForm('portInForm');
                $('#PortDate').val(new Date().toISOString().split("T")[0]);
                $(".portIn-section").show();
                break;
            case "PortOutButton":
                $('#Switchinginfo-Section').hide();
                $('#PortOutButtonDisabledSection').hide();
                $(".portIn-section").hide();
                $('#portingHeading').html('Port Out of Talk Home');
                ResetForm('portOutForm');
                $('#PortOutDate').val(new Date().toISOString().split("T")[0]);
                $(".portOut-section").show();
                break;
            case "BtnSwitchingInfo":
                $(".portOut-section").hide();
                $(".portIn-section").hide();
                $('#PortOutButtonDisabledSection').hide();
                $('#portingHeading').html('Switching information');
                $('#Switchinginfo-Section').show();
                break;
            case "PortOutButtonDisabled":
                $(".portOut-section").hide();
                $(".portIn-section").hide();
                $('#Switchinginfo-Section').hide();
                $('#portingHeading').html('Information');
                $('#PortOutButtonDisabledSection').show();
                break;
            case "PortInButtonDisabled":
                $(".portOut-section").hide();
                $(".portIn-section").hide();
                $('#Switchinginfo-Section').hide();
                $('#portingHeading').html('Information');
                $('#PortOutButtonDisabledSection').show();
                break;
        };

        
        $("#CreditModalContent").show();
    });

    $("#PortOutOK").on("click", function () {
       
        //$("#PortingSpinner").show();
        //$("#PortingModalContent").hide();
        $('#portoutloader').show();
        $.ajax({
            type: "POST",
            url: "/account/" + portOutMethod + "?codeTypes=" + $("input[name='CodeType']:checked").val() + "&Date=" + $('#PortOutDate').val() + "&Reason=" + $('#SelectReason').val(),
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=utf-8',
            success: function (result) {
           
                $('#portoutloader').hide();
                if (result != null) {
                    $("#PortingResult").html("<label class=\"alert alert-warning\">" + (result.message == null ? "Some Error Occured" : result.message =="Number Port-Out request generted successfully."? "Your request has been submitted":result.message) + "</label>");

                    if (result.message == null)
                        $.Toast("Some Error Occured",
                            { 'align': 'center', 'position': 'top', 'width': 300, }
                        );
                    else
                        $.Toast((result.message == "Number Port-Out request generted successfully.") ?"Number Port-Out request generated successfully.":result.message,
                            { 'align': 'center', 'position': 'top', 'width': 300, 'class': 'sl-notificationSuccess' }
                        );
                    $('#portOutForm')[0].reset();
                    PortOutModal.close();
                    $('#PortingOutConfirmModal').removeAttr('checked');
                    GetUserPortDetails();
                }
                else {
                    $('#portoutloader').hide();
                    $.Toast("Some Error Occured",
                        { 'align': 'center', 'position': 'top', 'width': 300, }
                    );
                  //  $("#PortingResult").html("<label class=\"alert alert-warning\">Some Error Occured</label>");
                }

            },
            error: function () {
                $('#portoutloader').hide();
                $("#PortingModalContent").fadeIn();
                $("#PortingSpinner").fadeOut();
                $.Toast("Some Error Occured",
                    { 'align': 'center', 'position': 'top', 'width': 300, }
                );
                
                //$("#PortingResult").html("<label class=\"alert alert-danger\">Some Error Occured</label>");

            }
        });

    });

    $('#PortOutDate,#PortDate').click(function () {
        
        $.each($('.calendar-dates'), function (key, val) {
           
            if (!$(this).find('span.current').hasClass('selected')) {
                $(this).find('span.selected').removeClass('selected');
                $(this).find('span.current').addClass('selected');
            }
        });

    });


    $("#PortInComfirm").on("click", function () {

        //AJAX call //
        if (!$("#portInForm").valid()) {
            return;
        }
        $('#portInloader').show();

        var msisdn = $("#PortInMsisdn").val();
        var scheduleDate = $("#PortDate").val();
        var pacCode = $("#PAC").val();
        var reason = Number($("#SelectReason").val());
        var data = { PortMsisdn: msisdn, UserPortingDate: scheduleDate, Code: pacCode };

        $.ajax({
            type: "POST",
            url: "/account/portin",
            dataType: 'json',
            data: data,
            contentType: 'application/x-www-form-urlencoded; charset=utf-8',
            success: function (result) {
                $('#portInloader').hide();
                $("#PortingModalContent").fadeIn();
                $("#PortingSpinner").fadeOut();
            
                $("#PortingResults").height("unset");

                if (result != null) {

                    if (result.message == null)
                        $.Toast("Some Error Occured",
                            { 'align': 'center', 'position': 'top', 'width': 300, }
                        );
                    else
                        $.Toast((result.message == "Number Port-In request generted successfully.") ?"Number Port-In request generated successfully.":result.message,
                            { 'align': 'center', 'position': 'top', 'width': 300, 'class': 'sl-notificationSuccess' }
                        );
                      $('#portInForm')[0].reset();

                    $('#PortingConfirmModal').removeAttr('checked');
                    PortInModal.close();
                    GetUserPortDetails();
                }
                else {
                    $('#PortingConfirmModal').removeAttr('checked');
                    $.Toast("Some Error Occured",
                        { 'align': 'center', 'position': 'top', 'width': 300, }
                    );
                }

            },
            error: function () {
                $('#portInloader').hide();
                $.Toast("Some Error Occured",
                    { 'align': 'center', 'position': 'top', 'width': 300, }
                );
            }
        });
    });

    $(".porting-modal-header .close, #CloseButton").on("click", function () {
        $(".modal").hide();
    });

    $("#PortInCheck").on("click", function (e) {
        $(".portIn-sim-section").toggle();

        if ($(this).is(':checked')) {
            $("#PortInMsisdn").attr('required', 'required');
            $("#PortDate").attr('required', 'required');
            $("#PAC").attr('required', 'required');
            //$("#SimSwapCheck").hide();
        }
        else {
            $("#PortInMsisdn").removeAttr('required');
            $("#PortDate").removeAttr('required');
            $("#PAC").removeAttr('required');

            $("#PortInMsisdn").val("");
            $("#PortDate").val("");
            $("#PAC").val("");
            //$("#SimSwapCheck").show();
        }

    });

}();

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

function ShowConfirmPortOutModal() {

    //AJAX call //
    if (!$("#portOutForm").valid()) {
        return;
    }
    $.ajax({
        type: "GET",
        url: "/Account/GetSwitchingInfo",
        beforeSend: function () {
            
        },
        success: function (result) {

            
          
            if (result != null) {
                if (result.payload != null) {

                    $('#txtSwtingInfoBalance').text(result.payload.Balance);
                    $("#PortingOutConfirmModal").attr("checked", "checked");
                
                    //$('#PortOutOK').show();

                }
                else {
                    $.Toast("Some Error Occured",
                        { 'align': 'center', 'position': 'top', 'width': 300, }
                    );
                    
                }
            }
            else {
               
                $.Toast("Some Error Occured",
                    { 'align': 'center', 'position': 'top', 'width': 300, }
                );
            }
        },
        error: function () {
            //$(".portOut-section").hide();
            //$("#PortingModalContent").show();
         
            $.Toast("Some Error Occured",
                { 'align': 'center', 'position': 'top', 'width': 300, }
            );
        },
        complete: function () {
            $("#PortingModalContent").fadeIn();
            $("#PortingSpinner").fadeOut();
        }
    });
}

function ShowConfirmPortInModal() {
   
    if (!$("#portInForm").valid()) {
        return;
    }
    $('#spnPortInNo').text($('#PortInMsisdn').val());
    //$(".portIn-section").hide();
    //$('#ConfirmPortIn-Section').show();
    //$('#PortInComfirm').show();
    //$('#PortIn').hide();
}

function GetUserPortDetails() {
    
    $('.pagesloader').show();
    $.ajax({
        type: "GET",
        url: "/account/GetUserPortDetials",
        success: function (result) {
            if (result != null) {
               
                if (result.errorCode == 0 && result.status == "Success") {
                   
                    if (result.payload.PortingRequestList != null) {
                       
                  
                        var html = '';
                        $.each(result.payload.PortingRequestList, function (key, val) {
                            html += "<tr><th scope=\"row\">" + (key+1) + "</th>"+
                                "<td> " + (val.PortType == 1 ? "Port In" : (val.PortType == 2 ? "Port In " : "Port Out ")) + "</td > " +
                                "<td>" + (val.PortMsisdn == null ? "" : val.PortMsisdn) + "</td>" +
                                "<td>" + (val.Code == null ? "..." : val.Code) + "</td>";

                            if (val.IsCancelled == false && val.IsError == false)
                            {
                                //$('#PortInButton').css('background', 'rgb(177, 199, 11) !important');
                                //$("#PortInButton").attr('disabled', 'disabled');
                                $('#PortInButton').hide();
                                $("#PortOutButton").hide()
                                //$("#PortOutButton").css('background', 'rgb(177, 199, 11) !important');
                                //$("#PortOutButton").attr('disabled', 'disabled');
                                $('#PortInButtonDisabled').show();
                                $('#PortOutButtonDisabled').show();
                             
                            }

                            if (val.CodeType == 0) {
                                html += "<td>....</td>";
                            }
                            else {
                                html += "<td>" + (val.CodeType == 1 ? "PAC" : "STAC") + "</td>";
                            }
                          

                            var ExpiryDate = new Date();

                            if (val.ExpiryDate != null) {
                                ExpiryDate = new Date(parseInt(val.ExpiryDate.substr(6)));
                                html += "<td>" + ExpiryDate.getDate() + "/" + (ExpiryDate.getMonth() + 1) + "/" + ExpiryDate.getFullYear() + "</td>";
                            }
                            else {
                                html += "<td>N/A</td>";
                            }
                            if (isNullOrWhitespace(val.Status)) {
                                html += "<td>Requested</td>";
                            }
                            else if (val.Status == "Error") {
                                if (val.DaemonErrorMessage == null) {
                                    html += "<td><span class=\"label-danger\">Error</span></td>";
                                }
                                else {
                                    html += "<td><span class=\"label-danger protip\" data-showError=\"Yes\"  title='" + val.DaemonErrorMessage + "'>" + val.Status + "</span></td>";
                                }
                            }
                            else {
                                html += "<td>" + val.Status + "</td>";
                            }
                            if (val.PortType === 1 || val.PortType === 2) {
                                if ((val.PortType ===1 || val.PortType ===2) && val.IsCancelled == false && val.IsError == false) {
                                    html += "<td><button title=\"Click to cancel request\" class=\"btn co-btn\" style=\"min-width:90px !important; background:rgb(227, 255, 25) !important\" onclick=\"ConfirmCancel(" + val.Id + ")\">Cancel</button></td>";
                                } else {
                                    html += "<td></td>";
                                }
                        
                            }
                            else {
                                if (val.PortType == 3 && val.IsError == false && val.IsCancelled == false) {
                                    html += "<td><button title=\"Click to cancel request\" class=\"btn co-btn\" style=\"min-width:90px !important; background:rgb(227, 255, 25) !important\" onclick=\"ConfirmCancel(" + val.Id + ")\">Cancel</button></td>";
                                } else {
                                    html += "<td></td>";
                                }
                              
                            }
                            html += "</tr>";
                        });
                        $('#porting-information-tbody').html(html);
                        $('.porting-information-table').show();
                        $('.pagesloader').hide();
                    }
                    else {
                        $("#PortRecord").html('<p class="error">No record found</p>');
                        $('.porting-information-table').hide();
                        $('.pagesloader').hide();
                    }
                }
                else {
                    $('.pagesloader').hide();
                    $("#PortRecord").html('<p class="error">Unable to load data .</p>');
                    //$.Toast("Some Error Occured",
                    //    { 'align': 'center', 'position': 'top', 'width': 300, }
                    //);
                    $('#porting-information-tbody').html('');
                }
            }
            else {
                $('.pagesloader').hide();
                $("#PortRecord").html('<p class="error">No record found</p>');
                $('#porting-information-tbody').html('');
            }
        },
        error: function () {
            //$.Toast("Some Error Occured",
            //    { 'align': 'center', 'position': 'top', 'width': 300, }
            //);
            $("#PortRecord").html('<p class="error">Unable to load record.</p>');
            $('#porting-information-tbody').html('');
            $('.pagesloader').hide();
        }
    });

}

function isNullOrWhitespace(input) {
    return !input || !input.trim();
}

function ConfirmCancel(id) {


    //$("#ConfirmPortIn-Section").hide();
    //$("#PortInComfirm").hide();

    $('#hdnRequestID').val(id);


    //$("#PortingResults").height("0");
    $("#PortingResult").html("");

    //$(".portOut-section").hide();
    //$(".portIn-section").hide();
    //$('#Switchinginfo-Section').hide();
    //$('#PortOutButtonDisabledSection').hide();
    //$('#portingHeading').html('Confirmation');


    //Confirm Portout Scenario
    //$('#ConfirmPortOut-Section').hide();
    //$('#PortOutOK').hide();
    CancelModal.open();
    //$('#ConfirmCancel-Section').show();
    //$('#CancelOK').show();

    //$("#CreditModalContent").show();

}

$("#CancelRequest").on("click", function (e) {

    $('#cancleloader').show();
    $.ajax({
        type: "POST",
        url: "/Account/CancelPorting",
        data: { RequestID: $('#hdnRequestID').val() },
        beforeSend: function () {
            $("#PortingSpinner").show();
            $("#PortingModalContent").hide();
        },
        success: function (result) {

            $('#cancleloader').hide();
            $("#PortInButton").removeAttr('disabled');

            $("#PortOutButton").removeAttr('disabled');


            if (result != null) {
                if (result.message == null)
                    $.Toast("Some Error Occured",
                        { 'align': 'center', 'position': 'top', 'width': 300, }
                    );
                else
                    $.Toast(result.message,
                        { 'align': 'center', 'position': 'top', 'width': 300, 'class': 'sl-notificationSuccess' }
                    );

                // $("#PortingResult").html("<label class=\"alert alert-warning\">" + (result.message == null ? "Some Error Occured" : result.message == "Reqeust cancelled successfully." ? "Request cancelled successfully." :result.message) + "</label>");
                $('#portInForm')[0].reset();
                CancelModal.close();
                GetUserPortDetails();
            }
            else {
                $('#cancleloader').hide();
                $.Toast("Some Error Occured",
                    { 'align': 'center', 'position': 'top', 'width': 300, }
                );
            }

        },
        error: function () {
            $('#cancleloader').hide();
            $("#PortingModalContent").fadeIn();
            $("#PortingSpinner").fadeOut();

            $.Toast("Some Error Occured",
                { 'align': 'center', 'position': 'top', 'width': 300, }
            );
        },
        complete: function () {
            $("#PortingModalContent").fadeIn();
            $("#PortingSpinner").fadeOut();
        }
    });

});



function GetSwitchingInfo() {
    $.ajax({
        type: "GET",
        url: "/Account/GetSwitchingInfo",
        beforeSend: function () {
            $('#BtnSwitchinginfo').attr('disabled', true);
        },
        success: function (result) {
            if (result != null) {
                if (result.payload != null) {
                    $('#txtAdditionalCharge').text(result.payload.AdditionalCharge);
                    $('#txtBalance').text(result.payload.Balance);
                    $('#txtHandsetCharge').text(result.payload.HandsetCharge);
                    $('#txtIsPostPaid').html(result.payload.IsPostPaid ? "Yes" : "No");
                    $('#txtTerminateDate').text((result.payload.TerminateDate == null ? 'N/A' : result.payload.TerminateDate));
                    $('#txtTerminateCharge').text(result.payload.TerminationCharge);
                    $('#txtURL').html(result.payload.Url == "" ? 'N/A' : result.payload.Url);
                }
                else {
                    $.Toast("Some Error Occured",
                        { 'align': 'center', 'position': 'top', 'width': 300, }
                    );
                }
            }
        },
        error: function () {
            $.Toast("Some Error Occured",
                { 'align': 'center', 'position': 'top', 'width': 300, }
            );
        },
        complete: function () {
            $('#BtnSwitchinginfo').attr('disabled', false);
        }
    });
}

function validateRechargablePin(evt) {

    var TotalLength = $("#" + evt.currentTarget.id).val().length;

    var theEvent = evt || window.event;

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
    if (TotalLength >= 14) {
        theEvent.returnValue = false;
        theEvent.preventDefault();
    }
}


$("#PortInMsisdn").keyup(function () {
   
    msisdnCheck = true;
    if (this.value.length == 0)
    {
        $("#PortDate").attr('disabled', true);
        $("#PortDate").css('background', '#dddddd');
        msisdnCheck = false;
    }
    else
    {
        if (msisdnCheck == true && paccheck == true)
        {

            $("#PortDate").attr('disabled', false);
            $("#PortDate").css('background', 'transparent');
        }

    }
});

$("#PAC").keyup(function () {
   
    paccheck = true;
    if (this.value.length == 0) {

        $("#PortDate").attr('disabled', true);
        $("#PortDate").css('background', '#dddddd');
        paccheck = false;
    }
    else
    {
        if (msisdnCheck == true && paccheck == true) {
            $("#PortDate").attr('disabled', false);
            $("#PortDate").css('background', 'transparent');
        }
    }
});
