using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using THPromotionPortal.Models.ViewModel;

namespace THPromotionPortal.Services.Interfaces
{
    public interface IPromotionService
    {
        public Task<dynamic> CreatePromotion(CreatePromotionViewModel model);
        public Task<dynamic> GetAllPromotion();
        public Task<dynamic> GetPromotion(int id);
    }
}
