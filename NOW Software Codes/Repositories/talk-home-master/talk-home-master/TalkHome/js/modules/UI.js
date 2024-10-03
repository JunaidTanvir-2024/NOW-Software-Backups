/**
 * Contains methodds and vars used throughout the website.
 * 
 * @author micheled
 * Created on 12.07.2017
 * 
 */
'use strict';
var UI = function () {

    /**
     * Initialises grid on plans/bundles pages.
     * 
     */
    var grid = $('#plans').isotope({
        itemSelector: '.product',
        layoutMode: 'fitRows',
        sortAscending: {
            weight: true,
            priceAsc: true,
            priceDesc: false,
            minutesDesc: false
        },
        getSortData: {
            weight: '[data-weight] parseFloat',
            priceAsc: '[data-price] parseFloat',
            priceDesc: '[data-price] parseFloat',
            minutesDesc: '[data-minutes] parseFloat'
        }
    });

    if ($("#app-plan-hidden").length > 0)
        grid.isotope({ filter: '' });
    else if ($("#intl-plan-hidden").length == 0)
        grid.isotope({ filter: '.calls-text-data' });
    else
        grid.isotope({ filter: '.recommended-plans' });
    var topPlans = $('#topPlans').isotope({
        itemSelector: '.product',
        masonry: {
            columnWidth: 300,
            gutter: 20,
            fitWidth: true
        }
    });

    grid.on('arrangeComplete', function (event, filteredItems) {
        $("ul.bundles  li.table-plans.oddItem").removeClass("oddItem");
        $("ul.bundles  li.table-plans.evenItem").removeClass("evenItem");
        $("ul.bundles  li.table-plans:visible:odd").addClass("oddItem");
        $("ul.bundles  li.table-plans:visible:even").addClass("evenItem");
    });

    // Filter function
    //function filter() {

    //    var activeFilters = '.All';

    //    if ($('#destination').length > 0) {
    //        var id = 0;
    //        $('#destination option').each(function () {
    //            if ($(this).is(':selected')) {
    //                var v = $(this).attr('value');
    //                if (v.indexOf("Telenor") > 0)
    //                    v = ".PK";
    //                if (id == 0 && v != "")
    //                    activeFilters = '';
    //                id++;
    //                activeFilters += v;
    //            }

    //        });
    //    }


    //    if ($('#category').length > 0) {
    //        $('#category a').each(function () {
    //            if ($(this).hasClass('active'))
    //                activeFilters += $(this).attr('data-filter');
    //        });
    //    }

    //    grid.isotope({ filter: activeFilters });
    //}

    // Mobile filter function
    function mobileFilter() {

        var activeFilters = '';

        if ($('#mobileDestination').length > 0) {
            $('#mobileDestination option').each(function () {
                if ($(this).is(':selected')) {
                    var v = $(this).attr('value');
                    if (v.indexOf("Telenor") > 0)
                        v = ".PK";
                    activeFilters += v;
                }
            });
        }


        if ($('#mobileCategory').length > 0) {
            $('#mobileCategory option').each(function () {
                if ($(this).is(':selected'))
                    activeFilters += $(this).attr('value');
            });
        }

        grid.isotope({ filter: activeFilters });
    }

    // UI events

    // Filter events
    //$('#destination').change(function () {


    //    var countryCode = $(this).find(':selected').attr('value');
    //    $('.rates-container').find('[data-destination]').hide(25);
    //    $('.rates-container').find('[data-destination="' + countryCode + '"]').fadeIn(50).css("display", "inline-block");
    //    filter();
    //});
    $('#destination').change(function () {
        var countryCode = $(this).find(':selected').attr('value').replace('.', '');

        if (countryCode === "recommended-plans") {
            $("#strongFilterMobile").hide();
            $("#strongFilterLandline").hide();
            $("#strongFilterSms").hide();
            $(".internationalPlansSingle").hide();
            $(".topPlan").fadeIn(300);
            //filter();
        }
        else if (countryCode === "") {
            $("#strongFilterMobile").hide();
            $("#strongFilterLandline").hide();
            $("#strongFilterSms").hide();
            $(".internationalPlansSingle").fadeIn(300);
            //filter();
        }
        else if (countryCode === "BG" || countryCode === "PL" || countryCode === "RO") {
            $(".internationalPlansSingle").hide();
            $(".countryCode_" + countryCode + ", .countryCode_GB").fadeIn(300);
            //filter();
        }
        else if (countryCode === "PK-Telenor") {
            $(".internationalPlansSingle").hide();
            $(".countryCode_PK").fadeIn(300);
        }
        else {
            var spnFilterRates = $('#filterRates_' + countryCode);
            $("#filterLandlineVal").text(spnFilterRates.data("landlinerate"));
            $("#filterMobileVal").text(spnFilterRates.data("mobilerate"));
            $("#filterSmsVal").text(spnFilterRates.data("smsrate"));

            $("#strongFilterMobile").show();
            $("#strongFilterLandline").show();
            $("#strongFilterSms").show();

            $(".internationalPlansSingle").hide();
            $(".countryCode_" + countryCode).fadeIn(300);
            //filter();
        }
    });

    $(document).ready(function () {
        $('#destination').trigger('change');
    });

    $('#category a').click(function () {
        if (!$(this).hasClass('active')) {
            $('#category a').removeClass('active');
            $(this).addClass('active');
            filter();
        }
    });

    $('#mobileDestination, #mobileCategory').change(function () {
        var countryCode = $(this).find(':selected').attr('value');
        $('.rates-container').find('[data-destination]').hide(25);
        $('.rates-container').find('[data-destination="' + countryCode + '"]').fadeIn(50).css("display", "inline-block");
        mobileFilter();
    });

    // Sort events
    $('#sort, #mobileSort').change(function () {
        var sortValue = $(this).find(':selected').attr('value');
        grid.isotope({ sortBy: sortValue });
    });

    //$("#destination").on("updatecomplete", function () {
        // this will fire when #select1 change event has finished updating the options.
        //$("ul.bundles  li.table-plans:visible:odd").addClass("oddItem");
        //$("ul.bundles  li.table-plans:visible:even").addClass("evenItem");
    //});

}();


