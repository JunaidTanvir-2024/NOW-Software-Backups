/**
 * Sets the destination value(s) for a content type. 
 * The value is stored as an array of strings representing one or more alpha-2 country code(s).
 *
 * @author micheled
 * Created on 11.07.2017
 *
 */
angular.module('umbraco')
    .controller('AppPlugins.Destinations',
    function ($scope) {
        'use strict';

        /**
         * Selectes the correct destination values at page load
         * 
         */
        function selectDestination () {
            var options = $('#callDestinations').find('option');

            $.each(options, function () {
                if ($(this).attr('value') === $scope.model.value)
                    $(this).attr('selected', true);
            });
        }

        // Register UI events

        //Sets values of the selected destination
        $(document).on('change', '#callDestinations', function () {
            $scope.model.value = $('#callDestinations').find(':selected').attr('value');
        });

        // Page load
        selectDestination();
    });
