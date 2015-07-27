using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PhotosWPF
{
    class PhotoOrganizer : IFileOrganizer
    {
        private string _source;
        private string _destination;
        private static Regex r = new Regex(":");

        private Dictionary<DateTime, List<MyImage>> photoOrganization = new Dictionary<DateTime, List<MyImage>>();

        #region Properties
        public string Source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
            }
        }

        public string Destination
        {
            get
            {
                return _destination;
            }
            set
            {
                _destination = value;
            }
        }
        #endregion
        
        public void MoveFiles(List<MyImage> files, string destination)
        {
            foreach (var photo in files)
            {
                string new_file = System.IO.Path.Combine(destination, photo.FileName);
                try
                {
                    File.Move(photo.FullPath, new_file);
                }
                catch (IOException ioe)
                {
                    Directory.CreateDirectory(System.IO.Path.Combine(destination, "Duplicates"));
                    new_file = System.IO.Path.Combine(destination, "Duplicates", photo.FileName);
                    try
                    {
                        File.Move(photo.FullPath, new_file);
                    }
                    catch (IOException ioe2)
                    {
                        Utilities.Log("Attempted to move into 'Duplicates' folder. " + ioe2.Message);
                    }
                }
            }
        }

        public void CopyFiles(List<MyImage> files, string destination)
        {
            foreach (var photo in files)
            {
                string new_file = System.IO.Path.Combine(destination, photo.FileName);
                File.Copy(photo.FullPath, new_file);
            }
        }

        public void CreateDirectories()
        {
            Utilities.Log("Creating Directories...");
        }

        public DateTime GetDateTaken(string path)
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
}
