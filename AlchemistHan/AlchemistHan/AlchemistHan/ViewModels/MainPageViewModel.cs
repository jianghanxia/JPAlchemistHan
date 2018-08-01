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
using Prism.Navigation;
using Prism.Services;
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
        public DelegateCommand HHCommand { get; }

        public MainPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
        {
            NavigationService = navigationService;
            PageDialogService = pageDialogService;

            DownloadDataCommand = new DelegateCommand(async () =>
            {
                IsBusy = false;
                await GetFileAsync("https://jianghanxia.gitee.io/jpalchemisthan/JPWord.txt", Path.Combine(DependencyService.Get<ISystem>().GetPersonalPath(), "JPWord.txt"));
                await PageDialogService.DisplayAlertAsync("完成", "完成数据下载", "OK");
                IsBusy = true;
            });

            HHCommand = new DelegateCommand(OnHHCommandExecuted);
        }

        private void OnHHCommandExecuted()
        {
            IsBusy = false;
            IsDownload = true;
            DownloadProgress = 0;

            Task.Factory.StartNew(async () =>
            {
                try
                {
                    if (!File.Exists(Path.Combine(DependencyService.Get<ISystem>().GetPersonalPath(), "JPWord.txt")))
                    {
                        await PageDialogService.DisplayAlertAsync("错误", "先下载数据", "OK");
                        return;
                    }

                    List<CBItem> cb = new List<CBItem>();
                    using (var sReader = new StreamReader(new FileStream(Path.Combine(DependencyService.Get<ISystem>().GetPersonalPath(), "JPWord.txt"), FileMode.Open), Encoding.UTF8))
                    {
                        while (!sReader.EndOfStream)
                        {
                            var res = sReader.ReadLine();
                            var a = res.Split('\t');
                            cb.Add(new CBItem {IDstr = a[0], ID = a[1], Chinese = a[2]});
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
                            using (var sReader = new StreamReader(new FileStream(Path.Combine(path, file), FileMode.Open), Encoding.UTF8))
                            {
                                using (var f = File.OpenWrite(Path.Combine(DependencyService.Get<ISystem>().GetPersonalPath(), "temp")))
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
                        }

                        DownloadProgress = ii / (float) nc;
                        ii += 1;
                    }

                    IsDownload = false;
                    IsBusy = true;
                    Message = "完成汉化";

                    await PageDialogService.DisplayAlertAsync("完成", "完成汉化", "OK");
                }
                catch (Exception ee)
                {
                    Message = ee.Message;
                    IsDownload = false;
                    IsBusy = true;

                    await PageDialogService.DisplayAlertAsync("错误", ee.Message, "OK");
                }
            });
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
                using (Stream output = File.OpenWrite(filename))
                {
                    steam.CopyTo(output);
                }
            }

            response.Close();
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
