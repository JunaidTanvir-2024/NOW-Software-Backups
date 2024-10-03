using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using TalkHome.Models.Enums;

namespace TalkHome.Models.Validation
{
    /// <summary>
    /// Parent class for View models inputs validation 
    /// </summary>
    public class RequestValidations
    {
        /// <summary>
        /// Gets the list of countries based on ISO 3166-1
        /// </summary>
        /// <returns>Returns the list of countries.</returns>
        private static List<RegionInfo> GetListOfCountries()
        {
            List<RegionInfo> countries = new List<RegionInfo>();
            foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                RegionInfo country = new RegionInfo(culture.LCID);
                if (countries.Where(p => p.Name == country.Name).Count() == 0)
                    countries.Add(country);
            }
            return countries.OrderBy(p => p.EnglishName).ToList();
        }

        /// <summary>
        /// Validates Address Line 1
        /// </summary>
        /// <param name="addressLine1">The input string</param>
        /// <returns>The validation result</returns>
        public static ValidationResult AddressLine1(string addressLine1)
        {
            if (string.IsNullOrWhiteSpace(addressLine1))
                return new ValidationResult(((int)Messages.AddressLine1).ToString());
            else
                return ValidationResult.Success;
        }

        /// <summary>
        /// Validates City
        /// </summary>
        /// <param name="addressLine1">The input string</param>
        /// <returns>The validation result</returns>
        public static ValidationResult City(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return new ValidationResult(((int)Messages.City).ToString());
            else
                return ValidationResult.Success;
        }

        /// <summary>
        /// Validates Country code
        /// </summary>
        /// <param name="countryCode">The input string</param>
        /// <returns>The validation result</returns>
        public static ValidationResult CountryCode(string countryCode)
        {
            var Countries = GetListOfCountries();

            if (string.IsNullOrWhiteSpace(countryCode))
                return new ValidationResult(((int)Messages.CountryCode).ToString());

            foreach (var country in Countries)
                if (countryCode.Equals(country.TwoLetterISORegionName))
                    return ValidationResult.Success; // Better to return a success early, as soon as a country code match is found

            return new ValidationResult(((int)Messages.InvalidCountryCode).ToString());
        }

        /// <summary>
        /// Validates Post code
        /// </summary>
        /// <param name="postCode">The input string</param>
        /// <returns>The validation result</returns>
        public static ValidationResult PostCode(string postCode)
        {
            if (string.IsNullOrWhiteSpace(postCode))
                return new ValidationResult(((int)Messages.PostCode).ToString());
            else
                return ValidationResult.Success;
        }

        /// <summary>
        /// Validates Email address
        /// </summary>
        /// <param name="email">The input string</param>
        /// <returns>The validation result</returns>
        public static ValidationResult Email(string email)
        {
            if (string.IsNullOrEmpty(email))
                return new ValidationResult(((int)Messages.Email).ToString());
            else
                return ValidationResult.Success;
        }

        /// <summary>
        /// Validates Salutation
        /// </summary>
        /// <param name="salutation">The input string</param>
        /// <returns>The validation result</returns>
        public static ValidationResult Salutation(string salutation)
        {
            if (string.IsNullOrEmpty(salutation))
                return new ValidationResult(((int)Messages.Salutation).ToString());
            else
                return ValidationResult.Success;
        }

        /// <summary>
        /// Validates First name
        /// </summary>
        /// <param name="firstName">The input string</param>
        /// <returns>The validation result</returns>
        public static ValidationResult FirstName(string firstName)
        {
            if (string.IsNullOrEmpty(firstName))
                return new ValidationResult(((int)Messages.FirstName).ToString());
            else
                return ValidationResult.Success;
        }

        /// <summary>
        /// Validates Last name
        /// </summary>
        /// <param name="lastName">The input string</param>
        /// <returns>The validation result</returns>
        public static ValidationResult LastName(string lastName)
        {
            if (string.IsNullOrEmpty(lastName))
                return new ValidationResult(((int)Messages.LastName).ToString());
            else
                return ValidationResult.Success;
        }

        /// <summary>
        /// Validates Password
        /// </summary>
        /// <param name="password">The input string</param>
        /// <returns>The validation result</returns>
        public static ValidationResult Password(string password)
        {
            if (string.IsNullOrEmpty(password))
                return new ValidationResult(((int)Messages.MissingPassword).ToString());

            if (password.Length < 8)
                return new ValidationResult(((int)Messages.PasswordTooShort).ToString());

            else
                return ValidationResult.Success;
        }

        /// <summary>
        /// Validates Confirm password
        /// </summary>
        /// <param name="confirmPassword">The input string</param>
        /// <returns>The validation result</returns>
        public static ValidationResult ConfirmPassword(string confirmPassword)
        {
            if (string.IsNullOrEmpty(confirmPassword))
                return new ValidationResult(((int)Messages.MissingConfirmPassword).ToString());

            if (confirmPassword.Length < 8)
                return new ValidationResult(((int)Messages.PasswordTooShort).ToString());

            else
                return ValidationResult.Success;
        }

        /// <summary>
        /// Validates a Talk Home product code
        /// </summary>
        /// <param name="productCode">The input string</param>
        /// <returns>The validation result</returns>
        public static ValidationResult ProductCode(string productCode)
        {
            if (string.IsNullOrEmpty(productCode))
                return new ValidationResult(((int)Messages.ProductCode).ToString());

            Enums.ProductCodes output;

            if (!Enum.TryParse(productCode, out output))
                return new ValidationResult(((int)Messages.InvalidProductCode).ToString());

            else
                return ValidationResult.Success;
        }

        /// <summary>
        /// Validates a mail order product request
        /// </summary>
        /// <param name="mailOrderProduct">The input string</param>
        /// <returns>The validation result</returns>
        public static ValidationResult MailOrderProduct(string mailOrderProduct)
        {
            if (string.IsNullOrEmpty(mailOrderProduct))
                return new ValidationResult(((int)Messages.MailOrderProductMissing).ToString());

            MailOrderAvailableProducts output;

            if (!Enum.TryParse(mailOrderProduct, out output))
                return new ValidationResult(((int)Messages.InvalidProductCode).ToString());

            else
                return ValidationResult.Success;
        }

        /// <summary>
        /// Validates a product type before checkout
        /// </summary>
        /// <param name="productType">The input string</param>
        /// <returns>The validation result</returns>
        public static ValidationResult ProductType(string productType)
        {
            if (string.IsNullOrEmpty(productType))
                return new ValidationResult(((int)Messages.ProductCode).ToString());

            ProductType output;

            if (!Enum.TryParse(productType, out output))
                return new ValidationResult(((int)Messages.InvalidProductType).ToString());

            else
                return ValidationResult.Success;
        }

        /// <summary>
        /// Validates a phone or card number
        /// </summary>
        /// <param name="number">The input string</param>
        /// <returns>The validation result</returns>
        public static ValidationResult Number(string number)
        {
            if (string.IsNullOrEmpty(number))
                return new ValidationResult(((int)Messages.Number).ToString());
            else
                return ValidationResult.Success;
        }

        /// <summary>
        /// Validates the activation code
        /// </summary>
        /// <param name="code">The input string</param>
        /// <returns>The validation result</returns>
        public static ValidationResult Code(string code)
        {
            if (string.IsNullOrEmpty(code))
                return new ValidationResult(((int)Messages.MissingActivationCode).ToString());
            else
                return ValidationResult.Success;
        }

        /// <summary>
        /// Validates the requested payment against the allowed payment methods
        /// </summary>
        /// <param name="paymentMethod">The input string</param>
        /// <returns>The validation result</returns>
        public static ValidationResult PaymentMethod(string paymentMethod)
        {
            if (string.IsNullOrEmpty(paymentMethod))
                return new ValidationResult(((int)Messages.PaymentMethod).ToString());

            PaymentMethod output;

            if (!Enum.TryParse(paymentMethod, out output))
                return new ValidationResult(((int)Messages.InvalidPaymentMethod).ToString());

            return ValidationResult.Success;
        }

        /// <summary>
        /// Validates a string to be used as Id
        /// </summary>
        /// <param name="Id">the Id</param>
        /// <returns>The validation result</returns>
        public static ValidationResult Id(string Id)
        {
            int Result;

            if (!int.TryParse(Id, out Result))
                return new ValidationResult(((int)Messages.InvalidId).ToString());

            return ValidationResult.Success;
        }

        /// <summary>
        /// Validates a string to be used as Id
        /// </summary>
        /// <param name="Id">the Id</param>
        /// <returns>The validation result</returns>
        public static ValidationResult Threshold(string threshold)
        {
            if (string.IsNullOrWhiteSpace(threshold))
                return new ValidationResult(((int)Messages.MissingThreshold).ToString());

            decimal Result;
            
            if (!decimal.TryParse(threshold, out Result)) // can be casted to decimal?
                return new ValidationResult(((int)Messages.NotADecimalNumber).ToString());
            
            if (Result < 1 && Result > 30) // is it between 1 and 30?
                return new ValidationResult(((int)Messages.NumberNotInRange).ToString());

            return ValidationResult.Success;
        }
    }
}
