using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TalkHome.Models
{
    public class ActiveCampaignResult
    {
        public int result_code { get; set; }
        public string listslist { get; set; }
        public string result_message { get; set; }
        public string id { get; set; }
        private JObject list;
        public JObject lists
        {
            get
            {
                return list;
            }

            set
            {
                list = JObject.Parse(value.ToString());
            }
        }
    }
}


