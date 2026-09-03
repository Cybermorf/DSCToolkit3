using DSC.Toolkit.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DSC.Toolkit;

public sealed partial class MainWindow : Window
{
    public MainViewModel ViewModel { get; } = new();

    public MainWindow()
    {
        InitializeComponent();
        Navigation.SelectedItem = Navigation.MenuItems[0];
    }

    private void Navigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItemContainer?.Tag is not string id) return;
        var module = ViewModel.Modules.FirstOrDefault(candidate => candidate.Id == id);
        if (module is not null) ViewModel.SelectedModule = module;
    }

    private void RecordList_SelectionChanged(object sender, SelectionChangedEventArgs args) => Editor.Record = ViewModel.SelectedRecord;
}
