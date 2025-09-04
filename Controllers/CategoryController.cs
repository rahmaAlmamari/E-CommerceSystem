using Microsoft.AspNetCore.Mvc;
using E_CommerceSystem.Models;
using E_CommerceSystem.Services;

namespace E_CommerceSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("GetAllCategories")]
        public IActionResult GetAllCategories()
        {
            var categories = _categoryService.GetAllCategory();
            return Ok(categories);
        }

        [HttpGet("GetCategoryById/{id:int}")]
        public IActionResult GetCategoryById(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null)
                return NotFound($"Category with ID {id} not found.");
            return Ok(category);
        }

        [HttpGet("GetCategoryByName/{name}")]
        public IActionResult GetCategoryByName(string name)
        {
            var category = _categoryService.GetCategoryByName(name);
            if (category == null)
                return NotFound($"Category with name '{name}' not found.");
            return Ok(category);
        }

        [HttpPost("AddCategory")]
        public IActionResult AddCategory([FromBody] Category category)
        {
            if (category == null || string.IsNullOrWhiteSpace(category.Name))
                return BadRequest("Category data is required.");

            _categoryService.AddCategory(category);
            return CreatedAtAction(nameof(GetCategoryById),
                                   new { id = category.CategoryId },
                                   category);
        }

        [HttpPut("UpdateCategory/{id:int}")]
        public IActionResult UpdateCategory(int id, [FromBody] Category category)
        {
            if (category == null || category.CategoryId != id)
                return BadRequest("Category data is invalid.");

            var existing = _categoryService.GetCategoryById(id);
            if (existing == null)
                return NotFound($"Category with ID {id} not found.");

            existing.Name = category.Name;
            _categoryService.UpdateCategory(existing);

            return Ok(existing);
        }

        [HttpDelete("DeleteCategory/{id:int}")]
        public IActionResult DeleteCategory(int id)
        {
            var existing = _categoryService.GetCategoryById(id);
            if (existing == null)
                return NotFound($"Category with ID {id} not found.");

            _categoryService.DeleteCategory(id);
            return Ok($"Category with ID {id} has been deleted.");
        }
    }
}
