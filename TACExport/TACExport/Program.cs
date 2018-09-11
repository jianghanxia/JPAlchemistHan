using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ExcelDataReader;
using Ionic.Zlib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TACTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Diff();

            //InitJP();
            InitCN();

            //WordList();
            //JsonList();

            Console.WriteLine("完成");
            Console.ReadLine();
        }

        public static void Diff()
        {
            List<CBItem> CB = new List<CBItem>();
            using (var reader = ExcelReaderFactory.CreateReader(File.Open("KeyMinori.xlsx", FileMode.Open, FileAccess.Read)))
            {
                do
                {
                    while (reader.Read())
                    {
                        CB.Add(new CBItem {IDstr = reader.GetString(0), ID = reader.GetString(2), Chinese = reader.GetString(4)});
                    }
                } while (reader.NextResult());
            }

            using (var result = new StreamWriter(new FileStream(@"ResultDiff.txt", FileMode.Create, FileAccess.Write), Encoding.UTF8))
            {
                using (var reader = ExcelReaderFactory.CreateReader(File.Open("Old.xlsx", FileMode.Open, FileAccess.Read)))
                {
                    do
                    {
                        while (reader.Read())
                        {
                            var km = CB.First(i => i.IDstr == reader.GetString(0) && i.ID == reader.GetString(2));
                            if (km.Chinese != reader.GetString(4))
                            {
                                result.WriteLine($"{reader.GetString(0)}\t{reader.GetString(2)}\t{km.Chinese}");
                            }
                        }
                    } while (reader.NextResult());
                }
            }
        }

        public static void InitJP()
        {
            Console.WriteLine("初始化日服数据");

            var code = GetWeb("https://jianghanxia.gitee.io/jpalchemisthan/ver");
            var verj = JToken.Parse(GetWeb("https://alchemist.gu3.jp/chkver2", "Post", $"ver={code}"));
            var url = $"https://alchemist-dlc2.gu3.jp/assets/{verj.SelectToken("body.environments.alchemist.assets")}/aatc";
            GetDataAsync($"{url}/ASSETLIST", "ASSETLISTJP_new");

            var colljp = GetCollection("ASSETLISTJP_new");

            GetCollectionFile(url, "ASSETLISTJP", "DataJP", colljp.list);
            GetFileAsync($"{url}/b9cc206f", "DataJP/b9cc206f");
            GetFileAsync($"{url}/49744fd6", "DataJP/49744fd6");

            File.Copy("ASSETLISTJP_new", "ASSETLISTJP", true);
            File.Delete("ASSETLISTJP_new");

            Console.WriteLine("生成日服词表");
            GetLoc("DataJP", true, colljp.list);
        }

        public static void InitCN()
        {
            Console.WriteLine("初始化国服数据");

            var code = "40058";
            var url = $"http://update-alccn-prod.ssl.91dena.cn/assets/{code}/aatc";
            GetDataAsync($"{url}/ASSETLIST", "ASSETLISTCN_new");

            var colljp = GetCollection("ASSETLISTCN_new");

            GetCollectionFile(url, "ASSETLISTCN", "DataCN", colljp.list);
            GetFileAsync($"{url}/35e7c476", "DataCN/35e7c476");
            GetFileAsync($"{url}/64a5ea86", "DataCN/64a5ea86");

            File.Copy("ASSETLISTCN_new", "ASSETLISTCN", true);
            File.Delete("ASSETLISTCN_new");

            Console.WriteLine("生成国服词表");
            GetLoc("DataCN", false, colljp.list);
        }

        public static void WordList()
        {
            Console.WriteLine("生成Loc词表");

            var diff = new List<CBItem>();
            using (var sReader = new StreamReader(new FileStream(@"ResultDiff.txt", FileMode.Open), Encoding.UTF8))
            {
                while (!sReader.EndOfStream)
                {
                    var res = sReader.ReadLine();
                    var a = res.Split('\t');
                    diff.Add(new CBItem { IDstr = a[0], ID = a[1], Chinese = a[2] });
                }
            }

            List<CBItem> GFCB = new List<CBItem>();
            using (var sReader = new StreamReader(new FileStream(@"CBCN.txt", FileMode.Open), Encoding.UTF8))
            {
                while (!sReader.EndOfStream)
                {
                    var res = sReader.ReadLine();
                    var a = res.Split('\t');
                    GFCB.Add(new CBItem { IDstr = a[0], Path = a[1], ID = a[2], Chinese = a[3] });
                }
            }

            using (var sReader = new StreamReader(new FileStream(@"CBJP.txt", FileMode.Open), Encoding.UTF8))
            {
                using (var result = new StreamWriter(new FileStream(@"ResultLoc.txt", FileMode.Create, FileAccess.Write), Encoding.UTF8))
                {
                    while (!sReader.EndOfStream)
                    {
                        var res = sReader.ReadLine();
                        var a = res.Split('\t');

                        var h = GFCB.Where(i => i.Path == a[1] && i.ID == a[2]);
                        var dh = diff.Where(i => i.IDstr == a[0] && i.ID == a[2]);
                        result.WriteLine($"{res}\t{(dh.Any() ? dh.First().Chinese : h.Any() ? h.First().Chinese : "")}");
                    }
                }
            }
        }

        private static void JsonList()
        {
            Console.WriteLine("解析JSON数据");
            var qpcn = QuestParam("35e7c476", "DataCN/35e7c476");
            var qpjp = QuestParam("b9cc206f", "DataJP/b9cc206f");

            var mpcn = MasterParam("64a5ea86", "DataCN/64a5ea86");
            var mpjp = MasterParam("49744fd6", "DataJP/49744fd6");

            Console.WriteLine("抓取WIKI数据");
            List<WikiData> unit;
            if (File.Exists("WikiUnit.txt"))
            {
                unit = JsonConvert.DeserializeObject<WikiJson>(File.ReadAllText("WikiUnit.txt"))._embedded;
            }
            else
            {
                unit = GetWikiData(@"https://tagatame.huijiwiki.com/api/rest_v1/namespace/data?filter={""_id"":{""$regex"":""Data:Unit/""}}&keys={""iname"":1,""name_chs"":1}&count=true");
            }

            unit.ForEach(i => i.Path = $"Unit[?(@.iname=='{i.iname}')].name");

            List<WikiData> jobs;
            if (File.Exists("WikiJobs.txt"))
            {
                jobs = JsonConvert.DeserializeObject<WikiJson>(File.ReadAllText("WikiJobs.txt"))._embedded;
            }
            else
            {
                jobs = GetWikiData(@"https://tagatame.huijiwiki.com/api/rest_v1/namespace/data?filter={""_id"":{""$regex"":""Data:Job/""}}&keys={""iname"":1,""name_chs"":1}&count=true");
            }

            jobs.ForEach(i => i.Path = $"Job[?(@.iname=='{i.iname}')].name");

            var wikidata = new List<WikiData>(unit);
            wikidata.AddRange(jobs);

            Console.WriteLine("合并JSON数据");
            using (var result = new StreamWriter(new FileStream("JSONWord.txt", FileMode.Create, FileAccess.Write), Encoding.UTF8))
            {
                WriteJsonResult(qpjp, qpcn, wikidata, result);
                WriteJsonResult(mpjp, mpcn, wikidata, result);
            }
        }

        public static List<CBItem> QuestParam(string id, string file)
        {
            List<CBItem> CB = new List<CBItem>();
            using (var sReader = new StreamReader(new FileStream(file, FileMode.Open), Encoding.UTF8))
            {
                var js = JToken.Parse(sReader.ReadToEnd());

                foreach (var a in js["areas"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"areas[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                    CB.Add(new CBItem { IDstr = id, ID = $"areas[?(@.iname=='{a["iname"]}')].expr", Chinese = $"{a["expr"]}" });
                }

                foreach (var a in js["quests"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"quests[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                    CB.Add(new CBItem { IDstr = id, ID = $"quests[?(@.iname=='{a["iname"]}')].expr", Chinese = $"{a["expr"]}" });
                    CB.Add(new CBItem { IDstr = id, ID = $"quests[?(@.iname=='{a["iname"]}')].cond", Chinese = $"{a["cond"]}" });
                    CB.Add(new CBItem { IDstr = id, ID = $"quests[?(@.iname=='{a["iname"]}')].title", Chinese = $"{a["title"]}" });
                }

                foreach (var a in js["towerFloors"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"towerFloors[?(@.iname=='{a["iname"]}')].title", Chinese = $"{a["title"]}" });
                    CB.Add(new CBItem { IDstr = id, ID = $"towerFloors[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                    CB.Add(new CBItem { IDstr = id, ID = $"towerFloors[?(@.iname=='{a["iname"]}')].cond", Chinese = $"{a["cond"]}" });
                }
            }

            return CB;
        }

        public static List<CBItem> MasterParam(string id, string file)
        {
            List<CBItem> CB = new List<CBItem>();
            using (var sReader = new StreamReader(new FileStream(file, FileMode.Open), Encoding.UTF8))
            {
                var js = JToken.Parse(sReader.ReadToEnd());

                foreach (var a in js["Ability"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"Ability[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                    CB.Add(new CBItem { IDstr = id, ID = $"Ability[?(@.iname=='{a["iname"]}')].expr", Chinese = $"{a["expr"]}" });
                }

                foreach (var a in js["Artifact"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"Artifact[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                }

                foreach (var a in js["Award"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"Award[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                    CB.Add(new CBItem { IDstr = id, ID = $"Award[?(@.iname=='{a["iname"]}')].expr", Chinese = $"{a["expr"]}" });
                }

                foreach (var a in js["Challenge"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"Challenge[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                }

                foreach (var a in js["ConceptCard"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"ConceptCard[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                    CB.Add(new CBItem { IDstr = id, ID = $"ConceptCard[?(@.iname=='{a["iname"]}')].expr", Chinese = $"{a["expr"]}" });
                }

                foreach (var a in js["Item"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"Item[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                }

                foreach (var a in js["Job"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"Job[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                }

                foreach (var a in js["Skill"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"Skill[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                    CB.Add(new CBItem { IDstr = id, ID = $"Skill[?(@.iname=='{a["iname"]}')].expr", Chinese = $"{a["expr"]}" });
                }

                foreach (var a in js["TobiraCategories"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"TobiraCategories[?(@.category=='{a["category"]}')].name", Chinese = $"{a["name"]}" });
                }

                foreach (var a in js["Trick"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"Trick[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                    CB.Add(new CBItem { IDstr = id, ID = $"Trick[?(@.iname=='{a["iname"]}')].expr", Chinese = $"{a["expr"]}" });
                }

                foreach (var a in js["Trophy"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"Trophy[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                }

                foreach (var a in js["Unit"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"Unit[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                }
            }

            return CB;
        }

        public static List<WikiData> GetWikiData(string url)
        {
            var wu = GetWeb(url);
            var jw = JsonConvert.DeserializeObject<WikiJson>(wu);
            var result = jw._embedded.ToList();

            if (jw._total_pages > 1)
            {
                for (int i = 2; i <= jw._total_pages; i++)
                {
                    var wup = GetWeb($"{url}&page={i}");
                    var jwp = JsonConvert.DeserializeObject<WikiJson>(wup);
                    result.AddRange(jwp._embedded);
                }
            }

            return result;
        }

        public static void WriteJsonResult(List<CBItem> jplist, List<CBItem> cnlist, List<WikiData> wikidata, StreamWriter result)
        {
            foreach (var item in jplist)
            {
                var wi = wikidata.Where(i => i.Path == item.ID);
                if (wi.Any())
                {
                    result.WriteLine($"{item.IDstr}\t{item.ID}\t{wi.First().name_chs}");
                }
                else
                {
                    var h = cnlist.Where(i => i.ID == item.ID);
                    if (h.Any())
                    {
                        if (!string.IsNullOrWhiteSpace(h.First().Chinese))
                        {
                            result.WriteLine($"{item.IDstr}\t{item.ID}\t{h.First().Chinese}");
                        }
                    }
                }
            }
        }

        private static void GetCollectionFile(string url, string oldfile, string dir, List<Item> collection)
        {
            if (File.Exists(oldfile))
            {
                var oldcol = GetCollection(oldfile);

                Directory.CreateDirectory(dir);
                Parallel.ForEach(collection.Where(i => i.Path.StartsWith("Loc/")), (item) =>
                {
                    if (!File.Exists($"{dir}/{item.IDStr}") || !oldcol.list.Any(i => i.ID == item.ID && i.Path == item.Path && i.Hash == item.Hash))
                    {
                        GetFileAsync($"{url}/{item.IDStr}", $"{dir}/{item.IDStr}");
                        Console.WriteLine(item.IDStr);
                    }
                });
            }
            else
            {
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                }

                Directory.CreateDirectory(dir);

                Parallel.ForEach(collection.Where(i => i.Path.StartsWith("Loc/")), (item) =>
                {
                    GetFileAsync($"{url}/{item.IDStr}", $"{dir}/{item.IDStr}");
                    Console.WriteLine(item.IDStr);
                });
            }
        }

        public static void GetLoc(string dir, bool jp, List<Item> collection)
        {
            var updateTime = DateTime.Now;
            using (var context = new Context())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                context.Configuration.ValidateOnSaveEnabled = false;

                foreach (var item in collection.Where(i => i.Path.StartsWith("Loc/")).OrderBy(i => i.IDStr))
                {
                    Console.WriteLine(item.IDStr);

                    using (var file = new StreamReader(new FileStream($"{dir}/{item.IDStr}", FileMode.Open), Encoding.UTF8))
                    {
                        while (!file.EndOfStream)
                        {
                            var s = file.ReadLine();

                            var a = s.Split(new[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);
                            if (a.Length > 1 && s != "\r")
                            {
                                var id = a[0];
                                if (context.TacTrans.Any(i => i.Path == item.Path && i.ID == id))
                                {
                                    var re = context.TacTrans.First(i => i.Path == item.Path && i.ID == id);
                                    if (jp)
                                    {
                                        re.JP = a[1];
                                    }
                                    else
                                    {
                                        re.CN = a[1];
                                    }

                                    re.UpdateTime = updateTime;
                                }
                                else
                                {
                                    context.TacTrans.Add(jp
                                        ? new TacTrans {IDStr = item.IDStr, Path = item.Path, ID = a[0], JP = a[1], UpdateTime = updateTime}
                                        : new TacTrans {IDStr = item.IDStr, Path = item.Path, ID = a[0], CN = a[1], UpdateTime = updateTime});
                                }
                            }
                        }
                    }
                    context.SaveChanges();
                }
            }

            //using (var wf = new StreamWriter(new FileStream("4.txt", FileMode.Create, FileAccess.Write), Encoding.UTF8))
            //{
            //    foreach (var item in collection.Where(i => i.Path.StartsWith("Loc/")).OrderBy(i => i.IDStr))
            //    {
            //        using (var file = new StreamReader(new FileStream($"{dir}/{item.IDStr}", FileMode.Open), Encoding.UTF8))
            //        {
            //            while (!file.EndOfStream)
            //            {
            //                var s = file.ReadLine();

            //                var a = s.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
            //                if (a.Length > 1 && s != "\r")
            //                {
            //                    wf.WriteLine($"{item.IDStr}\t{item.Path}\t{a[0]}\t{a[1]}");
            //                }
            //            }
            //        }
            //    }
            //}
        }

        public static void GetFileList(List<Item> collection, string filename)
        {
            using (var file = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write), Encoding.UTF8))
            {
                foreach (var item in collection)
                {
                    file.WriteLine($"{item.IDStr}\t{item.Flags}\t{item.Path}\t");
                }
            }
        }

        public static (List<Item> list, int ver) GetCollection(string file)
        {
            using (BinaryReader binaryReader = new BinaryReader(File.Open(file, FileMode.Open)))
            {
                var revision = binaryReader.ReadInt32();
                var num = binaryReader.ReadInt32();
                var collection = new List<Item>();

                for (int i = 0; i < num; i++)
                {
                    var item = new Item();
                    item.ID = binaryReader.ReadUInt32();
                    item.IDStr = item.ID.ToString("X8").ToLower();
                    item.Size = binaryReader.ReadInt32();
                    item.CompressedSize = binaryReader.ReadInt32();
                    item.Path = binaryReader.ReadString();
                    item.PathHash = binaryReader.ReadInt32();
                    item.Hash = binaryReader.ReadUInt32();
                    item.Flags = (AssetBundleFlags)binaryReader.ReadInt32();

                    int num2 = binaryReader.ReadInt32();
                    for (int j = 0; j < num2; j++)
                    {
                        int index = binaryReader.ReadInt32();
                        item.Dependencies.Add(index);
                    }

                    num2 = binaryReader.ReadInt32();
                    for (int k = 0; k < num2; k++)
                    {
                        int index = binaryReader.ReadInt32();
                        item.AdditionalDependencies.Add(index);
                    }

                    num2 = binaryReader.ReadInt32();
                    for (int l = 0; l < num2; l++)
                    {
                        int index = binaryReader.ReadInt32();
                        item.AdditionalStreamingAssets.Add(index);
                    }

                    collection.Add(item);
                }

                return (collection, revision);
            }
        }

        public static string GetWeb(string url, string method = "Get", string postData = "", int errnum = 0)
        {
            try
            {
                //HTTPSQ请求  
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;

                var request = WebRequest.CreateHttp(url);
                request.Method = method.ToUpper();
                request.Accept = "identity";
                request.UserAgent = "Dalvik/2.1.0 (Linux; U; Android 6.0; R11/MRA58K)";

                if (method == "Post" && postData != "")
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                    var bytes = Encoding.UTF8.GetBytes(postData);
                    var stream = request.GetRequestStream();
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Close();
                }

                var response = request.GetResponse();

                using (var stream = response.GetResponseStream())
                {
                    using (var sReader = new StreamReader(stream, Encoding.GetEncoding("UTF-8")))
                    {
                        var res = sReader.ReadToEnd();
                        return res;
                    }
                }
            }
            catch (Exception e)
            {
                if (errnum < 5)
                {
                    Thread.Sleep(2000);
                    errnum += 1;
                    return GetWeb(url, method, postData, errnum);
                }

                throw e;
            }
        }

        public static void GetDataAsync(string url, string filename)
        {
            //HTTPSQ请求  
            ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;

            var request = WebRequest.CreateHttp(url);

            request.Method = "GET";
            request.Accept = "identity";
            request.UserAgent = "Dalvik/2.1.0 (Linux; U; Android 6.0; R11/MRA58K)";

            var response = request.GetResponse();

            string res;
            using (var steam = response.GetResponseStream())
            {
                using (Stream output = File.OpenWrite(filename))
                {
                    steam.CopyTo(output);
                }
            }

            response.Close();
        }

        public static void GetFileAsync(string url, string filename)
        {
            //HTTPSQ请求  
            ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;

            var request = WebRequest.CreateHttp(url);

            request.Method = "GET";
            request.Accept = "identity";
            request.UserAgent = "Dalvik/2.1.0 (Linux; U; Android 6.0; R11/MRA58K)";

            var response = request.GetResponse();

            using (var steam = response.GetResponseStream())
            {
                using (var zlibsteam = new ZlibStream(steam, CompressionMode.Decompress))
                {
                    using (Stream output = File.OpenWrite(filename))
                    {
                        zlibsteam.CopyTo(output);
                    }
                }
            }

            response.Close();
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受     
        }
    }

    public class WikiJson
    {
        public List<WikiData> _embedded { get; set; }
        public string _id { get; set; }
        public int _size { get; set; }
        public int _total_pages { get; set; }
    }

    public class WikiData
    {
        public string _id { get; set; }
        public string iname { get; set; }
        public string name_chs { get; set; }
        public string Path { get; set; }
    }

    public class CBItem
    {
        public string IDstr;
        public string Path;
        public string ID;
        public string Chinese;
    }

    public class Item
    {
        public uint ID;
        public string IDStr;
        public int Size;
        public int CompressedSize;
        public string Path;
        public int PathHash;
        public uint Hash;
        public AssetBundleFlags Flags;
        public List<int> Dependencies = new List<int>();
        public List<int> AdditionalDependencies = new List<int>();
        public List<int> AdditionalStreamingAssets = new List<int>();
        public bool Exist;
    }

    [Flags]
    public enum AssetBundleFlags
    {
        Compressed = 1,
        RawData = 2,
        Required = 4,
        Scene = 8,
        Tutorial = 16,
        Multiplay = 32,
        StreamingAsset = 64,
        TutorialMovie = 128,
        Persistent = 256,
        DiffAsset = 512
    }

    [Serializable]
    public class ChapterParam
    {
        public string iname;
        public string name;
        public string expr;
    }

    [Serializable]
    public class TowerFloorParam
    {
        public string iname;
        public string title;
        public string name;
        public int floor;
        public string tower_id;
        public MapParam[] map;
    }

    [Serializable]
    public class QuestParam
    {
        public string iname;
        public string title;
        public string name;
        public string expr;
        public string area;
        public QuestTypes type;
        public MapParam[] map;
    }

    [Serializable]
    public class MapParam
    {
        public string scn;
        public string set;
        public string bgm;
    }

    public enum QuestTypes
    {
        Story,
        Multi,
        Arena,
        Tutorial,
        Free,
        Event,
        Character,
        Tower,
        VersusFree,
        VersusRank
    }

    [Table("TacTrans")]
    public class TacTrans
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Key { get; set; }

        public string IDStr { get; set; }
        public string Path { get; set; }
        public string ID { get; set; }
        public string JP { get; set; }
        public string CN { get; set; }
        public string Trans { get; set; }
        public DateTime UpdateTime { get; set; }
    }

    public class Context : DbContext
    {
        public Context() : base("MyDbCS")
        {
        }

        public DbSet<TacTrans> TacTrans { get; set; }
    }
}
