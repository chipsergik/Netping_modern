﻿using System;
using System.Collections.Generic;

namespace NetPing_modern.ViewModels
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortBody { get; set; }
        public string Body { get; set; }
        public TermViewModel Category { get; set; }
        public DateTime Created { get; set; }
        public string Url { get; set; }
        public bool IsTop { get; set; }
        public List<TagViewModel> Tags { get; set; }
    }
}