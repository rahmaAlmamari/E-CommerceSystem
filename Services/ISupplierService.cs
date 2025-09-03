using E_CommerceSystem.Models;

namespace E_CommerceSystem.Services
{
    public interface ISupplierService
    {
        void AddSupplier(Supplier supplier);
        void DeleteSupplier(int sid);
        IEnumerable<Supplier> GetAllSuppliers();
        Supplier GetSupplierById(int sid);
        Supplier GetSupplierByName(string name);
        void UpdateSupplier(Supplier supplier);
    }
}