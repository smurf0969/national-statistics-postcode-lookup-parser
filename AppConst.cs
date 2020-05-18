using System;
using System.Collections.Generic;
using System.Text;

namespace national_statistics_postcode_lookup_parser
{
    static class AppConst
    {
        public const string GeoPortalBase = "";
        public const string GeoPortalFolderStr = "";

        public const string TmpFolderName = "pcTmp";
        public static string TmpPath { get; set; }

        public const int ExpectedDocumentsCount = 2;

        public static string DataCsvPath { get; private set; }
        public static void SetDataCsvPath(string path) { DataCsvPath = path; }

        public const string ProcessedFolderName = "processed";
        public static string ProcessedPath { get; set; }

        public const int ProcessTaskCount= 8;
    }
}
