
// Data/UserRepository.cs
using Dapper;
using demoGrilla6.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;

namespace demoGrilla6.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _db;

        public UserRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            const string sql = @"
                SELECT TOP 1 [Id], [UserName], [Email], [PasswordHash], [IsActive], [CreatedAt], Proveedor
                FROM [Users]
                WHERE [UserName] = @userName AND [IsActive] = 1";
            return await _db.QueryFirstOrDefaultAsync<User>(sql, new { userName });
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT TOP 1 [Id], [UserName], [Email], [PasswordHash], [IsActive], [CreatedAt]
                FROM [Users]
                WHERE [Id] = @id";
            return await _db.QueryFirstOrDefaultAsync<User>(sql, new { id });
        }

        public async Task<int> CreateAsync(User user)
        {

            const string sp = "dbo.sp_ingresarusuario";

            var p = new DynamicParameters();
            p.Add("@UserName", user.UserName, DbType.String, size: 100);
            p.Add("@Email", user.Email, DbType.String, size: 100);
            p.Add("@PasswordHash", user.PasswordHash, DbType.String, size: 500);
            p.Add("@IsActive", user.IsActive ? 1 : 0, DbType.Int32);
            p.Add("@EsAdmin", user.EsAdmin, DbType.Boolean);
            p.Add("@Proveedor", user.Proveedor, DbType.String, size: 100);
            p.Add("@Nombre", user.Nombre, DbType.String, size: 50);
            p.Add("@Apellido", user.Apellido, DbType.String, size: 50);

            // OUTPUT param para el nuevo Id:
            p.Add("@NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _db.ExecuteAsync(sp, p, commandType: CommandType.StoredProcedure);

            var newId = p.Get<int>("@NewId");
            user.Id = newId; // opcional: conservar el Id en el objeto
            user.CreatedAt = DateTime.UtcNow; // si quieres mantenerlo coherente en memoria

            return newId;

        }

        public async Task<IEnumerable<User>> GetAllAsync(string rutProveedor)
        {
            const string sql = @"
                SELECT [Id], [UserName], [Email], [PasswordHash], [IsActive], [CreatedAt], Proveedor, Nombre, Apellido,
                        CASE 
                        WHEN ur.RoleId = 1 THEN 1 
                        ELSE 0 
                    END AS EsAdmin

                FROM [Users] U
                    inner join UserRoles ur on ur.UserId = U.Id
                WHERE Proveedor = @rutProveedor";
            return await _db.QueryAsync<User>(sql, new { rutProveedor });
        }

        public async Task<JsonResult> OnGetActivarAsync( int idUsuario, bool activar)
        {
            const string sql = @"
                UPDATE Users SET IsActive = 1 where Id=@idUsuario";

            var filas = await _db.ExecuteAsync(sql, new { idUsuario, activar });

            // Devuelve éxito si se actualizó al menos una fila
            return new JsonResult(new { success = filas > 0, affected = filas });

        }

    }
}
