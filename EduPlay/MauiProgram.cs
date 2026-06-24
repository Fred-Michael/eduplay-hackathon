using CommunityToolkit.Maui;
using EduPlay.Services;
using EduPlay.ViewModels;
using EduPlay.Views;

namespace EduPlay
{
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
            builder.Services.AddSingleton<GameGeneratorService>();
            builder.Services.AddSingleton<GameLibraryService>();

            // Register view models
            builder.Services.AddTransient<BuildPageViewModel>();
            builder.Services.AddTransient<GamePageViewModel>();
            builder.Services.AddTransient<LibraryPageViewModel>();

            // Register pages
            builder.Services.AddTransient<BuildPage>();
            builder.Services.AddTransient<GamePage>();
            builder.Services.AddTransient<LibraryPage>();

            return builder.Build();
        }
    }
}
