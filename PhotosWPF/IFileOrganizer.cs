using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotosWPF
{
    interface IFileOrganizer
    {
        String Source;
        String Destination;

        void MoveFiles(List<MyImage> files, String destination);
        void CopyFiles(List<MyImage> files, String destination);
        void CreateDirectories(String destination);
        void GetDateTaken(String pathToFile);
    }
}
