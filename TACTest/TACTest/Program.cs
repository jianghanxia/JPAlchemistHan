using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zlib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TACTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Init();
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

                Directory.CreateDirectory("GFData");
                foreach (var item in GFcollection.Where(i => i.Path.StartsWith("Data/")))
                {
                    GetFileAsync($"http://update-alccn-prod.ssl.91dena.cn/assets/40019/aatc/{item.IDStr}", "GFData/" + item.IDStr);
                    Console.WriteLine(item.IDStr);
                }

                Directory.CreateDirectory("JPData");
                foreach (var item in JPcollection.Where(i => i.Path.StartsWith("Data/")))
                {
                    GetFileAsync($"https://alchemist-dlc2.gu3.jp/assets/20180801_214aa310b0ba4d327f9b206ecc73fa3324fcce30_566f2/aatc/{item.IDStr}", "JPData/" + item.IDStr);
                    Console.WriteLine(item.IDStr);
                }
            }

            WordList();
            void WordList()
            {
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
            }

            //Han();
            void Han()
            {
                List<CBItem> cb = new List<CBItem>();
                using (var sReader = new StreamReader(new FileStream("JPWord.txt", FileMode.Open), Encoding.UTF8))
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
                    if (File.Exists("JPLOC/" + file))
                    {
                        var fcb = cb.Where(i => i.IDstr == file);
                        using (var sReader = new StreamReader(new FileStream(Path.Combine("JPLOC/", file), FileMode.Open), Encoding.UTF8))
                        {
                            using (var result = new StreamWriter(new FileStream(@"temp.txt", FileMode.Create, FileAccess.Write), Encoding.UTF8))
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

                        Console.WriteLine(file);
                        File.Copy("temp.txt", Path.Combine("JPLOC/", file), true);
                    }
                }
            }

            QuestParam("35e7c476", "GFData/35e7c476", "GFQuestParam.txt");
            QuestParam("b9cc206f", "JPData/b9cc206f", "JPQuestParam.txt");
            void QuestParam(string id, string file, string output)
            {
                using (var wf = new StreamWriter(new FileStream(output, FileMode.Create, FileAccess.Write), Encoding.UTF8))
                {
                    using (var sReader = new StreamReader(new FileStream(file, FileMode.Open), Encoding.UTF8))
                    {
                        var js = JToken.Parse(sReader.ReadToEnd());

                        foreach (var a in js["areas"].Children())
                        {
                            wf.WriteLine($"{id}\tData/QuestParam\tareas[?(@.iname=='{a["iname"]}')].name\t{a["name"]}");
                            //wf.WriteLine($"{id}\tData/QuestParam\tareas[?(@.iname=='{a["iname"]}')].expr\t{a["expr"]}");
                        }

                        //foreach (var a in js["MapEffect"].Children())
                        //{
                        //    wf.WriteLine($"{id}\tData/QuestParam\tMapEffect[?(@.iname=='{a["iname"]}')].name\t{a["name"]}");
                        //}

                        //foreach (var a in js["multitowerFloor"].Children())
                        //{
                        //    wf.WriteLine($"{id}\tData/QuestParam\tmultitowerFloor[?(@.id=={a["id"]})].title\t{a["title"]}");
                        //    wf.WriteLine($"{id}\tData/QuestParam\tmultitowerFloor[?(@.id=={a["id"]})].name\t{a["name"]}");
                        //    wf.WriteLine($"{id}\tData/QuestParam\tmultitowerFloor[?(@.id=={a["id"]})].cond\t{a["cond"]}");
                        //}

                        foreach (var a in js["quests"].Children())
                        {
                            wf.WriteLine($"{id}\tData/QuestParam\tquests[?(@.iname=='{a["iname"]}')].name\t{a["name"]}");
                            wf.WriteLine($"{id}\tData/QuestParam\tquests[?(@.iname=='{a["iname"]}')].expr\t{a["expr"]}");
                            wf.WriteLine($"{id}\tData/QuestParam\tquests[?(@.iname=='{a["iname"]}')].cond\t{a["cond"]}");
                            wf.WriteLine($"{id}\tData/QuestParam\tquests[?(@.iname=='{a["iname"]}')].title\t{a["title"]}");
                        }

                        //foreach (var a in js["towerFloors"].Children())
                        //{
                        //    wf.WriteLine($"{id}\tData/QuestParam\ttowerFloors[?(@.iname=='{a["iname"]}')].title\t{a["title"]}");
                        //    wf.WriteLine($"{id}\tData/QuestParam\ttowerFloors[?(@.iname=='{a["iname"]}')].name\t{a["name"]}");
                        //    wf.WriteLine($"{id}\tData/QuestParam\ttowerFloors[?(@.iname=='{a["iname"]}')].cond\t{a["cond"]}");
                        //}

                        //foreach (var a in js["WeatherSet"].Children())
                        //{
                        //    wf.WriteLine($"{id}\tData/QuestParam\tWeatherSet[?(@.iname=='{a["iname"]}')].name\t{a["name"]}");
                        //}

                        //foreach (var a in js["worlds"].Children())
                        //{
                        //    wf.WriteLine($"{id}\tData/QuestParam\tworlds[?(@.iname=='{a["iname"]}')].name\t{a["name"]}");
                        //    wf.WriteLine($"{id}\tData/QuestParam\tworlds[?(@.iname=='{a["iname"]}')].expr\t{a["expr"]}");
                        //}
                    }
                }
            }

            MasterParam("64a5ea86", "GFData/64a5ea86", "GFMasterParam.txt");
            MasterParam("49744fd6", "JPData/49744fd6", "JPMasterParam.txt");
            void MasterParam(string id, string file, string output)
            {
                using (var wf = new StreamWriter(new FileStream(output, FileMode.Create, FileAccess.Write), Encoding.UTF8))
                {
                    using (var sReader = new StreamReader(new FileStream(file, FileMode.Open), Encoding.UTF8))
                    {
                        var js = JToken.Parse(sReader.ReadToEnd());

                        //foreach (var a in js["Ability"].Children())
                        //{
                        //    wf.WriteLine($"{id}\tData/MasterParam\tAbility[?(@.iname=='{a["iname"]}')].name\t{a["name"]}");
                        //    wf.WriteLine($"{id}\tData/MasterParam\tAbility[?(@.iname=='{a["iname"]}')].expr\t{a["expr"]}");
                        //}

                        //foreach (var a in js["Artifact"].Children())
                        //{
                        //    wf.WriteLine($"{id}\tData/MasterParam\tArtifact[?(@.iname=='{a["iname"]}')].name\t{a["name"]}");
                        //    wf.WriteLine($"{id}\tData/MasterParam\tArtifact[?(@.iname=='{a["iname"]}')].tag\t{a["tag"]}");
                        //}

                        //foreach (var a in js["Award"].Children())
                        //{
                        //    wf.WriteLine($"{id}\tData/MasterParam\tAward[?(@.iname=='{a["iname"]}')].name\t{a["name"]}");
                        //    wf.WriteLine($"{id}\tData/MasterParam\tAward[?(@.iname=='{a["iname"]}')].expr\t{a["expr"]}");
                        //}

                        foreach (var a in js["Challenge"].Children())
                        {
                            wf.WriteLine($"{id}\tData/MasterParam\tChallenge[?(@.iname=='{a["iname"]}')].name\t{a["name"]}");
                        }

                        //foreach (var a in js["ConceptCard"].Children())
                        //{
                        //    wf.WriteLine($"{id}\tData/MasterParam\tConceptCard[?(@.iname=='{a["iname"]}')].name\t{a["name"]}");
                        //    wf.WriteLine($"{id}\tData/MasterParam\tConceptCard[?(@.iname=='{a["iname"]}')].expr\t{a["expr"]}");
                        //}

                        //foreach (var a in js["Geo"].Children())
                        //{
                        //    wf.WriteLine($"{id}\tData/MasterParam\tGeo[?(@.iname=='{a["iname"]}')].name\t{a["name"]}");
                        //}

                        foreach (var a in js["Item"].Children())
                        {
                            wf.WriteLine($"{id}\tData/MasterParam\tItem[?(@.iname=='{a["iname"]}')].name\t{a["name"]}");
                        }

                        foreach (var a in js["Job"].Children())
                        {
                            wf.WriteLine($"{id}\tData/MasterParam\tJob[?(@.iname=='{a["iname"]}')].name\t{a["name"]}");
                        }

                        foreach (var a in js["Skill"].Children())
                        {
                            wf.WriteLine($"{id}\tData/MasterParam\tSkill[?(@.iname=='{a["iname"]}')].name\t{a["name"]}");
                            //wf.WriteLine($"{id}\tData/MasterParam\tSkill[?(@.iname=='{a["iname"]}')].expr\t{a["expr"]}");
                        }

                        //foreach (var a in js["TobiraCategories"].Children())
                        //{
                        //    wf.WriteLine($"{id}\tData/MasterParam\tTobiraCategories[?(@.category=={a["category"]})].name\t{a["name"]}");
                        //}

                        foreach (var a in js["Trick"].Children())
                        {
                            wf.WriteLine($"{id}\tData/MasterParam\tTrick[?(@.iname=='{a["iname"]}')].name\t{a["name"]}");
                            //wf.WriteLine($"{id}\tData/MasterParam\tTrick[?(@.iname=='{a["iname"]}')].expr\t{a["expr"]}");
                        }

                        foreach (var a in js["Trophy"].Children())
                        {
                            wf.WriteLine($"{id}\tData/MasterParam\tTrophy[?(@.iname=='{a["iname"]}')].name\t{a["name"]}");
                        }

                        foreach (var a in js["Unit"].Children())
                        {
                            wf.WriteLine($"{id}\tData/MasterParam\tUnit[?(@.iname=='{a["iname"]}')].name\t{a["name"]}");
                            wf.WriteLine($"{id}\tData/MasterParam\tUnit[?(@.iname=='{a["iname"]}')].tag\t{a["tag"]}");
                            wf.WriteLine($"{id}\tData/MasterParam\tUnit[?(@.iname=='{a["iname"]}')].birth\t{a["birth"]}");
                        }

                        foreach (var a in js["UnitGroup"].Children())
                        {
                            wf.WriteLine($"{id}\tData/MasterParam\tUnitGroup[?(@.iname=='{a["iname"]}')].name\t{a["name"]}");
                        }

                        //foreach (var a in js["Weather"].Children())
                        //{
                        //    wf.WriteLine($"{id}\tData/MasterParam\tWeather[?(@.iname=='{a["iname"]}')].name\t{a["name"]}");
                        //    wf.WriteLine($"{id}\tData/MasterParam\tWeather[?(@.iname=='{a["iname"]}')].expr\t{a["expr"]}");
                        //}
                    }
                }
            }

            QuestList("GFMasterParam.txt", "JPMasterParam.txt", "JPMasterResult.txt");
            QuestList("GFQuestParam.txt","JPQuestParam.txt","JPQuestResult.txt");
            void QuestList(string gffile, string jpfile, string resultfile)
            {
                List<CBItem> GFCB = new List<CBItem>();
                using (var sReader = new StreamReader(new FileStream(gffile, FileMode.Open), Encoding.UTF8))
                {
                    while (!sReader.EndOfStream)
                    {
                        var res = sReader.ReadLine();
                        var a = res.Split('\t');
                        if (!string.IsNullOrWhiteSpace(a[3]))
                        {
                            GFCB.Add(new CBItem {IDstr = a[0], Path = a[1], ID = a[2], Chinese = a[3]});
                        }
                    }
                }

                using (var sReader = new StreamReader(new FileStream(jpfile, FileMode.Open), Encoding.UTF8))
                {
                    using (var result = new StreamWriter(new FileStream(resultfile, FileMode.Create, FileAccess.Write), Encoding.UTF8))
                    {
                        while (!sReader.EndOfStream)
                        {
                            var res = sReader.ReadLine();
                            var a = res.Split('\t');

                            if (!string.IsNullOrWhiteSpace(a[3]))
                            {
                                var h = GFCB.Where(i => i.Path == a[1] && i.ID == a[2]);
                                result.WriteLine($"{res}\t{(h.Any() ? h.First().Chinese : "")}");
                            }
                        }
                    }
                }
            }

            //JsonHan();
            void JsonHan()
            {
                List<CBItem> cb = new List<CBItem>();
                using (var sReader = new StreamReader(new FileStream("JSONWord.txt", FileMode.Open), Encoding.UTF8))
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
                    if (File.Exists("JPData/" + file))
                    {
                        var fcb = cb.Where(i => i.IDstr == file);
                        using (var sReader = new StreamReader(new FileStream(Path.Combine("JPData/", file), FileMode.Open), Encoding.UTF8))
                        {
                            var json = JToken.Parse(sReader.ReadToEnd());

                            foreach (var cbi in fcb)
                            {
                                var s = json.SelectToken(cbi.ID);
                                s.Replace(cbi.Chinese);
                            }

                            var sd = json.ToString(Formatting.None);

                            byte[] byteArray = Encoding.UTF8.GetBytes(sd);
                            MemoryStream stream = new MemoryStream(byteArray);
                            using (var raw = File.Create("temp.txt"))
                            {
                                byte[] buffer = new byte[4096];
                                int n;
                                while ((n = stream.Read(buffer, 0, buffer.Length)) != 0)
                                {
                                    raw.Write(buffer, 0, n);
                                }
                            }
                        }

                        Console.WriteLine(file);
                        File.Copy("temp.txt", Path.Combine("JPLOC/", file), true);
                    }
                }
            }
        }

        public static void GetLoc(string dir, string filename, string url, List<Item> collection)
        {
            Directory.CreateDirectory(dir);
            Parallel.ForEach(collection.Where(i => i.Path.StartsWith("Loc/")), (item) =>
            {
                GetFileAsync($"{url}{item.IDStr}", dir + item.IDStr);
                Console.WriteLine(item.IDStr);
            });

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
