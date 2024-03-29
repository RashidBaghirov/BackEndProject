﻿using BackEndProject.ViewModel.Addcart;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEndProject.Entities
{
	public class Product : BaseEntity
	{
		[Required]
		[StringLength(maximumLength: 50)]
		public string Name { get; set; }
		public decimal Price { get; set; }
		public decimal Discount { get; set; }
		public decimal? DiscountPrice { get; set; }
		public string ShortDesc { get; set; }
		public string Desc { get; set; }
		public bool InStock { get; set; }
		public string SKU { get; set; }
		public int BarCode { get; set; }
		public List<ProductSizeColor>? ProductSizeColors { get; set; }
		public List<ProductImage> ProductImages { get; set; }
		public List<ProductTag> ProductTags { get; set; }
		public List<ProductCategory> ProductCategories { get; set; }
		public List<Comment>? Comments { get; set; }
		public int CollectionsId { get; set; }
		public Collection Collections { get; set; }
		public int InstructionsId { get; set; }
		public Instruction Instructions { get; set; }
		public int GlobalTabId { get; set; }
		public GlobalTab GlobalTab { get; set; }

		[NotMapped]
		public AddCartVM AddCart { get; set; }

		public Product()
		{
			ProductImages = new();
			ProductTags = new();
			ProductCategories = new();
			ProductSizeColors = new();
			Comments = new();
		}
	}
}
