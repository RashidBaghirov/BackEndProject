namespace BackEndProject.Entities
{
	public class ProductImage:BaseEntity
	{
		public string Path { get; set; }
		public bool? IsMain { get; set; }
		public Product Product { get; set; }
	}
}
