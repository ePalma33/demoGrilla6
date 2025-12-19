
// Data/IUserRepository.cs
using demoGrilla6.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace demoGrilla6.Data
{
    public interface IUserRepository
    {
        Task<User?> GetByUserNameAsync(string userName);
        Task<User?> GetByUserEmailAsync(string userName);
        Task<User?> GetByIdAsync(int id);
        Task<int> CreateAsync(User user);
        Task<IEnumerable<User>> GetAllAsync(string rutProveedor);
        Task<JsonResult> OnGetActivarAsync( int idUsuario, bool activar);
        Task<JsonResult> OnPostGuardarAsync(User usuario);
    }
}
