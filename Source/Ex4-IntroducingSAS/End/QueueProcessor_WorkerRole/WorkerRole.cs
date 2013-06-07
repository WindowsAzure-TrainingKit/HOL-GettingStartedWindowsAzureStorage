using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Queue.Protocol;

namespace QueueProcessor_WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private DateTime serviceQueueSasExpiryTime;
        private CloudQueue client;
        private Uri uri = new Uri("http://127.0.0.1:10001/devstoreaccount1");

        public override void Run()
        {
            this.SetPermissions();

            Trace.TraceInformation("QueueProcessor_WorkerRole entry point called", "Information");

            var queueClient = this.RefreshQueueClient();

            while (true)
            {
                Thread.Sleep(10000);
                Trace.TraceInformation("Working", "Information");

                var queue = queueClient.GetQueueReference("messagequeue");

                if (DateTime.UtcNow.AddMinutes(-1) >= this.serviceQueueSasExpiryTime)
                {
                    queueClient = this.RefreshQueueClient();
                    queue = queueClient.GetQueueReference("messagequeue");
                }

                var msg = queue.GetMessage();

                if (msg != null)
                {
                    Trace.TraceInformation(string.Format("Message '{0}' processed.", msg.AsString));
                    queue.DeleteMessage(msg);
                }

            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }

        private CloudQueueClient RefreshQueueClient()
        {
            var token = client.GetSharedAccessSignature(
                      new SharedAccessQueuePolicy() { Permissions = SharedAccessQueuePermissions.ProcessMessages | SharedAccessQueuePermissions.Read | SharedAccessQueuePermissions.Add | SharedAccessQueuePermissions.Update, SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(15) },
                        null);

            this.serviceQueueSasExpiryTime = DateTime.UtcNow.AddMinutes(15);
            return new CloudQueueClient(uri, new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(token));
        }

        private void SetPermissions()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference("messagequeue");
            queue.CreateIfNotExists();
            client = queue;
        }
    }
}
