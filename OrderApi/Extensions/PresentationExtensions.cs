using Carter;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OrderApi.Database;
using OrderApi.Middlewares;
using OrderApi.Pipelines;
using System.Reflection;

namespace OrderApi.Extensions;

public static class PresentationExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        Assembly assembly = typeof(Program).Assembly;

        services.AddCarter();

        services.AddExceptionHandler<CustomGlobalExceptionHandler>();

        services.AddProblemDetails();

        services.AddDbContext<OrderDbContext>();

        services.AddValidatorsFromAssembly(assembly);

        services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssembly(assembly);
            x.AddOpenBehavior(typeof(TransactionPipeline<,>));
            x.AddOpenBehavior(typeof(ValidatorPipeline<,>)); 
        });

        return services;
    }
    public static WebApplication MapPresentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            dbContext.Database.Migrate();
        }

        app.UseExceptionHandler();

        app.MapCarter();

        return app;
    }
}
