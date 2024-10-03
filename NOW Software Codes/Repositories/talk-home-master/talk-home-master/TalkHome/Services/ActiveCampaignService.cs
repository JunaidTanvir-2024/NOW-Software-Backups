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
using Newtonsoft.Json;


namespace TalkHome.Services
{
    public enum SubscriptionStatus
    {
         Active = 1, 
         Unsubscribed = 2
    }

    public class ActiveCampaignService : IActiveCampaignService
    {

        /*
        <add key = "ACBaseUrl" value="https://nowtel.api-us1.com/admin/api.php?api_action=" />
        <add key = "ACApiKey" value="47f84f5c5acb755dfad857281652bf88260d3ee1a51a534afd728871b1d8b12af92e743d" />
        <add key = "ACApiOutput" value="json" />
        */

        private readonly string BaseUrl;
        private readonly string APIKey;
        private readonly string Output;
        private readonly ILoggerService LoggerService;



        public ActiveCampaignService(ILoggerService loggerService)
            {
                BaseUrl = ConfigurationManager.AppSettings["ACBaseUrl"];
                APIKey = ConfigurationManager.AppSettings["ACApiKey"];
                Output = ConfigurationManager.AppSettings["ACApiOutput"];
            LoggerService = loggerService;
            }
        
        public async Task<ActiveCampaignResult> AddTag(string tag, string emailAddress)
        {
            string action = "contact_tag_add";

            var values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("email", emailAddress ),
                    new KeyValuePair<string, string>("tags",tag ),
                    new KeyValuePair<string, string>("api_output","json")
            };

            
            return await Post(action, new FormUrlEncodedContent(values));
        }


        public async Task<ActiveCampaignResult> RemoveTag(string tag, string emailAddress)
        {
            string action = "contact_tag_remove";

            var values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("email", emailAddress ),
                    new KeyValuePair<string, string>("tags",tag ),
                    new KeyValuePair<string, string>("api_output","json")
            };


            return await Post(action, new FormUrlEncodedContent(values));
        }

        public async Task<ActiveCampaignResult> AddToList(string listId, string emailAddress, string firstName, string lastName)
        {
            string action = "contact_sync";
            
            var values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("email", emailAddress ),
                    new KeyValuePair<string, string>("first_name", firstName ),
                    new KeyValuePair<string, string>("last_name", lastName ),
                    new KeyValuePair<string, string>("p[" + listId + "]", listId ),
                    new KeyValuePair<string, string>("api_output","json")
            };


            return await Post(action, new FormUrlEncodedContent(values));
        }


        public async Task<ActiveCampaignResult> DeleteFromList(string listId, string emailAddress, string firstName, string lastName)
        {
            string action = "contact_sync";

            var values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("email", emailAddress ),
                    new KeyValuePair<string, string>("first_name", firstName ),
                    new KeyValuePair<string, string>("last_name", lastName ),
                    new KeyValuePair<string, string>("p[" + listId + "]", listId ),
                    new KeyValuePair<string, string>("api_output","json")
            };


            return await Post(action, new FormUrlEncodedContent(values));
        }

        public async Task<ActiveCampaignResult> GetContact(string emailAddress)
        {
            string action = "contact_sync";

            var values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("email", emailAddress ),
                    new KeyValuePair<string, string>("api_output","json")
            };

            return await Post(action, new FormUrlEncodedContent(values));
        }


        public async Task<ActiveCampaignResult> DeleteContact(string emailAddress)
        {
            string action = "contact_view_email";

            var values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("email", emailAddress),                    
                    new KeyValuePair<string, string>("api_output","json")


            };

            var response = await Post(action, new FormUrlEncodedContent(values));

            if (response.result_code == 1)
            {
                values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("id", response.id),
                    new KeyValuePair<string, string>("api_output","json")
                };
                return await Post("contact_delete", new FormUrlEncodedContent(values));
            }
            else
            {
                response.result_code = 0;
                return response;
            }
        }

        public async Task<ActiveCampaignResult> ChangeSubscriptionStatus(string listId, string emailAddress, string id, string status)
        {
            string action = "contact_edit";

            var values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("id", id ),
                    new KeyValuePair<string, string>("email", emailAddress ),
                    new KeyValuePair<string, string>("p[" + listId + "]", listId ),
                    new KeyValuePair<string, string>("status[" + listId + "]", status ),
                    new KeyValuePair<string, string>("api_output","json")
            };


            return await Post(action, new FormUrlEncodedContent(values));
        }


        /*
        api_action* contact_view_email
        api_key* Your API key
        api_output xml, json, or serialize(default is XML)
        email* Email address of the contact you are looking up.Example: 'test@example.com'
        contact_view_email
        */

        public async Task<ActiveCampaignResult> GetContactDetails(string emailAddress)
        {
            string action = "contact_view_email";

            var values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("email", emailAddress ),
                    new KeyValuePair<string, string>("api_output","json")
            };


            return await Post(action, new FormUrlEncodedContent(values));
        }


        public async Task<ActiveCampaignResult> Post(string action,FormUrlEncodedContent f)
        {

            string apiUrl = String.Format("{0}{1}&api_key={2}",BaseUrl,action,APIKey);

            try
            {
                using (var client = new HttpClient {Timeout = TimeSpan.FromSeconds(60) })
                {

                    var response = await client.PostAsync(apiUrl,f);

                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    if (!response.IsSuccessStatusCode || response.Content == null)
                        throw new WebException();

                    string content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ActiveCampaignResult>(content);

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