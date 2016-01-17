using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace WpfMk2._0
{
    public class slaveList : INotifyPropertyChanged 
    {
        private string _ip;
        private string _machine;
        private string _condition;
        private string _function;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        public slaveList(string ip, string machine, string condition, string function)
        {
            _ip = ip;
            _machine = machine;
            _condition = condition;
            _function = function;
        }

        public string FUNCTION
        {
            get { return _function; }
            set { _function = value; OnPropertyChanged(new PropertyChangedEventArgs("FUNCTION")); }
        }

        public string CONDITION
        {
            get { return _condition; }
            set { _condition = value; OnPropertyChanged(new PropertyChangedEventArgs("CONDITION")); }
        }

        public string MACHINE
        {
            get { return _machine; }
            set { _machine = value; OnPropertyChanged(new PropertyChangedEventArgs("MACHINE")); }
        }

        public string IP
        {
            get { return _ip; }
            set { _ip = value; OnPropertyChanged(new PropertyChangedEventArgs("IP")); }
        }


    }


}
