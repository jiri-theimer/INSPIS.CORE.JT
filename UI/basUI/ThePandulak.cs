using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace UI
{
    public class ThePandulak
    {
        private string _FolderFullPath;
        public ThePandulak(string strFolderFullPath)
        {
            _FolderFullPath = strFolderFullPath;
        }
        public string getPandulakImage(int intPokus)
        {
            var files=System.IO.Directory.GetFiles(_FolderFullPath);

            var r = new Random();
            var x = r.Next(1,files.Count());
            return files[x - 1].Split("\\").Last();

        }
    }
}
