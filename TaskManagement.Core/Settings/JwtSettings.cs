using TaskManagement.Core.Interfaces;

namespace TaskManagement.Core.Settings
{
    // Concrete settings object bound from configuration and used via IOptions<JwtSettings>
    public class JwtSettings : IJwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}