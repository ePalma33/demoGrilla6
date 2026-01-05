using Dapper;
using System.Data.SqlClient;
using demoGrilla6.Models;

namespace demoGrilla6.Data
{
    public class OrdenCompraCabRepository
    {
        private readonly string _connectionString;

        public OrdenCompraCabRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<OrdeCompraCab>> GetAllAsync(string proveedor, string empresa)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                
                string filtroProveedor = "";

                if (proveedor != "admin")
                {
                    filtroProveedor = " pt.ORDERACCOUNT = '" + proveedor + "'";
                }

                string query = @"SELECT pt.PURCHID, pt.PURCHNAME, pt.ORDERACCOUNT, pt.CURRENCYCODE, pt.PURCHSTATUS, " + 
                                "CASE pt.PurchStatus " +
                                    "WHEN 0 THEN 'Sin' " +
                                    "WHEN 1 THEN 'Pedido abierto' " +
                                    "WHEN 2 THEN 'Recibido' " +
                                    "WHEN 3 THEN 'Facturado' " +
                                    "WHEN 4 THEN 'Cancelado' " +
                                "ELSE 'Desconocido' " +
                                "END AS PURCHSTATUSTEXT ," +

                                "CASE pt.DocumentState " +
                                    "WHEN 0 THEN 'Borrador' " +
                                    "WHEN 10 THEN 'En revision' " +
                                    "WHEN 20 THEN 'Rechazado' " +
                                    "WHEN 30 THEN 'Aprobado' " +
                                    "WHEN 35 THEN 'En revisión externa' " +
                                    "WHEN 50 THEN 'Finalizada' " +
                                    "WHEN 40 THEN 'Confirmado' " +
                                    "ELSE 'Desconocido' " +
                                "END AS DOCUMENTSTATETEXT ," +
                                "pts.TOTALAMOUNT TotalAmount " +

                                "FROM PURCHTABLE pt " +
                                "left join PurchTotalsSummary pts on pts.PURCHID = pt.PURCHID " +
                                "where " +
                                 filtroProveedor +
                               " AND pt.DATAAREAID = '" + empresa + "'" +
                               " Order By PURCHID desc";

                return await conn.QueryAsync<OrdeCompraCab>(query);
            }
        }

        public async Task<IEnumerable<Empresa>> GetEmpresasAsync()
        {
            using (var conn = new SqlConnection(_connectionString))
            {

                string query = @"
                        SELECT distinct p.NAMEALIAS Nombre , d.ID Codigo, P.COREGNUM Rut
                        FROM DATAAREA d
                        INNER JOIN DIRPARTYTABLE p ON d.NAME = p.NAME  
                        where
                        d.ID in (select distinct DATAAREAid
                        from PURCHTABLE)
                        and P.COREGNUM  is not null

                ";

                return await conn.QueryAsync<Empresa>(query);
            }
        }


    }
}