using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using TalkHome.Models;
using TalkHome.Models.Enums;
using Umbraco.Web.PublishedContentModels;
using System.Linq;
using TalkHome.Interfaces;
using Umbraco.Core.Models;
using Newtonsoft.Json.Linq;

namespace TalkHome.Extensions.Html
{
    /// <summary>
    /// Provides helper methods for the UI.
    /// </summary>
    public static class UIExtensions
    {
        private static readonly IAccountService AccountService;
        

        static UIExtensions()
        {
            AccountService = DependencyResolver.Current.GetService<IAccountService>();
        }

        /// <summary>
        /// Exposes Utils class method for a list of countries (English names only).
        /// </summary>
        /// <param name="helper">The helper class</param>
        /// <returns>The list of countries</returns>
        public static List<I18nCountry> GetListOfCountries(this HtmlHelper helper)
        {
            return AccountService.GetCountryList();
        }

        /// <summary>
        /// Attempts to format the given Msisdn for the given region. In case of failure, the orignal msisdn is returned.
        /// </summary>
        /// <param name="helper">The helper class</param>
        /// <param name="msisdn">The Msisdn to format</param>
        /// <param name="region">the region code</param>
        /// <returns>The formatted Msisdn or the input Msisdn</returns>
        public static string FormatMsisdn(this HtmlHelper helper, string msisdn, string region)
        {
            var Util = PhoneNumberUtil.GetInstance();
            PhoneNumber Number = new PhoneNumber();

            try
            {
                Number = Util.Parse(msisdn, region);
            }
            catch
            {
                return msisdn;
            }

            return Util.Format(Number, PhoneNumberFormat.INTERNATIONAL);
        }

        /// <summary> 
        /// Checks for a product's Guid to find out if a product is a bundle or not. 
        /// </summary> 
        /// <param name="helper">The helper class</param> 
        /// <param name="product">The product</param> 
        /// <returns>Either TRUE or FALSE</returns> 
        public static bool IsBundle(this HtmlHelper helper, Product product)
        {
            if (product.ProductUuid.Equals("0"))
                return false;

            return true;
        }


        public static string DialCode(this HtmlHelper helper, string country)
        {
            var dialCodes = "{\"countries\":[{\"code\":\"+852\",\"name\":\"Hong Kong\"},{\"code\":\"+383\",\"name\":\"Kosova\"},{\"code\":\"+245\",\"name\":\"Guinea Bissau\"},{\"code\":\"+379\",\"name\":\"Vatican City\"},{\"code\":\"+64 3\",\"name\":\"Chatham Islands\"},{\"code\":\"+90\",\"name\":\"Cyprus North Mobile Turkcell\"},{\"code\":\"+90\",\"name\":\"Cyprus North\"},{\"code\":\"+357\",\"name\":\"Cyprus South\"},{\"code\":\"+34\",\"name\":\"Canary Islands\"},{\"code\":\"+971\",\"name\":\"Balearic Islands\"},{\"code\":\"+673\",\"name\":\"Brunei Darussalam\"},{\"code\":\"+345\",\"name\":\"Cayman Islands\"},{\"code\":\"+247\",\"name\":\"Ascension Island\"},{\"code\":\"+672\",\"name\":\"Antarctica\"},{\"code\":\"+7 840\",\"name\":\"Abkhazia\"},{\"code\":\"+93\",\"name\":\"Afghanistan\"},{\"code\":\"+355\",\"name\":\"Albania\"},{\"code\":\"+213\",\"name\":\"Algeria\"},{\"code\":\"+1 684\",\"name\":\"American Samoa\"},{\"code\":\"+376\",\"name\":\"Andorra\"},{\"code\":\"+244\",\"name\":\"Angola\"},{\"code\":\"+1 264\",\"name\":\"Anguilla\"},{\"code\":\"+1 268\",\"name\":\"Antigua and Barbuda\"},{\"code\":\"+54\",\"name\":\"Argentina\"},{\"code\":\"+374\",\"name\":\"Armenia\"},{\"code\":\"+297\",\"name\":\"Aruba\"},{\"code\":\"+247\",\"name\":\"Ascension\"},{\"code\":\"+61\",\"name\":\"Australia\"},{\"code\":\"+672\",\"name\":\"Australian External Territories\"},{\"code\":\"+43\",\"name\":\"Austria\"},{\"code\":\"+994\",\"name\":\"Azerbaijan\"},{\"code\":\"+1 242\",\"name\":\"Bahamas\"},{\"code\":\"+973\",\"name\":\"Bahrain\"},{\"code\":\"+880\",\"name\":\"Bangladesh\"},{\"code\":\"+1 246\",\"name\":\"Barbados\"},{\"code\":\"+1 268\",\"name\":\"Barbuda\"},{\"code\":\"+375\",\"name\":\"Belarus\"},{\"code\":\"+32\",\"name\":\"Belgium\"},{\"code\":\"+501\",\"name\":\"Belize\"},{\"code\":\"+229\",\"name\":\"Benin\"},{\"code\":\"+1 441\",\"name\":\"Bermuda\"},{\"code\":\"+975\",\"name\":\"Bhutan\"},{\"code\":\"+591\",\"name\":\"Bolivia\"},{\"code\":\"+387\",\"name\":\"Bosnia and Herzegovina\"},{\"code\":\"+267\",\"name\":\"Botswana\"},{\"code\":\"+55\",\"name\":\"Brazil\"},{\"code\":\"+246\",\"name\":\"British Indian Ocean Territory\"},{\"code\":\"+1 284\",\"name\":\"British Virgin Islands\"},{\"code\":\"+673\",\"name\":\"Brunei\"},{\"code\":\"+359\",\"name\":\"Bulgaria\"},{\"code\":\"+226\",\"name\":\"Burkina Faso\"},{\"code\":\"+257\",\"name\":\"Burundi\"},{\"code\":\"+855\",\"name\":\"Cambodia\"},{\"code\":\"+237\",\"name\":\"Cameroon\"},{\"code\":\"+1\",\"name\":\"Canada\"},{\"code\":\"+238\",\"name\":\"Cape Verde\"},{\"code\":\"+ 345\",\"name\":\"Cayman Island\"},{\"code\":\"+236\",\"name\":\"Central African Republic\"},{\"code\":\"+235\",\"name\":\"Chad\"},{\"code\":\"+56\",\"name\":\"Chile\"},{\"code\":\"+86\",\"name\":\"China\"},{\"code\":\"+61\",\"name\":\"Christmas Island\"},{\"code\":\"+61\",\"name\":\"Cocos Islands\"},{\"code\":\"+57\",\"name\":\"Colombia\"},{\"code\":\"+269\",\"name\":\"Comoros\"},{\"code\":\"+242\",\"name\":\"Congo\"},{\"code\":\"+243\",\"name\":\"DRC\"},{\"code\":\"+682\",\"name\":\"Cook Islands\"},{\"code\":\"+506\",\"name\":\"Costa Rica\"},{\"code\":\"+385\",\"name\":\"Croatia\"},{\"code\":\"+53\",\"name\":\"Cuba\"},{\"code\":\"+599\",\"name\":\"Curacao\"},{\"code\":\"+537\",\"name\":\"Cyprus\"},{\"code\":\"+420\",\"name\":\"Czech Republic\"},{\"code\":\"+45\",\"name\":\"Denmark\"},{\"code\":\"+246\",\"name\":\"Diego Garcia\"},{\"code\":\"+253\",\"name\":\"Djibouti\"},{\"code\":\"+1 767\",\"name\":\"Dominica\"},{\"code\":\"+1 809\",\"name\":\"Dominican Republic\"},{\"code\":\"+670\",\"name\":\"East Timor\"},{\"code\":\"+56\",\"name\":\"Easter Island\"},{\"code\":\"+593\",\"name\":\"Ecuador\"},{\"code\":\"+20\",\"name\":\"Egypt\"},{\"code\":\"+503\",\"name\":\"El Salvador\"},{\"code\":\"+240\",\"name\":\"Equatorial Guinea\"},{\"code\":\"+291\",\"name\":\"Eritrea\"},{\"code\":\"+372\",\"name\":\"Estonia\"},{\"code\":\"+251\",\"name\":\"Ethiopia\"},{\"code\":\"+500\",\"name\":\"Falkland Islands\"},{\"code\":\"+298\",\"name\":\"Faroe Islands\"},{\"code\":\"+679\",\"name\":\"Fiji\"},{\"code\":\"+358\",\"name\":\"Finland\"},{\"code\":\"+33\",\"name\":\"France\"},{\"code\":\"+596\",\"name\":\"French Antilles\"},{\"code\":\"+594\",\"name\":\"French Guiana\"},{\"code\":\"+689\",\"name\":\"French Polynesia\"},{\"code\":\"+241\",\"name\":\"Gabon\"},{\"code\":\"+220\",\"name\":\"Gambia\"},{\"code\":\"+995\",\"name\":\"Georgia\"},{\"code\":\"+49\",\"name\":\"Germany\"},{\"code\":\"+233\",\"name\":\"Ghana\"},{\"code\":\"+350\",\"name\":\"Gibraltar\"},{\"code\":\"+30\",\"name\":\"Greece\"},{\"code\":\"+299\",\"name\":\"Greenland\"},{\"code\":\"+1 473\",\"name\":\"Grenada\"},{\"code\":\"+590\",\"name\":\"Guadeloupe\"},{\"code\":\"+1 671\",\"name\":\"Guam\"},{\"code\":\"+502\",\"name\":\"Guatemala\"},{\"code\":\"+224\",\"name\":\"Guinea\"},{\"code\":\"+245\",\"name\":\"Guinea-Bissau\"},{\"code\":\"+595\",\"name\":\"Guyana\"},{\"code\":\"+509\",\"name\":\"Haiti\"},{\"code\":\"+504\",\"name\":\"Honduras\"},{\"code\":\"+852\",\"name\":\"Hong Kong SAR China\"},{\"code\":\"+36\",\"name\":\"Hungary\"},{\"code\":\"+354\",\"name\":\"Iceland\"},{\"code\":\"+91\",\"name\":\"India\"},{\"code\":\"+62\",\"name\":\"Indonesia\"},{\"code\":\"+98\",\"name\":\"Iran\"},{\"code\":\"+964\",\"name\":\"Iraq\"},{\"code\":\"+353\",\"name\":\"Ireland\"},{\"code\":\"+972\",\"name\":\"Israel\"},{\"code\":\"+39\",\"name\":\"Italy\"},{\"code\":\"+225\",\"name\":\"Ivory Coast\"},{\"code\":\"+1 876\",\"name\":\"Jamaica\"},{\"code\":\"+81\",\"name\":\"Japan\"},{\"code\":\"+962\",\"name\":\"Jordan\"},{\"code\":\"+7 7\",\"name\":\"Kazakhstan\"},{\"code\":\"+254\",\"name\":\"Kenya\"},{\"code\":\"+686\",\"name\":\"Kiribati\"},{\"code\":\"+965\",\"name\":\"Kuwait\"},{\"code\":\"+996\",\"name\":\"Kyrgyzstan\"},{\"code\":\"+856\",\"name\":\"Laos\"},{\"code\":\"+371\",\"name\":\"Latvia\"},{\"code\":\"+961\",\"name\":\"Lebanon\"},{\"code\":\"+266\",\"name\":\"Lesotho\"},{\"code\":\"+231\",\"name\":\"Liberia\"},{\"code\":\"+218\",\"name\":\"Libya\"},{\"code\":\"+423\",\"name\":\"Liechtenstein\"},{\"code\":\"+370\",\"name\":\"Lithuania\"},{\"code\":\"+352\",\"name\":\"Luxembourg\"},{\"code\":\"+853\",\"name\":\"Macau SAR China\"},{\"code\":\"+389\",\"name\":\"Macedonia\"},{\"code\":\"+261\",\"name\":\"Madagascar\"},{\"code\":\"+265\",\"name\":\"Malawi\"},{\"code\":\"+60\",\"name\":\"Malaysia\"},{\"code\":\"+960\",\"name\":\"Maldives\"},{\"code\":\"+223\",\"name\":\"Mali\"},{\"code\":\"+356\",\"name\":\"Malta\"},{\"code\":\"+692\",\"name\":\"Marshall Islands\"},{\"code\":\"+596\",\"name\":\"Martinique\"},{\"code\":\"+222\",\"name\":\"Mauritania\"},{\"code\":\"+230\",\"name\":\"Mauritius\"},{\"code\":\"+262\",\"name\":\"Mayotte\"},{\"code\":\"+52\",\"name\":\"Mexico\"},{\"code\":\"+691\",\"name\":\"Micronesia\"},{\"code\":\"+1 808\",\"name\":\"Midway Island\"},{\"code\":\"+373\",\"name\":\"Moldova\"},{\"code\":\"+377\",\"name\":\"Monaco\"},{\"code\":\"+976\",\"name\":\"Mongolia\"},{\"code\":\"+382\",\"name\":\"Montenegro\"},{\"code\":\"+1664\",\"name\":\"Montserrat\"},{\"code\":\"+212\",\"name\":\"Morocco\"},{\"code\":\"+95\",\"name\":\"Myanmar\"},{\"code\":\"+264\",\"name\":\"Namibia\"},{\"code\":\"+674\",\"name\":\"Nauru\"},{\"code\":\"+977\",\"name\":\"Nepal\"},{\"code\":\"+31\",\"name\":\"Netherlands\"},{\"code\":\"+599\",\"name\":\"Netherlands Antilles\"},{\"code\":\"+1 869\",\"name\":\"Nevis\"},{\"code\":\"+687\",\"name\":\"New Caledonia\"},{\"code\":\"+64\",\"name\":\"New Zealand\"},{\"code\":\"+505\",\"name\":\"Nicaragua\"},{\"code\":\"+227\",\"name\":\"Niger\"},{\"code\":\"+234\",\"name\":\"Nigeria\"},{\"code\":\"+683\",\"name\":\"Niue\"},{\"code\":\"+672\",\"name\":\"Norfolk Island\"},{\"code\":\"+850\",\"name\":\"Korea South\"},{\"code\":\"+1 670\",\"name\":\"Northern Mariana Islands\"},{\"code\":\"+47\",\"name\":\"Norway\"},{\"code\":\"+968\",\"name\":\"Oman\"},{\"code\":\"+92\",\"name\":\"Pakistan\"},{\"code\":\"+680\",\"name\":\"Palau\"},{\"code\":\"+970\",\"name\":\"Palestinian Territory\"},{\"code\":\"+507\",\"name\":\"Panama\"},{\"code\":\"+675\",\"name\":\"Papua New Guinea\"},{\"code\":\"+595\",\"name\":\"Paraguay\"},{\"code\":\"+51\",\"name\":\"Peru\"},{\"code\":\"+63\",\"name\":\"Philippines\"},{\"code\":\"+48\",\"name\":\"Poland\"},{\"code\":\"+351\",\"name\":\"Portugal\"},{\"code\":\"+1 787\",\"name\":\"Puerto Rico\"},{\"code\":\"+974\",\"name\":\"Qatar\"},{\"code\":\"+262\",\"name\":\"Reunion\"},{\"code\":\"+40\",\"name\":\"Romania\"},{\"code\":\"+7\",\"name\":\"Russia\"},{\"code\":\"+250\",\"name\":\"Rwanda\"},{\"code\":\"+685\",\"name\":\"Western Samoa\"},{\"code\":\"+378\",\"name\":\"San Marino\"},{\"code\":\"+966\",\"name\":\"Saudi Arabia\"},{\"code\":\"+221\",\"name\":\"Senegal\"},{\"code\":\"+381\",\"name\":\"Serbia\"},{\"code\":\"+248\",\"name\":\"Seychelles\"},{\"code\":\"+232\",\"name\":\"Sierra Leone\"},{\"code\":\"+65\",\"name\":\"Singapore\"},{\"code\":\"+421\",\"name\":\"Slovakia\"},{\"code\":\"+386\",\"name\":\"Slovenia\"},{\"code\":\"+677\",\"name\":\"Solomon Islands\"},{\"code\":\"+27\",\"name\":\"South Africa\"},{\"code\":\"+500\",\"name\":\"South Georgia and the South Sandwich Islands\"},{\"code\":\"+82\",\"name\":\"Korea North\"},{\"code\":\"+34\",\"name\":\"Spain\"},{\"code\":\"+94\",\"name\":\"Sri Lanka\"},{\"code\":\"+249\",\"name\":\"Sudan\"},{\"code\":\"+597\",\"name\":\"Suriname\"},{\"code\":\"+268\",\"name\":\"Swaziland\"},{\"code\":\"+46\",\"name\":\"Sweden\"},{\"code\":\"+41\",\"name\":\"Switzerland\"},{\"code\":\"+963\",\"name\":\"Syria\"},{\"code\":\"+886\",\"name\":\"Taiwan\"},{\"code\":\"+992\",\"name\":\"Tajikistan\"},{\"code\":\"+255\",\"name\":\"Tanzania\"},{\"code\":\"+66\",\"name\":\"Thailand\"},{\"code\":\"+670\",\"name\":\"Timor Leste\"},{\"code\":\"+228\",\"name\":\"Togo\"},{\"code\":\"+690\",\"name\":\"Tokelau\"},{\"code\":\"+676\",\"name\":\"Tonga\"},{\"code\":\"+1 868\",\"name\":\"Trinidad and Tobago\"},{\"code\":\"+216\",\"name\":\"Tunisia\"},{\"code\":\"+90\",\"name\":\"Turkey\"},{\"code\":\"+993\",\"name\":\"Turkmenistan\"},{\"code\":\"+1 649\",\"name\":\"Turks and Caicos Islands\"},{\"code\":\"+688\",\"name\":\"Tuvalu\"},{\"code\":\"+1 340\",\"name\":\"US Virgin Islands\"},{\"code\":\"+256\",\"name\":\"Uganda\"},{\"code\":\"+380\",\"name\":\"Ukraine\"},{\"code\":\"+971\",\"name\":\"United Arab Emirates\"},{\"code\":\"+44\",\"name\":\"United Kingdom\"},{\"code\":\"+1\",\"name\":\"USA\"},{\"code\":\"+598\",\"name\":\"Uruguay\"},{\"code\":\"+998\",\"name\":\"Uzbekistan\"},{\"code\":\"+678\",\"name\":\"Vanuatu\"},{\"code\":\"+58\",\"name\":\"Venezuela\"},{\"code\":\"+84\",\"name\":\"Vietnam\"},{\"code\":\"+1 808\",\"name\":\"Wake Island\"},{\"code\":\"+681\",\"name\":\"Wallis and Futuna Islands\"},{\"code\":\"+967\",\"name\":\"Yemen\"},{\"code\":\"+260\",\"name\":\"Zambia\"},{\"code\":\"+255\",\"name\":\"Zanzibar\"},{\"code\":\"+263\",\"name\":\"Zimbabwe\"}]}";
            
            JObject o = JObject.Parse(dialCodes);
            JToken t = o.SelectToken("$.countries[?(@.name == '" + country + "')]");
            if (t != null)
                return t["code"].ToString();
            else
                return "";
        }

        /// <summary>
        /// Returns a string to determnine whethe the country is in europe
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        public static string European(this HtmlHelper helper, string countryCode)
        {
            string[] euroCodes = new string[] {
                "AD",
                "AL",
                "AT",
                "AX",
                "BA",
                "BE",
                "BG",
                "BY",
                "CH",
                "CS",
                "CY",
                "CZ",
                "DE",
                "DK",
                "EE",
                "ES",
                "FI",
                "FO",
                "FR",
                "GB",
                "GG",
                "GI",
                "GR",
                "HR",
                "HU",
                "IE",
                "IM",
                "IS",
                "IT",
                "JE",
                "LI",
                "LT",
                "LU",
                "LV",
                "MC",
                "MD",
                "ME",
                "MK",
                "MT",
                "NL",
                "NO",
                "PL",
                "PT",
                "RO",
                "RS",
                "RU",
                "SE",
                "SI",
                "SJ",
                "SK",
                "SM",
                "UA",
                "VA",
                "XK"
            };

            if (euroCodes.Contains<String>(countryCode))
                return "EU";
            else
                return "Other";
        }

        /// <summary>
        /// Generates the list of salutation keys.
        /// </summary>
        /// <param name="helper">The helper class</param>
        /// <returns>The list of keys</returns>
        public static List<string> GetSalutationKeys(this HtmlHelper helper)
        {
            return SalutationKeys.Keys;
        }

        /// <summary>
        /// Parses and formats a date string to e.g. 12 Sep 2017. Much nicer :)
        /// </summary>
        /// <param name="helper">The Html helper</param>
        /// <param name="input">The input string</param>
        /// <returns>The formatted date or the original string if an error occurred</returns>
        public static string ToNiceDateFormat(this HtmlHelper helper, string input)
        {
            var Date = new DateTime();

            if (!DateTime.TryParse(input, out Date))
                return input;

            return Date.ToString("dd MMM yyyy");
        }

        /// <summary>
        /// Parses and formats a date string to e.g. 12 Sep 2017 with time. Much nicer :)
        /// </summary>
        /// <param name="helper">The Html helper</param>
        /// <param name="input">The input string</param>
        /// <returns>The formatted date or the original string if an error occurred</returns>
        public static string ToNiceDateFormatWithTime(this HtmlHelper helper, string input)
        {
            var Date = new DateTime();

            if (!DateTime.TryParse(input, out Date))
                return input;

            return Date.ToString("dd MMM yyyy");
        }

        /// <summary>
        /// Returns Now as e.g. September 2017.
        /// </summary>
        /// <param name="helper">The Html helper</param>
        /// <returns>The string</returns>
        public static string GetThisMonthAndYear(this HtmlHelper helper)
        {
            return DateTime.Now.ToString("MMMM yyyy");
        }

        /// <summary>
        /// Uses the Validation class to work out if a model is complete
        /// </summary>
        /// <param name="helper">The Html helper</param>
        /// <param name="model">The Address model</param>
        /// <returns>Result as TRUE or FALSE</returns>
        public static bool IsValidAddress(this HtmlHelper helper, AddressModel model)
        {
            return Validator.TryValidateObject(model, new ValidationContext(model, null, null), null, true);
        }

        /// <summary>
        /// Uses the Validation class to work out if a model is complete
        /// </summary>
        /// <param name="helper">The Html helper</param>
        /// <param name="model">The Full name model</param>
        /// <returns>Result as TRUE or FALSE</returns>
        public static bool HasFullName(this HtmlHelper helper, FullNameModel model)
        {
            return Validator.TryValidateObject(model, new ValidationContext(model, null, null), null, true);
        }

        /// <summary>
        /// Returns the correct label for number based on the product code
        /// </summary>
        /// <param name="helper">The helper class</param>
        /// <param name="productCode">The product code</param>
        /// <returns>The label</returns>
        public static string GetRegistrationNumberLabel(this HtmlHelper helper, string productCode)
        {
            switch (productCode)
            {
                case "THCC":
                    return "Rechargeable Card PIN number";

                default:
                    return "Mobile number";
            }
        }

        /// <summary>
        /// Returns the correct label for code based on the product code
        /// </summary>
        /// <param name="helper">The helper class</param>
        /// <param name="productCode">The product code</param>
        /// <returns>The label</returns>
        public static string GetRegistrationCodeLabel(this HtmlHelper helper, string productCode)
        {
            switch (productCode)
            {
                case "THM":
                    return "Enter the PUK";

                case "THA":
                    return "Enter your PIN";

                default:
                    return "Your PIN number";
            }
        }

        /// <summary>
        /// Shortens `Unlimited` strings to `Unltd.`
        /// </summary>
        /// <param name="helper">The helper class</param>
        /// <param name="productCode">The input string</param>
        /// <returns>The changed string, if required</returns>
        public static string ShortenToUnltd(this HtmlHelper helper, string input)
        {
            if (!input.Equals("unlimited", StringComparison.InvariantCultureIgnoreCase))
                return input;

            return "Unltd.";
        }

        /// <summary>
        /// Generates a machine-safe string from an input string.
        /// </summary>
        /// <param name="helper">The helper class</param>
        /// <param name="input">The input string</param>
        /// <returns>The resulting string</returns>
        /// <seealso cref="https://stackoverflow.com/questions/2920744/url-slugify-algorithm-in-c/2921135#2921135"/>
        public static string Slugify(this HtmlHelper helper, string input)
        {
            var output = input.ToLower();
            output = Regex.Replace(output, @"[^a-z0-9\s-]", "");    // remove invalid characters
            output = Regex.Replace(output, @"\s+", " ").Trim();     // single space
            output = Regex.Replace(output, @"\s", "-");             // insert hyphens

            return output;
        }

        /// <summary>
        /// Generates a machine-safe string from an input string.
        /// </summary>
        /// <param name="input">The input string</param>
        /// <returns>The resulting string</returns>
        /// <seealso cref="https://stackoverflow.com/questions/2920744/url-slugify-algorithm-in-c/2921135#2921135"/>
        public static string Slugify(string input)
        {
            var output = input.ToLower();
            output = Regex.Replace(output, @"[^a-z0-9\s-]", "");    // remove invalid characters
            output = Regex.Replace(output, @"\s+", " ").Trim();     // single space
            output = Regex.Replace(output, @"\s", "-");             // insert hyphens

            return output;
        }

        /// <summary>
        /// Generates the filter dropdown list from unique product categories
        /// </summary>
        /// <param name="helper">The helper class</param>
        /// <param name="products">The list of products</param>
        /// <returns>The list of filters</returns>
        public static List<SelectListItem> GetCategoriesFilter(this HtmlHelper helper, List<Product> products)
        {
            var Categories = new HashSet<string>();
            var Filters = new List<SelectListItem>();

            foreach (var Product in products)
                if (!Categories.Contains(Product.Category))
                    Categories.Add(Product.Category);

            foreach (var Category in Categories)
                Filters.Add(new SelectListItem { Value = Slugify(Category), Text = Category });

            return Filters;
        }

        /// <summary>
        /// Generates the filter dropdown list from unique product destinations
        /// </summary>
        /// <param name="helper">The helper class</param>
        /// <param name="products">The list of products</param>
        /// <returns>The list of filters</returns>
        public static List<SelectListItem> GetCountriesFilter(this HtmlHelper helper, List<Product> products)
        {
            var CountryCodes = new HashSet<string>();
            var CountryList = AccountService.GetCountryList();
            var Filters = new List<SelectListItem>();

            foreach (var Product in products)
                if (!CountryCodes.Contains(Product.ProductDestinations.ToString()))
                    CountryCodes.Add(Product.ProductDestinations.ToString());

            foreach (var CountryCode in CountryCodes)
                foreach(var Country in CountryList)
                    if (Country.cca2.Equals(CountryCode))
                        Filters.Add(new SelectListItem { Value = Country.cca2, Text = Country.name.common });

            Filters.Add(new SelectListItem { Value = "PK-Telenor", Text = "Pakistan Telenor" });

            return Filters.OrderBy(s => s.Text).ToList();
        }

        /// <summary>
        /// Selects a collection of Bundles based on the country code
        /// </summary>
        /// <param name="helper">The helper class</param>
        /// <param name="bundles">All Talk Home App bundles</param>
        /// <param name="countryCode">The country code</param>
        /// <returns>The list of products</returns>
        /// /// <remarks>
        /// Please note the following filtering for App bundles
        /// UK = 386
        /// France = 387
        /// Italy = 388
        /// </remarks>
        public static List<Product> GetBundlesForCountry(this HtmlHelper helper, IEnumerable<IPublishedContent> bundles, string countryCode)
        {
            int HomeCountry = 386;

            if (countryCode.Equals("IT"))
                HomeCountry = 388;
            else if (countryCode.Equals("FR"))
                HomeCountry = 387;

            return bundles.OfType<Product>().Where(x => x.HomeCountry == HomeCountry).ToList();
        }

        /// <summary>
        /// Convert a bundle expiry notice into a colour code CSS class
        /// </summary>
        /// <param name="helper"></param>
        /// <param name=""></param>
        public static string AlertClass(this HtmlHelper helper, ExpiryAlert expiryAlert)
        {
            string cssClass = "";
            switch (expiryAlert)
            {
                case ExpiryAlert.Tomorrow:
                    cssClass = "critical-expiry";
                    break;
                case ExpiryAlert.ThreeDays:
                    cssClass = "high-expiry";
                    break;
                case ExpiryAlert.FiveDays:
                    cssClass = "medium-expiry";
                    break;
            }
            return cssClass;
        }



    }
}
