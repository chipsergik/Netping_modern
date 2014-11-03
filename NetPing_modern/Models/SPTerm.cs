using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetPing.Models
{
    /// <summary>
    /// For store the values of taxonomy
    /// </summary>
    [Serializable]
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

        public bool IsGroup()
        {
            return (Level != 3);
        }

        public bool IsEqualStrId(string str_id)
        {
            return Id.ToString() == str_id;
        }

        public bool IsUnderOther(SPTerm other)  // This SPTerm is equal or under other
        {
            if (other == null) return false;
            var path = Path.Split(';');
            if (path.FirstOrDefault(p => p == other.OwnNameFromPath) == null) return false;
            return true;
        }

        public bool IsUnderAnyOthers(List<SPTerm> others)  // Thia SPTerm under or equal any one from others
        {
            if (others.FirstOrDefault(o => IsUnderOther(o)) == null) return false;
            return true;
        }

        public bool IsIncludeOther(SPTerm other)   // This SPTerm exist in Path of other (it's meam this SPTerm if group that include other)
        {
            if (other == null) return false;
            var path = other.Path.Split(';');
            if (path.FirstOrDefault(p => p == OwnNameFromPath) == null) return false;
            return true;
        }

        public bool IsIncludeAnyFromOthers(List<SPTerm> others)   // This SPTerm exist in Path of any from others (it's meam this SPTerm if group that include any one from others)
        {
            if (others.FirstOrDefault(o => IsIncludeOther(o)) == null) return false;
            return true;
        }

    }
}