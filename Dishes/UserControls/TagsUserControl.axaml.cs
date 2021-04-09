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
    public class TagsUserControl : ListsUserControl<Tag>
    {
        public TextBox TagName { get; set; }
        public event Action TagsUpdated = delegate { };

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
            TagsUpdated();
            return source;
        }

        protected override Tag UpdateEntity()
        {
            var source = Service.Tags.First(s => s.Id == GetEntityId());
            source.Name = TagName.Text;
            Service.UpdateTag(source);
            TagsUpdated();
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

        protected override async Task DeleteEntity()
        {
            var entityId = GetEntityId();
            var tagToDelete = Service.Tags.First(d => d.Id == entityId);
            var dialog = new ButtonDialog(ButtonEnum.OkCancel, Properties.Resources.RemoveTag, string.Format(Properties.Resources.ConfirmRemoveTag, tagToDelete.Name));
            if (await dialog.Open() == ButtonResult.Ok)
            {
                Service.DeleteTag(entityId);
                TagsUpdated();
            }
        }

        protected override List<Tag> PerformAdditionalFiltering(List<Tag> filteredEntities)
        {
            return filteredEntities;
        }
    }
}