using System.Windows;
using TPUM.Client.Presentation.ViewModel;

namespace TPUM.Client.Presentation.View.Triggers
{
    public static class OnLoadBehavior
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(OnLoadBehavior),
                new PropertyMetadata(null, OnCommandChanged));

        public static void SetCommand(DependencyObject obj, ICommand value)
            => obj.SetValue(CommandProperty, value);

        public static ICommand GetCommand(DependencyObject obj)
            => (ICommand)obj.GetValue(CommandProperty);

        public static readonly DependencyProperty ParameterProperty =
            DependencyProperty.RegisterAttached("Parameter", typeof(object), typeof(OnLoadBehavior),
                new PropertyMetadata(null));

        public static void SetParameter(DependencyObject obj, object value)
            => obj.SetValue(ParameterProperty, value);

        public static object GetParameter(DependencyObject obj)
            => obj.GetValue(ParameterProperty);

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement fe)
            {
                fe.Loaded += (s, _) =>
                {
                    var cmd = GetCommand(fe);
                    var param = GetParameter(fe);
                    if (cmd?.CanExecute(param) == true)
                        cmd.Execute(param);
                };
            }
        }
    }
}
