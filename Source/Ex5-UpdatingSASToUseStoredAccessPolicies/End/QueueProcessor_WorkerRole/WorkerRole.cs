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
using Microsoft.WindowsAzure.Storage.Blob;

namespace QueueProcessor_WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private DateTime serviceQueueSasExpiryTime;
        private CloudQueue client;
        private CloudBlobContainer container;

        private Uri uri = new Uri("http://127.0.0.1:10001/devstoreaccount1");

        public override void Run()
        {
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
                    queue.FetchAttributes();

                    var messageParts = msg.AsString.Split(new char[] { ',' });
                    var message = messageParts[0];
                    var blobReference = messageParts[1];

                    if (queue.Metadata.ContainsKey("Resize") && string.Equals(message, "Photo Uploaded"))
                    {
                        var maxSize = queue.Metadata["Resize"];

                        Trace.TraceInformation("Resize is configured");

                        CloudBlockBlob outputBlob = this.container.GetBlockBlobReference(blobReference);

                        outputBlob.FetchAttributes();

                        Trace.TraceInformation(string.Format("Image ContentType: {0}", outputBlob.Properties.ContentType)); 
                        Trace.TraceInformation(string.Format("Image width: {0}", outputBlob.Metadata["Width"]));
                        Trace.TraceInformation(string.Format("Image hieght: {0}", outputBlob.Metadata["Height"])); 
                    }

                    Trace.TraceInformation(string.Format("Message '{0}' processed.", message));
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

            this.SetPermissions();

            this.CreateCloudBlobClient();

            return base.OnStart();
        }

        private CloudQueueClient RefreshQueueClient()
        {
            var token = client.GetSharedAccessSignature(
                      new SharedAccessQueuePolicy(),
                        "process");

            this.serviceQueueSasExpiryTime = DateTime.UtcNow.AddMinutes(15);
            return new CloudQueueClient(uri, new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(token));
        }

        private void SetPermissions()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference("messagequeue");
            queue.CreateIfNotExists();

            QueuePermissions qp = new QueuePermissions();
            qp.SharedAccessPolicies.Add("add", new SharedAccessQueuePolicy { Permissions = SharedAccessQueuePermissions.Add | SharedAccessQueuePermissions.Read, SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(15) });
            qp.SharedAccessPolicies.Add("process", new SharedAccessQueuePolicy { Permissions = SharedAccessQueuePermissions.ProcessMessages | SharedAccessQueuePermissions.Read, SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(15) });
            queue.SetPermissions(qp);

            client = queue;
        }

        private void CreateCloudBlobClient()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudBlobClient blobStorage = storageAccount.CreateCloudBlobClient();
            this.container = blobStorage.GetContainerReference(CloudConfigurationManager.GetSetting("ContainerName"));
        }
    }
}
