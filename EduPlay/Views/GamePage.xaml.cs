using EduPlay.ViewModels;

namespace EduPlay.Views;

public partial class GamePage : ContentPage
{
    public GamePage(GamePageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void OnWebViewNavigated(object? sender, WebNavigatedEventArgs e)
    {
        if (BindingContext is GamePageViewModel vm)
        {
            vm.IsWebViewLoading = false;
        }

        // Enable JavaScript explicitly (required on some platforms)
        if (sender is WebView webView)
        {
            webView.Eval("console.log('EduPlay game loaded')");
        }
    }
}