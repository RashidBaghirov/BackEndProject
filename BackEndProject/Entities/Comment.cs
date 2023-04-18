namespace BackEndProject.Entities
{
	public class Comment : BaseEntity
	{
		public string Text { get; set; }
		public DateTime Date { get; set; }
		public User User { get; set; }
		public int ProductId { get; set; }
		public Product Product { get; set; }
	}
}
