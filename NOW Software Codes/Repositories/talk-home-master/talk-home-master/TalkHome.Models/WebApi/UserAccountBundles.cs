using System;
using TalkHome.Models.Enums;

namespace TalkHome.Models.WebApi
{
    /// <summary>
    /// Represents records for the active plans or bundles
    /// </summary>
    public class UserAccountBundles
    {
      
        public string bundlename { get; set; }

        public string remainingminutes { get; set; }

        public string bundleminutes { get; set; }

        public string remaininingdata { get; set; }

        public string remainingtext { get; set; }

        public string bundletext { get; set; }

        public string bundledata { get; set; }

        public string bundleguid { get; set; }

        public bool isautorenew { get; set; }

        public string expirydate { get; set; }

        public string expiresindays { get; set; }

        public ExpiryAlert expiryalert { get; set; }


        public UserAccountBundles(string bundlename, string remainingminutes, string bundleminutes, string remainingtext, string bundletext, string bundledata, string bundleguid, string expirydate, string remaininingdata)
        {
            if (bundlename != null)
                this.bundlename = bundlename;

            if (remainingminutes != null)
                this.remainingminutes = remainingminutes.ToLower().Replace(" minutes", "");

            if (bundleminutes != null)
                this.bundleminutes= bundleminutes.ToLower().Replace(" minutes", "");

            if (remainingtext != null)
                this.remainingtext = remainingtext.ToLower().Replace(" texts", "");

            if (bundletext != null)
                this.bundletext = bundletext.ToLower().Replace(" text", "");

            if (bundledata != null && bundledata.ToLower().Equals("0 bytes"))
                this.bundledata = null;
            else
                this.bundledata = bundledata;

            if (remaininingdata != null)
                this.remaininingdata = remaininingdata.ToLower().Replace(" data", "");

            if (remaininingdata != null)
                this.remaininingdata = remaininingdata.ToLower().Replace(" data", "");

            this.bundleguid = bundleguid;

            if (expirydate != null)
            {
                DateTime expiryDate = DateTime.Parse(expirydate);
                TimeSpan t = expiryDate - DateTime.Now;

                if (t.TotalDays < 0)
                {
                    expiryalert = ExpiryAlert.Expired;
                }
                else if (t.TotalDays < 2)
                {
                    expiryalert = ExpiryAlert.Tomorrow;
                }
                else if (t.TotalDays < 4)
                {
                    expiryalert = ExpiryAlert.ThreeDays;
                }
                else if (t.TotalDays < 6)
                {
                    expiryalert = ExpiryAlert.FiveDays;
                }
                else
                {
                    expiryalert = ExpiryAlert.Normal;
                }

                expiresindays = String.Format("{0:N0}", t.TotalDays);
            }
        }
    }
}
