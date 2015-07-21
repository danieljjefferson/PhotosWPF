using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace PhotosWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        StringBuilder logger = new StringBuilder();

        private static String DEFAULT_SOURCE = @"C:\Users\Daniel\Pictures\Dump";
        private static String DEFAULT_DESTINATION = @"C:\Users\Daniel\Pictures\";
        private static String[] DEFAULT_FILE_TYPES = { ".jpg", ".cr2" };
        private static Regex r = new Regex(":");

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            //get the source and destination
            String src = Source.Text;
            String dest = Destination.Text;

            Log("Button Clicked!");
            Log("Source: " + src);
            Log("Destination: " + dest);

            List<MyImage> photos = new List<MyImage>();
            Dictionary<DateTime, List<MyImage>> photoDict = new Dictionary<DateTime, List<MyImage>>();

            try
            {
                //get all files from the source
                foreach (var filename in Directory.GetFiles(src))
                {
                    var fileInfo = new FileInfo(filename);
                    if(fileInfo.Extension.ToLower().Contains(".jpg") || fileInfo.Extension.ToLower().Contains(".cr2"))
                    {
                        photos.Add(new MyImage
                        {
                            FileName = fileInfo.Name,
                            CreatedDate = fileInfo.CreationTime,
                            Extension = fileInfo.Extension,
                            FullPath = fileInfo.FullName,
                            PhotoTakenDate = GetDateTakenFromImage(fileInfo.FullName)
                        });
                    }
                    //Log(filename);
                }
            }
            catch(DirectoryNotFoundException ex)
            {
                Log(ex.Message);
            }

            foreach(var photo in photos)
            {
                if(photoDict.Keys.Contains(photo.PhotoTakenDate.Date))
                {
                    var dated_Photos = photoDict.First(k => k.Key == photo.PhotoTakenDate.Date).Value;
                    dated_Photos.Add(photo);
                }
                else
                {
                    List<MyImage> new_photos = new List<MyImage>();
                    new_photos.Add(photo);
                    photoDict.Add(photo.PhotoTakenDate.Date, new_photos);
                }
            }

            foreach(var list in photoDict)
            {
                Log("Date: " + list.Key + " Photos: " + list.Value.Count);
            }

        }

        public void Log(String message)
        {
            logger.AppendLine(message);
            LogBox.Text = logger.ToString();
        }

        private void PhotosBtn_Click(object sender, RoutedEventArgs e)
        {
            Destination.Text = DEFAULT_DESTINATION;
        }

        private void DumpBtn_Click(object sender, RoutedEventArgs e)
        {
            Source.Text = DEFAULT_SOURCE;
        }

        //retrieves the datetime WITHOUT loading the whole image
        private static DateTime GetDateTakenFromImage(string path)
        {

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (System.Drawing.Image myImage = System.Drawing.Image.FromStream(fs, false, false))
            {
                PropertyItem propItem = myImage.GetPropertyItem(36867);
                string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                return DateTime.Parse(dateTaken);
            }
        }
    }

    public class MyImage
    {
        public String FileName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime PhotoTakenDate { get; set; }
        public String Extension { get; set; }
        public String FullPath { get; set; }
    }
}
