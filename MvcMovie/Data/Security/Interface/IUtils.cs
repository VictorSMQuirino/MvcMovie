namespace MvcMovie.Data.Security.Interface;

public interface IUtils
{
    public string HashPassword(string password);
    public string GenerateJwtToken(string email, string role);
}
