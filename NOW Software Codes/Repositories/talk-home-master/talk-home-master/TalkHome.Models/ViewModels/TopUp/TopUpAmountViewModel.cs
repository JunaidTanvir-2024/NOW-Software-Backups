using System.Collections.Generic;
using Umbraco.Core.Models;

namespace TalkHome.Models.ViewModels.TopUp
{
    /// <summary>
    /// Defines the model to display the top ups for a product code
    /// </summary>
    public class TopUpAmountViewModel
    {
        public string ProductCode { get; set; }

        public IEnumerable<IPublishedContent> TopUps { get; set; }

        public JWTPayload Payload { get; set; }

        public TopUpAmountViewModel(string productCode, IEnumerable<IPublishedContent> topUps, JWTPayload payload)
        {
            ProductCode = productCode;

            TopUps = topUps;

            Payload = payload;
        }
    }
}
