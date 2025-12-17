namespace TaskManagement.Core.Interfaces
{
    public interface IJwtSettings
    {
        string Key { get; }
        string Issuer { get; }
        string Audience { get; }
    }
}