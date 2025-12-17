using System;
using BCrypt.Net;

class Program
{
    static void Main()
    {
        string password = "Admin123!"; // la contraseña que quieres
        string hash = BCrypt.Net.BCrypt.HashPassword(password);
        Console.WriteLine("Hash para SQL:");
        Console.WriteLine(hash);
    }
}
