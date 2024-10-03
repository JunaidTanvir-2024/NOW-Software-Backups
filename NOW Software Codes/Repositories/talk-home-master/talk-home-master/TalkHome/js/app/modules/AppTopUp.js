/**
 * Manages funcionalities for App top ups.
 * Performs operations on the customer's cookies basket via AJAX calls to the BasketController. AJAX calls are managed
 * by the Utils JS module.
 * 
 * @author micheled
 * Created on 11 Aug 2017 at 14:08
 *
 */
'use strict';
var AppTopUp = (function () {

    var utils = new Utils,
        emptyAndAddURI = '/basket/api/empty-add-top-up',
        swapUri = '/basket/api/swap-top-up',
        Unreg = "0";

    /*******************
     * Private methods *
     *******************/

    /**
     * Shows the edit details inputs
     * 
     */
    function editYourDetails() {
        $('[name=PostalCode]').val('');
        $('#editName, .editAddress').show();
        $('.valid-address').hide();
    }

    /**
     * Replaces personal detials with the input boxes values.
     * 
     */
    function saveDetails () {
        var name = $('[name=name]').val();
        var email = $('[name=email]').val();

        $('[data-name=name]').text(name);
        $('[data-name=email]').text(email);
    }

    /**
     * Replaces the address with the input boxes values.
     * 
     */
    function saveAddress () {
        var addressLine1 = $('[name=addressLine1]').val();
        var addressLine2 = $('[name=addressLine2]').val();
        var city = $('[name=city]').val();
        var state = $('[name=state]').val();
        var postcode = $('[name=postcode]').val();
        var country = $('[name=country]').find(':selected').text();
        var address = [addressLine1, addressLine2, city, state, postcode, country];

        $.each(address, function (i, v) {
            if (v == '') address.splice(i, 1);
        });

        $('[data-name=address]').text(address.join(', '));
    }

    /**
     * Registers the JS events needed on the checkout page.
     * 
     */
    function registerUiEvents () {

        // Show available cards
        $('[data-show=cards]').click(function () {
            if ($('.cards li').length > 0 && Unreg === "0")
                $('[data-show-elem=cards]').slideDown(250);
        });

        // Hide available cards
        $('[data-hide=cards]').click(function () {
            if ($('.cards li').length > 0)
                $('[data-show-elem=cards]').hide();
        });

        // Show details as input items
        $('[data-show=edit-details].edit, [data-show=edit-address].edit').click(function () {
            var groups = $(this).closest('.parent').find('.group');

            $('[name=Unreg]').val("1"); // Process payment as Unreg customer
            Unreg = "1";
            $('[data-show-elem=cards]').hide();
            editYourDetails();
        });

        // Top up and Checkout forms are submitted, show loading spinner to mimic Android behaviour.
        $('#checkoutForm, #amountForm').submit(function () {
            showSpinner();
        });
    }

    /**
     * Changes the CSS class for the newly selected top up div.
     * 
     * @param {int} newId The id of the top up
     * 
     */
    function updateViewAferSwap (newId) {
        $('[name=amount].current').removeClass('current');
        $('[data-id=' + newId + ']').addClass('current');
    }

    /**
     * Updates customer's cookie given the default top up.
     * 
     * @param {jQueryObject} radio The default product
     * 
     */
    function setDefaultTopUp (radio) {

        var id = radio.attr('data-id');
        args.data = { 'Id': id, 'empty': true };

        utils.Post(emptyAndAddURI, args);
    }

    /**
     * Updates customer's basket id given the selected top up amount.
     * 
     * @param {jQueryObject} radio The top up amount selected
     * 
     */
    function swapTopUp (radio) {

        var id = $('[name=amount].current').attr('data-id');
        var newId = radio.attr('data-id');
        args.data = { 'Id': id, 'newId': newId };

        args.success = function (result) {
            if (result.success) updateViewAferSwap(newId);
        };
        
        utils.Post(swapUri, args);
    }
    
    /**
     * After the continue button is hit, we proceed by storing the top up value and
     * displaying the details form.
     * 
     * @param {string} amount The top up amount selected
     * 
     */
    function processAmountRequest (amount) {
        $('#amountForm').hide();
        $('#checkoutForm').show();
    }

    /**
     * Shows the loading spinner.
     * 
     */
    function showSpinner () {
        $('.loader').show();
    }

    /*************
     * UI events *
     *************/

    // Set dafult top up in the cookie
    if ($('#amountForm').length > 0)
        setDefaultTopUp($('[name=amount].current'));

    // On selecting top up amount, update basket property in the cookie
    $('[name=amount]').change(function () {
        swapTopUp($(this));
    });

    // Register UI events
    registerUiEvents();
})();
