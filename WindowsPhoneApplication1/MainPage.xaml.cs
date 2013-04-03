using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Devices.Sensors;
using System.Windows.Threading;

namespace WindowsPhoneApplication1
{
    public partial class MainPage : PhoneApplicationPage
    {
        DispatcherTimer timer = new DispatcherTimer();
        DispatcherTimer timertemp = new DispatcherTimer();

        Accelerometer accelerometer;
        int current = 0,temp=-1,lastvalue=-1;
          int i=0;
         IsolatedStorageFile fileSystem = IsolatedStorageFile.GetUserStoreForApplication();


        // Constructor
        public MainPage()
        {
            InitializeComponent();
            cancelthis();

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            timer.Interval = TimeSpan.FromSeconds(0.8);

            timer.Tick += new EventHandler(timer_Tick);
            timertemp.Interval = TimeSpan.FromSeconds(5);

            timertemp.Tick += new EventHandler(timertemp_Tick);

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            current_fileno.Text = "timer value: " + current.ToString();
            current++;
            if (current > 60)
            {
                timer.Stop();
                current_fileno.Text = "SOrry!You have crossed 1 minute!";
                
            }
            write_ToFile();
        }

        private void write_ToFile()
        {

            if (!fileSystem.DirectoryExists("UserData"))
                fileSystem.CreateDirectory("UserData");

            if (fileSystem.FileExists("UserData\\user["+i+"].dat"))
                fileSystem.DeleteFile("UserData\\user["+i+"].dat");
            StreamWriter writer = new StreamWriter(new IsolatedStorageFileStream("UserData\\user["+i+"].dat", FileMode.CreateNew, fileSystem));
            writer.WriteLine(current_x.Text);
            writer.WriteLine(current_y.Text);
            writer.WriteLine(current_z.Text);
            writer.Close();
            i++;

        }
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            start.Click += new RoutedEventHandler(start_Click);
        }
        private void LoadDataFromIsolatedStorage()
        {

            if (fileSystem.FileExists("UserData\\storedlastvalue.dat"))
            {
                StreamReader readertemp = new StreamReader(new IsolatedStorageFileStream("UserData\\storedlastvalue.dat", FileMode.Open, fileSystem));
                temp = Convert.ToInt32(readertemp.ReadLine());
                stored_fileno.Text = "wait!there are " + temp.ToString() + " values";
                timertemp.Tick += new EventHandler(timertemp_Tick);
                timertemp.Start();
                readertemp.Close();

                if (fileSystem.FileExists("UserData\\lastvalue.dat"))
                {
                    StreamReader readcurrent = new StreamReader(new IsolatedStorageFileStream("UserData\\lastvalue.dat", FileMode.Open, fileSystem));
                    lastvalue = Convert.ToInt32(readcurrent.ReadLine());
                    readcurrent.Close();
                    current_fileno.Text = "there are " + lastvalue.ToString() + " values";
                }
              
            }
            else
                stored_fileno.Text = "Currently no stored values!";
        }

        private void timertemp_Tick(object sender, EventArgs e)
        {

            if (temp > -1)
            {
                stored_fileno.Text = "stored file: " + temp.ToString();

                StreamReader reader = new StreamReader(new IsolatedStorageFileStream("UserData\\store[" + temp + "].dat", FileMode.Open, fileSystem));
                
                //XmlSerializer serializer = new XmlSerializer(typeof(DataForm));
                //DataForm data = serializer.Deserialize(reader) as DataForm;

                stored_x.Text = reader.ReadLine();
                stored_y.Text = reader.ReadLine();
                stored_z.Text = reader.ReadLine();
                temp--;

                reader.Close();
         }
            if (lastvalue > -1)
            
            {
                current_fileno.Text = "file: " + lastvalue.ToString();

                StreamReader reader1 = new StreamReader(new IsolatedStorageFileStream("UserData\\user[" + lastvalue + "].dat", FileMode.Open, fileSystem));

                //XmlSerializer serializer = new XmlSerializer(typeof(DataForm));
                //DataForm data = serializer.Deserialize(reader) as DataForm;

                current_x.Text = reader1.ReadLine();
                current_y.Text = reader1.ReadLine();
                current_z.Text = reader1.ReadLine();
                lastvalue--;

                reader1.Close();

            }

            if(temp<0 )
                stored_fileno.Text = "Over";
            if(lastvalue< 0)
                current_fileno.Text="Over";
            if(temp<0 && lastvalue<0)
                timertemp.Stop();
            }
        


        private void start_Click(object sender, RoutedEventArgs e)
        {
            current=i = 0;
            accelerometer = new Accelerometer();
            accelerometer.ReadingChanged += new EventHandler<AccelerometerReadingEventArgs>(accelerometer_ReadingChanged);
            accelerometer.Start();
            timer.Start();

        }

        void accelerometer_ReadingChanged(object sender, AccelerometerReadingEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => MyReadingChanged(e));
        }


        void MyReadingChanged(AccelerometerReadingEventArgs e)
        {
            current_x.Text = "x: " + e.X.ToString("0.00000");// +Environment.NewLine;
            current_y.Text = "y: " + e.Y.ToString("0.00000");// +Environment.NewLine;
            current_z.Text="z: " + e.Z.ToString("0.00000");


            //DataForm data = new DataForm { Name = Name.Text, Email = Email.Text, Phone = Phone.Text };
           //XmlSerializer serializer = new XmlSerializer(typeof(DataForm));
          
            //    serializer.Serialize(writer, data);
                
            }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            timertemp.Stop();
            if (fileSystem.FileExists("UserData\\lastvalue.dat"))
                fileSystem.DeleteFile("UserData\\lastvalue.dat");
            i--;
            StreamWriter writer = new StreamWriter(new IsolatedStorageFileStream("UserData\\lastvalue.dat", FileMode.CreateNew, fileSystem));
            writer.WriteLine(i);
            writer.Close();


        }

      

        private void save_Click(object sender, RoutedEventArgs e)
        {
            int j;
            if (fileSystem.FileExists("UserData\\lastvalue.dat"))
            {

                StreamReader reader = new StreamReader(new IsolatedStorageFileStream("UserData\\lastvalue.dat", FileMode.Open, fileSystem));
                j = Convert.ToInt32(reader.ReadLine());
                if (fileSystem.FileExists("UserData\\storedlastvalue.dat"))
                    fileSystem.DeleteFile("UserData\\storedlastvalue.dat");
                reader.Close();
                fileSystem.MoveFile("Userdata\\lastvalue.dat", "Userdata\\storedlastvalue.dat");

                while (j > -1)
                {
                    if (fileSystem.FileExists("UserData\\store["+j+"].dat"))
                        fileSystem.DeleteFile("UserData\\store[" +j+"].dat");
                    fileSystem.MoveFile("Userdata\\user[" + j + "].dat", "Userdata\\store[" + j + "].dat");
                    j--;
                }

            }

            else
                current_fileno.Text = "No values to store";
          

        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            cancelthis();
        }

private void cancelthis()
{
 

            int j;
            if (fileSystem.FileExists("UserData\\lastvalue.dat"))
            {
                StreamReader reader1 = new StreamReader(new IsolatedStorageFileStream("UserData\\lastvalue.dat", FileMode.Open, fileSystem));
                j = Convert.ToInt32(reader1.ReadLine());
                reader1.Close();
                fileSystem.DeleteFile("UserData\\lastvalue.dat");
                while (j > -1)
                {
                    if (fileSystem.FileExists("UserData\\user[" + j + "].dat")) ;
                    fileSystem.DeleteFile("UserData\\user[" + j + "].dat");
                    j--;
                }
            }
                temp = lastvalue = -1;
                }

        private void readfiles_Click_(object sender, RoutedEventArgs e)
        {
            LoadDataFromIsolatedStorage();
        }

       

       

       
        }
        }
   

