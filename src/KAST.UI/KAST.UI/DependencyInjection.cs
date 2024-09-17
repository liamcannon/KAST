using KAST.Server.UI.Middlewares;
using KAST.UI.Components;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MudBlazor.Services;

namespace KAST.UI;

/// <summary>
/// Dependency Injection configuration for KAST UI
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddKastUI(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddHubOptions(options => options.MaximumReceiveMessageSize = 64 * 1024);

        services.AddCascadingAuthenticationState();
        services.AddMudServices(config =>
        {
            MudGlobal.InputDefaults.ShrinkLabel = true;
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 3000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            config.SnackbarConfiguration.PreventDuplicates = false;
        });

        services.AddMudPopoverService();
        services.AddMudBlazorSnackbar();
        services.AddMudBlazorDialog();

        services.AddControllers();

        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddProblemDetails();
        services.AddHealthChecks();

        services.AddHttpContextAccessor();

        return services;
    }

    public static WebApplication ConfigureKastServer(this WebApplication app, IConfiguration env)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error", true);
            app.UseHsts();
        }

        app.UseStatusCodePagesWithRedirects("/404");
        app.MapHealthChecks("/health");

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseExceptionHandler();
        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

        app.MapAdditionalIdentityEndpoints();
        app.UseForwardedHeaders();
        app.UseWebSockets(new WebSocketOptions()
        { // We obviously need this
            KeepAliveInterval = TimeSpan.FromSeconds(30), // Just in case
        });

        return app;
    }
}
