using System.ComponentModel.DataAnnotations;

namespace TalkHome.Models.Enums
{
    /// <summary>
    /// Defines display names for product codes
    /// </summary>
    public enum ReadableProductCodes
    {
        [Display(Name = "Talk Home Mobile")]
        THM = 1,

        [Display(Name = "Talk Home App")]
        THA = 2,

        [Display(Name = "Calling Card")]
        THCC = 3
    }
}
