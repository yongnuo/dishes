using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Dishes.Models;

namespace Dishes.UserControls
{
    public class DishesUserControl : ListsUserControl<Dish>
    {
        private TextBox DishName { get; set; }
        private TextBox DishComment { get; set; }
        private TextBox DishPath { get; set; }
        private ComboBox SelectedSource { get; set; }

        public event Action DishesUpdated = delegate { };

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
            ReloadSources();
            if (Service.Sources.Any())
                SelectedSource.SelectedIndex = 0;

            ReloadTags();
        }

        public void ReloadSources()
        {
            var selectedSource = SelectedSource.SelectedItem;
            SelectedSource.Items = Service.Sources;
            SelectedSource.SelectedItem = null;
            SelectedSource.SelectedItem = selectedSource;
        }

        public void ReloadTags()
        {
            var controls = new Controls();
            foreach (var tag in Service.Tags)
            {
                var isChecked = TagsGrid.Children.FirstOrDefault(c => ((CheckBox) c).Tag == tag) is CheckBox existing 
                                && existing.IsChecked.HasValue 
                                && existing.IsChecked.Value;
                controls.Add(new CheckBox
                {
                    Content = tag.Name,
                    Tag = tag,
                    IsChecked = isChecked
                });
            }
            TagsGrid.Children.Clear();
            TagsGrid.Children.AddRange(controls);
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
                Source = selectedSource,
                Tags = GetDishTagsFromGui()
            };
            Service.AddDish(dish);
            DishesUpdated();
            return dish;
        }

        private List<Tag> GetDishTagsFromGui()
        {
            return TagsGrid.Children
                .Where(c =>
                {
                    var cbIsChecked = ((CheckBox) c).IsChecked;
                    return cbIsChecked.HasValue && cbIsChecked.Value;
                })
                .Select(c => (Tag)((CheckBox)c).Tag)
                .ToList();
        }

        protected override Dish UpdateEntity()
        {
            var selectedSource = (Source)SelectedSource.SelectedItem;
            var dish = Service.Dishes.First(d => d.Id == GetEntityId());
            dish.Name = DishName.Text;
            dish.Path = DishPath.Text;
            dish.Comment = DishComment.Text;
            dish.Source = selectedSource;
            dish.Tags = GetDishTagsFromGui();
            Service.UpdateDish(dish);
            DishesUpdated();
            return dish;
        }

        protected override void DeleteEntity()
        {
            Service.DeleteDish(GetEntityId());
            DishesUpdated();
        }

        protected override void SetInputFields(Dish dish)
        {
            DishName.Text = dish.Name;
            DishPath.Text = dish.Path;
            DishComment.Text = dish.Comment;
            SelectedSource.SelectedItem = dish.Source;
            foreach (var tagsGridChild in TagsGrid.Children)
            {
                var cb = (CheckBox) tagsGridChild;
                cb.IsChecked = dish.Tags.Any(t => t == (Tag) cb.Tag);
            }
        }

        protected override void ClearAdditionalInputFields()
        {
            DishName.Text = string.Empty;
            DishPath.Text = string.Empty;
            DishComment.Text = string.Empty;
            foreach (var cb in TagsGrid.Children)
            {
                ((CheckBox) cb).IsChecked = false;
            }
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
