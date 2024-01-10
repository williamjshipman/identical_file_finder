using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdenticalFileFinder.Services
{
    public interface IFolderService
    {
        public Task<IStorageFolder?> OpenFolderAsync();
    }
}
