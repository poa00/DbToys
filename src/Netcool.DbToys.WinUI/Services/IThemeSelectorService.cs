﻿using Microsoft.UI.Xaml;

namespace Netcool.DbToys.WinUI.Services;

public interface IThemeSelectorService
{
    ElementTheme Theme
    {
        get;
    }

    Task InitializeAsync();

    Task SetThemeAsync(ElementTheme theme);

    Task SetRequestedThemeAsync();
}