using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using IdenticalFileFinder.Models;
using IdenticalFileFinder.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace IdenticalFileFinder.ViewModels;

public class MainViewModel : ViewModelBase
{
    #region Fields

    private AvaloniaList<string> m_DuplicateFileNames = [];

    #endregion

    #region Properties

    public List<DirectoryModel> SourceDirectories { get; set; } = [];
    public AvaloniaList<string> SourceDirectoryNames
    {
        get
        {
            AvaloniaList<string> names = new();
            foreach (DirectoryModel directory in SourceDirectories)
            {
                names.Add(directory.Path);
            }
            return names;
        }
    }

    public AvaloniaList<string> DuplicateFileNames
    {
        get
        {
            return m_DuplicateFileNames;
        }
        set
        {
            m_DuplicateFileNames = value;
            this.RaisePropertyChanged(nameof(DuplicateFileNames));
        }
    }

    #endregion

    #region Commands

    public ReactiveCommand<Unit, Unit> HandleFileOpenClickCommand { get; }
    public ReactiveCommand<Unit, Unit> ExitCommand { get; } = ReactiveCommand.Create(() => Environment.Exit(0));
    public ReactiveCommand<Unit, Unit> FileFindDuplicatesCommand { get; }

    #endregion

    #region Command Handler Methods

    private async Task HandleFileOpenClick()
    {
        var fileService = App.Current?.Services?.GetService(typeof(IFolderService)) as IFolderService;

        if (fileService != null)
        {
            IStorageFolder? folder = await fileService.OpenFolderAsync();
            if (folder != null)
            {
                DirectoryModel directory = new(folder.Path);
                await directory.PopulateRecursiveAsync();
                SourceDirectories.Add(directory);
                this.RaisePropertyChanged(nameof(SourceDirectoryNames));
            }
        }
    }

    private void AddFileToDictionary(Dictionary<string, List<FileModel>> hashToFile, FileModel file)
    {
        if (file.Hash == null)
        {
            throw new ArgumentException("File hash is null.");
        }
        if (hashToFile.TryGetValue(file.Hash, out List<FileModel>? value))
        {
            value.Add(file);
        }
        else
        {
            hashToFile.Add(file.Hash, new List<FileModel>() { file });
        }
    }

    private void AddFolderToDictionary(Dictionary<string, List<FileModel>> hashToFile, DirectoryModel directory)
    {
        foreach (DirectoryModel subDirectory in directory.SubDirectories)
        {
            AddFolderToDictionary(hashToFile, subDirectory);
        }
        foreach (FileModel file in directory.Files)
        {
            AddFileToDictionary(hashToFile, file);
        }
    }

    private async Task FileFindDuplicates()
    {
        // Calculate hashes for all files.
        List<Task> tasks = new List<Task>();
        foreach (DirectoryModel directory in SourceDirectories)
        {
            tasks.Add(directory.CalculateHashesRecursiveAsync());
        }
        await Task.WhenAll(tasks);

        // Create a dictionary of hashes to files. This will be used to find duplicates.
        Dictionary<string, List<FileModel>> hashToFile = new();
        foreach (DirectoryModel directory in SourceDirectories)
        {
            AddFolderToDictionary(hashToFile, directory);
        }

        // Find duplicates.
        List<List<FileModel>> duplicates = 
            hashToFile.Where((item) => item.Value.Count > 1)
                .Select((item) => item.Value).ToList();
        AvaloniaList<string> duplicateFileNames = DuplicateFileNames;
        duplicateFileNames.Clear();
        foreach (List<FileModel> duplicate in duplicates)
        {
            string sPrefix = " ↳ ";
            foreach (FileModel file in duplicate)
            {
                duplicateFileNames.Add(sPrefix + file.AbsolutePath);
                sPrefix = " " + sPrefix;
            }
        }
        DuplicateFileNames = duplicateFileNames;
    }

    #endregion

    public MainViewModel()
    {
        HandleFileOpenClickCommand = ReactiveCommand.CreateFromTask(HandleFileOpenClick);
        FileFindDuplicatesCommand = ReactiveCommand.CreateFromTask(FileFindDuplicates);
    }
}
