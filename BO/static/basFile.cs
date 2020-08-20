using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;


namespace BO
{
    public class BASFILE
    {
        public static FileInfo GetFileInfo(string strFullPath)
        {
            return new FileInfo(strFullPath);
        }

        public static void SaveStream2File(String strDestFullPath, Stream inputStream)
        {
            
            using (FileStream outputFileStream = new FileStream(strDestFullPath, FileMode.Create))
            {
                inputStream.CopyTo(outputFileStream);
            }

            
        }

        public static void AppendText2File(String strDestFullPath, string s)
        {
            File.AppendAllText(strDestFullPath, s);            
        }

        static List<string> GetFileNamesInDir(string strDir, string strPattern,bool getFullPath)
        {
            DirectoryInfo dir = new DirectoryInfo(strDir);
            var lis = new List<string>();
            foreach (FileInfo file in dir.GetFiles(strPattern))
            {
                if (getFullPath == true)
                {
                    lis.Add(file.FullName);
                }
                else
                {
                    lis.Add(file.Name);
                }
                

            }

            return lis;
        }

        

        
        
    }
}
