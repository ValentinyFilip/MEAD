using System.Security.Cryptography;
using System.Text;

namespace ApiService.Common;

public static class HashHelper
{
    public static string Sha256(string input)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var inputHash = SHA256.HashData(inputBytes);
        return Convert.ToHexString(inputHash);
    }
}