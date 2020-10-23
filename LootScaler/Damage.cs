using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LootScaler
{
    public class Damage
    {
        public int min;
        public int max;
        public int type;

        public Damage(int _min, int _max, int _type)
        {
            min = _min;
            max = _max;
            type = _type;
        }
    }
}
