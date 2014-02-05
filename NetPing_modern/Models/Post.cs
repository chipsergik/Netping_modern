using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetPing.Models
{
    /// <summary>
    /// Stores information about the post and related devices
    /// </summary>
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body {  get;set; }
        public List<SPTerm> Devices { get; set; }
        public string Cathegory { get; set; }
        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
    }
}