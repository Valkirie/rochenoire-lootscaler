using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LootScaler
{
    public class socketBonus
    {
        public int id;
        public int family;
        public double value;
        public string text;

        public socketBonus()
        {
        }

        public socketBonus(socketBonus socket)
        {
            id = socket.id;
            family = socket.family;
            text = socket.text;
            value = socket.value;
        }
    }
}
