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
using System.Xml;

namespace MobileService.Controllers
{
    public class AppController : ApiController
    {
        public ApiServices Services { get; set; }

        async public Task<string> GetFullString()
        {
            TableOperation retrieveOperation1 = TableOperation.Retrieve<varsGenderEntity>("gender", "gender");
            TableOperation retrieveOperation2 = TableOperation.Retrieve<varsEntity>("rakaat", "rakaat");
            TableOperation retrieveOperation3 = TableOperation.Retrieve<varsEntity>("isWaiting", "isWaiting");

            TableOperation retrieveOperation4 = TableOperation.Retrieve<varsEntity>("rakaat1", "rakaat1");

            // Execute the retrieve operation.

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable varsTable = tableClient.GetTableReference("varsTable");

            TableResult retrievedResult1 = await varsTable.ExecuteAsync(retrieveOperation1);
            TableResult retrievedResult2 = await varsTable.ExecuteAsync(retrieveOperation2);
            TableResult retrievedResult3 = await varsTable.ExecuteAsync(retrieveOperation3);

            TableResult retrievedResult4 = await varsTable.ExecuteAsync(retrieveOperation4);




            TimeZoneInfo myTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
            DateTime currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, myTimeZone);

            /*XmlDocument doc1 = new XmlDocument();
            doc1.Load("http://api.xhanch.com/islamic-get-prayer-time.php?lng=35.00931&lat=32.76158&yy=" + currentDateTime.Year.ToString() +
                "&mm=" + currentDateTime.Month.ToString() + "&gmt=2");

            XmlElement root = doc1.DocumentElement;*/
            string gender= ((varsGenderEntity)(retrievedResult1.Result)).value;
            string rakaat= ((varsEntity)(retrievedResult2.Result)).value;
            string isWaiting= ((varsEntity)(retrievedResult3.Result)).value;

            string rakaat1 = ((varsEntity)(retrievedResult4.Result)).value;



            string str = /*(root.GetElementsByTagName("fajr"))[currentDateTime.Day - 1].InnerXml;*/"02:54";
            string fajrtime = "" + (int.Parse(""+str[0]+str[1])+1).ToString("00") + ":"+ str[3]+str[4];

            string str2 = /*(root.GetElementsByTagName("zuhr"))[currentDateTime.Day - 1].InnerXml;*/"11:37";
            string zuhrtime = "" + (int.Parse("" + str2[0] + str2[1]) + 1).ToString("00") + ":" + str2[3] + str2[4];

            string str3 = /*(root.GetElementsByTagName("asr"))[currentDateTime.Day - 1].InnerXml;*/"15:17";
            string asrtime = "" + (int.Parse("" + str3[0] + str3[1]) + 1).ToString("00") + ":" + str3[3] + str3[4];

            string str4 = /*(root.GetElementsByTagName("maghrib"))[currentDateTime.Day - 1].InnerXml;*/"18:48";
            string maghribtime = "" + (int.Parse("" + str4[0] + str4[1]) + 1).ToString("00") + ":" + str4[3] + str4[4];



            string str5 = /*(root.GetElementsByTagName("isha"))[currentDateTime.Day - 1].InnerXml;*/"20:20";
            string ishatime = "" + (int.Parse("" + str5[0] + str5[1]) + 1).ToString("00") + ":" + str5[3] + str5[4];

            PrayerTimeController x = new PrayerTimeController();

            string currentPrayer = x.GetPrayer();
            DateTime a = ((varsGenderEntity)(retrievedResult1.Result)).first;

            TimeSpan span = ((varsGenderEntity)(retrievedResult1.Result)).last - ((varsGenderEntity)(retrievedResult1.Result)).first;

            return "" + gender + " " + rakaat + " " + fajrtime + " " + zuhrtime + " " + asrtime + " " + maghribtime + " " + ishatime
                + " " + currentPrayer + " " + span.Minutes.ToString() + " " +  span.Seconds.ToString() + " " + a.Year
                + " " + a.Month + " " + a.Day + " " + a.Hour + " " + a.Minute + " " + a.Second + " " + currentDateTime.Year +
                " " + currentDateTime.Month+ " " + currentDateTime.Day + " " + currentDateTime.Hour+ " " + currentDateTime.Minute
                + " " + currentDateTime.Second + " " + isWaiting+ " " + rakaat1;

        }

    }
}
