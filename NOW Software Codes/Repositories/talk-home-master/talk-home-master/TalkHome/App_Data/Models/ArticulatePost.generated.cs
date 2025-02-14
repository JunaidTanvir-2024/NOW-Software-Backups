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
	/// <summary>Articulate Post</summary>
	[PublishedContentModel("ArticulatePost")]
	public partial class ArticulatePost : PublishedContentModel
	{
#pragma warning disable 0109 // new is redundant
		public new const string ModelTypeAlias = "ArticulatePost";
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
#pragma warning restore 0109

		public ArticulatePost(IPublishedContent content)
			: base(content)
		{ }

#pragma warning disable 0109 // new is redundant
		public new static PublishedContentType GetModelContentType()
		{
			return PublishedContentType.Get(ModelItemType, ModelTypeAlias);
		}
#pragma warning restore 0109

		public static PublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<ArticulatePost, TValue>> selector)
		{
			return PublishedContentModelUtility.GetModelPropertyType(GetModelContentType(), selector);
		}

		///<summary>
		/// Author
		///</summary>
		[ImplementPropertyType("author")]
		public string Author
		{
			get { return this.GetPropertyValue<string>("author"); }
		}

		///<summary>
		/// Categories
		///</summary>
		[ImplementPropertyType("categories")]
		public IEnumerable<string> Categories
		{
			get { return this.GetPropertyValue<IEnumerable<string>>("categories"); }
		}

		///<summary>
		/// Enable Comments
		///</summary>
		[ImplementPropertyType("enableComments")]
		public bool EnableComments
		{
			get { return this.GetPropertyValue<bool>("enableComments"); }
		}

		///<summary>
		/// Excerpt
		///</summary>
		[ImplementPropertyType("excerpt")]
		public string Excerpt
		{
			get { return this.GetPropertyValue<string>("excerpt"); }
		}

		///<summary>
		/// Import Id: Used during a blogml import process to ensure that re-importing does not create duplicate posts.
		///</summary>
		[ImplementPropertyType("importId")]
		public string ImportId
		{
			get { return this.GetPropertyValue<string>("importId"); }
		}

		///<summary>
		/// Post Image: An optional image for your blog post
		///</summary>
		[ImplementPropertyType("postImage")]
		public Umbraco.Web.Models.ImageCropDataSet PostImage
		{
			get { return this.GetPropertyValue<Umbraco.Web.Models.ImageCropDataSet>("postImage"); }
		}

		///<summary>
		/// Published Date: This is the date that the document will be shown as published on your website
		///</summary>
		[ImplementPropertyType("publishedDate")]
		public DateTime PublishedDate
		{
			get { return this.GetPropertyValue<DateTime>("publishedDate"); }
		}

		///<summary>
		/// Social Description: Open Graph Description - Describe the article in one or two lines.
		///</summary>
		[ImplementPropertyType("socialDescription")]
		public string SocialDescription
		{
			get { return this.GetPropertyValue<string>("socialDescription"); }
		}

		///<summary>
		/// Tags
		///</summary>
		[ImplementPropertyType("tags")]
		public IEnumerable<string> Tags
		{
			get { return this.GetPropertyValue<IEnumerable<string>>("tags"); }
		}

		///<summary>
		/// umbracoUrlAlias
		///</summary>
		[ImplementPropertyType("umbracoUrlAlias")]
		public string UmbracoUrlAlias
		{
			get { return this.GetPropertyValue<string>("umbracoUrlAlias"); }
		}

		///<summary>
		/// Slug: If left blank then umbraco will auto-generate the URL name based on the node name
		///</summary>
		[ImplementPropertyType("umbracoUrlName")]
		public string UmbracoUrlName
		{
			get { return this.GetPropertyValue<string>("umbracoUrlName"); }
		}
	}
}
