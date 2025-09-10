using Microsoft.Extensions.Configuration;

namespace AutoStarter.Configuration;

public static class ConfigLoader
{
    public static T Load<T>(IConfiguration config, string sectionName) 
        where T : class =>
        config.GetSection(sectionName).Get<T>() 
        ?? throw new InvalidOperationException($"Configuration section '{sectionName}' is missing.");
}