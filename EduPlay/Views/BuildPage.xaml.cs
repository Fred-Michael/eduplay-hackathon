using EduPlay.ViewModels;

namespace EduPlay.Views;
public partial class BuildPage : ContentPage
{
    public BuildPage(BuildPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
