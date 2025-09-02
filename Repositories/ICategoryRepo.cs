using E_CommerceSystem.Models;

namespace E_CommerceSystem.Repositories
{
    public interface ICategoryRepo
    {
        void AddCategory(Category category);
        IEnumerable<Category> GetAllCategory();
        Category GetCategoryById(int cid);
        Category GetCategoryByName(string name);
        void UpdateCategory(Category category);
        void DeleteCategory(int cid);
    }
}