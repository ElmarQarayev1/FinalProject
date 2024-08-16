using System;
using Microsoft.AspNetCore.Http;

namespace Medical.Service.Helpers
{
    public class FileManagerWrapper : IFileManager
    {
        public string Save(IFormFile file, string root, string folder)
        {
            return FileManager.Save(file, root, folder); 
        }

        public bool Delete(string root, string folder, string fileName)
        {
            return FileManager.Delete(root, folder, fileName);
        }
    }
}

