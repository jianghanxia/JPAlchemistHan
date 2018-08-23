using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using Ionic.Zlib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AlchemistDMM
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;

            var code = GetWeb("https://jianghanxia.gitee.io/jpalchemisthan/ver");
            var ver = GetWeb("https://alchemist.gu3.jp/chkver2", "Post", $"ver={code}");
            var verj = JToken.Parse(ver);

            Console.WriteLine("下载ASSETLIST");
            GetData($"https://alchemist-dlc2.gu3.jp/assets/{verj.SelectToken("body.environments.alchemist.assets")}/aatc/ASSETLIST", "ASSETLIST_new");
            var collection = GetCollection("ASSETLIST_new");

            (List<Item> list, int ver) oldcol;
            if (File.Exists("ASSETLIST"))
            {
                oldcol = GetCollection("ASSETLIST");
            }
            else
            {
                if (Directory.Exists("Data"))
                {
                    Directory.Delete("Data", true);
                }
                oldcol = (new List<Item>(), 0);
            }
            Directory.CreateDirectory("Data");

            var cloc = collection.list.Where(i => i.Path.StartsWith("Loc/"));
            Parallel.ForEach(cloc, (item) =>
            {
                if (!File.Exists("Data/" + item.IDStr) || !oldcol.list.Any(i => i.ID == item.ID && i.Path == item.Path && i.Hash == item.Hash))
                {
                    GetData($"https://alchemist-dlc2.gu3.jp/assets/{verj.SelectToken("body.environments.alchemist.assets")}/aatc/{item.IDStr}", "Data/" + item.IDStr);
                    Console.WriteLine($"下载{item.IDStr}");
                }
            });

            GetData($"https://alchemist-dlc2.gu3.jp/assets/{verj.SelectToken("body.environments.alchemist.assets")}/aatc/b9cc206f", "Data/b9cc206f");
            GetData($"https://alchemist-dlc2.gu3.jp/assets/{verj.SelectToken("body.environments.alchemist.assets")}/aatc/49744fd6", "Data/49744fd6");

            Console.WriteLine("下载汉化数据");
            GetData("https://jianghanxia.gitee.io/jpalchemisthan/JPResult.xlsx", "JPResult.xlsx");
            GetData("https://jianghanxia.gitee.io/jpalchemisthan/JSONWord.gz", "JSONWord.gz");

            if (Directory.Exists("Han"))
            {
                Directory.Delete("Han", true);
            }
            Directory.CreateDirectory("Han");

            //Loc数据汉化
            HanLoc();

            //JSON汉化
            JsonHan();

            File.Copy("ASSETLIST_new", "ASSETLIST", true);
            File.Delete("ASSETLIST_new");
            File.Delete("JPResult.xlsx");
            File.Delete("JSONWord.gz");

            Console.WriteLine("汉化完成！");
            Console.Read();
        }

        public static void HanLoc()
        {
            var cb = new List<CBItem>();
            using (var stream = File.Open("JPResult.xlsx", FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    do
                    {
                        while (reader.Read())
                        {
                            if (!string.IsNullOrWhiteSpace(reader.GetString(4)))
                            {
                                cb.Add(new CBItem { IDstr = reader.GetString(0), ID = reader.GetString(2), Chinese = reader.GetString(4) });
                            }
                        }
                    } while (reader.NextResult());
                }
            }

            var fl = cb.Select(i => i.IDstr).Distinct();
            foreach (var file in fl)
            {
                if (File.Exists("Data/" + file))
                {
                    Console.WriteLine($"翻译{file}");

                    var fcb = cb.Where(i => i.IDstr == file);
                    using (var sReader = new StreamReader(new ZlibStream(new FileStream(Path.Combine("Data/", file), FileMode.Open), CompressionMode.Decompress), Encoding.UTF8))
                    {
                        using (var f = File.Open(Path.Combine("Han/", file), FileMode.Create))
                        {
                            using (var result = new StreamWriter(new ZlibStream(f, CompressionMode.Compress, CompressionLevel.BestCompression), Encoding.UTF8))
                            {
                                while (!sReader.EndOfStream)
                                {
                                    var res = sReader.ReadLine();

                                    var a = res.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                                    if (a.Length > 1 && res != "\r")
                                    {
                                        var h = fcb.Where(i => i.ID == a[0]);
                                        if (h.Any())
                                        {
                                            a[1] = h.First().Chinese;
                                        }

                                        var concat = string.Join("\t", a);
                                        result.WriteLine(concat);
                                    }
                                    else
                                    {
                                        result.WriteLine(res);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void JsonHan()
        {
            var cb = new List<CBItem>();
            using (var sReader = new StreamReader(new GZipStream(File.Open("JSONWord.gz", FileMode.Open), CompressionMode.Decompress), Encoding.UTF8))
            {
                while (!sReader.EndOfStream)
                {
                    var res = sReader.ReadLine();
                    var a = res.Split('\t');
                    cb.Add(new CBItem { IDstr = a[0], ID = a[1], Chinese = a[2] });
                }
            }

            var fl = cb.Select(i => i.IDstr).Distinct();
            foreach (var file in fl)
            {
                if (File.Exists("Data/" + file))
                {
                    Console.WriteLine($"翻译JSON:{file}");

                    var fcb = cb.Where(i => i.IDstr == file);
                    using (var sReader = new StreamReader(new ZlibStream(new FileStream("Data/" + file, FileMode.Open), CompressionMode.Decompress), Encoding.UTF8))
                    {
                        var json = JToken.Parse(sReader.ReadToEnd());

                        foreach (var cbi in fcb)
                        {
                            var s = json.SelectToken(cbi.ID);
                            s?.Replace(cbi.Chinese);
                        }

                        var sd = json.ToString(Formatting.None);

                        byte[] byteArray = Encoding.UTF8.GetBytes(sd);
                        MemoryStream stream = new MemoryStream(byteArray);
                        using (var f = File.Open(Path.Combine("Han/", file), FileMode.Create))
                        {
                            using (var result = new ZlibStream(f, CompressionMode.Compress, CompressionLevel.BestCompression))
                            {
                                byte[] buffer = new byte[4096];
                                int n;
                                while ((n = stream.Read(buffer, 0, buffer.Length)) != 0)
                                {
                                    result.Write(buffer, 0, n);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static (List<Item> list, int ver) GetCollection(string file)
        {
            using (BinaryReader binaryReader = new BinaryReader(File.Open(file, FileMode.Open)))
            {
                var mRevision = binaryReader.ReadInt32();
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

                return (collection, mRevision);
            }
        }

        public static void GetData(string url, string filename)
        {
            var request = WebRequest.CreateHttp(url);

            request.Method = "GET";
            request.Accept = "identity";
            request.UserAgent = "Dalvik/2.1.0 (Linux; U; Android 6.0; R11/MRA58K)";

            var response = request.GetResponse();

            string res;
            using (var steam = response.GetResponseStream())
            {
                using (Stream output = File.Open(filename, FileMode.Create))
                {
                    steam.CopyTo(output);
                }
            }

            response.Close();
        }

        public static string GetWeb(string url, string method = "Get", string postData = "")
        {
            var request = WebRequest.CreateHttp(url);
            request.Method = method.ToUpper();

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

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受     
        }
    }

    public class CBItem
    {
        public string IDstr;
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
        public string HashStr => Hash.ToString("X8").ToLower();
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
}
