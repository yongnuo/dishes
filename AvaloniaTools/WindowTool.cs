using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace AvaloniaTools
{
    public static class WindowTool
    {
        public static Window GetMainWindow()
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                return desktopLifetime.MainWindow;
            }
            return null;
        }

        public static PixelPoint GetCenterPosition(Window childWindow)
        {
            var mainWindow = GetMainWindow();
            var x = mainWindow.Position.X + mainWindow.Width / 2 - (double.IsNaN(childWindow.Width) ? 0 : childWindow.Width) / 2;
            var y = mainWindow.Position.Y + mainWindow.Height / 2 - (double.IsNaN(childWindow.Height) ? 0 : childWindow.Height) / 2;
            return new PixelPoint((int)x, (int)y);
        }

        public static Bitmap LoadIconAsBitmap()
        {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            var stream = assets.Open(new Uri("avares://GUI/Assets/Accounts.ico"));
            return new Bitmap(stream);
        }

        public static WindowIcon LoadIconAsWindowIcon()
        {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            var stream = assets.Open(new Uri("avares://Dishes/Assets/dishes.ico"));
            return new WindowIcon(stream);
        }
    }
}