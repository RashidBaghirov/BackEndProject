namespace BackEndProject.Entities
{
    public class ProductTag : BaseEntity
    {
        public int TagId { get; set; }
        public Tag Tag { get; set; }
        public Product Product { get; set; }
    }
}
