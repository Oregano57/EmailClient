using System.Collections.ObjectModel;
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
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;

namespace EmailClient;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public ObservableCollection<string> Labels { get; set; } = new ObservableCollection<string>();

    private GmailClient _gmailClient = new GmailClient();
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    private async void SignInWithGoogleButton_Click(object sender, RoutedEventArgs e)
    {
        await _gmailClient.AuthenticateAsync();
        LoginGrid.Visibility = Visibility.Collapsed;
        MainGrid.Visibility = Visibility.Visible;
        this.Height = 700;
        this.Width = 1100;
        CenterWindow();
        var labelNames = await _gmailClient.GetLabelNamesAsync();
        foreach (var name in labelNames)
        {
            Labels.Add(name);
        }

    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        bool valid = await _gmailClient.TryAuthenticateSilentlyAsync();

        if (valid)
        {
            LoginGrid.Visibility = Visibility.Collapsed;
            MainGrid.Visibility = Visibility.Visible;
            this.Height = 700;
            this.Width = 1100;
            CenterWindow();
            var labelNames = await _gmailClient.GetLabelNamesAsync();
            foreach (var name in labelNames)
            {
                Labels.Add(name);
            }
        }
        else
        {
            LoginGrid.Visibility = Visibility.Visible;
            MainGrid.Visibility  = Visibility.Collapsed;
        }
    }

    private void CenterWindow()
    {
        this.Left = (SystemParameters.PrimaryScreenWidth - this.Width) / 2;
        this.Top = (SystemParameters.PrimaryScreenHeight - this.Height) / 2;
    }

    private async void SignOutButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await _gmailClient.RevokeAccessAsync();
        }
        catch
        {
            
        }
        if (Directory.Exists("token.json"))
            Directory.Delete("token.json", true);

        LoginGrid.Visibility = Visibility.Visible;
        MainGrid.Visibility = Visibility.Collapsed;
        this.Height = 450;
        this.Width = 800;
        CenterWindow();
        
    }

    private void NotificationsButton_Click(object sender, RoutedEventArgs e)
    {
        if (NotificationsPopup.IsOpen)
            NotificationsPopup.IsOpen = false;
        NotificationsPopup.IsOpen = true;
    }
    
}