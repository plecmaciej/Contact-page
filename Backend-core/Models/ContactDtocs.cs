namespace Backend_core.Models
{
    namespace Backend_core.Dtos
    {
        public class ContactDto
        {
            public int Id { get; set; }
            public string FirstName { get; set; } = null!;
            public string LastName { get; set; } = null!;
            public string Email { get; set; } = null!;

            public string PasswordHash { get; set; } = null!;
            public string PhoneNumber { get; set; } = null!;
            public DateTime? BirthDate { get; set; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; } = null!;
            public int? SubcategoryId { get; set; }
            public string? SubcategoryName { get; set; }
        }
    }

}
