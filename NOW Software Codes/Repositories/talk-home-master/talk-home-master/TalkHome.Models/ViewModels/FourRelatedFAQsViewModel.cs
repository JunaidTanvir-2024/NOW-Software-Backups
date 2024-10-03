using Umbraco.Web.PublishedContentModels;

namespace TalkHome.Models.ViewModels
{
    /// <summary>
    /// Defines the view model for Umbraco Custom page content type realated FAQs
    /// </summary>
    public class FourRelatedFAQsViewModel
    {
        public FAQ FAQ1 { get; set; }

        public FAQ FAQ2 { get; set; }

        public FAQ FAQ3 { get; set; }

        public FAQ FAQ4 { get; set; }

        public FourRelatedFAQsViewModel(FAQ fAQ1, FAQ fAQ2, FAQ fAQ3, FAQ fAQ4)
        {
            FAQ1 = fAQ1;

            FAQ2 = fAQ2;

            FAQ3 = fAQ3;

            FAQ4 = fAQ4;
        }
    }
}
