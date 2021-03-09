using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Dishes
{
    public class SourcesUserControl : ListsUserControl<Source>
    {
        public TextBox SourceName { get; set; }

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
            return source;
        }

        protected override Source UpdateEntity()
        {
            var source = Service.Sources.First(s => s.Id == GetEntityId());
            source.Name = SourceName.Text;
            Service.UpdateSource(source);
            return source;
        }

        protected override void ClearAdditionalInputFields()
        {
            SourceName.Text = string.Empty;
        }

        protected override List<Source> GetData()
        {
            return Service.Sources;
        }

        protected override void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void DeleteEntity()
        {
            Service.DeleteSource(GetEntityId());
        }
    }
}
