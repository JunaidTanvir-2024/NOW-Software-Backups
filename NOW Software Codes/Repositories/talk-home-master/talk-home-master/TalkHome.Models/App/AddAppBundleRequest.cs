using System.ComponentModel.DataAnnotations;

namespace TalkHome.Models.App
{
    /// <summary>
    /// Defines the model for checking out an in-app bundle request.
    /// </summary>
    public class AddBundleRequestModel
    {
        [Required]
        public string Msisdn { get; set; }

        [Required]
        public string Guid { get; set; }
    }
}
