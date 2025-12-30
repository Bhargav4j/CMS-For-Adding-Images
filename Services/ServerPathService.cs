using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Services
{
    public class ServerPathService : IPathService
    {
        private readonly string _contentRoot;

        public ServerPathService(string contentRoot = null)
        {
            _contentRoot = contentRoot ?? Directory.GetCurrentDirectory();
        }

        public string MapPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return _contentRoot;

            // Remove leading ~ or / characters
            path = path.TrimStart('~', '/');

            // Combine with content root
            return Path.Combine(_contentRoot, path);
        }
    }
}
