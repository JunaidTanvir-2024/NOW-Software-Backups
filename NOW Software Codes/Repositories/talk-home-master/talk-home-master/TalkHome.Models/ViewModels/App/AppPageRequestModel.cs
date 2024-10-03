using System.ComponentModel.DataAnnotations;

namespace TalkHome.Models.ViewModels.App
{
    public class TopUpRequestModel
    {
        [Required]
        public string Msisdn { get; set; }
    }
}
