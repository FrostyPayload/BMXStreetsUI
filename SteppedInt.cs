using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BmxStreetsUI
{
    public class SteppedInt : CustomMenuOption
    {
        public float max, min, value;
        public override float GetMax()
        {
            return max;
        }
        public override float GetMin()
        {
            return min;
        }
    }

}
