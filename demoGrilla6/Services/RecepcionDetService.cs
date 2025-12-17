using demoGrilla6.Data;
using demoGrilla6.Models;

namespace demoGrilla6.Services
{
    public class RecepcionDetService
    {
        private readonly RecepcionDetRepository _repository;

        public RecepcionDetService(RecepcionDetRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<RecepcionDet>> GetLinesByPackingSlipIdAsync(string packingSlipId)
        {
            return await _repository.GetByPackingSlipIdAsync(packingSlipId);
        }
    }
}