using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.WebApi
{
    public class AddCompitionUserRequestModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool signme { get; set; }
        public string Promoname { get; set; }

    }
}
