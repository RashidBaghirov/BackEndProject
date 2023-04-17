using System.ComponentModel.DataAnnotations;

namespace BackEndProject.Entities;


public class OrderItem : BaseEntity
{
	//public int? ProductId { get; set; }
	//public int Count { get; set; }

	//[StringLength(maximumLength: 100)]
	//public string ProductName { get; set; }

	//[StringLength(maximumLength: 100)]
	//public string ProductImage { get; set; }

	//public double SalePrice { get; set; }
	//public double CostPrice { get; set; }

	//public Product Product { get; set; }
	public int OrderId { get; set; }
	public Order Order { get; set; }
	public decimal UnitPrice { get; set; }
	public int SaleQuantity { get; set; }
	public int ProductSizeColorId { get; set; }
	public ProductSizeColor ProductSizeColor { get; set; }
}
