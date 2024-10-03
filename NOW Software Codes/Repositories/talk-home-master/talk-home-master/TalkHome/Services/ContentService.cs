using Umbraco.Web;
using Umbraco.Web.PublishedContentModels;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using TalkHome.Models.Enums;
using System;
using TalkHome.Interfaces;

namespace TalkHome.Services
{
    /// <summary>
    /// Provides functionalities for Umbraco content operations.
    /// </summary>
    public class ContentService : IContentService
    {
        private Properties.ContentIds ContentIds = Properties.ContentIds.Default;

        /// <summary>
        /// Returns content by page Id
        /// </summary>
        /// <param name="id">The page id</param>
        /// <returns>the content</returns>
        public IPublishedContent GetGenericContentById(int id)
        {
            return new UmbracoHelper(UmbracoContext.Current).TypedContent(id);
        }

        /// <summary>
        /// Finds a Campaign Block among a collection of pages.
        /// </summary>
        /// <param name="pages">The collection</param>
        /// <returns>The page or null</returns>
        public CampaignBlock GetCampaign(IEnumerable<IPublishedContent> pages)
        {
            foreach (var page in pages)
                if (page.GetType() == typeof(CampaignBlock))
                    return page.OfType<CampaignBlock>();

            return null;
        }



        /// <summary>
        /// Finds a Campaign Block among a collection of pages.
        /// </summary>
        /// <param name="pages">The collection</param>
        /// <returns>The page or null</returns>
        public TestNestedDoc GetFold(IEnumerable<IPublishedContent> pages)
        {
            foreach (var page in pages)
                if (page.GetType() == typeof(TestNestedDoc))
                    return page.OfType<TestNestedDoc>();

            return null;
        }

        /// <summary>
        /// Finds a list of Offers among a collection of pages.
        /// </summary>
        /// <param name="pages">The collection</param>
        /// <returns>The list or empty</returns>
        public List<IPublishedContent> GetOffers(IEnumerable<IPublishedContent> pages)
        {
            var Result = new List<IPublishedContent>();

            foreach (var page in pages)
                if (page.GetType() == typeof(Offer))
                    Result.Add(page);

            return Result;
        }

        /// <summary>
        /// Finds a list of FAQs among a collection of pages.
        /// </summary>
        /// <param name="pages">The collection</param>
        /// <returns>The list or empty</returns>
        public List<IPublishedContent> GetFaqs(IEnumerable<IPublishedContent> pages)
        {
            var Result = new List<IPublishedContent>();

            foreach (var page in pages)
                if (page.GetType() == typeof(FAQ))
                    Result.Add(page);

            return Result;
        }

        /// <summary>
        /// Finds a mamimum number of children Products given a root page by Id.
        /// </summary>
        /// <param name="id">The root page id</param>
        /// /// <param name="id">Maximum results</param>
        /// <returns>The collection or null</returns>
        public IEnumerable<IPublishedContent> GetTopProducts(int id, int max)
        {
            var Root = new UmbracoHelper(UmbracoContext.Current).TypedContent(id);

            var Parent = Root.Children.Where(x => x.DocumentTypeAlias == "plansBundles").OfType<PlansBundles>().First();

            var Collection = Parent.DescendantsOrSelf<Product>().Where(x => x.ProductTopProduct == true).Take(max);

            if (!Collection.Any())
                return null;

            return Collection;
        }

        /// <summary>
        /// Finds a mamimum number of children Top ups given a root page by Id.
        /// </summary>
        /// <param name="id">The root page id</param>
        /// /// <param name="id">Maximum results</param>
        /// <returns>The collection or null</returns>
        public IEnumerable<IPublishedContent> GetTopUps(int id, int max = 0)
        {
            IEnumerable<Product> Collection;
            var Root = new UmbracoHelper(UmbracoContext.Current).TypedContent(id);

            var Parent = Root.Children.Where(x => x.DocumentTypeAlias == "topUp").OfType<TopUp>().First();

            if (max == 0)
                Collection = Parent.DescendantsOrSelf<Product>();
            else
                Collection = Parent.DescendantsOrSelf<Product>().Take(max);

            if (!Collection.Any())
                return null;

            return Collection;
        }

        /// <summary>
        /// Gets all top up content under Talk home App Top ups root.
        /// </summary>
        /// <returns>The collection or null</returns>
        public IEnumerable<IPublishedContent> GetAppTopUps()
        {
            return new UmbracoHelper(UmbracoContext.Current).TypedContent(ContentIds.TalkHomeAppTopUps).Children;
        }

        /// <summary>
        /// Gets the Talk Home Mobile Plans that don't have the UK as country
        /// </summary>
        /// <returns>The collection</returns>
        public IEnumerable<IPublishedContent> GetTalkHomeMobilePlans()
        {
            var Collection = new UmbracoHelper(UmbracoContext.Current).TypedContent(ContentIds.TalkHomeMobilePlans).Children;

            return Collection;
        }

        /// <summary>
        /// Gets the Talk Home App bundles
        /// </summary>
        /// <returns>The collection</returns>
        public IEnumerable<IPublishedContent> GetBundles()
        {
            return new UmbracoHelper(UmbracoContext.Current).TypedContent(ContentIds.TalkHomeAppBundles).Children;
        }

        /// <summary>
        /// Returns An App bundle given a Guid
        /// </summary>
        /// <param name="guid">The DigiTalk GUID of the bundle</param>
        /// <returns>The bundle or null</returns>
        public Product GetAppBundleByGuid(string guid)
        {
            var Root = new UmbracoHelper(UmbracoContext.Current).TypedContent(ContentIds.TalkHomeAppBundles);

            var Collection = Root.Children.OfType<Product>().Where(x => x.ProductUuid.Equals(guid, StringComparison.CurrentCultureIgnoreCase));

            if (!Collection.Any())
                return null;

            return Collection.First();
        }

        /// <summary>
        /// Returns An Plan bundle given a Guid
        /// </summary>
        /// <param name="guid">The DigiTalk GUID of the bundle</param>
        /// <returns>The bundle or null</returns>
        public Product GetMobilePlanByGuid(string guid)
        {
            var Root = new UmbracoHelper(UmbracoContext.Current).TypedContent(ContentIds.TalkHomeMobilePlans);

            var Collection = Root.Children.OfType<Product>().Where(x => x.ProductUuid.Trim().Equals(guid, StringComparison.CurrentCultureIgnoreCase));

            if (!Collection.Any())
                return null;

            return Collection.First();
        }



        /// <summary>
        /// Returns An App bundle Guid
        /// </summary>
        /// <param name="id">product id</param>
        /// <returns>The bundle guid</returns>
        public string GetAppBundleGuid(int id)
        {
            var Root = new UmbracoHelper(UmbracoContext.Current).TypedContent(ContentIds.TalkHomeAppBundles);

            var Collection = Root.Children.OfType<Product>().Where(x => x.Id.Equals(id));

            if (!Collection.Any())
                return null;

            return Collection.First().ProductUuid.ToString();
        }


        /// <summary>
        /// Returns An Mobile bundle Guid
        /// </summary>
        /// <param name="id">product id</param>
        /// <returns>The bundle guid</returns>
        public string GetMobileBundleGuid(int id)
        {
            var Root = new UmbracoHelper(UmbracoContext.Current).TypedContent(ContentIds.TalkHomeMobilePlans);

            var Collection = Root.Children.OfType<Product>().Where(x => x.Id.Equals(id));

            if (!Collection.Any())
                return null;

            return Collection.First().ProductUuid.ToString();
        }



        /// <summary>
        /// Gets a product by Id.
        /// </summary>
        /// <param name="id">The Id of the product.</param>
        /// <returns>The product or null if a failure occurred.</returns>
        public Product GetProducts(int id)
        {
            return new UmbracoHelper(UmbracoContext.Current).TypedContent(id).OfType<Product>();
        }

        /// <summary>
        /// Returns a list of products given a list of ids.
        /// </summary>
        /// <param name="ids">A list of product ids.</param>
        /// <returns>The list of products or null in case the id list was empty.</returns>
        public List<Product> GetProducts(HashSet<int> ids)
        {
            List<Product> Products = new List<Product>();

            if (!ids.Any())
                return Products; // Return an empty list

            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);        

            foreach (int id in ids)
                Products.Add(umbracoHelper.TypedContent(id).OfType<Product>());

            return Products;
        }

        /// <summary>
        /// Given a product, trasverses to its parent and returns the product code as a string.
        /// </summary>
        /// <param name="product">The product</param>
        /// <returns>The code</returns>
        public string GetProductCode(Product product)
        {
            var ParentId = product.Parent.Id.ToString();
            var Value = 0;

            if (ParentId.Equals(ContentIds.TalkHomeMobileTopUps) || ParentId.Equals(ContentIds.TalkHomeMobilePlans))
                Value = 1;
            else if (ParentId.Equals(ContentIds.TalkHomeAppTopUps) || ParentId.Equals(ContentIds.TalkHomeAppBundles))
                Value = 2;
            else if (ParentId.Equals(ContentIds.CallingCardsTopUps))
                Value = 3;

            return Enum.GetName(typeof(ProductCodes), Value);
        }

        /// <summary>
        /// Returns the product code given a product Id
        /// </summary>
        /// <param name="id">The Id</param>
        /// <returns>The product code</returns>
        public string GetProductCode(int id)
        {
            var Product = GetProducts(id);
            var ParentId = Product.Parent.Id.ToString();
            var Value = 0;

            if (ParentId.Equals(ContentIds.TalkHomeMobileTopUps) || ParentId.Equals(ContentIds.TalkHomeMobilePlans))
                Value = 1;
            else if (ParentId.Equals(ContentIds.TalkHomeAppTopUps) || ParentId.Equals(ContentIds.TalkHomeAppBundles))
                Value = 2;
            else if (ParentId.Equals(ContentIds.CallingCardsTopUps))
                Value = 3;

            return Enum.GetName(typeof(ProductCodes), Value);
        }

        /// <summary>
        /// Given a product, trasverses to its parent and returns the payment type as integer.
        /// </summary>
        /// <param name="product">The product</param>
        /// <returns>The type</returns>
        public int GetProductType(Product product)
        {
            var ParentId = product.Parent.Id.ToString();

            if (ParentId.Equals(ContentIds.TalkHomeMobilePlans) || ParentId.Equals(ContentIds.TalkHomeAppBundles))
                return (int)ProductType.Bundle;
            else if (ParentId.Equals(ContentIds.TalkHomeMobileTopUps) || ParentId.Equals(ContentIds.TalkHomeAppTopUps) || ParentId.Equals(ContentIds.CallingCardsTopUps))
                return (int)ProductType.TopUp;

            return 0;
        }

        /// <summary>
        /// Returns a collection of top ups for all product codes
        /// </summary>
        /// <returns>The collection</returns>
        public IDictionary<string, IEnumerable<IPublishedContent>> GetAllTopUps()
        {
            IDictionary<string, IEnumerable<IPublishedContent>> TopUps = new Dictionary<string, IEnumerable<IPublishedContent>>();

            TopUps.Add("THM", GetTopUps(int.Parse(ContentIds.TalkHomeMobile)));

            TopUps.Add("THA", GetTopUps(int.Parse(ContentIds.TalkHomeApp)));

            TopUps.Add("THCC", GetTopUps(int.Parse(ContentIds.CallingCards)));

            TopUps.Add("THATT", null);

            return TopUps;
        }

        /// <summary>
        /// Gets all top ups from a product Id
        /// </summary>
        /// <param name="id">The product Id</param>
        /// <returns>The collection</returns>
        public IDictionary<string, IEnumerable<IPublishedContent>> GetAllTopUpsByProductId(int id)
        {
            IDictionary<string, IEnumerable<IPublishedContent>> TopUps = new Dictionary<string, IEnumerable<IPublishedContent>>();

            var Product = GetProducts(id);
            var ProductCode = GetProductCode(Product);

            TopUps.Add(ProductCode, GetTopUps(Product.Parent.Parent.Id));

            return TopUps;
        }

        /// <summary>
        /// Gets all Top ups by Product code and returns a dictionary
        /// </summary>
        /// <param name="productCode">The product code</param>
        /// <returns>The dictionary of the collection</returns>
        public Product GetDefaultTopUpByProductCode(string productCode)
        {
            var AllTopUps = GetAllTopUps();

            foreach (var topUps in AllTopUps)
                if (topUps.Key.Equals(productCode))
                    return topUps.Value.OfType<Product>().Where(x => x.ProductPrice == 10).Select(x => x).First();

            return null;
        }

        /// <summary>
        /// Returns the content for the Error page
        /// </summary>
        /// <returns>The content</returns>
        public IPublishedContent GetErrorPage()
        {
            return GetGenericContentById(int.Parse(ContentIds.CustomErrorPage));
        }

        /// <summary>
        /// Returns the content for the Unuathorised page
        /// </summary>
        /// <returns>The content</returns>
        public IPublishedContent GetUnauthorisedPage()
        {
            return GetGenericContentById(int.Parse(ContentIds.Unauthorised));
        }



        public int GetProductsTotal(string product, HashSet<int> basket, int newProduct)
        {
            int total = 0;

            List<Product> products = GetProducts(basket);

            foreach (Product p in products)
            {
                total += p.ProductPrice;
            }

            Product np = GetProducts(newProduct);

            total += np.ProductPrice;

            return total;
        }

    }
}
