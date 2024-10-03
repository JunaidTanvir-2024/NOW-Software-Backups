using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using TalkHome.Models;
using TalkHome.Services;
using Umbraco.Web.PublishedContentModels;
using System.Linq;
using TalkHome.Interfaces;
using System.Collections;
using TalkHome.Models.ViewModels.BusinessIntelligence;
using umbraco;
using TalkHome.Extensions.Html;
using Umbraco.Web;

namespace TalkHome.Extensions.Html
{
    /// <summary>
    /// Adds customers accounts related method used in views
    /// </summary>
    public static class AccountExtensions
    {
        public enum DataType
        {
            Minutes,
            Data,
            Text
        }
        private static Properties.ContentIds ContentIds = Properties.ContentIds.Default;

        /// <summary>
        /// Returns the links for the main menu form the content cache
        /// </summary>
        /// <param name="helper">The helper class</param>
        /// <returns>The list of links</returns>
        public static List<HyperLink> GetMainMenuTopLinks(this HtmlHelper helper)
        {
            var ContentService = DependencyResolver.Current.GetService<IContentService>();
            var TopLinks = new List<HyperLink>();

            var TalkHomeRoot = ContentService.GetGenericContentById(int.Parse(ContentIds.TalkHome));

            foreach (var Child in TalkHomeRoot.Children)
                TopLinks.Add(new HyperLink { Target = Child.Url, Text = Child.Name });

            return TopLinks;
        }

        /// <summary>
        /// Returns a collection with the menu children links
        /// </summary>
        /// <param name="helper">The helper class</param>
        /// <returns>The collection of links</returns>
        public static Dictionary<int, List<HyperLink>> GetMainMenuChildrenLinks(this HtmlHelper helper)
        {
            var ContentService = DependencyResolver.Current.GetService<IContentService>();
            var ChildrenLinks = new Dictionary<int, List<HyperLink>>();

            var TalkHomeRoot = ContentService.GetGenericContentById(int.Parse(ContentIds.TalkHome));

            var Ids = new int[] {
                int.Parse(ContentIds.ThmUKPlans),
                int.Parse(ContentIds.ThmUKRates),
                int.Parse(ContentIds.ThaBundles),
                int.Parse(ContentIds.ThaGBRates),
                int.Parse(ContentIds.ThaDownload),
                int.Parse(ContentIds.ThccMinutes)
            };

            Func<int, List<HyperLink>> GetChildrenLinks = (y) =>
            {
                var ChildreLinks = TalkHomeRoot.Children.Where(x => x.Id == y).OfType<TalkHomeProduct>().First()
                    .Children.Where(z => Ids.Contains(z.Id)).OfType<CustomPage>()
                    .Select(z => new HyperLink { Target = z.Url, Text = z.Name }).ToList();

                return ChildreLinks;
            };

            foreach (var Child in TalkHomeRoot.Children)
                ChildrenLinks.Add(Child.Id, GetChildrenLinks(Child.Id));

            return ChildrenLinks;
        }

        public static Dictionary<int, HyperLink> GetUserMenuLinks(this HtmlHelper helper)
        {
            //var ContentService = new ContentService();
            //var UserLinks = new Dictionary<int, HyperLink>();

            //var UserPagesRoot = ContentService.GetGenericContentById(int.Parse(ContentIds.UserPages)).Children;

            //var Ids = new int[] {
            //    int.Parse(ContentIds.Help),
            //    int.Parse(ContentIds.Basket),
            //    int.Parse(ContentIds.LogIn),
            //    int.Parse(ContentIds.TopUp)
            //};

            //Func<int, HyperLink> GetLink = (x) =>
            //{
            //    return UserPagesRoot.Where(z => z.Id == x).Select(z => new HyperLink { Target = z.UrlName, Text = z.Name }).First();
            //};

            //foreach (var Id in Ids)
            //    UserLinks.Add(Id, GetLink(Id));

            //return UserLinks;
            return null;
        }

        /// <summary>
        /// Checks if the customer has a valid login
        /// </summary>
        /// <param name="helper">The Helper class</param>
        /// <param name="payload">The customer token</param>
        /// <returns>Result as FALSE or TRUE</returns>
        public static bool IsAuthorized(this HtmlHelper helper, JWTPayload payload)
        {
            var AccountService = DependencyResolver.Current.GetService<IAccountService>();
            return AccountService.IsAuthorized(payload);
        }

        /// <summary>
        /// Returns the content for the Error page
        /// </summary>
        /// <param name="helper">The Helper class</param>
        /// <returns>The strongly-typed content</returns>
        public static SimplePage GetErrorPage(this HtmlHelper helper)
        {
            var ContentService = DependencyResolver.Current.GetService<IContentService>();
            return (SimplePage)ContentService.GetErrorPage();
        }

        /// <summary>
        /// Returns the content for the Error page
        /// </summary>
        /// <param name="helper">The Helper class</param>
        /// <returns>The strongly-typed content</returns>
        public static SimplePage GetUnauthorisedPage(this HtmlHelper helper)
        {
            var ContentService = new ContentService();
            return (SimplePage)ContentService.GetUnauthorisedPage();
        }

        /// <summary>
        /// Returns the product code via the content service
        /// </summary>
        /// <param name="helper">The Helper class</param>
        /// <param name="id">The product id</param>
        /// <returns>The product code</returns>
        public static string GetProductCodeByProductId(this HtmlHelper helper, int id)
        {
            var ContentService = DependencyResolver.Current.GetService<IContentService>();
            return ContentService.GetProductCode(ContentService.GetProducts(id));
        }

        /// <summary>
        /// Gets the human-readable product code from the Account Service
        /// </summary>
        /// <param name="helper">The Helper class</param>
        /// <param name="productCode">The ref to the error</param>
        /// <returns>The readable code</returns>
        public static string GetReadableProductCode(this HtmlHelper helper, string productCode)
        {
            var AccountService = DependencyResolver.Current.GetService<IAccountService>();
            string Error = "";

            var ReadableProductCode = AccountService.GetReadableProductCode(productCode, out Error);

            if (!Error.Equals(""))
                return Error;

            return ReadableProductCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="helper">The Helper class</param>
        /// <param name="productCode">The ref to the error</param>
        /// <returns>The product name</returns>
        public static string GetProductName(this HtmlHelper helper, string productCode)
        {
            var AccountService = DependencyResolver.Current.GetService<IAccountService>();
            string Error = "";

            var ProductCode = AccountService.GetProductName(productCode, out Error);

            if (!Error.Equals(""))
                return Error;

            return ProductCode;
        }

        /// <summary>
        /// Static access to Account service method
        /// </summary>
        /// <param name="helper">The Helper class</param>
        /// <param name="number">The number</param>
        /// <param name="countryCode">The alpha-2 country code</param>
        /// <returns>The Msisdn</returns>
        public static string GetMsisdnFromNumber(this HtmlHelper helper, string number, string countryCode)
        {
            var AccountService = DependencyResolver.Current.GetService<IAccountService>();
            return AccountService.GetMsisdnFromNumber(number, countryCode);
        }

        /// <summary>
        /// Static access to Account service method
        /// </summary>
        /// <param name="helper">The Helper class</param>
        /// <param name="msisdn">The Msisdn</param>
        /// <param name="countryCode">The alpha-2 country code</param>
        /// <returns>The number</returns>
        public static string GetNumberFromMsisdn(this HtmlHelper helper, string msisdn, string countryCode)
        {
            var AccountService = DependencyResolver.Current.GetService<IAccountService>();
            return AccountService.GetNumberFromMsisdn(msisdn, countryCode);
        }

        /// <summary>
        /// Static access to content service GetProducts(int id)
        /// </summary>
        /// <param name="helper">The Helper class</param>
        /// <param name="id">the product Id</param>
        /// <returns>The product</returns>
        public static Product GetProducts(this HtmlHelper helper, int id)
        {
            var ContentService = DependencyResolver.Current.GetService<IContentService>();
            return ContentService.GetProducts(id);
        }

        /// <summary>
        /// Static access to Payment Service GetBasketTotal() method
        /// </summary>
        /// <param name="helper">The Helper class</param>
        /// <param name="products">the set of porducts Ids</param>
        /// <returns>The total</returns>
        public static string GetTotal(this HtmlHelper helper, HashSet<int> products)
        {
            var ContentService = DependencyResolver.Current.GetService<IContentService>();
            var Products = ContentService.GetProducts(products);
            var Total = new decimal(0.0);

            foreach (var product in Products)
                Total += product.ProductPrice;

            return Total.ToString();
        }


        /// <summary>
        /// Returns the product code via the content service
        /// </summary>
        /// <param name="helper">The Helper class</param>
        /// <param name="id">The product id</param>
        /// <returns>The product code</returns>
        public static Product GetProductCodeFromGuid(this HtmlHelper helper, string guid, string productCode)
        {
            var ContentService = DependencyResolver.Current.GetService<IContentService>();
            if (productCode == "THM")
                return ContentService.GetMobilePlanByGuid(guid);
            else
                return ContentService.GetAppBundleByGuid(guid);
        }

        public static Product GetProductCodeFromGuid(string guid, string productCode)
        {
            var ContentService = DependencyResolver.Current.GetService<IContentService>();
            if (productCode == "THM")
                return ContentService.GetMobilePlanByGuid(guid);
            else
                return ContentService.GetAppBundleByGuid(guid);
        }


        public static string VisualiseData(this HtmlHelper helper, double remaining, double total, DataType dt)
        {
            remaining = 120;
            double uP = (double)(((total - remaining) / total) * 100);
            double rP = (double)((remaining / total) * 100);
            return String.Format("<img src='http://chart.apis.google.com/chart?chs=250x100&amp;chd=t:{0},{1}&amp;cht=p&amp;chl=Used|Remaining' alt = 'chart' />", uP, rP);
        }


        public static IList<I18nCountry> GetCountries(this HtmlHelper helper)
        {
            var AccountService = DependencyResolver.Current.GetService<IAccountService>();
            return AccountService.GetCountryList();
        }

        public static ArrayList ProductImression(this HtmlHelper helper,List<UpgradePlan> Plans)
        {
            ArrayList ProductImpression = new ArrayList();
            foreach (var plan in Plans)
            {
                foreach (var item in plan.UpgradePlans)
                {
                    var Product = GetProductCodeFromGuid(item, "THM");
                    var ImpressionPlan = new[] {
                new  {name = Product.GetPropertyValue("ProductName").ToString() + " " +  Product.GetPropertyValue("SubName").ToString(),id = Product.Id.ToString(), price = Product.GetPropertyValue("ProductPrice").ToString(), brand ="TalkHome" ,category ="Sim Cards" ,variant = Product.GetPropertyValue("ProductName").ToString(),list ="Top Plans",position =1 }
};
                    ProductImpression.Add(ImpressionPlan);
                }
            }
            return ProductImpression;
        }
    }
}
