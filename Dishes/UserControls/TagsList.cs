using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using AvaloniaTools.Tools;
using Dishes.Interfaces;
using Dishes.Models;
using Dishes.Services;

namespace Dishes.UserControls
{
    public class TagsList : IList
    {
        private TextBox TagName { get; set; }
        private event Action TagsUpdated = delegate { };

        public string RemoveHeader => Properties.Resources.RemoveTag;
        public string ConfirmRemoveCaption(int id, Service service)
        {
            var tagToDelete = service.Tags.First(t => t.Id == id);
            return string.Format(Properties.Resources.ConfirmRemoveTag, tagToDelete.Name);
        }

        public IDbEntity CreateDefaultObject()
        {
            return new Tag();
        }

        public void SetupAdditionalEvents(Action updated)
        {
            TagsUpdated += updated;
        }

        public void FocusPrimaryEditField()
        {
            TagName.Focus();
        }

        public IDbEntity AddEntity(Service service)
        {
            var source = new Tag();
            source.Name = TagName.Text;
            service.AddTag(source);
            TagsUpdated();
            return source;
        }

        public IDbEntity UpdateEntity(int entityId, Service service)
        {
            var source = (Tag)service.Tags.First(s => s.Id == entityId);
            source.Name = TagName.Text;
            service.UpdateTag(source);
            TagsUpdated();
            return source;
        }

        public void DeleteEntity(int entityId, Service service)
        {
            service.DeleteTag(entityId);
            TagsUpdated();
        }

        public List<IDbEntity> GetData(Service service)
        {
            return service.Tags;
        }

        public List<IDbEntity> PerformAdditionalFiltering(List<IDbEntity> filteredEntities)
        {
            return filteredEntities;
        }

        public IDataTemplate EditDataTemplate()
        {
            return new FuncDataTemplate<Tag>((x, y) =>
            {
                var label = new TextBlock
                {
                    Text = Properties.Resources.NameLabel,
                    Classes = Classes.Parse("Label")
                };
                label.SetValue(Grid.RowProperty, 0);
                label.SetValue(Grid.ColumnProperty, 0);
                TagName = new TextBox
                {
                    [!TextBox.TextProperty] = new Binding("Name"),
                    Classes = Classes.Parse("Property")
                };
                TagName.GotFocus += TextBoxTools.SelectTextOnFocus;
                TagName.SetValue(Grid.RowProperty, 0);
                TagName.SetValue(Grid.ColumnProperty, 1);

                var grid = new Grid
                {
                    RowDefinitions = new RowDefinitions("Auto"),
                    ColumnDefinitions = new ColumnDefinitions("*,*"),
                    Children =
                    {
                        label,
                        TagName
                    }
                };
                return grid;
            });
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
                }
            };
        }
    }
}