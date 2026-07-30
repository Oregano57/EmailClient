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
            HttpClientInitializer = credential, ApplicationName = "EmailClient"
        });
    }
    
    public async Task<bool> TryAuthenticateSilentlyAsync()
    {
        if (!Directory.Exists("token.json"))
            return false;

        try
        {
            var dataStore = new FileDataStore("token.json", true);
            string[] scopes = { GmailService.Scope.GmailReadonly };

            using var stream = new FileStream("Credentials/client_secret.json", FileMode.Open, FileAccess.Read);
            var secrets = GoogleClientSecrets.FromStream(stream).Secrets;

            var flow = new Google.Apis.Auth.OAuth2.Flows.GoogleAuthorizationCodeFlow(
                new Google.Apis.Auth.OAuth2.Flows.GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = secrets,
                    Scopes = scopes,
                    DataStore = dataStore
                });

            var token = await dataStore.GetAsync<Google.Apis.Auth.OAuth2.Responses.TokenResponse>("user");
            if (token == null)
                return false;

            var credential = new UserCredential(flow, "user", token);
            bool refreshed = await credential.RefreshTokenAsync(CancellationToken.None);

            if (!refreshed)
                return false;

            _service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "EmailClient(C#)"
            });
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<List<string>> GetLabelNamesAsync()
    {
        var request = _service.Users.Labels.List("me");
        var response = await request.ExecuteAsync();
        return response.Labels.Select(l => l.Name).ToList();
    }
}