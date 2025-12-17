using demoGrilla6.Data;
using demoGrilla6.Models;
using Microsoft.AspNetCore.Mvc;

namespace demoGrilla6.Services
{
    public class UsuarioService
    {

        private readonly IUserRepository _repository;

        public UsuarioService(IUserRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<User>> GetAllAsync(string rutProveedor)
            => _repository.GetAllAsync(rutProveedor);

        public Task<JsonResult> OnGetActivarAsync( int idUsuario, bool activar)
        {
            return  _repository.OnGetActivarAsync(idUsuario, activar);
        }


    }
}
