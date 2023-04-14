using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BackEndProject.Entities;

namespace BackEndProject.ViewModel
{
    public class ProductVM
    {
        public int Id { get; set; }
        [StringLength(maximumLength: 20)]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal? Discount { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string SKU { get; set; }
        public int BarCode { get; set; }

        public string Desc { get; set; }
        public string ShortDesc { get; set; }
        public int CollectionId { get; set; }
        public int InstructionId { get; set; }
        public int GlobalTabId { get; set; }

        [NotMapped]
        public ICollection<int> CategoryIds { get; set; } = null!;
        [NotMapped]
        public ICollection<int> TagIds { get; set; } = null!;
        [NotMapped]
        public IFormFile? MainPhoto { get; set; }
        [NotMapped]
        public IFormFile? HoverPhoto { get; set; }
        [NotMapped]
        public ICollection<IFormFile>? Images { get; set; }
        [NotMapped]
        public ICollection<ProductImage>? AllImages { get; set; }
        [NotMapped]
        public ICollection<int>? ImagesId { get; set; }
        [NotMapped]
        public string? ColorsSizesQuantity { get; set; }
        public string? ProductSizeColorDelete { get; set; }
        public ICollection<ProductSizeColor>? ProductSizeColors { get; set; }

        public ProductVM()
        {
            ProductSizeColors = new List<ProductSizeColor>();
        }
    }
}
