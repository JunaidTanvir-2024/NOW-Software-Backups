using Foolproof;
using System.ComponentModel.DataAnnotations;

namespace TalkHome.Models.ViewModels
{
    public class BaksetViewModel
    {
        [Required]
        public int Id { get; set; }

        [NotEqualTo("Id")]
        public int? newId { get; set; }
    }
}
