namespace Caching;

public static class Extensions
{
    public static IServiceCollection AddDecoratedScoped<TService, TDec, TFinal>(
        this IServiceCollection services, object serviceKey)
        where TService : class
        where TDec : class, TService
        where TFinal : class, TService
    {
        services.AddKeyedScoped<TService, TFinal>(serviceKey);
        services.AddScoped<TService, TDec>();
        return services;
    }
}
