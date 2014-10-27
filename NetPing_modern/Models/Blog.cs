using System.Collections.Generic;
using NetPing.Models;

namespace NetPing_modern.Models
{
    public class Blog
    {
        public IEnumerable<Post> Posts { get; set; } 
    }
}