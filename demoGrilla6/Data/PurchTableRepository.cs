using Dapper;
using System.Data.SqlClient;
using demoGrilla6.Models;

namespace demoGrilla6.Data
{
    public class PurchTableRepository
    {
        private readonly string _connectionString;

        public PurchTableRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<PurchTable>> GetAllAsync()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT PURCHID, PURCHNAME, ORDERACCOUNT, CURRENCYCODE, DLVMODE, PURCHSTATUS, CREATEDDATETIME FROM PURCHTABLE";
                return await conn.QueryAsync<PurchTable>(query);
            }
        }
    }
}