using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PhotosWPF
{
    interface IFileOrganizer
    {
        String Source {get; set; }
        String Destination { get; set; }

        void MoveFiles(List<MyImage> files, String destination);
        void CopyFiles(List<MyImage> files, String destination);
        void CreateDirectories();
        DateTime GetDateTaken(String pathToFile);
    }
}
