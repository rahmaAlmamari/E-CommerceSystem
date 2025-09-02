using E_CommerceSystem.Models;

namespace E_CommerceSystem.Repositories
{
    public class SupplierRepo : ISupplierRepo
    {
        public ApplicationDbContext _context;
        public SupplierRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        //Get All Supplier
        public IEnumerable<Supplier> GetAllSupplier()
        {
            try
            {
                return _context.Suppliers.ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Database error: {ex.Message}");
            }
        }
        //Get Supplier by id
        public Supplier GetSupplierById(int sid)
        {
            try
            {
                return _context.Suppliers.FirstOrDefault(s => s.SupplierId == sid);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Database error: {ex.Message}");
            }
        }
        //Get Supplier by name
        public Supplier GetSupplierByName(string name)
        {
            try
            {
                return _context.Suppliers.FirstOrDefault(c => c.Name == name);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Database error: {ex.Message}");
            }
        }
        //Add new Supplier
        public void AddSupplier(Supplier supplier)
        {
            try
            {
                _context.Suppliers.Add(supplier);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Database error: {ex.Message}");
            }
        }

        //Update Supplier 
        public void UpdateSupplier(Supplier supplier)
        {
            try
            {
                _context.Suppliers.Update(supplier);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Database error: {ex.Message}");
            }
        }
        //Delete Supplier
        public void DeleteSupplier(int sid)
        {
            try
            {
                var supplier = GetSupplierById(sid);
                if (supplier != null)
                {
                    _context.Suppliers.Remove(supplier);
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
