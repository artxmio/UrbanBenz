using System.Windows;
using System.Windows.Controls;

namespace Mercedes.Pages;

public partial class HelpPage : Page
{
    public HelpPage()
    {
        InitializeComponent();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.GoBack();
    }
}