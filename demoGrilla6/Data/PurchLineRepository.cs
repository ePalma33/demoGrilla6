using Dapper;
using System.Data.SqlClient;
using demoGrilla6.Models;

namespace demoGrilla6.Data
{
    public class PurchLineRepository
    {
        private readonly string _connectionString;

        public PurchLineRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<PurchLine>> GetByPurchIdAsync(string purchId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT PURCHID, ITEMID, NAME, QTYORDERED, PURCHPRICE, LINEAMOUNT, DELIVERYDATE
                                 FROM PURCHLINE
                                 WHERE PURCHID = @PurchId";
                return await conn.QueryAsync<PurchLine>(query, new { PurchId = purchId });
            }
        }
    }
}