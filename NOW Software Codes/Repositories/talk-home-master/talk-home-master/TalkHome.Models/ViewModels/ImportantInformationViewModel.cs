using System.Web;

namespace TalkHome.Models.ViewModels
{
    /// <summary>
    /// Defines the view model for important information blocks
    /// </summary>
    public class ImportantInformationViewModel
    {
        public IHtmlString ImportantInformation { get; set; }

        public IHtmlString ImportantInformationSidebar { get; set; }

        public ImportantInformationViewModel(IHtmlString importantInformation, IHtmlString importantInformationSidebar)
        {
            ImportantInformation = importantInformation;

            ImportantInformationSidebar = importantInformationSidebar;
        }
    }
}
