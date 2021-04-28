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
    public class SourcesList : IList
    {
        private TextBox SourceName { get; set; }
        private event Action SourcesUpdated = delegate { };
        public string RemoveHeader => Properties.Resources.RemoveSource;

        public string ConfirmRemoveCaption(int id, Service service)
        {
            var sourceToDelete = service.Sources.First(t => t.Id == id);
            return string.Format(Properties.Resources.ConfirmRemoveSource, sourceToDelete.Name);
        }

        public void FocusPrimaryEditField()
        {
            SourceName.Focus();
        }

        public IDbEntity AddEntity(Service service)
        {
            var source = new Source();
            source.Name = SourceName.Text;
            service.AddSource(source);
            SourcesUpdated();
            return source;
        }

        public IDbEntity UpdateEntity(int id, Service service)
        {
            var source = (Source)service.Sources.First(s => s.Id == id);
            source.Name = SourceName.Text;
            service.UpdateSource(source);
            SourcesUpdated();
            return source;
        }

        public List<IDbEntity> GetData(Service service)
        {
            return service.Sources;
        }

        public void DeleteEntity(int id, Service service)
        {
            service.DeleteSource(id);
            SourcesUpdated();
        }

        public List<IDbEntity> PerformAdditionalFiltering(List<IDbEntity> entities)
        {
            return entities;
        }

        public IDataTemplate EditDataTemplate()
        {
            return new FuncDataTemplate<Source>((x, y) =>
            {
                var label = new TextBlock
                {
                    Text = Properties.Resources.NameLabel,
                    Classes = Classes.Parse("Label")
                };
                label.SetValue(Grid.RowProperty, 0);
                label.SetValue(Grid.ColumnProperty, 0);
                SourceName = new TextBox
                {
                    [!TextBox.TextProperty] = new Binding("Name"),
                    Classes = Classes.Parse("Property")
                };
                SourceName.GotFocus += TextBoxTools.SelectTextOnFocus;
                SourceName.SetValue(Grid.RowProperty, 0);
                SourceName.SetValue(Grid.ColumnProperty, 1);

                var grid = new Grid
                {
                    RowDefinitions = new RowDefinitions("Auto"),
                    ColumnDefinitions = new ColumnDefinitions("*,*"),
                    Children =
                    {
                        label,
                        SourceName
                    }
                };
                return grid;
            });
        }

        public IDbEntity CreateDefaultObject()
        {
            return new Source();
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

        public void SetupAdditionalEvents(Action updated)
        {
            SourcesUpdated += updated;
        }
    }
}