using Dapper;
using System.Data.SqlClient;
using demoGrilla6.Models;

namespace demoGrilla6.Data
{
    public class VendInvoiceJourRepository
    {
        private readonly string _connectionString;

        public VendInvoiceJourRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<VendInvoiceJour>> GetAllAsync()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT top 100 VENDGROUP, PURCHID, ORDERACCOUNT, INVOICEACCOUNT, INVOICEID, INVOICEDATE, DUEDATE, INVOICEAMOUNT, CURRENCYCODE, DESCRIPTION
                                 FROM VENDINVOICEJOUR";
                return await conn.QueryAsync<VendInvoiceJour>(query);
            }
        }

        public async Task<IEnumerable<VendInvoiceJour>> GetByPurchIdAsync(string purchId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT VENDGROUP, PURCHID, ORDERACCOUNT, INVOICEACCOUNT, INVOICEID, INVOICEDATE, DUEDATE, INVOICEAMOUNT, CURRENCYCODE, DESCRIPTION
                                 FROM VENDINVOICEJOUR
                                 WHERE PURCHID = @PurchId";
                return await conn.QueryAsync<VendInvoiceJour>(query, new { PurchId = purchId });
            }
        }
    }
}