using System.Windows;
using System.Windows.Controls;

namespace Mercedes.Pages;

public partial class AboutPage : Page
{
    public AboutPage()
    {
        InitializeComponent();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.GoBack();
    }
}