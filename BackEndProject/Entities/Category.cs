namespace BackEndProject.Entities
{
	public class Category:BaseEntity
	{
		public string Name { get; set; }

		public ICollection<ProductCategory> ProductCategories { get; set; }

		public Category()
		{
			ProductCategories = new List<ProductCategory>();
		}

	}
}
