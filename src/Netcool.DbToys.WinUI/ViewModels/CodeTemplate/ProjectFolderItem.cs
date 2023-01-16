﻿using Windows.Storage;
using Windows.Storage.FileProperties;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Netcool.DbToys.WinUI.Helpers;
using Netcool.DbToys.WinUI.Services;

namespace Netcool.DbToys.WinUI.ViewModels.CodeTemplate;

public record RenamedArgs(string OldName, string NewName, string OldPath, string NewPath);

public record ProjectDeletedArg(string Name, string Path);

public class ProjectFolderItem : TreeItem
{
    private readonly Lazy<INotificationService> _notificationService = new(App.GetService<INotificationService>);

    private ImageSource _icon;
    public ImageSource Icon { get => _icon; set => SetProperty(ref _icon, value); }

    public StorageFolder Folder { get; set; }

    public Action<RenamedArgs> RenamedAction { get; set; }
    public Action<ProjectDeletedArg> DeletedAction { get; set; }

    public IAsyncRelayCommand CreateTemplateCommand { get; set; }

    public IAsyncRelayCommand RenameCommand { get; set; }

    public ProjectFolderItem(StorageFolder folder, bool lazyLoadChildren) : base(folder.Name, lazyLoadChildren)
    {
        Folder = folder;
        CreateTemplateCommand = new AsyncRelayCommand(CreateTemplateFile);
        RenameCommand = new AsyncRelayCommand(RenameAsync);
        LoadIcon();
    }

    private async Task RenameAsync()
    {
        var oldName = Folder.Name;
        var oldPath = Folder.Path;
        var dialog = DynamicDialogFactory.GetFor_RenameDialog(Folder.Name);
        await dialog.ShowAsync();
        string newName;
        if (dialog.ViewModel.DialogResult == ContentDialogResult.Primary)
        {
            newName = (string)dialog.ViewModel.AdditionalData;
        }
        else return;

        if (string.IsNullOrEmpty(newName) || oldName == newName) return;
        try
        {
            await Folder.RenameAsync(newName);
        }
        catch (Exception ex)
        {
            _notificationService.Value.Error($"Rename project folder [] failed with error: {ex.Message}",
                Constants.Notification.ShortErrorDuration);
            return;
        }
        RenamedAction?.Invoke(new RenamedArgs(oldName, newName, oldPath, Folder.Path));
    }

    private async Task CreateTemplateFile()
    {
        var dialog = DynamicDialogFactory.GetFor_CreateTemplateDialog();
        await dialog.ShowAsync();
        string fileName;
        if (dialog.ViewModel.DialogResult == ContentDialogResult.Primary)
        {
            fileName = (string)dialog.ViewModel.AdditionalData + ".tpl";
        }
        else return;

        if (string.IsNullOrEmpty(fileName)) return;

        StorageFile file;
        try
        {
            file = await Folder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
        }
        catch (Exception ex)
        {
            _notificationService.Value.Error($"Create file failed with error: {ex.Message}",
                Constants.Notification.ShortErrorDuration);
            return;
        }
        var vm = new TemplateFileItem(file, false);
        AddChild(vm); // todo: use event action
    }

    public async void LoadIcon()
    {
        var thumbnail = await Folder.GetThumbnailAsync(ThumbnailMode.ListView, 32, ThumbnailOptions.UseCurrentScale);
        var img = new BitmapImage();
        img.SetSource(thumbnail);
        Icon = img;
    }
}