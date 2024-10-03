using System.Collections.Generic;
using Umbraco.Web.PublishedContentModels;

namespace TalkHome.Models.ViewModels.TopUp
{
    /// <summary>
    /// Defines the view model for checkout and top up pages.
    /// </summary>
    public class TopUpPageViewModel
    {
        public string ProductCode { get; set; }

        public JWTPayload Payload { get; set; }

        public List<Product> BasketSummary { get; set; }

        public TopUpPageViewModel(JWTPayload payload, List<Product> basketSummary)
        {
            Payload = payload;

            BasketSummary = basketSummary;
        }

        public TopUpPageViewModel(string productCode, JWTPayload payload, List<Product> basketSummary)
        {
            ProductCode = productCode;

            Payload = payload;

            BasketSummary = basketSummary;
        }
    }
}
