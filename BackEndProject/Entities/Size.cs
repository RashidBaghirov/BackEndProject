namespace BackEndProject.Entities
{
	public class Size:BaseEntity
	{
		public string Name { get; set; }
		public ICollection<ProductSizeColor> ProductSizeColors { get; set; }


		public Size()
        {
			ProductSizeColors = new List<ProductSizeColor>();
		}
    }
}
