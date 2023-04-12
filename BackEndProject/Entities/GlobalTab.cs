namespace BackEndProject.Entities
{
	public class GlobalTab : BaseEntity
	{
		public string Text { get; set; }
		public ICollection<Product> Products { get; set; }
		public GlobalTab()
		{
			Products = new List<Product>();
		}
	}
}
