using System;
using System.Collections.Generic;

namespace NetPing.Models
{
    /// <summary>
    /// Stores information about the post and related devices
    /// </summary>
    [Serializable]
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body {  get;set; }
        public List<SPTerm> Devices { get; set; }
        public string Cathegory { get; set; }
        public DateTime Created { get; set; }
        public string Url_name { get; set; }
        public bool IsTop { get; set; }

        public string Url
        {
            get
            {
                if (Id != 0)
                {
                    return string.Format("/view.aspx?id={0}", Id);
                }
                return "#";
            }
        }
    }
}