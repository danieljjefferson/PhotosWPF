using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotosWPF.Model
{
    class SimpleMediaFile
    {
        public String FileName { get; set; }
        public DateTime CreatedDate { get; set; }
        public String Extension { get; set; }
        public String FullPath { get; set; }
    }
}
