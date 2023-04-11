namespace BackEndProject.Entities
{
	public class Instruction:BaseEntity
	{
		public string QualityDetails { get; set; }
		public string Lining { get; set; }
		public string Clean { get; set; }
		public string Only { get; set; }

		public ICollection<Product> Products { get; set; }

        public Instruction()
        {
			Products= new List<Product>();
		}
    }
}
