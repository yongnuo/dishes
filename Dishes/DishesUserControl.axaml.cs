using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;

namespace Dishes
{
    public class DishesUserControl : ListsUserControl<Dish>
    {
        private TextBox DishName { get; set; }
        private TextBox DishComment { get; set; }
        private TextBox DishPath { get; set; }
        private ComboBox SelectedSource { get; set; }

        protected override void SetupAdditionalGuiControls()
        {
            DishName = this.FindControl<TextBox>("DishName");
            DishPath = this.FindControl<TextBox>("DishPath");
            DishComment = this.FindControl<TextBox>("DishComment");
            SelectedSource = this.FindControl<ComboBox>("DishSource");
            TagsGrid = this.FindControl<UniformGrid>("TagsGrid");
        }

        public UniformGrid TagsGrid { get; set; }

        protected override void InitializeAdditionalGuiData()
        {
            SelectedSource.Items = Service.Sources;
            if (Service.Sources.Any())
                SelectedSource.SelectedIndex = 0;

            foreach (var tag in Service.Tags)
            {
                TagsGrid.Children.Add(new CheckBox
                {
                    Content = tag.Name
                });
            }
        }

        protected override void SetupAdditionalEvents()
        {
            DishName.KeyDown += HandleEditKeyDown;
            DishPath.KeyDown += HandleEditKeyDown;
            SelectedSource.KeyDown += HandleEditKeyDown;

            DishName.GotFocus += SelectTextOnFocus;
            DishPath.GotFocus += SelectTextOnFocus;
        }

        protected override void FocusPrimaryEditField()
        {
            DishName.Focus();
        }

        protected override Dish AddEntity()
        {
            var selectedSource = (Source)SelectedSource.SelectedItem;
            var dish = new Dish
            {
                Name = DishName.Text,
                Path = DishPath.Text,
                Comment = DishComment.Text,
                Source = selectedSource
            };
            Service.AddDish(dish);
            return dish;
        }

        protected override Dish UpdateEntity()
        {
            var selectedSource = (Source)SelectedSource.SelectedItem;
            var dish = Service.Dishes.First(d => d.Id == GetEntityId());
            dish.Name = DishName.Text;
            dish.Path = DishPath.Text;
            dish.Comment = DishComment.Text;
            dish.Source = selectedSource;
            Service.UpdateDish(dish);
            return dish;
        }

        protected override void DeleteEntity()
        {
            Service.DeleteDish(GetEntityId());
        }

        protected override void SetInputFields(Dish dish)
        {
            DishName.Text = dish.Name;
            DishPath.Text = dish.Path;
            DishComment.Text = dish.Comment;
            SelectedSource.SelectedItem = dish.Source;
        }

        protected override void ClearAdditionalInputFields()
        {
            DishName.Text = string.Empty;
            DishPath.Text = string.Empty;
            DishComment.Text = string.Empty;
        }

        protected override List<Dish> GetData()
        {
            return Service.Dishes;
        }

        protected override void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
