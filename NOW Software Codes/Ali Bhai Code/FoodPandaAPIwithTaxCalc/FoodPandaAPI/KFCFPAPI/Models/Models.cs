using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace KFCFPAPI.Models
{
    public class FPOrder
    {
        public string token { get; set; }

        public string code { get; set; }

        public string expeditionType { get; set; }

        public string shortCode { get; set; }

        public bool preOrder { get; set; }

        public string expiryDate { get; set; }

        public string createdAt { get; set; }

        public string corporateTaxId { get; set; }

        public bool webOrder { get; set; }

        public bool mobileOrder { get; set; }

        public bool corporateOrder { get; set; }

        public bool test { get; set; }

        public string[] vouchers { get; set; }

        public string[] discounts { get; set; }

        public PlatformResturants platformRestaurant { get; set; }

        public LocalInformation localInfo { get; set; }

        public FPCustomer customer { get; set; }

        public FPPayment payment { get; set; }

        public DeliveryAddress delivery { get; set; }

        public Comments comments { get; set; }

        public List<FPItemsList> products { get; set; }

        public FPPrice price { get; set; }

        public OrderUrl callbackUrls { get; set; }
    }


    public class OrderUrl
    {
        public string orderAcceptedUrl { get; set; }

        public string orderRejectedUrl { get; set; }

        public string orderPickedUpUrl { get; set; }

        public string orderPreparedUrl { get; set; }
    }


    public class FPPrice
    {
        public string grandTotal { get; set; }

        public string minimumDeliveryValue { get; set; }

        public string serviceFeeTotal { get; set; }

        public string comission { get; set; }

        public string deliveryFee { get; set; }

        public string containerCharge { get; set; }

        public string deliveryFeeDiscount { get; set; }

        public string serviceFeePercent { get; set; }

        public string serviceTax { get; set; }

        public string serviceTaxValue { get; set; }

        public string subTotal { get; set; }

        public string totalNet { get; set; }

        public string vatVisible { get; set; }

        public string vatPercent { get; set; }

        public string vatTotal { get; set; }

        public string discountAmountTotal { get; set; }

        public string differenceToMinimumDeliveryValue { get; set; }

        public string payRestaurant { get; set; }

        public string collectFromCustomer { get; set; }

        public string riderTip { get; set; }

        public List<DeliveryFees> deliveryFees { get; set; }
    }

    public class DeliveryFees
    {
        public string name { get; set; }

        public string value { get; set; }
    }


    public class FPItemsList
    {
        public string categoryName { get; set; }

        public string name { get; set; }

        public string paidPrice { get; set; }

        public string quantity { get; set; }

        public string remoteCode { get; set; }

        public string unitPrice { get; set; }

        public string comment { get; set; }

        public string id { get; set; }

        public string description { get; set; }

        public string discountAmount { get; set; }

        public bool halfHalf { get; set; }

        public string vatPercentage { get; set; }

        public string selectedChoices { get; set; }

        public ItemsVariations variation { get; set; }

        public List<FPToppings> selectedToppings { get; set; }
    }


    public class FPToppings
    {
        public string name { get; set; }

        public string price { get; set; }

        public string quantity { get; set; }

        public string id { get; set; }

        public string remoteCode { get; set; }

        public string type { get; set; }

        public string[] children { get; set; }
    }


    public class ItemsVariations
    {
        public string name { get; set; }
    }


    public class Comments
    {
        public string customerComment { get; set; }

        public string vendorComment { get; set; }
    }


    public class Address
    {
        public string line1 { get; set; }

        public string line2 { get; set; }

        public string line3 { get; set; }

        public string line4 { get; set; }

        public string line5 { get; set; }

        public string postcode { get; set; }

        public string city { get; set; }

        public string street { get; set; }

        public string number { get; set; }

        public string room { get; set; }

        public string flatNumber { get; set; }

        public string building { get; set; }

        public string intercom { get; set; }

        public string entrance { get; set; }

        public string structure { get; set; }

        public string floor { get; set; }

        public string district { get; set; }

        public string other { get; set; }

        public string company { get; set; }

        public string deliveryMainArea { get; set; }

        public string deliveryMainAreaPostcode { get; set; }

        public string deliveryArea { get; set; }

        public string deliveryAreaPostcode { get; set; }

        public string deliveryInstructions { get; set; }

        public string latitude { get; set; }

        public string longitude { get; set; }
    }


    public class DeliveryAddress
    {
        public bool expressDelivery { get; set; }

        public string expectedDeliveryTime { get; set; }

        public string riderPickupTime { get; set; }

        public string deliveryInstructions { get; set; }

        public Address address { get; set; }
    }

    public class FPPayment
    {
        public string status { get; set; }

        public string type { get; set; }

        public string remoteCode { get; set; }

        public string requiredMoneyChange { get; set; }

        public string vatId { get; set; }

        public string vatName { get; set; }
    }

    public class FPCustomer
    {
        public string id { get; set; }

        public string code { get; set; }

        public string mobilePhoneCountryCode { get; set; }

        public string email { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public string mobilePhone { get; set; }

        public string[] flags { get; set; }
    }

    public class PlatformResturants
    {
        public string id { get; set; }
    }

    public class LocalInformation
    {
        public string platform { get; set; }

        public string platformKey { get; set; }

        public string countryCode { get; set; }

        public string currencySymbol { get; set; }

        public string currencySymbolPosition { get; set; }

        public string currencySymbolSpaces { get; set; }

        public string decimalSeparator { get; set; }

        public string decimalDigits { get; set; }

        public string thousandsSeparator { get; set; }

        public string website { get; set; }

        public string email { get; set; }

        public string phone { get; set; }
    }


    public class AccessToken
    {
        public string access_token { get; set; }

        public string expires_in { get; set; }

        public string scope { get; set; }

        public string token_type { get; set; }
    }

    public class FailureResponse
    {
        public string reason { get; set; }

        public string message { get; set; }
    }

    public class FPOrderItemsDetails
    {
        public string BarCode { get; set; }

        public int Quantity { get; set; }

        public int GrossPrice { get; set; }
        public string Intruction { get; set; }
    }

    public class SuccessResponse
    {
        public RemoteOrderId remoteResponse { get; set; }

        public SuccessResponse()
        {
            remoteResponse = new RemoteOrderId();
        }
    }

    public class RemoteOrderId
    {
        public string remoteOrderId { get; set; }
    }

    public class CancelOrderPayload
    {
        public string status { get; set; }

        public string message { get; set; }
    }

    public class ProductItemCode
    {
        public string BarCode { get; set; }
        public string GrossPrice { get; set; }
        public string Quantity { get; set; }
    }

    public class FPCancelationResponse
    {
        public string Reason { get; set; }
        public string Message { get; set; }

    }


    [DataContract]
    public class selectedMenuItemList2
    {
        [DataMember]
        public string MenuFamilyID { get; set; }
        [DataMember]
        public string MenuItemID { get; set; }
        [DataMember]
        public string MenuItemText { get; set; }
        [DataMember]
        public string MenuFamilyMeasureID { get; set; }
        [DataMember]
        public string MenuFamilyMeasureText { get; set; }
        [DataMember]
        public string Quantity { get; set; }
        [DataMember]
        public string IsWithCheze { get; set; }
        [DataMember]
        public string IsFrenchFriesUpSize { get; set; }
        [DataMember]
        public string IsDrinkUpsize { get; set; }
        [DataMember]
        public string GrossPrice { get; set; }
        [DataMember]
        public string DeliveryOrderDetailID { get; set; }
        [DataMember]
        public string barcode { get; set; }

        [DataMember]
        public long ItemTotalAmountforTax { get; set; }

        [DataMember]
        public long ItemAmountExcofTax { get; set; }

        [DataMember]
        public long ItemTaxRate { get; set; }

        [DataMember]
        public long ItemTaxCharged { get; set; }



    }

    public class ItemforFBR
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public double Quantity { get; set; }
        public string PCTCode { get; set; }
        public double TaxRate { get; set; }
        public double SaleValue { get; set; }
        public double TotalAmount { get; set; }
        public double TaxCharged { get; set; }
        public double Discount { get; set; }
        public double FurtherTax { get; set; }
        public int InvoiceType { get; set; }
        public object RefUSIN { get; set; }
    }

    public class Root
    {
        public string InvoiceNumber { get; set; }
        public int POSID { get; set; }
        public string USIN { get; set; }
        public string DateTime { get; set; }
        public string BuyerNTN { get; set; }
        public string BuyerCNIC { get; set; }
        public string BuyerName { get; set; }
        public string BuyerPhoneNumber { get; set; }
        public List<ItemforFBR> items { get; set; }
        public double TotalBillAmount { get; set; }
        public int TotalQuantity { get; set; }
        public double TotalSaleValue { get; set; }
        public double TotalTaxCharged { get; set; }
        public int PaymentMode { get; set; }
        public int InvoiceType { get; set; }
        public object RefUSIN { get; set; }

        //public Root()
        //{

        //    this.items = new List<Item>();
        //}
    }
}