using BackEndProject.Utilities.Enum;
using System.ComponentModel.DataAnnotations;

namespace BackEndProject.Entities
{
	public class Order : BaseEntity
	{

		[StringLength(maximumLength: 250)]
		public string Address { get; set; }

		[Required]
		[StringLength(maximumLength: 50)]
		public string FullName { get; set; }

		[Required]
		[StringLength(maximumLength: 100)]
		public string Email { get; set; }

		[StringLength(maximumLength: 500)]
		public string Note { get; set; }

		public DateTime CreatedAt { get; set; }
		public OrderStatus Status { get; set; }
		public decimal TotalPrice { get; set; }

		public List<OrderItem> OrderItems { get; set; }
		public int BasketId { get; set; }
		public Basket Basket { get; set; }
		public string UserId { get; set; }
	}
}
