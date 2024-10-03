using System.Collections.Generic;

namespace TalkHome.Models.WebApi.Payment
{
    /// <summary>
    /// Defines the data structure for a One-click checkout request
    /// </summary>
    public class OneClickCheckoutRequest
    {
        public string UniqueIDValue { get; set; }

        public string CurrencyCode { get; set; }

        public string ChannelType { get; set; }

        public List<string> Basket { get; set; }

        public MethodOfPayment MethodOfPayment { get; set; }

        public OneClickCheckoutRequest(string uniqueIDValue, string currencyCode, string channelType, List<string> basket, MethodOfPayment methodOfPayment)
        {
            UniqueIDValue = uniqueIDValue;

            CurrencyCode = currencyCode;

            ChannelType = channelType;

            Basket = basket;

            MethodOfPayment = methodOfPayment;
        }
    }
}
