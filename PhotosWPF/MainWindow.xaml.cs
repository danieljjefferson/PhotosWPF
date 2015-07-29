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
        private static String DEFAULT_PHOTOS_SOURCE = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Staging");
        private static String DEFAULT_VIDEOS_SOURCE = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "Staging");
        
        private static String DEFAULT_PHOTOS_DESTINATION = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        private static String DEFAULT_VIDEOS_DESTINATION = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);

        public MainWindow()
        {
            InitializeComponent();
            Utilities.InitLogger(LogBox);

            DefaultDestBtn.Click += DefaultDestBtn_Click;
            DefaultSourceBtn.Click += DefaultSourceBtn_Click;

            Source.TextChanged += Source_TextChanged;
            IsVideos.Checked += IsVideos_Checked;
            IsCopy.Checked += IsCopy_Checked;
        }

        

        void IsCopy_Checked(object sender, RoutedEventArgs e)
        {
            if (IsCopy.IsChecked.Value)
            {
                Utilities.Log("Files will only be copied");
            }
        }

        void IsVideos_Checked(object sender, RoutedEventArgs e)
        {
            if (IsVideos.IsChecked.Value)
            {
                Utilities.Log("Looking for video files");
            }
                
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
            IFileOrganizer orgainizer;
            if(IsVideos.IsChecked.Value)
                orgainizer = new VideoOrganizer();
            else
                orgainizer = new PhotoOrganizer();
           
            //get the source and destination
            orgainizer.Source = Source.Text;
            orgainizer.Destination = Destination.Text == "" ? Source.Text : Destination.Text;
            orgainizer.IsCopy = IsCopy.IsChecked.Value;
            Utilities.Log("Source: " + orgainizer.Source);
            Utilities.Log("Destination: " + orgainizer.Destination);

            orgainizer.CreateStructure();
            orgainizer.OrganizeFiles();
        }

        #region Button Click Handlers
        void DefaultSourceBtn_Click(object sender, RoutedEventArgs e)
        {
            Source.Text = DEFAULT_PHOTOS_SOURCE;
        }

        private void DefaultDestBtn_Click(object sender, RoutedEventArgs e)
        {
            Destination.Text = DEFAULT_PHOTOS_DESTINATION;
        }
        #endregion
    }
}
