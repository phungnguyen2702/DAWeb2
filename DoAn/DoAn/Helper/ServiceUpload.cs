using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using DoAn.Controllers;

namespace DoAn.Helper
{
    public class ServiceUpload
    {
        public static void UploadImage(string spDirPath, int pro, 
            HttpPostedFileBase fumain, HttpPostedFileBase futhumsmain)
        { 
            if (fumain != null && fumain.ContentLength >= 0)
            {                 
                string targetDirPath = Path.Combine(spDirPath, pro.ToString());
                Directory.CreateDirectory(targetDirPath);
                string mainFile = Path.Combine(targetDirPath, "main.jpg");

                fumain.SaveAs(mainFile);
            }
            if (futhumsmain != null && futhumsmain.ContentLength >= 0)
            {
                string targetDirPath = Path.Combine(spDirPath, pro.ToString());

                Directory.CreateDirectory(targetDirPath);
                string thumbsFile = Path.Combine(targetDirPath, "main_thumbs.jpg");
                
                futhumsmain.SaveAs(thumbsFile);
            }
        }

        public static void Remove(string spDirPath, int pro)
        {
            string targetDirPath = Path.Combine(spDirPath, pro.ToString());
            if(Directory.Exists(targetDirPath))
                Directory.Delete(targetDirPath,true);               
        }
    }
}