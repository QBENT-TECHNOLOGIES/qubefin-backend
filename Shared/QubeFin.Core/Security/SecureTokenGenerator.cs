using System.Security.Cryptography;
using System.Text;

namespace QubeFin.Core.Security;

public static class SecureTokenGenerator
{
    public static string Generate(int length)
    {
        const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        StringBuilder sb = new();
        byte[] uintBuffer = new byte[sizeof(uint)];
        while (length-- > 0)
        {
            RandomNumberGenerator.Fill(uintBuffer);
            uint num = BitConverter.ToUInt32(uintBuffer, 0);
            sb.Append(validChars[(int)(num % (uint)validChars.Length)]);
        }
        return sb.ToString();
    }
}

