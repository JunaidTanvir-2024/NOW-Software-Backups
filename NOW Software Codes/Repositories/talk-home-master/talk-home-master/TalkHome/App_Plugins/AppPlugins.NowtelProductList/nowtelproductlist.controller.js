angular.module('umbraco')
    .controller('AppPlugins.NowtelProductList',
    function ($scope) {

        // Setting the default value of the model if empty.
        if ($scope.model.value == '')
            $scope.model.value = 'thm';

        // Set `selected` property on the correct option of the select menu.
        $('#nowtelProductList').find('option').each(function () {

            if ($(this).attr('value') == $scope.model.value)
                $(this).attr('selected', true);
        });

        // On select menu change stores the actual value used in the model.
        $('#nowtelProductList').change(function () {

            $scope.model.value = $('#nowtelProductList').find(':selected').attr('value');
        });
    });
