using System;
using System.Collections.Generic;
using System.Linq;

namespace LootScaler
{
    public class Item : IDisposable
    {
        public int entry;
        public int patch;
        public int parent_entry;
        public int _class;
        public int subclass;
        public int subclass_new;
        public string name;
        public List<string> name_split = new List<string>();
        public int displayid;
        public int Quality;
        public int Flags;
        public int BuyCount;
        public int BuyPrice;
        public int SellPrice;
        public int InventoryType;
        public int AllowableClass;
        public int AllowableRace;
        public int sItemLevel;
        public int ItemLevel;
        public int RequiredLevel;
        public int RequiredSkill;
        public int RequiredSkillRank;
        public int requiredspell;
        public int requiredhonorrank;
        public int RequiredCityRank;
        public int RequiredReputationFaction;
        public int RequiredReputationRank;
        public int maxcount;
        public int stackable;
        public int ContainerSlots;
        public int loopmin;
        public int loopmax;

        public int stat_type1;
        public int stat_value1;
        public int stat_type2;
        public int stat_value2;
        public int stat_type3;
        public int stat_value3;
        public int stat_type4;
        public int stat_value4;
        public int stat_type5;
        public int stat_value5;
        public int stat_type6;
        public int stat_value6;
        public int stat_type7;
        public int stat_value7;
        public int stat_type8;
        public int stat_value8;
        public int stat_type9;
        public int stat_value9;
        public int stat_type10;
        public int stat_value10;

        public int armor;
        public int bonus_armor;
        public int holy_res;
        public int fire_res;
        public int nature_res;
        public int frost_res;
        public int shadow_res;
        public int arcane_res;
        public int delay;
        public int ammo_type;
        public float RangedModRange;
        public int bonding;
        public string description;
        public int PageText;
        public int LanguageID;
        public int PageMaterial;
        public int startquest;
        public int lockid;
        public int Material;
        public int sheath;
        public int RandomProperty;

        public int RandomSuffix; // TBC
        public int TotemCategory; // TBC
        public int socketColor_1; // TBC
        public int socketContent_1; // TBC
        public int socketColor_2; // TBC
        public int socketContent_2; // TBC
        public int socketColor_3; // TBC
        public int socketContent_3; // TBC
        public int GemProperties; // TBC
        public int RequiredDisenchantSkill; // TBC
        public float ArmorDamageModifier; // TBC

        public int block;
        public int itemset;
        public int MaxDurability;
        public int area;
        public int Map;
        public int BagFamily;
        public string ScriptName;
        public int DisenchantID;
        public int FoodType;
        public int minMoneyLoot;
        public int maxMoneyLoot;
        public int Duration;
        public int ExtraFlags;

        public Flagsenum Flags2;

        public string description_loc2 = null;
        public string name_loc2 = null;

        public socketBonus socketBonus;
        public socketBonus socketBonus_new;

        string outputString;

        public List<Spell> spells = new List<Spell>() { null, null, null, null, null };
        public List<Spell> spells_ori = new List<Spell>() { null, null, null, null, null };
        public List<Enchantment> enchantments_ori = new List<Enchantment>();
        public List<Enchantment> enchantments_new = new List<Enchantment>();

        public List<int> pathing_stat = new List<int>(new int[ITEM_MOD_MAX]);
        public List<int> pathing_spell = new List<int>(new int[SPELL_MOD_MAX]);

        public List<Damage> damages = new List<Damage>() { null, null, null, null, null };
        public List<Damage> damages_ori = new List<Damage>() { null, null, null, null, null };

        public Dictionary<int, List<Item>> wip_item_list = new Dictionary<int, List<Item>>();

        public const int ITEM_MOD_MANA = 0;
        public const int ITEM_MOD_HEALTH = 1;
        public const int ITEM_MOD_AGILITY = 3;
        public const int ITEM_MOD_STRENGTH = 4;
        public const int ITEM_MOD_INTELLECT = 5;
        public const int ITEM_MOD_SPIRIT = 6;
        public const int ITEM_MOD_STAMINA = 7;
        public const int ITEM_MOD_DEFENSE_SKILL_RATING = 12;
        public const int ITEM_MOD_DODGE_RATING = 13;
        public const int ITEM_MOD_PARRY_RATING = 14;
        public const int ITEM_MOD_BLOCK_RATING = 15;
        public const int ITEM_MOD_HIT_MELEE_RATING = 16;
        public const int ITEM_MOD_HIT_RANGED_RATING = 17;
        public const int ITEM_MOD_HIT_SPELL_RATING = 18;
        public const int ITEM_MOD_CRIT_MELEE_RATING = 19;
        public const int ITEM_MOD_CRIT_RANGED_RATING = 20;
        public const int ITEM_MOD_CRIT_SPELL_RATING = 21;
        public const int ITEM_MOD_HIT_TAKEN_MELEE_RATING = 22;
        public const int ITEM_MOD_HIT_TAKEN_RANGED_RATING = 23;
        public const int ITEM_MOD_HIT_TAKEN_SPELL_RATING = 24;
        public const int ITEM_MOD_CRIT_TAKEN_MELEE_RATING = 25;
        public const int ITEM_MOD_CRIT_TAKEN_RANGED_RATING = 26;
        public const int ITEM_MOD_CRIT_TAKEN_SPELL_RATING = 27;
        public const int ITEM_MOD_HASTE_MELEE_RATING = 28;
        public const int ITEM_MOD_HASTE_RANGED_RATING = 29;
        public const int ITEM_MOD_HASTE_SPELL_RATING = 30;
        public const int ITEM_MOD_HIT_RATING = 31;
        public const int ITEM_MOD_CRIT_RATING = 32;
        public const int ITEM_MOD_HIT_TAKEN_RATING = 33;
        public const int ITEM_MOD_CRIT_TAKEN_RATING = 34;
        public const int ITEM_MOD_RESILIENCE_RATING = 35;
        public const int ITEM_MOD_HASTE_RATING = 36;
        public const int ITEM_MOD_EXPERTISE_RATING = 37;

        public const int ITEM_MOD_LAST = 40;

        public const int ITEM_MOD_MAX = 50;

        public const int ITEM_MOD_CASTER_WEAP = 41;  // custom stat to know if the weapon is a caster/healer/druid weapon
        public const int ITEM_MOD_HEALER_WEAP = 42;  // custom stat to know if the weapon is a caster/healer/druid weapon
        public const int ITEM_MOD_DRUID_WEAP = 43;  // custom stat to know if the weapon is a caster/healer/druid weapon

        public const int SPELL_MOD_MIN = 0;
        public const int SPELL_MOD_MAX = 50;

        public const int SPELL_MOD_TYPES_HANDLED = 0;
        public const int SPELL_MOD_TYPES_DAMAGE = 1;
        public const int SPELL_MOD_TYPES_RESIST = 2;
        public const int SPELL_MOD_TYPES_MAX = 3;

        private static List<int> list_fish = new List<int>() { 787, 1326, 2682, 2683, 4592, 4593, 4594, 5476, 5525, 5527, 6290, 6887, 8364, 8957, 12216, 13893, 13927, 13930, 13932, 13934, 13935, 21072, 21217, 21552, 27661, 27858, 29452, 33048, 33053, 33867 };
        private static List<int> list_cheese = new List<int>() { 414, 422, 1707, 2070, 3927, 8932, 17406, 27857, 29448, 30458 };
        private static List<int> list_bread = new List<int>() { 724, 1113, 1114, 1487, 4540, 4541, 4542, 4544, 4601, 5349, 8075, 8076, 8950, 13724, 17197, 17407, 19301, 19696, 20857, 21030, 21254, 22019, 22895, 23160, 23756, 24072, 27855, 28486, 29394, 29449, 30816, 34062 };
        private static List<int> list_meat = new List<int>() { 117, 1017, 2287, 2679, 2680, 2681, 2684, 2685, 2687, 2888, 3220, 3662, 3726, 3727, 3728, 3770, 3771, 4457, 4599, 5472, 5474, 5480, 6890, 7097, 8952, 11444, 12209, 12224, 13851, 13934, 17119, 17222, 17407, 18045, 19224, 19304, 19995, 21023, 21215, 27635, 27659, 27660, 27854, 29292, 29451, 30610, 31672, 33866, 35563, 35565 };
        private static List<int> list_fruit = new List<int>() { 1205, 4536, 4537, 4538, 4539, 4601, 4602, 4656, 5057, 34411, 8953, 11415, 11584, 13810, 16168, 19994, 20031, 20516, 21215, 24009, 27856, 29393, 29450 };

        private static List<string> listname_fish = new List<String>() { "sunfish", "sagefish", "fillet", "filet", "mightfish", "crab", "clamette", "mackerel", "snapper", "catfish", "cod", "yellowtail", "halibut", "carp", "trout", "icefin", "salmon", "clams", "smallfish", "bloodfin" };
        private static List<string> listname_cheese = new List<String>() { "muenster", "bleu", "sharp", "cheese", "mild", "cheesewheel", "brie", "cheddar", "swiss" };
        private static List<string> listname_bread = new List<String>() { "biscuit", "bread", "cornbread", "pie", "grainbeard", "bagel", "flatbread", "cookie", "muffin", "rye", "roll", "croissant", "pumpernickel", "sourdough" };
        private static List<string> listname_meat = new List<String>() { "beefstick", "shortribs", "shortrib", "burger", "kabob", "bacon", "boar", "jerky", "meat", "chop", "shank", "steak", "quail", "venison", "ribs", "caribou", "wing", "wings", "sausage" };
        private static List<string> listname_fruit = new List<String>() { "grapes", "grape", "berries", "mango", "sunfruit", "fruit", "apple", "banana", "melon", "watermelon", "peach", "pumpkin", "plantains", "berries", "grapes", "snowplum" };

        public bool IsFood()
        {
            return !foodtype.Equals("none");
        }

        public string GetFood()
        {
            return foodtype;
        }

        private string foodtype = "none";
        public void SetFood()
        {
            // fast
            if (list_fish.Contains(entry))
                foodtype = "fish";
            else if (list_cheese.Contains(entry))
                foodtype = "cheese";
            else if (list_bread.Contains(entry))
                foodtype = "bread";
            else if (list_meat.Contains(entry))
                foodtype = "meat";
            else if (list_fruit.Contains(entry))
                foodtype = "fruit";

            // slow
            foreach (string name in name_split)
            {
                if (listname_fish.Contains(name))
                    foodtype = "fish";
                if (listname_cheese.Contains(name))
                    foodtype = "cheese";
                if (listname_bread.Contains(name))
                    foodtype = "bread";
                if (listname_meat.Contains(name))
                    foodtype = "meat";
                if (listname_fruit.Contains(name))
                    foodtype = "fruit";
            }
        }

        public int GetIlvlFromLvl(int _Quality, int pLevel, int BonusQuality = 0)
        {
            int iLevel;

            if (pLevel < 61)
                iLevel = pLevel + 5 + (ItemLevel > 92 ? 0 : (pLevel == 60 ? Math.Max(ItemLevel - 65, 0) : 0));
            else
                iLevel = _Quality < (int)ItemQualities.ITEM_QUALITY_RARE ? (pLevel - 60) * 3 + 90 : (pLevel - 60) * 3 + 90 + (pLevel == 70 ? Math.Max(ItemLevel - 115, 0) : 0);

            // BonusQuality effect on iLevel
            // iLevel += BonusQuality * 5;

            return iLevel;
        }

        public static List<Dictionary<int, ITEM_MOD>> spell_types = new List<Dictionary<int, ITEM_MOD>>
        {
            new Dictionary<int,ITEM_MOD>() // spell type 0 (0 : 29) : Misc / Handled
            {
                { 0, new ITEM_MOD("HEAL", 1) },
                { 1, new ITEM_MOD("ATTACKPWR", 0.5) },
                { 2, new ITEM_MOD("ATTACKPWR_RANGED", 0.4) },
                { 3, new ITEM_MOD("SPELLDMG", 0.86) },
                { 4, new ITEM_MOD("MP5", 2.5) },
                { 5, new ITEM_MOD("HP5", 2.5) },
                { 6, new ITEM_MOD("DEFENSE", 1) },       // ITEM_MOD_DEFENSE_SKILL_RATING (1.2 pour boucliers)
                { 7, new ITEM_MOD("BLOCK", 1, 5) },         // ITEM_MOD_BLOCK_RATING 
                { 8, new ITEM_MOD("PARRY", 1, 15) },         // ITEM_MOD_PARRY_RATING 
                { 9, new ITEM_MOD("DODGE", 1, 12) },         // ITEM_MOD_DODGE_RATING
                { 10, new ITEM_MOD("CRIT", 1, 14) },         // ITEM_MOD_CRIT_RATING
                { 11, new ITEM_MOD("SPELLCRIT", 1, 14) },    // ITEM_MOD_CRIT_SPELL_RATING 
                { 12, new ITEM_MOD("HIT", 1, 10) },          // ITEM_MOD_HIT_RATING 
                { 13, new ITEM_MOD("SPELLHIT", 1, 8) },     // ITEM_MOD_HIT_SPELL_RATING ?
                { 14, new ITEM_MOD("ARMOR", 0.07) },
                { 15, new ITEM_MOD("SPELLPENETRATION", 0.80) },
                { 16, new ITEM_MOD("WEAPONDAMAGE", 1) },
                { 17, new ITEM_MOD("EXPERTISE", 1, 10) },    // ITEM_MOD_EXPERTISE_RATING : ITEMSETS
                { 18, new ITEM_MOD("ARMORPENETRATION", 0.14) },
                { 19, new ITEM_MOD("ATTACKPWR_FERAL", 0.5) },
                { 20, new ITEM_MOD("HASTE", 0.5, 10) }
            },
            new Dictionary<int,ITEM_MOD>() // spell type 1 (30 : 39) : Damage
            {
                { 30, new ITEM_MOD("FIRE", 1) },
                { 31, new ITEM_MOD("FROST", 1) },
                { 32, new ITEM_MOD("SHADOW", 1) },
                { 33, new ITEM_MOD("HOLY", 1) },
                { 34, new ITEM_MOD("NATURE", 1) },
                { 35, new ITEM_MOD("ARCANE", 1) }
            },
            new Dictionary<int,ITEM_MOD>() // spell type 2 (40 : 59) : Resist
            {
                { 40, new ITEM_MOD("ALL", 1) },
                { 41, new ITEM_MOD("FIRE", 1) },
                { 42, new ITEM_MOD("FROST", 1) },
                { 43, new ITEM_MOD("SHADOW", 1) },
                { 44, new ITEM_MOD("HOLY", 1) },
                { 45, new ITEM_MOD("NATURE", 1) },
                { 46, new ITEM_MOD("ARCANE", 1) }
            }
        };

        public int IsBearWeapon(int IDitem)
        {
            List<int> BearWeapList = new List<int>() { 9452, 1317, 15259, 3227, 880, 9603, 12969, 12969, 11921, 12776, 9408, 1722, 12251, 943, 18420, 19944, 19358, 12252, 18376, 13167, 19357 };

            if (BearWeapList.Contains(IDitem))
                return 1;
            else
                return 0;
        }

        //Pathing Main Function
        public void GeneratePathing()
        {
            setPStat(ITEM_MOD_DRUID_WEAP, IsBearWeapon(entry));

            int ArmorType = GetArmorType();
            int INT = GetItemModValue(ITEM_MOD_INTELLECT);
            int SPI = GetItemModValue(ITEM_MOD_SPIRIT);
            int AGI = GetItemModValue(ITEM_MOD_AGILITY);
            int STA = GetItemModValue(ITEM_MOD_STAMINA);
            int STR = GetItemModValue(ITEM_MOD_STRENGTH);

            int mods_value = 0;
            for (int i = Item.ITEM_MOD_DEFENSE_SKILL_RATING; i < Item.ITEM_MOD_MAX; i++)
                mods_value += GetItemModValue(i);

            int spells_value = 0;
            for (int MOD_TYPE = 0; MOD_TYPE < SPELL_MOD_TYPES_RESIST; MOD_TYPE++)
                for (int j = 0; j < spell_types[MOD_TYPE].Count; j++)
                {
                    string spell_type = Item.spell_types[MOD_TYPE].ElementAt(j).Value.name;
                    spells_value += GetSpellValue(spell_type, MOD_TYPE);
                }

            if ((bonus_armor / (armor + 1) > 0.1) || (mods_value + spells_value) > 0)
            {
                if (STR + STA + AGI + INT + SPI == 0)  //Wich stat to increase if none are on an item
                {
                    setPStat(ITEM_MOD_STAMINA, 1);
                    if ((GetSpellValue("BLOCK", 0) + GetItemModValue(ITEM_MOD_BLOCK_RATING)) > 0)
                        setPStat(ITEM_MOD_INTELLECT, 1);
                    if ((GetSpellValue("DODGE", 0) + GetItemModValue(ITEM_MOD_DODGE_RATING)) > 0)
                        setPStat(ITEM_MOD_AGILITY, 1);
                    if ((GetSpellValue("CRIT", 0) + GetItemModValue(ITEM_MOD_CRIT_MELEE_RATING)) > 0)
                        setPStat(ITEM_MOD_AGILITY, 1);
                    if ((GetSpellValue("SPELLCRIT", 0) + GetItemModValue(ITEM_MOD_CRIT_SPELL_RATING)) > 0)
                    {
                        setPStat(ITEM_MOD_INTELLECT, 1);
                        setPStat(ITEM_MOD_SPIRIT, 1);
                    }
                    if ((GetSpellValue("HIT", 0) + GetItemModValue(ITEM_MOD_HIT_MELEE_RATING)) > 0)
                        setPStat(ITEM_MOD_INTELLECT, 1);
                    if ((GetSpellValue("SPELLHIT", 0) + GetItemModValue(ITEM_MOD_HIT_SPELL_RATING)) > 0)
                        setPStat(ITEM_MOD_INTELLECT, 1);

                    if (GetSlotMod() == 5 && ((GetSpellValue("BLOCK", 0) + GetItemModValue(ITEM_MOD_BLOCK_RATING)) + (GetSpellValue("DODGE", 0) + GetItemModValue(ITEM_MOD_DODGE_RATING)) + (GetSpellValue("CRIT", 0) + GetItemModValue(ITEM_MOD_CRIT_MELEE_RATING)) + (GetSpellValue("SPELLCRIT", 0) + GetItemModValue(ITEM_MOD_CRIT_SPELL_RATING)) + (GetSpellValue("HIT", 0) + GetItemModValue(ITEM_MOD_HIT_MELEE_RATING)) + (GetSpellValue("SPELLHIT", 0) + GetItemModValue(ITEM_MOD_HIT_SPELL_RATING))) == 0)
                    {
                        setPSpell("ATTACKPWR_RANGED", ItemLevel % 5 == 0 ? 1 : 0);
                        setPStat(ITEM_MOD_INTELLECT, ItemLevel % 5 == 1 ? 1 : 0);
                        setPStat(ITEM_MOD_STAMINA, ItemLevel % 5 == 2 ? 1 : 0);
                        setPStat(ITEM_MOD_AGILITY, ItemLevel % 5 == 3 ? 1 : 0);
                        setPStat(ITEM_MOD_DEFENSE_SKILL_RATING, ItemLevel % 5 == 4 ? 1 : 0);
                    }
                    else if (GetSlotMod() == 7)   // 
                    {
                        setPStat(ITEM_MOD_STAMINA, 0);
                        setPSpell("SPELLDMG", ItemLevel % 4 == 0 ? 1 : 0);
                        setPSpell("SPELLDMG", ItemLevel % 4 == 1 ? 1 : 0);
                        setPSpell("MP5", ItemLevel % 4 == 2 ? 1 : 0);
                        setPSpell("HEAL", ItemLevel % 4 == 3 ? 1 : 0);
                        //setPSpell("spellcrit = ItemLevel % 5 == 3 ? 1 : 0;
                        //setPSpell("spellhit = ItemLevel % 5 == 4 ? 1 : 0;
                    }
                }
            }
            else if (STR + STA + AGI + INT + SPI == 0)
            {
                setPStat(ITEM_MOD_STAMINA, 1);
                if ((GetSpellValue("BLOCK", 0) + GetItemModValue(ITEM_MOD_BLOCK_RATING)) > 0)
                    setPStat(ITEM_MOD_INTELLECT, 1);
                if ((GetSpellValue("DODGE", 0) + GetItemModValue(ITEM_MOD_DODGE_RATING)) > 0)
                    setPStat(ITEM_MOD_AGILITY, 1);
                if ((GetSpellValue("CRIT", 0) + GetItemModValue(ITEM_MOD_CRIT_MELEE_RATING)) > 0)
                    setPStat(ITEM_MOD_AGILITY, 1);
                if ((GetSpellValue("SPELLCRIT", 0) + GetItemModValue(ITEM_MOD_CRIT_SPELL_RATING)) > 0)
                {
                    setPStat(ITEM_MOD_INTELLECT, 1);
                    setPStat(ITEM_MOD_SPIRIT, 1);
                }
                if ((GetSpellValue("HIT", 0) + GetItemModValue(ITEM_MOD_HIT_MELEE_RATING)) > 0)
                    setPStat(ITEM_MOD_INTELLECT, 1);
                if ((GetSpellValue("SPELLHIT", 0) + GetItemModValue(ITEM_MOD_HIT_SPELL_RATING)) > 0)
                    setPStat(ITEM_MOD_INTELLECT, 1);
                if (GetSlotMod() == 5 && ((GetSpellValue("BLOCK", 0) + GetItemModValue(ITEM_MOD_BLOCK_RATING)) + (GetSpellValue("DODGE", 0) + GetItemModValue(ITEM_MOD_DODGE_RATING)) + (GetSpellValue("CRIT", 0) + GetItemModValue(ITEM_MOD_CRIT_MELEE_RATING)) + (GetSpellValue("SPELLCRIT", 0) + GetItemModValue(ITEM_MOD_CRIT_SPELL_RATING)) + (GetSpellValue("HIT", 0) + GetItemModValue(ITEM_MOD_HIT_MELEE_RATING)) + (GetSpellValue("SPELLHIT", 0) + GetItemModValue(ITEM_MOD_HIT_SPELL_RATING))) == 0)
                {
                    setPSpell("ATTACKPWR_RANGED", ItemLevel % 5 == 0 ? 1 : 0);
                    setPStat(ITEM_MOD_INTELLECT, ItemLevel % 5 == 1 ? 1 : 0);
                    setPStat(ITEM_MOD_STAMINA, ItemLevel % 5 == 2 ? 1 : 0);
                    setPStat(ITEM_MOD_AGILITY, ItemLevel % 5 == 3 ? 1 : 0);
                    setPStat(ITEM_MOD_DEFENSE_SKILL_RATING, ItemLevel % 5 == 4 ? 1 : 0);
                }
                else if (GetSlotMod() == 7)
                {
                    setPStat(ITEM_MOD_STAMINA, 0);
                    setPSpell("SPELLDMG", ItemLevel % 4 == 0 ? 1 : 0);
                    setPSpell("SPELLDMG", ItemLevel % 4 == 1 ? 1 : 0);
                    setPSpell("MP5", ItemLevel % 4 == 2 ? 1 : 0);
                    setPSpell("HEAL", ItemLevel % 4 == 3 ? 1 : 0);
                    //setPSpell("spellcrit = ItemLevel % 5 == 3 ? 1 : 0;
                    //setPSpell("spellhit = ItemLevel % 5 == 4 ? 1 : 0;
                }
            }                                       //     ITEM_MOD_CASTER_WEAP  ITEM_MOD_HEALER_WEAP   ITEM_MOD_DRUID_WEAP  setPStat(ITEM_MOD_DRUID_WEAP, 1);
            else
            {
                if (((0.8 * Math.Max(INT, SPI) - Math.Max(STR, AGI)) > 0) || (GetSlotMod() == 7))     //Item caster
                {
                    bool g1 = (0.8 * INT - SPI) > 0;
                    bool g2 = (0.8 * SPI - INT) > 0;
                    bool g = !(g1 || g2);
                    if (STA == 0 && SPI == 0)
                    {
                        setPStat(ITEM_MOD_INTELLECT, -1);
                        setPStat(ITEM_MOD_STRENGTH, -1);
                        setPStat(ITEM_MOD_AGILITY, -1);
                        setPSpell("SPELLDMG", 1);
                        setPStat(ITEM_MOD_CASTER_WEAP, 1);
                    }  // cas M1
                    else if (STA == 0 && INT == 0)
                    {
                        setPStat(ITEM_MOD_SPIRIT, -1);
                        setPStat(ITEM_MOD_STRENGTH, -1);
                        setPStat(ITEM_MOD_AGILITY, -1);
                        setPSpell("HEAL", 1);
                        setPStat(ITEM_MOD_HEALER_WEAP, 1);
                    }  //cas M2
                    else if (STA == 0 && g1 && SPI > 0)
                    {
                        setPStat(ITEM_MOD_STRENGTH, -1);
                        setPStat(ITEM_MOD_AGILITY, -1);
                        setPStat(ITEM_MOD_SPIRIT, -1);
                        setPSpell("SPELLDMG", 1);
                        setPStat(ITEM_MOD_CASTER_WEAP, 1);
                        setPStat(ITEM_MOD_HIT_SPELL_RATING, 1);
                    } //cas M3
                    else if (STA == 0 && g2 && SPI > 0 && INT > 0)
                    {
                        setPStat(ITEM_MOD_INTELLECT, -1);
                        setPStat(ITEM_MOD_STRENGTH, -1);
                        setPStat(ITEM_MOD_AGILITY, -1);
                        setPSpell("HEAL", 1);
                        setPStat(ITEM_MOD_HEALER_WEAP, 1);
                        setPStat(ITEM_MOD_CRIT_SPELL_RATING, 1);
                    } //cas M4
                    else if (STA >= INT && INT > 0 && SPI == 0)
                    {
                        setPStat(ITEM_MOD_INTELLECT, -1);
                        setPStat(ITEM_MOD_STRENGTH, -1);
                        setPStat(ITEM_MOD_AGILITY, -1);
                        setPSpell("SPELLDMG", 1);
                        setPStat(ITEM_MOD_CASTER_WEAP, 1);
                        if (ArmorType != 4 && ArmorType != 6)
                            setPStat(ITEM_MOD_CRIT_SPELL_RATING, 1);
                        else
                            setPStat(ITEM_MOD_DEFENSE_SKILL_RATING, 1);
                    } //cas M5
                    else if (STA >= SPI && SPI > 0 && INT == 0)
                    {
                        setPStat(ITEM_MOD_STAMINA, -1);
                        setPStat(ITEM_MOD_AGILITY, -1);
                        setPSpell("MP5", 1);
                        if (ArmorType != 4 && ArmorType != 6)
                        {
                            setPStat(ITEM_MOD_STRENGTH, -1);
                            setPSpell("HEAL", 1);
                            setPStat(ITEM_MOD_HEALER_WEAP, 1);
                        }
                        else
                            setPStat(ITEM_MOD_DEFENSE_SKILL_RATING, 1);
                    } //Cas M6
                    else if (INT > STA && STA > 0 && SPI == 0)
                    {
                        setPStat(ITEM_MOD_INTELLECT, -1);
                        setPStat(ITEM_MOD_STRENGTH, -1);
                        setPStat(ITEM_MOD_AGILITY, -1);
                        setPSpell("SPELLDMG", 1);
                        setPStat(ITEM_MOD_CASTER_WEAP, 1);
                        setPStat(ITEM_MOD_CRIT_SPELL_RATING, 1);
                    } //cas M7
                    else if (SPI > STA && STA > 0 && INT == 0)
                    {
                        setPStat(ITEM_MOD_STRENGTH, -1);
                        setPStat(ITEM_MOD_AGILITY, -1);
                        setPStat(ITEM_MOD_SPIRIT, -1);
                        setPStat(ITEM_MOD_STAMINA, -1);
                        setPSpell("MP5", 1);
                        setPSpell("HEAL", 1);
                        setPStat(ITEM_MOD_HEALER_WEAP, 1);
                    } //cas M8
                    else if (SPI > 0 && STA > 0 && INT > STA && g1)
                    {
                        setPStat(ITEM_MOD_INTELLECT, -1);
                        setPStat(ITEM_MOD_STRENGTH, -1);
                        setPStat(ITEM_MOD_AGILITY, -1);
                        setPStat(ITEM_MOD_SPIRIT, -1);
                        setPStat(ITEM_MOD_STAMINA, -1);
                        setPSpell("SPELLDMG", 1);
                        setPStat(ITEM_MOD_CASTER_WEAP, 1);
                        setPStat(ITEM_MOD_HIT_SPELL_RATING, 1);
                    } //cas M9
                    else if (SPI > 0 && STA > 0 && SPI > STA && g2)
                    {
                        setPStat(ITEM_MOD_INTELLECT, -1);
                        setPStat(ITEM_MOD_STRENGTH, -1);
                        setPStat(ITEM_MOD_AGILITY, -1);
                        setPStat(ITEM_MOD_SPIRIT, -1);
                        setPStat(ITEM_MOD_STAMINA, -1);
                        setPSpell("MP5", 1);
                        setPSpell("HEAL", 1);
                        setPStat(ITEM_MOD_HEALER_WEAP, 1);
                    } //cas M10
                    else if (SPI > 0 && INT > 0 && (STA >= Math.Max(SPI, INT)))
                    {
                        setPStat(ITEM_MOD_INTELLECT, -1);
                        setPStat(ITEM_MOD_AGILITY, -1);
                        setPStat(ITEM_MOD_SPIRIT, -1);
                        setPSpell("SPELLDMG", 1);
                        setPStat(ITEM_MOD_CASTER_WEAP, 1);
                        if (ArmorType != 4 && ArmorType != 6)
                        {
                            setPStat(ITEM_MOD_STRENGTH, -1);
                            setPStat(ITEM_MOD_STAMINA, -1);
                            setPStat(ITEM_MOD_CRIT_SPELL_RATING, 1);
                        }
                        else
                            setPStat(ITEM_MOD_DEFENSE_SKILL_RATING, 1);
                    } //cas M11
                    else if (SPI > 0 && INT > 0 && g && (STA < Math.Max(SPI, INT)))
                    {
                        setPStat(ITEM_MOD_INTELLECT, -1);
                        setPStat(ITEM_MOD_STRENGTH, -1);
                        setPStat(ITEM_MOD_AGILITY, -1);
                        setPStat(ITEM_MOD_SPIRIT, -1);
                        setPStat(ITEM_MOD_STAMINA, -1);
                        setPStat(ITEM_MOD_HIT_SPELL_RATING, 1);
                        setPStat(ITEM_MOD_CRIT_SPELL_RATING, 1);
                    } //cas M12
                }
                else if (((0.8 * Math.Max(STR, AGI) - Math.Max(INT, SPI)) > 0) || (ArmorType == 3 && ((1.2 * Math.Max(STR, AGI) - Math.Max(INT, SPI)) > 0)))  // Item Physic   // 0.8 <=> 1.1 detection pour item physique maille (chasseurs et enh)
                {
                    bool h1 = (0.8 * STR - AGI) > 0;
                    bool h2 = (0.8 * AGI - STR) > 0;
                    bool h = !(h1 || h2);
                    if (ArmorType == 6)   //Cas P12
                    {
                        setPStat(ITEM_MOD_STRENGTH, -1);
                        setPStat(ITEM_MOD_AGILITY, -1);
                        setPStat(ITEM_MOD_DEFENSE_SKILL_RATING, 1);
                        setPStat(ITEM_MOD_BLOCK_RATING, 1);
                    }
                    else
                    {
                        if (AGI > 0 && STR == 0 && STA == 0)
                        {
                            setPStat(ITEM_MOD_INTELLECT, -1);
                            setPStat(ITEM_MOD_SPIRIT, -1);
                            setPStat(ITEM_MOD_AGILITY, 1);
                        } //cas p1
                        else if (AGI == 0 && STR > 0 && STA == 0)
                        {
                            setPStat(ITEM_MOD_INTELLECT, -1);
                            setPStat(ITEM_MOD_SPIRIT, -1);
                            setPStat(ITEM_MOD_STRENGTH, -1);
                            setPStat(ITEM_MOD_HIT_RATING, 1);
                        } //cas P2
                        else if (AGI > 0 && h1 && STA == 0)
                        {
                            setPStat(ITEM_MOD_INTELLECT, -1);
                            setPStat(ITEM_MOD_SPIRIT, -1);
                            setPStat(ITEM_MOD_AGILITY, -1);
                            setPStat(ITEM_MOD_CRIT_RATING, 1);
                            if (GetSlotMod() != 5)
                                setPSpell("ATTACKPWR", 1);
                            else if (ItemLevel % 2 == 1)
                                setPSpell("ATTACKPWR_RANGED", 1);
                            else
                                setPSpell("ATTACKPWR", 1);

                        } //cas P3
                        else if (AGI > 0 && h2 && STA == 0 && STR > 0)
                        {
                            setPStat(ITEM_MOD_INTELLECT, -1);
                            setPStat(ITEM_MOD_SPIRIT, -1);
                            setPStat(ITEM_MOD_AGILITY, 1);
                            setPStat(ITEM_MOD_CRIT_RATING, 1);
                        } //cas P4
                        else if (AGI > 0 && STR == 0 && STA >= AGI)
                        {
                            setPStat(ITEM_MOD_INTELLECT, -1);
                            setPStat(ITEM_MOD_SPIRIT, -1);
                            if (ArmorType != 2 && ArmorType != 4 && ArmorType != 0)
                            {
                                setPStat(ITEM_MOD_STAMINA, -1);
                                if (GetSlotMod() != 5)
                                    setPSpell("ATTACKPWR", 1);
                                else if (ItemLevel % 2 == 0)
                                    setPSpell("ATTACKPWR_RANGED", 1);
                                else
                                    setPSpell("ATTACKPWR", 1);
                            }
                            else
                            {
                                setPStat(ITEM_MOD_AGILITY, -1);
                                setPStat(ITEM_MOD_DEFENSE_SKILL_RATING, 1);
                                if (ArmorType == 2 || ArmorType == 0)
                                    setPStat(ITEM_MOD_STAMINA, 1);
                            }
                        } //cas P5
                        else if (AGI == 0 && STR > 0 && STA >= STR)
                        {
                            setPStat(ITEM_MOD_INTELLECT, -1);
                            setPStat(ITEM_MOD_SPIRIT, -1);
                            if (ArmorType == 2)
                            {
                                setPStat(ITEM_MOD_STAMINA, 1);
                                setPStat(ITEM_MOD_STRENGTH, -1);
                            }
                            else
                            {
                                setPStat(ITEM_MOD_STAMINA, -1);
                                if (GetSlotMod() != 5)
                                    setPSpell("ATTACKPWR", 1);
                                else
                                    setPSpell("ATTACKPWR_RANGED", 1);
                            }
                            if (ArmorType == 4)
                            {
                                setPStat(ITEM_MOD_DEFENSE_SKILL_RATING, 1);
                                setPStat(ITEM_MOD_STRENGTH, -1);
                            }
                            else
                            {
                                setPStat(ITEM_MOD_STAMINA, -1);
                                if (GetSlotMod() != 5)
                                    setPSpell("ATTACKPWR", 1);
                                else if (ItemLevel % 2 == 1)
                                    setPSpell("ATTACKPWR_RANGED", 1);
                                else
                                    setPSpell("ATTACKPWR", 1);
                            }
                        } //cas P6
                        else if (AGI > STA && STR == 0 && STA > 0)
                        {
                            setPStat(ITEM_MOD_INTELLECT, -1);
                            setPStat(ITEM_MOD_SPIRIT, -1);
                            setPStat(ITEM_MOD_AGILITY, -1);
                            setPStat(ITEM_MOD_CRIT_RATING, 1);
                        } //cas P7
                        else if (STR > STA && AGI == 0 && STA > 0)
                        {
                            setPStat(ITEM_MOD_INTELLECT, -1);
                            setPStat(ITEM_MOD_SPIRIT, -1);
                            setPStat(ITEM_MOD_HIT_RATING, 1);
                        } //cas P8
                        else if ((ArmorType == 2 || ArmorType == 4) && STA >= Math.Max(AGI, STR) && STA > 0 && AGI > 0 & STR > 0)
                        {
                            setPStat(ITEM_MOD_INTELLECT, -1);
                            setPStat(ITEM_MOD_SPIRIT, -1);
                            setPStat(ITEM_MOD_AGILITY, -1);
                            setPStat(ITEM_MOD_DODGE_RATING, 1);
                            setPStat(ITEM_MOD_DEFENSE_SKILL_RATING, 1);
                        } //cas P9
                        else if (h2 && AGI > STA && STA > 0 && STR > 0)
                        {
                            setPStat(ITEM_MOD_INTELLECT, -1);
                            setPStat(ITEM_MOD_SPIRIT, -1);
                            setPStat(ITEM_MOD_STRENGTH, -1);
                            setPStat(ITEM_MOD_STAMINA, -1);
                            setPStat(ITEM_MOD_HIT_RATING, 1);
                            if (GetSlotMod() != 5)
                                setPSpell("ATTACKPWR", 1);
                            else if (ItemLevel % 2 == 0)
                                setPSpell("ATTACKPWR_RANGED", 1);
                            else
                                setPSpell("ATTACKPWR", 1);
                        } //cas P10
                        else if (h1 && STR > STA && STA > 0 && AGI > 0)
                        {
                            setPStat(ITEM_MOD_INTELLECT, -1);
                            setPStat(ITEM_MOD_SPIRIT, -1);
                            setPStat(ITEM_MOD_AGILITY, -1);
                            setPStat(ITEM_MOD_STAMINA, -1);
                            setPStat(ITEM_MOD_HIT_RATING, 1);
                            if (GetSlotMod() != 5)
                                setPSpell("ATTACKPWR", 1);
                            else if (ItemLevel % 2 == 1)
                                setPSpell("ATTACKPWR_RANGED", 1);
                            else
                                setPSpell("ATTACKPWR", 1);
                        } //cas P11
                        else if (ArmorType != 2 && ArmorType != 4 && STA >= Math.Max(AGI, STR) && STA > 0 && AGI > 0 && STR > 0)
                        {
                            setPStat(ITEM_MOD_INTELLECT, -1);
                            setPStat(ITEM_MOD_STRENGTH, -1);
                            setPStat(ITEM_MOD_AGILITY, -1);
                            setPStat(ITEM_MOD_SPIRIT, -1);
                            setPStat(ITEM_MOD_STAMINA, -1);
                            setPStat(ITEM_MOD_CRIT_RATING, 1);
                            if (GetSlotMod() != 5)
                                setPSpell("ATTACKPWR", 1);
                            else if (ItemLevel % 2 == 0)
                                setPSpell("ATTACKPWR_RANGED", 1);
                            else
                            {
                                setPSpell("ATTACKPWR", 1);
                                setPStat(ITEM_MOD_CRIT_RATING, 1);
                            }
                        } //cas P13
                        else if (h && Math.Min(STR, AGI) >= STA && STA > 0)
                        {
                            setPStat(ITEM_MOD_INTELLECT, -1);
                            setPStat(ITEM_MOD_SPIRIT, -1);
                            setPStat(ITEM_MOD_STAMINA, -1);
                            if (GetSlotMod() != 5)
                                setPSpell("ATTACKPWR", 1);
                            else if (ItemLevel % 2 == 1)
                                setPSpell("ATTACKPWR_RANGED", 1);
                            else
                                setPSpell("ATTACKPWR", 1);
                        } //cas P14
                        else if (h && Math.Min(STR, AGI) >= STA && STA == 0)
                        {
                            setPStat(ITEM_MOD_INTELLECT, -1);
                            setPStat(ITEM_MOD_SPIRIT, -1);
                            setPStat(ITEM_MOD_STAMINA, -1);
                            setPStat(ITEM_MOD_STRENGTH, -1);
                            setPStat(ITEM_MOD_AGILITY, -1);
                            if (GetSlotMod() != 5)
                                setPSpell("ATTACKPWR", 1);
                            else if (ItemLevel % 2 == 0)
                                setPSpell("ATTACKPWR_RANGED", 1);
                            else
                                setPSpell("ATTACKPWR", 1);
                        } //cas P15 à ajouter
                    }
                }
                else   //Item mixed
                {
                    if (STA > Math.Max(AGI, Math.Max(INT, Math.Max(SPI, STR))))
                    {
                        setPStat(ITEM_MOD_INTELLECT, -1);
                        setPStat(ITEM_MOD_SPIRIT, -1);
                        setPStat(ITEM_MOD_AGILITY, -1);
                        setPStat(ITEM_MOD_STRENGTH, -1);

                        if (Math.Max(AGI, Math.Max(INT, Math.Max(SPI, STR))) == 0)
                            setPStat(ITEM_MOD_STAMINA, -1);
                        if (ArmorType == 1)
                            setPSpell("SPELLDMG", 1);
                        setPStat(ITEM_MOD_CASTER_WEAP, 1);
                        if (ArmorType == 2)
                            setPStat(ITEM_MOD_STAMINA, 1);
                        //if (ArmorType == 0) { setPSpell("defense,1); }
                        if (ArmorType > 3)
                            setPStat(ITEM_MOD_DEFENSE_SKILL_RATING, 1);
                        if (ArmorType == 5)
                            setPStat(ITEM_MOD_DODGE_RATING, 1);
                        if (ArmorType == 6)
                            setPStat(ITEM_MOD_BLOCK_RATING, 1);
                    }  //Cas Mi1 
                    else if (INT > Math.Max(AGI, Math.Max(STA, Math.Max(SPI, STR))))
                    {
                        setPStat(ITEM_MOD_STAMINA, -1);
                        setPStat(ITEM_MOD_SPIRIT, -1);
                        setPStat(ITEM_MOD_AGILITY, -1);
                        setPStat(ITEM_MOD_STRENGTH, -1);
                        setPSpell("SPELLDMG", 1);
                        setPStat(ITEM_MOD_CASTER_WEAP, 1);
                        if (Math.Max(AGI, Math.Max(STA, Math.Max(SPI, STR))) == 0)
                            setPStat(ITEM_MOD_INTELLECT, -1);
                    } //Cas Mi2
                    else if (SPI > Math.Max(AGI, Math.Max(STA, Math.Max(INT, STR))))
                    {
                        setPStat(ITEM_MOD_STAMINA, -1);
                        setPStat(ITEM_MOD_INTELLECT, -1);
                        setPStat(ITEM_MOD_AGILITY, -1);
                        setPStat(ITEM_MOD_STRENGTH, -1);
                        setPSpell("HEAL", 1);
                        setPStat(ITEM_MOD_HEALER_WEAP, 1);
                        if (Math.Max(AGI, Math.Max(STA, Math.Max(INT, STR))) == 0)
                            setPStat(ITEM_MOD_SPIRIT, -1);
                    }  //Cas Mi3 
                    else if (AGI > Math.Max(SPI, Math.Max(STA, Math.Max(INT, STR))))
                    {
                        setPStat(ITEM_MOD_STAMINA, -1);
                        setPStat(ITEM_MOD_INTELLECT, -1);
                        setPStat(ITEM_MOD_SPIRIT, -1);
                        setPStat(ITEM_MOD_STRENGTH, -1);
                        setPSpell("ATTACKPWR", 1);
                        if (Math.Max(SPI, Math.Max(STA, Math.Max(INT, STR))) == 0)
                            setPStat(ITEM_MOD_AGILITY, -1);
                        // setPSpell("ATTACKPWR", 0); // ???????????
                    }  //Cas Mi4 
                    else if (STR > Math.Max(SPI, Math.Max(STA, Math.Max(INT, AGI))))
                    {
                        setPStat(ITEM_MOD_STAMINA, -1);
                        setPStat(ITEM_MOD_INTELLECT, -1);
                        setPStat(ITEM_MOD_SPIRIT, -1);
                        setPStat(ITEM_MOD_AGILITY, -1);
                        if (GetSlotMod() != 5)
                            setPSpell("ATTACKPWR", 1);
                        else
                            setPSpell("ATTACKPWR_RANGED", 1);
                        if (Math.Max(SPI, Math.Max(STA, Math.Max(INT, AGI))) == 0)
                            setPStat(ITEM_MOD_STRENGTH, -1);
                    }  //Cas Mi5 
                    else
                    {
                        setPStat(ITEM_MOD_STAMINA, -1);
                        setPStat(ITEM_MOD_INTELLECT, -1);
                        setPStat(ITEM_MOD_SPIRIT, -1);
                        setPStat(ITEM_MOD_AGILITY, -1);
                        setPStat(ITEM_MOD_STRENGTH, -1);
                        setPSpell("HEAL", 1);
                        setPStat(ITEM_MOD_HEALER_WEAP, 1);
                    }
                }
            }
        }

        public void setPSpell(string Name, int value)
        {
            for (int MOD_TYPE = 0; MOD_TYPE < SPELL_MOD_TYPES_MAX; MOD_TYPE++)
                for (int j = 0; j < spell_types[MOD_TYPE].Count; j++)
                    if (spell_types[MOD_TYPE].ElementAt(j).Value.name.ToUpper() == Name.ToUpper())
                        pathing_spell[spell_types[MOD_TYPE].ElementAt(j).Key] = value;
        }

        public void setPStat(int stat, int value)
        {
            pathing_stat[stat] = value;
        }

        public string Export()
        {
            return outputString;
        }

        public Item(Item item)
        {
            entry = item.entry;
            patch = item.patch;
            parent_entry = item.parent_entry;
            _class = item._class;
            subclass = item.subclass;
            name = item.name;
            name_split = item.name_split;
            displayid = item.displayid;
            Quality = item.Quality;
            Flags = item.Flags;
            BuyCount = item.BuyCount;
            BuyPrice = item.BuyPrice;
            SellPrice = item.SellPrice;
            InventoryType = item.InventoryType;
            AllowableClass = item.AllowableClass;
            AllowableRace = item.AllowableRace;
            ItemLevel = item.ItemLevel;
            sItemLevel = item.sItemLevel;
            RequiredLevel = item.RequiredLevel;
            RequiredSkill = item.RequiredSkill;
            RequiredSkillRank = item.RequiredSkillRank;
            requiredspell = item.requiredspell;
            requiredhonorrank = item.requiredhonorrank;
            RequiredCityRank = item.RequiredCityRank;
            RequiredReputationFaction = item.RequiredReputationFaction;
            RequiredReputationRank = item.RequiredReputationRank;
            maxcount = item.maxcount;
            stackable = item.stackable;
            ContainerSlots = item.ContainerSlots;
            stat_type1 = item.stat_type1;
            stat_value1 = item.stat_value1;
            stat_type2 = item.stat_type2;
            stat_value2 = item.stat_value2;
            stat_type3 = item.stat_type3;
            stat_value3 = item.stat_value3;
            stat_type4 = item.stat_type4;
            stat_value4 = item.stat_value4;
            stat_type5 = item.stat_type5;
            stat_value5 = item.stat_value5;
            stat_type6 = item.stat_type6;
            stat_value6 = item.stat_value6;
            stat_type7 = item.stat_type7;
            stat_value7 = item.stat_value7;
            stat_type8 = item.stat_type8;
            stat_value8 = item.stat_value8;
            stat_type9 = item.stat_type9;
            stat_value9 = item.stat_value9;
            stat_type10 = item.stat_type10;
            stat_value10 = item.stat_value10;
            armor = item.armor;
            bonus_armor = item.bonus_armor;
            holy_res = item.holy_res;
            fire_res = item.fire_res;
            nature_res = item.nature_res;
            frost_res = item.frost_res;
            shadow_res = item.shadow_res;
            arcane_res = item.arcane_res;
            delay = item.delay;
            ammo_type = item.ammo_type;
            RangedModRange = item.RangedModRange;
            bonding = item.bonding;
            description = item.description;
            PageText = item.PageText;
            LanguageID = item.LanguageID;
            PageMaterial = item.PageMaterial;
            startquest = item.startquest;
            lockid = item.lockid;
            Material = item.Material;
            sheath = item.sheath;
            RandomProperty = item.RandomProperty;

            RandomSuffix = item.RandomSuffix; // TBC
            TotemCategory = item.TotemCategory; //TBC
            socketColor_1 = item.socketColor_1; //TBC
            socketContent_1 = item.socketContent_1; //TBC
            socketColor_2 = item.socketColor_2; //TBC
            socketContent_2 = item.socketContent_2; //TBC
            socketColor_3 = item.socketColor_3; //TBC
            socketContent_3 = item.socketContent_3; //TBC
            socketBonus = item.socketBonus; //TBC
            GemProperties = item.GemProperties; //TBC
            RequiredDisenchantSkill = item.RequiredDisenchantSkill; //TBC
            ArmorDamageModifier = item.ArmorDamageModifier; //TBC

            block = item.block;
            itemset = item.itemset;
            MaxDurability = item.MaxDurability;
            area = item.area;
            Map = item.Map;
            BagFamily = item.BagFamily;
            ScriptName = item.ScriptName;
            DisenchantID = item.DisenchantID;
            FoodType = item.FoodType;
            minMoneyLoot = item.minMoneyLoot;
            maxMoneyLoot = item.maxMoneyLoot;
            Duration = item.Duration;
            ExtraFlags = item.ExtraFlags;
            Flags2 = item.Flags2;
            description_loc2 = item.description_loc2;
            name_loc2 = item.name_loc2;

            // spells_ori = item.spells_ori;
            // foreach (Spell sp in item.spells_ori)
            for (int i = 0; i < item.spells_ori.Count(); i++)
                spells_ori[i] = (item.spells_ori[i]);

            // damages_ori = item.damages_ori;
            // foreach (Damage dmg in item.damages_ori)
            for (int i = 0; i < item.damages_ori.Count(); i++)
                damages_ori[i] = (item.damages_ori[i]);

            // enchantments_ori = item.enchantments_ori;
            foreach (Enchantment ench in item.enchantments_ori)
                enchantments_ori.Add(ench);

            // pathing_stat = item.pathing_stat;
            // foreach (int path in item.pathing_stat)
            for (int i = 0; i < item.pathing_stat.Count(); i++)
                pathing_stat[i] = (item.pathing_stat[i]);

            // pathing_spell = item.pathing_spell;
            // foreach (int path in item.pathing_spell)
            for (int i = 0; i < item.pathing_spell.Count(); i++)
                pathing_spell[i] = (item.pathing_spell[i]);

            // spell_shortlist = item.spell_shortlist;
            foreach (var sp in item.spell_masterlist)
                spell_masterlist.Add(sp.Key, sp.Value);
        }

        public Item()
        {
        }

        public int GetArmorType()
        {
            if (InventoryType == 16)
                return 0;
            else if (_class == 4 && subclass <= 6 && subclass > 0)
                return subclass;
            else if (_class == 2)
                return 5;
            else
                return 0;
        }

        public int GetSlotMod()
        {
            switch (InventoryType)
            {
                case 1:
                case 5:
                case 7:
                case 17:
                case 20:
                    return 1;
                case 3:
                case 10:
                case 6:
                case 8:
                    return 2;
                case 9:
                case 2:
                case 16:
                case 11:
                case 22:    // 22=offhands /!\ Some weapon may be considered as off-hand ! An other test must be done. Maybe use Class and subclass
                case 23:
                case 14:
                    return 3;  // Weapons 
                case 21:
                case 13:
                    return 4;
                case 15:
                case 26:
                case 25:
                    return subclass == 19 ? 7 : 5; // return 7 si baguette
                case 12:
                    return 6;
                default:
                    return 0;
            }
        }

        public bool HasItemStat(int stat)
        {
            if (stat_type1 == stat ||
                stat_type2 == stat ||
                stat_type3 == stat ||
                stat_type4 == stat ||
                stat_type5 == stat ||
                stat_type6 == stat ||
                stat_type7 == stat ||
                stat_type8 == stat ||
                stat_type9 == stat ||
                stat_type10 == stat)
                return true;
            return false;
        }

        /// <summary>
        /// Set an ITEM_MOD value to the item.
        /// </summary>
        public void SetItemModValue(int stat, int value)
        {
            if (value != 0)
            {
                if (stat_type1 == stat)
                    stat_value1 = value;
                else if (stat_type2 == stat)
                    stat_value2 = value;
                else if (stat_type3 == stat)
                    stat_value3 = value;
                else if (stat_type4 == stat)
                    stat_value4 = value;
                else if (stat_type5 == stat)
                    stat_value5 = value;
                else if (stat_type6 == stat)
                    stat_value6 = value;
                else if (stat_type7 == stat)
                    stat_value7 = value;
                else if (stat_type8 == stat)
                    stat_value8 = value;
                else if (stat_type9 == stat)
                    stat_value9 = value;
                else if (stat_type10 == stat)
                    stat_value10 = value;
                else if (stat_type1 == 0)   // On place la stat dans la première boite vide
                { stat_value1 = value; stat_type1 = stat; }
                else if (stat_type2 == 0)
                { stat_value2 = value; stat_type2 = stat; }
                else if (stat_type3 == 0)
                { stat_value3 = value; stat_type3 = stat; }
                else if (stat_type4 == 0)
                { stat_value4 = value; stat_type4 = stat; }
                else if (stat_type5 == 0)
                { stat_value5 = value; stat_type5 = stat; }
                else if (stat_type6 == 0)
                { stat_value6 = value; stat_type6 = stat; }
                else if (stat_type7 == 0)
                { stat_value7 = value; stat_type7 = stat; }
                else if (stat_type8 == 0)
                { stat_value8 = value; stat_type8 = stat; }
                else if (stat_type9 == 0)
                { stat_value9 = value; stat_type9 = stat; }
                else if (stat_type10 == 0)
                { stat_value10 = value; stat_type10 = stat; }
            }
        }

        /// <summary>
        /// Get an ITEM_MOD value from the item.
        /// </summary>
        public int GetItemModValue(int stat)
        {
            if (stat_type1 == stat)
                return stat_value1;
            else if (stat_type2 == stat)
                return stat_value2;
            else if (stat_type3 == stat)
                return stat_value3;
            else if (stat_type4 == stat)
                return stat_value4;
            else if (stat_type5 == stat)
                return stat_value5;
            else if (stat_type6 == stat)
                return stat_value6;
            else if (stat_type7 == stat)
                return stat_value7;
            else if (stat_type8 == stat)
                return stat_value8;
            else if (stat_type9 == stat)
                return stat_value9;
            else if (stat_type10 == stat)
                return stat_value10;
            return 0;
        }

        /// <summary>
        /// Set a resistance value to the item (Holy, Fire, Nature, Frost, Shadow, Arcane)
        /// </summary>
        public void SetItemResValue(string res, int value)
        {
            switch (res)
            {
                case "FIRE":
                    fire_res = value;
                    break;
                case "FROST":
                    frost_res = value;
                    break;
                case "SHADOW":
                    shadow_res = value;
                    break;
                case "HOLY":
                    holy_res = value;
                    break;
                case "NATURE":
                    nature_res = value;
                    break;
                case "ARCANE":
                    arcane_res = value;
                    break;
            }
        }

        /// <summary>
        /// Get a item resistance value from the item.
        /// </summary>
        public int getItemResValue(string res)
        {
            switch (res)
            {
                case "FIRE":
                    return fire_res;
                case "FROST":
                    return frost_res;
                case "SHADOW":
                    return shadow_res;
                case "HOLY":
                    return holy_res;
                case "NATURE":
                    return nature_res;
                case "ARCANE":
                    return arcane_res;
            }
            return 0;
        }

        /// <summary>
        /// Set a new handled spell to the item.
        /// </summary>
        public void SetSpellValue(string cat, int value, int type)
        {
            if (value != 0)
            {
                Dictionary<int, int> dictionary = GetSpellsFromName(cat, type);

                while (!dictionary.ContainsKey(value) && value > -1)
                    value--;

                if (value >= 0)
                {
                    int spellid = dictionary[value];
                    for (int i = 0; i < 5; i++)
                        if (spells[i] == null)
                        {
                            spells[i] = new Spell(Form1.spell_list[spellid])
                            {
                                spelltrigger = 1,
                                spellcharges = 0,
                                spellppmRate = 0,
                                spellcooldown = -1,
                                spellcategory = 0,
                                spellcategorycooldown = -1
                            };
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Set a new non-handled (classic) spell to the item.
        /// </summary>
        public void SetClassicSpell(Spell spell)
        {
            for (int i = 0; i < 5; i++)
                if (spells[i] == null)
                {
                    spells[i] = spell;
                    break;
                }
        }

        private Dictionary<int, int> GetSpellsFromName(string cat, int type)
        {
            Dictionary<int, int> this_list = new Dictionary<int, int>();
            cat = cat.ToUpper();

            foreach (Spell sp in Form1.spell_list.Values)
            {
                int spell_pts = sp.GetPoints();
                if (!this_list.ContainsKey(spell_pts))
                    switch (type)
                    {
                        case 0:
                            if (sp.IsType(cat)) this_list.Add(spell_pts, sp.spellID);
                            break;
                        case 1:
                            if (sp.IsSchoolDamage(cat)) this_list.Add(spell_pts, sp.spellID);
                            break;
                        case 2:
                            if (sp.IsSchoolResist(cat)) this_list.Add(spell_pts, sp.spellID);
                            break;
                    }
            }

            return this_list;
        }

        public bool canScale()
        {
            int hasmod = 0;
            for (int i = 0; i < ITEM_MOD_MAX; i++)
                hasmod += GetItemModValue(i);

            int hasspell = 0;
            foreach (Spell sp in spells_ori.Where(a => a != null))
                hasspell += sp.spellID;

            double hasdamage = 0;
            foreach (Damage dmg in damages_ori.Where(a => a != null))
                hasdamage += dmg.min;

            bool hasresist = false;
            if (holy_res != 0 || fire_res != 0 || nature_res != 0 || frost_res != 0 || shadow_res != 0 || arcane_res != 0)
                hasresist = true;
            
            int hassocket = socketBonus != null ? 1 : 0;

            if ((hasmod != 0 || hasspell != 0 || hasdamage != 0 || hassocket != 0 || RandomProperty != 0 || armor != 0 || hasresist) && requiredhonorrank == 0 && ItemLevel >= 10)
                return true;

            return false;
        }

        public int GetSpellValue(string v, int type)
        {
            v = v.ToUpper();

            // Has no scaled spells ?
            foreach (Spell s in spells_ori.Where(a => a != null))
            {
                switch (type)
                {
                    case 0:
                        if (s.IsType(v)) return s.GetPoints();
                        break;
                    case 1:
                        if (s.IsSchoolDamage(v)) return s.GetPoints();
                        break;
                    case 2:
                        if (s.IsSchoolResist(v)) return s.GetPoints();
                        break;
                }
            }

            return 0;
        }

        public void Generate()
        {
            outputString += "(";
            outputString += (entry + ",");                          // ENTRY
            outputString += (patch + ",");                          // patch
            outputString += (_class + ",");
            outputString += (subclass + ",");                      // SUBCLASS
            outputString += ("-1,");                      // unk0
            outputString += ("\"" + name + "\",");                  // NAME
            outputString += (displayid + ",");
            outputString += (Quality + ",");
            outputString += (Flags + ",");
            outputString += (BuyCount + ",");
            outputString += (BuyPrice + ",");       // BuyPrice
            outputString += (SellPrice + ",");      // SellPrice
            outputString += (InventoryType + ",");
            outputString += (AllowableClass + ",");
            outputString += (AllowableRace + ",");
            outputString += (sItemLevel + ",");           // ItemLevel
            outputString += (RequiredLevel + ",");           // RequiredLevel
            outputString += (RequiredSkill + ",");
            outputString += (RequiredSkillRank + ",");
            outputString += (requiredspell + ",");
            outputString += (requiredhonorrank + ",");              // HonorRank
            outputString += (RequiredCityRank + ",");
            outputString += (RequiredReputationFaction + ",");
            outputString += (RequiredReputationRank + ",");
            outputString += (maxcount + ",");
            outputString += (stackable + ",");
            outputString += (ContainerSlots + ",");
            outputString += (stat_type1 + ",");
            outputString += (stat_value1 + ",");
            outputString += (stat_type2 + ",");
            outputString += (stat_value2 + ",");
            outputString += (stat_type3 + ",");
            outputString += (stat_value3 + ",");
            outputString += (stat_type4 + ",");
            outputString += (stat_value4 + ",");
            outputString += (stat_type5 + ",");
            outputString += (stat_value5 + ",");
            outputString += (stat_type6 + ",");
            outputString += (stat_value6 + ",");
            outputString += (stat_type7 + ",");
            outputString += (stat_value7 + ",");
            outputString += (stat_type8 + ",");
            outputString += (stat_value8 + ",");
            outputString += (stat_type9 + ",");
            outputString += (stat_value9 + ",");
            outputString += (stat_type10 + ",");
            outputString += (stat_value10 + ",");

            for (int i = 0; i < 5; i++)
            {
                outputString += ((damages[i] != null ? (damages[i].min) : 0) + ",");                       // EXPECTED MIN DAMAGE
                outputString += ((damages[i] != null ? (damages[i].max) : 0) + ",");                       // EXPECTED MAX DAMAGE
                outputString += ((damages[i] != null ? damages[i].type : 0) + ",");
            }

            outputString += (armor + ",");                       // ARMOR
            outputString += (holy_res + ",");                    // SCALING DISABLED
            outputString += (fire_res + ",");                    // SCALING DISABLED
            outputString += (nature_res + ",");                  // SCALING DISABLED
            outputString += (frost_res + ",");                   // SCALING DISABLED
            outputString += (shadow_res + ",");                  // SCALING DISABLED
            outputString += (arcane_res + ",");                  // SCALING DISABLED
            outputString += (delay + ",");
            outputString += (ammo_type + ",");
            outputString += (RangedModRange + ",");

            for (int i = 0; i < 5; i++)
            {
                outputString += ((spells[i] != null ? spells[i].spellID : 0) + ",");
                outputString += ((spells[i] != null ? spells[i].spelltrigger : 0) + ",");
                outputString += ((spells[i] != null ? spells[i].spellcharges : 0) + ",");
                outputString += ((spells[i] != null ? spells[i].spellppmRate.ToString() : "0") + ",");
                outputString += ((spells[i] != null ? spells[i].spellcooldown : -1) + ",");
                outputString += ((spells[i] != null ? spells[i].spellcategory : 0) + ",");
                outputString += ((spells[i] != null ? spells[i].spellcategorycooldown : -1) + ",");
            }

            outputString += (bonding + ",");
            outputString += ("\"" + description + "\",");           // ITEM DESCRIPTION + SCALED MENTION
            outputString += (PageText + ",");
            outputString += (LanguageID + ",");
            outputString += (PageMaterial + ",");
            outputString += (startquest + ",");
            outputString += (lockid + ",");
            outputString += (Material + ",");
            outputString += (sheath + ",");

            if (RandomProperty != 0) // RANDOM PROPERTY
            {
                if (entry != parent_entry)
                    outputString += ((enchantments_new.Count > 0 ? "" + entry : "0") + ",");
                else
                    outputString += (RandomProperty + ",");
            }
            else
                outputString += ("0,");

            outputString += (RandomSuffix + ",");                // TBC

            outputString += (block + ",");                       // SHIELD BLOCK VALUE
            outputString += (itemset + ",");                              // itemset
            outputString += (MaxDurability + ",");
            outputString += (area + ",");
            outputString += (Map + ",");
            outputString += (BagFamily + ",");

            outputString += (TotemCategory + ","); //TBC
            outputString += (socketColor_1 + ","); //TBC
            outputString += (socketContent_1 + ","); //TBC
            outputString += (socketColor_2 + ","); //TBC
            outputString += (socketContent_2 + ","); //TBC
            outputString += (socketColor_3 + ","); //TBC
            outputString += (socketContent_3 + ","); //TBC

            if (socketBonus_new != null)
                outputString += (socketBonus_new.id.ToString() + ","); //TBC
            else
                outputString += ("0,"); //TBC

            outputString += (GemProperties + ","); //TBC
            outputString += (RequiredDisenchantSkill + ","); //TBC
            outputString += (ArmorDamageModifier + ","); //TBC

            outputString += ("\"" + ScriptName + "\",");
            outputString += (DisenchantID + ",");
            outputString += (FoodType + ",");
            outputString += (minMoneyLoot + ",");
            outputString += (maxMoneyLoot + ",");
            outputString += (Duration + ",");
            outputString += (ExtraFlags);
            outputString += (")");
        }

        public Dictionary<int, Dictionary<int, Spell>> spell_masterlist = new Dictionary<int, Dictionary<int, Spell>>();

        public bool SpellNameFilter(string SpellName)
        {
            var SpellNameList = new List<string>() { "Firebolt", "Drain Life", "Shadow Bolt", "Wrath", "Rend","Fatal Wound", "Fireball",
                                                     "Wound","Frostbolt","Shock", "Chain Lightning", "Lightning Bolt", "Frost Blast",
                                                     "Siphon Health", "Puncture Armor", "Poison","Corruption", "Corrosive Poison", "Fade" , "Thunder Clap", "Holy Smite", "Poison Cloud" };

            foreach (string Name in SpellNameList)
                if (SpellName.ToLower() == Name.ToLower())
                    return true;

            return false;
        }

        public bool SpellNameExclude(string SpellName)
        {
            var SpellNameList = new List<string>() { "Stun", "Creeping Mold", "Silence", "Dispel Magic", "Thrash", "Strength of Stone", "Disarm" , "Dazed", "Haste", "Blaze", "Malown's Slam",
                                                     "Electric Discharge", "Acid Blast", "Earthshaker", "Glimpse of Madness" };

            foreach (string Name in SpellNameList)
                if (SpellName.ToLower() == Name.ToLower())
                    return true;

            return false;
        }

        public class RATING
        {
            public int Mode = -1;
            public int Value = 0;
        }

        public static RATING GetModRating(Spell mainspell)
        {
            RATING rate = new RATING();

            switch (mainspell.spelltype)
            {
                case "DEFENSE": rate.Mode = 12; break;
                case "DODGE": rate.Mode = 13; break;
                case "PARRY": rate.Mode = 14; break;
                case "BLOCK": rate.Mode = 15; break;
                case "RANGEDHIT": rate.Mode = 17; break;
                case "SPELLHIT": rate.Mode = 18; break;
                case "RANGEDCRIT": rate.Mode = 20; break;
                case "SPELLCRIT": rate.Mode = 21; break;
                case "HASTMELEE": rate.Mode = 28; break;
                case "HASTERANGED": rate.Mode = 29; break;
                case "HASTESPELL": rate.Mode = 30; break;
                case "HIT": rate.Mode = 31; break;
                case "CRIT": rate.Mode = 32; break;
                case "RESILIENCE": rate.Mode = 35; break;
                case "EXPERTISE": rate.Mode = 37; break;
            }

            rate.Value = mainspell.GetPoints();

            return rate;
        }

        public void ConvertSpellInModRating()
        {
            // TO DO : Optimize !!
            List<Spell> spells_Remove = new List<Spell>();

            foreach (Spell mainspell in spells_ori.Where(a => a != null))
            {
                RATING SpellRating = GetModRating(mainspell);
                if (SpellRating.Mode != -1)
                {
                    SetItemModValue(SpellRating.Mode, SpellRating.Value);
                    spells_Remove.Add(mainspell);
                }
            }

            foreach (Spell mainspell in spells_Remove.Where(a => a != null))
                spells_ori.Remove(mainspell);
        }

        public void GenerateSpellShortList()
        {
            spell_masterlist.Clear();
            foreach (Spell mainspell in spells_ori.Where(a => a != null))
            {
                Dictionary<int, Spell> spell_shortlist = new Dictionary<int, Spell>();
                if (!mainspell.IsHandled() && !SpellNameExclude(mainspell.spellname_loc0) && !mainspell.HasTriggers())
                {
                    List<Spell> newList = Form1.spell_list.Values.OrderBy(x => x.GetPoints()).ToList();
                    foreach (Spell s in newList.Where(a => !a.spellname_loc0.ToLower().Contains("zz")))
                    {
                        if (!spell_shortlist.ContainsKey(s.spellID))
                            if ((SpellNameFilter(mainspell.spellname_loc0) && mainspell.spellname_loc0.ToLower() == s.spellname_loc0.ToLower()) || !SpellNameFilter(mainspell.spellname_loc0))  //filtre suivant ce nom de sort uniquement si dans la liste des sorts définies dans SpellNameFilter
                                if (mainspell.effect1Aura == s.effect1Aura)
                                    if (mainspell.effect2Aura == s.effect2Aura)
                                        if (mainspell.effect3Aura == s.effect3Aura)
                                            if (mainspell.effect1id == s.effect1id)
                                                if (mainspell.effect2id == s.effect2id)
                                                    if (mainspell.effect3id == s.effect3id)
                                                        if (mainspell.rangeID == s.rangeID)
                                                            if (mainspell.resistancesID == s.resistancesID)
                                                                if (mainspell.effect1ChainTarget == s.effect1ChainTarget)
                                                                    if (mainspell.effect2ChainTarget == s.effect2ChainTarget)
                                                                        if (mainspell.effect3ChainTarget == s.effect3ChainTarget)
                                                                            // if (mainspell.cooldown == s.cooldown)
                                                                            // if (mainspell.manacost == s.manacost)
                                                                            if (mainspell.mechanicID == s.mechanicID)
                                                                                if (mainspell.effect1MiscValue == s.effect1MiscValue)
                                                                                    if (mainspell.effect2MiscValue == s.effect2MiscValue)
                                                                                        if (mainspell.effect3MiscValue == s.effect3MiscValue)
                                                                                            if (mainspell.duration == s.duration)
                                                                                                if (mainspell.schoolmask == s.schoolmask)
                                                                                                    if(mainspell.effect1itemtype == s.effect1itemtype)
                                                                                                    // if ((mainspell.effect1triggerspell != 0 && s.effect1triggerspell != 0) || (mainspell.effect1triggerspell == 0 && s.effect1triggerspell == 0))
                                                                                                    // if ((mainspell.effect2triggerspell != 0 && s.effect2triggerspell != 0) || (mainspell.effect2triggerspell == 0 && s.effect2triggerspell == 0))
                                                                                                    // if ((mainspell.effect3triggerspell != 0 && s.effect3triggerspell != 0) || (mainspell.effect3triggerspell == 0 && s.effect3triggerspell == 0))
                                                                                                    spell_shortlist.Add(s.spellID, s);
                    }
                }
                spell_masterlist.Add(mainspell.spellID, spell_shortlist);
            }
        }

        public void Dispose()
        {
        }
    }
}
