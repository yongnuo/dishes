using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Dishes.Services;
using Dishes.UserControls;

namespace Dishes
{
    public class MainWindow : Window
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Service _service;

        private SourcesUserControl SourcesUserControl { get; }
        private DishesUserControl DishesUserControl { get; }
        private TagsUserControl TagsUserControl { get; }

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
            TagsUserControl = (TagsUserControl)this.FindControl<UserControl>("TagsUserControl");
            DishesUserControl.Initialize(_service);
            SourcesUserControl.Initialize(_service);
            TagsUserControl.Initialize(_service);

            SourcesUserControl.SourcesUpdated += DishesUserControl.ReloadSources;
            DishesUserControl.DishesUpdated += SourcesUserControl.UpdateEntityList;
            TagsUserControl.TagsUpdated += DishesUserControl.ReloadTags;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
