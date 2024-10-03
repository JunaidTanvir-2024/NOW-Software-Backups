using System.Collections.Generic;
using Umbraco.Core.Models;
using Umbraco.Web.PublishedContentModels;

namespace TalkHome.Interfaces
{
    public interface IContentService
    {
        IPublishedContent GetGenericContentById(int id);

        CampaignBlock GetCampaign(IEnumerable<IPublishedContent> children);

        TestNestedDoc GetFold(IEnumerable<IPublishedContent> children);

        List<IPublishedContent> GetOffers(IEnumerable<IPublishedContent> children);

        List<IPublishedContent> GetFaqs(IEnumerable<IPublishedContent> children);

        IEnumerable<IPublishedContent> GetTopProducts(int id, int max);

        IEnumerable<IPublishedContent> GetTopUps(int id, int max = 0);

        Product GetAppBundleByGuid(string guid);

        Product GetMobilePlanByGuid(string guid);

        IEnumerable<IPublishedContent> GetAppTopUps();

        IEnumerable<IPublishedContent> GetTalkHomeMobilePlans();

        IEnumerable<IPublishedContent> GetBundles();

        Product GetProducts(int id);

        List<Product> GetProducts(HashSet<int> ids);

        string GetProductCode(Product product);

        string GetProductCode(int id);

        int GetProductType(Product product);

        IDictionary<string, IEnumerable<IPublishedContent>> GetAllTopUps();

        IDictionary<string, IEnumerable<IPublishedContent>> GetAllTopUpsByProductId(int id);

        Product GetDefaultTopUpByProductCode(string productCode);

        IPublishedContent GetErrorPage();

        IPublishedContent GetUnauthorisedPage();

        string GetMobileBundleGuid(int id);

        string GetAppBundleGuid(int id);

        int GetProductsTotal(string product, HashSet<int> basket, int newProduct);

    }
}
