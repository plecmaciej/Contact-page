namespace Backend_core.Models
{

    public class Contact
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public DateTime? BirthDate { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int? SubcategoryId { get; set; }
        public Subcategory? Subcategory { get; set; }

        public string? CustomSubcategory { get; set; }
    }
}