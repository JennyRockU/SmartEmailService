using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Azure;

namespace SendEmailApi
{
    public class EmailHandler
    {
        private static readonly CloudStorageAccount StorageAccount = 
            CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("mainemailstorage_AzureStorageConnectionString"));

        public static async Task SendEmailToQueue(EmailDetails email)
        {
            var queueClient = StorageAccount.CreateCloudQueueClient();
            
            var queue = queueClient.GetQueueReference("email-queue");
            await queue.CreateIfNotExistsAsync();

            var msg = new CloudQueueMessage(email.GetMessageQueueString());
            await queue.AddMessageAsync(msg);
        }
    }
}