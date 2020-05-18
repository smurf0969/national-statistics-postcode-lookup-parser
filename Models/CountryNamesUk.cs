using System;
using System.Collections.Generic;
using System.Text;

namespace national_statistics_postcode_lookup_parser.Models
{
    /// <summary>
    /// Country names in English and Welsh
    /// </summary>
    public class CountryNamesUk
    {
        public static readonly string ColumnNames = "CTRY12CD,CTRY12CDO,CTRY12NM,CTRY12NMW";
        private static int _expectedColumns = 4;
        /// <summary>
        /// Check if Columns have changed
        /// </summary>
        /// <param name="line">First line from csv including the column names</param>
        /// <returns></returns>
        public static bool CsvCheckOk(string line)
        {
            return line.Trim().Equals(ColumnNames);
        }
        public static CountryNamesUk FromLine(string line)
        {
            var parts = line.Split(',');
            if (parts.Length != _expectedColumns) throw new ArgumentException($"Expected {_expectedColumns} values, parsed: " + parts.Length);
            return new CountryNamesUk(parts[0],uint.Parse( parts[1]), parts[2], parts[3]);
        }
        public CountryNamesUk() { }
        /// <summary>
        /// UK Country Names
        /// </summary>
        /// <param name="idx0">String ID, usually 9 charaters long</param>
        /// <param name="idx1">Numeric representation of ID</param>
        /// <param name="name">Country name in English</param>
        /// <param name="namewelsh">Country name is Welsh or null</param>
        public CountryNamesUk(string idx0,uint idx1 ,string name,string namewelsh)
        {
            CTRY12CD = idx0;
            CTRY12CDO = idx1;
            CTRY12NM = name;
            CTRY12NMW = namewelsh;
        }
        /// <summary>
        /// String ID, usually 9 charaters long
        /// </summary>
        public string CTRY12CD { get; set; }
        /// <summary>
        /// Numeric representation of ID
        /// </summary>
        public uint CTRY12CDO { get; set; }
        /// <summary>
        /// Country name in English
        /// </summary>
        public string CTRY12NM { get; set; }
        /// <summary>
        /// Country name is Welsh or null
        /// </summary>
        public string CTRY12NMW { get; set; }
    }
}
