using System.Text;

public class Conversion
{
    public static string GenerateBasicAuthHeader(string username, string password)
    {
        string credentials = $"{username}:{password}";       
        byte[] byteCredentials = Encoding.UTF8.GetBytes(credentials);
        string base64Credentials = Convert.ToBase64String(byteCredentials);
        return $"Basic {base64Credentials}";
    }
}