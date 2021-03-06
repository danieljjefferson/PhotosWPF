﻿using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using PhotosWPF.Model;

namespace PhotosWPF
{
    class VideoOrganizer : IFileOrganizer
    {
        #region Private Variables
        private string _source;
        private string _destination;
        private bool _iscopy;
        private int _filecount = 4;
        private static Regex r = new Regex(":");
        private static Regex video_extensions = new Regex(@"\.mp4|\.mts|\.mov"); //TODO: allow users to configure image types

        private Dictionary<DateTime, List<SimplePhotoFile>> photoOrganization = new Dictionary<DateTime, List<SimplePhotoFile>>();
        #endregion

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

        public bool IsCopy
        {
            get
            {
                return _iscopy;
            }
            set
            {
                _iscopy = value;
            }
        }

        public int FileCount
        {
            get { return _filecount; }
            set { _filecount = value; }
        }

        public void OrganizeFiles()
        {
            Utilities.Log("Organizing files");
            foreach (var list in photoOrganization)
                foreach (var photo in list.Value)
                {
                    string path;
                    string year = list.Key.Year.ToString();
                    string month = month = list.Key.Month < 10 ? "0" + list.Key.Month : list.Key.Month.ToString();
                    string day = day = list.Key.Day < 10 ? "0" + list.Key.Day : list.Key.Day.ToString();

                    if (list.Value.Count >= _filecount)
                        path = System.IO.Path.Combine(Destination, year, String.Format("{0}.{1} (Description)", month, day));
                    else
                        path = System.IO.Path.Combine(Destination, year, String.Format("{0} Misc", month));

                    if (!Directory.Exists(path.ToString()))
                        Directory.CreateDirectory(path.ToString());

                    string new_file = System.IO.Path.Combine(path, photo.FileName);

                    try
                    {
                        if (_iscopy)
                            File.Copy(photo.FullPath, new_file);
                        else
                            File.Move(photo.FullPath, new_file);
                    }
                    catch(IOException ioe)
                    {
                        Directory.CreateDirectory(System.IO.Path.Combine(_destination, "Duplicates"));
                        new_file = System.IO.Path.Combine(_destination, "Duplicates", photo.FileName);
                        try
                        {
                            if (_iscopy)
                                File.Copy(photo.FullPath, new_file);
                            else
                                File.Move(photo.FullPath, new_file);
                        }
                        catch (IOException ioe2)
                        {
                            Utilities.Log("Attempted to move into 'Duplicates' folder. " + ioe2.Message);
                        }
                    }
                    
                }

            photoOrganization.Clear();
            Utilities.Log("Organization Done!");
        }

        public void CreateStructure()
        {
            Utilities.Log("Creating Directories in " + _destination);

            /*  get the files so the date of each file can be grabbed and the correct
             *  directory structure can be created */
            foreach (var filename in Directory.GetFiles(_source))
            {
                var fileInfo = new FileInfo(filename);
                if (video_extensions.IsMatch(fileInfo.Extension.ToLower())) //only grab files that match the expected extension
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

        /// <summary>
        /// Returns the creation date of the Photo.  The creation date is pulled from either the EXIF data, file name or created date
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>The file's created date as a DateTime</returns>
        public DateTime GetDateTaken(string path)
        {
            //Utilities.Log("EXIF data not found: " + ae.Message);
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
            //returns the earlier creation date or modifed date
            else
                return DateTime.Compare(fileInfo.CreationTime.Date, fileInfo.LastWriteTime.Date) > 0 ? fileInfo.LastWriteTime.Date : fileInfo.CreationTime.Date;

            ////try getting the EXIF property otherwise 
            //try
            //{
            //    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            //    using (System.Drawing.Image myImage = System.Drawing.Image.FromStream(fs, false, false))
            //    {
            //        PropertyItem propItem = myImage.GetPropertyItem(36867);
            //        string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
            //        return DateTime.Parse(dateTaken);
            //    }
            //}
            //catch (ArgumentException ae) //if the EXIF property is unavailable then use the filename or created date as a last resort
            //{

            //}
        }
    }
}
