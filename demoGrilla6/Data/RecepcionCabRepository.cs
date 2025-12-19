using Dapper;
using System.Data.SqlClient;
using demoGrilla6.Models;

namespace demoGrilla6.Data
{
    public class RecepcionCabRepository
    {
        private readonly string _connectionString;

        public RecepcionCabRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<RecepcionCab>> GetAllAsync(string proveedor)
        {

            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    
                    SELECT 
                        vpsj.InvoiceAccount AS ctaFacturacion,
                        vpsj.PurchId AS pedidoCompra,
                        vpsj.PackingSlipId AS recepcionProducto,
                        vpsj.DeliveryDate AS fecha,
                        ISNULL(vij.INVOICEID,'') AS factura,
                        CASE WHEN vij.INVOICEID IS NULL THEN 0 ELSE 1 END AS estaFacturado
                    FROM VendPackingSlipJour vpsj
                    LEFT JOIN (
                        SELECT PackingSlipId, MIN(SOURCEDOCUMENTLINE) AS SOURCEDOCUMENTLINE
                        FROM VendPackingSlipTrans
                        GROUP BY PackingSlipId
                    ) vpst ON vpst.PackingSlipId = vpsj.PackingSlipId
                    LEFT JOIN VendInvoicePackingSlipQuantityMatch vipsqm 
                        ON vipsqm.PACKINGSLIPSOURCEDOCUMENTLINE = vpst.SOURCEDOCUMENTLINE
                    LEFT JOIN VendInvoiceTrans vit 
                           ON vit.SOURCEDOCUMENTLINE = vipsqm.INVOICESOURCEDOCUMENTLINE
                    LEFT JOIN VendInvoiceJour vij 
                        ON vij.INVOICEID = vit.INVOICEID 
                        AND vij.INVOICEACCOUNT = vpsj.InvoiceAccount
                    WHERE 
                        vpsj.InvoiceAccount = '" + proveedor + "'" +
                    "Order By estaFacturado";



                return await conn.QueryAsync<RecepcionCab>(query);
            }
        }

        public async Task<IEnumerable<RecepcionCab>> GetByPurchIdAsync(string purchId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT 
                        vpsj.InvoiceAccount 'ctaFacturacion',
                        vpsj.PurchId 'pedidoCompra',
                        vpsj.PackingSlipId 'recepcionProducto',
                        vpsj.DeliveryDate 'fecha',
                        isnull(vij.INVOICEID,'') 'factura' ,
                        CASE 
                            WHEN vij.INVOICEID IS NULL THEN 0 
                            ELSE 1 
                        END AS estaFacturado
                    FROM VendPackingSlipJour vpsj
                    OUTER APPLY (
                        SELECT TOP 1 SOURCEDOCUMENTLINE
                        FROM VendPackingSlipTrans tt
                        WHERE tt.PackingSlipId = vpsj.PackingSlipId
                    ) vpst
                    LEFT JOIN VendInvoicePackingSlipQuantityMatch vipsqm ON vipsqm.PACKINGSLIPSOURCEDOCUMENTLINE = vpst.SOURCEDOCUMENTLINE
                    LEFT JOIN VendInvoiceTrans vit on vit.SOURCEDOCUMENTLINE = vipsqm.INVOICESOURCEDOCUMENTLINE
                    LEFT JOIN VendInvoiceJour  vij on vij.INVOICEID = vit.INVOICEID and vij.INVOICEACCOUNT = vpsj.InvoiceAccount
                    where 
                        vpsj.PurchId = '" + purchId + "'";
                    

                return await conn.QueryAsync<RecepcionCab>(query, new { PurchId = purchId });
            }
        }

        public async Task<IEnumerable<RecepcionNoFacturada>> GetNumeroNoFacturadoAsync(string proveedor)
        {

            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"

                    SELECT COUNT(*) AS numero
                    FROM VendPackingSlipJour AS vpsj
                    WHERE vpsj.InvoiceAccount = @Proveedor
                      AND NOT EXISTS (
                          SELECT 1
                          FROM VendPackingSlipTrans AS tt
                          JOIN VendInvoicePackingSlipQuantityMatch AS vipsqm
                            ON vipsqm.PACKINGSLIPSOURCEDOCUMENTLINE = tt.SOURCEDOCUMENTLINE
                          JOIN VendInvoiceTrans AS vit
                            ON vit.SOURCEDOCUMENTLINE = vipsqm.INVOICESOURCEDOCUMENTLINE
                          JOIN VendInvoiceJour AS vij
                            ON vij.INVOICEID = vit.INVOICEID
                           AND vij.INVOICEACCOUNT = vpsj.InvoiceAccount
                          WHERE tt.PackingSlipId = vpsj.PackingSlipId
                      )";

                return await conn.QueryAsync<RecepcionNoFacturada>(query, new { Proveedor = proveedor });
            }
        }
    }
}