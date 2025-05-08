using Backend_core.Data;
using Backend_core.Models;
using Backend_core.Models.Backend_core.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_core.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/contacts")]
    public class ContactsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContactsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ContactDto>>> GetContacts()
        {
            var contacts = await _context.Contacts
                .Include(c => c.Category)
                .Include(c => c.Subcategory)
                .ToListAsync();

            var result = contacts.Select(c => new ContactDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                PasswordHash = c.PasswordHash,
                PhoneNumber = c.PhoneNumber,
                BirthDate = c.BirthDate,
                CategoryId = c.CategoryId,
                CategoryName = c.Category?.Name,
                SubcategoryId = c.SubcategoryId,
                SubcategoryName = c.Subcategory?.Name
            });

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateContact([FromBody] UpdateContactDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Walidacja unikalności e-maila
            var emailExists = await _context.Contacts.AnyAsync(c => c.Email == dto.Email);
            if (emailExists)
            {
                return BadRequest(new
                {
                    message = "Użytkownik z takim adresem e-mail już istnieje."
                });
            }

            var contact = new Contact
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = dto.PasswordHash,
                PhoneNumber = dto.PhoneNumber,
                BirthDate = dto.BirthDate,
                CategoryId = dto.CategoryId,
                SubcategoryId = dto.SubcategoryId,
                CustomSubcategory = dto.CustomSubcategory
            };

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return Ok(contact); // ← zwracamy to, co stworzyliśmy
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContactById(int id)
        {
            var contact = await _context.Contacts
                .Include(c => c.Category)
                .Include(c => c.Subcategory)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null)
            {
                return NotFound();
            }

            return Ok(contact);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound("Nie znaleziono kontaktu.");
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(int id, UpdateContactDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID kontaktu nie zgadza się z danymi.");

            var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == id);
            if (contact == null)
                return NotFound();

            // Logika zależna od kategorii
            var category = await _context.Categories.Include(c => c.Subcategories).FirstOrDefaultAsync(c => c.Id == dto.CategoryId);
            if (category == null)
                return BadRequest("Podana kategoria nie istnieje.");

            contact.FirstName = dto.FirstName;
            contact.LastName = dto.LastName;
            contact.Email = dto.Email;
            contact.PasswordHash = dto.PasswordHash;
            contact.PhoneNumber = dto.PhoneNumber;
            contact.BirthDate = dto.BirthDate;
            contact.CategoryId = dto.CategoryId;

            // Jeśli "służbowy", wymagana SubcategoryId, brak Custom
            if (category.Name.ToLower() == "służbowy")
            {
                if (dto.SubcategoryId == null)
                    return BadRequest("Dla kategorii 'Służbowy' wymagana jest podkategoria.");

                contact.SubcategoryId = dto.SubcategoryId;
                contact.CustomSubcategory = null;
            }
            // Jeśli "inny", wymagana CustomSubcategory, brak SubcategoryId
            else if (category.Name.ToLower() == "inne")
            {
                if (string.IsNullOrWhiteSpace(dto.CustomSubcategory))
                    return BadRequest("Dla kategorii 'Inny' wymagana jest niestandardowa podkategoria.");

                // Sprawdź czy taka podkategoria już istnieje w tej kategorii
                var existingSub = category.Subcategories
                    .FirstOrDefault(s => s.Name.ToLower() == dto.CustomSubcategory.ToLower());

                if (existingSub != null)
                {
                    contact.SubcategoryId = existingSub.Id;
                    contact.CustomSubcategory = null; // bo korzystamy z relacji
                }
                else
                {
                    // Utwórz nową podkategorię
                    var newSubcategory = new Subcategory
                    {
                        Name = dto.CustomSubcategory,
                        CategoryId = category.Id
                    };

                    _context.Subcategories.Add(newSubcategory);
                    await _context.SaveChangesAsync(); // Zapisz od razu, żeby mieć ID

                    contact.SubcategoryId = newSubcategory.Id;
                    contact.CustomSubcategory = null;
                }
            }
            // W pozostałych przypadkach: brak subkategorii
            else
            {
                contact.SubcategoryId = null;
                contact.CustomSubcategory = null;
            }

            await _context.SaveChangesAsync();
            return Ok(new ContactDto
            {
                Id = contact.Id,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Email = contact.Email,
                PasswordHash = contact.PasswordHash,
                PhoneNumber = contact.PhoneNumber,
                BirthDate = contact.BirthDate,
                CategoryId = contact.CategoryId,
                CategoryName = category.Name,
                SubcategoryId = contact.SubcategoryId,
                SubcategoryName = contact.Subcategory?.Name ?? dto.CustomSubcategory
            });
        }
    }
}
