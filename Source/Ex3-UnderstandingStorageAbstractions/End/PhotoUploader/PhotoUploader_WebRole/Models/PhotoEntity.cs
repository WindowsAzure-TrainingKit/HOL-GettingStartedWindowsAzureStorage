using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhotoUploader_WebRole.Models
{
    public class PhotoEntity : TableEntity
    {
        public PhotoEntity()
        {
            PartitionKey = "Photo";
            RowKey = string.Format("{0:10}_{1}", DateTime.MaxValue.Ticks - DateTime.Now.Ticks, Guid.NewGuid());
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public string BlobReference { get; set; }
    }
}