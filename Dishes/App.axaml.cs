using System.Globalization;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Dishes
{
    public class App : Application
    {
        public override void Initialize()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("sv-SE");
            CultureInfo.CurrentCulture = new CultureInfo("sv-SE");
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
                
            }

            base.OnFrameworkInitializationCompleted();
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop2)
            {
                var window = (MainWindow) desktop2.MainWindow;
                window.Initialize();
            }
        }
    }
}
