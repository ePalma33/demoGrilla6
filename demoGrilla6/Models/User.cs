namespace demoGrilla6.Models
{

    // Models/User.cs
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = default!;
        public string? Email { get; set; }
        public string PasswordHash { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Proveedor { get; set; }
        public bool EsAdmin { get; set; } = false;
        public string Nombre { get; set; }
        public string Apellido { get; set; }
    }

}
