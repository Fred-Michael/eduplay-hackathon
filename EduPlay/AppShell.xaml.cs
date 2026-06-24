using EduPlay.Views;

namespace EduPlay;
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute(nameof(GamePage), typeof(GamePage));
    }
}
