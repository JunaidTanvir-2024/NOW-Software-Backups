namespace TalkHome.Models.WebApi.CallingCards
{
    public class Minutes
    {
        public string localaccess { get; set; }

        public string access0207 { get; set; }

        public string access0800 { get; set; }

        public Minutes(string localaccess, string access0207, string access0800)
        {
            if (localaccess != null)
                this.localaccess = localaccess.Replace(".00", "");

            if (access0207 != null)
                this.access0207 = access0207.Replace(".00", "");

            if (access0800 != null)
                this.access0800 = access0800.Replace(".00", "");
        }
    }
}
