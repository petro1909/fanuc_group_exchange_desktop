using fanuc_group_exchange_desktop.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text;

namespace fanuc_group_exchange_desktop.Services
{
    public class FileService
    {
        public string ReadAllText(string filePath)
        {
            string fileStrings = File.ReadAllText(filePath);
            return fileStrings;
        }

        public string GetFileName(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            return file.Name;
        }

        public void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        } 

        public void WriteToFile(string path, string code, bool isOverride = true)
        {
            using (StreamWriter streamWriter = new StreamWriter(path, !isOverride))
            {
                streamWriter.WriteLine(code);
            }
        }
    }
}