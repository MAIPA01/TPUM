using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Presentation.ViewModel
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
    }
}
