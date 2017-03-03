using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillimanMath
{
    public static class Basic
    {
        public static double CalcMedian(List<double> ScenarioData)
        {                     
            ScenarioData.Sort();            
            return ScenarioData[(int)ScenarioData.Count / 2];
        }



    }
}
