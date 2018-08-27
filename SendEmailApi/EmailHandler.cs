using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Threading.Tasks;
using Microsoft.Azure;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace SendEmailApi
{
    public static class EmailHandler
    {
        // get the cloud queue refrence
        private static readonly CloudStorageAccount StorageAccount = 
            CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("mainemailstorage_AzureStorageConnectionString"));
        private static CloudQueueClient QueueClient = StorageAccount.CreateCloudQueueClient();
        private static CloudQueue Queue = QueueClient.GetQueueReference("email-queue");

        public static async Task AddEmailToQueue(EmailDetails email)
        {
            // make sure the queue refrence exists in the cloud
           await Queue.CreateIfNotExistsAsync();

            // create the xml message to store
            var xmlAsString = Serialize(email);
            var msg = new CloudQueueMessage(xmlAsString);

            // add the message to queue
            await Queue.AddMessageAsync(msg);
           
        }


        public static string Serialize<T>(this T data)
        {
            var xml = string.Empty;
            if (data == null) return xml;

            var xmlSerilizer = new XmlSerializer(typeof(T));
            using (var sw = new StringWriter())
            {
                using (var writer = XmlWriter.Create(sw))
                {
                    xmlSerilizer.Serialize(writer, data);
                    return sw.ToString();
                }
            }
        }
    }
}
