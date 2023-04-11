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
        public ICollection<ProductSizeColor>? ProductSizeColors { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; }
        public ICollection<ProductTag> ProductTags { get; set; }
        public ICollection<ProductCategory> ProductCategories { get; set; }
        public Collection Collections { get; set; }
        public Instruction Instructions { get; set; }

        public Product()
        {
            ProductImages = new List<ProductImage>();
            ProductTags = new List<ProductTag>();
            ProductCategories = new List<ProductCategory>();
            ProductSizeColors = new List<ProductSizeColor>();
        }
    }
}
