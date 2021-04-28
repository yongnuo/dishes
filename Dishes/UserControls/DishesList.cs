using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using AvaloniaTools.Tools;
using Dishes.Interfaces;
using Dishes.Models;
using Dishes.Services;

namespace Dishes.UserControls
{
    public class DishesList : IList
    {
        private TextBox DishName { get; set; }
        private TextBox DishComment { get; set; }
        private TextBox DishPath { get; set; }
        private ComboBox SelectedSource { get; set; }
        private UniformGrid TagsGrid { get; set; }
        private UniformGrid FindTagsGrid { get; set; }

        private readonly Dictionary<Tag, bool?> _findTagsGridValues;
        public event Action DishesUpdated = delegate { };

        public void FocusPrimaryEditField()
        {
            DishName.Focus();
        }

        public IDbEntity AddEntity(Service service)
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
            service.AddDish(dish);
            DishesUpdated();
            return dish;
        }

        private List<Tag> GetDishTagsFromGui()
        {
            return TagsGrid.Children
                .Where(c =>
                {
                    var cbIsChecked = ((CheckBox)c).IsChecked;
                    return cbIsChecked.HasValue && cbIsChecked.Value;
                })
                .Select(c => (Tag)((CheckBox)c).Tag)
                .ToList();
        }

        public IDbEntity UpdateEntity(int id, Service service)
        {
            var selectedSource = (Source)SelectedSource.SelectedItem;
            var dish = (Dish)service.Dishes.First(d => d.Id == id);
            dish.Name = DishName.Text;
            dish.Path = DishPath.Text;
            dish.Comment = DishComment.Text;
            dish.Source = selectedSource;
            dish.Tags = GetDishTagsFromGui();
            service.UpdateDish(dish);
            DishesUpdated();
            return dish;
        }

        public List<IDbEntity> GetData(Service service)
        {
            return service.Dishes;
        }

        public void DeleteEntity(int id, Service service)
        {
            service.DeleteDish(id);
            DishesUpdated();
        }

        public List<IDbEntity> PerformAdditionalFiltering(List<IDbEntity> entities)
        {
            return entities;
        }

        public IDataTemplate EditDataTemplate()
        {
            return new FuncDataTemplate<Dish>((x, y) =>
            {
                var nameLabel = new TextBlock
                {
                    Text = Properties.Resources.NameLabel,
                    Classes = Classes.Parse("Label")
                };
                nameLabel.SetValue(Grid.RowProperty, 0);
                nameLabel.SetValue(Grid.ColumnProperty, 0);
                DishName = new TextBox
                {
                    [!TextBox.TextProperty] = new Binding("Name"),
                    Classes = Classes.Parse("Property")
                };
                DishName.GotFocus += TextBoxTools.SelectTextOnFocus;
                DishName.SetValue(Grid.RowProperty, 0);
                DishName.SetValue(Grid.ColumnProperty, 1);

                var pathLabel = new TextBlock
                {
                    Text = Properties.Resources.DishPathLabel,
                    Classes = Classes.Parse("Label")
                };
                pathLabel.SetValue(Grid.RowProperty, 1);
                pathLabel.SetValue(Grid.ColumnProperty, 0);
                DishPath = new TextBox
                {
                    [!TextBox.TextProperty] = new Binding("Path"),
                    Classes = Classes.Parse("Property")
                };
                DishPath.GotFocus += TextBoxTools.SelectTextOnFocus;
                DishPath.SetValue(Grid.RowProperty, 1);
                DishPath.SetValue(Grid.ColumnProperty, 1);

                var sourceLabel = new TextBlock
                {
                    Text = Properties.Resources.DishSourceLabel,
                    Classes = Classes.Parse("Label")
                };
                sourceLabel.SetValue(Grid.RowProperty, 2);
                sourceLabel.SetValue(Grid.ColumnProperty, 0);
                SelectedSource = new ComboBox
                {
                    [!ComboBox.ItemsProperty] = new Binding("Path"),
                    Classes = Classes.Parse("Property")
                };
                DishPath.GotFocus += TextBoxTools.SelectTextOnFocus;
                DishPath.SetValue(Grid.RowProperty, 1);
                DishPath.SetValue(Grid.ColumnProperty, 1);

                var grid = new Grid
                {
                    RowDefinitions = new RowDefinitions("Auto, Auto"),
                    ColumnDefinitions = new ColumnDefinitions("*,*"),
                    Children =
                    {
                        nameLabel,
                        DishName,
                        pathLabel,
                        DishPath
                    }
                };
                return grid;
            });
        }

        public IDbEntity CreateDefaultObject()
        {
            return new Dish();
        }

        public IEnumerable<DataGridColumn> DataGridColumns()
        {
            return new List<DataGridColumn>
            {
                new DataGridTextColumn
                {
                    MinWidth = 30,
                    Header = Properties.Resources.IdLabel,
                    Binding = new Binding("Id")
                },
                new DataGridTextColumn
                {
                    MinWidth = 270,
                    Header = Properties.Resources.NameLabel,
                    Binding = new Binding("Name")
                },
                new DataGridTextColumn
                {
                    MinWidth = 270,
                    Header = Properties.Resources.DishSourceLabel,
                    Binding = new Binding("Source.Name")
                }
            };
        }

        public string RemoveHeader => Properties.Resources.RemoveDish;

        public string ConfirmRemoveCaption(int id, Service service)
        {
            var dishToDelete = service.Dishes.First(t => t.Id == id);
            return string.Format(Properties.Resources.ConfirmRemoveDish, dishToDelete.Name);

        }

        public void SetupAdditionalEvents(Action updated)
        {
            DishesUpdated += updated;
        }
    }
}