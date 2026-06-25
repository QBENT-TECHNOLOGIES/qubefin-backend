namespace QubeFin.Core.Settings;

public record JwtSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SigningSecret { get; set; } = string.Empty;
}
