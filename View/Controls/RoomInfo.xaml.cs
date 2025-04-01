using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TPUM.Presentation.View.Controls
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    public partial class RoomInfo : UserControl
    {
        public static readonly DependencyProperty RoomIdProperty =
            DependencyProperty.Register(nameof(RoomId), typeof(long), typeof(RoomInfo));
        public long RoomId
        {
            get => (long)GetValue(RoomIdProperty);
            set => SetValue(RoomIdProperty, value);
        }

        public static readonly DependencyProperty RoomNameProperty =
            DependencyProperty.Register(nameof(RoomName), typeof(string), typeof(RoomInfo));
        public string RoomName
        {
            get => (GetValue(RoomNameProperty).ToString() ?? "");
            set => SetValue(RoomNameProperty, value);
        }

        public static readonly DependencyProperty RoomWidthProperty =
            DependencyProperty.Register(nameof(RoomWidth), typeof(float), typeof(RoomInfo));
        public float RoomWidth
        {
            get => (float)GetValue(RoomWidthProperty);
            set => SetValue(RoomWidthProperty, value);
        }

        public static readonly DependencyProperty RoomHeightProperty =
            DependencyProperty.Register(nameof(RoomHeight), typeof(float), typeof(RoomInfo));
        public float RoomHeight
        {
            get => (float)GetValue(RoomHeightProperty);
            set => SetValue(RoomHeightProperty, value);
        }

        public static readonly DependencyProperty HeatersCountProperty =
            DependencyProperty.Register(nameof(HeatersCount), typeof(int), typeof(RoomInfo));
        public int HeatersCount
        {
            get => (int)GetValue(HeatersCountProperty);
            set => SetValue(HeatersCountProperty, value);
        }

        public static readonly DependencyProperty SensorsCountProperty =
            DependencyProperty.Register(nameof(SensorsCount), typeof(int), typeof(RoomInfo));
        public int SensorsCount
        {
            get => (int)GetValue(SensorsCountProperty);
            set => SetValue(SensorsCountProperty, value);
        }

        public static readonly DependencyProperty TemperatureProperty =
            DependencyProperty.Register(nameof(Temperature), typeof(int), typeof(RoomInfo));
        public int Temperature
        {
            get => (int)GetValue(TemperatureProperty);
            set => SetValue(TemperatureProperty, value);
        }

        public static readonly DependencyProperty ShowCommandProperty =
            DependencyProperty.Register(nameof(ShowCommand), typeof(ICommand), typeof(RoomInfo));
        public ICommand ShowCommand
        {
            get => (ICommand)GetValue(ShowCommandProperty);
            set => SetValue(ShowCommandProperty, value);
        }

        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register(nameof(RemoveCommand), typeof(ICommand), typeof(RoomInfo));
        public ICommand RemoveCommand
        {
            get => (ICommand)GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        public RoomInfo()
        {
            InitializeComponent();
        }
    }
}
