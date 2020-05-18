using System;
using System.Collections.Generic;
using System.Text;

namespace national_statistics_postcode_lookup_parser.Models
{
    public class ColMap
    {
        public ColMap(string id,params string[] nspl_cols)
        {
            ID = id;
            NSPL_COLS = nspl_cols;
        }
        public string ID { get; private set; }
        public string[] NSPL_COLS { get; private set; }
    }
}
