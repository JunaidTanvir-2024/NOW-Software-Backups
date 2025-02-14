//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder v3.0.7.99
//
//   Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.ModelsBuilder;
using Umbraco.ModelsBuilder.Umbraco;

namespace Umbraco.Web.PublishedContentModels
{
	/// <summary>Friend Zone</summary>
	[PublishedContentModel("friendZone")]
	public partial class FriendZone : PublishedContentModel, IMetaProperties
	{
#pragma warning disable 0109 // new is redundant
		public new const string ModelTypeAlias = "friendZone";
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
#pragma warning restore 0109

		public FriendZone(IPublishedContent content)
			: base(content)
		{ }

#pragma warning disable 0109 // new is redundant
		public new static PublishedContentType GetModelContentType()
		{
			return PublishedContentType.Get(ModelItemType, ModelTypeAlias);
		}
#pragma warning restore 0109

		public static PublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<FriendZone, TValue>> selector)
		{
			return PublishedContentModelUtility.GetModelPropertyType(GetModelContentType(), selector);
		}

		///<summary>
		/// Heading
		///</summary>
		[ImplementPropertyType("heading")]
		public string Heading
		{
			get { return this.GetPropertyValue<string>("heading"); }
		}

		///<summary>
		/// Offer SubHeading
		///</summary>
		[ImplementPropertyType("offerSubHeading")]
		public string OfferSubHeading
		{
			get { return this.GetPropertyValue<string>("offerSubHeading"); }
		}

		///<summary>
		/// OfferText
		///</summary>
		[ImplementPropertyType("offerText")]
		public IHtmlString OfferText
		{
			get { return this.GetPropertyValue<IHtmlString>("offerText"); }
		}

		///<summary>
		/// SubHeading
		///</summary>
		[ImplementPropertyType("subHeading")]
		public string SubHeading
		{
			get { return this.GetPropertyValue<string>("subHeading"); }
		}

		///<summary>
		/// OfferHeading
		///</summary>
		[ImplementPropertyType("text")]
		public string Text
		{
			get { return this.GetPropertyValue<string>("text"); }
		}

		///<summary>
		/// Meta description: Enter the SEO description.
		///</summary>
		[ImplementPropertyType("metaDescription")]
		public string MetaDescription
		{
			get { return Umbraco.Web.PublishedContentModels.MetaProperties.GetMetaDescription(this); }
		}

		///<summary>
		/// Meta image: Upload and crop the image for link previews.
		///</summary>
		[ImplementPropertyType("metaImage")]
		public Umbraco.Web.Models.ImageCropDataSet MetaImage
		{
			get { return Umbraco.Web.PublishedContentModels.MetaProperties.GetMetaImage(this); }
		}

		///<summary>
		/// Meta tags: Enter the SEO meta tags.
		///</summary>
		[ImplementPropertyType("metaTags")]
		public string MetaTags
		{
			get { return Umbraco.Web.PublishedContentModels.MetaProperties.GetMetaTags(this); }
		}

		///<summary>
		/// Meta title: Enter the {title} tag content.
		///</summary>
		[ImplementPropertyType("metaTitle")]
		public string MetaTitle
		{
			get { return Umbraco.Web.PublishedContentModels.MetaProperties.GetMetaTitle(this); }
		}

		///<summary>
		/// OG Description: Enter the Open Graph tag description.
		///</summary>
		[ImplementPropertyType("oGDescription")]
		public string OGdescription
		{
			get { return Umbraco.Web.PublishedContentModels.MetaProperties.GetOGdescription(this); }
		}
	}
}
