namespace Backend_core.Models
{
    public class Subcategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
