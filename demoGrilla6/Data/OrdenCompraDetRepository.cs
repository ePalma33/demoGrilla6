using Dapper;
using System.Data.SqlClient;
using demoGrilla6.Models;

namespace demoGrilla6.Data
{
    public class OrdenCompraDetRepository
    {
        private readonly string _connectionString;

        public OrdenCompraDetRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<OrdenCompraDet>> GetByPurchIdAsync(string purchId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT PURCHID, ITEMID, NAME, QTYORDERED, PURCHPRICE, LINEAMOUNT, DELIVERYDATE, purchqty
                                 FROM PURCHLINE
                                 WHERE PURCHID = @PurchId";
                return await conn.QueryAsync<OrdenCompraDet>(query, new { PurchId = purchId });
            }
        }
    }
}