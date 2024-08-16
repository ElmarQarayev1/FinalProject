using System;
using Microsoft.AspNetCore.Http;

namespace Medical.Service.Helpers
{
    public interface IFileManager
    {
        string Save(IFormFile file, string root, string folder);
        bool Delete(string root, string folder, string fileName);
    }

}

