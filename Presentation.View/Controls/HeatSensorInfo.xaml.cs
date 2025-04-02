using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows;
using System.Windows.Controls;
using TPUM.Presentation.ViewModel;

namespace TPUM.Presentation.View.Controls
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    public partial class HeatSensorInfo : UserControl
    {
        public static readonly DependencyProperty HeatSensorIdProperty =
            DependencyProperty.Register(nameof(HeatSensorId), typeof(long), typeof(HeatSensorInfo));
        public long HeatSensorId
        {
            get => (long)GetValue(HeatSensorIdProperty);
            set => SetValue(HeatSensorIdProperty, value);
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(nameof(Position), typeof(IPosition), typeof(HeatSensorInfo));
        public IPosition Position
        {
            get => (IPosition)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public static readonly DependencyProperty TemperatureProperty =
            DependencyProperty.Register(nameof(Temperature), typeof(int), typeof(HeatSensorInfo));
        public int Temperature
        {
            get => (int)GetValue(TemperatureProperty);
            set => SetValue(TemperatureProperty, value);
        }

        public static readonly DependencyProperty MoveCommandProperty =
            DependencyProperty.Register(nameof(MoveCommand), typeof(ViewModel.ICommand), typeof(HeatSensorInfo));
        public ViewModel.ICommand MoveCommand
        {
            get => (ViewModel.ICommand)GetValue(MoveCommandProperty);
            set => SetValue(MoveCommandProperty, value);
        }

        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register(nameof(RemoveCommand), typeof(ViewModel.ICommand), typeof(HeatSensorInfo));
        public ViewModel.ICommand RemoveCommand
        {
            get => (ViewModel.ICommand)GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        public HeatSensorInfo()
        {
            InitializeComponent();
        }
    }
}