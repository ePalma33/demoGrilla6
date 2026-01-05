using demoGrilla6.Data;
using demoGrilla6.Models;

namespace demoGrilla6.Services
{
    public class OrdenCompraDetService
    {
        private readonly OrdenCompraCabRepository _repository;

        public OrdenCompraDetService(OrdenCompraCabRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<OrdeCompraCab>> GetPedidoComprasAsync(string proveedor, string empresa)
        {
            var pedidos = await _repository.GetAllAsync(proveedor, empresa);

            //if (!string.IsNullOrEmpty(search))
            //{
            //    pedidos = pedidos.Where(p => p.PurchId.Contains(search, StringComparison.OrdinalIgnoreCase)
            //                              || p.PurchName.Contains(search, StringComparison.OrdinalIgnoreCase));
            //}

            return pedidos;
        }

        public async Task<IEnumerable<Empresa>> GetEmpresasAsync()
        {
            var pedidos = await _repository.GetEmpresasAsync();

            return pedidos;
        }

    }
}