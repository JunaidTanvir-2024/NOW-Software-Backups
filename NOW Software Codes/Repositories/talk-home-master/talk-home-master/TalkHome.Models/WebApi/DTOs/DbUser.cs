using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.WebApi.DTOs
{
    public class DbUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public byte[] PasswordPrefix { get; set; }
        public byte[] PasswordSuffix { get; set; }
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string ContactNo { get; set; }
        public string MobileNo { get; set; }
        public bool IsSubscribedToNewsletter { get; set; }
        public int? SubscriberId { get; set; }
        public int? ClientID { get; set; }
        public int? ReturnCode { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsConfirmedUser { get; set; }
        public string ConfirmationToken { get; set; }
        public SignUpTypeId SignUpTypeId { get; set; }
        public bool IsActive { get; set; }
    }
    public enum SignUpTypeId
    {
        UsernameAndPassword = 1,
        Google = 2,
        Facebook = 3,
        Twitter = 4
    }
}
