using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkHome.Models;

namespace TalkHome.Models.ViewModels.BusinessIntelligence
{
    public class TransferPromotionsViewModel
    {
        public TransferPromotionItems AvailablePromotions { get; set; }
        public string TopCountry { get; set; }
        public IEnumerable<string> Countries { get; set; }

        public TransferPromotionsViewModel(TransferPromotionItems transferPromotion, IEnumerable<string> countries,string topCountry)
        {
            AvailablePromotions = transferPromotion;
            Countries = countries;
            TopCountry = topCountry;
        }
    }


}
