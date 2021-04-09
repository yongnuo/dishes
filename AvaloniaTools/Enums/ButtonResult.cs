namespace AvaloniaTools.Enums
{
    public enum ButtonResult
    {
        Ok = 0,
        Yes = 1,
        No = 2,
        Abort = No | Yes, // 0x00000003
        Cancel = 4,
        None = Cancel | Yes, // 0x00000005
    }
}