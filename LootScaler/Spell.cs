using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;



namespace LootScaler
{
   
    public class Spell
    {
        public int spellID;
        public int value;
        public string spellname_loc0;
        public string tooltip_loc0;
        public int effect1BasePoints;
        public int effect1DieSides;
        public int effect1Aura; //filter
        public int effect2Aura; //filter
        public int effect3Aura; //filter
        public int resistancesID; //filter
        public int rangeID; //filter
        public int effect1id; //filter
        public int effect2id;
        public int effect3id;
        public int effect1itemtype;
        public int effect2itemtype;
        public int effect3itemtype;
        public int effect1triggerspell;
        public int effect2triggerspell;
        public int effect3triggerspell;
        public int effect1ChainTarget; //filter
        public int effect2ChainTarget; //filter
        public int effect3ChainTarget; //filter
        public int cooldown; //filter
        public int mechanicID; //filter
        public int effect1MiscValue; //filter
        public int effect2MiscValue; //filter
        public int effect3MiscValue; //filter
        public int effect1Amplitude; //filter
        public int effect2Amplitude; //filter
        public int effect3Amplitude; //filter
        public int duration; //filter
        public bool manacost; //filter
        public int procChance;

        public int spelltrigger; // 0 (On use), 1 (On equip), 2 (Chance on hit), 4 (Soulstone), 5 (On use without delay)
        public int spellcharges;
        public double spellppmRate;
        public int spellcooldown;
        public int spellcategory;
        public int spellcategorycooldown;
        public SchoolMask schoolmask;

        public bool isHandled;

        public string spelltype;

        public Spell(Spell s)
        {
            spellID = s.spellID;
            value = s.value;
            spellname_loc0 = s.spellname_loc0;
            tooltip_loc0 = s.tooltip_loc0;

            effect1BasePoints = s.effect1BasePoints;
            effect1DieSides = s.effect1DieSides;

            effect1Aura = s.effect1Aura;
            effect2Aura = s.effect2Aura;
            effect3Aura = s.effect3Aura;
            resistancesID = s.resistancesID;

            rangeID = s.rangeID;

            effect1id = s.effect1id;
            effect2id = s.effect2id;
            effect3id = s.effect3id;

            effect1itemtype = s.effect1itemtype;
            effect2itemtype = s.effect2itemtype;
            effect3itemtype = s.effect3itemtype;

            effect1ChainTarget = s.effect1ChainTarget;
            effect2ChainTarget = s.effect2ChainTarget;
            effect3ChainTarget = s.effect3ChainTarget;

            effect1triggerspell = s.effect1triggerspell;
            effect2triggerspell = s.effect2triggerspell;
            effect3triggerspell = s.effect3triggerspell;

            cooldown = s.cooldown;
            mechanicID = s.mechanicID;

            effect1MiscValue = s.effect1MiscValue;
            effect2MiscValue = s.effect2MiscValue;
            effect3MiscValue = s.effect3MiscValue;

            effect1Amplitude = s.effect1Amplitude;
            effect2Amplitude = s.effect2Amplitude;
            effect3Amplitude = s.effect3Amplitude;

            duration = s.duration;
            manacost = s.manacost;
            procChance = s.procChance;

            spelltrigger = s.spelltrigger;
            spellcharges = s.spellcharges;
            spellppmRate = s.spellppmRate;
            spellcooldown = s.spellcooldown;
            spellcategory = s.spellcategory;
            spellcategorycooldown = s.spellcategorycooldown;
            spelltype = s.spelltype;

            isHandled = s.isHandled;
            schoolmask = s.schoolmask;
        }

        public Spell()
        {
        }

        public int GetPoints()
        {
            int bp = Math.Abs(effect1BasePoints);
            int ds = Math.Abs(effect1DieSides);

            if (bp >= ds && ds > 1)         // value range (x to y)
                return (bp + (bp + ds)) / 2;
            else if(bp >= ds && ds == 1)    // fixed value
                return (bp + ds);
            else if (bp == 0 && ds == 1)    // default
                return ds;

            return Math.Max(bp, 1);
        }

        public bool IsSchoolResist(string school) // Fire, Frost, Shadow, Holy, Nature, Arcane, Resist, All
        {
            Match match = Regex.Match(spellname_loc0, @"Increased " + school + " Resist +[0-9]+$", RegexOptions.IgnoreCase);
            if (match.Success)
                return true;

            return false;
        }

        public bool IsSchoolDamage(string school) // Fire, Frost, Shadow, Holy, Nature, Arcane, Resist, All
        {
            Match match = Regex.Match(spellname_loc0, @"Increase " + school + " Dam +[0-9]+$", RegexOptions.IgnoreCase);
            if (match.Success)
                return true;

            return false;
        }
		
        public void SetHandled()
        {
            List<string> handled = new List<string>() { "MP5", "HP5", "SPELLDMG", "HEAL", "ATTACKPWR", "ATTACKPWR_RANGED", "ATTACKPWR_FERAL", "DEFENSE", "ARMOR", "BLOCK", "PARRY", "DODGE", "CRIT", "SPELLCRIT", "HIT", "SPELLHIT", "SPELLPENETRATION", "WEAPONDAMAGE", "EXPERTISE", "ARMORPENETRATION" };
            List<string> schools = new List<string>() { "FIRE", "FROST", "SHADOW", "HOLY", "NATURE", "ARCANE", "RESIST", "ALL" };

            foreach (string t in handled)
                isHandled = isHandled || IsType(t);

            foreach (string s in schools)
            {
                isHandled = isHandled || IsSchoolDamage(s);
                isHandled = isHandled || IsSchoolResist(s);
            }
        }

        public bool IsHandled()
        {
            return isHandled;
        }

        public bool HasTriggers()
        {
            return effect1triggerspell != 0 || effect2triggerspell != 0 || effect3triggerspell != 0;
        }

        private bool UpdateType(string cat)
        {
            if (spelltype == cat)
                return true;
            else
            {
                MySqlConnection connection = new MySqlConnection(Form1.connectionString);
                connection.Open();

                MySqlCommand cmd = new MySqlCommand("UPDATE tbcaowow.aowow_spell SET spelltype = '" + cat + "' WHERE spellID = " + this.spellID, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                connection.Close();

                return true;
            }
        }

        private bool ReadIsType(string cat)
        {
            if(spelltype != "")
                return (spelltype == cat);
            return false;
        }

        public bool IsType(string cat)
        {
            if (ReadIsType(cat))
                return true;

            bool matcha = false;
            bool matchb = false;
            bool matchc = false;
            bool matchd = false;

            if (cat == "MP5")
            {
                //IsMP5
                matcha = Regex.Match(tooltip_loc0, @"^Restores.*.mana per 5 sec[.]$", RegexOptions.IgnoreCase).Success; // Restores +[0-9]+ mana per +[0-9]+ sec.
                matchb = Regex.Match(tooltip_loc0, @"^Restores.*.mana every 5 sec[.]$", RegexOptions.IgnoreCase).Success; // Restores +[0-9]+ mana every +[0-9]+ sec.
                matchc = Regex.Match(tooltip_loc0, @"^Restores.*.mana per.*.sec[.]$", RegexOptions.IgnoreCase).Success && effect1Amplitude == 5000;  //==5000 ???
                matchd = Regex.Match(tooltip_loc0, @"^Restores.*.mana every.*.sec[.]$", RegexOptions.IgnoreCase).Success && effect1Amplitude == 5000; //==5000 ???
            }
            else if(cat == "HP5")
            {
                //IsHP5
                matcha = Regex.Match(tooltip_loc0, @"^Restores.*.health every 5 sec[.]$", RegexOptions.IgnoreCase).Success; // Restores +[0-9]+ health every +[0-9]+ sec.
                matchb = Regex.Match(tooltip_loc0, @"^Restores.*.health per 5 sec[.]$", RegexOptions.IgnoreCase).Success; // Restores +[0-9]+ health per +[0-9]+ sec.
                matchc = Regex.Match(tooltip_loc0, @"^Restores.*.health every.*.sec[.]$", RegexOptions.IgnoreCase).Success && effect1Amplitude == 5000;  //==5000 ???
                matchd = Regex.Match(tooltip_loc0, @"^Restores.*.health every.*.sec[.]$", RegexOptions.IgnoreCase).Success && effect1Amplitude == 5000;  //==5000 ???
            }
            else if (cat == "SPELLDMG")
            {
                //IsSpellDam
                matcha = Regex.Match(spellname_loc0, @"Increase Spell Dam +[0-9]+$", RegexOptions.IgnoreCase).Success; // Increase Spell Dam 1
            }
            else if (cat == "HEAL")
            {
                //IsHealing
                matcha = Regex.Match(spellname_loc0, @"Increase Healing +[0-9]+$", RegexOptions.IgnoreCase).Success; // Increase Healing 2
            }
            else if (cat == "ATTACKPWR")
            {
                //IsAttackPower
                matcha = Regex.Match(spellname_loc0, @"Attack Power +[0-9]+$", RegexOptions.IgnoreCase).Success; // Attack Power 02
            }
            else if (cat == "ATTACKPWR_RANGED")
            {
                //IsAttackPowerRanged
                matcha = Regex.Match(spellname_loc0, @"Attack Power Ranged +[0-9]+$", RegexOptions.IgnoreCase).Success; // Attack Power Ranged 02
            }
            else if (cat == "ATTACKPWR_FERAL")
            {
                //IsAttackPowerRanged
                matcha = Regex.Match(spellname_loc0, @"Attack Power - Feral \(\+[0-9]+\)$", RegexOptions.IgnoreCase).Success; // Attack Power - Feral (+0007)
            }
            else if (cat == "DEFENSE")
            {
                //IsDefense
                matcha = Regex.Match(spellname_loc0, @"Increased Defense$", RegexOptions.IgnoreCase).Success; // Increased Defense
            }
            else if (cat == "ARMOR")
            {
                //IsArmor
                matcha = Regex.Match(spellname_loc0, @"Increased Armor +[0-9]+$", RegexOptions.IgnoreCase).Success; // Increased Armor 10
            }
            else if (cat == "BLOCK")
            {
                //IsBlock
                matcha = Regex.Match(spellname_loc0, @"Increased Block +[0-9]+$", RegexOptions.IgnoreCase).Success; // Increased Block 1
            }
            else if (cat == "PARRY")
            {
                //IsParry
                matcha = Regex.Match(spellname_loc0, @"Increased Parry +[0-9]+$", RegexOptions.IgnoreCase).Success; // Increased Parry 1
            }
            else if (cat == "DODGE")
            {
                //IsDodge
                matcha = Regex.Match(spellname_loc0, @"Increased Dodge +[0-9]+$", RegexOptions.IgnoreCase).Success; // Increased Dodge 1
            }
            else if (cat == "CRIT")
            {
                //IsCrit
                matcha = Regex.Match(spellname_loc0, @"Increased Critical +[0-9]+$", RegexOptions.IgnoreCase).Success; // Increased Critical 1
            }
            else if (cat == "RANGEDCRIT")
            {
                //IsCrit
                matcha = Regex.Match(spellname_loc0, @"Increased Ranged Critical +[0-9]+$", RegexOptions.IgnoreCase).Success; // Increased Critical 1
            }
            else if (cat == "SPELLCRIT")
            {
                //IsSpellCrit
                matcha = Regex.Match(spellname_loc0, @"Increased Critical Spell$", RegexOptions.IgnoreCase).Success; // Increased Critical Spell
            }
            else if (cat == "HIT")
            {
                //IsHit
                matcha = Regex.Match(spellname_loc0, @"Increased Hit Rating +[0-9]+$", RegexOptions.IgnoreCase).Success; // Increased Hit Chance 1
            }
            else if (cat == "RANGEDHIT")
            {
                //IsRangedHit
                matcha = Regex.Match(spellname_loc0, @"Increased ranged Hit Rating +[0-9]+$", RegexOptions.IgnoreCase).Success; // Increased Hit Chance 1
            }
            else if (cat == "SPELLHIT")
            {
                //IsSpellHit
                matcha = Regex.Match(spellname_loc0, @"Increased Spell Hit Chance +[0-9]+$", RegexOptions.IgnoreCase).Success; // Increased Spell Hit Chance 1
            }
            else if (cat == "SPELLPENETRATION")
            {
                //IsSpellPenetration
                matcha = Regex.Match(spellname_loc0, @"Increased Spell Penetration +[0-9]+$", RegexOptions.IgnoreCase).Success; // Increased Spell Penetration 10
            }
            else if (cat == "WEAPONDAMAGE")
            {
                //IsWeaponDamage
                matcha = Regex.Match(spellname_loc0, @"Weapon Damage$", RegexOptions.IgnoreCase).Success; // Weapon Damage
            }
            else if (cat == "EXPERTISE")
            {
                //IsExpertise
                matcha = Regex.Match(spellname_loc0, @"Expertise Rating$", RegexOptions.IgnoreCase).Success; // Expertise Rating
            }
            else if (cat == "ARMORPENETRATION")
            {
                //IsArmorPenetration
                matcha = Regex.Match(spellname_loc0, @"Armor Penetration +[0-9]+$", RegexOptions.IgnoreCase).Success; // Armor Penetration 42
                matchb = Regex.Match(spellname_loc0, @"Armor Penetration$", RegexOptions.IgnoreCase).Success; // Armor Penetration
            }

            if (matcha || matchb || matchc || matchd)
            {
                UpdateType(cat);
                return true;
            }

            return false;
        }
    }
}
