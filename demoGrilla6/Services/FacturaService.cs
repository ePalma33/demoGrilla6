using demoGrilla6.Data;
using demoGrilla6.Models;

namespace demoGrilla6.Services
{
    public class FacturaService
    {
        private readonly FacturaRepository _repository;

        public FacturaService(FacturaRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Factura>> GetInvoicesAsync(string proveedor)
        {
            return await _repository.GetAllAsync(proveedor);
        }

        public async Task<IEnumerable<Factura>> GetInvoicesByPurchIdAsync(string purchId)
        {
            return await _repository.GetByPurchIdAsync(purchId);
        }

    }
}