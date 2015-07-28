using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

using PhotosWPF.Model;

namespace PhotosWPF
{
    class PhotoOrganizer : IFileOrganizer
    {
        private string _source;
        private string _destination;
        private static Regex r = new Regex(":");
        private static Regex photo_extensions = new Regex(@"\.jpg|\.cr2"); //TODO: allow users to configure image types

        private Dictionary<DateTime, List<SimplePhotoFile>> photoOrganization = new Dictionary<DateTime, List<SimplePhotoFile>>();

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

        public void MoveFiles()
        {
            //foreach (var photo in files)
            //{
            //    string new_file = System.IO.Path.Combine(destination, photo.FileName);
            //    try
            //    {
            //        File.Move(photo.FullPath, new_file);
            //    }
            //    catch (IOException ioe)
            //    {
            //        Directory.CreateDirectory(System.IO.Path.Combine(destination, "Duplicates"));
            //        new_file = System.IO.Path.Combine(destination, "Duplicates", photo.FileName);
            //        try
            //        {
            //            File.Move(photo.FullPath, new_file);
            //        }
            //        catch (IOException ioe2)
            //        {
            //            Utilities.Log("Attempted to move into 'Duplicates' folder. " + ioe2.Message);
            //        }
            //    }
            //}
        }

        public void CopyFiles()
        {
            //foreach (var photo in files)
            //{
            //    string new_file = System.IO.Path.Combine(destination, photo.FileName);
            //    File.Copy(photo.FullPath, new_file);
            //}
        }

        //create the structure for the files to be copied or moved.
        public void CreateStructure()
        {
            Utilities.Log("Creating Directories in " + _destination);

            /*  get the files so the date of each file can be grabbed and the correct
             *  directory structure can be created */
            foreach (var filename in Directory.GetFiles(_source))
            {
                var fileInfo = new FileInfo(filename);
                if (photo_extensions.IsMatch(fileInfo.Extension.ToLower())) //only grab files that match the expected extension
                {
                    SimplePhotoFile photo = new SimplePhotoFile()
                    {
                        FileName = fileInfo.Name,
                        Extension = fileInfo.Extension,
                        FullPath = fileInfo.FullName,
                        CreatedDate = fileInfo.CreationTime,
                        PhotoTakenDate = GetDateTaken(fileInfo.FullName)
                    };

                    //if the dictionary already has a list for that date then add the photo to the list. otherwise create a new list and add the photo
                    if (photoOrganization.Keys.Contains(photo.PhotoTakenDate.Date))
                        photoOrganization.First(d => d.Key == photo.PhotoTakenDate.Date).Value.Add(photo);
                    else
                    {
                        var new_list = new List<SimplePhotoFile>();
                        new_list.Add(photo);
                        photoOrganization.Add(photo.PhotoTakenDate.Date, new_list);
                    }
                }
            }

            //output the structure to the log
            foreach (var list in photoOrganization)
            {
                Utilities.Log("Date: " + list.Key + " Photos: " + list.Value.Count);
            }
        }

        public DateTime GetDateTaken(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (System.Drawing.Image myImage = System.Drawing.Image.FromStream(fs, false, false))
                {
                    PropertyItem propItem = myImage.GetPropertyItem(36867);
                    string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                    return DateTime.Parse(dateTaken);
                }
            }
            catch(ArgumentException ae)
            {
                //the property that returns the date is not found so check the file name for a date string in the name
                var fileInfo = new FileInfo(path);
                Regex date_regex = new Regex(@"(19|20)\d\d(0[1-9]|1[012])(0[1-9]|[12][0-9]|3[01])");
                if (date_regex.IsMatch(fileInfo.Name))
                {
                    var date_match = date_regex.Match(fileInfo.Name);
                    int year = Int32.Parse(date_match.Value.Substring(0, 4));
                    int month = Int32.Parse(date_match.Value.Substring(4, 2));
                    int day = Int32.Parse(date_match.Value.Substring(6, 2));

                    //Utilities.Log(String.Format("File Name: {3} Date: {0}\\{1}\\{2}", day, month, year, fileInfo.Name));
                    return new DateTime(year, month, day);
                }
                else
                    return fileInfo.CreationTime.Date;
            }
        }
    }
}
