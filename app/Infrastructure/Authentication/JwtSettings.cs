using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Authentication;

public class JwtSettings
{
    public const string SectionName = "Jwt";
    
    public string Audience { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public int ExpirationInMinutes { get; init; }
    public string Key { get; init; } = string.Empty;
}
