using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetPing.Models
{
    // Store information about File
    [Serializable]
    public class SFile
    {
        public int Id { get; set; }
        public List<SPTerm> Devices { get; set; }
        public SPTerm File_type { get; set; }
        public List<SPTerm> Languages { get; set; }
        public DateTime Created { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }  // Public url of file

    }
}