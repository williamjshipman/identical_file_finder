using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IdenticalFileFinder.Models
{
    internal class DirectoryModel
    {
        public string AbsolutePath { get; private set; }
        public string Path { get; set; }
        public List<FileModel> Files { get; set; }
        public List<DirectoryModel> SubDirectories { get; set; }

        public DirectoryModel? Parent { get; set; }

        private void InitThisClass(string path, DirectoryModel? parent)
        {
            Path = path;
            Parent = parent;

            if (parent != null)
            {
                AbsolutePath = System.IO.Path.Combine(parent.AbsolutePath, path);
            }
            else
            {
                AbsolutePath = System.IO.Path.GetFullPath(path);
            }

            Files = new List<FileModel>();
            SubDirectories = new List<DirectoryModel>();
        }

        public DirectoryModel(string path, DirectoryModel parent)
        {
            InitThisClass(path, parent);
        }

        public DirectoryModel(string path)
        {
            InitThisClass(path, null);
        }
    }
}
