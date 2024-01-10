using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdenticalFileFinder.Services
{
    public class FolderService : IFolderService
    {
        private readonly TopLevel _target;

        public FolderService(TopLevel target)
        {
            _target = target;
        }

        public async Task<IStorageFolder?> OpenFolderAsync()
        {
            var folders = await _target.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                AllowMultiple = false,
                SuggestedStartLocation = await _target.StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Documents),
                Title = "Select a folder to scan"
            });

            return (folders.Count > 0) ? folders[0] : null;
        }
    }
}
