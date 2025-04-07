using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows;
using System.Windows.Controls;
using TPUM.Client.Presentation.ViewModel;

namespace TPUM.Client.Presentation.View.Controls
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    public partial class HeaterInfo : UserControl
    {
        public static readonly DependencyProperty HeaterIdProperty =
            DependencyProperty.Register(nameof(HeaterId), typeof(long), typeof(HeaterInfo));
        public long HeaterId
        {
            get => (long)GetValue(HeaterIdProperty);
            set => SetValue(HeaterIdProperty, value);
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(nameof(Position), typeof(IPosition), typeof(HeaterInfo));
        public IPosition Position
        {
            get => (IPosition)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public static readonly DependencyProperty IsOnProperty =
            DependencyProperty.Register(nameof(IsOn), typeof(bool), typeof(HeaterInfo));
        public bool IsOn
        {
            get => (bool)GetValue(IsOnProperty);
            set => SetValue(IsOnProperty, value);
        }

        public static readonly DependencyProperty TemperatureProperty =
            DependencyProperty.Register(nameof(Temperature), typeof(float), typeof(HeaterInfo));
        public float Temperature
        {
            get => (float)GetValue(TemperatureProperty);
            set => SetValue(TemperatureProperty, value);
        }

        public static readonly DependencyProperty TurnTextProperty =
            DependencyProperty.Register(nameof(TurnText), typeof(string), typeof(HeaterInfo));

        public string TurnText
        {
            get => (string)GetValue(TurnTextProperty);
            set => SetValue(TurnTextProperty, value);
        }

        public static readonly DependencyProperty TurnCommandProperty =
            DependencyProperty.Register(nameof(TurnCommand), typeof(ICommand), typeof(HeaterInfo));
        public ICommand TurnCommand
        {
            get => (ICommand)GetValue(TurnCommandProperty);
            set => SetValue(TurnCommandProperty, value);
        }

        public static readonly DependencyProperty MoveCommandProperty =
            DependencyProperty.Register(nameof(MoveCommand), typeof(ICommand), typeof(HeaterInfo));
        public ICommand MoveCommand
        {
            get => (ICommand)GetValue(MoveCommandProperty);
            set => SetValue(MoveCommandProperty, value);
        }

        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register(nameof(RemoveCommand), typeof(ICommand), typeof(HeaterInfo));

        public ICommand RemoveCommand
        {
            get => (ICommand)GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        public HeaterInfo()
        {
            InitializeComponent();
        }
    }
}