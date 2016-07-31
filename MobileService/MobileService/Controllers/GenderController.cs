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
    public class varsGenderEntity : TableEntity
    {
        public varsGenderEntity(string a, string b)
        {
            this.PartitionKey = a;
            this.RowKey = b;
        }

        public varsGenderEntity(){ }

        public string value { get; set; }
        public DateTime first { get; set; }
        public DateTime last { get; set; }

    }

    public class GenderController : ApiController
    {
        public ApiServices Services { get; set; }

        public int GetChooseGender(string gender)
        {
            changeGender(gender);
            return 1;
        }




        async public Task<string> GetGender()
        {

            TableOperation retrieveOperation = TableOperation.Retrieve<varsGenderEntity>("gender", "gender");

            // Execute the retrieve operation.
            

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable varsTable = tableClient.GetTableReference("varsTable");

            TableResult retrievedResult = await varsTable.ExecuteAsync(retrieveOperation);

            return ((varsGenderEntity)(retrievedResult.Result)).value;

        }



        async public void changeGender(string gender)
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

            updateEntity.value = gender;
            updateEntity.first = currentDateTime;
            updateEntity.last= currentDateTime;

            TableOperation updateOperation = TableOperation.Replace(updateEntity);

            // Execute the operation.
            await varsTable.ExecuteAsync(updateOperation);



            /*await varsTable.CreateIfNotExistsAsync();
            varsGenderEntity customer1 = new varsGenderEntity("gender", "gender");
            customer1.first = DateTime.Now;
            customer1.last = DateTime.Now;

            // Create the TableOperation that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(customer1);

            // Execute the insert operation.
            await varsTable.ExecuteAsync(insertOperation);*/

        }

    }
}
