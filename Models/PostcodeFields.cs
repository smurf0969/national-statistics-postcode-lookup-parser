using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace national_statistics_postcode_lookup_parser.Models
{
    public class PostcodeFields
    {
        [JsonIgnore]
        public bool HasPCD { get { return !string.IsNullOrEmpty(PCD); } }
        #region IDS
        /// <summary>
        /// Unit postcode – 7 character version
        /// </summary>
        public string PCD { get; set; }
        /// <summary>
        /// Unit postcode – 8 character version
        /// </summary>
        public string PCD2 { get; set; }
        /// <summary>
        /// Unit postcode - variable length (e-Gif) version
        /// </summary>
        public string PCDS { get; set; }
        #endregion

        #region dates
        /// <summary>
        /// Date of introduction
        /// YYYYMM
        /// </summary>
        public string DOINTR { get; set; }
        /// <summary>
        /// Date of termination
        /// YYYYMM or null if still live
        /// </summary>
        public string DOTERM { get; set; }
        #endregion

        /// <summary>
        /// Postcode user type
        /// 0 = small user;
        /// 1 = large user
        /// </summary>
        public uint USERTYPE { get; set; }

        #region Ordinance_Survey
        /// <summary>
        /// National grid reference - Easting
        /// numeric or null
        /// </summary>
        public string OSEAST1M { get; set; }
        /// <summary>
        /// National grid reference - Northing
        /// numeric or null
        /// </summary>
        public string OSNRTH1M { get; set; }
        /// <summary>
        /// Grid reference positional quality indicator
        /// Shows the status of the assigned grid reference.
        /// 1 = within the building of the matched address closest to the postcode mean;
        /// 2 = as for status value 1, except by visual inspection of Landline maps(Scotland only);
        /// 3 = approximate to within 50 metres;
        /// 4 = postcode unit mean(mean of matched addresses with the same postcode, but not snapped to a building);
        /// 5 = imputed by ONS, by reference to surrounding postcode grid references;
        /// 6 = postcode sector mean, (mainly PO Boxes);
        /// 8 = postcode terminated prior to Gridlink® initiative, last known ONS postcode grid reference2;
        /// 9 = no grid reference available
        /// </summary>
        public int OSGRDIND { get; set; }
        #endregion

        /// <summary>
        /// County
        /// </summary>
        public string CTY { get; set; }
        /// <summary>
        /// Country
        /// </summary>
        public string CTRY { get; set; }

        #region Coordinates
        /// <summary>
        /// Decimal degrees latitude
        /// </summary>
        public string LAT { get; set; }
        /// <summary>
        /// Decimal degrees longitude
        /// </summary>
        public string LONG { get; set; }
        #endregion
    }
}
