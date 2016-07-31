using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;

using Microsoft.Azure; // Namespace for CloudConfigurationManager 
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types
using System.Threading.Tasks;

namespace MobileService.Controllers
{
    public class NewPersonController : ApiController
    {
        public ApiServices Services { get; set; }


        async public void GetNewPerson()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable varsTable = tableClient.GetTableReference("varsTable");

            //varsTable.DeleteIfExists();

            TableOperation retrieveOperation = TableOperation.Retrieve<varsGenderEntity>("gender", "gender");

            TableResult retrievedResult = await varsTable.ExecuteAsync(retrieveOperation);

            varsGenderEntity updateEntity = (varsGenderEntity)retrievedResult.Result;
            TimeZoneInfo myTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
            DateTime currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, myTimeZone);

            updateEntity.last = currentDateTime;

            TableOperation updateOperation = TableOperation.Replace(updateEntity);

            // Execute the operation.
            await varsTable.ExecuteAsync(updateOperation);
        }

        async public void GetIsSomeoneWaiting(string isWaiting)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable varsTable = tableClient.GetTableReference("varsTable");

            //varsTable.DeleteIfExists();

            //await varsTable.CreateIfNotExistsAsync();

            /*varsEntity customer1 = new varsEntity("isWaiting", "isWaiting");
            customer1.value = isWaiting;

            // Create the TableOperation that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(customer1);

            // Execute the insert operation.
            await varsTable.ExecuteAsync(insertOperation);*/


            TableOperation retrieveOperation = TableOperation.Retrieve<varsEntity>("isWaiting", "isWaiting");

            TableResult retrievedResult = await varsTable.ExecuteAsync(retrieveOperation);

            varsEntity updateEntity = (varsEntity)retrievedResult.Result;

            updateEntity.value = isWaiting;

            TableOperation updateOperation = TableOperation.Replace(updateEntity);

            // Execute the operation.
            await varsTable.ExecuteAsync(updateOperation);

        }

    }
}
