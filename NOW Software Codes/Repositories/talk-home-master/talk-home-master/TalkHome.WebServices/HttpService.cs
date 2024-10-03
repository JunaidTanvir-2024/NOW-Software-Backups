using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TalkHome.WebServices.Interfaces;
using TalkHome.Models.Enums;
using TalkHome.Logger;
using TalkHome.Models.CustomExceptions;

namespace TalkHome.WebServices
{
    /// <summary>
    /// Handles the actual HTTP requests to the WebApi
    /// </summary>
    class HttpService : IHttpService
    {
        private readonly ILoggerService LoggerService;
        private Properties.URIs URIs = Properties.URIs.Default;
        private Properties.Hosts Hosts = Properties.Hosts.Default;
        private Properties.Creds Creds = Properties.Creds.Default;

        public HttpService(ILoggerService loggerService)
        {
            LoggerService = loggerService;
        }

        /// <summary>
        /// Encodes default Web Api credentials
        /// </summary>
        /// <returns>The encoded string</returns>
        private string GetWebApiBaiscAuth()
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(Creds.WebApiBasicAuth));
        }

        /// <summary>
        /// Encodes AddressIo credentials
        /// </summary>
        /// <returns>The encoded string</returns>
        private string GetAddressIoAuth()
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(Creds.AddressIoAuth));
        }

        /// <summary>
        /// Encodes default Talk Home App API basic auth credentials
        /// </summary>
        /// <returns>The encoded string</returns>
        private string GetAppApiBasicAuth()
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(Creds.AppApiBasicAuth));
        }


        private string GetAppApiBasicAuthGBP()
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(Creds.AppApiBasicAuthGBP));
        }
        private string GetAppApiBasicAuthGBPEUR()
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(Creds.AppApiBasicAuthEUR));
        }
        private string GetAppApiBasicAuthGBPUSD()
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(Creds.AppApiBasicAuthUSD));
        }



        /// <summary>
        /// Returns the correct host based on the application environment
        /// </summary>
        /// <param name="type">The request type</param>
        /// <returns>The host or null</returns>
        private string GetServiceHost(ApiRequestType type)
        {
#pragma warning disable CS0162
#if (!DEBUG)
            switch ((int)type)
            {
                case 1: return Hosts.ProdWebApi;

                case 2: return Hosts.AddressIo;

                case 3: return Hosts.IpInfo;

                case 4: return Hosts.ProdAppApi;

                case 5: return Hosts.ProdPayment; 
            
                case 6: return Hosts.LabAppApi;

                 case 7:return Hosts.ProdThmApi;

                default: return null;
            }

            return null;
#endif
            switch ((int)type)
            {
                case 1: return Hosts.LabWebApi;

                case 2: return Hosts.AddressIo;

                case 3: return Hosts.IpInfo;

                case 4: return Hosts.LabAppApi;

                case 5: return Hosts.LabPayment;

                case 6: return Hosts.LabAppApi;
                case 7: return Hosts.LabThmApi;
            }

            return null;
        }

        /// <summary>
        /// Formats the request address
        /// </summary>
        /// <param name="baseUri">The base of the request address</param>
        /// <param name="type">The type of request</param>
        /// <returns>The address or null</returns>
        /// <remarks>
        /// AddressIo and IpInfo are special cases as their address is formatted differently
        /// </remarks>
        private string CreateAddress(string baseUri, ApiRequestType type)
        {
            var Host = GetServiceHost(type);

            switch ((int)type)
            {
                case 1:
                case 5:
                    return string.Format("{0}{1}", Host, baseUri);

                case 2: return string.Format("{0}/{1}", Host, baseUri); // Just append the post code a the end of the Host

                case 3: return string.Format("{0}/{1}/json?token={2}", Host, baseUri, Creds.IpInfoKey);

                case 4: return string.Format("{0}{1}?msisdn={2}", Host, URIs.GetAppUserByMsisdn, baseUri);

                case 6: return string.Format("{0}{1}", Host, baseUri);
                case 7: return string.Format("{0}{1}", Host, baseUri);
            }

            return null;
        }

        /// <summary>
        /// Performs an async GET request
        /// </summary>
        /// <param name="baseUri">The resource requested</param>
        /// <param name="type">the type of request</param>
        /// <returns>The response or null</returns>
        public async Task<string> Get(string baseUri, ApiRequestType type)
        {
            var Address = CreateAddress(baseUri, type);

            LoggerService.Debug(GetType(), Address);

            try
            {
                using (var client = new HttpClient { BaseAddress = new Uri(Address), Timeout = TimeSpan.FromSeconds(60) })
                {
                    switch ((int)type)
                    {
                        case 1:
                            client.DefaultRequestHeaders.Add("Authorization", "Basic " + GetWebApiBaiscAuth());
                            break;

                        case 2:
                            client.DefaultRequestHeaders.Add("Authorization", "Basic " + GetAddressIoAuth());
                            break;

                        case 4:
                            client.DefaultRequestHeaders.Add("Authorization", "Basic " + GetAppApiBasicAuth());
                            break;
                    }

                    HttpResponseMessage response = await client.GetAsync("");
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        throw new UnAuthorisedException();
                    }

                    if (!response.IsSuccessStatusCode || response.Content == null)
                        throw new WebException();

                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (UnAuthorisedException)
            {
                throw;
            }
            catch (Exception e)
            {
                LoggerService.SendCriticalAlert((int)Messages.WebApiHttpException);
                LoggerService.Error(GetType(), e.Message + "  Address:" + Address + ".   ApiRequestType:" + type, e);
                return null;
            }
        }

        public async Task<string> Get(string baseUri, ApiRequestType type, string CurrencyType)
        {
            var Address = CreateAddress(baseUri, type);

            LoggerService.Debug(GetType(), Address);

            try
            {
                using (var client = new HttpClient { BaseAddress = new Uri(Address), Timeout = TimeSpan.FromSeconds(60) })
                {
                    switch (CurrencyType)
                    {
                        case "GBP":
                            client.DefaultRequestHeaders.Add("Authorization", "Basic " + GetAppApiBasicAuth());
                            break;

                        case "EUR":
                            client.DefaultRequestHeaders.Add("Authorization", "Basic " + GetAppApiBasicAuthGBPEUR());
                            break;

                        case "USD":
                            client.DefaultRequestHeaders.Add("Authorization", "Basic " + GetAppApiBasicAuthGBPUSD());
                            break;

                    }

                    HttpResponseMessage response = await client.GetAsync("");
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        throw new UnAuthorisedException();
                    }

                    if (!response.IsSuccessStatusCode || response.Content == null)
                        throw new WebException();

                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (UnAuthorisedException)
            {
                throw;
            }
            catch (Exception e)
            {
                LoggerService.SendCriticalAlert((int)Messages.WebApiHttpException);
                LoggerService.Error(GetType(), e.Message, e);
                return null;
            }
        }


        /// <summary>
        /// Performs an async POST request
        /// </summary>
        /// <param name="baseUri">The resource requested</param>
        /// <param name="json">The content for the request</param>
        /// <param name="apiToken">The auth token provided by the Web Api</param>
        /// <param name="type">the type of request</param>
        /// <returns>The response or null</returns>
        public async Task<string> Post(string baseUri, string json, string apiToken, ApiRequestType type)
        {
            var Address = CreateAddress(baseUri, type);

            var Content = (type != ApiRequestType.ThmApi ? new StringContent(json, Encoding.Default, "application/json") : new StringContent(json, Encoding.UTF8, "application/json"));
            //var Content = new StringContent(json, Encoding.Default, "application/json");
            if (Address == "https://api.talkhome.co.uk/talkhome/sim/order/new")
            {
                Address = "http://172.24.1.70:14001/talkhome/sim/order/new";
            }
            LoggerService.Debug(GetType(), Address);

            try
            {
                using (var client = new HttpClient { BaseAddress = new Uri(Address), Timeout = TimeSpan.FromSeconds(60) })
                {
                    switch ((int)type)
                    {
                        case 1:
                            if (string.IsNullOrEmpty(apiToken))
                                client.DefaultRequestHeaders.Add("Authorization", "Basic " + GetWebApiBaiscAuth());
                            else
                                Content.Headers.Add("Token", apiToken);
                            break;
                    }

                    HttpResponseMessage response = await client.PostAsync("", Content);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        throw new UnAuthorisedException();
                    }

                    if (!response.IsSuccessStatusCode || response.Content == null)
                        throw new WebException();

                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (UnAuthorisedException)
            {
                throw;
            }
            catch (Exception e)
            {
                LoggerService.SendCriticalAlert((int)Messages.WebApiHttpException);
                LoggerService.Error(GetType(), e.Message, e);
                return null;
            }
        }
    }
}
