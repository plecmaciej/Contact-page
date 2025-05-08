using Backend_core.Data;
using Backend_core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_core.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        [HttpGet("{id}/subcategories")]
        public async Task<ActionResult<IEnumerable<SubcategoryDto>>> GetSubcategoriesForCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Subcategories)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound("Nie znaleziono kategorii.");
            }

            var result = category.Subcategories
                .Select(sub => new SubcategoryDto
                {
                    Id = sub.Id,
                    Name = sub.Name
                })
                .ToList();

            return Ok(result);
        }
    }
}
