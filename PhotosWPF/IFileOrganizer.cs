using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using PhotosWPF.Model;

namespace PhotosWPF
{
    interface IFileOrganizer
    {
        //Source Directory of the files to be organized
        String Source {get; set; }

        //Destination Directory of the files to be organized. If this is empty/null then the Destincatin will be the Source.
        String Destination { get; set; }

        //If the files should be copied instead of moved this will be true
        Boolean IsCopy { get; set; }

        //number of files that should be grouped together
        int FileCount { get; set; }

        /// <summary>
        /// Organize the files to the Destination.  If the Destination does not exist, create it.
        /// </summary>
        void OrganizeFiles();

        /// <summary>
        /// Gets the files (of the correct type) and adds them to a dictionary where the Key is the created date of the file
        /// </summary>
        void CreateStructure();

        /// <summary>
        /// Get the actual date of creation for the file. For photos and videos this can be the EXIF data or parsing the file name
        /// </summary>
        /// <param name="pathToFile">Full path to the file that needs an actual creation date</param>
        /// <returns>DateTime of the actual creation date</returns>
        DateTime GetDateTaken(String pathToFile);
    }
}
