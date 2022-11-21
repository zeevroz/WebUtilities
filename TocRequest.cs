using System.Collections.Generic;

namespace WebUtilities
{
    public class TocRequest
    {
        public string File { get; set; }
        public List<Chapter> Chapters { get; set; }
    }

    public class Chapter
    {
        public string Title { get; set; }
        
        public int PageNum { get; set; }
    }
}
