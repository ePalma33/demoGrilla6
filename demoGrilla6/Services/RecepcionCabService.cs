using demoGrilla6.Data;
using demoGrilla6.Models;

namespace demoGrilla6.Services
{
    public class RecepcionCabService
    {
        private readonly RecepcionCabRepository _repository;

        public RecepcionCabService(RecepcionCabRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<RecepcionCab>> GetPackingSlipsAsync(string proveedor, string empresa)
        {
            return await _repository.GetAllAsync(proveedor,empresa);
        }

        public async Task<IEnumerable<RecepcionCab>> GetPackingSlipByPurchIdAsync(string purchId)
        {
            return await _repository.GetByPurchIdAsync(purchId);
        }

        public async Task<IEnumerable<RecepcionNoFacturada>> GetNumeroNoFacturadoAsync(string username, string empresa  )
        {
            return await _repository.GetNumeroNoFacturadoAsync(username , empresa);
        }
    }
}
