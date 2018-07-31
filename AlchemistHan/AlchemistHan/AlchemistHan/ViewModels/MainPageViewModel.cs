using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AlchemistHan.Services;
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

        public DelegateCommand DownloadCommand { get; }

        public MainPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
	    {
	        NavigationService = navigationService;
	        PageDialogService = pageDialogService;

	        DownloadCommand = new DelegateCommand(OnResourcesCommandExecuted);
        }

	    private void OnResourcesCommandExecuted()
        {
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    IsBusy = false;
                    IsDownload = true;
                    DownloadProgress = 0;

                    var path = DependencyService.Get<ISystem>().GetLocalFilePath();

                    var ck = await GetWebAsync("https://jianghanxia.gitee.io/jpalchemisthan/list.txt", "");
                    var list = ck.Split('\n');
                    var nc = list.Length;
                    int i = 0;


                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    foreach (var file in list)
                    {
                        var name = Path.Combine(path, file);

                        using (var wclient = new WebClient())
                        {
                            wclient.BaseAddress = "https://jianghanxia.gitee.io/jpalchemisthan/";
                            wclient.DownloadFile(file, name);
                        }

                        DownloadProgress = i / (float)nc;
                        i += 1;
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

        public async Task<string> GetWebAsync(string url, string action, string method = "Get", string postData = "")
        {
            var u = $"{url}{action}";
            var request = WebRequest.CreateHttp(u);

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

            var response = await request.GetResponseAsync();
            var steam = response.GetResponseStream();
            var sReader = new StreamReader(steam, Encoding.GetEncoding("UTF-8"));
            var res = sReader.ReadToEnd();
            response.Close();

            return res;
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
