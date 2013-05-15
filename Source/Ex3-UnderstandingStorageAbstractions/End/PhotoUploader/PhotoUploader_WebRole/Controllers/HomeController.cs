using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using PhotoUploader_WebRole.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhotoUploader_WebRole.Controllers
{
    public class HomeController : Controller
    {
        private CloudStorageAccount StorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        //
        // GET: /

        public ActionResult Index()
        {
            CloudTableClient cloudTableClient = this.StorageAccount.CreateCloudTableClient();
            var photoContext = new PhotoDataServiceContext(cloudTableClient);

            return View(photoContext.Photos.Select(x => this.ToViewModel(x)).ToList());
        }

        //
        // GET: /Home/Details/5

        public ActionResult Details(string id)
        {
            CloudTableClient cloudTableClient = this.StorageAccount.CreateCloudTableClient();
            var photoContext = new PhotoDataServiceContext(cloudTableClient);
            PhotoEntity photo = photoContext.Photos.SingleOrDefault(x => x.RowKey.Equals(id));

            if (photo == null)
            {
                return HttpNotFound();
            }

            //Get URI
            var viewModel = this.ToViewModel(photo);
            viewModel.Uri = this.GetBlobContainer().GetBlockBlobReference(photo.BlobReference).Uri.ToString();

            return View(viewModel);
        }

        //
        // GET: /Home/Create

        public ActionResult Create()
        {
            this.EnsureContainerExists();

            return View();
        }

        //
        // POST: /Home/Create

        [HttpPost]
        public ActionResult Create(PhotoViewModel photoViewModel, HttpPostedFileBase file, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                //Upload image to Blob Storage
                var photo = this.FromViewModel(photoViewModel);
                
                if (file != null)
                {
                    var blob = this.GetBlobContainer().GetBlockBlobReference(file.FileName);
                    blob.Properties.ContentType = file.ContentType;
                    blob.UploadFromStream(file.InputStream);
                    photo.BlobReference = file.FileName;
                }
                else
                {
                    throw new ArgumentNullException("file");
                }

                //Save information in Table Storage
                CloudTableClient cloudTableClient = this.StorageAccount.CreateCloudTableClient();
                var photoContext = new PhotoDataServiceContext(cloudTableClient);
                photoContext.AddPhoto(photo);

                //Send create notification
                var msg = new CloudQueueMessage("Photo Uploaded");
                this.GetCloudQueue().AddMessage(msg);

                return RedirectToAction("Index");
            }

            return View();
        }
        //
        // GET: /Home/Edit/5

        public ActionResult Edit(string id)
        {
            CloudTableClient cloudTableClient = this.StorageAccount.CreateCloudTableClient();
            var photoContext = new PhotoDataServiceContext(cloudTableClient);
            PhotoEntity photo = photoContext.Photos.SingleOrDefault(x => x.RowKey.Equals(id));

            if (photo == null)
            {
                return HttpNotFound();
            }
            
            //Get URI from Blob storage
            var viewModel = this.ToViewModel(photo);
            viewModel.Uri = this.GetBlobContainer().GetBlockBlobReference(photo.BlobReference).Uri.ToString();

            return View(viewModel);
        }

        //
        // POST: /Home/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PhotoEntity photo, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                //Update information in Table Storage
                CloudTableClient cloudTableClient = this.StorageAccount.CreateCloudTableClient();
                var photoContext = new PhotoDataServiceContext(cloudTableClient);
                photoContext.UpdatePhoto(photo);

                return RedirectToAction("Index");
            }
            return View();
        }

        //
        // GET: /Home/Delete/5

        public ActionResult Delete(string id)
        {
            //Get all Photos from Talb
            CloudTableClient cloudTableClient = this.StorageAccount.CreateCloudTableClient();
            var photoContext = new PhotoDataServiceContext(cloudTableClient);
            PhotoEntity photo = photoContext.Photos.SingleOrDefault(x => x.RowKey.Equals(id));

            if (photo == null)
            {
                return HttpNotFound();
            }

            //Get URI from Blob storage
            var viewModel = this.ToViewModel(photo);
            viewModel.Uri = this.GetBlobContainer().GetBlockBlobReference(photo.BlobReference).Uri.ToString();

            return View(viewModel);
        }

        //
        // POST: /Home/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            //Delete information From Table Storage
            CloudTableClient cloudTableClient = this.StorageAccount.CreateCloudTableClient();
            var photoContext = new PhotoDataServiceContext(cloudTableClient);
            var photo = photoContext.Photos.SingleOrDefault(x => x.RowKey.Equals(id));
            photoContext.DeletePhoto(photo);

            //Deletes the image from Blob storage
            var blob = this.GetBlobContainer().GetBlockBlobReference(photo.BlobReference);
            blob.DeleteIfExists();

            //Send delete notification
            var msg = new CloudQueueMessage("Photo Deleted");
            this.GetCloudQueue().AddMessage(msg);

            return RedirectToAction("Index");
        }

        private CloudBlobContainer GetBlobContainer()
        {
            var client = this.StorageAccount.CreateCloudBlobClient();
            return client.GetContainerReference(CloudConfigurationManager.GetSetting("ContainerName"));
        }

        private void EnsureContainerExists()
        {
            var container = this.GetBlobContainer();
            container.CreateIfNotExists();
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
        }

        private CloudQueue GetCloudQueue()
        {
            var queueClient = this.StorageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference("messagequeue");
            queue.CreateIfNotExists();
            return queue;
        }

        private PhotoViewModel ToViewModel(PhotoEntity photo)
        {
            return new PhotoViewModel
            {
                PartitionKey = photo.PartitionKey,
                RowKey = photo.RowKey,
                Title = photo.Title,
                Description = photo.Description                
            };
        }

        private PhotoEntity FromViewModel(PhotoViewModel photoViewModel)
        {
            return new PhotoEntity
            {
                PartitionKey = photoViewModel.PartitionKey,
                RowKey = photoViewModel.RowKey,
                Title = photoViewModel.Title,
                Description = photoViewModel.Description
            };
        }

    }
}
