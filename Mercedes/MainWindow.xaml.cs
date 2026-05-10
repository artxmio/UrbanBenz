using System.Windows;
using Mercedes.Pages;

namespace Mercedes
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var mainPage = new MainPage();

            frame.Navigate(mainPage);
        }
    }
}