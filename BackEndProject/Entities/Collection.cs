namespace BackEndProject.Entities
{
	public class Collection:BaseEntity
	{
		public string Name { get; set; }
		public ICollection<Product> Products { get; set; }
        public Collection()
        {
			Products = new List<Product>();
		}
    }
}
