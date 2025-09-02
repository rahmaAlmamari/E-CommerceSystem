using E_CommerceSystem.Models;

namespace E_CommerceSystem.Repositories
{
    public interface ISupplierRepo
    {
        void AddSupplier(Supplier supplier);
        void DeleteSupplier(int sid);
        IEnumerable<Supplier> GetAllSupplier();
        Supplier GetSupplierById(int sid);
        Supplier GetSupplierByName(string name);
        void UpdateSupplier(Supplier supplier);
    }
}