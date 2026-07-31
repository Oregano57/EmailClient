using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Google.Apis.Gmail.v1;

namespace EmailClient;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private GmailClient _gmailClient = new GmailClient();
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void SignInWithGoogleButton_Click(object sender, RoutedEventArgs e)
    {
        await _gmailClient.AuthenticateAsync();
        LoginGrid.Visibility = Visibility.Collapsed;
        this.Height = 700;
        this.Width = 1100;

    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        bool valid = await _gmailClient.TryAuthenticateSilentlyAsync();

        if (valid)
        {
            LoginGrid.Visibility = Visibility.Collapsed;
            this.Height = 700;
            this.Width = 1100;
        }
        else
        {
            LoginGrid.Visibility = Visibility.Visible;
        }
    }
    
    
}