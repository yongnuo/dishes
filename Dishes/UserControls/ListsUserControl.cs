using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Dishes.Interfaces;
using Dishes.Services;
using Dishes.Tools;

namespace Dishes.UserControls
{
    public abstract class ListsUserControl<T> : UserControl where T : IDbEntity
    {
        protected Service Service;

        private readonly QueueHandler _queueHandler;
        private List<T> _filteredEntities;
        private string _searchText;

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private TextBox SearchBox { get; set; }
        private TextBlock EntityId { get; set; }
        private DataGrid Entities { get; set; }

        private Button SaveButton { get; set; }
        private Button CancelButton { get; set; }
        protected Button DeleteButton { get; set; }

        protected abstract void SetupAdditionalGuiControls();
        protected abstract void InitializeAdditionalGuiData();
        protected abstract void SetupAdditionalEvents();
        protected abstract void SetInputFields(T selectedEntity);
        protected abstract void FocusPrimaryEditField();
        protected abstract T AddEntity();
        protected abstract T UpdateEntity();
        protected abstract void ClearAdditionalInputFields();
        protected abstract List<T> GetData();
        protected abstract void InitializeComponent();


        protected ListsUserControl()
        {
            _queueHandler = new QueueHandler(UpdateEntityList);

            // ReSharper disable once VirtualMemberCallInConstructor
            InitializeComponent();

            SetupGuiControls();

            _searchText = string.Empty;
            SearchBox.AttachedToVisualTree += (s, e) => SearchBox.Focus(); // Verkar ta tid
        }

        private void SetupGuiControls()
        {
            Entities = this.FindControl<DataGrid>("Entities");
            EntityId = this.FindControl<TextBlock>("EntityId");
            SearchBox = this.FindControl<TextBox>("SearchBox");
            SaveButton = this.FindControl<Button>("Save");
            CancelButton = this.FindControl<Button>("Cancel");
            DeleteButton = this.FindControl<Button>("Delete");
            DeleteButton.IsEnabled = false;

            SetupAdditionalGuiControls();
        }

        public void Initialize(Service service)
        {
            Service = service;
            InitializeGuiData();
            SetupEvents();
        }


        private void InitializeGuiData()
        {
            UpdateEntityList();
            if (GetData().Any())
            {
                Entities.SelectedIndex = 0;
            }
            InitializeAdditionalGuiData();
        }

        private void SetupEvents()
        {
            SearchBox.KeyDown += HandleSearchKeyDown;

            SearchBox.GotFocus += SelectTextOnFocus;
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
            DeleteButton.Click += (s, e) => Delete();

            SetupAdditionalEvents();
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

        protected void SelectTextOnFocus(object sender, GotFocusEventArgs e)
        {
            if (!(sender is TextBox textBox))
                return;
            textBox.SelectionStart = 0;
            textBox.SelectionEnd = textBox.Text?.Length ?? 0;
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
                //DeleteButton.IsEnabled = false;
                //return;
            }

            var selectedEntity = (T)Entities.SelectedItem;
            EntityId.Text = selectedEntity.Id.ToString();
            SaveButton.Content = Properties.Resources.Update;
            DeleteButton.IsEnabled = true;
            SetInputFields(selectedEntity);
            FocusPrimaryEditField();
        }


        private void SaveOrUpdate()
        {
            var entity = GetEntityId() == -1 ? AddEntity() : UpdateEntity();
            Entities.SelectedItem = entity;
            UpdateEntityList();
            ClearInputFields();
            SearchBox.Focus();
        }

        private void Delete()
        {
            var selectedIndex = Entities.SelectedIndex;
            DeleteEntity();
            UpdateEntityList();
            ClearInputFields();
            if (selectedIndex >= GetData().Count)
                selectedIndex--;
            if (selectedIndex < 0)
                selectedIndex = 0;
            Entities.SelectedIndex = selectedIndex;
            SearchBox.Focus();
        }

        protected abstract void DeleteEntity();

        protected int GetEntityId()
        {
            if (string.IsNullOrWhiteSpace(EntityId.Text))
                return -1;
            return Convert.ToInt32(EntityId.Text);
        }

        private void ClearInputFields()
        {
            EntityId.Text = string.Empty;
            DeleteButton.IsEnabled = false;
            SaveButton.Content = Properties.Resources.Save;
            ClearAdditionalInputFields();
            SearchBox.Focus();
        }

        public void UpdateEntityList()
        {
            var selectedEntity = (T)Entities.SelectedItem;
            var items = GetData();
            _filteredEntities = items.Where(d =>
                d.Name.Contains(_searchText, StringComparison.CurrentCultureIgnoreCase)).ToList();
            Entities.Items = _filteredEntities;
            if (_filteredEntities.Contains(selectedEntity))
                Entities.SelectedItem = selectedEntity;
            else
                Entities.SelectedIndex = 0;
        }
    }
}