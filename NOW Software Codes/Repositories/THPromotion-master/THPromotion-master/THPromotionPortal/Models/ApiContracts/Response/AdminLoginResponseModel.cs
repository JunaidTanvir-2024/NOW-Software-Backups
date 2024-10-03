
using THPromotionPortal.Models.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Contracts.Response
{
    public class AdminLoginResponseModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpirationDateTime { get; set; }
        public List<MenuResponse> MenuJson { get; set; }
        public List<ProductResponse> ProductJson { get; set; }
        public List<RoleResponse>  RoleResponse { get; set; }
    }
    public class MenuResponse
    {
        public int Id { get; set; }
        public string MenuName { get; set; }
        public string MenuId { get; set; }
        public string MenuUrl { get; set; }
        public string IconClass { get; set; }
    }
    public class ProductResponse
    {
        public string Product { get; set; }
    }
    public class RoleResponse
    {
        public Roles RoleId { get; set; }
    }
}
