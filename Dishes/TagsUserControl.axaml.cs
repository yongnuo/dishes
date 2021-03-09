using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Dishes
{
    public class TagsUserControl : ListsUserControl<Tag>
    {
        public TextBox TagName { get; set; }

        protected override void SetupAdditionalGuiControls()
        {
            TagName = this.FindControl<TextBox>("TagName");
        }

        protected override void InitializeAdditionalGuiData()
        {
        }

        protected override void SetupAdditionalEvents()
        {
            TagName.GotFocus += SelectTextOnFocus;

            TagName.KeyDown += HandleEditKeyDown;
        }

        protected override void SetInputFields(Tag selectedEntity)
        {
            TagName.Text = selectedEntity.Name;
        }

        protected override void FocusPrimaryEditField()
        {
            TagName.Focus();
        }

        protected override Tag AddEntity()
        {
            var source = new Tag();
            source.Name = TagName.Text;
            Service.AddTag(source);
            return source;
        }

        protected override Tag UpdateEntity()
        {
            var source = Service.Tags.First(s => s.Id == GetEntityId());
            source.Name = TagName.Text;
            Service.UpdateTag(source);
            return source;
        }

        protected override void ClearAdditionalInputFields()
        {
            TagName.Text = string.Empty;
        }

        protected override List<Tag> GetData()
        {
            return Service.Tags;
        }

        protected override void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void DeleteEntity()
        {
            Service.DeleteTag(GetEntityId());
        }
    }
}