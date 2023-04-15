namespace BackEndProject.Entities
{
	public class WishListItem : BaseEntity
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		public string UserId { get; set; }

		public Product Product { get; set; }
		public User User { get; set; }
	}
}
