using E_CommerceSystem.Models;

namespace E_CommerceSystem.Repositories
{
    public class CategoryRepo : ICategoryRepo
    {
        public ApplicationDbContext _context;
        public CategoryRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        //Get All Category
        public IEnumerable<Category> GetAllCategory()
        {
            try
            {
                return _context.Categories.ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Database error: {ex.Message}");
            }
        }
        //Get Category by id
        public Category GetCategoryById(int cid)
        {
            try
            {
                return _context.Categories.FirstOrDefault(c => c.CategoryId == cid);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Database error: {ex.Message}");
            }
        }
        //Get Category by name
        public Category GetCategoryByName(string name)
        {
            try
            {
                return _context.Categories.FirstOrDefault(c => c.Name == name);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Database error: {ex.Message}");
            }
        }
        //Add new Category
        public void AddCategory(Category category)
        {
            try
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Database error: {ex.Message}");
            }
        }
        //testing ......
        //Update Category 
        public void UpdateCategory(Category category)
        {
            try
            {
                _context.Categories.Update(category);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Database error: {ex.Message}");
            }
        }

        //Delete Category
        public void DeleteCategory(int cid)
        {
            try
            {
                var category = GetCategoryById(cid);
                if (category != null)
                {
                    _context.Categories.Remove(category);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Database error: {ex.Message}");
            }
        }
    }
}
