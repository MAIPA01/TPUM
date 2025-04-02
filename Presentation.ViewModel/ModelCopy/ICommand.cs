using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPUM.Presentation.ViewModel
{
    public interface ICommand : System.Windows.Input.ICommand
    {
        void OnCanExecuteChanged();
    }
}
