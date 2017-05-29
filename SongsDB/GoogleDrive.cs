using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis;
using Google.Apis.Drive.v3;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using System.Diagnostics;

namespace SongsDB
{
    public class GoogleDrive
    {
        static string[] Scopes = { DriveService.Scope.DriveReadonly };
        static string ApplicationName = "Song DB";

        public static void getFiles()
        {
            UserCredential credential;

            using (var stream =
                //new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
                new System.IO.FileStream("client_secret_976131788542.json", System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = System.IO.Path.Combine(credPath, ".credentials\\song-db.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Debug.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Drive API service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name)";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                .Files;
            Debug.WriteLine("Files:");
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    Debug.WriteLine("{0} ({1})", file.Name, file.Id);
                }
            }
            else
            {
                Debug.WriteLine("No files found.");
            }

        }

        public static void demo()
        {
            // Connect with Oauth2 Ask user for permission
            String CLIENT_ID = "367406449849-038pisls6ne8m875p24d4qhn33jp157t.apps.googleusercontent.com";
            String CLIENT_SECRET = "5kGgaUTycFl6Olp8HuIn4RZP";
            DriveService service = Authentication.AuthenticateOauth(CLIENT_ID, CLIENT_SECRET, Environment.UserName);


            // connect with a Service Account
            //string ServiceAccountEmail = "1046123799103-6v9cj8jbub068jgmss54m9gkuk4q2qu8@developer.gserviceaccount.com";
            //string serviceAccountkeyFile = @"C:\GoogleDevelop\Diamto Test Everything Project-78049f608668.p12";
            //DriveService service = Authentication.AuthenticateServiceAccount(ServiceAccountEmail, serviceAccountkeyFile);

            if (service == null)
            {
                Debug.WriteLine("Authentication error");
            }


            try
            {

                // Listing files with search.  
                // This searches for a directory with the name DiamtoSample
                string Q = "title = 'DiamtoSample' and mimeType = 'application/vnd.google-apps.folder'";
                IList<Google.Apis.Drive.v3.Data.File> _Files = DaimtoGoogleDriveHelper.GetFiles(service, "");

                foreach (Google.Apis.Drive.v3.Data.File item in _Files)
                {
                    Debug.WriteLine(item.Name + " " + item.MimeType);
                }

                //// If there isn't a directory with this name lets create one.
                //if (_Files.Count == 0)
                //{
                //    _Files.Add(DaimtoGoogleDriveHelper.createDirectory(service, "DiamtoSample", "DiamtoSample", "root"));
                //}

                //// We should have a directory now because we either had it to begin with or we just created one.
                //if (_Files.Count != 0)
                //{

                //    // This is the ID of the directory 
                //    string directoryId = _Files[0].Id;

                //    //Upload a file
                //    Google.Apis.Drive.v3.Data.File newFile = DaimtoGoogleDriveHelper.uploadFile(service, @"c:\GoogleDevelop\dummyUploadFile.txt", directoryId);
                //    // Update The file
                //    Google.Apis.Drive.v3.Data.File UpdatedFile = DaimtoGoogleDriveHelper.updateFile(service, @"c:\GoogleDevelop\dummyUploadFile.txt", directoryId, newFile.Id);
                //    // Download the file
                //    DaimtoGoogleDriveHelper.downloadFile(service, newFile, @"C:\GoogleDevelop\downloaded.txt");
                //    // delete The file
                //    FilesResource.DeleteRequest request = service.Files.Delete(newFile.Id);
                //    request.Execute();
                //}

                //// Getting a list of ALL a users Files (This could take a while.)
                //_Files = DaimtoGoogleDriveHelper.GetFiles(service, null);

                //foreach (Google.Apis.Drive.v3.Data.File item in _Files)
                //{
                //    Console.WriteLine(item.Name + " " + item.MimeType);
                //}
            }
            catch (Exception ex)
            {

                int i = 1;
            }

            
        }

        //public void downloadFile()
        //    {
        //        var fileId = "0BwwA4oUTeiV1UVNwOHItT0xfa2M";
        //        var request = driveService.Files.Get(fileId);
        //        var stream = new System.IO.MemoryStream();

        //        // Add a handler which will be notified on progress changes.
        //        // It will notify on each chunk download and when the
        //        // download is completed or failed.
        //        request.MediaDownloader.ProgressChanged +=
        //            (IDownloadProgress progress) =>
        //            {
        //                switch (progress.Status)
        //                {
        //                    case DownloadStatus.Downloading:
        //                        {
        //                            Console.WriteLine(progress.BytesDownloaded);
        //                            break;
        //                        }
        //                    case DownloadStatus.Completed:
        //                        {
        //                            Console.WriteLine("Download complete.");
        //                            break;
        //                        }
        //                    case DownloadStatus.Failed:
        //                        {
        //                            Console.WriteLine("Download failed.");
        //                            break;
        //                        }
        //                }
        //            };
        //        request.Download(stream);
        //    }
    }
}
