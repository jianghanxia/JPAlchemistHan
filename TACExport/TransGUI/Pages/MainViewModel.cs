using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Stylet;
using TransGUI.Model;

namespace TransGUI.Pages
{
    public class MainViewModel : Screen
    {
        public ObservableCollection<TacTrans> List { get; set; } = new ObservableCollection<TacTrans>();

        public MainViewModel()
        {
            using (var udb = new LiteDatabase(@"MyData.db"))
            {
                var ucol = udb.GetCollection<TacTrans>();
                List = new ObservableCollection<TacTrans>(ucol.FindAll());
            }
        }

        public void Read()
        {
            
        }
    }
}
