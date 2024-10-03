
$("#btnAddCompetition").click(function () {
    $('#btncompetitonspinner').show();
    validateCompition();
});


// Jqeury Validete
function validateCompition() {
    var validator = $("#frmcompition").validate({
        rules: {
            Email: {
                required: true,
                email: true   
            },
            Name: {
                required: true,
                maxlength: 200
            }
        },
        messages: {
            Email: {
                required: "Please enter email address."
            },
            Name: {
                required: "Please enter name.",
                maxlength: "Maxlength is 200 characters"
            }
        }
    });

    if (validator.form()) {
        isRegistered();
    } else {
        $('#btncompetitonspinner').hide();
        return false;
    }
}


function addCompitionUser() {
    var model = {};
    model.Email = $("#email").val();
    model.Name = $("#name").val();
    model.Promoname = "thmcompnov19";
    if ($('#chkSignUpOffer').is(":checked")) {
        model.signme = true;
    } else {
        model.signme = false;
    }
    $.ajax({
        type: "POST",
        url: "/register-compition",
        dataType: 'json',
        data: { model },
        success: function (data) {
            if (data.status == true) {
                $("#frmcompition").trigger("reset");
                $('#mainsection').hide();
                $('#thankssection').show();
                $('#btncompetitonspinner').hide();
            }
        },
        error: function () {
            alert("error");
        }
    });
}

function isRegistered() {
    var model = {};
    model.Email = $("#email").val();
    model.Promoname = "thmcompnov19";
    $.ajax({
        type: "POST",
        url: "/isregistered",
        dataType: 'json',
        data: { Email: $("#email").val(), Promoname:"thmcompnov19"},
        success: function (data) {
            if (data.status == true) {
                addCompitionUser();
            } else {
                $("#lblemail").show();
                $('#btncompetitonspinner').hide();
            }
        },
        error: function () {
            alert("error");
        }
    });
}

$('#email').keyup(function () {
    $('#lblemail').hide();
})