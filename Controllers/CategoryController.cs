using Microsoft.AspNetCore.Mvc;
using E_CommerceSystem.Models;
using E_CommerceSystem.Services;
namespace E_CommerceSystem.Controllers
{
    public class CategoryController
    {
        [ApiController]
        [Route("api/[Controller]")]
        public class CategoriesController : ControllerBase
        {
            private readonly ICategoryService _categoryService;

            //dependency injection
            public CategoriesController(ICategoryService categoryService)
            {
                _categoryService = categoryService;
            }
            [HttpGet]
            public IActionResult GetAllCategories() 
            { 
            var categories = _categoryService.GetAllCategory();
                return Ok(categories);
            }

            [HttpGet("{id}")]
            public IActionResult GetCategoryById(int id)
            {
                var category = _categoryService.GetCategoryById(id);
                if (category == null)
                {
                    return NotFound($"Category with ID {id} not found.");
                }
                return Ok(category);
            }

            [HttpGet("ByName/{name}")]
            public IActionResult GetCategoryByName(string name)
            {
                var category = _categoryService.GetCategoryByName(name);
                if (category == null)
                {
                    return NotFound($"Category with name '{name}' not found.");
                }
                return Ok(category);
            }

            [HttpPost]
            public IActionResult AddCategory([FromBody] Category category)
            {
                if (category == null || string.IsNullOrWhiteSpace(category.Name))
                {
                    return BadRequest("Category data is required.");
                }
                // Add the new category to the database/service layer
                _categoryService.AddCategory(category);
                return CreatedAtAction(nameof(GetCategoryById), new {id=category.CategoryId},category);
            }

            [HttpPut("{id}")]
            [HttpPut("{id}")]
            public IActionResult UpdateCategory(int id, [FromBody] Category category)
            {
                if (category == null || category.CategoryId != id)
                    return BadRequest("Category data is invalid.");

                var existingCategory = _categoryService.GetCategoryById(id);
                if (existingCategory == null)
                    return NotFound($"Category with ID {id} not found.");

               
                existingCategory.Name = category.Name;

            
               _categoryService.UpdateCategory(existingCategory);
                return Ok(existingCategory);
            }
            [HttpDelete("{id}")]
            public IActionResult DeleteCategory(int id)
            {
                var category = _categoryService.GetCategoryById(id);
                if (category == null)
                    return NotFound($"Category with ID {id} not found.");

                _categoryService.DeleteCategory(id);
                return Ok($"Category with ID {id} has been deleted.");
            }
        }
    }
}
    
