using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotosWPF
{
    class PhotoOrganizer : IFileOrganizer
    {
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
                        //Log("Attempted to move into 'Duplicates' folder. " + ioe2.Message);
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

        public void CreateDirectories(string destination)
        {
            throw new NotImplementedException();
        }

        public void GetDateTaken(string pathToFile)
        {
            throw new NotImplementedException();
        }
    }
}
