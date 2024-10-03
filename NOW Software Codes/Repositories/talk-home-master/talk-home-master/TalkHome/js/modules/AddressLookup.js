/**
 * Functionalities for the address lookup service.
 * 
 * @author micheled
 * Created on 28.07.2017
 * 
 */
'use strict';
(function () {

    var utils = new Utils,
        uri = '/address-lookup/getaddresses/',
        addresses = [];
    
    /*******************
     * Private methods *
     ******************/

    function cannotFindAddress () {
       
        $('[name=AddressLine1], [name=AddressLine2], [name=City], [name=CountyOrProvince]').val(''); // Empty any previous result
        $('#addressFields').fadeIn(250);
    }

    function setAddressValues (address) {
        $('#addressFields').fadeOut(250);
        $('[name=AddressLine1]').val(address.Line1 + ((address.Line2 != ' ') ? ', ' + address.Line2 : ''));
        $('[name=AddressLine2]').val(address.Line3);
        $('[name=City]').val(address.City);
        $('[name=CountyOrProvince]').val((address.Locality != ' ' ? address.Locality : address.County));
    }

    /**
     * Populates the select menu with the options and sets the first address in the hidden inputs.
     * Note that this function uses a non 0-indexed array. Value 0 is reserved for `I couldn't find my address` option.
     * 
     * @param JSON Array The list of addresses found
     *  
     */
    function found(result) {
        checkoutlookup = true;
        addresses = result; // Caching the current result for later reuse.

        $('#destinations option').remove();
        $('#addressFields').hide();
        $('.lookup-response').find('.error').hide();

        $.each(result, function (i, v) {

            var text = $.grep([v.Line1, v.Line2, v.Line3, v.Line4, v.Locality, v.City, v.County], function (elem, i) {
                return elem != ' ';
            }).join(', ');

            $('#destinations').append('<option value="' + (i+1) + '">' + text + '</option>');
        });

        setAddressValues(result[0]);
        $('#destinations').show();
        $('label[for=destinations]').show();

        $('.lookup-response .waiting').hide();

        //$('#destinations').append('<option value="0">I can\'t find my address</option>');
        $('.destinations-lookup').fadeIn(250);
    }

    /**
     * The request was malformed or there was an error. Alert the user and.
     * It also prompts the user to try again or offer the possibility to manually fill in the fields.
     * 
     */
    function noResultsOrInvalid() {
        
        $('.lookup-response .waiting').hide();
        $('#destinations').hide();

        $('label[for=destinations]').hide();
        $('.lookup-response').find('.error').fadeIn(250);
        $(".error").text("We couldn't find that post code. Please check it and try again");
        //$('#addressFields').show();
    }

    /**
     * Callback to address.io for the array of address objects.
     * 
     * @param string the postcode for the lookup
     *
     */
    function getAddresses (postCode) {
        var fullUri = uri + postCode;
        
        args.success = function (result) {

            if ($.isArray(result)) found(result);
            else noResultsOrInvalid();
        };
        
        utils.Get(fullUri, args);
    }
    
    /**************
     * UI events *
     **************/
    $('#lookupLink').click(function () {
        var postCode = $('#PostalCode').val();
        if (postCode != '') {
            getAddresses(postCode);
            $('.lookup-response .waiting').show();
        }  
    });

    $('#destinations').change(function () {
        if ($(this).find(':selected').attr('value') == 0)
            return cannotFindAddress();

        var i = $(this).find(':selected').attr('value');
        setAddressValues(addresses[i-1]);
    });



    /**************
    * Checkout  *
    **************/



    $('#CheckoutlookupLink').click(function () {
        $("#btnPay360Pay").attr("disabled", true);
        document.getElementById("btnPay360Pay").style.backgroundColor = "#90922d";

        $('#CheckoutFormPay360').valid() 
        var postCode = $('#postalCode').val();
        if (postCode != '') {
            checkoutlookup = true;
            getAddresses_checkout(postCode);
            $('#checkoutlookupspinner').show();
        } else {
            $("#postalcodeerror").show();
            $("#btnPay360Pay").attr("disabled", false);
            document.getElementById("btnPay360Pay").style.backgroundColor = "#dee228";
        }
    });




    $('#postalCode').keyup(function () {
        checkoutlookup = false;
    }); 

    function getAddresses_checkout(postCode) {
        
        var fullUri = uri + postCode;
        $.get(fullUri, function (result) {
            
            if ($.isArray(result)) found_Checkout(result);
            else noResultsOrInvalid_checkout();
        });


        //utils.Get(fullUri, args);
    }

    function found_Checkout(result) {
        
        addresses = result; // Caching the current result for later reuse.
        $('#billingcountry_address option').remove();

        $.each(result, function (i, v) {

            var text = $.grep([v.Line1, v.Line2, v.Line3, v.Line4, v.Locality, v.City, v.County], function (elem, i) {
                return elem != ' ';
            }).join(', ');

            $('#billingcountry_address').append('<option value="' + (i + 1) + '">' + text + '</option>');
        });

        setAddressValues_Checkout(result[0]);
        //$('#billingcountry').hide();
        $('#billingcountry_address').show();
        $('#checkoutlookupspinner').hide();
        $('.lookup-response').find('.error').fadeOut(250);
        $('#addressblock').hide();

        $('#billingcountry_address').append('<option value="0">I can\'t find my address</option>');
    }


    function setAddressValues_Checkout(address) {
        
        $('[name=AddressLine1]').val(address.Line1 + ((address.Line2 != ' ') ? ', ' + address.Line2 : ''));
        $('[name=AddressLine2]').val(address.Line3);
        $('[name=City]').val(address.City);
        $('[name=CountyOrProvince]').val((address.Locality != ' ' ? address.Locality : address.County));
        $("#btnPay360Pay").attr("disabled", false);
        document.getElementById("btnPay360Pay").style.backgroundColor = "#dee228";
    }


    function noResultsOrInvalid_checkout() {
        $("#billingcountry_address").hide();
        $('#checkoutlookupspinner').hide();
        $('.lookup-response').find('.error').fadeIn(250);
        $('#addressblock').show();
        $("#btnPay360Pay").attr("disabled", false);
        document.getElementById("btnPay360Pay").style.backgroundColor = "#dee228";
    }



    $('#billingcountry_address').change(function () {
        $('#addressblock').hide();
        if ($(this).find(':selected').attr('value') == 0)
            return cannotFindAddress_checkout();

        var i = $(this).find(':selected').attr('value');
        setAddressValues_Checkout(addresses[i - 1]);
    });
    function cannotFindAddress_checkout() {

        $('[name=AddressLine1], [name=AddressLine2], [name=City], [name=CountyOrProvince]').val(''); // Empty any previous result
        $("#addressblock").show();
          $("#btnPay360Pay").attr("disabled", false);
    }


})();
