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

        private ListBaseUserControl Sources { get; }
        private ListBaseUserControl DishesUserControl { get; }
        //private TagsUserControl TagsUserControl { get; }
        private ListBaseUserControl Tags { get; }

        public MainWindow()
        {
            _service = new Service();
            _service.LoadAll();

            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            DishesUserControl = (ListBaseUserControl) this.FindControl<UserControl>("Dishes");
            Sources = (ListBaseUserControl) this.FindControl<UserControl>("Sources");
            //TagsUserControl = (TagsUserControl) this.FindControl<UserControl>("TagsUserControl");
            Tags = (ListBaseUserControl) this.FindControl<UserControl>("Tags");

        }

        public void Initialize()
        {
            DishesUserControl.Initialize(_service, new DishesList());
            Sources.Initialize(_service, new SourcesList());
            Tags.Initialize(_service, new TagsList());

            //Sources.Updated += DishesUserControl.ReloadSources;
            //DishesUserControl.DishesUpdated += Sources.UpdateEntityList;
            //Tags.Updated += DishesUserControl.ReloadTags;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
