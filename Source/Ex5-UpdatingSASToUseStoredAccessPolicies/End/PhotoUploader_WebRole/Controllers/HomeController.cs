using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using PhotoUploader_WebRole.Models;
using System;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Auth;
using System.Collections.Generic;

namespace PhotoUploader_WebRole.Controllers
{
    public class HomeController : Controller
    {
        private CloudStorageAccount StorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        private Uri UriTable = new Uri("http://127.0.0.1:10002/devstoreaccount1");
        private Uri UriQueue = new Uri("http://127.0.0.1:10001/devstoreaccount1");

        //
        // GET: /

        public ActionResult Index()
        {
            this.RefreshAccessCredentials();

            CloudTableClient cloudTableClient = new CloudTableClient(this.UriTable, new StorageCredentials(Session["Sas"].ToString()));
            var photoContext = new PhotoDataServiceContext(cloudTableClient);
            var photoList = new List<PhotoViewModel>();

            var photos = photoContext.GetPhotos("Public");
            if (photos.Count() > 0)
            {
                photoList = photos.Select(x => this.ToViewModel(x)).ToList();
            }

            var privatePhotos = new List<PhotoViewModel>();

            if (this.User.Identity.IsAuthenticated)
            {
                cloudTableClient = new CloudTableClient(this.UriTable, new StorageCredentials(Session["MySas"].ToString()));
                photoContext = new PhotoDataServiceContext(cloudTableClient);

                photos = photoContext.GetPhotos(this.User.Identity.Name);
                if (photos.Count() > 0)
                {
                    photoList.AddRange(photos.Select(x => this.ToViewModel(x)).ToList());
                }
            }

            return this.View(photoList);
        }

        //
        // GET: /Home/Details/5

        public ActionResult Details(string partitionKey, string rowKey)
        {
            this.RefreshAccessCredentials();

            CloudTableClient cloudTableClient;
            PhotoEntity photo;

            if (partitionKey == "Public")
            {
                cloudTableClient = new CloudTableClient(this.UriTable, new StorageCredentials(Session["Sas"].ToString()));
                var photoContext = new PhotoDataServiceContext(cloudTableClient);
                photo = photoContext.GetById(partitionKey, rowKey);
                if (photo == null)
                {
                    return this.HttpNotFound();
                }
            }
            else
            {
                cloudTableClient = new CloudTableClient(this.UriTable, new StorageCredentials(Session["MySas"].ToString()));
                var photoContext = new PhotoDataServiceContext(cloudTableClient);

                photo = photoContext.GetById(this.User.Identity.Name, rowKey);
                if (photo == null)
                {
                    return this.HttpNotFound();
                }
            }

            var viewModel = this.ToViewModel(photo);
            if (!string.IsNullOrEmpty(photo.BlobReference))
            {
                viewModel.Uri = this.GetBlobContainer().GetBlockBlobReference(photo.BlobReference).Uri.ToString();
            }

            return this.View(viewModel);
        }

        //
        // GET: /Home/Create

        public ActionResult Create()
        {
            this.RefreshAccessCredentials();

            return View();
        }

        //
        // POST: /Home/Create

        [HttpPost]
        public ActionResult Create(PhotoViewModel photoViewModel, HttpPostedFileBase file, bool Public, FormCollection collection)
        {
            this.RefreshAccessCredentials();

            if (this.ModelState.IsValid)
            {
                photoViewModel.PartitionKey = Public ? "Public" : this.User.Identity.Name;
                var photo = this.FromViewModel(photoViewModel);

                if (file != null)
                {
                    //Save file stream to Blob Storage
                    var blob = this.GetBlobContainer().GetBlockBlobReference(file.FileName);
                    blob.Properties.ContentType = file.ContentType;
                    var image = new System.Drawing.Bitmap(file.InputStream);
                    if (image != null)
                    {
                        blob.Metadata.Add("Width", image.Width.ToString());
                        blob.Metadata.Add("Height", image.Height.ToString());
                    }

                    blob.SetMetadata();
                    blob.SetProperties();

                    blob.UploadFromStream(file.InputStream);
                    photo.BlobReference = file.FileName;
                }
                else
                {
                    this.ModelState.AddModelError("File", new ArgumentNullException("file"));
                    return this.View(photoViewModel);
                }

                //Save information to Table Storage
                var sasKey = Public ? "Sas" : "MySas";
                if (!this.User.Identity.IsAuthenticated)
                {
                    sasKey = "Sas";
                    photo.PartitionKey = "Public";
                }

                CloudTableClient cloudTableClient = new CloudTableClient(this.UriTable, new StorageCredentials(Session[sasKey].ToString()));
                var photoContext = new PhotoDataServiceContext(cloudTableClient);

                photoContext.AddPhoto(photo);

                //Send create notification
                var msg = new CloudQueueMessage(string.Format("Photo Uploaded,{0}", photo.BlobReference));
                this.GetCloudQueue().AddMessage(msg);

                return this.RedirectToAction("Index");
            }

            return this.View();
        }

        //
        // GET: /Home/Edit/5

        public ActionResult Edit(string partitionKey, string rowKey)
        {
            this.RefreshAccessCredentials();

            string token = partitionKey == "Public" ? Session["Sas"].ToString() : Session["MySas"].ToString();

            CloudTableClient cloudTableClient = new CloudTableClient(this.UriTable, new StorageCredentials(token));
            var photoContext = new PhotoDataServiceContext(cloudTableClient);
            PhotoEntity photo = photoContext.GetById(partitionKey, rowKey);

            if (photo == null)
            {
                return this.HttpNotFound();
            }

            var viewModel = this.ToViewModel(photo);
            if (!string.IsNullOrEmpty(photo.BlobReference))
            {
                viewModel.Uri = this.GetBlobContainer().GetBlockBlobReference(photo.BlobReference).Uri.ToString();
            }

            return this.View(viewModel);
        }

        //
        // POST: /Home/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PhotoViewModel photoViewModel, FormCollection collection)
        {
            this.RefreshAccessCredentials();

            if (ModelState.IsValid)
            {
                var photo = this.FromViewModel(photoViewModel);

                var token = photoViewModel.PartitionKey == "Public" ? Session["Sas"].ToString() : Session["MySas"].ToString();

                CloudTableClient cloudTableClient = new CloudTableClient(this.UriTable, new StorageCredentials(token));
                var photoContext = new PhotoDataServiceContext(cloudTableClient);
                photoContext.UpdatePhoto(photo);

                return this.RedirectToAction("Index");
            }

            return this.View();
        }

        //
        // GET: /Home/Delete/5
        public ActionResult Delete(string partitionKey, string rowKey)
        {
            this.RefreshAccessCredentials();

            string token = partitionKey == "Public" ? Session["Sas"].ToString() : Session["MySas"].ToString();

            CloudTableClient cloudTableClient = new CloudTableClient(this.UriTable, new StorageCredentials(token));
            var photoContext = new PhotoDataServiceContext(cloudTableClient);
            PhotoEntity photo = photoContext.GetById(partitionKey, rowKey);

            if (photo == null)
            {
                return this.HttpNotFound();
            }

            var viewModel = this.ToViewModel(photo);
            if (!string.IsNullOrEmpty(photo.BlobReference))
            {
                viewModel.Uri = this.GetBlobContainer().GetBlockBlobReference(photo.BlobReference).Uri.ToString();
            }

            return this.View(viewModel);
        }

        //
        // POST: /Home/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string partitionKey, string rowKey)
        {
            this.RefreshAccessCredentials();

            if (ModelState.IsValid)
            {
                string token = partitionKey == "Public" ? Session["Sas"].ToString() : Session["MySas"].ToString();

                CloudTableClient cloudTableClient = new CloudTableClient(this.UriTable, new StorageCredentials(token));
                var photoContext = new PhotoDataServiceContext(cloudTableClient);
                PhotoEntity photo = photoContext.GetById(partitionKey, rowKey);

                if (photo == null)
                {
                    return this.HttpNotFound();
                }

                photoContext.DeletePhoto(photo);

                //Deletes the Image from Blob Storage
                if (!string.IsNullOrEmpty(photo.BlobReference))
                {
                    var blob = this.GetBlobContainer().GetBlockBlobReference(photo.BlobReference);
                    blob.DeleteIfExists();
                }

                //Send delete notification
                var msg = new CloudQueueMessage(string.Format("Photo Deleted,{0}", photo.BlobReference));
                this.GetCloudQueue().AddMessage(msg);
            }
            return this.RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ToPrivate(string partitionKey, string rowKey)
        {
            CloudTableClient cloudTableClient = new CloudTableClient(this.UriTable, new StorageCredentials(Session["Sas"].ToString()));
            var photoContext = new PhotoDataServiceContext(cloudTableClient);
            var photo = photoContext.GetById(partitionKey, rowKey);
            if (photo == null)
            {
                return this.HttpNotFound();
            }

            photoContext.DeletePhoto(photo);

            cloudTableClient = new CloudTableClient(this.UriTable, new StorageCredentials(Session["MySas"].ToString()));
            photoContext = new PhotoDataServiceContext(cloudTableClient);
            photo.PartitionKey = this.User.Identity.Name;
            photoContext.AddPhoto(photo);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ToPublic(string partitionKey, string rowKey)
        {
            CloudTableClient cloudTableClient = new CloudTableClient(this.UriTable, new StorageCredentials(Session["MySas"].ToString()));
            var photoContext = new PhotoDataServiceContext(cloudTableClient);
            var photo = photoContext.GetById(partitionKey, rowKey);
            if (photo == null)
            {
                return this.HttpNotFound();
            }

            photoContext.DeletePhoto(photo);

            cloudTableClient = new CloudTableClient(this.UriTable, new StorageCredentials(Session["Sas"].ToString()));
            photoContext = new PhotoDataServiceContext(cloudTableClient);
            photo.PartitionKey = "Public";
            photoContext.AddPhoto(photo);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Share(string partitionKey, string rowKey)
        {
            this.RefreshAccessCredentials();

            PhotoEntity photo;

            var cloudTableClient = new CloudTableClient(this.UriTable, new StorageCredentials(Session["MySas"].ToString()));
            var photoContext = new PhotoDataServiceContext(cloudTableClient);

            photo = photoContext.GetById(partitionKey, rowKey);
            if (photo == null)
            {
                return this.HttpNotFound();
            }

            string sas = string.Empty;
            if (!string.IsNullOrEmpty(photo.BlobReference))
            {
                CloudBlockBlob blobBlockReference = this.GetBlobContainer().GetBlockBlobReference(photo.BlobReference);
                sas = photoContext.GetSaSForBlob(blobBlockReference, "read");
            }

            if (!string.IsNullOrEmpty(sas))
            {
                return View("Share", null, sas);
            }

            return RedirectToAction("Index");
        }

        private PhotoViewModel ToViewModel(PhotoEntity photo)
        {
            return new PhotoViewModel
            {
                PartitionKey = photo.PartitionKey,
                RowKey = photo.RowKey,
                Title = photo.Title,
                Description = photo.Description,
            };
        }

        private PhotoEntity FromViewModel(PhotoViewModel photoViewModel)
        {
            var photo = new PhotoEntity
                {
                    Title = photoViewModel.Title,
                    Description = photoViewModel.Description
                };

            photo.PartitionKey = photoViewModel.PartitionKey ?? photo.PartitionKey;
            photo.RowKey = photoViewModel.RowKey ?? photo.RowKey;
            return photo;
        }

        private CloudBlobContainer GetBlobContainer()
        {
            var client = this.StorageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(CloudConfigurationManager.GetSetting("ContainerName"));
            if (container.CreateIfNotExists())
            {
                container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }

            return container;
        }

        private CloudQueue GetCloudQueue()
        {
            var queueClient = new CloudQueueClient(this.UriQueue, new StorageCredentials(Session["QueueSas"].ToString()));
            var queue = queueClient.GetQueueReference("messagequeue");
            return queue;
        }

        public void RefreshAccessCredentials()
        {
            if ((Session["ExpireTime"] as DateTime? == null) || ((DateTime)Session["ExpireTime"] < DateTime.UtcNow))
            {
                CloudTableClient cloudTableClientAdmin = this.StorageAccount.CreateCloudTableClient();
                var photoContextAdmin = new PhotoDataServiceContext(cloudTableClientAdmin);

                Session["Sas"] = photoContextAdmin.GetSas("Public", "edit");
                Session["QueueSas"] = this.StorageAccount.CreateCloudQueueClient().GetQueueReference("messagequeue").GetSharedAccessSignature(
                        new SharedAccessQueuePolicy(),
                        "add"
                        );

                if (this.User.Identity.IsAuthenticated)
                {
                    Session["MySas"] = photoContextAdmin.GetSas(this.User.Identity.Name, "admin");
                    Session["Sas"] = photoContextAdmin.GetSas("Public", "admin");
                }

                Session["ExpireTime"] = DateTime.UtcNow.AddMinutes(15);
            }
        }
    }
}
