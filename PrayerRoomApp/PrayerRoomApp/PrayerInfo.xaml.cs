using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;


namespace PrayerRoomApp
{
    public sealed partial class PrayerInfo : Page
    {
        private string str=null;
        private string gender = null;
        string[] arr;
        string[] status = { "0", "0" };
        private DateTime finishedTime;
        private bool jamaaFinished = false;
        private bool isNotificated1 = false;
        private bool isNotificated2 = false;
        private bool isNotificated3 = false;

        public PrayerInfo()
        {
            this.InitializeComponent();    
        }

        DispatcherTimer dispatcherTimer;

        private void DispatcherTimerSetup()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0,0,1);
            dispatcherTimer.Start();
            //TimerStatus.Text = "dispatcherTimer.IsEnabled = " + dispatcherTimer.IsEnabled + "\n";
        }

        async void dispatcherTimer_Tick(object sender, object e)
        {
            try
            {
                this.str = await App.MobileService.InvokeApiAsync<string>("app/fullstring",
                 HttpMethod.Get, null);
            }
            catch
            {
                noConnectionControl();
                return;
            }


            arr = this.str.ToString().Split(' ');

            if (arr[0] != "Finished")
            {


                this.myFlipview.IsEnabled = true;
                this.image3.Visibility = Visibility.Visible;
                this.image4.Visibility = Visibility.Visible;
                this.PrayerPanerl.Visibility = Visibility.Visible;
                this.NoConnectionPanel1.Visibility = Visibility.Collapsed;
                this.NoConnectionPanel2.Visibility = Visibility.Collapsed;

                this.massageButton.Visibility = Visibility.Visible;
                this.loveButton.Visibility = Visibility.Visible;
                this.shareButton.Visibility = Visibility.Visible;
                this.likeButton.Visibility = Visibility.Visible;


                this.loading.Visibility = Visibility.Collapsed;
                this.image2.Visibility = Visibility.Visible;
                this.controlStackP.Visibility = Visibility.Visible;
                this.textBlock.Visibility = Visibility.Visible;
                if (gender == "boy")
                {
                    BoysButton.Visibility = Visibility.Visible;
                    BoysButton2.Visibility = Visibility.Visible;
                    BoysButton3.Visibility = Visibility.Visible;
                }
                if (gender == "girl")
                {
                    girlsButton.Visibility = Visibility.Visible;
                    girlsButton2.Visibility = Visibility.Visible;
                    girlsButton3.Visibility = Visibility.Visible;
                }

                if (status[0] != arr[0] || status[1] != arr[7])
                {
                    jamaaFinished = false;
                    isNotificated1 = false;
                    isNotificated2 = false;
                    isNotificated3 = false;


                    if (!((gender == "girl" && status[0] == "Girls") || (gender == "boy" && status[0] == "Boys")))
                    {
                        this.rb1.IsEnabled = false;
                        this.rb2.IsEnabled = false;
                        this.rb3.IsEnabled = false;
                        this.rb1.IsChecked = true;
                        this.offNotButton.Visibility = Visibility.Visible;
                        this.onNotButton.Visibility = Visibility.Collapsed;
                        this.notPanel.Visibility = Visibility.Collapsed;

                    }
                }
            }

            if(arr[0] != "Finished")
            {
                status[0] = arr[0];
                status[1] = arr[7];
            }


            DateTime now = new DateTime(int.Parse(arr[16]), int.Parse(arr[17]), int.Parse(arr[18]), int.Parse(arr[19]),
                int.Parse(arr[20]), int.Parse(arr[21]));

            DateTime date = new DateTime(int.Parse(arr[10]), int.Parse(arr[11]), int.Parse(arr[12]), int.Parse(arr[13])
             , int.Parse(arr[14]), int.Parse(arr[15]));

            TimeSpan span = now - date;
            
            if (arr[0] == "Boys" || arr[0] == "Girls")
            {
                this.textBlock.Text = "" + span.Minutes.ToString("00") + ":" + span.Seconds.ToString("00");
                int totalRakaat = getPrayerRakaat(arr[7]);
                if (totalRakaat == int.Parse(arr[1]))
                {
                    if (!jamaaFinished)
                    {
                        finishedTime = now;
                    }
                    jamaaFinished = true;
                }

                TimeSpan remainingTime;
                TimeSpan tmp = new TimeSpan(0, int.Parse(arr[8]), int.Parse(arr[9]));
                if (!jamaaFinished)
                {
                    remainingTime = new TimeSpan(0, totalRakaat - int.Parse(arr[1]), 0) + tmp;
                }
                else
                {
                    remainingTime = tmp - (now - finishedTime);
                }
                if((gender=="boy" && arr[0]=="Girls") || (gender=="girl" && arr[0] == "Boys"))
                {
                    if(rb3.IsChecked==true && rb3.IsEnabled==true)
                        checkNotification3(remainingTime);
                    if(rb2.IsChecked == true && rb2.IsEnabled == true)
                        checkNotification2(remainingTime);
                    if(rb1.IsChecked == true && rb1.IsEnabled == true)
                        checkNotification1(remainingTime);
                }
            }
            if(arr[0]=="Empty")
            {
                this.textBlock.Visibility = Visibility.Collapsed;
                this.textBlock.Text = "00:00";
            }

            if(arr[0]=="Boys" && this.gender == "boy")
            {
                boysInForBoy(span);
            }
            if(arr[0] == "Boys" && this.gender == "girl")
            {
                boysInForGirl();
            }
            if(arr[0]=="Girls" && this.gender == "boy")
            {
                girlsInForBoy();
            }
            if(arr[0]=="Girls" && this.gender == "girl")
            {
                girlsInForGirl(span);
            }
            
            if(arr[0]=="Empty")
            {
                notificationButton1.Visibility = Visibility.Collapsed;
                this.imageCanEnter.Visibility = Visibility.Visible;
                this.imageCantEnter.Visibility = Visibility.Collapsed;
                this.whoseInTextBlock.Visibility = Visibility.Visible;
                this.enterTextBlock.Text= "You can enter";
                this.whoseInTextBlock.Text = "MUSALLAH is empty";
            }
            prayerControl();
        }

        private void checkNotification3(TimeSpan span)
        {
            if ((span.Minutes < 3 || (span.Minutes==3 && span.Seconds==0))
                && !isNotificated3 && onNotButton.Visibility == Visibility.Visible 
                && rb3.IsChecked==true && rb3.IsEnabled)
            {
                MakingToastNotification("remember: ", "remain 3 minutes", "T", "G1", true);
                rb3.IsEnabled = false;
                isNotificated3 = true;
                this.rb1.IsEnabled = false;
                this.rb2.IsEnabled = false;
                this.rb1.IsChecked = true;
                this.offNotButton.Visibility = Visibility.Visible;
                this.onNotButton.Visibility = Visibility.Collapsed;
            }
            if ((span.Minutes < 3 || (span.Minutes == 3 && span.Seconds == 0))
                && !isNotificated3 && rb3.IsChecked == false)
            {
                rb3.IsEnabled = false;
                isNotificated3 = true;
                this.rb1.IsEnabled = false;
                this.rb2.IsEnabled = false;
                this.rb1.IsChecked = true;
                this.offNotButton.Visibility = Visibility.Visible;
                this.onNotButton.Visibility = Visibility.Collapsed;
            }
        }
        private void checkNotification2(TimeSpan span)
        {
            if ((span.Minutes < 2 || (span.Minutes == 2 && span.Seconds == 0))
                && !isNotificated2 && onNotButton.Visibility == Visibility.Visible
                && rb2.IsChecked == true && rb2.IsEnabled)
            {
                MakingToastNotification("remember: ", "remain 2 minutes", "T", "G1", true);
                rb2.IsEnabled = false;
                isNotificated2 = true;
                this.rb1.IsEnabled = false;
                this.rb3.IsEnabled = false;
                this.rb1.IsChecked = true;
                this.offNotButton.Visibility = Visibility.Visible;
                this.onNotButton.Visibility = Visibility.Collapsed;
            }
            if ((span.Minutes < 2 || (span.Minutes == 2 && span.Seconds == 0))
                && !isNotificated2 && rb2.IsChecked == false)
            {
                rb2.IsEnabled = false;
                isNotificated2 = true;
                this.rb1.IsEnabled = false;
                this.rb3.IsEnabled = false;
                this.rb1.IsChecked = true;
                this.offNotButton.Visibility = Visibility.Visible;
                this.onNotButton.Visibility = Visibility.Collapsed;
            }
        }
        private void checkNotification1(TimeSpan span)
        {
            if ((span.Minutes < 1 || (span.Minutes == 1 && span.Seconds == 0)) 
                && !isNotificated1 && onNotButton.Visibility == Visibility.Visible
                && rb1.IsChecked == true && rb1.IsEnabled)
            {
                MakingToastNotification("remember: ", "remain 1 minutes", "T", "G1", true);
                rb1.IsEnabled = false;
                isNotificated1 = true;
                this.rb2.IsEnabled = false;
                this.rb3.IsEnabled = false;
                this.rb1.IsChecked = true;
                this.offNotButton.Visibility = Visibility.Visible;
                this.onNotButton.Visibility = Visibility.Collapsed;
            }
            if ((span.Minutes < 1 || (span.Minutes == 1 && span.Seconds == 0))
                && !isNotificated1 && rb1.IsChecked == false)
            {
                rb1.IsEnabled = false;
                isNotificated1 = true;
                this.rb2.IsEnabled = false;
                this.rb3.IsEnabled = false;
                this.rb1.IsChecked = true;
                this.offNotButton.Visibility = Visibility.Visible;
                this.onNotButton.Visibility = Visibility.Collapsed;
            }
        }

        private int getPrayerRakaat(string prayer)
        {
            if(prayer== "Fajr")
            {
                return 4;
            }
            if(prayer== "Dhuhor")
            {
                return 8;
            }
            if (prayer == "Asr")
            {
                return 4;
            }
            if(prayer== "Maghrib")
            {
                return 5;
            }
            return 7;
        }

        private void boysInForBoy(TimeSpan span)
        {
            if (arr[22] == "0" || (arr[22]=="1" && arr[7]=="Dhuhor" && (span.Minutes<3 || (span.Minutes==3 && span.Seconds<30) ))
                || (arr[22] == "1" && arr[7] != "Dhuhor" && span.Minutes < 2))
            {
                notificationButton1.Visibility = Visibility.Collapsed;
                this.imageCanEnter.Visibility = Visibility.Visible;
                this.imageCantEnter.Visibility = Visibility.Collapsed;
                this.whoseInTextBlock.Visibility = Visibility.Visible;
                this.enterTextBlock.Text = "You can enter";
                this.whoseInTextBlock.Text = "Boys are in";
            }else
            {
                if (this.notPanel.Visibility == Visibility.Collapsed)
                {
                    this.notificationButton1.Visibility = Visibility.Visible;
                }
                this.imageCanEnter.Visibility = Visibility.Collapsed;
                this.imageCantEnter.Visibility = Visibility.Visible;
                this.enterTextBlock.Text = "You can't enter";
                this.whoseInTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void boysInForGirl()
        {
            if (this.notPanel.Visibility == Visibility.Collapsed)
            {
                this.notificationButton1.Visibility = Visibility.Visible;
            }
            this.imageCanEnter.Visibility = Visibility.Collapsed;
            this.imageCantEnter.Visibility = Visibility.Visible;
            this.whoseInTextBlock.Visibility = Visibility.Visible;
            this.enterTextBlock.Text = "You can't enter";
            this.whoseInTextBlock.Text = "Boys are in";

        }

        private void girlsInForBoy()
        {
            if (this.notPanel.Visibility == Visibility.Collapsed)
            {
                this.notificationButton1.Visibility = Visibility.Visible;
            }
            this.imageCanEnter.Visibility = Visibility.Collapsed;
            this.imageCantEnter.Visibility = Visibility.Visible;
            this.whoseInTextBlock.Visibility = Visibility.Visible;
            this.enterTextBlock.Text = "You can't enter";
            this.whoseInTextBlock.Text = "Girls are in";
        }

        private void girlsInForGirl(TimeSpan span)
        {
            if (arr[22] == "0" || (arr[22] == "1" && arr[7] == "Dhuhor" && (span.Minutes < 3 || (span.Minutes == 3 && span.Seconds < 30)))
                || (arr[22] == "1" && arr[7] != "Dhuhor" && span.Minutes < 2))
            {
                notificationButton1.Visibility = Visibility.Collapsed;
                this.imageCanEnter.Visibility = Visibility.Visible;
                this.imageCantEnter.Visibility = Visibility.Collapsed;
                this.whoseInTextBlock.Visibility = Visibility.Visible;
                this.enterTextBlock.Text = "You can enter";
                this.whoseInTextBlock.Text = "Girls are in";
            }
            else
            {
                if (this.notPanel.Visibility == Visibility.Collapsed)
                {
                    this.notificationButton1.Visibility = Visibility.Visible;
                }
                this.imageCanEnter.Visibility = Visibility.Collapsed;
                this.imageCantEnter.Visibility = Visibility.Visible;
                this.enterTextBlock.Text = "You can't enter";
                this.whoseInTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void noConnectionControl()
        {
            this.myFlipview.IsEnabled = false;
            this.notPanel.Visibility = Visibility.Collapsed;
            this.massageButton.Visibility = Visibility.Collapsed;
            this.loveButton.Visibility = Visibility.Collapsed;
            this.shareButton.Visibility = Visibility.Collapsed;
            this.likeButton.Visibility = Visibility.Collapsed;
            this.girlsButton.Visibility = Visibility.Collapsed;
            this.BoysButton.Visibility = Visibility.Collapsed;
            this.girlsButton2.Visibility = Visibility.Collapsed;
            this.BoysButton2.Visibility = Visibility.Collapsed;
            this.girlsButton3.Visibility = Visibility.Collapsed;
            this.notificationButton1.Visibility = Visibility.Collapsed;
            this.BoysButton3.Visibility = Visibility.Collapsed;
            this.image.Visibility = Visibility.Collapsed;
            this.image1.Visibility = Visibility.Collapsed;
            this.controlStackP.Visibility = Visibility.Collapsed;
            this.image2.Visibility = Visibility.Collapsed;
            this.textBlock.Visibility = Visibility.Collapsed;
            this.image3.Visibility = Visibility.Collapsed;
            this.image4.Visibility = Visibility.Collapsed;
            this.PrayerPanerl.Visibility = Visibility.Collapsed;
            this.NoConnectionPanel1.Visibility = Visibility.Visible;
            this.NoConnectionPanel2.Visibility = Visibility.Visible;
            this.loading.Visibility = Visibility.Visible;
            this.noConnectionImage.Visibility = Visibility.Visible;
            this.NoConnectionTextBlock.Visibility = Visibility.Visible;
        }



        /*********************************************************************************************************/

        private void MakingToastNotification(string ToastTitle, string ToastBody, string strTag,
            string strGroup, bool IsToastPopUpRequired)
        {
            // Using the ToastText02 toast template.This template contains a maximum of two text elements. The first text element is treated as a header text and is always bold. 
            ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;

            // Retrieve the content part of the toast so we can change the text. 
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            //Find the text component of the content 
            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");

            // Set the text on the toast.  
            // The first line of text in the ToastText02 template is treated as header text, and will be bold. 
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(ToastTitle));//Toast notification title 
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(ToastBody + " (Tag:" + strTag + ", Group:" + strGroup + ")"));//Toast notification body 

            // Set the duration on the toast 
            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            ((XmlElement)toastNode).SetAttribute("duration", "long");
            ToastNotification toast = new ToastNotification(toastXml);
            toast.ExpirationTime = DateTimeOffset.UtcNow.AddSeconds(10);

            //Check Toast popup required to display 
            if (!IsToastPopUpRequired)
            {
                toast.SuppressPopup = true;//to send notification directly to action center without displaying a popup on phone. 
            }

            //Note: Tag & Group properties are optional,but these are userful for delete/update particular notification from app 
            toast.Tag = strTag;
            toast.Group = strGroup;

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        /*public async void DisplayMessage(string Message)
        {
            MessageDialog messageDialog = new MessageDialog(Message);
            await messageDialog.ShowAsync();
        }*/



        

        /*********************************************************************************************************/


        private void prayerControl()
        {

            if (str == null)
            {
                return;
            }
            string[] arr = this.str.ToString().Split(' ');
            this.fajrTimeTextBlock.Text = arr[2];
            this.zuhrTimeTextBlock.Text = arr[3];
            this.asrTimeTextBlock.Text = arr[4];
            this.maghribTimeTextBlock.Text = arr[5];
            this.ishaTimeTextBlock.Text = arr[6];
            if(arr[7]== "Fajr")
            {
                rb3.IsEnabled = false;
                this.fajrTimeTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                this.zuhrTimeTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.asrTimeTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.maghribTimeTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.ishaTimeTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);

                this.fajrTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                this.zuhrTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.asrTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.maghribTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.ishaTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
            }
            if (arr[7] == "Dhuhor")
            {
                this.fajrTimeTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.zuhrTimeTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                this.asrTimeTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.maghribTimeTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.ishaTimeTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);

                this.fajrTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.zuhrTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                this.asrTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.maghribTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.ishaTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);

            }
            if (arr[7] == "Asr")
            {
                rb3.IsEnabled = false;
                this.fajrTimeTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.zuhrTimeTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.asrTimeTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                this.maghribTimeTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.ishaTimeTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);

                this.fajrTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.zuhrTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.asrTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                this.maghribTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.ishaTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);

            }
            if (arr[7] == "Maghrib")
            {
                this.fajrTimeTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.zuhrTimeTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.asrTimeTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.maghribTimeTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                this.ishaTimeTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);

                this.fajrTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.zuhrTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.asrTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.maghribTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                this.ishaTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);

            }
            if (arr[7] == "Ishaa")
            {
                this.fajrTimeTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.zuhrTimeTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.asrTimeTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.maghribTimeTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.ishaTimeTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);

                this.fajrTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.zuhrTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.asrTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.maghribTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                this.ishaTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
            }

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var parameter = e.Parameter as String;
            this.gender = parameter;
            this.loading.Visibility = Visibility.Visible;
            DispatcherTimerSetup();
        }

        private void BoysButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void GirlsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void notificationButton1_Click(object sender, RoutedEventArgs e)
        {
            this.notificationButton1.Visibility = Visibility.Collapsed;
            this.notPanel.Visibility = Visibility.Visible;
        }

        private void notificationButton_Click(object sender, RoutedEventArgs e)
        {

            this.notificationButton1.Visibility = Visibility.Visible;
            this.notificationButton1.IsEnabled = true;
            this.notPanel.Visibility = Visibility.Collapsed;
        }

        private void off_Click(object sender, RoutedEventArgs e)
        {
            if (!isNotificated1)
            {
                this.rb1.IsEnabled = true;
            }
            if (!isNotificated2)
            {
                this.rb2.IsEnabled = true;
            }
            if (!isNotificated3 && arr[7]!="Asr" && arr[7]!= "Fajr")
            {
                this.rb3.IsEnabled = true;
            }
            this.offNotButton.Visibility = Visibility.Collapsed;
            this.onNotButton.Visibility = Visibility.Visible;
        }

        private void on_Click(object sender, RoutedEventArgs e)
        {
            this.rb1.IsEnabled = false;
            this.rb2.IsEnabled = false;
            this.rb3.IsEnabled = false;
            this.rb1.IsChecked = true;
            this.offNotButton.Visibility = Visibility.Visible;
            this.onNotButton.Visibility = Visibility.Collapsed;
        }
    }



}
