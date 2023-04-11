namespace BackEndProject.Entities
{
    public class ProductCategory:BaseEntity
    {
        public Category Category { get; set; }
        public Product Product { get; set; }
    }
}
