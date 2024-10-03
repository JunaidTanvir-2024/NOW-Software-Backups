using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using THPromotionPortal.Models.enums;

namespace THPromotionPortal.Models.ViewModel
{
    public class CreatePromotionViewModel
    {
        public CreatePromotionViewModel()
        {
            if (IsPromoCode)
            {
                if (string.IsNullOrEmpty(PromoCode))
                    throw new ValidationException("PromoCode is Required");
            }
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Promotion Name is Required")]
        public string PromotionUniqueName { get; set; }

        [MaxLength(100, ErrorMessage = "PromoCode length must be less than 100 characters")]
        public string PromoCode { get; set; }

        [Required(ErrorMessage = "Start Date is Required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is Required")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "PromotionType is Required")]
        public PromotionType TypeId { get; set; }

        [Required(ErrorMessage = "Promotion Value is Required")]
        public decimal PromotionValue { get; set; }

        [Required(ErrorMessage = "PaymentType Value is Required")]
        public PromotionValueType PromotionValueTypeId { get; set; }

        [Required(ErrorMessage = "PromotionMedium is Required")]
        public Medium MediumTypeId { get; set; }

        [Required(ErrorMessage = "PurchaseType Value is Required")]
        public PurchaseType PurchaseTypeId { get; set; }

        public string Description { get; set; }

        public bool IsPromoCode { get; set; }

        public decimal Threshold { get; set; }

        public ThresholdType ThresholdTypeId { get; set; }

        [Required(ErrorMessage = "Audience Status Type is Required")]
        public AudienceStatusType AudienceStatusTypeId { get; set; }
        public List<PromotionAudienceRequestModel> PromotionAudience { get; set; }
        public List<PromotionPaymentRequestModel> PromotionPayment { get; set; }
        public List<PromotionProductRequestModel> PromotionProduct { get; set; }
        public List<PromotionDate> PromotionDate { get; set; }
    }

    public class PromotionAudienceRequestModel
    {
        [Required(ErrorMessage = "PromotionAudienceStatus Type is Required")]
        public AudienceType AudienceTypeId { get; set; }

        [Required(ErrorMessage = "CountryCode is Required")]
        public string CountryCode { get; set; }
    }

    public class PromotionPaymentRequestModel
    {
        [Required(ErrorMessage = "PaymentType Value is Required")]
        public PromotionValueType PromotionPaymentTypeId { get; set; }
    }

    public class PromotionProductRequestModel
    {
        public Product PromotionProduct { get; set; }
    }

    public class PromotionDate
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
