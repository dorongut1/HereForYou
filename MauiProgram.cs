using CommunityToolkit.Maui;
using HereForYou.Services;
using HereForYou.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HereForYou;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Register services
		RegisterServices(builder.Services);

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

	private static void RegisterServices(IServiceCollection services)
	{
		// Database - Singleton
		var dbPath = Path.Combine(FileSystem.AppDataDirectory, "hereforyou.db3");
		services.AddSingleton<IDatabaseService>(sp => new DatabaseService(dbPath));

		// Other services - Singletons for shared state
		services.AddSingleton<ISettingsService, SettingsService>();
		services.AddSingleton<IAnalyticsService, AnalyticsService>();
		services.AddSingleton<IAlertCoordinatorService, AlertCoordinatorService>();

		// ViewModels - Transient
		services.AddTransient<ViewModels.MainViewModel>();

		// Pages - Transient (new instance each time)
		services.AddTransient<MainPage>();

		// Initialize database on startup
		var serviceProvider = services.BuildServiceProvider();
		var database = serviceProvider.GetRequiredService<IDatabaseService>();
		database.InitializeAsync().Wait();
	}
}
