using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Hosting;
using MonolithicNetCore.Common;
using MonolithicNetCore.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace MonolithicNetCore.Service.GoogleService
{
    public interface IGoogleDriverService
    {
        DriveService Authorize();
        void FileUploadToServer(string fileUrl);
        void BackUpDatabase();
        List<GoogleDriveFiles> GetDriveFiles(DriveService service = null);
        List<GoogleDriveFiles> RemoveDriveFilesBackup(List<GoogleDriveFiles> listFiles, DriveService service = null);
    }
    public class GoogleDriverService : IGoogleDriverService
    {
        private IHostingEnvironment _hostingEnvironment;
        private IDbFactory _dbFactory;
        private string FileZipUrl = "";
        public GoogleDriverService(IHostingEnvironment hostingEnvironment, IDbFactory dbFactory) => (_hostingEnvironment, _dbFactory) = (hostingEnvironment, dbFactory);
        public DriveService Authorize()
        {
            string[] Scopes = { DriveService.Scope.Drive };
            UserCredential credential;
            using (var stream = new FileStream($"{_hostingEnvironment.WebRootPath}{ConfigAppSetting.DriverCredentialsPath}", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore($"{_hostingEnvironment.WebRootPath}{ConfigAppSetting.DriveServiceCredentials}", true)).Result;
            }

            //Create Drive API service.
            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "VilComXuatNhap",
            });
            return service;
        }

        public void FileUploadToServer(string fileUrl)
        {
            this.FileZipUrl = fileUrl;
            DriveService service = Authorize();
            Google.Apis.Drive.v3.Data.File body = new Google.Apis.Drive.v3.Data.File();
            body.Name = System.IO.Path.GetFileName(fileUrl);
            body.Description = $"Backup database {body.Name}";
            body.MimeType = "application/mysql";
            body.Parents = new List<string>
            {
                ConfigAppSetting.FolderIdBackupDriver
            };
            // body.Parents = new List<string> { _parent };// UN comment if you want to upload to a folder(ID of parent folder need to be send as paramter in above method)
            byte[] byteArray = System.IO.File.ReadAllBytes(fileUrl);
            System.IO.MemoryStream stream1 = new System.IO.MemoryStream(byteArray);
            try
            {
                FilesResource.CreateMediaUpload request = service.Files.Create(body, stream1, body.MimeType);
                request.SupportsTeamDrives = true;
                // You can bind event handler with progress changed event and response recieved(completed event)
                request.ResponseReceived += Request_ResponseReceived;
                request.Upload();
                Console.WriteLine(request.ResponseBody);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private void Request_ResponseReceived(Google.Apis.Drive.v3.Data.File obj)
        {
            if (obj != null)
            {
                Console.WriteLine("File was uploaded sucessfully--" + obj.Id);
                if (!string.IsNullOrEmpty(this.FileZipUrl) && System.IO.File.Exists(this.FileZipUrl))
                {
                    System.IO.File.Delete(this.FileZipUrl);
                }
                RemoveDriveFilesBackup(GetDriveFiles());
            }
        }

        public void BackUpDatabase()
        {
            var pathDatabaseBackup = _dbFactory.BackUpDatabase();
            FileUploadToServer(pathDatabaseBackup);
        }

        public List<GoogleDriveFiles> GetDriveFiles(DriveService service = null)
        {
            service = service ?? Authorize();
            // Define parameters of request.
            FilesResource.ListRequest FileListRequest = service.Files.List();
            FileListRequest.Q = $"'{ConfigAppSetting.FolderIdBackupDriver}' in parents";
            //listRequest.PageSize = 10;
            //listRequest.PageToken = 10;
            FileListRequest.Fields = "nextPageToken, files(id, name, size, version, trashed, createdTime)";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;
            List<GoogleDriveFiles> FileList = new List<GoogleDriveFiles>();

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    GoogleDriveFiles File = new GoogleDriveFiles
                    {
                        Id = file.Id,
                        Name = file.Name,
                        Size = file.Size,
                        Version = file.Version,
                        CreatedTime = file.CreatedTime
                    };
                    FileList.Add(File);
                }
            }
            return FileList;
        }

        public List<GoogleDriveFiles> RemoveDriveFilesBackup(List<GoogleDriveFiles> listFiles, DriveService service = null)
        {
            List<GoogleDriveFiles> FileListRemove = new List<GoogleDriveFiles>();
            service = service ?? Authorize();
            for (var i = 2; i < listFiles.Count; i++)
            {
                service.Files.Delete(listFiles[i].Id).Execute();
                FileListRemove.Add(listFiles[i]);
            }
            return FileListRemove;
        }
    }

    public class GoogleDriveFiles
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public long? Size { get; set; }
        public long? Version { get; set; }
        public DateTime? CreatedTime { get; set; }
    }
}
