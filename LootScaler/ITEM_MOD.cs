using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LootScaler
{
    public class ITEM_MOD
    {
        public string name;
        public double weigth;
        public double stat60;

        public ITEM_MOD(string _name, double _weigth)
        {
            name = _name;
            weigth = _weigth;
            stat60 = 100;
        }

        public ITEM_MOD(string _name, double _weigth, double _stat60)
        {
            name = _name;
            weigth = _weigth;
            stat60 = _stat60;
        }
    }
}
