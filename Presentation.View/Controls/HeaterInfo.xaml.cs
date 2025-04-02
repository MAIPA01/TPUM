using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using TPUM.Presentation.ViewModel;

namespace TPUM.Presentation.View.Controls
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
            DependencyProperty.Register(nameof(TurnCommand), typeof(ViewModel.ICommand), typeof(HeaterInfo));
        public ViewModel.ICommand TurnCommand
        {
            get => (ViewModel.ICommand)GetValue(TurnCommandProperty);
            set => SetValue(TurnCommandProperty, value);
        }

        public static readonly DependencyProperty MoveCommandProperty =
            DependencyProperty.Register(nameof(MoveCommand), typeof(ViewModel.ICommand), typeof(HeaterInfo));
        public ViewModel.ICommand MoveCommand
        {
            get => (ViewModel.ICommand)GetValue(MoveCommandProperty);
            set => SetValue(MoveCommandProperty, value);
        }

        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register(nameof(RemoveCommand), typeof(ViewModel.ICommand), typeof(HeaterInfo));

        public event PropertyChangedEventHandler? PropertyChanged;

        public ViewModel.ICommand RemoveCommand
        {
            get => (ViewModel.ICommand)GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        public HeaterInfo()
        {
            InitializeComponent();
        }
    }
}
