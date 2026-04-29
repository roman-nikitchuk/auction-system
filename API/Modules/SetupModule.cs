using FluentValidation;

namespace Api.Modules;

public static class SetupModule
{
    public static void SetupServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(
                    new System.Text.Json.Serialization.JsonStringEnumConverter());
            });
        services.AddRequestValidation();
        services.AddCorsPolicy();
    }

    private static void AddRequestValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<Program>();
    }

    private static void AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
            options.AddPolicy("AllowAll", policy =>
                policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
            ));
    }
}