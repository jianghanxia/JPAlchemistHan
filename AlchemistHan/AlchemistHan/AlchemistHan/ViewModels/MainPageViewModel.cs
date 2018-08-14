using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AlchemistHan.Models;
using AlchemistHan.Services;
using Ionic.Zlib;
using Newtonsoft.Json.Linq;
using Prism.Navigation;
using Prism.Services;
using ExcelDataReader;
using Newtonsoft.Json;
using DependencyService = Xamarin.Forms.DependencyService;

namespace AlchemistHan.ViewModels
{
    public class MainPageViewModel : BindableBase, INavigationAware
    {
        private INavigationService NavigationService { get; }
        private IPageDialogService PageDialogService { get; }

        public bool IsBusy { get; set; } = true;
        public bool IsDownload { get; set; }
        public float DownloadProgress { get; set; }
        public string Message { get; set; }

        public DelegateCommand DownloadDataCommand { get; }
        public DelegateCommand DownloadSYFontCommand { get; }
        public DelegateCommand DownloadWRFontCommand { get; }
        public DelegateCommand HHCommand { get; }
        public DelegateCommand JsonCommand { get; }
        public DelegateCommand RestoreHanCommand { get; }
        public DelegateCommand RestoreJsonCommand { get; }

        public MainPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
        {
            NavigationService = navigationService;
            PageDialogService = pageDialogService;

            DownloadDataCommand = new DelegateCommand(async () =>
            {
                try
                {
                    Message = "";
                    IsBusy = false;
                    await GetFileAsync("https://jianghanxia.gitee.io/jpalchemisthan/JPResult.xlsx", Path.Combine(DependencyService.Get<ISystem>().GetPersonalPath(), "JPResult.xlsx"));
                    await GetFileAsync("https://jianghanxia.gitee.io/jpalchemisthan/JSONWord.gz", Path.Combine(DependencyService.Get<ISystem>().GetPersonalPath(), "JSONWord.gz"));
                    await PageDialogService.DisplayAlertAsync("完成", "完成数据下载", "OK");
                    IsBusy = true;
                }
                catch (Exception ee)
                {
                    Message = ee.Message;
                    IsBusy = true;
                }
            });

            DownloadSYFontCommand = new DelegateCommand(async () =>
            {
                try
                {
                    Message = "";
                    IsBusy = false;
                    await GetFileAsync("https://jianghanxia.gitee.io/jpalchemisthan/SY", Path.Combine(DependencyService.Get<ISystem>().GetLocalFilePath(), "0c9a8047"));
                    await PageDialogService.DisplayAlertAsync("完成", "完成字体下载", "OK");
                    IsBusy = true;
                }
                catch (Exception ee)
                {
                    Message = ee.Message;
                    IsBusy = true;
                }
            });
            DownloadWRFontCommand = new DelegateCommand(async () =>
            {
                try
                {
                    Message = "";
                    IsBusy = false;
                    await GetFileAsync("https://jianghanxia.gitee.io/jpalchemisthan/WR", Path.Combine(DependencyService.Get<ISystem>().GetLocalFilePath(), "0c9a8047"));
                    await PageDialogService.DisplayAlertAsync("完成", "完成字体下载", "OK");
                    IsBusy = true;
                }
                catch (Exception ee)
                {
                    Message = ee.Message;
                    IsBusy = true;
                }
            });

            HHCommand = new DelegateCommand(OnHHCommandExecuted);
            JsonCommand = new DelegateCommand(OnJsonCommandExecuted);
            RestoreHanCommand = new DelegateCommand(OnRestoreHanCommandExecuted);
            RestoreJsonCommand = new DelegateCommand(OnRestoreJsonCommandExecuted);
        }

        private void OnRestoreHanCommandExecuted()
        {
            Message = "";
            IsBusy = false;
            IsDownload = true;
            DownloadProgress = 0;

            var code = GetWeb("https://jianghanxia.gitee.io/jpalchemisthan/ver");
            var ver = GetWeb("https://alchemist.gu3.jp/chkver2", "Post", $"ver={code}");
            var verj = JToken.Parse(ver);

            Task.Factory.StartNew(async () =>
            {
                try
                {
                    await GetFileAsync($"https://alchemist-dlc2.gu3.jp/assets/{verj.SelectToken("body.environments.alchemist.assets")}/aatc/ASSETLIST",
                        Path.Combine(DependencyService.Get<ISystem>().GetPersonalPath(), "ASSETLIST"));

                    var collection = GetCollection(Path.Combine(DependencyService.Get<ISystem>().GetPersonalPath(), "ASSETLIST"));

                    var path = DependencyService.Get<ISystem>().GetLocalFilePath();
                    var cloc = collection.Where(i => i.Path.StartsWith("Loc/"));
                    var nc = cloc.Count();
                    int ii = 0;
                    foreach (var item in cloc)
                    {
                        await GetFileAsync($"https://alchemist-dlc2.gu3.jp/assets/{verj.SelectToken("body.environments.alchemist.assets")}/aatc/{item.IDStr}",
                            Path.Combine(path, item.IDStr));
                        DownloadProgress = ii / (float)nc;
                        ii += 1;
                    }

                    IsDownload = false;
                    IsBusy = true;
                    Message = "还原文本完成";
                }
                catch (Exception ee)
                {
                    Message = ee.Message;
                    IsDownload = false;
                    IsBusy = true;
                }
            });
        }

        private void OnRestoreJsonCommandExecuted()
        {
            Message = "";
            IsBusy = false;

            var code = GetWeb("https://jianghanxia.gitee.io/jpalchemisthan/ver");
            var ver = GetWeb("https://alchemist.gu3.jp/chkver2", "Post", $"ver={code}");
            var verj = JToken.Parse(ver);

            Task.Factory.StartNew(async () =>
            {
                try
                {
                    var path = DependencyService.Get<ISystem>().GetLocalFilePath();

                    await GetFileAsync($"https://alchemist-dlc2.gu3.jp/assets/{verj.SelectToken("body.environments.alchemist.assets")}/aatc/b9cc206f",
                        Path.Combine(path, "b9cc206f"));
                    await GetFileAsync($"https://alchemist-dlc2.gu3.jp/assets/{verj.SelectToken("body.environments.alchemist.assets")}/aatc/49744fd6",
                        Path.Combine(path, "49744fd6"));

                    IsBusy = true;
                    Message = "还原数据完成";
                }
                catch (Exception ee)
                {
                    Message = ee.Message;
                    IsBusy = true;
                }
            });
        }

        private void OnJsonCommandExecuted()
        {
            Message = "";
            IsBusy = false;
            IsDownload = true;
            DownloadProgress = 0;

            if (!File.Exists(Path.Combine(DependencyService.Get<ISystem>().GetPersonalPath(), "JSONWord.gz")))
            {
                Message = "先下载数据";
                return;
            }

            Task.Factory.StartNew(() =>
            {
                try
                {
                    List<CBItem> cb = new List<CBItem>();
                    using (var sReader =
                        new StreamReader(
                            new GZipStream(File.Open(Path.Combine(DependencyService.Get<ISystem>().GetPersonalPath(), "JSONWord.gz"), FileMode.Open), CompressionMode.Decompress),
                            Encoding.UTF8))
                    {
                        while (!sReader.EndOfStream)
                        {
                            var res = sReader.ReadLine();
                            var a = res.Split('\t');
                            cb.Add(new CBItem {IDstr = a[0], ID = a[1], Chinese = a[2]});
                        }
                    }

                    var fl = cb.Select(i => i.IDstr).Distinct();

                    var path = DependencyService.Get<ISystem>().GetLocalFilePath();
                    foreach (var file in fl)
                    {
                        if (File.Exists(Path.Combine(path, file)))
                        {
                            var fcb = cb.Where(i => i.IDstr == file);
                            var nc = fcb.Count();
                            int ii = 0;

                            using (var sReader = new StreamReader(new ZlibStream(new FileStream(Path.Combine(path, file), FileMode.Open), CompressionMode.Decompress), Encoding.UTF8))
                            {
                                var json = JToken.Parse(sReader.ReadToEnd());

                                foreach (var cbi in fcb)
                                {
                                    var s = json.SelectToken(cbi.ID);
                                    s?.Replace(cbi.Chinese);

                                    DownloadProgress = ii / (float)nc;
                                    ii += 1;
                                }

                                var sd = json.ToString(Formatting.None);

                                byte[] byteArray = Encoding.UTF8.GetBytes(sd);
                                MemoryStream stream = new MemoryStream(byteArray);
                                using (var f = File.Open(Path.Combine(DependencyService.Get<ISystem>().GetPersonalPath(), "temp"), FileMode.Create))
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

                            File.Copy(Path.Combine(DependencyService.Get<ISystem>().GetPersonalPath(), "temp"), Path.Combine(path, file), true);
                            File.Delete(Path.Combine(DependencyService.Get<ISystem>().GetPersonalPath(), "temp"));
                        }
                    }

                    IsDownload = false;
                    IsBusy = true;
                    Message = "完成数据汉化";
                }
                catch (Exception ee)
                {
                    Message = ee.Message;
                    IsDownload = false;
                    IsBusy = true;
                }
            });
        }

        private void OnHHCommandExecuted()
        {
            Message = "";
            IsBusy = false;
            IsDownload = true;
            DownloadProgress = 0;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (!File.Exists(Path.Combine(DependencyService.Get<ISystem>().GetPersonalPath(), "JPResult.xlsx")))
                    {
                        Message = "先下载数据";
                        return;
                    }

                    List<CBItem> cb = new List<CBItem>();
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    using (var stream = File.Open(Path.Combine(DependencyService.Get<ISystem>().GetPersonalPath(), "JPResult.xlsx"), FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            do
                            {
                                while (reader.Read())
                                {
                                    if (!string.IsNullOrWhiteSpace(reader.GetString(4)))
                                    {
                                        cb.Add(new CBItem {IDstr = reader.GetString(0), ID = reader.GetString(2), Chinese = reader.GetString(4)});
                                    }
                                }
                            } while (reader.NextResult());
                        }
                    }

                    var fl = cb.Select(i => i.IDstr).Distinct();
                    var nc = fl.Count();
                    int ii = 0;

                    var path = DependencyService.Get<ISystem>().GetLocalFilePath();
                    foreach (var file in fl)
                    {
                        if (File.Exists(Path.Combine(path, file)))
                        {
                            var fcb = cb.Where(i => i.IDstr == file);
                            using (var sReader = new StreamReader(new ZlibStream(new FileStream(Path.Combine(path, file), FileMode.Open), CompressionMode.Decompress),
                                Encoding.UTF8))
                            {
                                using (var f = File.Open(Path.Combine(DependencyService.Get<ISystem>().GetPersonalPath(), "temp"), FileMode.Create))
                                {
                                    using (var result = new StreamWriter(new ZlibStream(f, CompressionMode.Compress, CompressionLevel.BestCompression), Encoding.UTF8))
                                    {
                                        while (!sReader.EndOfStream)
                                        {
                                            var res = sReader.ReadLine();

                                            var a = res.Split(new[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);
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

                            File.Copy(Path.Combine(DependencyService.Get<ISystem>().GetPersonalPath(), "temp"), Path.Combine(path, file), true);
                            File.Delete(Path.Combine(DependencyService.Get<ISystem>().GetPersonalPath(), "temp"));
                        }

                        DownloadProgress = ii / (float) nc;
                        ii += 1;
                    }

                    IsDownload = false;
                    IsBusy = true;
                    Message = "完成文本汉化";
                }
                catch (Exception ee)
                {
                    Message = ee.Message;
                    IsDownload = false;
                    IsBusy = true;
                }
            });
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

                return collection;
            }
        }

        public static async Task GetFileAsync(string url, string filename)
        {
            var request = WebRequest.CreateHttp(url);

            request.Method = "GET";
            request.Accept = "identity";
            request.UserAgent = "Dalvik/2.1.0 (Linux; U; Android 6.0; R11/MRA58K)";

            var response = await request.GetResponseAsync();

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

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
        }
    }
}
