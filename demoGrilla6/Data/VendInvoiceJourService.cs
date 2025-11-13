using demoGrilla6.Data;
using demoGrilla6.Models;

namespace demoGrilla6.Services
{
    public class VendInvoiceJourService
    {
        private readonly VendInvoiceJourRepository _repository;

        public VendInvoiceJourService(VendInvoiceJourRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<VendInvoiceJour>> GetInvoicesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<VendInvoiceJour>> GetInvoicesByPurchIdAsync(string purchId)
        {
            return await _repository.GetByPurchIdAsync(purchId);
        }
    }
}