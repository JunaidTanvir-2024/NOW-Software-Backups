namespace TalkHome.Models.WebApi.CallingCards
{
    public class MinutesRecord
    {
        public string iso_code { get; set; }

        public string destination { get; set; }

        public Minutes landline { get; set; }

        public Minutes mobile { get; set; }
    }
}
