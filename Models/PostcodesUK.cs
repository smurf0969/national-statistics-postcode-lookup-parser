using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace national_statistics_postcode_lookup_parser.Models
{
    public class PostcodesUK
    {
        static readonly ColMap[] colMaps= {
                new ColMap("pcd", "pcd","pcd7"),
                    new ColMap("pcd2", "pcd2","pcd8"),
                    new ColMap("pcds", "pcds"),
                    new ColMap("dointr", "dointr"),
                    new ColMap("doterm", "doterm"),
                    new ColMap("usertype", "usertype","usrtypind"),
                    new ColMap("oseast1m", "oseast1m","east1m"),
                    new ColMap("osnrth1m", "osnrth1m","north1m"),
                    new ColMap("osgrdind", "osgrdind","gridind"),
                    new ColMap("cty", "cty","cty19cd"),
                    new ColMap("ctry", "ctry","ctry11cd"),
                    new ColMap("lat", "lat"),
                    new ColMap("long", "long")
                };
        static Dictionary<string, string> remap = new Dictionary<string, string>();
        static PostcodesUK()
        {
            foreach(var m in colMaps)
            {
                foreach(var nsplcol in m.NSPL_COLS)
                {
                    remap.Add(nsplcol, m.ID);
                    remap.Add(nsplcol.ToUpper(), m.ID);
                }
            }
        }
        //static string[] ColumnsWanted = {
        //    "pcd","pcd2","pcds",
        //    "dointr","doterm","usertype",
        //    "oseast1m","osnrth1m","osgrdind",
        //    "cty","ctry",
        //    "lat","long"
        //};
        static Dictionary<int, string> IdxCol = new Dictionary<int, string>();
        public static bool IsValid { get; private set; }
        static int count=-1;
        public PostcodesUK(string firstLine)
        {
            var c = Interlocked.Increment(ref count);
            if(c>=1 && c < AppConst.ProcessTaskCount)
            {
                //delay so c==0 can build the index of columns
                Task.Delay(1000);
            }else if (c == 0)
            {
                var parts = firstLine.Split(',');
                if (parts.Length >= colMaps.Length)
                {
                    for(int i = 0; i < parts.Length; i++)
                    {

                        if (remap.Keys.Contains(parts[i])){
                            IdxCol.Add(i, remap[parts[i]]);
                        }
                    }
                }
                IsValid = IdxCol.Count == colMaps.Length;
            }
        }
        public PostcodeFields GetPostcode(string line)
        {
            PostcodeFields pc = new PostcodeFields();
            var parts = line.Split(',');
            int c = parts.Length;
            try
            {
                foreach (var kvp in IdxCol)
                {
                    if (kvp.Key >= c) break;
                    switch (kvp.Value)
                    {
                        case "pcd":
                            pc.PCD = parts[kvp.Key].Trim('"');
                            break;
                        case "pcd2":
                            pc.PCD2 = parts[kvp.Key].Trim('"');
                            break;
                        case "pcds":
                            pc.PCDS = parts[kvp.Key].Trim('"');
                            break;
                        case "dointr":
                            pc.DOINTR = parts[kvp.Key].Trim('"');
                            break;
                        case "doterm":
                            pc.DOTERM = parts[kvp.Key].Trim('"');
                            break;
                        case "usertype":
                            pc.USERTYPE = uint.Parse(parts[kvp.Key].Trim('"'));
                            break;
                        case "oseast1m":
                            pc.OSEAST1M = parts[kvp.Key].Trim('"');
                            break;
                        case "osnrth1m":
                            pc.OSNRTH1M = parts[kvp.Key].Trim('"');
                            break;
                        case "osgrdind":
                            pc.OSGRDIND = int.Parse(parts[kvp.Key].Trim('"'));
                            break;
                        case "cty":
                            pc.CTY = parts[kvp.Key].Trim('"');
                            break;
                        case "ctry":
                            pc.CTRY = parts[kvp.Key].Trim('"');
                            break;
                        case "lat":
                            pc.LAT = parts[kvp.Key].Trim('"');
                            break;
                        case "long":
                            pc.LONG = parts[kvp.Key].Trim('"');
                            break;
                    }
                }
            }catch(Exception ex)
            {

            }
            return pc;
        }
    }

}
