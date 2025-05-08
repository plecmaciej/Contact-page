namespace Backend_core.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
    }
}
