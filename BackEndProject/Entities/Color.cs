namespace BackEndProject.Entities
{
	public class Color:BaseEntity
	{
		public string Name { get; set; }
		public string ColorPath { get; set; }
		public ICollection<ProductSizeColor> ProductSizeColors { get; set; }

		public Color()
		{
			ProductSizeColors = new List<ProductSizeColor>();
		}
	}
}
