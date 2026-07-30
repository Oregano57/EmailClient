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
        var labels = await _gmailClient.GetLabelNamesAsync();

        foreach (var label in labels)
        {
            MessageBox.Show(label);
        }
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (!Directory.Exists("token.json"))
        {
            LoginGrid.Visibility = Visibility.Visible;
            return;
        }

        try
        {
            await _gmailClient.AuthenticateAsync();
            LoginGrid.Visibility = Visibility.Collapsed;
        }
        catch
        {
            LoginGrid.Visibility = Visibility.Visible;
        }
    }
    
    
}