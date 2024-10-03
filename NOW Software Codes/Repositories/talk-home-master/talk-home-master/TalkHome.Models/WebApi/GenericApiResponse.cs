namespace TalkHome.Models.WebApi
{
    public class GenericApiResponse<T>
    {
        public int status { get; set; }

        public string message { get; set; }

        public T payload { get; set; }

        public int errorCode { get; set; }
    }
}
