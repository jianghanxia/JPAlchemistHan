using System;
using System.Collections.Generic;
using System.Linq;
using AlchemistHan.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AlchemistHan.Droid.System_Android))]
namespace AlchemistHan.Droid
{
    public class System_Android : ISystem
    {
        public void CloseApp()
        {
            //MessagingCenter.Send(new ServiceMessage("Stop"), "Service");
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }

        public string GetLocalFilePath()
        {
            return $"{Android.OS.Environment.ExternalStorageDirectory.AbsolutePath}/Android/data/jp.co.gu3.alchemist/files/new_aatc/";
        }

        public string GetPersonalPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }
    }
}