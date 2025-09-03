using E_CommerceSystem.Models;

namespace E_CommerceSystem.Services
{
    public interface ICategoryService
    {
        void AddCategory(Category category);
        void DeleteCategory(int cid);
        IEnumerable<Category> GetAllCategory();
        Category GetCategoryById(int cid);
        Category GetCategoryByName(string name);
        void UpdateCategory(Category category);
    }
}