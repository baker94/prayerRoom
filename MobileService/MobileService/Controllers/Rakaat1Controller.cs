using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure;
using System.Threading.Tasks;

namespace MobileService.Controllers
{
    public class Rakaat1Controller : ApiController
    {
        public ApiServices Services { get; set; }


        async public Task<string> GetRakaat()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<varsEntity>("rakaat1", "rakaat1");

            // Execute the retrieve operation.


            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable varsTable = tableClient.GetTableReference("varsTable");

            TableResult retrievedResult = await varsTable.ExecuteAsync(retrieveOperation);

            return ((varsEntity)(retrievedResult.Result)).value;
        }

        public int GetChooseRakaat(string rakaat)
        {
            changeRakaat(rakaat);
            return 1;
        }

        async public void changeRakaat(string rakaat)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable varsTable = tableClient.GetTableReference("varsTable");

            //varsTable.DeleteIfExists();

            TableOperation retrieveOperation = TableOperation.Retrieve<varsEntity>("rakaat1", "rakaat1");

            TableResult retrievedResult = await varsTable.ExecuteAsync(retrieveOperation);

            varsEntity updateEntity = (varsEntity)retrievedResult.Result;

            updateEntity.value = rakaat;

            TableOperation updateOperation = TableOperation.Replace(updateEntity);

            // Execute the operation.
            await varsTable.ExecuteAsync(updateOperation);


            //await varsTable.CreateIfNotExistsAsync();
            /*varsEntity customer1 = new varsEntity("rakaat1", "rakaat1");
            customer1.value = rakaat;

            // Create the TableOperation that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(customer1);

            // Execute the insert operation.
            await varsTable.ExecuteAsync(insertOperation);*/

        }

    }
}
