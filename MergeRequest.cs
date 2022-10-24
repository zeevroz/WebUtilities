using System;
using System.Collections.Generic;

namespace WebUtilities
{
    [Serializable]
    public class InsertParams
    {
        public string file { get; set; }
        public string after { get; set; }

        public InsertParams()
        {
        }

        public InsertParams(string file, string after)
        {
            this.file = file;
            this.after = after;
        }
    }

    [Serializable]
    public class MergeRequest
    {
        public string src { get; set; }
        public List<InsertParams> insert { get; set; }
        public string dst { get; set; }
        public string status { get; set; }

        public MergeRequest()
        {
        }

        public MergeRequest(string src, string dst)
        {
            this.src = src;
            this.insert = new List<InsertParams>();
            this.dst = dst;
        }
    }
}
