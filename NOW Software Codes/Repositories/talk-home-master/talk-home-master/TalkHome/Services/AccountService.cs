using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TalkHome.Interfaces;
using TalkHome.Models;
using TalkHome.Models.Enums;
using TalkHome.Models.WebApi;
using TalkHome.Models.WebApi.App;
using TalkHome.Models.Porting;
using TalkHome.WebServices.Interfaces;
using TalkHome.I18n;
using TalkHome.Models.WebApi.DTOs;
using TalkHome.Logger;
using TalkHome.Extensions;
using StackExchange.Profiling.Helpers.Dapper;

namespace TalkHome.Services
{
    /// <summary>
    /// Collection of general utilities
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly IJWTService JWTService;
        private readonly ITalkHomeWebService TalkHomeWebService;
        private readonly ITalkHomeAppWebService TalkHomeAppWebService;
        private readonly ILoggerService LoggerService;
        private Properties.App AppSettings = Properties.App.Default;
        private string TalkHomeWebConnectionString;
        private IDbConnection TalkHomeWebConnectionString1;
        private string DigitalkMobileConnectionString;
        private string LegacyCardsConnectionString;
        private string AppsConnectionString;
        private string DigitalkMobileDbConnectionString;
        public AccountService(IJWTService jwtService, ITalkHomeWebService talkHomeWebService, ITalkHomeAppWebService talkHomeAppWebService, ILoggerService loggerService)
        {
            JWTService = jwtService;
            TalkHomeWebService = talkHomeWebService;
            TalkHomeAppWebService = talkHomeAppWebService;
            LoggerService = loggerService;
            TalkHomeWebConnectionString = ConfigurationManager.ConnectionStrings["TalkHomeWebDb"].ConnectionString;
            DigitalkMobileConnectionString = ConfigurationManager.ConnectionStrings["DigitalkMobileRepDb"].ConnectionString;
            LegacyCardsConnectionString = ConfigurationManager.ConnectionStrings["LegacyCallingCardsDb"].ConnectionString;
            AppsConnectionString = ConfigurationManager.ConnectionStrings["DigitalkAppsDb"].ConnectionString;
            DigitalkMobileDbConnectionString = ConfigurationManager.ConnectionStrings["DigitalkMobileDb"].ConnectionString;
        }





        /// <summary>
        /// Retrieves an App user via the Talk Home App Webservices
        /// </summary>
        /// <param name="msisdn">The msisdn</param>
        /// <returns>The user account or null</returns>
        public async Task<GenericApiResponse<AppUserModel>> GetAppUserByMsisdn(string msisdn)
        {
            return await TalkHomeAppWebService.GetAppUserByMsisdn(msisdn);
        }

        /// <summary>
        /// Async method from the Talk home Web services
        /// </summary>
        /// <param name="apiToken">The logged in user api token</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<AccountDetailsResponseDTO>> GetAccountDetails(string apiToken)
        {
            return await TalkHomeWebService.GetAccountDetails(apiToken);
        }

        /// <summary>
        /// Calls the Web Api to attempt a promo sign up request
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<string>> PromoSignUp(PromoSignUpRequestDTO model)
        {
            return await TalkHomeWebService.PromoSignUp(model);
        }

        /// <summary>
        /// Provides the reset password request method
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<ResetPasswordResponseDTO>> ResetPasswordRequest(ResetPasswordRequestDTO model)
        {
            return await TalkHomeWebService.ResetPasswordRequest(model);
        }

        /// <summary>
        /// Provides the confirm reset password method
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<ResetPasswordConfirmResponseDTO>> ResetPasswordConfirm(ResetPasswordConfirmRequestDTO model)
        {
            return await TalkHomeWebService.ResetPasswordConfirm(model);
        }

        /// <summary>
        /// Provides the new password method
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<NewPasswordResponseDTO>> NewPassword(NewPasswordRequestDTO model)
        {
            return await TalkHomeWebService.NewPassword(model);
        }

        /// <summary>
        /// Provides the log in method
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<LoginResponseDTO<AuthenticationContent>>> AuthenticateCustomer(LoginRequestDTO model)
        {
            return await TalkHomeWebService.AuthenticateCustomer(model);
        }

        /// <summary>
        /// Provides the sign up verification method
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<VerifySignUpResponseDTO>> VerifySignUpEmail(string token)
        {
            return await TalkHomeWebService.VerifySignUpEmail(token);
        }

        /// <summary>
        /// Provides the account detials summary method
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        public async Task<GenericApiResponse<AccountSummaryResponseDTO>> GetAccountSummary(AccountSummaryRequestDTO model)
        {
            return await TalkHomeWebService.GetAccountSummary(model);
        }



        /*
        declare @FirstName varchar(200),  
        @LastName varchar(200),  
        @Email varchar(1000),  
        @AddressL1 varchar(1000),  
        @AddressL2 varchar(1000),  
        @City varchar(200),  
        @County varchar(200),  
        @PostCode varchar(100),  
        @Country varchar(100),  
        @isSimSwap bit = 0,
        @thaMSISDN varchar(50),  
        @ErrorCode int ,  
        @ErrorMessage varchar(250)

        select @FirstName = 'Darmendra', @LastName = 'Tharma',
        @Email = 'dtharma@nowtel.co.uk', @AddressL1 = 'SE1 2LY , 50 Shad Thames', @AddressL2 = '',
        @City = 'London', @County = 'Kent', @PostCode = 'SE1 2LY', @Country = 'UK', @isSimSwap = 0, @thaMSISDN = '447825152591'

        exec th_create_sim_order_promotion @FirstName,@LastName,@Email,@AddressL1,@AddressL2,@City,@County,@PostCode,@Country,@isSimSwap,@thaMSISDN,@ErrorCode output, @ErrorMessage output

        select @ErrorCode,@ErrorMessage
        */

        public async Task<DBResponseDTO> SimPromoOrder(MailOrderRequest model)
        {
            DBResponseDTO response = new DBResponseDTO();
            try
            {

                SqlConnection TalkHomeWebConnection = new SqlConnection(TalkHomeWebConnectionString);
                SqlCommand command = new SqlCommand("th_create_sim_order_promotion", TalkHomeWebConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@FirstName", SqlDbType.VarChar, 200).Value = model.FirstName;
                command.Parameters.Add("@LastName", SqlDbType.VarChar, 200).Value = model.LastName;
                command.Parameters.Add("@Email", SqlDbType.VarChar, 1000).Value = model.EmailAddress;
                command.Parameters.Add("@AddressL1", SqlDbType.VarChar, 1000).Value = model.AddressLine1;
                command.Parameters.Add("@AddressL2", SqlDbType.VarChar, 1000).Value = model.AddressLine2;
                command.Parameters.Add("@City", SqlDbType.VarChar, 200).Value = model.City;
                command.Parameters.Add("@County", SqlDbType.VarChar, 200).Value = model.CountyOrProvince;
                command.Parameters.Add("@Country", SqlDbType.VarChar, 200).Value = model.CountryCode;
                command.Parameters.Add("@PostCode", SqlDbType.VarChar, 100).Value = model.PostalCode;
                command.Parameters.Add("@isSimSWap", SqlDbType.Bit).Value = false;
                command.Parameters.Add("@thaMSISDN", SqlDbType.VarChar, 100).Value = model.ThaMsisdn;
                command.Parameters.Add("@ErrorCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                command.Parameters.Add("@ErrorMessage", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;

                TalkHomeWebConnection.Open();

                await command.ExecuteNonQueryAsync();

                response.ErrorCode = (int)command.Parameters["@ErrorCode"].Value;
                response.ErrorMessage = command.Parameters["@ErrorMessage"].Value.ToString();


                TalkHomeWebConnection.Close();



            }
            catch (SqlException ex)
            {
                LoggerService.Error(GetType(), ex.Message, ex);

            }
            return response;

        }



        public async Task<AppMsisdnResponseDTO> ValidAppUser(string msisdn)
        {
            AppMsisdnResponseDTO response = new AppMsisdnResponseDTO();
            try
            {
                SqlConnection connection = new SqlConnection(AppsConnectionString);
                SqlCommand command = new SqlCommand("tha_validate_msisdn", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@msisdn", SqlDbType.VarChar, 1000).Value = msisdn;

                command.Parameters.Add("@errorCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                command.Parameters.Add("@errormsg", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;

                connection.Open();

                await command.ExecuteNonQueryAsync();

                response.errorCode = (int)command.Parameters["@errorCode"].Value;
                if (response.errorCode == 0)
                    response.isApplicable = true;

                connection.Close();

            }
            catch (SqlException ex)
            {
                LoggerService.Error(GetType(), ex.Message, ex);

            }
            return response;

        }

        /// <summary>
        /// Provides the account detials summary method
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        public async Task<UserExistsResponseDTO> UserExists(string emailAddress)
        {

            UserExistsResponseDTO response = new UserExistsResponseDTO();

            try
            {
                SqlConnection TalkHomeWebConnection = new SqlConnection(TalkHomeWebConnectionString);
                SqlCommand command = new SqlCommand("thm_email_exist", TalkHomeWebConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@email", SqlDbType.VarChar).Value = emailAddress;
                command.Parameters.Add("@userExist", SqlDbType.Bit).Direction = ParameterDirection.Output;
                command.Parameters.Add("@hasSIM", SqlDbType.Bit).Direction = ParameterDirection.Output;
                TalkHomeWebConnection.Open();
                await command.ExecuteNonQueryAsync();
                response.UserExists = (bool)command.Parameters["@userExist"].Value;
                response.HasSim = (bool)command.Parameters["@hasSIM"].Value;
                TalkHomeWebConnection.Close();
            }
            catch (SqlException ex)
            {
                LoggerService.Error(GetType(), ex.Message, ex);
                return null;
            }
            return response;
        }



        public async Task<UserExistsResponseDTO> UserExistsandUserId(string emailAddress)
        {

            UserExistsResponseDTO response = new UserExistsResponseDTO();

            try
            {
                SqlConnection TalkHomeWebConnection = new SqlConnection(TalkHomeWebConnectionString);
                SqlCommand command = new SqlCommand("thm_email_exist_v1", TalkHomeWebConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@email", SqlDbType.VarChar).Value = emailAddress;
                command.Parameters.Add("@userExist", SqlDbType.Bit).Direction = ParameterDirection.Output;
                command.Parameters.Add("@hasSIM", SqlDbType.Bit).Direction = ParameterDirection.Output;
                command.Parameters.Add("@UserId", SqlDbType.Int).Direction = ParameterDirection.Output;
                command.Parameters.Add("@hasProduct", SqlDbType.Bit).Direction = ParameterDirection.Output;
                TalkHomeWebConnection.Open();
                await command.ExecuteNonQueryAsync();
                response.UserExists = (bool)command.Parameters["@userExist"].Value;
                response.HasSim = (bool)command.Parameters["@hasSIM"].Value;
                if (response.UserExists == true)
                {
                    response.UserId = (int)command.Parameters["@UserId"].Value;
                }
                response.hasProduct = (bool)command.Parameters["@hasProduct"].Value;

                TalkHomeWebConnection.Close();
            }
            catch (SqlException ex)
            {
                LoggerService.Error(GetType(), ex.Message, ex);
                return null;
            }
            return response;
        }


        /// <summary>
        /// Provides the account detials summary method
        /// </summary>
        /// <param name="model">The request model</param>
        /// <returns>The response model</returns>
        /// 
        /*
        declare @oldemail varchar(1000),
        @newemail varchar(1000),
        @errorCode int ,
        @errorMsg Varchar(100),
        @newFirstname varchar(100),
        @newLastname varchar(100)

        select @oldemail='darmendrat@gmail.com' , @newemail='darmendra@outlook.com'
        ,@newFirstname='Tharma',@newLastname='Darmendra'

        exec th_change_email @oldemail,@newemail ,@newFirstname,@newLastname,@errorCode output , @errorMsg output

        select @errorCode , @errorMsg
        */

        public async Task<DBResponseDTO> UpdatePersonalDetails(UpdatePersonalDetailsRequest details)
        {

            DBResponseDTO response = new DBResponseDTO();

            try
            {

                SqlConnection TalkHomeWebConnection = new SqlConnection(TalkHomeWebConnectionString);
                SqlCommand command = new SqlCommand("th_change_email", TalkHomeWebConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@oldemail", SqlDbType.VarChar, 1000).Value = details.OldEmailAddress;
                command.Parameters.Add("@newemail", SqlDbType.VarChar, 1000).Value = details.NewEmailAddress;
                command.Parameters.Add("@newFirstname", SqlDbType.VarChar, 100).Value = details.FirstName;
                command.Parameters.Add("@newLastname", SqlDbType.VarChar, 100).Value = details.LastName;
                command.Parameters.Add("@errorCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                command.Parameters.Add("@errormsg", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;

                TalkHomeWebConnection.Open();

                await command.ExecuteNonQueryAsync();

                response.ErrorCode = (int)command.Parameters["@errorCode"].Value;
                response.ErrorMessage = command.Parameters["@errormsg"].Value.ToString();


                TalkHomeWebConnection.Close();



            }
            catch (SqlException ex)
            {
                LoggerService.Error(GetType(), ex.Message, ex);

            }
            return response;
        }
        /*
        declare @email varchar(100),      
        @pin char(20),      
        @account char(20) ,      
        @password varchar(25) ,  
        @LNAME varchar(50) ,  
        @FNAME varchar(50) ,     
        @error_code int ,      
        @errormsg varchar(100)

        select @pin = '4157848331', @email = 'darmendrat@gmail.com'
        exec thcc_validate_user_pin_v1 @email,@pin,@account output, @password output ,@LNAME output, @FNAME output, @error_code output, @errormsg output
        select @account,  @password, @LNAME, @FNAME, @error_code, @errormsg
        */


        public async Task<LegacyCardUserExistsPinResponseDTO> LegacyCardUserExistsWithPin(string emailAddress, string pin)
        {
            LegacyCardUserExistsPinResponseDTO response = new LegacyCardUserExistsPinResponseDTO();
            try
            {
                SqlConnection TalkHomeWebConnection = new SqlConnection(LegacyCardsConnectionString);
                SqlCommand command = new SqlCommand("thcc_validate_user_pin_v1", TalkHomeWebConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@email", SqlDbType.VarChar, 100).Value = emailAddress;
                command.Parameters.Add("@pin", SqlDbType.VarChar, 20).Value = pin;
                command.Parameters.Add("@password", SqlDbType.VarChar, 25).Direction = ParameterDirection.Output;
                command.Parameters.Add("@account", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                command.Parameters.Add("@LNAME", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                command.Parameters.Add("@FNAME", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                command.Parameters.Add("@error_code", SqlDbType.Int).Direction = ParameterDirection.Output;
                command.Parameters.Add("@errormsg", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                TalkHomeWebConnection.Open();
                await command.ExecuteNonQueryAsync();
                response.ErrorCode = (int)command.Parameters["@error_code"].Value;
                response.ErrorMessage = command.Parameters["@errormsg"].Value.ToString();
                if (response.ErrorCode == 0)
                {
                    response.Password = command.Parameters["@password"].Value.ToString().Trim();
                    response.CallingCardNumber = command.Parameters["@account"].Value.ToString().Trim();
                    response.FirstName = command.Parameters["@FNAME"].Value.ToString().Trim();
                    response.LastName = command.Parameters["@LNAME"].Value.ToString().Trim();
                }
                TalkHomeWebConnection.Close();
            }

            catch (SqlException ex)
            {
                LoggerService.Error(GetType(), ex.Message, ex);
                return null;
            }
            return response;

        }



        /*
        ALTER procedure[dbo].[thcc_validate_user]
        @email varchar(100),
        @password varchar(25),
        @account char(20) output,
        @pin char(20) output,
        @error_code int output,
        @errormsg varchar(100) output
        @account char(20) output,    
        @LNAME varchar(50) output,
        @FNAME varchar(50) output
        
        */

        public async Task<LegacyCardUserExistsResponseDTO> LegacyCardUserExists(string emailAddress, string password)
        {
            LegacyCardUserExistsResponseDTO response = new LegacyCardUserExistsResponseDTO();
            try
            {
                SqlConnection TalkHomeWebConnection = new SqlConnection(LegacyCardsConnectionString);
                SqlCommand command = new SqlCommand("thcc_validate_user_v1", TalkHomeWebConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@email", SqlDbType.VarChar, 100).Value = emailAddress;
                command.Parameters.Add("@password", SqlDbType.VarChar, 25).Value = password;
                command.Parameters.Add("@pin", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                command.Parameters.Add("@account", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                command.Parameters.Add("@LNAME", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                command.Parameters.Add("@FNAME", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                command.Parameters.Add("@error_code", SqlDbType.Int).Direction = ParameterDirection.Output;
                command.Parameters.Add("@errormsg", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                TalkHomeWebConnection.Open();
                await command.ExecuteNonQueryAsync();
                response.ErrorCode = (int)command.Parameters["@error_code"].Value;
                response.ErrorMessage = command.Parameters["@errormsg"].Value.ToString();
                if (response.ErrorCode == 0)
                {
                    response.CallingPinNumber = command.Parameters["@pin"].Value.ToString().Trim();
                    response.CallingCardNumber = command.Parameters["@account"].Value.ToString().Trim();
                    response.FirstName = command.Parameters["@FNAME"].Value.ToString().Trim();
                    response.LastName = command.Parameters["@LNAME"].Value.ToString().Trim();
                }
                TalkHomeWebConnection.Close();
            }

            catch (SqlException ex)
            {
                LoggerService.Error(GetType(), ex.Message, ex);
                return null;
            }
            return response;

        }


        /*
        declare @oldemail varchar(1000),
        @newemail varchar(1000),
        @errorCode int ,
        @errorMsg Varchar(100),
        @newFirstname varchar(100),
        @newLastname varchar(100)

        select @oldemail = 'darmendrat@gmail.com', @newemail = 'darmendra@outlook.com'
        , @newFirstname = 'Tharma', @newLastname = 'Darmendra'

        exec th_change_email @oldemail,@newemail ,@newFirstname,@newLastname,@errorCode output, @errorMsg output

        select @errorCode , @errorMsg
        */



        public async Task<string> GetSubscriberId(string msisdn)
        {

            //UserExistsResponseDTO response = new UserExistsResponseDTO();
            string response = "";
            try
            {
                SqlConnection Connection = new SqlConnection(DigitalkMobileConnectionString);
                SqlCommand command = new SqlCommand("DBA_CLI_to_subscriber_id ", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@cli", SqlDbType.VarChar).Value = msisdn;
                Connection.Open();
                var i = await command.ExecuteScalarAsync();
                response = i.ToString();
                Connection.Close();
            }
            catch (SqlException ex)
            {
                LoggerService.Error(GetType(), ex.Message, ex);
                return null;
            }
            return response;
        }


        /// <summary>
        /// Checks the current authorization token exists and is not expired
        /// </summary>
        /// <param name="payload">The customer token</param>
        /// <returns>The result as FALSE or TRUE</returns>
        public bool IsAuthorized(JWTPayload payload)
        {
            if (string.IsNullOrEmpty(payload.ApiToken))
                return false;

            return payload.ApiTokenExpiry >= DateTime.Now;
        }

        /// <summary>
        /// Sets the culture specified by the country code for the current thread
        /// </summary>
        /// <param name="countryCode">The alpha-2 country code</param>
        public void SetCultureOnCurrentThread(string countryCode)
        {
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(countryCode);
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(countryCode);
        }

        /// <summary>
        /// Returns the list of countries
        /// </summary>
        /// <returns>The list</returns>
        public List<I18nCountry> GetCountryList()
        {
            return Countries.Instance.I18nCountries;
        }

        /// <summary>
        /// Checks if a customer has any registered products are empty
        /// </summary>
        /// <returns>The result as FALSE or TRUE</returns>
        public bool HasAnyProducts(JWTPayload payload)
        {
            return payload.ProductCodes.Any();
        }

        /// <summary>
        /// Encodes a JWT token with the given payload and returns a new cookie. Sets payload's `Updated` property
        /// </summary>
        /// <param name="payload">The payload for the cookie</param>
        /// <returns>The cookie to set in the Response</returns>
        public HttpCookie EncodeCookie(JWTPayload payload)
        {
            payload.Updated = DateTime.Now;
            return JWTService.EncodeCookie(payload);
        }

        /// <summary>
        /// Checks if a mobile number is valid with the given country code
        /// </summary>
        /// <param name="countryCode">TwoLetters country code</param>
        /// <param name="number">The number to validate</param>
        /// <returns>The number if valid, null if an error occcurred</returns>
        public PhoneNumber ValidateNumber(string number, string countryCode)
        {
            var phoneUtil = PhoneNumberUtil.GetInstance();

            try
            {
                return phoneUtil.Parse(number, countryCode);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Attempts parsing a phone number and sets the ref value to its result if successful
        /// </summary>
        /// <param name="number">The number to parse</param>
        /// <param name="countryCode">The alpha-2 country code</param>
        /// <param name="msisdn">The ref to the phone number</param>
        /// <returns>Result as TRUE or FALSE</returns>
        /// <remarks>Uses the Try pattern</remarks>
        public bool TryValidateNumber(string number, string countryCode, out string msisdn)
        {
            msisdn = "";
            var PhoneNumber = new PhoneNumber();

            var PhoneUtil = PhoneNumberUtil.GetInstance();

            try
            {
                PhoneNumber = PhoneUtil.Parse(number, countryCode);
                msisdn = PhoneUtil.Format(PhoneNumber, PhoneNumberFormat.E164).Replace("+", "");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks that a customer is not attempting to register another product on the same account
        /// </summary>
        /// <param name="productCode">The code of the product</param>
        /// <returns>The result as FALSE or TRUE</returns>
        public bool IsAlreadyActive(string productCode, JWTPayload payload)
        {
            foreach (var product in payload.ProductCodes)
                if (product.Equals(productCode))
                    return true;

            return false;
        }

        /// <summary>
        /// Validates Talk Home Product codes
        /// </summary>
        /// <param name="productCode">The product code</param>
        /// <param name="error">The ref to the error</param>
        /// <returns>Result as TRUE | FALSE</returns>
        /// <remarks>Uses the Try pattern</remarks>
        public bool TryValidateProductCode(string productCode, out string error)
        {
            error = "";

            Models.Enums.ProductCodes Output;

            if (!Enum.TryParse(productCode, out Output))
            {
                error = ((int)Messages.InvalidProductCode).ToString();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the human-readable product code from the API product code
        /// </summary>
        /// <param name="productCode">The product code</param>
        /// <param name="error">The ref to the error</param>
        /// <returns>The readable product code</returns>
        /// <remarks>Uses the Try pattern</remarks>
        public string GetReadableProductCode(string productCode, out string error)
        {
            error = "";

            if (!TryValidateProductCode(productCode, out error))
                return error;

            var ReadableProductCode = (Enum)Enum.Parse(typeof(ReadableProductCodes), productCode);

            return ReadableProductCode.GetDisplayName();
        }

        /// <summary>
        /// Returns the product name based on product code
        /// </summary>
        /// <param name="productCode">The product code</param>
        /// <param name="error">The ref to the error</param>
        /// <returns>The product name</returns>
        /// <remarks>Uses the Try pattern</remarks>
        public string GetProductName(string productCode, out string error)
        {
            error = "";

            if (!TryValidateProductCode(productCode, out error))
                return error;

            var ProductName = (Enum)Enum.Parse(typeof(ProductNames), productCode);

            return ProductName.GetDisplayName();
        }

        /// <summary>
        /// Unsafe method to convert from national number to Msisdn
        /// </summary>
        /// <param name="number">The numner</param>
        /// <param name="countryCode">the alpha-2 country code</param>
        /// <returns>The Msisdn</returns>
        public string GetMsisdnFromNumber(string number, string countryCode)
        {
            var PhoneUtil = PhoneNumberUtil.GetInstance();
            return PhoneUtil.Format(PhoneUtil.Parse(number, countryCode), PhoneNumberFormat.E164).Replace("+", "");
        }




        public string GetRechargeableNumber(string pin, out int error)
        {
            using (SqlConnection con = new SqlConnection(LegacyCardsConnectionString))
            {
                try
                {
                    DataTable dt = new DataTable();

                    SqlCommand cmd = new SqlCommand("thcc_get_pin_account", con);
                    cmd.Parameters.Add("@pin", SqlDbType.VarChar, 20).Value = pin;
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    if (dt.Rows.Count > 1)
                    {
                        error = 2;
                        return "";
                    }
                    else if (dt.Rows.Count == 0)
                    {
                        error = 1;
                        return "";
                    }
                    else
                    {
                        DataRow dr = dt.Rows[0];
                        error = 0;
                        string num = dr[0].ToString();
                        return num;
                    }

                }
                catch (SqlException ex)
                {
                    error = 1;
                    return null;
                }
            }
        }


        /// <summary>
        /// Unsafe method to convert from national number to Msisdn
        /// </summary>
        /// <param name="msisdn">The Msisdn</param>
        /// <param name="countryCode">the alpha-2 country code</param>
        /// <returns>The number</returns>
        public string GetNumberFromMsisdn(string msisdn, string countryCode)
        {
            var PhoneUtil = PhoneNumberUtil.GetInstance();
            return PhoneUtil.Format(PhoneUtil.Parse(msisdn, countryCode), PhoneNumberFormat.NATIONAL);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AccountDetails"></param>
        /// <param name="addressModel"></param>
        /// <returns></returns>
        public bool TryGetBillingAddress(GenericApiResponse<AccountDetailsResponseDTO> AccountDetails, out AddressModel addressModel)
        {
            if (AccountDetails.errorCode != 0 || AccountDetails.payload.addresses.billing == null)
            {
                addressModel = new AddressModel();
                return false;
            }

            addressModel = AccountDetails.payload.addresses.billing;
            return true;
        }

        /// <summary>
        /// Works out the result of a promotional sign up Web Api response
        /// </summary>
        /// <param name="requestDTO">The request model</param>
        /// <param name="responseDTO">The response model</param>
        /// <param name="error">The ref to the error</param>
        /// <returns>The result as TRUE or FALSE</returns>
        /// <remarks>Uses the Try pattern</remarks>
        public bool TryPromoSignUpSuccess(PromoSignUpRequestDTO requestDTO, GenericApiResponse<string> responseDTO, out string error)
        {
            error = "";

            if (responseDTO.status != 200 || responseDTO.errorCode != 0)
            {
                error = ((int)Messages.PromoSignUpFailure).ToString();

                LoggerService.Info(GetType(), string.Format("{0} {1}", "Failed promo sign up for", requestDTO.Email));
                return false;
            }

            LoggerService.Info(GetType(), string.Format("{0} {1}", "Promo sign up for:", requestDTO.Email));
            return true;
        }

        public int AddCompitionUser(AddCompitionUserRequestModel model)
        {
            try
            {

                SqlConnection TalkHomeWebConnection = new SqlConnection(TalkHomeWebConnectionString);
                SqlCommand command = new SqlCommand("th_insert_compition_log", TalkHomeWebConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@name", SqlDbType.NVarChar, 200).Value = model.Name;
                command.Parameters.Add("@email", SqlDbType.NVarChar, 200).Value = model.Email;
                command.Parameters.Add("@promo_name", SqlDbType.NVarChar, 200).Value = model.Promoname;
                command.Parameters.Add("@signme", SqlDbType.Bit, 200).Value = model.signme;
                TalkHomeWebConnection.Open();
                var result = command.ExecuteNonQuery();
                TalkHomeWebConnection.Close();
                if (result != 0)
                {
                    result = 1;
                }
                else
                {
                    result = 0;
                }
                return result;
            }
            catch (SqlException ex)
            {
                LoggerService.Error(GetType(), ex.Message, ex);
                return 0;

            }
        }

        public int IsRegistered(string email, string promoname)
        {
            try
            {

                SqlConnection TalkHomeWebConnection = new SqlConnection(TalkHomeWebConnectionString);
                SqlCommand command = new SqlCommand("th_IsRegistered", TalkHomeWebConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@email", SqlDbType.NVarChar, 200).Value = email;
                command.Parameters.Add("@promoname", SqlDbType.NVarChar, 200).Value = promoname;
                command.Parameters.Add("@errorcode", SqlDbType.Int).Direction = ParameterDirection.Output;
                TalkHomeWebConnection.Open();
                command.ExecuteNonQuery();
                var result = (int)command.Parameters["@errorcode"].Value;
                TalkHomeWebConnection.Close();
                if (result != 0)
                {
                    result = 0;
                }
                else
                {
                    result = 1;
                }
                return result;
            }
            catch (SqlException ex)
            {
                LoggerService.Error(GetType(), ex.Message, ex);
                return 0;

            }
        }
        public async Task<ThResetPassword> InsertPasswordToken(string email)
        {
            ThResetPassword trp = new ThResetPassword();
            try
            {
                SqlConnection TalkHomeWebConnection = new SqlConnection(TalkHomeWebConnectionString);
                SqlCommand command = new SqlCommand("th_insert_reset_password_resquest", TalkHomeWebConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@email", SqlDbType.VarChar).Value = email;
                TalkHomeWebConnection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        trp.ReturnCode = (int)reader[0];
                        if (!Convert.ToBoolean(trp.ReturnCode))
                        {
                            trp.UniqueId = (string)reader[1];
                            trp.Email = (string)reader[2];
                        }
                        else
                        {
                            return trp;
                        }
                    }
                }
                TalkHomeWebConnection.Close();
                return trp;
            }
            catch (SqlException ex)
            {
                LoggerService.Error(GetType(), ex.Message, ex);
                return null;
            }


        }

        public async Task<string> SubscriberName(string email)
        {
            SqlConnection TalkHomeWebConnection = new SqlConnection(TalkHomeWebConnectionString);
            SqlCommand command = new SqlCommand("SubscriberName", TalkHomeWebConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@email", SqlDbType.VarChar).Value = email;
            TalkHomeWebConnection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader[0] != null)
                    {
                        return (string)reader[0];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            TalkHomeWebConnection.Close();
            return null;
        }
        //Tested
        public async Task<SimReturnResponse> InsertSimOrder(MailOrderRequestDTO so)
        {
            SimReturnResponse sr = new SimReturnResponse();
            try
            {

                sr.reference = 0;
                sr.errorCode = 1;


                SqlConnection TalkHomeWebConnection = new SqlConnection(TalkHomeWebConnectionString);
                SqlCommand command = new SqlCommand("th_create_sim_order_v3", TalkHomeWebConnection);
                command.CommandType = CommandType.StoredProcedure;


                command.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = so.firstName;
                command.Parameters.Add("@LastName", SqlDbType.VarChar).Value = so.lastName;
                command.Parameters.Add("@Email", SqlDbType.VarChar).Value = so.email;
                command.Parameters.Add("@AddressL1", SqlDbType.VarChar).Value = so.addressL1;
                command.Parameters.Add("@AddressL2", SqlDbType.VarChar).Value = so.addressL2;
                command.Parameters.Add("@City", SqlDbType.VarChar).Value = so.city;
                command.Parameters.Add("@County", SqlDbType.VarChar).Value = so.county;
                command.Parameters.Add("@PostCode", SqlDbType.VarChar).Value = so.postCode;
                command.Parameters.Add("@Country", SqlDbType.VarChar).Value = so.country;
                command.Parameters.Add("@isSimSwap", SqlDbType.VarChar).Value = so.isSimSwap;


                SqlParameter @reference = new SqlParameter("@reference", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                @reference.Value = null;
                command.Parameters.Add(@reference);


                SqlParameter ErrorCode = new SqlParameter("@ErrorCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                ErrorCode.Value = null;
                command.Parameters.Add(ErrorCode);

                SqlParameter ErrorMessage = new SqlParameter("@ErrorMessage", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Output,
                    Size = 200
                };
                ErrorMessage.Value = null;
                command.Parameters.Add(ErrorMessage);



                SqlParameter @Id = new SqlParameter("@Id", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.Output
                };
                @Id.Value = null;
                command.Parameters.Add(@Id);


                TalkHomeWebConnection.Open();

                var confirmation = command.ExecuteNonQuery();

                long OrderId = Convert.ToInt64(@Id.Value);

                int referenceValue = Convert.ToInt32(@reference.Value);
                var errorOccured = Convert.ToInt32(ErrorCode.Value);

                var errorMsg = (dynamic)ErrorMessage.Value;
                sr.errorCode = errorOccured;
                sr.reference = referenceValue;
                sr.OrderId = OrderId;
                return sr;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public IEnumerable<BillingCountries> GetCountries()
        {

            try
            {
                SqlConnection TalkHomeWebConnection = new SqlConnection(TalkHomeWebConnectionString);
                SqlCommand command = new SqlCommand("th_getcountries", TalkHomeWebConnection);
                command.CommandType = CommandType.StoredProcedure;
                TalkHomeWebConnection.Open();

                //using (SqlDataReader reader = command.ExecuteReader())
                //{
                //    while (reader.Read())
                //    {



                //    }
                //}

                using (var reader = command.ExecuteReader())
                    return reader.Cast<IDataRecord>()
                        .Select(x => new BillingCountries(x.GetString(0), x.GetString(1), x.GetInt32(2)))
                        .ToList();



                //TalkHomeWebConnection.Close();
                //return countryInfo;
            }
            catch (Exception e)
            {
                return null;
            }


        }
        public string Get_Sim_ActivationDate(string Msisdn)
        {
            string result = "";
            try
            {

                SqlConnection TalkHomeWebConnection = new SqlConnection(DigitalkMobileDbConnectionString);
                SqlCommand command = new SqlCommand("get_sim_activationdate", TalkHomeWebConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@msisdn", SqlDbType.VarChar).Value = Msisdn;
                TalkHomeWebConnection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result = reader[0].ToString();
                    }
                };
                return result;
            }
            catch (Exception e)
            {

                return null;
            }
        }


        public async Task<string> Get_UserId(string Email)
        {
            string result = "";
            try
            {
                SqlConnection TalkHomeWebConnection = new SqlConnection(TalkHomeWebConnectionString);
                SqlCommand command = new SqlCommand("get_userIdbyEmail", TalkHomeWebConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@email", SqlDbType.VarChar).Value = Email;
                TalkHomeWebConnection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result = reader[0].ToString();
                    }
                };
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public int SaveGCLID_NormalSim(string GCLID, long OrderId)
        {
            try
            {
                SqlConnection TalkHomeWebConnection = new SqlConnection(TalkHomeWebConnectionString);
                SqlCommand command = new SqlCommand("INSERT_GCLID_NormalSim", TalkHomeWebConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@GCLID", SqlDbType.NVarChar).Value = GCLID;
                command.Parameters.Add("@OrderId", SqlDbType.BigInt).Value = OrderId;
                TalkHomeWebConnection.Open();
                var result = command.ExecuteNonQuery();
                return result;
            }
            catch (Exception)
            {

                return 0;
            }
        }





        public int SaveGCLID_CreditSim(string GCLID, long OrderId)
        {
            try
            {
                SqlConnection TalkHomeWebConnection = new SqlConnection(TalkHomeWebConnectionString);
                SqlCommand command = new SqlCommand("INSERT_GCLID_CreditSim", TalkHomeWebConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@GCLID", SqlDbType.NVarChar).Value = GCLID;
                command.Parameters.Add("@OrderId", SqlDbType.BigInt).Value = OrderId;
                TalkHomeWebConnection.Open();
                var result = command.ExecuteNonQuery();
                return result;
            }
            catch (Exception)
            {

                return 0;
            }
        }

        public int Insertemail_minutemaker(Insertemail_minutemaker_model model)
        {

            SqlConnection TalkHomeWebConnection = new SqlConnection(TalkHomeWebConnectionString);
            SqlCommand command = new SqlCommand("insert_email_subscriptions", TalkHomeWebConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@email", SqlDbType.VarChar).Value = model.Email;
            command.Parameters.Add("@emailtypeid", SqlDbType.Int).Value = model.type;
            TalkHomeWebConnection.Open();
            var result = command.ExecuteNonQuery();

            return result;


        }

        public async Task<int> Insert_otp_data(int otp, string email)
        {

            SqlConnection TalkHomeWebConnection = new SqlConnection(TalkHomeWebConnectionString);
            SqlCommand command = new SqlCommand("thm_Insert_otp", TalkHomeWebConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@email", email);
            command.Parameters.Add("@otp", otp);

            try
            {
                TalkHomeWebConnection.Open();
                var result = await command.ExecuteNonQueryAsync();
                TalkHomeWebConnection.Close();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> Verify_user_otp(int otp, int expiry, string EmailAddress)
        {

            SqlConnection TalkHomeWebConnection = new SqlConnection(TalkHomeWebConnectionString);
            SqlCommand command = new SqlCommand("Thm__VerifyUserby_otp", TalkHomeWebConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@email", EmailAddress);
            command.Parameters.Add("@otp", otp);
            command.Parameters.Add("@ExpiryHours", expiry);
            command.Parameters.Add("@return_value", SqlDbType.Int).Direction = ParameterDirection.Output;

            try
            {
                TalkHomeWebConnection.Open();
                await command.ExecuteNonQueryAsync();
                var result = (int)command.Parameters["@return_value"].Value;
                TalkHomeWebConnection.Close();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public async Task<string> GetUserByEmail(string email)
        {
            var parameter = new DynamicParameters();

            SqlConnection TalkHomeWebConnection = new SqlConnection(TalkHomeWebConnectionString);
            SqlCommand command = new SqlCommand("thm_revamp_GetUserByEmail", TalkHomeWebConnection);
            command.Parameters.Add("@email", email);
            command.CommandType = CommandType.StoredProcedure;
            TalkHomeWebConnection.Open();
            string userstatus = "";
            try
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        userstatus = reader["IsConfirmedUser"].ToString();
                    }
                }
                TalkHomeWebConnection.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return userstatus;
        }

        public int Isvalid_simorder(string email)
        {
            try
            {

                SqlConnection TalkHomeWebConnection = new SqlConnection(TalkHomeWebConnectionString);
                SqlCommand command = new SqlCommand("thm_sim_order_validation", TalkHomeWebConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@email", email);
                command.Parameters.Add("@ErrorCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                TalkHomeWebConnection.Open();
                command.ExecuteNonQuery();
                var result = (int)command.Parameters["@ErrorCode"].Value;
                TalkHomeWebConnection.Close();

                return result;
            }
            catch (SqlException ex)
            {
                LoggerService.Error(GetType(), ex.Message, ex);
                return 0;

            }
        }

        public async Task<int> SImOrderValidations(string email, string address, string postcode)
        {
            try
            {
                SqlConnection TalkHomeWebConnection = new SqlConnection(TalkHomeWebConnectionString);
                SqlCommand command = new SqlCommand("thm_sim_order_validation", TalkHomeWebConnection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@Email", SqlDbType.VarChar).Value = email;
                command.Parameters.Add("@PostCode", SqlDbType.VarChar).Value = postcode;
                command.Parameters.Add("@Address", SqlDbType.VarChar).Value = address;

                command.Parameters.Add("@ErrorCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                TalkHomeWebConnection.Open();
                await command.ExecuteNonQueryAsync();
                var result = (int)command.Parameters["@ErrorCode"].Value;
                TalkHomeWebConnection.Close();

                return result;
            }
            catch (SqlException ex)
            {
                LoggerService.Error(GetType(), ex.Message, ex);
                return 2;
            }
        }

        public async Task<int> IsEmailRegistered(string emailAddress)
        {
            try
            {
                SqlConnection TalkHomeWebConnection = new SqlConnection(TalkHomeWebConnectionString);
                SqlCommand command = new SqlCommand("thm_revamp_IsEmailAlreadyRegisteredORConfirmed", TalkHomeWebConnection);

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@Email", SqlDbType.VarChar).Value = emailAddress;

                TalkHomeWebConnection.Open();
                var response = Convert.ToInt32(await command.ExecuteScalarAsync());
                TalkHomeWebConnection.Close();

                return response;
            }
            catch (SqlException ex)
            {
                LoggerService.Error(GetType(), ex.Message, ex);
                return 0;
            }
        }

        public async Task<bool> Verifyuseremail_against_otpAsync(string email)
        {
            try
            {
                SqlConnection TalkHomeWebConnection = new SqlConnection(TalkHomeWebConnectionString);
                SqlCommand command = new SqlCommand("thm_revamp_CheckOtpStatusViaEmail", TalkHomeWebConnection);

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@Email", email);
                command.Parameters.Add("@IsValid", SqlDbType.Bit).Direction = ParameterDirection.Output;

                TalkHomeWebConnection.Open();

                await command.ExecuteNonQueryAsync();
                bool result = (bool)command.Parameters["@IsValid"].Value;
                
                TalkHomeWebConnection.Close();

                return result;
            }
            catch (SqlException ex)
            {
                LoggerService.Error(GetType(), ex.Message, ex);
                return false;
            }
        }
    }
}
