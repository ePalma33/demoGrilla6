using demoGrilla6.Data;
using demoGrilla6.Models;

namespace demoGrilla6.Services
{
    public class OrdenCompraCabService
    {
        private readonly OrdenCompraDetRepository _repository;

        public OrdenCompraCabService(OrdenCompraDetRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<OrdenCompraDet>> GetLinesAsync(string purchId)
        {
            return await _repository.GetByPurchIdAsync(purchId);
        }
    }
}