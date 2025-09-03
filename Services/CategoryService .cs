using E_CommerceSystem.Models;
using E_CommerceSystem.Repositories;

namespace E_CommerceSystem.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepo _categoryRepo;
        public CategoryService(ICategoryRepo categoryRepo)
        {
            //Dependency Injection
            _categoryRepo = categoryRepo;
        }
        //Get All Category
        public IEnumerable<Category> GetAllCategory()
        {
            //call the repo method to get all categories data
            return _categoryRepo.GetAllCategory();
        }
        //Get Category by id
        public Category GetCategoryById(int cid)
        {
            //call the repo method to get category data by id
            return _categoryRepo.GetCategoryById(cid);
        }
        //Get Category by name
        public Category GetCategoryByName(string name)
        {
            //call the repo method to get category data by name
            return _categoryRepo.GetCategoryByName(name);
        }
        //Add new Category
        public void AddCategory(Category category)
        {
            //check if category with same name already exists
            _categoryRepo.AddCategory(category);
        }
    }

}

