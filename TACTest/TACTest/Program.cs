using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zlib;

namespace TACTest
{
    class Program
    {
        static void Main(string[] args)
        {
            void Init()
            {
                GetDataAsync("http://update-alccn-prod.ssl.91dena.cn/assets/40019/aatc/ASSETLIST", "ASSETLISTGF");
                GetDataAsync("https://alchemist-dlc2.gu3.jp/assets/20180801_214aa310b0ba4d327f9b206ecc73fa3324fcce30_566f2/aatc/ASSETLIST", "ASSETLISTJP");

                var GFcollection = GetCollection("ASSETLISTGF");
                var JPcollection = GetCollection("ASSETLISTJP");

                GetFileList(GFcollection, "GFList.txt");
                GetFileList(JPcollection, "JPList.txt");

                GetLoc("GFLOC/", "GFCB.txt", "http://update-alccn-prod.ssl.91dena.cn/assets/40019/aatc/", GFcollection);
                GetLoc("JPLOC/", "JPCB.txt", "https://alchemist-dlc2.gu3.jp/assets/20180801_214aa310b0ba4d327f9b206ecc73fa3324fcce30_566f2/aatc/", JPcollection);
            }
            //Init();

            List<CBItem> GFCB = new List<CBItem>();
            using (var sReader = new StreamReader(new FileStream(@"GFCB.txt", FileMode.Open), Encoding.UTF8))
            {
                while (!sReader.EndOfStream)
                {
                    var res = sReader.ReadLine();
                    var a = res.Split('\t');
                    GFCB.Add(new CBItem {IDstr = a[0], Path = a[1], ID = a[2], Chinese = a[3]});
                }
            }
            ;

            using (var sReader = new StreamReader(new FileStream(@"JPCB.txt", FileMode.Open), Encoding.UTF8))
            {
                using (var result = new StreamWriter(new FileStream(@"JPResult.txt", FileMode.Create, FileAccess.Write), Encoding.UTF8))
                {
                    while (!sReader.EndOfStream)
                    {
                        var res = sReader.ReadLine();
                        var a = res.Split('\t');

                        var h = GFCB.Where(i => i.Path == a[1] && i.ID == a[2]);
                        result.WriteLine($"{res}\t{(h.Any() ? h.First().Chinese : "")}");
                    }
                }
            }

            //using (var sReader = new StreamReader(new FileStream(@"JPLOC/0a3ea344", FileMode.Open), Encoding.UTF8))
            //{
            //    using (var result = new StreamWriter(new FileStream(@"2.txt", FileMode.Create, FileAccess.Write), Encoding.UTF8))
            //    {
            //        while (!sReader.EndOfStream)
            //        {
            //            var res = sReader.ReadLine();

            //            var a = res.Split(new[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);
            //            if (a.Length > 1 && res != "\r")
            //            {
            //                var h = CB.Where(i => i.Path == "Loc/japanese/EQ_VALENTINE2018_02_a_2d" && i.ID == a[0]);
            //                if (h.Any())
            //                {
            //                    a[1] = h.First().Chinese;
            //                }

            //                var concat = string.Join("\t", a);
            //                result.WriteLine(concat);
            //            }
            //            else
            //            {
            //                result.WriteLine(res);
            //            }
            //        }
            //    }
            //}


            ////http://update-alccn-prod.ssl.91dena.cn/assets/40019/aatc/e14329af

            //using (var wclient = new WebClient())
            //{
            //    wclient.BaseAddress = "http://update-alccn-prod.ssl.91dena.cn/assets/40019/aatc/";
            //    //Directory.CreateDirectory(Path.GetDirectoryName("LocalMaps/ev_seiseki_dj_2_ex2_set"));
            //    wclient.DownloadFile($"35e7c476", "QuestParam");
            //}

            //using (var file = File.OpenRead("QuestParam"))
            //{
            //    string res;
            //    using (var steam = new ZlibStream(file, CompressionMode.Decompress))
            //    {
            //        var sReader = new StreamReader(steam, Encoding.GetEncoding("UTF-8"));
            //        res = sReader.ReadToEnd();
            //    }
            //    ;
            //}

            //using (var input = File.OpenRead("1.txt"))
            //{
            //    using (var raw = File.Create("1"))
            //    {
            //        using (Stream compressor = new ZlibStream(raw, CompressionMode.Compress, CompressionLevel.BestCompression))
            //        {
            //            byte[] buffer = new byte[4096];
            //            int n;
            //            while ((n = input.Read(buffer, 0, buffer.Length)) != 0)
            //            {
            //                compressor.Write(buffer, 0, n);
            //            }
            //        }
            //    }
            //}
        }

        public static void GetLoc(string dir, string filename, string url, List<Item> collection)
        {
            Directory.CreateDirectory(dir);
            foreach (var item in collection.Where(i => i.Path.StartsWith("Loc/")))
            {
                GetFileAsync($"{url}{item.IDStr}", dir + item.IDStr);
                Console.WriteLine(item.IDStr);
            }

            using (var wf = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write), Encoding.UTF8))
            {
                var folder = new DirectoryInfo(dir);
                foreach (var file in folder.GetFiles())
                {
                    using (var sReader = new StreamReader(new FileStream(file.FullName, FileMode.Open), Encoding.UTF8))
                    {
                        while (!sReader.EndOfStream)
                        {
                            var s = sReader.ReadLine();

                            var a = s.Split(new[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);
                            if (a.Length > 1 && s != "\r")
                            {
                                var tt = collection.Single(i => i.IDStr == file.Name);
                                wf.WriteLine($"{tt.IDStr}\t{tt.Path}\t{a[0]}\t{a[1]}");
                            }
                        }
                    }
                }
            }
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

        public static List<Item> GetCollection(string file)
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
                    item.Flags = (AssetBundleFlags) binaryReader.ReadInt32();

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

                return collection;
            }
        }

        public static void GetDataAsync(string url, string filename)
        {
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
}
