
// Data/RoleRepository.cs
using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace demoGrilla6.Data
{
    public class RoleRepository : IRoleRepository
    {
        private readonly string _cs;
        public RoleRepository(string connectionString) => _cs = connectionString;

        private IDbConnection OpenConn()
        {
            var conn = new SqlConnection(_cs);
            conn.Open();
            return conn;
        }

        public async Task<int> EnsureRoleAsync(string name)
        {
            using var db = OpenConn();

            var roleId = await db.ExecuteScalarAsync<int?>(
                "SELECT [Id] FROM [Roles] WHERE [Name] = @name", new { name });

            if (roleId is not null) return roleId.Value;

            return await db.ExecuteScalarAsync<int>(
                "INSERT INTO [Roles] ([Name]) VALUES (@name); SELECT CAST(SCOPE_IDENTITY() AS int);",
                new { name });
        }

        public async Task<IEnumerable<string>> GetRolesForUserAsync(int userId)
        {
            using var db = OpenConn();
            const string sql = @"
                SELECT r.[Name]
                FROM [UserRoles] ur
                INNER JOIN [Roles] r ON r.[Id] = ur.[RoleId]
                WHERE ur.[UserId] = @userId";
            return await db.QueryAsync<string>(sql, new { userId });
        }

        public async Task AssignRoleAsync(int userId, string roleName)
        {
            using var db = OpenConn();

            // Obtener/crear rol
            var roleId = await db.ExecuteScalarAsync<int?>(
                "SELECT [Id] FROM [Roles] WHERE [Name] = @roleName", new { roleName });
            if (roleId is null)
            {
                roleId = await db.ExecuteScalarAsync<int>(
                    "INSERT INTO [Roles] ([Name]) VALUES (@roleName); SELECT CAST(SCOPE_IDENTITY() AS int);",
                    new { roleName });
            }

            // Idempotente: insert solo si no existe
            const string upsert = @"
                IF NOT EXISTS (SELECT 1 FROM [UserRoles] WHERE [UserId] = @userId AND [RoleId] = @roleId)
                INSERT INTO [UserRoles] ([UserId], [RoleId]) VALUES (@userId, @roleId);";
            await db.ExecuteAsync(upsert, new { userId, roleId });
        }

        public async Task RemoveRoleAsync(int userId, string roleName)
        {
            using var db = OpenConn();
            const string del = @"
                DELETE ur
                FROM [UserRoles] ur
                INNER JOIN [Roles] r ON r.[Id] = ur.[RoleId]
                WHERE ur.[UserId] = @userId AND r.[Name] = @roleName;";
            await db.ExecuteAsync(del, new { userId, roleName });
        }
    }
}
