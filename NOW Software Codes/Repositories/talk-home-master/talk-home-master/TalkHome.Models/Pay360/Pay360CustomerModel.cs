using System.Collections.Generic;
//
namespace TalkHome.Models.Pay360
{
    /*
    {
    "message": "Cutomer details retrieved successfully",
    "status": "Success",
    "errorCode": 0,
    "payload": {
        "displayName": "Ali",
        "merchantRef": "923465392222",
        "pay360CustId": 2072830,
        "email": null,
        "defaultCurrency": null,
        "dob": null,
        "addressLine1": null,
        "addressLine2": null,
        "addressLine3": null,
        "addressLine4": null,
        "city": null,
        "region": null,
        "postCode": null,
        "country": null,
        "countryCode": null,
        "telephone": null
        }
    }
    */

    public class Pay360CustomerModel
    {
 
    public string displayName { get; set; }
    public string merchantRef { get; set; }
    public string pay360CustId { get; set; }
    public string email { get; set; }
    public string defaultCurrency { get; set; }
    public string dob { get; set; }
    public string addressLine1 { get; set; }
    public string addressLine2 { get; set; }
    public string addressLine3 { get; set; }
    public string addressLine4 { get; set; }
    public string city { get; set; }
    public string region { get; set; }
    public string postCode { get; set; }
    public string country { get; set; }
    public string countryCode { get; set; }
    public string telephone { get; set; }
    public List<paymentMethodResponse> PaymentMethods { get; set; }
    }
}