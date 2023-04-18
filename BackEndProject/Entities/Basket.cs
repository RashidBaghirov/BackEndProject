using BackEndProject.Utilities.Enum;

namespace BackEndProject.Entities
{
	public class Basket : BaseEntity
	{
		public decimal TotalPrice { get; set; }
		public User User { get; set; }
		public Order Order { get; set; }
		public OrderStatus IsOrdered { get; set; } = OrderStatus.Pending;
		public List<BasketItem> BasketItems { get; set; } = null!;

		public Basket()
		{
			BasketItems = new List<BasketItem>();
		}
	}
}
