namespace BackEndProject.Entities
{
	public class BasketItem : BaseEntity
	{
		public decimal UnitPrice { get; set; }
		public int SaleQuantity { get; set; }
		public Basket Basket { get; set; }
		public ProductSizeColor ProductSizeColor { get; set; }
	}
}
