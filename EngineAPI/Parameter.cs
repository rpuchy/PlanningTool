using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;




namespace EngineAPI
{
    public class Parameter : DependencyObject, INotifyPropertyChanged
    {
        public Parameter()
        {

        }


        public  bool Equals(object obj)
        {
            if (obj == null) return false;
            Parameter objAsPart = obj as Parameter;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }

        public bool Equals(Parameter other)
        {
            if (other == null) return false;
            return (this.Name==other.Name && this.Value==other.Value);
        }


        public static readonly DependencyProperty _name =
        DependencyProperty.Register("Name", typeof(string),
        typeof(Parameter), new UIPropertyMetadata(null));

        public string Name
        {
            get { return (string)GetValue(_name); }
            set { SetValue(_name, value); }
        }

        public static readonly DependencyProperty _value =
        DependencyProperty.Register("Value", typeof(string),
        typeof(Parameter), new UIPropertyMetadata(OnPropertyChanged));

        public object Value
        {
            get { return (object)GetValue(_value); }
            set { SetValue(_value, value.ToString()); NotifyPropertyChanged("Value"); }
        }

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((Parameter)sender).NotifyPropertyChanged("Value");            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string Obj)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(Obj));
            }
        }

    }
}
