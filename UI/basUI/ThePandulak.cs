using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace UI
{
    public class ThePandulak
    {
        private IWebHostEnvironment _hostingEnvironment;
        public ThePandulak(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public string getPandulakImage(int intPokus)
        {
            var files=System.IO.Directory.GetFiles(_hostingEnvironment.WebRootPath + "\\images\\pandulak");

            var r = new Random();
            var x = r.Next(1,files.Count());
            return files[x - 1].Split("\\").Last();

        }
    }
}
