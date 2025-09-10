using System.ComponentModel.DataAnnotations;

namespace AutoStarter.Configuration.Configs;

public sealed class DbSettings
{
    [Required] 
    public required string ConnectionString { get; init; }
}