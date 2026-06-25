namespace QubeFin.Core.Security;

public static class OtpGenerator
{
    public static string Generate(int otpLength, bool isAlphaNumeric)
    {
        string uppercaseAlphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string lowercasealphabets = "abcdefghijklmnopqrstuvwxyz";
        string numbers = "1234567890";

        var characters = numbers;
        if (isAlphaNumeric)
        {
            characters += uppercaseAlphabets + lowercasealphabets + numbers;
        }

        var otp = string.Empty;
        for (int i = 0; i < otpLength; i++)
        {
            string? character;
            do
            {
                var index = new Random().Next(0, characters.Length);
                character = characters.ToCharArray()[index].ToString();
            } while (otp.IndexOf(character) != -1);
            otp += character;
        }

        return otp;
    }
}