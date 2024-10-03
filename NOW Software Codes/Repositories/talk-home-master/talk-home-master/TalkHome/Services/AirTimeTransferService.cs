using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TalkHome.Interfaces;
using TalkHome.Models;
using System.Configuration;
using TalkHome.Logger;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using TalkHome.Models.Enums;
using TalkHome.Models.WebApi;
using TalkHome.Models.WebApi.DTOs;
using Newtonsoft.Json;
using System.Text;

namespace TalkHome.Services
{
    public class AirTimeTransferService : IAirTimeTransferService
    {
        
        private readonly string BaseUrl;
        private readonly ILoggerService LoggerService;
        
        public AirTimeTransferService(ILoggerService loggerService)
        {
            //BaseUrl = ConfigurationManager.AppSettings["SochitelAPI"];
            BaseUrl = ConfigurationManager.AppSettings["DtOneAPI"];

            LoggerService = loggerService;
        }


        public async Task<TransferResponse> Transfer(TransferRequestDTO request)
        {
            //string fullApiCall = BaseUrl + "/transferToExecuteTransaction";
            string fullApiCall = BaseUrl + "DTOneExecute";

            var Json = JsonConvert.SerializeObject(request);

            HttpContent content = new StringContent(Json, Encoding.UTF8, "application/json");

            try
            {
                using (var client = new HttpClient { BaseAddress = new Uri(fullApiCall), Timeout = TimeSpan.FromSeconds(60)})
                {
                    HttpResponseMessage response = await client.PostAsync("",content);

                    if (!response.IsSuccessStatusCode || response.Content == null)
                        throw new WebException();


                    var res = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<TransferResponse>(res);
                }
            }
            catch (Exception e)
            {
                LoggerService.SendCriticalAlert((int)Messages.WebApiHttpException);
                LoggerService.Error(GetType(), e.Message, e);
                return null;
            }
        }
        
        public async Task<string> GetOperators(string Msisdn,string fromMsisdn)
        {
        /* http://localhost:10000/transfertoGetOperatorProductsMSISDN?nsid=1&account=EUR&destinationMSISDN=+13122010300 */

        /* http://172.24.1.196:7009/transfertoGetOperatorProductsMSISDN?nsid=1&account=GBP&destinationMSISDN=447825152591 */

        /*http://172.24.1.196:7009/transfertoGetOperatorProductsMSISDN?nsid=1&account=GBP&destinationMSISDN=923325226145 */


            //string fullApiCall = BaseUrl + "/transfertoDirectGetOperatorProductsMSISDN?nsid=1&account=GBP&destinationMSISDN={0}&fromMSISDN={1}";
            string fullApiCall = BaseUrl + "DTOneProducts?fromMSISDN="+ fromMsisdn + "&destinationMSISDN="+Msisdn+"&account=GBP&product=THM";

            try
            {
                using (var client = new HttpClient { BaseAddress = new Uri(fullApiCall), Timeout = TimeSpan.FromSeconds(60) })
                {
            
                    HttpResponseMessage response = await client.GetAsync("");
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    if (!response.IsSuccessStatusCode || response.Content == null)
                        throw new WebException();

                    return await response.Content.ReadAsStringAsync();
                }
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