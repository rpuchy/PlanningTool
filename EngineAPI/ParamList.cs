using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EngineAPI
{
    public class ParamList : ObservableCollection<Parameter>
    {    
        public Parameter this[string name]
        {
            get
            {
                return this.Single(x => (((Parameter)x).Name == name) || (((Parameter)x).Name == EngineObject.GetAlternate(name)));                
            }
            set
            {
                try
                {
                    var t = this.Single(x => (((Parameter)x).Name == name) || (((Parameter)x).Name == EngineObject.GetAlternate(name)));
                    ((Parameter)t).Value = value.ToString();                                        
                }
                catch (Exception e)
                {
                    throw new Exception("Parameter : ");
                }                
            }
        }
        
                
    }
}
