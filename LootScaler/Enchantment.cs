using System.Collections.Generic;

namespace LootScaler
{
    public class Enchantment
    {
        public int id;
        public int family;
        public double ilevel;
        public string name;
        public List<double> value = new List<double>();
        public double chance;

        public Enchantment()
        {
        }

        public Enchantment(Enchantment enchantment)
        {
            id = enchantment.id;
            family = enchantment.family;
            name = enchantment.name;
            value = enchantment.value;

            // chance has to be defined when binded on an item only
            // chance = enchantment.chance;
        }

        public void SetChance(double _chance)
        {
            chance = _chance;
        }
    }
}
