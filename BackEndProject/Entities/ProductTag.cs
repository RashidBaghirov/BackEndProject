namespace BackEndProject.Entities
{
    public class ProductTag : BaseEntity
    {

        public Tag Tag { get; set; }
        public Product Product { get; set; }
    }
}
