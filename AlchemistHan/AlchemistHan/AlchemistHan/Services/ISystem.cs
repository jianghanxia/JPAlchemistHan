using System;
using System.Collections.Generic;
using System.Text;

namespace AlchemistHan.Services
{
    public interface ISystem
    {
        void CloseApp();

        string GetLocalFilePath();

        string GetPersonalPath();
    }
}
