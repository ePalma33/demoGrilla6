using Dapper;
using System.Data.SqlClient;
using demoGrilla6.Models;

namespace demoGrilla6.Data
{
    public class PagoFacturaRepository
    {
        private readonly string _connectionString;

        public PagoFacturaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<PagoFactura>> GetPagoFacturaAsync(string idFactura, string proveedor, string voucher)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"
                                SELECT
                                    VT.INVOICE numeroFactura,
                                    VS.TransDate   fechaPago,
                                    VS.SettleAmountCur  montoPago
                           
                                FROM 
                                VendTrans VT 
                                INNER JOIN VendSettlement VS ON VS.TransRecId = VT.RecId

                                WHERE VT.AccountNum = @proveedor
                                AND VT.INVOICE = @idFactura
                                AND VT.VOUCHER = @voucher";

                return await conn.QueryAsync<PagoFactura>(query, new { IdFactura = idFactura, Proveedor = proveedor, Voucher = voucher });
            }
        }


        //SELECT
        //    VIJ.InvoiceId numeroFactura,
        //    MIN(VIJ.InvoiceDate) fechaFactura,
        //    VT.AccountNum proveedor,
        //    VT.Voucher numeroPago,
        //    VT.TransDate fechaPago,
        //    VT.AmountMST montoPago
        //                           -- VT2.VOUCHER
        //FROM VendInvoiceJour VIJ

        //INNER JOIN VendTrans VT ON VT.Invoice = VIJ.InvoiceId

        //INNER JOIN VendSettlement VS ON VS.TransRecId = VT.RecId

        //INNER JOIN VendTrans VT2 ON VT2.RecId = VS.OffsetRecId

        //WHERE VT.AccountNum = @proveedor

        //AND VIJ.InvoiceId = @idFactura

        //and VT.AmountMST< 0

        //and VT2.VOUCHER<> vt.VOUCHER
        //GROUP BY VIJ.InvoiceId, VT.AccountNum, VT.Voucher, VT.TransDate, VT.AmountMST
        //ORDER BY VIJ.InvoiceId


        public async Task<IEnumerable<PagoFacturaTotalPendiente>> GetTotalPendienteAsync(string idFactura, string proveedor)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"
                                SELECT TOP 1
                                    ABS(VT.AmountCur - VT.SettleAmountCur) AS saldoPendiente
                                FROM VendTrans VT
                                WHERE VT.Invoice = @IdFactura
                                  AND VT.AccountNum = @Proveedor
                                  AND VT.AmountCur <> 0
                                ORDER BY ABS(VT.AmountCur) DESC; -- la línea con el mayor importe (la factura)
                                ";
                return await conn.QueryAsync<PagoFacturaTotalPendiente>(query, new { IdFactura = idFactura, Proveedor = proveedor });
            }
        }

        public async Task<IEnumerable<UltimoPago>> GetUltimoPagoAsync(string proveedor)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"
                                SELECT TOP 1
                                    VT2.TransDate AS fechaPago,
                                    abs(VT2.AmountMST) AS montoPago,
                                    VT2.CurrencyCode AS moneda,
                                    VT2.Voucher AS comprobante,
                                    VT2.Txt AS descripcion
                                FROM VendTrans VT
                                INNER JOIN VendSettlement VS ON VS.TransRecId = VT.RecId
                                INNER JOIN VendTrans VT2 ON VT2.RecId = VS.OffsetRecId
                                WHERE VT.AccountNum = @Proveedor
                                ORDER BY VT2.TransDate DESC;
                                ";
                return await conn.QueryAsync<UltimoPago>(query, new { Proveedor = proveedor });
            }
        }


    }
}
