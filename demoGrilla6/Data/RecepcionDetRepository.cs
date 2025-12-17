using Dapper;
using System.Data.SqlClient;
using demoGrilla6.Models;

namespace demoGrilla6.Data
{
    public class RecepcionDetRepository
    {
        private readonly string _connectionString;

        public RecepcionDetRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<RecepcionDet>> GetByPackingSlipIdAsync(string packingSlipId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    select 
	                    vpst.ItemId 'articulo',
	                    PROCUREMENTCATEGORY 'categoriaCompras',
	                    vpst.Ordered 'pedido',
	                    vpst.Qty 'recibido',
	                    vpst.ValueMST 'importe',
	                    vpst.Remain 'cantidadRestante',
                        vpst.Name 'descripcion' 

                    from VENDPACKINGSLIPTRANS vpst
                         left join EcoResCategory erc on erc.CATEGORYHIERARCHY = vpst.PROCUREMENTCATEGORY
                    where 
                    vpst.PACKINGSLIPID ='" + packingSlipId + "'";

                return await conn.QueryAsync<RecepcionDet>(query, new { PackingSlipId = packingSlipId });
            }
        }
    }
}