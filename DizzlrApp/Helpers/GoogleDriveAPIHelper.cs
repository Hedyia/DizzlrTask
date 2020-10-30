using DizzlrApp.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DizzlrApp.Helpers
{
    public class GoogleDriveAPIHelper
    {
        IWebHostEnvironment _environment;
        private readonly Data.AppContext _context;

        public GoogleDriveAPIHelper(IWebHostEnvironment environment, Data.AppContext context)
        {
            _environment = environment;
            _context = context;
        }

        //add scope
        public string[] Scopes = { Google.Apis.Drive.v3.DriveService.Scope.Drive };

        //create Drive API service.
        public  DriveService GetService()
        {
            //get Credentials from client_secret.json file 
            UserCredential credential;

            string path = Path.Combine(_environment.WebRootPath, "GoogleCredintial");
            using (var stream = new FileStream(Path.Combine(path, "client_secret_575182456954-6te6kdb5a7l2oc476rqjvc3c1aoehkes.apps.googleusercontent.com.json"), FileMode.Open, FileAccess.Read))
            {
                String FilePath = Path.Combine(path, "DriveServiceCredentials.json");
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(FilePath, true)).Result;
            }
            //create Drive API service.
            DriveService service = new Google.Apis.Drive.v3.DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GoogleDriveMVCUpload",
            });
            return service;
        }

        //get all files from Google Drive.
        public List<Models.File> GetDriveFiles()
        {
            Google.Apis.Drive.v3.DriveService service = GetService();
            // Define parameters of request.
            Google.Apis.Drive.v3.FilesResource.ListRequest FileListRequest = service.Files.List();

            // for getting folders only.
            //FileListRequest.Q = "mimeType='application/vnd.google-apps.folder'";
            FileListRequest.Fields = "nextPageToken, files(*)";
            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;
            List<Models.File> FileList = new List<Models.File>();
            // For getting only folders
            // files = files.Where(x => x.MimeType == "application/vnd.google-apps.folder").ToList();
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    Models.File File = new Models.File
                    {
                        GoogleFileId = file.Id,
                        Name = file.Name,
                        CreatedOn = file.CreatedTime,
                        FileType = file.MimeType
                    };
                    FileList.Add(File);
                }
            }
            return FileList;
        }

        public async Task<bool> DownloadGoogleFile(string fileId, string userId)
        {
            Google.Apis.Drive.v3.DriveService service = GetService();
            string path = Path.Combine(_environment.WebRootPath, "Uploads");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            FilesResource.GetRequest request = service.Files.Get(fileId);
            string FileName = request.Execute().Name;
            string FilePath = Path.Combine(path, FileName);
            MemoryStream stream1 = new MemoryStream();
            // Add a handler which will be notified on progress changes.
            // It will notify on each chunk download and when the
            // download is completed or failed.
             bool flag = false;
            request.MediaDownloader.ProgressChanged += (IDownloadProgress progress) =>
            {
                switch (progress.Status)
                {
                    case DownloadStatus.Downloading:
                        {
                            Console.WriteLine(progress.BytesDownloaded);
                            break;
                        }
                    case DownloadStatus.Completed:
                        {
                            Console.WriteLine("Download complete.");
                            SaveStream(stream1, FilePath);
                            flag = true;
                            break;
                        }
                    case DownloadStatus.Failed:
                        {
                            Console.WriteLine("Download failed.");
                            flag = false;
                            break;
                        }
                }
            };
            request.Download(stream1);
            var file = new Models.File
            {
                Description = "From Google Drive",
                CreatedOn = request.Execute().CreatedTime,
                FileType = request.Execute().MimeType,
                Extension = request.Execute().FileExtension,
                UserId = userId,
                Name = request.Execute().Name,
            };
            await _context.FilesOnFileSystem.AddAsync(file);
            await _context.SaveChangesAsync();
            return flag;
        }

        // file save to server path
        private static void SaveStream(MemoryStream stream, string FilePath)
        {
            using (FileStream file = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                stream.WriteTo(file);
            }
        }

    }
}

