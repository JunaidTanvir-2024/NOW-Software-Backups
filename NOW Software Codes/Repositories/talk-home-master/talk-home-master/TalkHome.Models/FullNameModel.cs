using System.ComponentModel.DataAnnotations;
using TalkHome.Models.Validation;

namespace TalkHome.Models
{
    /// <summary>
    /// Describes the full name model with email
    /// </summary>
    public class FullNameModel
    {
        public string Salutation { get; set; }

        [CustomValidation(typeof(RequestValidations), "FirstName")]
        public string FirstName { get; set; }

        [CustomValidation(typeof(RequestValidations), "LastName")]
        public string LastName { get; set; }

        [CustomValidation(typeof(RequestValidations), "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public FullNameModel() { }

        public FullNameModel(string salutation, string firstName, string lastName, string email)
        {
            Salutation = salutation;

            FirstName = firstName;

            LastName = lastName;

            Email = email;
        }

        /// <summary>
        /// Provides ToString() override for display purposes
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join(" ",
                FirstName,
                LastName);
        }
    }
}
