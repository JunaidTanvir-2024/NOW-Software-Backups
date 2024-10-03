using System;
using TalkHome.Models.Enums;

namespace TalkHome.Models.WebApi.Payment
{
    /// <summary>
    /// Describes a request to get a customer's payment details.
    /// </summary>
    public class GetCustomerRequestModel
    {
        public string ChannelType { get; set; }

        public string UniqueIDValue { get; set; } // Card or Msisdn number

        public string UniqueIDType { get; set; }

        public GetCustomerRequestModel() { }

        public GetCustomerRequestModel(ChannelType channelType, string uniqueIDValue, UniqueIDType uniqueIDType)
        {
            ChannelType = Enum.GetName(typeof(ChannelType), channelType);

            UniqueIDValue = uniqueIDValue;

            UniqueIDType = Enum.GetName(typeof(UniqueIDType), uniqueIDType);
        }
    }
}
