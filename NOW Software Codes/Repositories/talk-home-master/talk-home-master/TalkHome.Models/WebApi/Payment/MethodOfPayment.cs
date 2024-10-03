namespace TalkHome.Models.WebApi.Payment
{
    /// <summary>
    /// Defines a summary of the payment method for One click checkouts
    /// </summary>
    public class MethodOfPayment
    {
        public string TypeOfMethodOfPayment { get; set; }

        public string MethodOfPaymentIdentifier { get; set; }

        public MethodOfPayment(string typeOfMethodOfPayment, string methodOfPaymentIdentifier)
        {
            TypeOfMethodOfPayment = typeOfMethodOfPayment;

            MethodOfPaymentIdentifier = methodOfPaymentIdentifier;
        }
    }
}
