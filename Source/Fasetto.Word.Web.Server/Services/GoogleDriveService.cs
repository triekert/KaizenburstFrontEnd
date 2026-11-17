using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fasetto.Word.Web.Server
{
    public class GoogleDriveService
    {
        private readonly DriveService _driveService;

        public DriveService DriveClient => _driveService;

        public GoogleDriveService(IConfiguration configuration)
        {
            UserCredential credential;
            var secretPath = Path.Combine(AppContext.BaseDirectory, "client_secret.json");

            using (var stream = new FileStream(secretPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    new[] { DriveService.Scope.Drive },
                    "user",
                    CancellationToken.None,
                    new FileDataStore("Fasetto.Word.GoogleDrive.Auth", true)
                ).Result;
            }

            _driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Fasetto.Word.Recon"
            });
        }

        public async Task<(Stream FileStream, string MimeType)> DownloadFileByNameAsync(string fileName)
        {
            // Sanitize whitespace and hidden characters from the file name
            var cleanFileName = fileName?.Trim();

            var listRequest = _driveService.Files.List();
            listRequest.Q = $"name = '{cleanFileName}' and trashed = false";
            listRequest.Fields = "files(id, name, mimeType)";
            listRequest.PageSize = 1;

            var result = await listRequest.ExecuteAsync();
            var file = result.Files.FirstOrDefault()
                       ?? throw new FileNotFoundException($"File '{cleanFileName}' was not found on Google Drive.");

            var getRequest = _driveService.Files.Get(file.Id);
            var memoryStream = new MemoryStream();
            await getRequest.DownloadAsync(memoryStream);
            memoryStream.Position = 0;

            return (memoryStream, file.MimeType);
        }
    }
}