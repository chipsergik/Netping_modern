using System.Collections.Generic;

namespace NetPing_modern.Models
{
    public class SectionModel
    {
        public string Title { get; set; }

        public string FormattedTitle
        {
            get
            {
                return Title.Replace("<br/>", " ");
            }
        }

        public string Url { get; set; }

        public string ImageUrl { get; set; }

        public string Description { get; set; }

        public List<SectionModel> Sections { get; set; }

        public bool IsSelected { get; set; }
    }
}