using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkHome.Models.Validation;

namespace TalkHome.Models.ViewModels
{

    /// <summary>
    /// Defines the request model for a bundle/plan purchase request
    /// </summary>
    public class UpdatePasswordModel
    {

        public string CurrentPassword { get; set; }
        [CustomValidation(typeof(RequestValidations), "Password")]
        public string Password { get; set; }
        public string RepeatPassword { get; set; }
        public string ResetEmail { get; set; }
        public string ProductCode { get; set; }
    }

    
    
}
