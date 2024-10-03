using System.ComponentModel.DataAnnotations;

namespace TalkHome.Models.Enums
{
    /// <summary>
    /// Defines product descriptions based on product codes
    /// </summary>
    public enum ProductNames
    {
        [Display(Name = "Plan")]
        THM = 1,

        [Display(Name = "Bundle")]
        THA = 2,

        [Display(Name = "PIN")]
        THCC = 3
    }
}
