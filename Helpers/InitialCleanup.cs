using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace national_statistics_postcode_lookup_parser.Helpers
{
    static class InitialCleanup
    {
        private const string _csvDir = "multi_csv";
        private static readonly string _countyRegex = @"\\Count.*UK.*\.csv$";

        public static List<string> RemoveDirs { get; private set; }
        public static List<string> RemoveFiles { get; private set; }
        public static List<string> DocsToKeep { get; set; } = new List<string>();

        public static bool BuildLists(string baseDir)
        {
            if (RemoveDirs == null) RemoveDirs = new List<string>();
            if (RemoveFiles == null) RemoveFiles = new List<string>();
            RemoveDirs.Clear();
            RemoveFiles.Clear();
            DocsToKeep.Clear();

            string[] dirs = Directory.EnumerateDirectories(baseDir,"*", SearchOption.AllDirectories).ToArray();
            bool hasMultiCsv = dirs.Any(x => x.Contains(_csvDir));
            if (!hasMultiCsv) return false;
            AppConst.SetDataCsvPath(dirs.Where(x => x.TrimEnd(Path.DirectorySeparatorChar).EndsWith(_csvDir)).First());
            for(int x = 0; x < dirs.Length; x++)
            {
                if (!AppConst.DataCsvPath.Contains(dirs[x])&&!dirs[x].Equals(AppConst.TmpPath)) { 
                    RemoveDirs.Add(dirs[x]);
                }
                else
                {
                    if (!dirs[x].Equals(AppConst.DataCsvPath)&&!dirs[x].Equals(AppConst.TmpPath))
                    {
                        RemoveFiles.AddRange(Directory.EnumerateFiles(dirs[x]));
                    }
                }
            }

            string docsDir = dirs.Where(x => x.Contains("Documents")).FirstOrDefault();
            if (!string.IsNullOrEmpty(docsDir))
            {
                var docfiles = Directory.EnumerateFiles(docsDir, "*.csv").ToList();
                var cRegex = new Regex(_countyRegex, RegexOptions.IgnoreCase);
                for (int x = 0; x < docfiles.Count; x++)
                {
                    if (cRegex.IsMatch(docfiles[x])) DocsToKeep.Add(docfiles[x]);
                }
            }
            if(DocsToKeep.Count != AppConst.ExpectedDocumentsCount)
            {
                if (DocsToKeep.Count == 0)
                {
                    //scan tmp for csv files
                    if (Directory.Exists(AppConst.TmpPath)) DocsToKeep = Directory.EnumerateFiles(AppConst.TmpPath, "*.csv").ToList();
                }
                if (DocsToKeep.Count != AppConst.ExpectedDocumentsCount)  Console.Write("DOCUMENT COUNT INCORRECT - ");
            }
            return true;
        }
        /// <summary>
        /// This will remove all directories in the list.
        /// NO checks are done regarding documents to keep, so make sure they are copied somewhere safe.
        /// </summary>
        public  static bool Clean()
        {
            bool noErrors = true;
            if (RemoveDirs != null && RemoveDirs.Count > 0)
            {
                Queue<string> dirs = new Queue<string>();
                RemoveDirs.ForEach((x) => { dirs.Enqueue(x); });
                RemoveDirs.Clear();
                while (dirs.Count > 0)
                {
                    try
                    {
                        var qdir = dirs.Dequeue();
                        if (!Directory.Exists(qdir)) continue;// as we delete recurseivly, check to avoid not found errors
                        Directory.Delete(qdir, true);
                    }
                    catch (Exception)
                    {
                        noErrors = false;
                    }
                }
            }
            if (RemoveFiles != null && RemoveFiles.Count > 0)
            {
                Queue<string> files = new Queue<string>();
                RemoveFiles.ForEach((x) => { files.Enqueue(x); });
                RemoveFiles.Clear();
                while (files.Count > 0)
                {
                    try
                    {
                        var qfile = files.Dequeue();
                        if (!File.Exists(qfile)) continue;// as we delete recurseivly, check to avoid not found errors
                        File.Delete(qfile);
                    }
                    catch (Exception)
                    {
                        noErrors = false;
                    }
                }
            }
            return noErrors;
        }
    }
}
