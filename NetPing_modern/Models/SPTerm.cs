using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetPing.Models
{
    /// <summary>
    /// For store the values of taxonomy
    /// </summary>
    public class SPTerm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        private string _ownNameFromPath="";
        public string OwnNameFromPath
        {
            get
            {
                return _ownNameFromPath;
            }
        }

        private string _path;
        public string Path
        {
            get {return _path;}
            set
            {
                _path = value;
                _level = _path.Where(c => c == ';').Count();

                if (Path != null)  {
                    List<string> path_names = Path.Split(';').ToList();
                    _ownNameFromPath= path_names[Level];
                }
            }
        }

        private int _level;
        public int Level
        {
            get
            {
                return _level;
            }
        }
        
        public bool IsEqualStrId(string str_id)
        {
            return Id.ToString() == str_id;
        }
    }
}