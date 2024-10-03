using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models
{
    public  class BillingCountries
    {
        public BillingCountries(string code, string name,int id)
        {
            Id = id;
            Name = name;
            ISO_Code = code;
            
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ISO_Code { get; set; }
    }
}
