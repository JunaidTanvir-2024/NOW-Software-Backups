/**
 * Defines the functionalities for rates/minutes pages.
 * Populates the value after a select menu option was clicked.
 * 
 * @author micheled
 * Created on 23.07.2017
 * 
 */
'use strict';
var Rates = function () {

    var selectedOptions = $('#minutes').find(':selected');
    if (selectedOptions.length > 0 && !selectedOptions.hasClass('default')) {
        
        $('.empty').hide();
    }



    if ($('#appRates').length > 0) {
        $('#appRates').select2({
            placeholder: 'Select an option',
            closeOnSelect: true
        });
    }

    $('#minutes,#appRates').change(function () {
        
        $('.empty').hide();
        var countryCode = $(this).find(':selected').attr('value');
        var euro = $(this).find(':selected').data("europe");

        if (euro == "EU"){
            $(".plan-tabs").show();
            $("#plan-rates").show();
            $("#rates").hide();
            $("#eu-footnote").show();

            if ($(".with-plan").hasClass("alt")){
                $(".with-plan").addClass("active");
                $(".with-plan").removeClass("alt");
                $(".pay-go").addClass("alt");
                $(".pay-go").removeClass("active");
            } 
        }
        else{
            $(".plan-tabs").hide();
            $("#plan-rates").hide();
            $("#rates").show();
            $("#eu-footnote").hide();
        }
          

        $('#rates').find('[data-destination]').hide(25);
        $('#rates').find('[data-destination=' + countryCode + ']').first().fadeIn(50);
        $('#appRates').select2('close');
    });



  

    $(".rates-tabs .tab").click(function(e){
        e.preventDefault();
        var plan = $(".with-plan");
        var paygo = $(".pay-go");

        if ($(this).hasClass("with-plan") && !$(this).hasClass("active")) {
            $(this).addClass("active");
            $(this).removeClass("alt");
            $(paygo).addClass("alt");
            $(paygo).removeClass("active");
            $("#plan-rates").show();
            $("#rates").hide();
        }
        else if ($(this).hasClass("pay-go") && !$(this).hasClass("active")) {
            $(this).addClass("active");
            $(this).removeClass("alt");
            $(plan).addClass("alt");
            $(plan).removeClass("active");
            $("#plan-rates").hide();
            $("#rates").show();
        }
    });

};
