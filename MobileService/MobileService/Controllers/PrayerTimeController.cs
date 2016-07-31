using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;

using System.Xml;

namespace MobileService.Controllers
{
    public class PrayerTimeController : ApiController
    {
        public ApiServices Services { get; set; }

        public string GetPrayer()
        {
            TimeZoneInfo myTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
            DateTime currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, myTimeZone);

            /*XmlDocument doc1 = new XmlDocument();
            doc1.Load("http://api.xhanch.com/islamic-get-prayer-time.php?lng=35.00931&lat=32.76158&yy="+ currentDateTime.Year.ToString()+
                "&mm="+ currentDateTime.Month.ToString()+"&gmt=2");
            XmlElement root = doc1.DocumentElement;*/



            string str= /*(root.GetElementsByTagName("fajr"))[currentDateTime.Day-1].InnerXml;*/"02:54";
            string fajrHour = "" + str[0] + str[1];
            string fajrMinute = "" + str[3] + str[4];

            string str2 = /*(root.GetElementsByTagName("zuhr"))[currentDateTime.Day - 1].InnerXml;*/"11:37";
            string zuhrHour = "" + str2[0] + str2[1];
            string zuhrMinute = "" + str2[3] + str2[4];

            string str3 = /*(root.GetElementsByTagName("asr"))[currentDateTime.Day - 1].InnerXml;*/"15:17";
            string asrHour = "" + str3[0] + str3[1];
            string asrMinute = "" + str3[3] + str3[4];

            string str4 = /*(root.GetElementsByTagName("maghrib"))[currentDateTime.Day - 1].InnerXml;*/"18:48";
            string maghribHour = "" + str4[0] + str4[1];
            string maghribMinute = "" + str4[3] + str4[4];

            

            string str5 = /*(root.GetElementsByTagName("isha"))[currentDateTime.Day - 1].InnerXml;*/"20:20";
            string ishaHour = "" + str5[0] + str5[1];
            string ishaMinute = "" + str5[3] + str5[4];

            if (compareDate(currentDateTime.Hour-1, currentDateTime.Minute, int.Parse(fajrHour), int.Parse(fajrMinute),int.Parse(zuhrHour),int.Parse(zuhrMinute))==true){
                return "Fajr";
            }

            if (compareDate(currentDateTime.Hour-1, currentDateTime.Minute, int.Parse(zuhrHour) , int.Parse(zuhrMinute) , int.Parse(asrHour) , int.Parse(asrMinute) ) == true)
            {
                return "Dhuhor";
            }

            if (compareDate(currentDateTime.Hour - 1, currentDateTime.Minute, int.Parse(asrHour) , int.Parse(asrMinute) , int.Parse(maghribHour) , int.Parse(maghribMinute) ) == true)
            {
                return "Asr";
            }

            if (compareDate(currentDateTime.Hour - 1, currentDateTime.Minute, int.Parse(maghribHour) , int.Parse(maghribMinute) , int.Parse(ishaHour) , int.Parse(ishaMinute) ) == true)
            {
                return "Maghrib";
            }
            return "Ishaa";

        }
        private bool compareDate(int x,int y,int a,int b,int c,int d)
        {
            if (x < a)
            {
                return false;
            }
            if (x > c)
            {
                return false;
            }
            if(x==a && y < b)
            {
                return false;
            }
            if (x == c && y >= d)
            {
                return false;
            }
            return true;

        }

    }
}
