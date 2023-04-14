namespace BackEndProject.Entities
{
    public class ProductCategory : BaseEntity
    {
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public Product Product { get; set; }
    }
}
