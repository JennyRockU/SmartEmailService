using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SendEmailApi
{
    public class Authentication
    {
        // Checks whether the provided token is among the existing authorized tokens
        public static bool VaidateUser(string token)
        {
            return AuthorizedTokens.Contains(token);
        }
        

        // Init cloud storage instances
        private static CloudStorageAccount StorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("mainemailstorage_AzureStorageConnectionString"));

        private static CloudFileClient FileClient = StorageAccount.CreateCloudFileClient();
        private static CloudFileShare FileShare = FileClient.GetShareReference("apiusers");
        private static CloudFileDirectory FileDirectory = FileShare.GetRootDirectoryReference();
        private static CloudFile File = FileDirectory.GetFileReference("allowedusers.txt");

        private static IReadOnlyCollection<string> AuthorizedTokens = GetKeys();

        // Retrieves the valid tokens from storage 
        private static IReadOnlyCollection<string> GetKeys()
        {
            var data = File.DownloadText();

            return data.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
        }
    }
}