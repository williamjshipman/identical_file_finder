using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IdenticalFileFinder.Models
{
    public class DirectoryModel
    {
        public delegate void DirectoryProgressDelegate(DirectoryModel directory, string action, string stage);
        public event DirectoryProgressDelegate? DirectoryProgressEvent;

        public string AbsolutePath { get; private set; }
        public string Path { get; set; }
        public List<FileModel> Files { get; set; }
        public List<DirectoryModel> SubDirectories { get; set; }

        public DirectoryModel? Parent { get; set; }

        private void InitThisClass(string path, DirectoryModel? parent, DirectoryProgressDelegate progressDelegate)
        {
            Path = path;
            Parent = parent;
            DirectoryProgressEvent += progressDelegate;

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

        public DirectoryModel(string path, DirectoryModel parent, DirectoryProgressDelegate progressDelegate)
        {
            InitThisClass(path, parent, progressDelegate);
        }

        public DirectoryModel(string path, DirectoryProgressDelegate progressDelegate)
        {
            InitThisClass(path, null, progressDelegate);
        }

        public DirectoryModel(Uri uri, DirectoryProgressDelegate progressDelegate)
        {
            InitThisClass(uri.LocalPath, null, progressDelegate);
        }

        public void Populate()
        {
            DirectoryProgressEvent?.Invoke(this, "Listing files in", "Populate directory structure");
            string[] files = Directory.GetFiles(AbsolutePath);
            foreach (string file in files)
            {
                Files.Add(new FileModel(file, this));
            }

            string[] directories = Directory.GetDirectories(AbsolutePath);
            foreach (string directory in directories)
            {
                SubDirectories.Add(new DirectoryModel(directory, this, DirectoryProgressEvent));
            }
        }

        public void PopulateRecursive()
        {
            Populate();

            foreach (DirectoryModel directory in SubDirectories)
            {
                directory.PopulateRecursive();
            }
        }

        public async Task PopulateAsync()
        {
            await Task.Run(() => Populate());
        }

        public async Task PopulateRecursiveAsync()
        {
            await Task.Run(() => PopulateRecursive());
        }

        public void CalculateHashes()
        {
            DirectoryProgressEvent?.Invoke(this, "Hashing files in", "Calculate file hashes");
            foreach (FileModel file in Files)
            {
                file.CalculateHash();
            }
        }

        public async Task CalculateHashesAsync()
        {
            await Task.Run(() => CalculateHashes());
        }

        public void CalculateHashesRecursive()
        {
            CalculateHashes();

            foreach (DirectoryModel directory in SubDirectories)
            {
                directory.CalculateHashesRecursive();
            }
        }

        public async Task CalculateHashesRecursiveAsync()
        {
            await Task.Run(() => CalculateHashesRecursive());
        }
    }
}
