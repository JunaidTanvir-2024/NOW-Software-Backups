using KFCDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace KFCFPAPI.App_Start
{
    public class FPService
    {
        private string baseURL = string.Empty;

        private string userName = string.Empty;

        private string password = string.Empty;

        private bool UseProxy = true;

        public FPService()
        {
            baseURL = ConfigurationManager.AppSettings["BaseURL"].ToString();
            userName = ConfigurationManager.AppSettings["userName"].ToString();
            password = ConfigurationManager.AppSettings["uPassword"].ToString();
        }

        private string getAccessToken()
        {
            string requestURL = "v2/login";
            string myURL = baseURL + requestURL;
            string grantType = "client_credentials";
            string data = "username=" + userName + "&password=" + password + "&grant_type=" + grantType;
            try
            {
                using (WebClient client = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    //ServicePointManager.ServerCertificateValidationCallback = (object P_0, X509Certificate P_1, X509Chain P_2, SslPolicyErrors P_3) => true;
                    if (UseProxy)
                    {
                        client.Proxy = new WebProxy();
                    }
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                    string str = client.UploadString(myURL, "POST", data);
                    LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(str);
                    if (loginResponse.access_token != null && !string.IsNullOrEmpty(loginResponse.access_token))
                    {
                        return loginResponse.access_token;
                    }
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public string RejectOrder(string token)
        {
            string requestURL = "/v2/order/status/" + token;
            DateTime startTime = DateTime.Now;
            string myURL = baseURL + requestURL;
            string payLoad = string.Empty;
            string apiType = "order_rejected";
            try
            {
                string accessToken = getAccessToken();
                if (!string.IsNullOrEmpty(accessToken))
                {
                    using (WebClient webClient = new WebClient())
                    {
                        OrderRejectPayload value = new OrderRejectPayload
                        {
                            message = "Duplicate Order",
                            status = apiType,
                            reason = "MENU_ACCOUNT_SETTINGS"
                        };
                        webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                        webClient.Headers.Add("Authorization", "bearer " + accessToken);
                        if (UseProxy)
                        {
                            webClient.Proxy = new WebProxy();
                        }
                        payLoad = JsonConvert.SerializeObject(value);
                        string response = webClient.UploadString(myURL, "POST", payLoad);
                        LogAPIResponse(myURL, payLoad, response, "OK", startTime, apiType);
                        return "Success";
                    }
                }
                LogAPIResponse(baseURL + "v2/login", payLoad, "Unable to get Access Token", "ERROR", startTime, "Token");
                return string.Empty;
            }
            catch (WebException ex)
            {
                using (Stream stream = ex.Response.GetResponseStream())
                {
                    string response2 = string.Empty;
                    using (StreamReader streamReader = new StreamReader(stream))
                    {
                        response2 = streamReader.ReadToEnd();
                    }
                    LogAPIResponse(myURL, payLoad, response2, ex.Message, startTime, apiType);
                }
                return string.Empty;
            }
            catch (Exception ex2)
            {
                LogAPIResponse(myURL, payLoad, "Error occoured while calling api", ex2.Message, startTime, apiType);
                return string.Empty;
            }
        }

        public string AcceptOrder(string token, string orderId)
        {
            string requestURL = "/v2/order/status/" + token;
            DateTime startTime = DateTime.Now;
            string myURL = baseURL + requestURL;
            string payLoad = string.Empty;
            string apiType = "order_accepted";
            try
            {
                string accessToken = getAccessToken();
                if (!string.IsNullOrEmpty(accessToken))
                {
                    using (WebClient webClient = new WebClient())
                    {
                        OrderAcceptedPayload value = new OrderAcceptedPayload
                        {
                            acceptanceTime = DateTime.Now.AddMinutes(30.0).ToString(),
                            remoteOrderId = orderId,
                            status = apiType
                        };
                        webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                        webClient.Headers.Add("Authorization", "bearer " + accessToken);
                        if (UseProxy)
                        {
                            webClient.Proxy = new WebProxy();
                        }
                        payLoad = JsonConvert.SerializeObject(value);
                        string response = webClient.UploadString(myURL, "POST", payLoad);
                        LogAPIResponse(myURL, payLoad, response, "OK", startTime, apiType);
                        return "Success";
                    }
                }
                LogAPIResponse(baseURL + "v2/login", payLoad, "Unable to get Access Token", "ERROR", startTime, "Token");
                return string.Empty;
            }
            catch (WebException ex)
            {
                using (Stream stream = ex.Response.GetResponseStream())
                {
                    string response2 = string.Empty;
                    using (StreamReader streamReader = new StreamReader(stream))
                    {
                        response2 = streamReader.ReadToEnd();
                    }
                    LogAPIResponse(myURL, payLoad, response2, ex.Message, startTime, apiType);
                }
                return string.Empty;
            }
            catch (Exception ex2)
            {
                LogAPIResponse(myURL, payLoad, "Error occoured while calling api", ex2.Message, startTime, apiType);
                return string.Empty;
            }
        }

        public string SendAcknowledgment(string token, string orderId)
        {
            string requestURL = "/v2/order/status/" + token;
            DateTime startTime = DateTime.Now;
            string myURL = baseURL + requestURL;
            string payLoad = string.Empty;
            string apiType = "order_accepted";
            try
            {
                string accessToken = getAccessToken();
                if (!string.IsNullOrEmpty(accessToken))
                {
                    using (WebClient webClient = new WebClient())
                    {
                        OrderAcceptedPayload value = new OrderAcceptedPayload
                        {
                            acceptanceTime = DateTime.Now.AddMinutes(30.0).ToString(),
                            remoteOrderId = orderId,
                            status = apiType
                        };
                        webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                        webClient.Headers.Add("Authorization", "bearer " + accessToken);
                        if (UseProxy)
                        {
                            webClient.Proxy = new WebProxy();
                        }
                        payLoad = JsonConvert.SerializeObject(value);
                        string response = webClient.UploadString(myURL, "POST", payLoad);
                        LogAPIResponse(myURL, payLoad, response, "OK", startTime, apiType);
                        return "Success";
                    }
                }
                LogAPIResponse(baseURL + "v2/login", payLoad, "Unable to get Access Token", "ERROR", startTime, "Token");
                return string.Empty;
            }
            catch (WebException ex)
            {
                using (Stream stream = ex.Response.GetResponseStream())
                {
                    string response2 = string.Empty;
                    using (StreamReader streamReader = new StreamReader(stream))
                    {
                        response2 = streamReader.ReadToEnd();
                    }
                    LogAPIResponse(myURL, payLoad, response2, ex.Message, startTime, apiType);
                }
                return string.Empty;
            }
            catch (Exception ex2)
            {
                LogAPIResponse(myURL, payLoad, "Error occoured while calling api", ex2.Message, startTime, apiType);
                return string.Empty;
            }
        }

        private void LogAPIResponse(string apiLink, string requestJson, string response, string description, DateTime startTime, string apiType)
        {
            DateTime endTime = DateTime.Now;
            TimeSpan timeSpan = endTime - startTime;
            double duration = timeSpan.TotalSeconds;
            using (KFC_PortalEntities entity = new KFC_PortalEntities())
            {
                entity.Proc_LogFPAPICall(apiLink, requestJson, response, description, startTime, endTime, duration.ToString(), apiType);
            }
        }

    }

    class LoginResponse
    {
        public string access_token { get; set; }

        public int expires_in { get; set; }

        public string token_type { get; set; }
    }

    class OrderAcceptedPayload
    {
        public string acceptanceTime { get; set; }

        public string remoteOrderId { get; set; }

        public string status { get; set; }
    }

    class OrderRejectPayload
    {
        public string message { get; set; }

        public string reason { get; set; }

        public string status { get; set; }
    }
}