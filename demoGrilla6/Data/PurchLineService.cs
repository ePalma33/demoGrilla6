using demoGrilla6.Data;
using demoGrilla6.Models;

namespace demoGrilla6.Services
{
    public class PurchLineService
    {
        private readonly PurchLineRepository _repository;

        public PurchLineService(PurchLineRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PurchLine>> GetLinesAsync(string purchId)
        {
            return await _repository.GetByPurchIdAsync(purchId);
        }
    }
}