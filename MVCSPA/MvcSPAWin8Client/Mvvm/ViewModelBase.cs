using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MvcSPAWin8Client.Mvvm
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public bool IsDesignMode { get { return Windows.ApplicationModel.DesignMode.DesignModeEnabled; } }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
