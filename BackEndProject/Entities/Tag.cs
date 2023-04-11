namespace BackEndProject.Entities
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<ProductTag> ProductTags { get; set; }

        public Tag()
        {
            ProductTags = new List<ProductTag>();
        }
    }
}
