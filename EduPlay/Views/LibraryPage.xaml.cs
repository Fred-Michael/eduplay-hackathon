using EduPlay.ViewModels;

namespace EduPlay.Views;

public partial class LibraryPage : ContentPage
{
    public LibraryPage(LibraryPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}