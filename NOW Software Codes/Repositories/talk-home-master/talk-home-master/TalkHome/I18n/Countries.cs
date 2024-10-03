using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TalkHome.Models;
using System.Linq;

namespace TalkHome.I18n
{
    /// <summary>
    /// Singleton class that creates the list of countries
    /// </summary>
    public sealed class Countries
    {
        private static Countries instance = null;
        private static readonly object padlock = new object();
        public List<I18nCountry> I18nCountries { get; set; }

        Countries()
        {
            GetCountries();
        }

        /// <summary>
        /// Instance of the singleton
        /// </summary>
        public static Countries Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Countries();
                    }
                    return instance;
                }
            }
        }

        /// <summary>
        /// Parses the Json file and return the list of countries
        /// </summary>
        private void GetCountries()
        {
            using (StreamReader r = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "I18n/Countries.json")))
            {
                string json = r.ReadToEnd();
                I18nCountries = JsonConvert.DeserializeObject<List<I18nCountry>>(json).OrderBy(s => s.name.common).ToList();
            }
        }
    }
}
