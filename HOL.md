<a name="Title" />
# Getting Started With Windows Azure Storage #

---
<a name="Overview" />
## Overview ##

In this lab, you will learn the basics of Windows Azure Storage, how to create and configure storage accounts and how you can programmatically can access the different types of storage service. **Blobs**, **Tables**, and **Queues** are all available as part of the **Windows Azure Storage** account, and provide durable storage on the Windows Azure platform. All are accessible from both inside and outside the Windows Azure platform by using classes in the [Windows Azure Storage Client SDK](http://msdn.microsoft.com/en-us/library/microsoft.windowsazure.storageclient.aspx), or via URI using [REST APIs]  (http://msdn.microsoft.com/en-us/library/dd179355.aspx). Windows Azure Table Storage also supports LINQ access.

The **Windows Azure Storage** offers the following services:

![storage-diagram](Images/storage-diagram.png?raw=true)

**Table Storage**

Table storage is a collection of row like entities, each of which can contain up to 255 properties; however unlike tables in a database, there is no schema that enforces a certain set of values on all the rows within a table. And while a table stores structured data, it does not provide any way to represent relationships between data. Windows Azure Storage tables are more like rows within a spreadsheet application such as Excel than rows within a database such as SQL Database, in that each row can contain a different number of columns, and of different data types, than the other rows in the same table.

**Blog Storage**

Blobs provide a way to store large amounts of unstructured, binary data, such as video, audio, images, etc.  In fact, one of the features of blobs is streaming content such as video or audio.

**Queue Storage**

Queues provide storage for passing messages between applications, similar to Microsoft Message Queuing (MSMQ.) Messages stored to the queue are limited to a maximum of 8KB in size, and are generally stored and retrieved on a first in, first out (FIFO,) basis; however FIFO is not guaranteed. Processing messages from a queue is a two stage process, which involves getting the message, and then deleting the message after it has been processed.  This pattern allows you to implement guaranteed message delivery by leaving the message in the queue until it has been fully processed.


<a name="Objectives" />
### Objectives ###

In this hands-on lab, you will learn how to:

* Create an Storage Account.
* Learn the different configuration options for Geo-Replication, Monitoring and Logging.
* Access to Tables, Blobs and Queues using **Windows Azure SDK 2.0** in a MVC Web Application.

<a name="Prerequisites" />
### Prerequisites ###

The following is required to complete this hands-on lab:

- [Microsoft Visual Studio 2012] [1]
- [Windows Azure Tools for Microsoft Visual Studio 2.0] [2]
- A Windows Azure subscription - [sign up for a free trial][3]

[1]: http://msdn.microsoft.com/vstudio/products/
[2]: http://www.microsoft.com/windowsazure/sdk/
[3]: http://aka.ms/WATK-FreeTrial

<a name="Setup" />
### Setup ###
In order to execute the exercises in this hands-on lab you need to set up your environment.

1. Open a Windows Explorer window and browse to the lab’s **Source** folder.

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

This exercise describes how to create a storage account in the Windows Azure Management Portal. To store files and data in the Blob, Table, and Queue services in Windows Azure, you must create a storage account in the geographic region where you want to store the data. A storage account can contain up to 100 TB of blob, table, and queue data. You can create up to five storage accounts for each Windows Azure subscription.
 
<a name="Ex1Task1" />
#### Task 1 – Creating an Storage Account from Management Portal ####

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
#### Task 1 – Enabling Geo-Replication ####

When geo-replication is turned on for a storage account, the stored content is replicated to a secondary location to enable failover to that location in case of a major disaster in the primary location. The secondary location is in the same region, but is hundreds of miles from the primary location. This is the highest level of storage durability, known as geo redundant storage (GRS). Geo-replication is turned on by default.

1.	In the Storage Account page, click the **Configure** tab from the top menu.

	![configure-storage-menu](Images/configure-storage-menu.png?raw=true)

	_Configuring Storage Account_

1.  You can choose to enable or disable it in the **Geo-Replication** section.

	![configuring-storage-georeplication](Images/configuring-storage-georeplication.png?raw=true)

	_Enabling Geo-Replication_

	> **Note:** If you turn off geo-replication, you have locally redundant storage (LRS). For locally redundant storage, account data is replicated three times within the same data center. LRS is offered at discounted rates. Be aware that if you turn off geo-replication, and you later change your mind, you will incur a one-time data cost to replicate your existing data to the secondary location.

<a name="Ex2Task2" />
#### Task 2 – Configuring Monitoring ####

You can monitor your storage accounts in the Windows Azure Management Portal. For each storage service associated with the storage account (Blob, Queue, and Table), you can choose the level of monitoring - minimal or verbose - and specify the appropriate data retention policy. You can do this in the **Monitoring** section.

1. In the **Configure** page, go to the **Monitoring** section.

	![configuring-storage-monitoring](Images/configuring-storage-monitoring.png?raw=true)

	_Configuring Monitoring Options_

1.	To set the monitoring level, select one of the following:

	**Minimal** - Collects metrics such as ingress/egress, availability, latency, and success percentages, which are aggregated for the Blob, Table, and Queue services.

	**Verbose** – In addition to the minimal metrics, collects the same set of metrics for each storage operation in the Windows Azure Storage Service API. Verbose metrics enable closer analysis of issues that occur during application operations.

	**Off** - Turns off monitoring. Existing monitoring data is persisted through the end of the retention period.

	> **Note:** There are costs considerations when you select monitoring. For more information, see [Storage Analytics and Billing](http://msdn.microsoft.com/en-us/library/windowsazure/hh360997.aspx).

1. To set the data retention policy, in **Retention** (in days), type the number of days of data to retain from 1-365 days. If you do not want to set a retention policy, enter zero. If there is no retention policy, it is up to you to delete the monitoring data. We recommend setting a retention policy based on how long you want to retain storage analytics data for your account so that old and unused analytics data can be deleted by system at no cost.

1. Once Monitoring is enabled, you can customize the **Dashboard** to choose up to six metrics to plot on the metrics chart. There are nine available metrics for each service. To configure this, go to the **Dashboard** page.

	![storage-dashboard-menu](Images/storage-dashboard-menu.png?raw=true)

1.	In the **Dashboard** page, you will see the default metrics displayed on the chart. To add a different metric, click on the **More** button to display the available metrics. Select one from the list.

	![adding-metrics-dashboard](Images/adding-metrics-dashboard.png?raw=true)
	
	_Adding Metrics to the Dashboard_

	> **Note:** You can hide metrics that are plotted on the chart by clearing the check box by the metric header.

1.	By default, the chart shows trends, displaying only the current value of each metric (the **Relative** option at the top of the chart). To display a Y axis so you can see absolute values, select **Absolute**.

	![dashboard-absolute-values](Images/dashboard-absolute-values.png?raw=true)

	_Changing Chart values to Absolute_

1.	To change the time range the metrics chart displays, select **6 hours**, 2**4 hours**, or **7 days** at the top of the chart.

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
#### Task 3 – Configuring Logging ####

For each of the storage services available with your storage account (Blob, Table, and Queue), you can save diagnostics logs for Read Requests, Write Requests, and/or Delete Requests, and can set the data retention policy for each of the services. In this task you will configure logging for your storage account.

1. In the **Configure** page, go to the **Logging** section.

1.	For each service (Blob, Table, and Queue), you can configure the types of request to log: Read Requests, Write Requests, and Delete Requests. You can also configure the number of days to retain the logged data. Enter zero is if you do not want to set a retention policy. If you do not set a retention policy, it is up to you to delete the logs.

	![configuring-storage-logging](Images/configuring-storage-logging.png?raw=true)

	_Configuring Logging Options_

	> **Note:** The diagnostics logs are saved in a blob container named **$logs** in your storage account. For information about accessing the $logs container, see [About Storage Analytics Logging](http://msdn.microsoft.com/en-us/library/windowsazure/hh343262.aspx).


<a name="Ex2Task4" />
#### Task 4 – Managing Account Keys ####
When you create a storage account, Windows Azure generates two 512-bit storage access keys, which are used for authentication when the storage account is accessed. By providing two storage access keys, Windows Azure enables you to regenerate the keys with no interruption to your storage service or access to that service.

1.	In the Storage Account Dashboard, select the option **Manage Access Keys** from the bottom menu.

	![manage-keys-menu](Images/manage-keys-menu.png?raw=true)

	_Managing Access Keys_

1. You can use **Manage Keys** to copy a storage access key to use in a connection string. The connection string requires the storage account name and a key to use in authentication. Take note of the Primary access key and the storage account name as they will be used in the following exercise.

	![managing-access-keys](Images/managing-access-keys.png?raw=true)

	_Copying Access Keys_

1.	By clicking the **Regenerate** button, a new Access Key is created. You should change the access keys to your storage account periodically to help keep your storage connections more secure. Two access keys are assigned to enable you to maintain connections to the storage account using one access key while you regenerate the other access key. 

	> **Note:** Regenerating your access keys affects virtual machines, media services, and any applications that are dependent on the storage account.


<a name="Exercise3"></a>
###Exercise 3: Understanding the Windows Azure Storage Abstractions ###


<a name="Ex3Task1" />
#### Task 1 – (TODO: Insert Task description here) ####


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

