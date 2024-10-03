using System.Collections.Generic;
using System.Web.Mvc;
using TalkHome.Models.WebApi.Rates;

namespace TalkHome.Models.ViewModels
{
    /// <summary>
    /// Defines the view model for filtering bundles and plans
    /// </summary>
    public class ProductFilterViewModel
    {
        public List<SelectListItem> Filters { get; set; }

        public List<SelectListItem> Countries { get; set; }

        public IList<Rate> Rates { get; set; }

        public ProductFilterViewModel(List<SelectListItem> filters, List<SelectListItem> countries, IList<Rate> rates)
        {
            Filters = filters;

            Countries = countries;

            Rates = rates;
        }
    }
}
