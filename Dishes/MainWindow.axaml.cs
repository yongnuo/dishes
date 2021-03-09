using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Dishes
{
    public class MainWindow : Window
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Service _service;

        private SourcesUserControl SourcesUserControl { get; set; }
        private DishesUserControl DishesUserControl { get; set; }

        public MainWindow()
        {
            _service = new Service();
            _service.LoadAll();

            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            DishesUserControl = (DishesUserControl)this.FindControl<UserControl>("DishesUserControl");
            SourcesUserControl = (SourcesUserControl)this.FindControl<UserControl>("SourcesUserControl");
            DishesUserControl.Initialize(_service);
            SourcesUserControl.Initialize(_service);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
