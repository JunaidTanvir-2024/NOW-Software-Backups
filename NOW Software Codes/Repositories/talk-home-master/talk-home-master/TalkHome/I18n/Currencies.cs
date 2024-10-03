using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TalkHome.Models;

namespace TalkHome.I18n
{
    /// <summary>
    /// Singleton class that creates the list of countries with respective currencies
    /// </summary>
    public sealed class Currencies
    {
        private static Currencies instance = null;
        private static readonly object padlock = new object();
        public Dictionary<string, I18nCurrencyDetails> I18nCurrencies { get; set; }

        Currencies()
        {
            GetCurrencies();
        }

        /// <summary>
        /// Instance of the singleton
        /// </summary>
        public static Currencies Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Currencies();
                    }
                    return instance;
                }
            }
        }

        /// <summary>
        /// Parses the Json file and return the list of countries
        /// </summary>
        private void GetCurrencies()
        {
            using (StreamReader r = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "I18n/Currencies.json")))
            {
                string json = r.ReadToEnd();
                I18nCurrencies = JsonConvert.DeserializeObject<Dictionary<string, I18nCurrencyDetails>>(json);
            }
        }
    }
}