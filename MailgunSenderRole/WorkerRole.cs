using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MainSender;
using Microsoft.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private readonly string WorkerRoleName = "MailgunRole";
        private static readonly CloudStorageAccount StorageAccount =
           CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("mainemailstorage_AzureStorageConnectionString"));


        public override void Run()
        {
            Trace.TraceInformation($"{WorkerRoleName} is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).GetAwaiter().GetResult();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            bool result = base.OnStart();

            Trace.TraceInformation($"{WorkerRoleName} has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation($"{WorkerRoleName} is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation($"{WorkerRoleName} has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            var queueClient = StorageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference("email-queue");
            var mailgunVendor = new MailgunSender.Sender();

            while (!cancellationToken.IsCancellationRequested)
            {
                var queueMsg = await queue.GetMessageAsync(cancellationToken);
                if (queueMsg == null) continue;
                var isProcessed = await MessageHandler.ProcessMessage(queueMsg, mailgunVendor, cancellationToken);

                // if the vendor's api call failed, move next, and the message will be dequeued and
                // picked up again after its visibilty time of 30 sec. expires
                if (isProcessed)
                {
                    // otherwise delete the message from the queue
                    await queue.DeleteMessageAsync(queueMsg, cancellationToken);
                }

            }
        }
    }
}
