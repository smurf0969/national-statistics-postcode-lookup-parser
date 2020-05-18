using System;
using System.Collections.Generic;
using System.Text;

namespace national_statistics_postcode_lookup_parser.Models
{
    /// <summary>
    /// County names for the UK.
    /// Ids of [A-Z]99999999 are used encase of unknown
    /// Raw csv has unmapped columns that will be dropped
    /// </summary>
    public class CountyNames
    {
        private static readonly string expectedFirstLine = "CTY10CD,CTY10NM,,";
        /// <summary>
        /// Partial check of first line as raw csv has unmapped columns.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool CsvCheckOK(string line)
        {
            return line.StartsWith(expectedFirstLine.TrimEnd(','));
        }
        public static CountyNames FromLine(string line)
        {
            return new CountyNames(line);
        }
        public CountyNames() { }
        /// <summary>
        /// County
        /// </summary>
        /// <param name="line">Csv Line</param>
        /// <param name="pseudoTrim">Remove (pseudo) from unknown names</param>
        public CountyNames(string line, bool pseudoTrim = true)
        {
            var parts = line.Split(',');
            if (parts.Length < 2) throw new ArgumentException("Expected at least two values");
            fill(parts[0], parts[1], pseudoTrim);
        }
        /// <summary>
        /// County
        /// </summary>
        /// <param name="idx0">Index</param>
        /// <param name="name">County Name</param>
        /// <param name="pseudoTrim">Remove (pseudo) from unknown names</param>
        public CountyNames(string idx0,string name,bool pseudoTrim=true)
        {
            fill(idx0, name, pseudoTrim);
        }
        private void fill(string idx0, string name, bool pseudoTrim)
        {
            CTY10CD = idx0;
            if (pseudoTrim && name[0] == '(')
            {
                int fcb = name.IndexOf(')');
                CTY10NM = name.Substring(fcb).TrimStart();
            }
            else
            {
                CTY10NM = name;
            }
        }
        /// <summary>
        /// Index for the County, usually 9 characters
        /// [A-Z]99999999 is used for unknown
        /// </summary>
        public string CTY10CD { get; set; }
        /// <summary>
        /// County name.
        /// Unknown names normaly start with (pseudo), can be optionally(default:true) changed if using constructor
        /// </summary>
        public string CTY10NM { get; set; }
    }
}
