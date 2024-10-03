namespace TalkHome.Models.ViewModels
{
    /// <summary>
    /// Defines a container for SEO tags content
    /// </summary>
    public class SEOTags
    {
        public string MetaTitle { get; set; }

        public string MetaTags { get; set; }

        public string MetaDescription { get; set; }

        public string OGDescription { get; set; }

        public string OGImage { get; set; }

        public string Url { get; set; }

        public SEOTags() { }

        public SEOTags(string metaTitle, string metaTags, string metaDescription, string ogDescription, string ogImage, string url)
        {
            MetaTitle = metaTitle;

            MetaTags = metaTags;

            MetaDescription = metaDescription;

            OGDescription = ogDescription;

            OGImage = ogImage;

            Url = url;
        }
    }
}
