using System;

namespace NetPing_modern.ViewModels
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Cathegory { get; set; }
        public DateTime Created { get; set; }
        public string Url { get; set; }
        public bool IsTop { get; set; }
    }
}