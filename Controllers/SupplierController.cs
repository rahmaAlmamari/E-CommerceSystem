using E_CommerceSystem.Models;
using E_CommerceSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet("GetAllSuppliers")]
        public IActionResult GetAllSuppliers()
        {
            var suppliers = _supplierService.GetAllSuppliers();
            return Ok(suppliers);
        }

        [HttpGet("GetSupplierById/{id}")]
        public IActionResult GetSupplierById(int id)
        {
            var supplier = _supplierService.GetSupplierById(id);
            if (supplier == null)

                return NotFound($"Supplier with ID {id}not found.");
            return Ok(supplier);
        }

        [HttpPost("GetSupplierByName/{name}")]
        public IActionResult GetSupplierByName(string name)
        {
            var supplier = _supplierService.GetSupplierByName(name);
            if (supplier == null)
                return NotFound($"Supplier with name '{name}' not found.");
            return Ok(supplier);
        }

        [HttpPut("AddSupplier")]
        public IActionResult AddSupplier([FromBody] Supplier supplier)
        {
            if(supplier==null)
                return BadRequest("Supplier data is required.");

            _supplierService.AddSupplier(supplier);
            return Ok($"Supplier '{supplier.Name}' added successfully with ID {supplier.SupplierId}");

        }

        [HttpPut("UpdateSupplier/{id}")]
        public IActionResult UpdateSupplier(int id, [FromBody] Supplier supplier)
        {
            if (supplier == null || supplier.SupplierId != id)
                return BadRequest("Supplier data is invalid.");
            
            var existingSupplier = _supplierService.GetSupplierById(id);
            if (existingSupplier == null)
                return NotFound($"Supplier with ID {id} not found.");

            //to edit only specific fields
            existingSupplier.Name= supplier.Name;
            existingSupplier.ContactEmail = supplier.ContactEmail;
            existingSupplier.Phone = supplier.Phone;

             _supplierService.UpdateSupplier(existingSupplier);
            //to return the updated supplier
            return Ok($"Supplier '{existingSupplier.Name}' updated successfully.");
        }

        [HttpDelete("DeleteSupplier/{id}")]
        public IActionResult DeleteSupplier(int id)
        {
            var supplier = _supplierService.GetSupplierById(id);
            if (supplier == null)
                return NotFound($"Supplier with ID {id} not found.");

            _supplierService.DeleteSupplier(id);
            return Ok($"Supplier with ID {id} deleted successfully.");
        }


        }
}
