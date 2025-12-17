namespace demoGrilla6.Data
{
    public interface IRoleRepository
    {
        Task<int> EnsureRoleAsync(string name);                 // crea si no existe y retorna Id
        Task<IEnumerable<string>> GetRolesForUserAsync(int userId);
        Task AssignRoleAsync(int userId, string roleName);      // idempotente
        Task RemoveRoleAsync(int userId, string roleName);
    }
}

