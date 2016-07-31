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
    public class varsEntity : TableEntity
    {
        public varsEntity(string a, string b)
        {
            this.PartitionKey = a;
            this.RowKey = b;
        }

        public varsEntity() { }

        public string value { get; set; }

    }

    public class RakaatController : ApiController
    {
        public ApiServices Services { get; set; }


        async public Task<string> GetRakaat()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<varsEntity>("rakaat", "rakaat");

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

            TableOperation retrieveOperation = TableOperation.Retrieve<varsEntity>("rakaat", "rakaat");

            TableResult retrievedResult = await varsTable.ExecuteAsync(retrieveOperation);

            varsEntity updateEntity = (varsEntity)retrievedResult.Result;

            updateEntity.value = rakaat;

            TableOperation updateOperation = TableOperation.Replace(updateEntity);

            // Execute the operation.
            await varsTable.ExecuteAsync(updateOperation);

        }

    }
}
