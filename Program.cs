using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace national_statistics_postcode_lookup_parser
{
    class Program
    {
        static ConcurrentQueue<string> PcFiles = new ConcurrentQueue<string>();
        static void Main(string[] args)
        {
            // ###################################################################
            // TODO: Load from file or via args
            // ###################################################################
            // EDIT THESE VARIABLES
            // ###################################################################
                var unzippedDir = @"F:\Stuart\Dowloads\NSPL_MAY_2020_UK";
                string ZipPassword = null;
                int ZipCompressionLevel = 7;//0-9, 9 being the best compression
                bool PressAnyKeyToContinue = true;
                bool keepProcessedFiles = true;
            // ###################################################################

            Console.WriteLine("National Statistics Postcode - Parser / Remapper");
            Console.WriteLine();
            

            AppConst.TmpPath = Path.Combine(unzippedDir, AppConst.TmpFolderName);
            bool removeTmpOnStart = false;
            AppConst.ProcessedPath = Path.Combine(unzippedDir, AppConst.ProcessedFolderName);
            var msw = new Stopwatch();
            msw.Start();
            bool isOK= Directory.Exists(unzippedDir);
            Console.WriteLine("Postcode data folder: " + boolStatus(isOK));
            if (isOK)
            {
                //remove old tmp dir if exists due to failures
                if (removeTmpOnStart && Directory.Exists(AppConst.TmpPath))
                {
                    Directory.Delete(AppConst.TmpPath, true);
                }
                Console.Write("Building initial lists: ");
                Console.WriteLine(boolStatus(isOK = Helpers.InitialCleanup.BuildLists(unzippedDir)));
                if (isOK)
                {
                    if (!(isOK = Directory.Exists(AppConst.TmpPath)))
                    {
                        try { Directory.CreateDirectory(AppConst.TmpPath); isOK = true; } catch (Exception) { }
                    }
                    Console.WriteLine("Temp Folder: " + boolStatus(isOK));
                    if (isOK)
                    {
                        Console.Write("Copying files to keep: ");
                        Helpers.InitialCleanup.DocsToKeep.ForEach((x) =>
                        {
                            var fi = new FileInfo(x);
                            if (!fi.Directory.FullName.Equals(AppConst.TmpPath))
                            {
                                try { fi.CopyTo(Path.Combine(AppConst.TmpPath, fi.Name), true); } catch (Exception) { isOK = false; }
                            }
                        });
                        Helpers.InitialCleanup.DocsToKeep.Clear();
                        Console.WriteLine(boolStatus(isOK));
                        Console.Write("Removing unwanted files and folders: ");
                        Console.WriteLine(boolStatus(Helpers.InitialCleanup.Clean()));
                    }
                }
            }
            var timeout = DateTime.UtcNow.AddSeconds(30).Ticks;
            while (Directory.Exists(AppConst.ProcessedPath)&&DateTime.UtcNow.Ticks<=timeout)
            {
                //cleanup may still happening
                Task.Delay(1000);
            }
            //create processed folder
            if (!Directory.Exists(AppConst.ProcessedPath)) Directory.CreateDirectory(AppConst.ProcessedPath);

            //we should now only have the files left that we want to work with.
            if(Directory.Exists(AppConst.TmpPath))Directory.EnumerateFiles(AppConst.TmpPath,"*.csv").ToList().ForEach((x) => {
                FileInfo fi = new FileInfo(x);
                //currently should start country or county
                var fn = fi.Name.Substring(0, 7).ToLower().TrimEnd();
                var newfn = Path.Combine(AppConst.ProcessedPath, fn + ".json");
                using(FileStream fs = fi.OpenRead())
                {
                    using (StreamReader sr=new StreamReader(fs))
                    {
                        var firstline = sr.ReadLine();
                        if (string.IsNullOrWhiteSpace(firstline)) throw new NullReferenceException("First line of file is incorrect " + fi.Name);
                        switch (fn)
                        {
                            case "county":
                                if (Models.CountyNames.CsvCheckOK(firstline))
                                {
                                    var counties = new List<Models.CountyNames>();
                                    while (!sr.EndOfStream)
                                    {
                                        var l = sr.ReadLine();
                                        if (!string.IsNullOrWhiteSpace(l)) counties.Add(Models.CountyNames.FromLine(l));
                                    }
                                    if (counties.Count > 0)
                                    {
                                       File.WriteAllText(newfn, Newtonsoft.Json.JsonConvert.SerializeObject(counties),System.Text.Encoding.UTF8);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Error with first line of " + fi.Name);
                                }
                                break;
                            case "country":
                                if (Models.CountryNamesUk.CsvCheckOk(firstline))
                                {
                                    var countries = new List<Models.CountryNamesUk>();
                                    while (!sr.EndOfStream)
                                    {
                                        var l = sr.ReadLine();
                                        if (!string.IsNullOrWhiteSpace(l)) countries.Add(Models.CountryNamesUk.FromLine(l));
                                    }
                                    if (countries.Count > 0)
                                    {
                                        File.WriteAllText(newfn, Newtonsoft.Json.JsonConvert.SerializeObject(countries), System.Text.Encoding.UTF8);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Error with first line of " + fi.Name);
                                }
                                break;
                            default:
                                Console.WriteLine("Unknown file to process: " + fi.Name);
                                break;
                        }
                    }
                }
            });

            Console.WriteLine();
            Console.Write("Postcode files to process: ");
            var postcodeCsvFiles =!string.IsNullOrEmpty(AppConst.DataCsvPath)? Directory.EnumerateFiles(AppConst.DataCsvPath).ToList():new List<string>();
            Console.WriteLine(postcodeCsvFiles.Count);
            if (postcodeCsvFiles.Count > 0)
            {
                PcFiles.Clear();
                for(int x = 0; x < postcodeCsvFiles.Count; x++)
                {
                    PcFiles.Enqueue(postcodeCsvFiles[x]);
                }
                Console.Write("Please Wait");
                var sw = new Stopwatch();
                sw.Start();
                Task[] tasks = new Task[AppConst.ProcessTaskCount];
                for (int i = 0; i < AppConst.ProcessTaskCount; i++)
                {
                    tasks[i] = Task.Factory.StartNew(myAction);
                }
                Task.WaitAll(tasks);
                sw.Stop();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Took: " + sw.Elapsed.ToString());
                Console.WriteLine();
                sw.Restart();
                Console.WriteLine("Zipping files.");
                var outPathname = Path.Combine(unzippedDir, "postcode-data.zip");
                using (FileStream fsOut = File.Create(outPathname))
                using (var zipStream = new ZipOutputStream(fsOut))
                {

                    //0-9, 9 being the highest level of compression
                    zipStream.SetLevel(Math.Max(0,Math.Min(9,ZipCompressionLevel)));

                    // optional. Null is the same as not setting. Required if using AES.
                    zipStream.Password = ZipPassword;

                    // This setting will strip the leading part of the folder path in the entries, 
                    // to make the entries relative to the starting folder.
                    // To include the full path for each entry up to the drive root, assign to 0.
                    int folderOffset = AppConst.ProcessedPath.Length + (AppConst.ProcessedPath.EndsWith("\\") ? 0 : 1);

                    CompressFolder(AppConst.ProcessedPath, zipStream, folderOffset);

                }
                sw.Stop();
                Console.WriteLine("Zipping files took: " + sw.Elapsed.ToString());
                Console.WriteLine();
                Console.Write("Deleteing Folders: ");
                try
                {

                    if(!keepProcessedFiles) Directory.Delete(AppConst.ProcessedPath, true);
                    Directory.Delete(AppConst.TmpPath, true);
                    //reverse traverse data folders
                    var dn = AppConst.DataCsvPath;
                    while (!dn.Equals(unzippedDir))
                    {
                        Directory.Delete(dn, true);
                        dn = dn.Substring(0, dn.LastIndexOf(Path.DirectorySeparatorChar));
                    }
                    Console.WriteLine("Ok");
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed");
                }
            }
            msw.Stop();
            Console.WriteLine();
            Console.WriteLine("Total Process took: " + msw.Elapsed.ToString());
            if (PressAnyKeyToContinue)
            {
                Console.WriteLine();
                Console.WriteLine("Press Any Key To Exit");
                var k = Console.ReadKey();
            }
        }
        // Recursively compresses a folder structure
        private static void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
        {

            var files = Directory.GetFiles(path);
            Console.WriteLine($"Adding {files.Length} filtes to zip.");
            foreach (var filename in files)
            {
                Console.Write(".");
                var fi = new FileInfo(filename);

                // Make the name in zip based on the folder
                var entryName = filename.Substring(folderOffset);

                // Remove drive from name and fix slash direction
                entryName = ZipEntry.CleanName(entryName);

                var newEntry = new ZipEntry(entryName);

                // Note the zip format stores 2 second granularity
                newEntry.DateTime = fi.LastWriteTime;

                // Specifying the AESKeySize triggers AES encryption. 
                // Allowable values are 0 (off), 128 or 256.
                // A password on the ZipOutputStream is required if using AES.
                //   newEntry.AESKeySize = 256;

                // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003,
                // WinZip 8, Java, and other older code, you need to do one of the following: 
                // Specify UseZip64.Off, or set the Size.
                // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, 
                // you do not need either, but the zip will be in Zip64 format which
                // not all utilities can understand.
                //   zipStream.UseZip64 = UseZip64.Off;
                newEntry.Size = fi.Length;

                zipStream.PutNextEntry(newEntry);

                // Zip the file in buffered chunks
                // the "using" will close the stream even if an exception occurs
                var buffer = new byte[4096];
                using (FileStream fsInput = File.OpenRead(filename))
                {
                    StreamUtils.Copy(fsInput, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }
            Console.WriteLine();
            // Recursively call CompressFolder on all folders in path
            var folders = Directory.GetDirectories(path);
            foreach (var folder in folders)
            {
                CompressFolder(folder, zipStream, folderOffset);
            }
        }
        public static Action myAction = delegate ()
              {
                  while (PcFiles.Count > 0)
                  {
                      string fn;
                      if (PcFiles.TryDequeue(out fn))
                      {
                          Console.Write(".");
                          if (string.IsNullOrWhiteSpace(fn)) continue;
                          var fi = new FileInfo(fn);
                          bool del = false;
                          using (FileStream fs = fi.OpenRead())
                          {
                              using (StreamReader sr = new StreamReader(fs))
                              {
                                  var firstLine = sr.ReadLine();
                                  Models.PostcodesUK puk = new Models.PostcodesUK(firstLine);
                                  if (Models.PostcodesUK.IsValid)
                                  {
                                      try
                                      {
                                          List<Models.PostcodeFields> postcodes = new List<Models.PostcodeFields>();
                                          while (!sr.EndOfStream)
                                          {
                                              var line = sr.ReadLine();
                                              if (!string.IsNullOrWhiteSpace(line))
                                              {
                                                  var p = puk.GetPostcode(line);
                                                  if (p.HasPCD) postcodes.Add(p);
                                              }
                                          }
                                          if (postcodes.Count > 0)
                                          {
                                              var n = new System.Text.RegularExpressions.Regex(@"([A-Z]+)\.csv").Match(fn).Groups[1].Value;
                                              var fnnew = Path.Combine(AppConst.ProcessedPath, "postcodes_" + n + ".json");
                                              File.WriteAllText(fnnew, Newtonsoft.Json.JsonConvert.SerializeObject(postcodes), System.Text.Encoding.UTF8);
                                              del = true;
                                          }
                                      }
                                      catch (Exception)
                                      {
                                          Console.WriteLine("Failed " + fn);
                                      }
                                  }
                                  else
                                  {
                                      Console.WriteLine("Invalid " + fn);
                                  }
                              }
                          }
                          if (del)
                          {
                              try
                              {
                                  Task.Delay(10);
                                  fi.Delete();
                              }
                              catch (Exception) { }
                          }
                      }
                      else
                      {
                          Task.Delay(10);
                      }
                  }
              };
        static string boolStatus(bool tf)
        {
            return tf ? "Ok" : "Fail";
        }
    }
}
