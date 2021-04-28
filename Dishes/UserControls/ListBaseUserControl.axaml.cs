using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using AvaloniaTools.Enums;
using AvaloniaTools.Tools;
using AvaloniaTools.Windows;
using Dishes.Interfaces;
using Dishes.Services;
using Dishes.Tools;

namespace Dishes.UserControls
{
    public class ListBaseUserControl : UserControl
    {
        public event Action Updated = delegate { };

        private IList _list;
        private Service _service;
        private readonly QueueHandler _queueHandler;
        private List<IDbEntity> _filteredEntities;
        private string _searchText;

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private TextBox SearchBox { get; set; }
        private TextBlock EntityId { get; set; }
        private DataGrid Entities { get; set; }
        private TextBlock NoOfEntities { get; set; }
        private ContentPresenter EditFields { get; set; }

        private Button SaveButton { get; set; }
        private Button CancelButton { get; set; }
        private Button DeleteButton { get; set; }

        public ListBaseUserControl()
        {
            _queueHandler = new QueueHandler(UpdateEntityList);
            _searchText = string.Empty;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void SetupGuiControls()
        {
            Entities = this.FindControl<DataGrid>("Entities");
            EntityId = this.FindControl<TextBlock>("EntityId");
            NoOfEntities = this.FindControl<TextBlock>("NoOfEntities");
            SearchBox = this.FindControl<TextBox>("SearchBox");
            SaveButton = this.FindControl<Button>("Save");
            CancelButton = this.FindControl<Button>("Cancel");
            DeleteButton = this.FindControl<Button>("Delete");
            EditFields = this.FindControl<ContentPresenter>("EditFields");
            DeleteButton.IsEnabled = false;
            
            EditFields.ContentTemplate = _list.EditDataTemplate();
            EditFields.Content = _list.CreateDefaultObject();

            foreach (var column in _list.DataGridColumns())
            {
                Entities.Columns.Add(column);
            }
        }

        private void Update()
        {
            Updated();
        }

        public void Initialize(Service service, IList list)
        {
            _list = list;
            _service = service;
            
            SetupGuiControls();
            SearchBox.AttachedToVisualTree += (s, e) => SearchBox.Focus(); // Verkar ta tid
            InitializeGuiData();
            SetupEvents();
        }


        private void InitializeGuiData()
        {
            UpdateEntityList();
            if (_list.GetData(_service).Any())
            {
                Entities.SelectedIndex = 0;
            }
            //_list.InitializeAdditionalGuiData();
        }

        private void SetupEvents()
        {
            SearchBox.KeyDown += HandleSearchKeyDown;

            SearchBox.GotFocus += TextBoxTools.SelectTextOnFocus;
            Entities.DoubleTapped += (s, e) => LoadSelectedEntity();
            SearchBox.PropertyChanged += (s, e) =>
            {
                if (SearchBox.Text == null)
                    return;
                if (SearchBox.Text != _searchText)
                {
                    _searchText = SearchBox.Text;
                    _queueHandler.Trigger(_searchText);
                }
            };
            SaveButton.Click += (s, e) => SaveOrUpdate();
            CancelButton.Click += (s, e) => ClearInputFields();
            DeleteButton.Click += async (s, e) => await Delete();

            _list.SetupAdditionalEvents(Update);
        }

        protected void HandleEditKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    ClearInputFields();
                    SearchBox.Focus();
                    break;
                case Key.Enter:
                    SaveOrUpdate();
                    break;
            }
        }

        private void HandleSearchKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    LoadSelectedEntity();
                    break;
                case Key.Down:
                    if (Entities.SelectedIndex < _filteredEntities.Count - 1)
                        Entities.SelectedIndex += 1;
                    break;
                case Key.Up:
                    Entities.SelectedIndex -= 1;
                    if (Entities.SelectedIndex < 0)
                        Entities.SelectedIndex = 0;
                    break;
            }
        }

        private void LoadSelectedEntity()
        {
            if (Entities.SelectedIndex == -1)
            {
                throw new InvalidOperationException("No entity selected");
            }

            var selectedEntity = (IDbEntity)Entities.SelectedItem;
            EditFields.Content = selectedEntity;
            EntityId.Text = selectedEntity.Id.ToString();
            SaveButton.Content = Properties.Resources.Update;
            DeleteButton.IsEnabled = true;
            //_list.SetInputFields(selectedEntity);
            _list.FocusPrimaryEditField();
        }


        private void SaveOrUpdate()
        {
            var entity = GetEntityId() == -1 ? _list.AddEntity(_service) : _list.UpdateEntity(GetEntityId(), _service);
            Entities.SelectedItem = entity;
            UpdateEntityList();
            ClearInputFields();
            SearchBox.Focus();
        }

        private async Task Delete()
        {
            var entityId = GetEntityId();
            var dialog = new ButtonDialog(ButtonEnum.OkCancel, _list.RemoveHeader,
                _list.ConfirmRemoveCaption(entityId, _service));
            if (await dialog.Open() == ButtonResult.Ok)
            {
                var selectedIndex = Entities.SelectedIndex;
                _list.DeleteEntity(entityId, _service);
                UpdateEntityList();
                ClearInputFields();
                if (selectedIndex >= _list.GetData(_service).Count)
                    selectedIndex--;
                if (selectedIndex < 0)
                    selectedIndex = 0;
                Entities.SelectedIndex = selectedIndex;
                SearchBox.Focus();
            }
        }

        private int GetEntityId()
        {
            if (string.IsNullOrWhiteSpace(EntityId.Text))
                return -1;
            return Convert.ToInt32(EntityId.Text);
        }

        private void ClearInputFields()
        {
            EntityId.Text = string.Empty;
            EditFields.Content = _list.CreateDefaultObject();
            DeleteButton.IsEnabled = false;
            SaveButton.Content = Properties.Resources.Save;
            //_list.ClearAdditionalInputFields();
            SearchBox.Focus();
        }

        public void UpdateEntityList()
        {
            var selectedEntity = (IDbEntity)Entities.SelectedItem;
            var items = _list.GetData(_service);
            _filteredEntities = items.Where(d =>
                d.Name.Contains(_searchText, StringComparison.CurrentCultureIgnoreCase)).ToList();
            _filteredEntities = _list.PerformAdditionalFiltering(_filteredEntities);
            Entities.Items = _filteredEntities;
            if (_filteredEntities.Contains(selectedEntity))
                Entities.SelectedItem = selectedEntity;
            else
                Entities.SelectedIndex = 0;
            NoOfEntities.Text = $"{_filteredEntities.Count} / {items.Count}";
        }
    }
}