using demoGrilla6.Data;
using demoGrilla6.Models;

namespace demoGrilla6.Services
{
    public class PedidoCompraService
    {
        private readonly PurchTableRepository _repository;

        public PedidoCompraService(PurchTableRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PurchTable>> GetPedidoComprasAsync(string? search)
        {
            var pedidos = await _repository.GetAllAsync();

            //if (!string.IsNullOrEmpty(search))
            //{
            //    pedidos = pedidos.Where(p => p.PurchId.Contains(search, StringComparison.OrdinalIgnoreCase)
            //                              || p.PurchName.Contains(search, StringComparison.OrdinalIgnoreCase));
            //}

            return pedidos;
        }
    }
}