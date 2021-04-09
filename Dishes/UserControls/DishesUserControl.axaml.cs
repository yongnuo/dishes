using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using AvaloniaTools.Enums;
using AvaloniaTools.Windows;
using Dishes.Models;

namespace Dishes.UserControls
{
    public class DishesUserControl : ListsUserControl<Dish>
    {
        private TextBox DishName { get; set; }
        private TextBox DishComment { get; set; }
        private TextBox DishPath { get; set; }
        private ComboBox SelectedSource { get; set; }
        private UniformGrid TagsGrid { get; set; }
        private UniformGrid FindTagsGrid { get; set; }

        private readonly Dictionary<Tag, bool?> _findTagsGridValues;
        public event Action DishesUpdated = delegate { };

        public DishesUserControl()
        {
            _findTagsGridValues = new Dictionary<Tag, bool?>();
        }

        protected override void SetupAdditionalGuiControls()
        {
            DishName = this.FindControl<TextBox>("DishName");
            DishPath = this.FindControl<TextBox>("DishPath");
            DishComment = this.FindControl<TextBox>("DishComment");
            SelectedSource = this.FindControl<ComboBox>("DishSource");
            TagsGrid = this.FindControl<UniformGrid>("TagsGrid");
            FindTagsGrid = this.FindControl<UniformGrid>("FindTagsGrid");
        }


        protected override void InitializeAdditionalGuiData()
        {
            ReloadSources();
            if (Service.Sources.Any())
                SelectedSource.SelectedIndex = 0;

            ReloadTags();
        }

        public void ReloadSources()
        {
            var selectedSource = (Source)SelectedSource.SelectedItem;
            SelectedSource.Items = null; // Needed for a working reload of Items
            SelectedSource.Items = Service.Sources;
            SelectedSource.SelectedItem = selectedSource;
        }

        public void ReloadTags()
        {
            var tagsGridChildren = new Controls();
            var findTagsGridChildren = new Controls();
            foreach (var tag in Service.Tags)
            {
                var isChecked = TagsGrid.Children.FirstOrDefault(c => ((CheckBox)c).Tag == tag) is CheckBox existing
                                && existing.IsChecked.HasValue
                                && existing.IsChecked.Value;
                tagsGridChildren.Add(new CheckBox
                {
                    Content = tag.Name,
                    Tag = tag,
                    IsChecked = isChecked
                });

                var findIsChecked = _findTagsGridValues.ContainsKey(tag)
                    ? _findTagsGridValues[tag]
                    : null;
                var findCheckBox = new CheckBox
                {
                    Content = tag.Name,
                    Tag = tag,
                    IsChecked = findIsChecked,
                    IsThreeState = true
                };
                findCheckBox.PropertyChanged += FindTagsGridChanged;
                findTagsGridChildren.Add(findCheckBox);
            }
            TagsGrid.Children.Clear();
            TagsGrid.Children.AddRange(tagsGridChildren);
            FindTagsGrid.Children.Clear();
            FindTagsGrid.Children.AddRange(findTagsGridChildren);
        }


        private void FindTagsGridChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property != ToggleButton.IsCheckedProperty)
                return;
            if (!(sender is CheckBox cb)) return;
            var tag = (Tag) cb.Tag;
            if (_findTagsGridValues.ContainsKey(tag))
                _findTagsGridValues[tag] = cb.IsChecked;
            else
                _findTagsGridValues.Add(tag, cb.IsChecked);
            QueueHandler.Trigger(string.Empty);
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

        protected override async Task DeleteEntity()
        {
            var entityId = GetEntityId();
            var dishToDelete = Service.Dishes.First(d => d.Id == entityId);
            var dialog = new ButtonDialog(ButtonEnum.OkCancel, Properties.Resources.RemoveDish, string.Format(Properties.Resources.ConfirmRemoveDish, dishToDelete.Name));
            if (await dialog.Open() == ButtonResult.Ok)
            {
                Service.DeleteDish(entityId);
                DishesUpdated();
            }
        }

        protected override List<Dish> PerformAdditionalFiltering(List<Dish> filteredEntities)
        {
            var trueTags = _findTagsGridValues.Where(tt => tt.Value == true).Select(tt => tt.Key);
            var falseTags = _findTagsGridValues.Where(ft => ft.Value == false).Select(ft => ft.Key);

            return filteredEntities.Where(fe =>
                                    trueTags.All(tt => fe.Tags.Contains(tt)) 
                                   && 
                                   falseTags.All(ft => !fe.Tags.Contains(ft))
                )
                .ToList();
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
