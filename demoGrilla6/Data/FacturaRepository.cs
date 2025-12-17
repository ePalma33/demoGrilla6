using Dapper;
using System.Data.SqlClient;
using demoGrilla6.Models;

namespace demoGrilla6.Data
{
    public class FacturaRepository
    {
        private readonly string _connectionString;

        public FacturaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Factura>> GetAllAsync(string proveedor)
        {

            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT 
	                                DocumentDate 'fechaDocumento',
	                                DOCNUMINTERNAL 'correlativoInterno',
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
                                "order by DocumentDate desc";

                return await conn.QueryAsync<Factura>(query);  
            }
        }

        public async Task<IEnumerable<Factura>> GetByPurchIdAsync(string purchId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT 
	                                DocumentDate 'fechaDocumento',
	                                DOCNUMINTERNAL 'correlativoInterno',
									TaxDocType 'tipoDocumento',
	                                InvoiceAccount 'cuentaFacturacion',
	                                PurchId 'ordenCompra',
	                                InvoiceId 'factura',
									LedgerVoucher 'asiento',
	                                CurrencyCode 'divisa',
	                                SumTax 'impuesto',
	                                InvoiceAmount 'importeFactura',
	                                InterCompanyCompanyId 'empresa',
	                                InterCompanySalesId 'pedidoVentas',
	                                InterCompanyPosted 'registradoTravesEmpresaVinculada',
	                                DueDate 'fechaVencimiento'
                                from VENDINVOICEJOUR
								where
                                        TaxDocType not in('NL','60NCM','61NCE','961NCE')
									and PurchId	= '" + purchId + "' ";
                                    
                                
                return await conn.QueryAsync<Factura>(query, new { PurchId = purchId });
            }
        }


    }
}