using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransGUI.Model
{
    public class TacTrans
    {
        public int id { get; set; }

        public string File { get; set; }

        public string IDStr { get; set; }
        public string Path { get; set; }

        public string JP { get; set; }

        public string CN { get; set; }
        public string Trans { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
