using System.ComponentModel.DataAnnotations;

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
		public int Stock { get; set; }
		public string SKU { get; set; }
		public int BarCode { get; set; }
		public List<ProductSizeColor>? ProductSizeColors { get; set; }
		public List<ProductImage> ProductImages { get; set; }
		public List<ProductTag> ProductTags { get; set; }
		public List<ProductCategory> ProductCategories { get; set; }
		public Collection Collections { get; set; }
		public Instruction Instructions { get; set; }
		public GlobalTab GlobalTab { get; set; }

		public Product()
		{
			ProductImages = new List<ProductImage>();
			ProductTags = new List<ProductTag>();
			ProductCategories = new List<ProductCategory>();
			ProductSizeColors = new List<ProductSizeColor>();
		}
	}
}
