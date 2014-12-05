using System.Collections.Generic;

namespace NetPing_modern.ViewModels
{
    public class BlogViewModel
    {
        public IEnumerable<PostViewModel> Posts { get; set; }

        public PostViewModel Post { get; set; }

        public IEnumerable<PostViewModel> TopPosts { get; set; }

        public IList<CategoryViewModel> Categories { get; set; }

        public IList<TagViewModel> Tags { get; set; } 

        public string Query { get; set; }

    }
}