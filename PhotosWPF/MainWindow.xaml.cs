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
        static StringBuilder logger = new StringBuilder();

        private static String DEFAULT_SOURCE = @"C:\Users\Daniel\Pictures\Dump";
        private static String DEFAULT_DESTINATION = @"C:\Users\Daniel\Pictures\";
        private static String images_pattern = @"\.jpg|\.cr2";
        private static String videos_pattern = @"\.mp4|\.mts|\.mov";
        
        
        private Regex type_regex;

        private List<MyImage> photos = new List<MyImage>();
        private Dictionary<DateTime, List<MyImage>> photoDict = new Dictionary<DateTime, List<MyImage>>();

        public MainWindow()
        {
            InitializeComponent();
            Utilities.InitLogger(LogBox);

            Source.TextChanged += Source_TextChanged;
            IsVideos.Checked += IsVideos_Checked;
        }

        void IsVideos_Checked(object sender, RoutedEventArgs e)
        {
            if (IsVideos.IsChecked.Value)
                Utilities.Log("Looking for video files");
            else
                Utilities.Log("Looking for photo files");
        }

        void Source_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if the source.text is empty then disable the run button
            if (Source.Text.Length < 1)
            {
                Utilities.Log("Source is Empty!");
                GoBtn.IsEnabled = false;
            }
            else if(!GoBtn.IsEnabled) //only enable the button if is disabled
                GoBtn.IsEnabled = true;

            //TODO: Get the available file types in the source folder - I want to allow users to select file types to be organized
            // maybe the button shouldn't be enabled until we have all the types ready?
            //possibly create a 'advanced' and 'basic' mode to allow users to run from a known set of types - regardless of what is found in the folder[s]
            
        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            PhotoOrganizer po = new PhotoOrganizer();
            
            
            //get the source and destination
            po.Source = Source.Text;
            po.Destination = Destination.Text == "" ? Source.Text : Destination.Text;
            Utilities.Log("Source: " + po.Source);
            Utilities.Log("Destination: " + po.Destination);

            po.CreateDirectories();
            return;

            //if photos are being organized the 'images_pattern' needs to be used.
            //videos are a slightly different problem since there does not seem to be any standard
            //of how the date created is written.  I could possibly use the "Modifed Date" and match that against the file
            //name which typically has the data as part of the file name
            type_regex = new Regex(images_pattern);

            try
            {
                //TODO: Get files in sub folders

                //get all files from the source
                foreach (var filename in Directory.GetFiles(po.Source))
                {
                    var fileInfo = new FileInfo(filename);
                    if(type_regex.IsMatch(fileInfo.Extension.ToLower()))
                    {
                        photos.Add(new MyImage
                        {
                            FileName = fileInfo.Name,
                            CreatedDate = fileInfo.CreationTime,
                            Extension = fileInfo.Extension,
                            FullPath = fileInfo.FullName
                            //PhotoTakenDate = GetDateTakenFromImage(fileInfo.FullName)
                        });
                    }
                    //Utilities.Log(filename);
                }
            }
            catch(DirectoryNotFoundException ex)
            {
                Utilities.Log(ex.Message);
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
                Utilities.Log("Date: " + list.Key + " Photos: " + list.Value.Count);
            }

            //BuildFileSystem(dest);

        }

        private void BuildFileSystem(string destination)
        {
            string year, month, day;
            

            foreach(var list in photoDict)
            {
                StringBuilder path = new StringBuilder(destination);
                year = list.Key.Year.ToString();
                month = list.Key.Month < 10 ? "0" + list.Key.Month : list.Key.Month.ToString();
                path.Append("\\").Append(year);

                ////create a 'Duplicates' folder in each 'Year' to catch possible duplicate files
                //string duplicate_path = System.IO.Path.Combine(path.ToString(), "Duplicates");
                //if (!Directory.Exists(duplicate_path))
                //    Directory.CreateDirectory(duplicate_path);

                if(list.Value.Count >= 5) //5 or more photos from a specific date
                {
                    day = list.Key.Day < 10 ? "0" + list.Key.Day : list.Key.Day.ToString();
                    path.Append("\\").Append(month).Append(".").Append(day).Append(" (Description Needed)");
                }
                else
                {
                    path.Append("\\").Append(month).Append(" Misc");
                }

                if (!Directory.Exists(path.ToString()))
                    Directory.CreateDirectory(path.ToString());

                if(true)
                    MovePhotos(list.Value, path.ToString());
                else
                    CopyPhotos(list.Value, path.ToString());
                path = null;
            }
        }

        private void CopyPhotos(List<MyImage> photos, String dest)
        {
            foreach(var file in photos)
            {
                string new_file = System.IO.Path.Combine(dest, file.FileName);
                File.Copy(file.FullPath, new_file);
            }
        }

        private void MovePhotos(List<MyImage> photos, String dest)
        {
            foreach (var file in photos)
            {
                string new_file = System.IO.Path.Combine(dest, file.FileName);
                try
                {
                    File.Move(file.FullPath, new_file);
                }
                catch(IOException ioe)
                {
                    Directory.CreateDirectory(System.IO.Path.Combine(dest, "Duplicates"));
                    new_file = System.IO.Path.Combine(dest, "Duplicates", file.FileName);
                    try
                    {
                        File.Move(file.FullPath, new_file);
                    }
                    catch(IOException ioe2)
                    {
                        Utilities.Log("Attempted to move into 'Duplicates' folder. " + ioe2.Message);
                    }
                }
            }
        }

        private void PhotosBtn_Click(object sender, RoutedEventArgs e)
        {
            Destination.Text = DEFAULT_DESTINATION;
        }

        private void DumpBtn_Click(object sender, RoutedEventArgs e)
        {
            Source.Text = DEFAULT_SOURCE;
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
