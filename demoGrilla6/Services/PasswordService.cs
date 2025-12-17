namespace demoGrilla6.Services
{

    public interface IPasswordService
    {
        string Hash(string password);
        bool Verify(string password, string hash);
    }


    public class BcryptPasswordService : IPasswordService
    {
        public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);
        public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
    }

}
