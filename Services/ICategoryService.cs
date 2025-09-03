using E_CommerceSystem.Models;

namespace E_CommerceSystem.Services
{
    public interface ICategoryService
    {
        void AddCategory(Category category);
        IEnumerable<Category> GetAllCategory();
        Category GetCategoryById(int cid);
        Category GetCategoryByName(string name);
    }
}