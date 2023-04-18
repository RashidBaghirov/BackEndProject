using System.ComponentModel.DataAnnotations;

namespace BackEndProject.Entities;


public class OrderItem : BaseEntity
{
	public int OrderId { get; set; }
	public Order Order { get; set; }
	public decimal UnitPrice { get; set; }
	public int SaleQuantity { get; set; }
	public int ProductSizeColorId { get; set; }
	public ProductSizeColor ProductSizeColor { get; set; }
}
