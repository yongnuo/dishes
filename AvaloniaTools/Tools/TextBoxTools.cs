using Avalonia.Controls;
using Avalonia.Input;

namespace AvaloniaTools.Tools
{ 
    public class TextBoxTools
    {
        public static void SelectTextOnFocus(object sender, GotFocusEventArgs e)
        {
            if (!(sender is TextBox textBox))
                return;
            textBox.SelectionStart = 0;
            textBox.SelectionEnd = textBox.Text?.Length ?? 0;
        }
    }
}
