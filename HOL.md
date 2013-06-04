<a name="Title" />
# Getting Started With Windows Azure Storage #

---
<a name="Overview" />
## Overview ##

In this lab, you will learn the basics of **Windows Azure Storage**, how to create and configure storage accounts and how you can programmatically access the different types of storage service. **Blobs**, **Tables**, and **Queues** are all available as part of the **Windows Azure Storage** account, and provide durable storage on the Windows Azure platform. These services are accessible from both inside and outside the Windows Azure platform by using the [Windows Azure Storage Client SDK](http://msdn.microsoft.com/en-us/library/microsoft.windowsazure.storageclient.aspx), or via URI using [REST APIs]  (http://msdn.microsoft.com/en-us/library/dd179355.aspx).

You will learn how the following services work:

![storage-diagram](Images/storage-diagram.png?raw=true)

**Table Storage**

Table storage is a collection of row like entities, each of which can contain up to 255 properties. There is no schema that enforces a certain set of values on all the rows within a table, unlike tables in a database. It does not provide any way to represent relationships between data. Windows Azure Storage tables are more like rows within a spreadsheet application such as Excel than rows within a database such as SQL Database, in that each row can contain a different number of columns, and of different data types, than the other rows in the same table.

**Blog Storage**

Blobs provide a way to store large amounts of unstructured, binary data, such as video, audio, images, etc.  One of the features of blobs is streaming content such as video or audio.

**Queue Storage**

Queues provide storage for passing messages between applications. Messages stored to the queue are limited to a maximum of 8KB in size, and are generally stored and retrieved on a first in, first out (FIFO) basis (however FIFO is not guaranteed). Processing messages from a queue is a two stage process, which involves getting the message, and then deleting the message after it has been processed.  This pattern allows you to implement guaranteed message delivery by leaving the message in the queue until it has been fully processed.


<a name="Objectives" />
### Objectives ###

In this hands-on lab, you will learn how to:

* Create an Storage Account.
* Learn the different configuration options for Geo-Replication, Monitoring and Logging.
* Access to Tables, Blobs and Queues using **Windows Azure SDK 2.0** in a MVC Web Application.

<a name="Prerequisites" />
### Prerequisites ###

The following is required to complete this hands-on lab:

- [Microsoft Visual Studio Express 2012 for Web] [1]
- [Windows Azure Tools for Microsoft Visual Studio 2.0] [2]
- A Windows Azure subscription - [sign up for a free trial][3]

[1]: http://msdn.microsoft.com/vstudio/products/
[2]: http://www.microsoft.com/windowsazure/sdk/
[3]: http://aka.ms/WATK-FreeTrial

<a name="Setup" />
### Setup ###
In order to execute the exercises in this hands-on lab you need to set up your environment.

1. Open a Windows Explorer window and browse to the  **Source** folder of this lab.

1. Execute the **Setup.cmd** file with Administrator privileges to launch the setup process. This process will configure your environment and install the Visual Studio code snippets for this lab.
1. If the User Account Control dialog is shown, confirm the action to proceed.

> **Note:** Make sure you have checked all the dependencies for this lab before running the setup.

<a name="UsingCodeSnippets" />
### Using the Code Snippets ###

Throughout the lab document, you will be instructed to insert code blocks. For your convenience, most of that code is provided as Visual Studio Code Snippets, which you can use from within Visual Studio 2010 to avoid having to add it manually. 

---
<a name="Exercises" />
## Exercises ##

This hands-on lab includes the following exercises:

1.	[Exercise 1 - Creating a Windows Azure Storage Account](#Exercise1)
2.	[Exercise 2 - Managing a Windows Azure Storage Account](#Exercise2)
1.	[Exercise 3 - Understanding the Windows Azure Storage Abstractions](#Exercise3)

> **Note:** Each exercise is accompanied by a starting solution. These solutions are missing some code sections that are completed through each exercise and therefore will not necessarily work if running them directly.
Inside each exercise you will also find an end folder where you find the resulting solution you should obtain after completing the exercises. You can use this solution as a guide if you need additional help working through the exercises.

Estimated time to complete this lab: **60** minutes.

> **Note:** When you first start Visual Studio, you must select one of the predefined settings collections. Every predefined collection is designed to match a particular development style and determines window layouts, editor behavior, IntelliSense code snippets, and dialog box options. The procedures in this lab describe the actions necessary to accomplish a given task in Visual Studio when using the **General Development Settings** collection. If you choose a different settings collection for your development environment, there may be differences in these procedures that you need to take into account.

<a name="Exercise1" />
### Exercise 1: Creating a Windows Azure Storage Account ###

This exercise describes how to create a storage account in the Windows Azure Management Portal. To store files and data in the storage services in Windows Azure, you must create a storage account in the geographic region where you want to store the data.

> **Note:** A storage account can contain up to 100 TB of blob, table, and queue data. You can create up to five storage accounts for each Windows Azure subscription.

<a name="Ex1Task1" />
#### Task 1 - Creating an Storage Account from Management Portal ####

In this task you will learn how to create a new Storage Account using the Management Portal.

1. Navigate to http://manage.windowsazure.com using a Web browser and sign in using the Microsoft Account associated with your Windows Azure account.

	![logging-azure-portal](Images/logging-azure-portal.png?raw=true)

	_Logging to the Management Portal_

1. In the menu located at the bottom, select **New | Data Services | Storage | Quick Create** to start creating a new Storage Account. Enter an unique name for the account and select a **Region** from the list. Click **OK** to continue.


	![create-storage-account-menu](Images/create-storage-account-menu.png?raw=true)

	_Creating a new storage account_

1.  In the **Storage** section, you will see the Storage Account you created with a _Creating_ status. Wait until it changes to _Online_ in order to continue with the following step.

	![storage-account-created](Images/storage-account-created.png?raw=true)

	_Storage Account created_

1. Click on the storage account name you created. You will enter to the **Dashboard** page which provides you with information about the status of the account and the service endpoints that can be used within your applications.

	![storage-account-dashboard](Images/storage-account-dashboard.png?raw=true)

	_Displaying the Storage Account Dashboard_

	In the next exercise, you will configure the storage account to enable Geo-Replication, Monitoring and Logging and manage the Access Keys.

<a name="Exercise2" />
### Exercise 2: Managing a Windows Azure Storage Account ###

In this exercise, you will configure the common settings for your storage account. You will manage your **Access Keys**, enabling **Geo-Replication** and configuring **Monitoring and Logging**.

<a name="Ex2Task1" />
#### Task 1 - Enabling Geo-Replication ####

Geo-replication replicates the stored content to a secondary location to enable failover to that location in case of a major disaster in the primary location. The secondary location is in the same region, but is hundreds of miles from the primary location. This is the highest level of storage durability, known as geo redundant storage (GRS). Geo-replication is turned on by default.

1.	In the Storage Account page, click the **Configure** tab from the top menu.

	![configure-storage-menu](Images/configure-storage-menu.png?raw=true)

	_Configuring Storage Account_

1.  You can choose to enable or disable it in the **Geo-Replication** section.

	![configuring-storage-georeplication](Images/configuring-storage-georeplication.png?raw=true)

	_Enabling Geo-Replication_

	> **Note:** If you turn off geo-replication, you have locally redundant storage (LRS). For locally redundant storage, account data is replicated three times within the same data center. LRS is offered at discounted rates. Be aware that if you turn off geo-replication, and you later change your mind, you will incur a one-time data cost to replicate your existing data to the secondary location.

<a name="Ex2Task2" />
#### Task 2 - Configuring Monitoring ####

You can monitor your storage accounts in the Windows Azure Management Portal. For each storage service associated with the storage account (Blob, Queue, and Table), you can choose the level of monitoring - minimal or verbose - and specify the appropriate data retention policy. You can do this in the **Monitoring** section.

1. In the **Configure** page, go to the **Monitoring** section.

	![configuring-storage-monitoring](Images/configuring-storage-monitoring.png?raw=true)

	_Configuring Monitoring Options_

1.	To set the monitoring level, select one of the following:

	**Minimal** - Collects metrics such as ingress/egress, availability, latency, and success percentages, which are aggregated for the Blob, Table, and Queue services.

	**Verbose** - In addition to the minimal metrics, collects the same set of metrics for each storage operation in the Windows Azure Storage Service API. Verbose metrics enable closer analysis of issues that occur during application operations.

	**Off** - Turns off monitoring. Existing monitoring data is persisted through the end of the retention period.

	> **Note:** There are costs considerations when you select monitoring. For more information, see [Storage Analytics and Billing](http://msdn.microsoft.com/en-us/library/windowsazure/hh360997.aspx).

1. To set the data retention policy, in **Retention** (in days), type the number of days of data to retain from 1-365 days. If there is no retention policy (by entering zero value), it is up to you to delete the monitoring data. 

	> **Note:** It's recommended setting a retention policy based on how long you want to retain storage analytics data for your account so that old and unused analytics data can be deleted by system at no cost.

1. Once Monitoring is enabled, you can customize the **Dashboard** to choose up to six metrics to plot on the metrics chart. There are nine available metrics for each service. To configure this, go to the **Dashboard** page.

	![storage-dashboard-menu](Images/storage-dashboard-menu.png?raw=true)

1.	In the **Dashboard** page, you will see the default metrics displayed on the chart. To add a different metric, click on the **More** button to display the available metrics. Select one from the list.

	![adding-metrics-dashboard](Images/adding-metrics-dashboard.png?raw=true)
	
	_Adding Metrics to the Dashboard_

	> **Note:** You can hide metrics that are plotted on the chart by clearing the check box by the metric header.

1.	By default, the chart shows trends, displaying only the current value of each metric (the **Relative** option at the top of the chart). To display a Y axis, so you can see absolute values, select **Absolute**.

	![dashboard-absolute-values](Images/dashboard-absolute-values.png?raw=true)

	_Changing Chart values to Absolute_

1.	To change the time range the metrics chart displays, select **6 hours**, **24 hours**, or **7 days** at the top of the chart.

	![dashboard-time-ranges](Images/dashboard-time-ranges.png?raw=true)

	_Changing Chart Time Ranges_

1.	Click **Monitor** on the top menu. On the **Monitor** page, you can view the full set of metrics for your storage account.

	![storage-monitor-menu](Images/storage-monitor-menu.png?raw=true)

1.	By default, the metrics table displays a subset of the metrics that are available for monitoring. The illustration shows the default Monitor display for a storage account with verbose monitoring configured for all three services. Click **Add Metrics** button from the bottom menu.

	![add-metrics-menu](Images/add-metrics-menu.png?raw=true)

	_Adding Metrics_

1.	In the dialog box, you can choose from a list of different types of metrics for each service. On the right, click the arrow to collapse the list.

	![collapsing-metrics](Images/collapsing-metrics.png?raw=true)

	_Collapsing Metrics_

1.	You will see a list of groups of metrics for the different services. You can expand and select the metrics you want to display in the **Monitor** table. Click **OK** to continue.

	![collapsed-metrics-list](Images/collapsed-metrics-list.png?raw=true)

	_Groups of Metrics by Service_

1.	The metrics you selected will be displayed on the **Monitor** table.

	![metrics-table](Images/metrics-table.png?raw=true)

1.	You can delete a metric by selecting it and clicking the **Delete Metric** button from the bottom menu.

	![delete-metric-menu](Images/delete-metric-menu.png?raw=true)

	_Deleting a Metric_

<a name="Ex2Task3" />
#### Task 3 - Configuring Logging ####

You can save diagnostics logs for Read Requests, Write Requests, and/or Delete Requests, and can set the data retention policy for each of the services. In this task you will configure logging for your storage account.

1. In the **Configure** page, go to the **Logging** section.

1.	For each service (Blob, Table, and Queue), you can configure the types of request to log: Read Requests, Write Requests, and Delete Requests. You can also configure the number of days to retain the logged data. Enter zero if you do not want to set a retention policy. If you do not set a retention policy, it is up to you to delete the logs.

	![configuring-storage-logging](Images/configuring-storage-logging.png?raw=true)

	_Configuring Logging Options_

	> **Note:** The diagnostics logs are saved in a blob container named **$logs** in your storage account. For information about accessing the $logs container, see [About Storage Analytics Logging](http://msdn.microsoft.com/en-us/library/windowsazure/hh343262.aspx).


<a name="Ex2Task4" />
#### Task 4 - Managing Account Keys ####

When you create a storage account, Windows Azure generates two 512-bit storage access keys, which are used for authentication when the storage account is accessed. By providing two storage access keys, Windows Azure enables you to regenerate the keys with no interruption to your storage service or access to it.

1.	In the Storage Account Dashboard, select the option **Manage Access Keys** from the bottom menu.

	![manage-keys-menu](Images/manage-keys-menu.png?raw=true)

	_Managing Access Keys_

1. You can use **Manage Keys** to copy a storage access key to use in a connection string. The connection string requires the storage account name and a key to use in authentication. Take note of the Primary access key and the storage account name as they will be used in the following exercise.

	![managing-access-keys](Images/managing-access-keys.png?raw=true)

	_Copying Access Keys_

1.	By clicking the **Regenerate** button, a new Access Key is created. You should change the access keys to your storage account periodically to help keep your storage connections more secure. Two access keys are assigned to enable you to maintain connections to the storage account using one access key while you regenerate the other access key. 

	> **Note:** Regenerating your access keys affects virtual machines, media services, and any applications that are dependent on the storage account.

	Next exercise, you will consume Windows Azure Storage services from a MVC application.

<a name="Exercise3"></a>
###Exercise 3: Understanding the Windows Azure Storage Abstractions ###

This sample application is comprised of 5 Views, one for each CRUD operation (Create, Read, Update, Delete) and one to list all the entities from the Table storage. In this exercise, you will update the MVC application actions to perform operations against each storage service (Table, Blob and Queue) using **Windows Azure SDK v2.0**.

<a name="Ex3Task1" />
#### Task 1 - Configuring Storage Account in the Cloud Project ####

In this task you will configure the _storage connection string_ of the application with the storage account you previously created using the _Access Key_ you took note from the previous exercise.

1. Open **Visual Studio Express 2012 for Web** as Administrator.

1. Browse to the **Source\Ex3-UnderstandingStorageAbstractions\Begin\PhotoUploader** folder of this lab and open the **Begin.sln** solution. Make sure to set the **PhotoUploader** cloud project as the default project.

1. Go to the **PhotoUploader_WebRole** located in the **Roles** folder of the **PhotoUploader** solution. Right-click it and select **Properties**.

	![Web Role Properties](Images/webrol-properties.png?raw=true "WebRol Properties")

	_Web Role Properties_

1. Go to the **Settings** tab and locate the _StorageConnectionString_ setting. Click the ellipsis beside the _UseDevelopmentStorage=true_ value.

	![Settings Tab](Images/settings-tab.png?raw=true "Settings Tab")

	_Settings Tab_

1. Select **Manually Entered Credentials** and set the **Account name** and **Account key** values from the previous exercise. 

	![Create Storage Connection String Dialog](Images/create-storage-connection-string-dialog.png?raw=true "Create Storage Connection String Dialog")

	_Create Storage Connection String Dialog_

1. Click **OK** to update the connection string.

1. Repeat the previous steps to configure the _StorageConnectionString_ for the **PhotoUploader_WorkerRole**.

<a name="Ex3Task2" />
#### Task 2 - Working with Table Storage ####

In this task you will update the MVC application actions to perform operations against the Table Storage. You are going to use Table Storage to save infromation from the photo uploaded such as Title and Description.

1. Open **PhotoEntity.cs** under **Models** folder and add the following directives.

	````C#
	using Microsoft.WindowsAzure.Storage.Table;
	````

1. Update the class to inherit from **TableEntity**. The TableEntity has a **PartitionKey** and  **RowKey** property that need to be set when adding a new row to the Table Storage. To do so, add the following Constructor and inherit the class from **TableEntity**.

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-InheritingTableEntity_)
	<!-- mark:3-7 -->
	````C#
	public class PhotoEntity : TableEntity
	{
		public PhotoEntity()
		{
			PartitionKey = "Photo";
			RowKey = Guid.NewGuid().ToString();
		}

		...
	}
	````

1. Now you will add a new class to implement a **TableServiceContext** to interact with Table Storage. Right-click the **Models** folder and select **Add** | **Class**.

	![Add new class](Images/add-new-class.png?raw=true "Add new class")

	_Adding a new class_

1. In the **Add New Item** window, set the name of the class to **PhotoDataServiceContext.cs** and click **Add**. 

	![PhotoDataServiceContext class](Images/photodataservicecontext-class.png?raw=true "PhotoDataServiceContext class")

	_PhotoDataServiceContext class_

1. Add the following directives to the **PhotoDataServiceContext** class.

	````C#
	using Microsoft.WindowsAzure.Storage.Table;
	using Microsoft.WindowsAzure.Storage.Table.DataServices;
	````

1. Replace the class content with the following code. 

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-PhotoDataServiceContext_)

	````C#
	public class PhotoDataServiceContext : TableServiceContext
	{
		public PhotoDataServiceContext(CloudTableClient client): base(client)
		{
			
		}

		public IEnumerable<PhotoEntity> GetPhotos()
		{
			return this.CreateQuery<PhotoEntity>("Photos");			
		}
	}
	````
	
	>**Note**: You need to make the class inherit from TableServiceContext to interact with Table Storage.

1. Now, you will add an operation to retrieve a single entity from the table. Add the following code to the **PhotoDataServiceContext** class:

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-DataContextGetById_)

	````C#
	public PhotoEntity GetById(string rowKey)
	{
		CloudTable table = this.ServiceClient.GetTableReference("Photos");
		TableOperation retrieveOperation = TableOperation.Retrieve<PhotoEntity>("Photo", rowKey);

		TableResult retrievedResult = table.Execute(retrieveOperation);

		if (retrievedResult.Result != null)
			 return (PhotoEntity)retrievedResult.Result;
		else
			 return null;
	}
	````

	>**Note**: The following code uses a **TableOperation** to retrieve the photo with the specific **RowKey**. This method returns just one entity, rather than a collection, and the returned value in **TableResult.Result** is a **PhotoEntity**.

1.	In order to add a new entity, you can use the **Insert** table operation. Add the following code to implement it:

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-DataContextAddPhoto_)

	````C#
	public void AddPhoto(PhotoEntity photo)
	{
		TableOperation operation = TableOperation.Insert(photo);
		CloudTable table = this.ServiceClient.GetTableReference("Photos");
		table.Execute(operation);
	}
	````

	>**Note**: To prepare the insert operation, a **TableOperation** is created to insert the photo entity into the table. Finally, the operation is executed by calling **CloudTable.Execute.**

1. **Update** operations are similar to insert, but first we need to retrieve the entity and then use a **Replace** table operation. Add the following code:

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-DataContextUpdatePhoto_)

	````C#
	public void UpdatePhoto(PhotoEntity photo)
	{
		CloudTable table = this.ServiceClient.GetTableReference("Photos");
		TableOperation retrieveOperation = TableOperation.Retrieve<PhotoEntity>(photo.PartitionKey, photo.RowKey);
		
		TableResult retrievedResult = table.Execute(retrieveOperation);
		PhotoEntity updateEntity = (PhotoEntity)retrievedResult.Result;

		if (updateEntity != null)
		{
			 updateEntity.Description = photo.Description;
			 updateEntity.Title = photo.Title;

			 TableOperation replaceOperation = TableOperation.Replace(updateEntity);
			 table.Execute(replaceOperation);
		}
	}
	````

1. To delete an entity, we need to first retrieve it from the table and then execute a **Delete** table operation. Add the following code to implement it:

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-DataContextDeletePhoto_)

	````C#
	public void DeletePhoto(PhotoEntity photo)
	{
		CloudTable table = this.ServiceClient.GetTableReference("Photos");
		TableOperation retrieveOperation = TableOperation.Retrieve<PhotoEntity>(photo.PartitionKey, photo.RowKey);
		TableResult retrievedResult = table.Execute(retrieveOperation);
		PhotoEntity deleteEntity = (PhotoEntity)retrievedResult.Result;

		if (deleteEntity != null)
		{
			 TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
			 table.Execute(deleteOperation);
		}
	}
	````

1. Open **HomeController.cs** under **Controllers** folder. We'll update the controller's actions to execute the table operations from the DataContext you created in the previous steps. Add the following using directives.

	````C#
	using Microsoft.WindowsAzure;
	using Microsoft.WindowsAzure.Storage;
	using Microsoft.WindowsAzure.Storage.Table;
	````

1. Add a private field to create a _StorageAccount_ object. This object will be used to perform operations for each storage service.

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-StorageAccountVariable_)

	<!-- mark:3 -->
	````C#
	public class HomeController : Controller
	{
		private CloudStorageAccount StorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

		...
	}
	````

1.	In order to display the entities in the View, you will convert them to a ViewModel class. You are going to add two helper methods to convert from **ViewModel** to a **Model** and from a **Model** to a **ViewModel**. Add the following methods at the end of the class declaration.

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-ViewModelHelpers_)

	````C#
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
		var photo = new PhotoEntity
		{
			Title = photoViewModel.Title,
			Description = photoViewModel.Description
		};

		photo.PartitionKey = photoViewModel.PartitionKey ?? photo.PartitionKey;
		photo.RowKey = photoViewModel.RowKey ?? photo.RowKey;
		return photo;
	}
	````

1. The **Home** page will diplay a list of entities from the table storage. To do so, replace the **Index** action to retrieve the entire list of entities from the table storage using the **PhotoDataServiceContext** with the following code.

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-TableStorageIndex_)

	````C#
	public ActionResult Index()
	{
		CloudTableClient cloudTableClient = this.StorageAccount.CreateCloudTableClient();
		var photoContext = new PhotoDataServiceContext(cloudTableClient);
		return this.View(photoContext.GetPhotos().Select(x => this.ToViewModel(x)).ToList());
	}
	````

1.	The **Details** view will show specific information of a particular Photo. Replace the **Details** action with the following code to display the information of a single entity using the **PhotoDataServiceContext**.

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-TableStorageDetails_)

	````C#
	public ActionResult Details(string id)
	{
		CloudTableClient cloudTableClient = this.StorageAccount.CreateCloudTableClient();
		var photoContext = new PhotoDataServiceContext(cloudTableClient);
		PhotoEntity photo = photoContext.GetPhotos().SingleOrDefault(x => x.RowKey.Equals(id));

		if (photo == null)
		{
			 return HttpNotFound();
		}

		var viewModel = this.ToViewModel(photo);
		return this.View(viewModel);
	}
	````

1.	Replace the **Create** _POST_ action with the following code to insert a new entity in the table storage.

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-TableStorageCreate_)

	````C#
	[HttpPost]
	public ActionResult Create(PhotoViewModel photoViewModel, HttpPostedFileBase file, FormCollection collection)
	{
		if (this.ModelState.IsValid)
		{
			var photo = this.FromViewModel(photoViewModel);

			// Save information to Table Storage
			CloudTableClient cloudTableClient = this.StorageAccount.CreateCloudTableClient();
			var photoContext = new PhotoDataServiceContext(cloudTableClient);
			photoContext.AddPhoto(photo);

			return this.RedirectToAction("Index");
		}

		return this.View();
	}
	````

1.	Replace the **Edit** _GET_ Action with the following code to retrieve existing entity information from the table.

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-TableStorageEdit_)

	````C#
	public ActionResult Edit(string id)
	{
		CloudTableClient cloudTableClient = this.StorageAccount.CreateCloudTableClient();
		var photoContext = new PhotoDataServiceContext(cloudTableClient);
		PhotoEntity photo = photoContext.GetPhotos().SingleOrDefault(x => x.RowKey.Equals(id));

		if (photo == null)
		{
			return this.HttpNotFound();
		}

		var viewModel = this.ToViewModel(photo);

		return this.View(viewModel);
	}
	````

1.	Replace the **Edit** _POST_ action with the following code to update an existing entity in the table storage.

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-TableStoragePostEdit_)

	````C#
	[HttpPost]
	[ValidateAntiForgeryToken]
	public ActionResult Edit(PhotoViewModel photoViewModel, FormCollection collection)
	{
		if (ModelState.IsValid)
		{
			var photo = this.FromViewModel(photoViewModel);

			//Update information in Table Storage
			CloudTableClient cloudTableClient = this.StorageAccount.CreateCloudTableClient();
			var photoContext = new PhotoDataServiceContext(cloudTableClient);
			photoContext.UpdatePhoto(photo);

			return this.RedirectToAction("Index");
		}

		return this.View();
	}
	````

1.	Replace the **Delete** _GET_ Action with the following code to retrieve existing entity data from the table storage.

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-TableStorageDelete_)

	````C#
	public ActionResult Delete(string id)
	{
		CloudTableClient cloudTableClient = this.StorageAccount.CreateCloudTableClient();
		var photoContext = new PhotoDataServiceContext(cloudTableClient);
		PhotoEntity photo = photoContext.GetPhotos().SingleOrDefault(x => x.RowKey.Equals(id));

		if (photo == null)
		{
			return this.HttpNotFound();
		}

		var viewModel = this.ToViewModel(photo);

		return this.View(viewModel);
	}
	````

1.	Replace the **DeleteConfirmed** action with the following code to delete an existing entity from the table.

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-TableStoragePostDelete_)
	
	````C#
	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public ActionResult DeleteConfirmed(string id)
	{
		//Delete information From Table Storage
		CloudTableClient cloudTableClient = this.StorageAccount.CreateCloudTableClient();
		var photoContext = new PhotoDataServiceContext(cloudTableClient);
		var photo = photoContext.GetPhotos().SingleOrDefault(x => x.RowKey.Equals(id));
		photoContext.DeletePhoto(photo);

		return this.RedirectToAction("Index");
	}
	````

1. In order to be able to work with table storage, we first need to have the table created. Data tables should only be created once. Typically, you would do this during a provisioning step and rarely in application code. The **Application_Start** method in the **Global.asax** class is a recommended place for this initialization logic. To do so, open **Global.asax.cs** and add the following using directives.

	````C#
	using Microsoft.WindowsAzure;
	using Microsoft.WindowsAzure.Storage;
	using Microsoft.WindowsAzure.Storage.Table;
	````

1. Add the following code at the end of the **Application_Start** method.

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-TableStorageAppStart_)

	<!-- mark:10-13 -->
	````C#
	protected void Application_Start()
	{
		AreaRegistration.RegisterAllAreas();

		WebApiConfig.Register(GlobalConfiguration.Configuration);
		FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
		RouteConfig.RegisterRoutes(RouteTable.Routes);
		BundleConfig.RegisterBundles(BundleTable.Bundles);

		CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
		CloudTableClient cloudTableClient = storageAccount.CreateCloudTableClient();
		CloudTable table = cloudTableClient.GetTableReference("Photos");
		table.CreateIfNotExists();
	}
	````

1.	Press **F5** and run the application.

	![Index Home Page](Images/index-home-page.png?raw=true "Index Home Page")

	_Index Home Page_

1. Create a new entity. To do so, click the **Create** link.

1. Complete the **Title** and **Description** fields and **Submit** the form

	![Create Image Form](Images/create-image-form.png?raw=true "Create Image Form")

	_Create Image Form_

	> **Note**: You can ignore the Upload file input in this exercise.

1. Close the browser to stop the application.

<a name="Ex3Task3" />
#### Task 3 - Working with Blobs ####

In this task you will configure the MVC application to upload images to Blob Storage. 

1.	Open **HomeController.cs** and add the following directives to work with Blobs.

	````C#
	using Microsoft.WindowsAzure.Storage.Blob;
	````

1. Add the following helper method at the end of the class that allows you to retrieve the blob container from the storage account that will be used to store the images.

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-BlobHelper_)
	
	````C#
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
	````

1.	Now, you will update the **Create** action of the **HomeController** to upload an image to a blob. You will save the blob reference name in the table to reference it in the future. To do this, add the following code in the **Create** _POST_ action method.

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-BlobCreate_)

	<!-- mark:8-20 -->
	````C#
	[HttpPost]
	public ActionResult Create(PhotoViewModel photoViewModel, HttpPostedFileBase file, FormCollection collection)
	{
		if (this.ModelState.IsValid)
		{
			var photo = this.FromViewModel(photoViewModel);

			if (file != null)
			{
				// Save file stream to Blob Storage
				var blob = this.GetBlobContainer().GetBlockBlobReference(file.FileName);
				blob.Properties.ContentType = file.ContentType;
				blob.UploadFromStream(file.InputStream);
				photo.BlobReference = file.FileName;
			}
			else
			{
				this.ModelState.AddModelError("File",new ArgumentNullException("file"));
				return this.View(photoViewModel);
			}

			//Save information to Table Storage
			...
		}

		return this.View();
	}
	````

1.	In the **Details** action, you will need to display the image that was stored in the blob container. To do this, you need to retrieve the URL using the **Blob Reference** name that was saved when creating a new entity. Add the following code.

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-BlobDetails_)

	<!-- mark:6-9 -->
	````C#
	public ActionResult Details(string id)
	{
		...

		var viewModel = this.ToViewModel(photo);
		if (!string.IsNullOrEmpty(photo.BlobReference))
		{
			viewModel.Uri = this.GetBlobContainer().GetBlockBlobReference(photo.BlobReference).Uri.ToString();
		}

		return this.View(viewModel);
	}	
	````

1.	Add the same line of code for the **Edit** _GET_ Action to get the image when editing.

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-BlobEdit_)
	<!-- mark:6-9 -->
	````C#
	public ActionResult Edit(string id)
	{
		...

		var viewModel = this.ToViewModel(photo);
		if (!string.IsNullOrEmpty(photo.BlobReference))
		{
			viewModel.Uri = this.GetBlobContainer().GetBlockBlobReference(photo.BlobReference).Uri.ToString();
		}

		return this.View(viewModel);
	}	
	````

1. Add the same code line for the **Delete** _GET_ Action to get the image when deleting.

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-BlobDelete_)

	<!-- mark:6-9 -->
	````C#
	public ActionResult Delete(string id)
	{
		...

		var viewModel = this.ToViewModel(photo);
		if (!string.IsNullOrEmpty(photo.BlobReference))
		{
			viewModel.Uri = this.GetBlobContainer().GetBlockBlobReference(photo.BlobReference).Uri.ToString();
		}

		return this.View(viewModel);
	}	
	````
	
1.	To delete the blob from the container, you will use the blob reference name to retrieve the container and perform a _delete_ operation to it. To do this, add the following code to the **DeleteConfirmed** action.

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-BlobPostDelete_)

	<!-- mark:8-13 -->
	````C#
	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public ActionResult DeleteConfirmed(string id)
	{
		//Delete information From Table Storage
		...

		//Deletes the Image from Blob Storage
		if (!string.IsNullOrEmpty(photo.BlobReference))
		{
			var blob = this.GetBlobContainer().GetBlockBlobReference(photo.BlobReference);
			blob.DeleteIfExists();
		}

		return this.RedirectToAction("Index");
	}
	````

1.	Press **F5** to run the application.

1. Browse for an image, insert a title and a description for it and then click **Create** to perform the upload.

	![Upload image](Images/upload-image.png?raw=true "Upload image")

	_Upload image_

	> **Note:** You can use one of the images that are included in this lab, under the **Assets** folder.

1. Go to the **Details** page and see the image uploaded successfully and then close the browser.

<a name="Ex3Task4" />
#### Task 4 - Working with Queues ####

In this task, you will use queues to simulate a notification service, where a message is sent to a worker role for processing.

1.	Open **HomeController.cs** and add the following directive.

	````C#
	using Microsoft.WindowsAzure.Storage.Queue;
	````

1.	You will add the following helper method at the end of the class to retrieve the **Cloud Queue** object.

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-QueueHelper_)

	````C#
	private CloudQueue GetCloudQueue()
	{
		var queueClient = this.StorageAccount.CreateCloudQueueClient();
		var queue = queueClient.GetQueueReference("messagequeue");
		queue.CreateIfNotExists();
		return queue;
	}
	````

1.	To notify when a new photo is uploaded, you must insert a message to the **Queue** with the specific text to display. Add the following highlighted code in the **Create** _POST_ action method.

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-QueueSendMessageCreate_)

	<!-- mark:9-11 -->
	````C#
	[HttpPost]
	public ActionResult Create(PhotoViewModel photoViewModel, HttpPostedFileBase file, FormCollection collection)
	{
		if (this.ModelState.IsValid)
		{
			...
			photoContext.AddPhoto(photo);

			//Send create notification
			var msg = new CloudQueueMessage("Photo Uploaded");
			this.GetCloudQueue().AddMessage(msg);

			return this.RedirectToAction("Index");
		}

		return this.View();
	}	
	````

1.	To notify that a photo was deleted, you will insert a message to the **Queue** with the specific text to display. Add the following highlighted code to the **DeleteConfirmed** action method.

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-QueueSendMessageDelete_)

	<!-- mark:7-9 -->
	````C#
	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public ActionResult DeleteConfirmed(string id)
	{
		...

		//Send delete notification
		var msg = new CloudQueueMessage("Photo Deleted");
		this.GetCloudQueue().AddMessage(msg);

		return this.RedirectToAction("Index");
	}
	````

1. Open the **WorkerRole.cs** file located in the **QueueProcessor_WorkerRole** project.

1. The worker role will read the **Queue** for notification messages. First, you need to get a queue reference. To do this, add following highlighted code in the **Run** method.

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-QueueWorkerAccount)

	<!-- mark:6-11 -->
	````C#
	public override void Run()
	{
		// This is a sample worker implementation. Replace with your logic.
		Trace.TraceInformation("QueueProcessor_WorkerRole entry point called", "Information");

		// Initialize the account information
		var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
		
		// retrieve a reference to the messages queue
		var queueClient = storageAccount.CreateCloudQueueClient();
		var queue = queueClient.GetQueueReference("messagequeue");

		while (true)
		{
			Thread.Sleep(10000);
			Trace.TraceInformation("Working", "Information");			
		}
	}
	````

1. Now, add the following code inside the **while** block to read messages from the queue.

	(Code Snippet - _GettingStartedWindowsAzureStorage - Ex3-QueueReadingMessages_)

	<!-- mark:9-18 -->
	````C#
	public override void Run()
	{		
		...

		while (true)
		{
			Thread.Sleep(10000);
			Trace.TraceInformation("Working", "Information");		
			if (queue.Exists())
			{
				var msg = queue.GetMessage();

				if (msg != null)
				{
					Trace.TraceInformation(string.Format("Message '{0}' processed.", msg.AsString));
					queue.DeleteMessage(msg);
				}
			}	
		}
	}
	````

	> **Note:** The worker process will try to get a message from the queue every 10 seconds using the **GetMessage** method. If there are messages in the queue, it will show them in the Compute Emulator log.

1. Press **F5** to run the application. Once the browser is opened, upload a new image.

1. Open the **Compute Emulator**. To do so, right-click the Windows Azure icon tray and select **Show Compute Emulator UI**.

	![Windows Azure Tray Icon](Images/windows-azure-tray-icon.png?raw=true "Windows Azure Tray Icon")

	_Windows Azure Tray Icon_

1. Select the worker role instance. Wait until the process reads the message from the queue.

	![Worker role processing the queue](Images/worker-role-processing-the-queue.png?raw=true "Worker role processing the queue")

	_Worker role processing the queue_

<a name="Ex3Task5" />
#### Task 5 - Verification with Visual Studio####

In this task, you will use Visual Studio to inspect the Windows Azure Storage Account.

1. If not already opened, open **Visual Studio Express 2012 for Web**.

1. Go to **View** menu, and open **Database Explorer**.

1. In the Database Explorer pane, right-click **Windows Azure Storage** and select **Add New Storage Account**.

	![Database Explorer](Images/database-explorer.png?raw=true "Database Explorer")

	_Database Explorer_

1. Select **Manually entered credentials** and complete the **Account name** and **Account key** fields with the keys of the storage account you've created in Exercise 1. Click **OK**.

	![Add New Storage Account](Images/add-new-storage-account.png?raw=true "Add New Storage Account")

	_Add New Storage Account_

1. Expand the storage account you configured in the Server Explorer. Notice that there is an entry for Tables, Blobs and Queues.

1. Expand the **Tables** container. You will see the **Photos** table inside it.

	![Photos Table in Database Explorer](Images/photos-table-in-database-explorer.png?raw=true "Photos Table in Database Explorer")

	_Photos Table in Database Explorer_

1. Right-click the **Photos** table and select **View Table**.

	![Photos Table](Images/photo-table.png?raw=true "Photos Table")

	_Photos Table_

	> **Note**: You can see the data you've created in the previous task. Notice the blob reference column. This column references the name of a blob storage.

1. Expand the **Blobs** container. Right-click the gallery blob and select **View Blob Container**. 

	![Gallery Blob Container](Images/blob-container.png?raw=true "Gallery Blob Container")

	_Gallery Blob Container_

---

<a name="Exercise4" />
### Exercise 4: Introducing SAS (Shared Access Signature) ###

Shared Access Signatures allow granular access to tables, queues, blob containers, and blobs. A SAS token can be configured to provide specific access rights, such as read, write, update, delete, etc. to a specific table, key range within a table, queue, blob, or blob container; for a specified time period or without any limit. The SAS token appears as part of the resource’s URI as a series of query parameters.

In this exercise you will learn how to use Shared Access Signatures with the three storage abstractions: Tables, Blobs, and Queues.

<a name="Ex4Task1" />
#### Task 1 - Adding SAS at table level  ####

In this task you will learn how to create SAS for Azure tables. SAS for table allows owners to grant SAS token access by restricting the operations in several ways.

You can grant access to an entire table, to a table range (for example, to all the rows under a particular partion key), or some specific rows. Additionally, you can grant access rights to the specified table or table range such as _Query_, _Add_, _Update_, _Delete_ or a combination of them. Finally, you can specify the SAS token access time.

1. First Step.

<a name="Ex4Task2" />
#### Task 2 - Adding SAS at Blob level  ####

In this task you will learn how to create SAS for Azure Blobs. SAS can be created for blobs and for blobs containers. SAS tokens can be used on blogs to read, update and delete the specified blob. Regarding blob containers, SAS tokens can be used to list the content of the container, and to create, read, update and delete blobs in it.

1. First Step.

<a name="Ex4Task3" />
#### Task 3 - Adding SAS at Queue level  ####

1. First Step.

---

<a name="summary" />
## Summary ##

By completing this hands-on lab you have learned how to:

* Create a Storage Account.
* Enabling Geo-Replication.
* Configure Monitoring metrics for your account.
* Configure Logging for each service.
* Consume Storage Services from a Web Application.

---

