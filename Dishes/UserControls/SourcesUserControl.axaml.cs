using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaTools.Enums;
using AvaloniaTools.Windows;
using Dishes.Models;

namespace Dishes.UserControls
{
    public class SourcesUserControl : ListsUserControl<Source>
    {
        public TextBox SourceName { get; set; }
        public event Action SourcesUpdated = delegate { };

        protected override void SetupAdditionalGuiControls()
        {
            SourceName = this.FindControl<TextBox>("SourceName");
        }

        protected override void InitializeAdditionalGuiData()
        {
        }

        protected override void SetupAdditionalEvents()
        {
            SourceName.GotFocus += SelectTextOnFocus;

            SourceName.KeyDown += HandleEditKeyDown;
        }

        protected override void SetInputFields(Source selectedEntity)
        {
            SourceName.Text = selectedEntity.Name;
            DeleteButton.IsEnabled = selectedEntity.NoOfUsages == 0;
        }

        protected override void FocusPrimaryEditField()
        {
            SourceName.Focus();
        }

        protected override Source AddEntity()
        {
            var source = new Source();
            source.Name = SourceName.Text;
            Service.AddSource(source);
            SourcesUpdated();
            return source;
        }

        protected override Source UpdateEntity()
        {
            var source = Service.Sources.First(s => s.Id == GetEntityId());
            source.Name = SourceName.Text;
            Service.UpdateSource(source);
            SourcesUpdated();
            return source;
        }

        protected override void ClearAdditionalInputFields()
        {
            SourceName.Text = string.Empty;
        }

        protected override List<Source> GetData()
        {
            foreach (var source in Service.Sources)
            {
                source.NoOfUsages = Service.Dishes.Count(d => d.Source == source);
            }
            return Service.Sources;
        }

        protected override void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override async Task DeleteEntity()
        {
            var entityId = GetEntityId();
            var sourceToDelete = Service.Sources.First(d => d.Id == entityId);
            var dialog = new ButtonDialog(ButtonEnum.OkCancel, Properties.Resources.RemoveSource, string.Format(Properties.Resources.ConfirmRemoveSource, sourceToDelete.Name));
            if (await dialog.Open() == ButtonResult.Ok)
            {
                Service.DeleteSource(entityId);
                SourcesUpdated();
            }
        }

        protected override List<Source> PerformAdditionalFiltering(List<Source> filteredEntities)
        {
            return filteredEntities;
        }
    }
}
