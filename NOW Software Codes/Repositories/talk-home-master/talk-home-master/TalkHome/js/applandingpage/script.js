$(document).ready(function () {
    //Topup Number
    var topUpInput = document.querySelector("#TopUpNumber");
    window.intlTelInput(topUpInput, {
        //allowDropdown: true,
        // autoHideDialCode: false,
        //autoPlaceholder: "on",
        dropdownContainer: document.body,
        // excludeCountries: ["us"],
        formatOnDisplay: true,
        //geoIpLookup: function (callback) {
        //    $.get("http://ipinfo.io", function () { }, "jsonp").always(function (resp) {
        //        var countryCode = (resp && resp.country) ? resp.country : "";
        //        callback(countryCode);
        //    });
        //},
        // hiddenInput: "full_number",
        initialCountry: "auto",
        // localizedCountries: { 'de': 'Deutschland' },
        //nationalMode: true,
        // onlyCountries: ['us', 'gb', 'ch', 'ca', 'do'],
        //placeholderNumberType: "MOBILE",
        preferredCountries: ['gb', 'fr'],
        //separateDialCode: true,
        utilsScript: "assets/lib/intl-tel-input/js/utils.js",
    });
    
    //Initialize Swiper
    var swiper = new Swiper('.swiper-container', {
        loop: false,
        grabCursor: true,
        // autoplay: {
        //   delay: 2500,
        //   disableOnInteraction: false
        // },
        slidesPerView: 4,
        spaceBetween: 15,
        navigation: {
            nextEl: '.swiper-button-next',
            prevEl: '.swiper-button-prev',
        },
        breakpoints: {
            800: {
                slidesPerView: 3,
                spaceBetween: 10,
            },
            767: {
                slidesPerView: 1,
                spaceBetween: 10,
            },
        }
    });
})

$(document).ready(function () {
    //Log in / Sign Up Number 
    var input = document.querySelector("#phone");
    window.intlTelInput(input, {
        //allowDropdown: true,
        // autoHideDialCode: false,
        //autoPlaceholder: "on",
        dropdownContainer: document.body,
        // excludeCountries: ["us"],
        formatOnDisplay: true,
        //geoIpLookup: function (callback) {
        //    $.get("http://ipinfo.io", function () { }, "jsonp").always(function (resp) {
        //        var countryCode = (resp && resp.country) ? resp.country : "";
        //        callback(countryCode);
        //    });
        //},
        // hiddenInput: "full_number",
        //initialCountry: "auto",
        // localizedCountries: { 'de': 'Deutschland' },
        //nationalMode: true,
        // onlyCountries: ['us', 'gb', 'ch', 'ca', 'do'],
        //placeholderNumberType: "MOBILE",
        preferredCountries: ['gb', 'fr'],
        //separateDialCode: true,
        utilsScript: "assets/lib/intl-tel-input/js/utils.js",
    });

});