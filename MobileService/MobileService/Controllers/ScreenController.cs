using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Azure;

namespace MobileService.Controllers
{
    public class ScreenController : ApiController
    {
        public ApiServices Services { get; set; }

        async public Task<string> GetDataRequest()
        {
            TableOperation retrieveOperation1 = TableOperation.Retrieve<varsGenderEntity>("gender", "gender");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                 CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable varsTable = tableClient.GetTableReference("varsTable");
            TableResult retrievedResult1 = await varsTable.ExecuteAsync(retrieveOperation1);

            DateTime date = ((varsGenderEntity)(retrievedResult1.Result)).first;

            TimeZoneInfo myTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, myTimeZone);

            TimeSpan span = now - date;

            string gender = await (new GenderController()).GetGender();
            string prayer = (new PrayerTimeController()).GetPrayer();

            return "MusallahData "+ gender + " " + prayer+ " " + "-"+span.Minutes.ToString("00") + ":" + span.Seconds.ToString("00");

        }

    }
}
