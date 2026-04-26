using Infrastructure;

namespace Api.Modules;

public static class DbModules
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initialiser = scope.ServiceProvider
            .GetRequiredService<AppDbContextInitializer>();
        await initialiser.InitialiseAsync();
    }
}