using demoGrilla6.Data;
using demoGrilla6.Models;

namespace demoGrilla6.Services
{
    public class PagoFacturaService
    {
        private readonly PagoFacturaRepository _repository;

        public PagoFacturaService(PagoFacturaRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PagoFactura>> GetPagoFacturaAsync(string idFactura, string proveedor, string voucher)
        {
            return await _repository.GetPagoFacturaAsync(idFactura, proveedor, voucher);
        }

        public async Task<IEnumerable<PagoFacturaTotalPendiente>> GetTotalPendienteAsync(string idFactura, string proveedor)
        {
            return await _repository.GetTotalPendienteAsync(idFactura, proveedor);
        }

        public async Task<IEnumerable<UltimoPago>> GetUltimoPagoAsync( string proveedor, string empresa)
        {
            return await _repository.GetUltimoPagoAsync( proveedor, empresa);
        }

    }

}
