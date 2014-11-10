using System.Collections.Generic;

namespace NetPing_modern.ViewModels
{
    public class BlogViewModel
    {
        public IEnumerable<PostViewModel> Posts { get; set; }

        public PostViewModel Post { get; set; }

        public IList<TermViewModel> Categories { get; set; }

        public IList<TagViewModel> Tags { get; set; } 

        public string Query { get; set; }

    }
}