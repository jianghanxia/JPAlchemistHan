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
using System.Security.Cryptography;

namespace AlchemistDMM
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;

            var code = GetWeb("https://jianghanxia.gitee.io/jpalchemisthan/ver");
            var verj = JToken.Parse(GetEncWeb("https://alchemist.gu3.jp", "/chkver2", $"{{\"ver\":\"{code}\"}}"));

            Console.WriteLine("下载ASSETLIST");
            GetData($"https://alchemist-dlc2.gu3.jp/assets/{verj.SelectToken("body.environments.alchemist.assets")}/ios/ASSETLIST", "ASSETLIST_new");
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
                    GetData($"https://alchemist-dlc2.gu3.jp/assets/{verj.SelectToken("body.environments.alchemist.assets")}/ios/{item.IDStr}", "Data/" + item.IDStr);
                    Console.WriteLine($"下载{item.IDStr}");
                }
            });

            //GetData($"https://alchemist-dlc2.gu3.jp/assets/{verj.SelectToken("body.environments.alchemist.assets")}/aatc/a8a590fa", "Data/a8a590fa");
            //GetData($"https://alchemist-dlc2.gu3.jp/assets/{verj.SelectToken("body.environments.alchemist.assets")}/aatc/f8ed758b", "Data/f8ed758b");

            Console.WriteLine("下载汉化数据");
            GetData("https://jianghanxia.gitee.io/jpalchemisthan/JPResult.xlsx", "JPResult.xlsx");
            //GetData("https://jianghanxia.gitee.io/jpalchemisthan/JSONWord.gz", "JSONWord.gz");

            if (Directory.Exists("Han"))
            {
                Directory.Delete("Han", true);
            }
            Directory.CreateDirectory("Han");

            //Loc数据汉化
            HanLoc();

            //JSON汉化
            //JsonHan();

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

        public static string GetEncWeb(string url, string action, string postData = "")
        {
            var u = $"{url}{action}";
            var request = WebRequest.CreateHttp(u);

            request.Method = "POST";
            request.ServicePoint.Expect100Continue = false;
            request.Accept = "*/*";
            request.UserAgent = "UnityPlayer/5.6.6f2 (UnityWebRequest/1.0, libcurl/7.51.0-DEV)";
            request.Headers.Add("X-Unity-Version", "5.6.6f2");
            request.Headers.Add("Accept-Encoding", "identity");
            request.Headers.Add("Content-Encoding", "identity");

            if (postData != "")
            {
                request.ContentType = "application/octet-stream+jhotuhiahanoatuhinga+fakamunatanga";
                var bytes = Encoding.UTF8.GetBytes(postData);
                var stream = request.GetRequestStream();
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
            }

            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    MemoryStream ms = new MemoryStream();
                    stream.CopyTo(ms);
                    var dec = ms.ToArray();
                    return Encoding.UTF8.GetString(Runtime.AesDecrypt(Runtime.GetEncryptionApp(action), dec));
                }
            }
        }

        public static string GetWeb(string url, string method = "Get", string postData = "")
        {
            var request = WebRequest.CreateHttp(url);
            request.Method = method.ToUpper();
            request.UserAgent = "alchemist/6.3.0.3 CFNetwork/902.2 Darwin/17.7.0";

            if (method == "Post" && postData != "")
            {
                request.ContentType = "application/json; charset=utf-8";
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

    public sealed class Runtime
    {
        static Runtime()
        {
        }

        public static byte[] AesDecrypt(byte[] key, byte[] data)
        {
            var iv = data.Take(16).ToArray();
            var se = data.Skip(16).ToArray();

            RijndaelManaged rDel = new RijndaelManaged { KeySize = 0x80, BlockSize = 0x80, Key = key, IV = iv, Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7 };
            var resultArray = rDel.CreateDecryptor().TransformFinalBlock(se, 0, se.Length);
            return resultArray;
        }

        public static byte[] GetEncryptionApp(string acion)
        {
            var app = new byte[] { 0x5F, 0x3E, 0x18, 0xC9, 0xC7, 0xD7, 0x43, 0xE8, 0xC7, 0x0B, 0x55, 0xDD, 0xED, 0xC8, 0x3B, 0xC9 };
            var ac = Encoding.ASCII.GetBytes(acion);
            var ha = app.Concat(ac).ToArray();
            return new SHA256Managed().ComputeHash(ha).Take(16).ToArray();
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
