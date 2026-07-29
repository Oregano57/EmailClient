using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace EmailClient;

public class GmailClient
{
    private GmailService? _service;

    public async Task AuthenticateAsync()
    {
        string[] scopes = { GmailService.Scope.GmailReadonly };

        UserCredential credential;
        using (var stream = new FileStream("Credentials/client_secret.json", FileMode.Open, FileAccess.Read))
        {
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets, scopes, "user", CancellationToken.None, new FileDataStore("token.json", true));
        }

        _service = new GmailService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential, ApplicationName = "MyEmailClient"
        });
    }
    
    public async Task<List<string>> GetLabelNamesAsync()
    {
        var request = _service.Users.Labels.List("me");
        var response = await request.ExecuteAsync();
        return response.Labels.Select(l => l.Name).ToList();
    }
}