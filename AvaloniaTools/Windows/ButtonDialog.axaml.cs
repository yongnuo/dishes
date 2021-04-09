using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaTools.Enums;

namespace AvaloniaTools.Windows
{
    public class ButtonDialog : Window
    {
        public TextBlock Text { get; set; }
        public Button Cancel { get; set; }
        public Button Ok { get; set; }

        public ButtonEnum Type { get; }

        private ButtonResult _result;

        // ReSharper disable once UnusedMember.Global
        public ButtonDialog()
        {
            InitializeComponent();
        }

        public ButtonDialog(ButtonEnum type, string title, string text)
        {
            Icon = WindowTool.LoadIconAsWindowIcon();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            Type = type;
            Title = title;
            Text.Text = text;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            Ok = this.FindControl<Button>("Ok");
            Cancel = this.FindControl<Button>("Cancel");
            Text = this.FindControl<TextBlock>("Text");

            Ok.Click += (s, e) =>
            {
                _result = ButtonResult.Ok;
                Close();
            };

            Cancel.Click += (s, e) =>
            {
                _result = ButtonResult.Cancel;
                Close();
            };
        }

        public async Task<ButtonResult> Open()
        {
            var showDialog = ShowDialog(WindowTool.GetMainWindow());
            Cancel.Focus();
            await showDialog;
            return _result;
        }
    }
}