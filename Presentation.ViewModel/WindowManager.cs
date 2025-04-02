using System.Diagnostics;
using System.Printing;
using System.Windows;

namespace TPUM.Presentation.ViewModel
{
    internal static class WindowManager
    {
        public static void OpenSubWindow(Type windowType)
        {
            var win = Activator.CreateInstance(windowType);
            (win as Window)?.ShowDialog();
        }

        public static void CloseLastSubWindow()
        {
            var win = Application.Current.Windows
                .OfType<Window>()
                .LastOrDefault(w => w.Visibility == Visibility.Visible);
            win?.Close();
        }

        public static bool MakeYesNoWindow(string message, string title = "Yes No Message")
        {
            var dialogResult = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            switch (dialogResult)
            {
                case MessageBoxResult.Yes:
                    return true;
                case MessageBoxResult.No:
                    return false;
                case MessageBoxResult.None:
                case MessageBoxResult.OK:
                case MessageBoxResult.Cancel:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dialogResult));
            }

            return false;
        }
    }
}