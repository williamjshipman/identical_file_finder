using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IdenticalFileFinder.Models
{
    public class FileModel
    {
        public string Path { get; set; }
        public string AbsolutePath { get; private set; }
        public string? Hash { get; set; }
        public DirectoryModel? Parent { get; set; }

        private void InitThisClass(string path, DirectoryModel? parent)
        {
            Path = path;
            Parent = parent;

            if (Parent != null)
            {
                if (Path.StartsWith(Parent.AbsolutePath))
                {
                    Path = Path[(Parent.AbsolutePath.Length+1)..];
                }
                AbsolutePath = System.IO.Path.Combine(Parent.AbsolutePath, path);
            }
            else
            {
                AbsolutePath = System.IO.Path.GetFullPath(path);
                Path = System.IO.Path.GetFileName(path);
            }
        }

        public FileModel(string path, DirectoryModel parent)
        {
            InitThisClass(path, parent);
        }

        public void CalculateHash()
        {
            SHA512 sha512 = SHA512.Create();
            using FileStream stream = File.OpenRead(AbsolutePath);
            byte[] pbHash = sha512.ComputeHash(stream);
            Hash = BitConverter.ToString(pbHash).Replace("-", "");
        }

        public async Task CalculateHashAsync()
        {
            await Task.Run(() => CalculateHash());
        }
    }
}
