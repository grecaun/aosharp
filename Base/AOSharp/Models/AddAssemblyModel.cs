using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;

namespace AOSharp.Models
{
    public class AddAssemblyModel : INotifyPropertyChanged
    {
        public string[] _dllPath;
        public string _dllInfo;

        public string[] DllPath
        {
            get { return _dllPath; }
            set
            {
                _dllPath = value;
                DllInfo = value.Length == 1 ? value[0] : value.Length > 1 ? $"{value.Length} files." : null;
            }
        }

        public string DllInfo
        {
            get { return _dllInfo; }
            set
            {
                _dllInfo = value;
                OnPropertyChanged("DllInfo");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
