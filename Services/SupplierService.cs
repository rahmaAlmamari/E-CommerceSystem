using E_CommerceSystem.Models;
using E_CommerceSystem.Repositories;

namespace E_CommerceSystem.Services
{
    public class SupplierService
    {
        private readonly ISupplierRepo _supplierRepo;
        public SupplierService(ISupplierRepo supplierRepo)
        {
            //Dependency Injection
            _supplierRepo = supplierRepo;
        }
        //Get All Suppliers
        public IEnumerable<Supplier> GetAllSuppliers()
        {
            //call the repo method to get all suppliers data
            return _supplierRepo.GetAllSupplier();
        }
        //Get Supplier by id
        public Supplier GetSupplierById(int sid)
        {
            //call the repo method to get supplier data by id
            return _supplierRepo.GetSupplierById(sid);
        }
        //Get Supplier by name
        public Supplier GetSupplierByName(string name)
        {
            //call the repo method to get supplier data by name
            return _supplierRepo.GetSupplierByName(name);
        }
        //Add new Supplier
        public void AddSupplier(Supplier supplier)
        {
            //check if supplier with same name already exists
            _supplierRepo.AddSupplier(supplier);
        }

        public void UpdateSupplier(Supplier supplier)
        {
            _supplierRepo.UpdateSupplier(supplier);
        }

        public void DeleteSupplier(int sid)
        {
            _supplierRepo.DeleteSupplier(sid);
        }
    }
}
