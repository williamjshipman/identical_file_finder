using System.Collections.ObjectModel;

namespace IdenticalFileFinder.Models
{
    public class TreeViewNode
    {
        public required string Name { get; set; }
        public required string Path { get; set; }
        public FileModel? File { get; set; }
        public ObservableCollection<TreeViewNode> Children { get; set; } = [];
    }
}