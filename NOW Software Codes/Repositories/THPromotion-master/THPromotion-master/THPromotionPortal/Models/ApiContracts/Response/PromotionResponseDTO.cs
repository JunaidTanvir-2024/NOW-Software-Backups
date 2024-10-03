using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using THPromotionPortal.Models.enums;

namespace THPromotionPortal.Models.ApiContracts.Response
{
    public class PromotionsAmount
    {
        public decimal Amount { get; set; }
        public List<PromotionResponseDTO> promotions { get; set; }
    }
    public class PromotionResponseDTO
    {
        public int Id { get; set; }
        public string PromotionUniqueName { get; set; }
        public string PromoCode { get; set; }
        public bool IsActive { get; set; }
        public PromotionType TypeId { get; set; }
        public int PromotionValue { get; set; }
        public PromotionValueType PromotionValueTypeId { get; set; }
        public Medium MediumTypeId { get; set; }
        public PurchaseType PurchaseTypeId { get; set; }
        public string Description { get; set; }
        public bool IsPromoCode { get; set; }
        public int? Threshold { get; set; }
        public ThresholdType? ThresholdTypeId { get; set; }
        public AudienceStatusType AudienceStatusTypeId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int CreatedBy { get; set; }
        public DateTime LastUpdatedDateTime { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsArchived { get; set; }
        public List<AudienceDTO> Audiences { get; set; }
        public List<PaymentMethodDTO> PaymentMethods { get; set; }
        public List<ProductDTO> Products { get; set; }
        public List<PromotionDatesDTO> PromotionDates { get; set; }
    }
    public class AudienceDTO
    {
        public int PromotionId { get; set; }
        public int AudienceTypeId { get; set; }
        public string CountryCode { get; set; }
    }
    public class PaymentMethodDTO
    {
        public int PromotionId { get; set; }
        public int PromotionPaymentTypeId { get; set; }
    }
    public class ProductDTO
    {
        public int Id { get; set; }
        public int PromotionId { get; set; }
        public int ProductId { get; set; }
    }
    public class PromotionDatesDTO
    {
        public int Id { get; set; }
        //public ActivationType ActivationTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public int PromotionId { get; set; }
        public DateTime EndDate { get; set; }
    }
}
