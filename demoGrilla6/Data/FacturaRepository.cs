using Dapper;
using demoGrilla6.Models;
using System.Data;
using System.Data.SqlClient;


namespace demoGrilla6.Data
{
    public class FacturaRepository
    {
        private readonly string _connectionString;

        public FacturaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


#if DEBUG
        // Código solo para Debug (no queda en Release)
        public async Task<IEnumerable<Factura>> GetAllAsync(string proveedor, string empresa)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT 
                                     DocumentDate 'fechaDocumento',
                                     TaxDocType 'tipoDocumento',
                                     InvoiceAccount 'cuentaFacturacion',
                                     PurchId 'ordenCompra',
                                     InvoiceId 'numFactura',
            	LedgerVoucher 'asiento',
                                     CurrencyCode 'divisa',
                                     SumTax 'impuesto',
                                     InvoiceAmount 'importeFactura',
                                     InterCompanyCompanyId 'empresa',
                                     InterCompanySalesId 'pedidoVentas',
                                     InterCompanyPosted 'registradoTravesEmpresaVinculada',
                                     DueDate 'fechaVencimiento',

            	abs(isnull(
            	(
            	SELECT TOP 1
            		(VT.AmountCur - VT.SettleAmountCur) AS saldoPendiente
            	FROM VendTrans VT
            	WHERE VT.Invoice = InvoiceId
            		AND VT.AccountNum = InvoiceAccount
            		AND VT.AmountCur <> 0
            	ORDER BY ABS(VT.AmountCur) DESC -- la línea con el mayor importe (la factura)
            	)
            	,0)) 'montoPendiente'

                                    from VENDINVOICEJOUR
            where	
            TaxDocType not in('NL','60NCM','61NCE','961NCE')
                        AND InvoiceAccount = '" + proveedor + "' " +
                              " AND DATAAREAID ='" + empresa + "'" +
                                "order by DocumentDate desc";

                return await conn.QueryAsync<Factura>(query);

            }

        }

#else
        public async Task<IEnumerable<Factura>> GetAllAsync(string proveedor, string empresa)
        {

            using (var conn = new SqlConnection(_connectionString))
            { 
                string query = @"
                SELECT
                DocumentDate 'fechaDocumento',
                SUNAT.DESCRIPTION 'tipoDocumento',
                InvoiceAccount 'cuentaFacturacion',
                PurchId 'ordenCompra',
                InvoiceId 'numFactura',
                LedgerVoucher 'asiento',
                CurrencyCode 'divisa',
                SumTax 'impuesto',
                InvoiceAmount 'importeFactura',
                InterCompanyCompanyId 'empresa',
                InterCompanySalesId 'pedidoVentas',
                InterCompanyPosted 'registradoTravesEmpresaVinculada',
                DueDate 'fechaVencimiento',

                abs(isnull(
                (
                SELECT TOP 1
                    (VT.AmountCur - VT.SettleAmountCur) AS saldoPendiente
                FROM VendTrans VT
                WHERE VT.Invoice = InvoiceId

                    AND VT.AccountNum = InvoiceAccount

                    AND VT.AmountCur<> 0
                ORDER BY ABS(VT.AmountCur) DESC-- la línea con el mayor importe(la factura)
                )
                , 0)) 'montoPendiente'

                from VENDINVOICEJOUR vij
                LEFT JOIN LATPECUSTVENDSUNATDOCUMENTTYPES SUNAT
                    ON SUNAT.RECID = VIJ.LATPEVENDSUNATDOCUMENTTYPES

                where
                    SUNAT.SUNATDOCUMENTTYPEID NOT IN('NL', '00', '07', '08')-- Excluye NC y ND
                    AND InvoiceAccount  = '" + proveedor + "' " +
                "order by DocumentDate desc ";

                return await conn.QueryAsync<Factura>(query);

            }


        }

#endif

#if DEBUG
        public async Task<IEnumerable<Factura>> GetByPurchIdAsync(string purchId, string empresa)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                    
                string query = @"
                        SELECT 
                                     DocumentDate 'fechaDocumento',
                                     TaxDocType 'tipoDocumento',
                                     InvoiceAccount 'cuentaFacturacion',
                                     PurchId 'ordenCompra',
                                     InvoiceId 'numFactura',
            	                     LedgerVoucher 'asiento',
                                     CurrencyCode 'divisa',
                                     SumTax 'impuesto',
                                     InvoiceAmount 'importeFactura',
                                     InterCompanyCompanyId 'empresa',
                                     InterCompanySalesId 'pedidoVentas',
                                     InterCompanyPosted 'registradoTravesEmpresaVinculada',
                                     DueDate 'fechaVencimiento',

            	        abs(isnull(
            	        (
            	        SELECT TOP 1
            		        (VT.AmountCur - VT.SettleAmountCur) AS saldoPendiente
            	        FROM VendTrans VT
            	        WHERE VT.Invoice = InvoiceId
            		        AND VT.AccountNum = InvoiceAccount
            		        AND VT.AmountCur <> 0
            	        ORDER BY ABS(VT.AmountCur) DESC -- la línea con el mayor importe (la factura)
            	        )
            	        ,0)) 'montoPendiente'

                        from VENDINVOICEJOUR
                        where	
                        PurchId	= '" + purchId + "' " +
                      "order by DocumentDate desc ";
                                    
                                
                return await conn.QueryAsync<Factura>(query, new { PurchId = purchId });
            }
        }

#else 
        public async Task<IEnumerable<Factura>> GetByPurchIdAsync(string purchId, string empresa)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                    
                string query = @"
                        SELECT 
                                     DocumentDate 'fechaDocumento',
                                     SUNAT.DESCRIPTION 'tipoDocumento',
                                     InvoiceAccount 'cuentaFacturacion',
                                     PurchId 'ordenCompra',
                                     InvoiceId 'numFactura',
            	                     LedgerVoucher 'asiento',
                                     CurrencyCode 'divisa',
                                     SumTax 'impuesto',
                                     InvoiceAmount 'importeFactura',
                                     InterCompanyCompanyId 'empresa',
                                     InterCompanySalesId 'pedidoVentas',
                                     InterCompanyPosted 'registradoTravesEmpresaVinculada',
                                     DueDate 'fechaVencimiento',

            	        abs(isnull(
            	        (
            	        SELECT TOP 1
            		        (VT.AmountCur - VT.SettleAmountCur) AS saldoPendiente
            	        FROM VendTrans VT
            	        WHERE VT.Invoice = InvoiceId
            		        AND VT.AccountNum = InvoiceAccount
            		        AND VT.AmountCur <> 0
            	        ORDER BY ABS(VT.AmountCur) DESC -- la línea con el mayor importe (la factura)
            	        )
            	        ,0)) 'montoPendiente'

                        from VENDINVOICEJOUR VIJ
                        LEFT JOIN LATPECUSTVENDSUNATDOCUMENTTYPES SUNAT
                        ON SUNAT.RECID = VIJ.LATPEVENDSUNATDOCUMENTTYPES
                        where	
                        VIJ.PurchId	= '" + purchId + "' " +
                      "order by DocumentDate desc ";
                                    
                                
                return await conn.QueryAsync<Factura>(query, new { PurchId = purchId });
            }
        }
#endif



    }


}

