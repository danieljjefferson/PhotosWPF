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

namespace PhotosWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        StringBuilder logger = new StringBuilder();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //get the source and destination
            String src = Source.Text;
            String dest = Destination.Text;

            Log("Button Clicked!");
            Log("Source: " + src);
            Log("Destination: " + dest);

            //get all files from the source
            foreach(var file in Directory.GetFiles(src))
            {
                Log(file);
            }
        }

        public void Log(String message)
        {
            logger.AppendLine(message);
            LogBox.Text = logger.ToString();
        }
    }
}
