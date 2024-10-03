namespace TalkHome.Models.WebApi.Rates
{
    /// <summary>
    /// Describes the model for UK rates.
    /// </summary>
    public class UKNationalRate
    {
        public string content_header { get; set; }

        public string content { get; set; }

        public string description { get; set; }

        public UKNationalRate(string content_header, string content, string description)
        {
            this.content_header = content_header;

            this.content = content;

            this.description = description;
        }
    }
}
