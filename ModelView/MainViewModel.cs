
using Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModelView
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _textValue = "T";
        public string TextValue {
            get
            {
                return _textValue;
            }
            set
            {
                _textValue = value;
                OnPropertyChanged(nameof(TextValue));
            }
        }

        public ICommand ChangeTextCommand { get; set; }

        public int Counter { get; set; } = 0;

        public MainViewModel()
        {
            ChangeTextCommand = new CustomCommand(ExecuteChangeText, CanExecuteChangeText);
        }

        private void ExecuteChangeText(object? parameter)
        {
            Counter += 1;
            TextValue = "Clicked " + Counter + " Times";
        }

        private bool CanExecuteChangeText(object? parameter)
        {
            return true;
        }

        protected void OnPropertyChanged(string value)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(value));
        }
    }
}
