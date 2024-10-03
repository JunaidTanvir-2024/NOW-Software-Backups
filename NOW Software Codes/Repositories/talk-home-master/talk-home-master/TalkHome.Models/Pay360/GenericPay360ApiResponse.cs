namespace TalkHome.Models.Pay360
{
    public class GenericPay360ApiResponse <T>
    {
        public string status { get; set; }

        public string message { get; set; }

        public T payload { get; set; }

        public int errorCode { get; set; }
    }
}





