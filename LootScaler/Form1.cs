using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
//using System.Data.MySqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

//using System.Windows.Forms;

namespace LootScaler
{
    public partial class Form1 : Form
    {
        public static void CorrectNumberFormat()
        {
            CultureInfo culture = new CultureInfo(ConfigurationManager.AppSettings["DefaultCulture"]);
            culture.NumberFormat.NumberDecimalSeparator = ".";
            culture.NumberFormat.CurrencyDecimalSeparator = ".";
            culture.NumberFormat.PercentDecimalSeparator = ".";
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        public Form1()
        {
            InitializeComponent();
        }

        static public string server;
        static public string database;
        static public string uid;
        static public string password;
        static public string connectionString;
        static public int connectionTimeout;
        static public int maxThread;
        static public bool garbageCollector;
        static public bool BonusUpgrade;
        static public int BonusUpgradeValue;
        static public UTF8Encoding UTF8NoPreamble = new UTF8Encoding(false);

        static Dictionary<int, Item> item_list = new Dictionary<int, Item>();
        List<Item> consumables_list = new List<Item>();
        List<Item> weapons_list = new List<Item>();
        List<Item> armors_list = new List<Item>();
        List<Item> junk_list = new List<Item>();
        List<Item> quest_list = new List<Item>();

        static public Dictionary<int, Spell> spell_list = new Dictionary<int, Spell>();
        List<int> forbidden_list = new List<int>();

        static public Dictionary<int, Enchantment> Enchantment_list = new Dictionary<int, Enchantment>();
        static public Dictionary<int, socketBonus> socketBonus_list = new Dictionary<int, socketBonus>();

        const int MIN_ENTRY_SCALE = 41000;      // Ne jamais toucher !  180  ! Never Change this value !
        const int MIN_ILEVEL_SCALE = 10;        // Ne jamais toucher !  10   ! Never Change this value !
        const int MAX_ILEVEL_SCALE = 70;        // Ne jamais toucher !  70   ! Never Change this value !
        const int MAX_QUALITY_SCALE = 3;        // Ne jamais toucher !  3    ! Never Change this value !

        private void Form1_Load(object sender, EventArgs e)
        {
            CorrectNumberFormat();

            Process Proc = Process.GetCurrentProcess();

            server = ConfigurationManager.AppSettings["serverIP"];          // realm.rochenoi.re
            database = ConfigurationManager.AppSettings["serverDB"];        // serverValue.Value;
            uid = ConfigurationManager.AppSettings["serverLOGIN"];          // "rochenoire";
            password = ConfigurationManager.AppSettings["serverPASS"];      // "kzMn7:t*zgqiL+uzEzdyb>+Aiu7>jS8a";
            connectionTimeout = Int32.Parse(ConfigurationManager.AppSettings["serverTIMEOUT"]);
            maxThread = Int32.Parse(ConfigurationManager.AppSettings["threadCount"]); // Environment.ProcessorCount / 2
            garbageCollector = bool.Parse(ConfigurationManager.AppSettings["garbageCollector"]); // Environment.ProcessorCount / 2
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            Console.WriteLine("Connecting to server: " + server);
            Console.WriteLine("Connecting to database: " + database);
            Console.WriteLine();

            Console.WriteLine("Allocated threads: " + maxThread);
            Console.WriteLine("Garbage collector: " + garbageCollector);
            Console.WriteLine();

            string[] lines = System.IO.File.ReadAllLines(@".\filter.txt");

            foreach (string line in lines)
                filter.Items.Add(Int32.Parse(line));

            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlConnection connection2 = new MySqlConnection(connectionString);
            connection2.Open();

            MySqlCommand cmd = new MySqlCommand("SELECT itemID from tbcaowow.aowow_itemscale WHERE forbidden = 1 LIMIT 999999", connection)
            {
                CommandTimeout = connectionTimeout
            };
            MySqlDataReader dataReader = cmd.ExecuteReader();
            Console.WriteLine("Connection to " + server + "...");

            while (dataReader.Read())
            {
                int itemID = int.Parse(dataReader["itemID"].ToString());
                if (!forbidden_list.Contains(itemID))
                    forbidden_list.Add(itemID);
            }

            dataReader.Close();

            Console.WriteLine("Loading tbcaowow.aowow_spell...");

            cmd = new MySqlCommand("SELECT sp.spellID,sd.durationBase,sp.spellname,sp.spelltype,sp.tooltip,sp.effect1BasePoints,sp.effect1DieSides,sp.effect1Aura,sp.effect2Aura,sp.effect3Aura,sp.resistancesID,sp.rangeID,sp.mechanicID,sp.effect1id,sp.effect2id,sp.effect3id,sp.effect1MiscValue,sp.effect2MiscValue,sp.effect3MiscValue,sp.effect1Amplitude,sp.effect2Amplitude,sp.effect3Amplitude,sp.effect1itemtype,sp.effect2itemtype,sp.effect3itemtype,sp.cooldown,sp.effect1triggerspell,sp.effect2triggerspell,sp.effect3triggerspell,sp.effect1ChainTarget,sp.effect2ChainTarget,sp.effect3ChainTarget,sp.manacost,sp.procChance,sp.schoolmask FROM tbcaowow.aowow_spell sp, tbcaowow.aowow_spellduration sd WHERE sp.durationID = sd.durationID LIMIT 999999", connection)
            {
                CommandTimeout = connectionTimeout
            };
            dataReader = cmd.ExecuteReader();

            while (dataReader.Read())
            {
                Spell mainspell = new Spell
                {
                    spellID = int.Parse(dataReader["spellID"].ToString()),
                    spellname_loc0 = dataReader["spellname"].ToString(),
                    spelltype = dataReader["spelltype"].ToString(),
                    tooltip_loc0 = dataReader["tooltip"].ToString(),
                    effect1BasePoints = int.Parse(dataReader["effect1BasePoints"].ToString()),
                    effect1DieSides = int.Parse(dataReader["effect1DieSides"].ToString()),

                    effect1Aura = int.Parse(dataReader["effect1Aura"].ToString()),
                    effect2Aura = int.Parse(dataReader["effect2Aura"].ToString()),
                    effect3Aura = int.Parse(dataReader["effect3Aura"].ToString()),

                    resistancesID = int.Parse(dataReader["resistancesID"].ToString()),
                    rangeID = int.Parse(dataReader["rangeID"].ToString()),

                    effect1id = int.Parse(dataReader["effect1id"].ToString()),
                    effect2id = int.Parse(dataReader["effect2id"].ToString()),
                    effect3id = int.Parse(dataReader["effect3id"].ToString()),

                    effect1itemtype = int.Parse(dataReader["effect1itemtype"].ToString()),
                    effect2itemtype = int.Parse(dataReader["effect2itemtype"].ToString()),
                    effect3itemtype = int.Parse(dataReader["effect3itemtype"].ToString()),

                    effect1triggerspell = int.Parse(dataReader["effect1triggerspell"].ToString()),
                    effect2triggerspell = int.Parse(dataReader["effect2triggerspell"].ToString()),
                    effect3triggerspell = int.Parse(dataReader["effect3triggerspell"].ToString()),

                    cooldown = int.Parse(dataReader["cooldown"].ToString()),

                    effect1ChainTarget = int.Parse(dataReader["effect1ChainTarget"].ToString()),
                    effect2ChainTarget = int.Parse(dataReader["effect2ChainTarget"].ToString()),
                    effect3ChainTarget = int.Parse(dataReader["effect3ChainTarget"].ToString()),

                    mechanicID = int.Parse(dataReader["mechanicID"].ToString()),

                    effect1MiscValue = int.Parse(dataReader["effect1MiscValue"].ToString()),
                    effect2MiscValue = int.Parse(dataReader["effect2MiscValue"].ToString()),
                    effect3MiscValue = int.Parse(dataReader["effect3MiscValue"].ToString()),

                    effect1Amplitude = int.Parse(dataReader["effect1Amplitude"].ToString()),
                    effect2Amplitude = int.Parse(dataReader["effect2Amplitude"].ToString()),
                    effect3Amplitude = int.Parse(dataReader["effect3Amplitude"].ToString()),

                    duration = int.Parse(dataReader["durationBase"].ToString())
                };
                int manacost = int.Parse(dataReader["manacost"].ToString());
                mainspell.manacost = (manacost > 0);
                mainspell.procChance = int.Parse(dataReader["procChance"].ToString());
                mainspell.schoolmask = (SchoolMask)int.Parse(dataReader["schoolmask"].ToString());

                spell_list.Add(mainspell.spellID, mainspell);
            }

            foreach (Spell s in spell_list.Values)
                s.SetHandled();

            dataReader.Close();

            Console.WriteLine("Loading aowow_itemrandomproperties...");

            cmd = new MySqlCommand("SELECT * FROM tbcaowow.aowow_itemrandomproperties LIMIT 999999", connection)
            {
                CommandTimeout = connectionTimeout
            };
            dataReader = cmd.ExecuteReader();

            while (dataReader.Read())
            {
                Enchantment ench = new Enchantment
                {
                    id = int.Parse(dataReader["id"].ToString()),
                    family = int.Parse(dataReader["type"].ToString()),
                    name = dataReader["suffix"].ToString()
                };
                ench.value.Add(double.Parse(dataReader["v_green"].ToString()));
                ench.value.Add(double.Parse(dataReader["v_blue"].ToString()));
                Enchantment_list.Add(ench.id, ench);
            }

            dataReader.Close();

            Console.WriteLine("Loading aowow_itemenchantmet...");

            cmd = new MySqlCommand("SELECT * FROM tbcaowow.aowow_itemenchantmet LIMIT 999999", connection)
            {
                CommandTimeout = connectionTimeout
            };
            dataReader = cmd.ExecuteReader();

            while (dataReader.Read())
            {
                socketBonus socket = new socketBonus
                {
                    id = int.Parse(dataReader["itemenchantmetID"].ToString()),
                    family = int.Parse(dataReader["familyID"].ToString()),
                    text = dataReader["text"].ToString(),
                    value = double.Parse(dataReader["value"].ToString())
                };
                if (socket.value > 0)
                    socketBonus_list.Add(socket.id, socket);
            }

            dataReader.Close();

            Console.WriteLine("Loading item_template...");


            cmd = new MySqlCommand("SELECT i.entry, i.patch, i.class, i.subclass, i.name, i.displayid, i.Quality, i.Flags, i.BuyCount, i.BuyPrice, i.SellPrice, i.InventoryType, i.AllowableClass, i.AllowableRace, i.ItemLevel, i.RequiredLevel, i.RequiredSkill, i.RequiredSkillRank, i.requiredspell, i.requiredhonorrank, i.RequiredCityRank, i.RequiredReputationFaction, i.RequiredReputationRank, i.maxcount, i.stackable, i.ContainerSlots, i.stat_type1, i.stat_value1, i.stat_type2, i.stat_value2, i.stat_type3, i.stat_value3, i.stat_type4, i.stat_value4, i.stat_type5, i.stat_value5, i.stat_type6, i.stat_value6, i.stat_type7, i.stat_value7, i.stat_type8, i.stat_value8, i.stat_type9, i.stat_value9, i.stat_type10, i.stat_value10, i.dmg_min1, i.dmg_max1, i.dmg_type1, i.dmg_min2, i.dmg_max2, i.dmg_type2, i.dmg_min3, i.dmg_max3, i.dmg_type3, i.dmg_min4, i.dmg_max4, i.dmg_type4, i.dmg_min5, i.dmg_max5, i.dmg_type5, i.armor, i.holy_res, i.fire_res, i.nature_res, i.frost_res, i.shadow_res, i.arcane_res, i.delay, i.ammo_type, i.RangedModRange, i.spellid_1, i.spelltrigger_1, i.spellcharges_1, i.spellppmRate_1, i.spellcooldown_1, i.spellcategory_1, i.spellcategorycooldown_1, i.spellid_2, i.spelltrigger_2, i.spellcharges_2, i.spellppmRate_2, i.spellcooldown_2, i.spellcategory_2, i.spellcategorycooldown_2, i.spellid_3, i.spelltrigger_3, i.spellcharges_3, i.spellppmRate_3, i.spellcooldown_3, i.spellcategory_3, i.spellcategorycooldown_3, i.spellid_4, i.spelltrigger_4, i.spellcharges_4, i.spellppmRate_4, i.spellcooldown_4, i.spellcategory_4, i.spellcategorycooldown_4, i.spellid_5, i.spelltrigger_5, i.spellcharges_5, i.spellppmRate_5, i.spellcooldown_5, i.spellcategory_5, i.spellcategorycooldown_5, i.bonding, i.description, i.PageText, i.LanguageID, i.PageMaterial, i.startquest, i.lockid, i.Material, i.sheath, i.RandomProperty, i.RandomSuffix, i.block, i.itemset, i.MaxDurability, i.area, i.Map, i.BagFamily, i.TotemCategory, i.socketColor_1, i.socketContent_1, i.socketColor_2, i.socketContent_2, i.socketColor_3, i.socketContent_3, i.socketBonus, i.GemProperties, i.RequiredDisenchantSkill, i.ArmorDamageModifier, i.ScriptName, i.DisenchantID, i.FoodType, i.minMoneyLoot, i.maxMoneyLoot, i.Duration, i.ExtraFlags FROM item_template i WHERE i.entry < " + MIN_ENTRY_SCALE + " LIMIT 99999999", connection)
            {
                CommandTimeout = connectionTimeout
            };
            dataReader = cmd.ExecuteReader();

            if (dataReader != null)
            {
                while (dataReader.Read())
                {
                    Item it = new Item
                    {
                        entry = int.Parse(dataReader["entry"].ToString()),
                        patch = int.Parse(dataReader["patch"].ToString()),
                        parent_entry = int.Parse(dataReader["entry"].ToString()),
                        _class = int.Parse(dataReader["class"].ToString()),
                        subclass = int.Parse(dataReader["subclass"].ToString()),
                        name = (string)(dataReader["name"].ToString())
                    };

                    if (it.name.Contains(' '))
                        it.name_split = it.name.ToLower().Split(' ').ToList();
                    else
                        it.name_split.Add(it.name);

                    it.displayid = int.Parse(dataReader["displayid"].ToString());
                    it.Quality = int.Parse(dataReader["Quality"].ToString());
                    it.Flags = int.Parse(dataReader["Flags"].ToString());
                    it.BuyCount = int.Parse(dataReader["BuyCount"].ToString());
                    it.BuyPrice = int.Parse(dataReader["BuyPrice"].ToString());
                    it.SellPrice = int.Parse(dataReader["SellPrice"].ToString());
                    it.InventoryType = int.Parse(dataReader["InventoryType"].ToString());
                    it.AllowableClass = int.Parse(dataReader["AllowableClass"].ToString());
                    it.AllowableRace = int.Parse(dataReader["AllowableRace"].ToString());
                    it.ItemLevel = int.Parse(dataReader["ItemLevel"].ToString());
                    it.sItemLevel = it.ItemLevel;
                    it.RequiredLevel = int.Parse(dataReader["RequiredLevel"].ToString());
                    it.RequiredSkill = int.Parse(dataReader["RequiredSkill"].ToString());
                    it.RequiredSkillRank = int.Parse(dataReader["RequiredSkillRank"].ToString());
                    it.requiredspell = int.Parse(dataReader["requiredspell"].ToString());
                    it.requiredhonorrank = int.Parse(dataReader["requiredhonorrank"].ToString());
                    it.RequiredCityRank = int.Parse(dataReader["RequiredCityRank"].ToString());
                    it.RequiredReputationFaction = int.Parse(dataReader["RequiredReputationFaction"].ToString());
                    it.RequiredReputationRank = int.Parse(dataReader["RequiredReputationRank"].ToString());
                    it.maxcount = int.Parse(dataReader["maxcount"].ToString());
                    it.stackable = int.Parse(dataReader["stackable"].ToString());
                    it.ContainerSlots = int.Parse(dataReader["ContainerSlots"].ToString());
                    it.stat_type1 = int.Parse(dataReader["stat_type1"].ToString());
                    it.stat_value1 = int.Parse(dataReader["stat_value1"].ToString());
                    it.stat_type2 = int.Parse(dataReader["stat_type2"].ToString());
                    it.stat_value2 = int.Parse(dataReader["stat_value2"].ToString());
                    it.stat_type3 = int.Parse(dataReader["stat_type3"].ToString());
                    it.stat_value3 = int.Parse(dataReader["stat_value3"].ToString());
                    it.stat_type4 = int.Parse(dataReader["stat_type4"].ToString());
                    it.stat_value4 = int.Parse(dataReader["stat_value4"].ToString());
                    it.stat_type5 = int.Parse(dataReader["stat_type5"].ToString());
                    it.stat_value5 = int.Parse(dataReader["stat_value5"].ToString());
                    it.stat_type6 = int.Parse(dataReader["stat_type6"].ToString());
                    it.stat_value6 = int.Parse(dataReader["stat_value6"].ToString());
                    it.stat_type7 = int.Parse(dataReader["stat_type7"].ToString());
                    it.stat_value7 = int.Parse(dataReader["stat_value7"].ToString());
                    it.stat_type8 = int.Parse(dataReader["stat_type8"].ToString());
                    it.stat_value8 = int.Parse(dataReader["stat_value8"].ToString());
                    it.stat_type9 = int.Parse(dataReader["stat_type9"].ToString());
                    it.stat_value9 = int.Parse(dataReader["stat_value9"].ToString());
                    it.stat_type10 = int.Parse(dataReader["stat_type10"].ToString());
                    it.stat_value10 = int.Parse(dataReader["stat_value10"].ToString());

                    for (int i = 1; i < 6; i++)
                    {
                        double min = double.Parse(dataReader["dmg_min" + i].ToString());
                        double max = double.Parse(dataReader["dmg_max" + i].ToString());
                        int type = int.Parse(dataReader["dmg_type" + i].ToString());

                        min = (min < 0) ? 0 : min;
                        max = (max < 0) ? 0 : max;

                        Damage dmg = new Damage((int)min, (int)max, type);
                        it.damages_ori[i - 1] = dmg;
                    }

                    it.armor = int.Parse(dataReader["armor"].ToString());
                    it.holy_res = int.Parse(dataReader["holy_res"].ToString());
                    it.fire_res = int.Parse(dataReader["fire_res"].ToString());
                    it.nature_res = int.Parse(dataReader["nature_res"].ToString());
                    it.frost_res = int.Parse(dataReader["frost_res"].ToString());
                    it.shadow_res = int.Parse(dataReader["shadow_res"].ToString());
                    it.arcane_res = int.Parse(dataReader["arcane_res"].ToString());
                    it.delay = int.Parse(dataReader["delay"].ToString());
                    it.ammo_type = int.Parse(dataReader["ammo_type"].ToString());
                    it.RangedModRange = float.Parse(dataReader["RangedModRange"].ToString());

                    for (int i = 1; i < 6; i++)
                    {
                        int spellid = int.Parse(dataReader["spellid_" + i].ToString());

                        if (spell_list.ContainsKey(spellid))
                        {
                            Spell sp = new Spell(spell_list[spellid])
                            {
                                spellID = spellid,
                                spelltrigger = int.Parse(dataReader["spelltrigger_" + i].ToString()),
                                spellcharges = int.Parse(dataReader["spellcharges_" + i].ToString()),
                                spellppmRate = float.Parse(dataReader["spellppmRate_" + i].ToString()),
                                spellcooldown = int.Parse(dataReader["spellcooldown_" + i].ToString()),
                                spellcategory = int.Parse(dataReader["spellcategory_" + i].ToString()),
                                spellcategorycooldown = int.Parse(dataReader["spellcategorycooldown_" + i].ToString())
                            };

                            it.spells_ori[i - 1] = sp;
                        }
                    }

                    it.bonding = int.Parse(dataReader["bonding"].ToString());
                    it.description = (string)(dataReader["description"].ToString());
                    it.PageText = int.Parse(dataReader["PageText"].ToString());
                    it.LanguageID = int.Parse(dataReader["LanguageID"].ToString());
                    it.PageMaterial = int.Parse(dataReader["PageMaterial"].ToString());
                    it.startquest = int.Parse(dataReader["startquest"].ToString());
                    it.lockid = int.Parse(dataReader["lockid"].ToString());
                    it.Material = int.Parse(dataReader["Material"].ToString());
                    it.sheath = int.Parse(dataReader["sheath"].ToString());
                    it.RandomProperty = int.Parse(dataReader["RandomProperty"].ToString());

                    it.RandomSuffix = int.Parse(dataReader["RandomSuffix"].ToString()); // TBC
                    it.TotemCategory = int.Parse(dataReader["TotemCategory"].ToString()); //TBC
                    it.socketColor_1 = int.Parse(dataReader["socketColor_1"].ToString()); //TBC
                    it.socketContent_1 = int.Parse(dataReader["socketContent_1"].ToString()); //TBC
                    it.socketColor_2 = int.Parse(dataReader["socketColor_2"].ToString()); //TBC
                    it.socketContent_2 = int.Parse(dataReader["socketContent_2"].ToString()); //TBC
                    it.socketColor_3 = int.Parse(dataReader["socketColor_3"].ToString()); //TBC
                    it.socketContent_3 = int.Parse(dataReader["socketContent_3"].ToString()); //TBC

                    int socketBonus = int.Parse(dataReader["socketBonus"].ToString()); //TBC
                    if (socketBonus_list.ContainsKey(socketBonus))
                        it.socketBonus = socketBonus_list[socketBonus];

                    it.GemProperties = int.Parse(dataReader["GemProperties"].ToString()); // Properties of the gem 
                    it.RequiredDisenchantSkill = int.Parse(dataReader["RequiredDisenchantSkill"].ToString()); // Required enchanting level, to disenchant the item
                    it.ArmorDamageModifier = float.Parse(dataReader["ArmorDamageModifier"].ToString()); // Shows the added armor (item class 4) or weapon damage (item class 2) for which item points are spend. Can also be negative as seen for caster weapons (damage subtracted, to gain points to spend on caster stats).

                    it.block = int.Parse(dataReader["block"].ToString());
                    it.itemset = int.Parse(dataReader["itemset"].ToString());
                    it.MaxDurability = int.Parse(dataReader["MaxDurability"].ToString());
                    it.area = int.Parse(dataReader["area"].ToString());
                    it.Map = int.Parse(dataReader["Map"].ToString());
                    it.BagFamily = int.Parse(dataReader["BagFamily"].ToString());
                    it.ScriptName = (string)(dataReader["ScriptName"].ToString());
                    it.DisenchantID = int.Parse(dataReader["DisenchantID"].ToString());
                    it.FoodType = int.Parse(dataReader["FoodType"].ToString());
                    it.minMoneyLoot = int.Parse(dataReader["minMoneyLoot"].ToString());
                    it.maxMoneyLoot = int.Parse(dataReader["maxMoneyLoot"].ToString());
                    it.Duration = int.Parse(dataReader["Duration"].ToString());
                    it.ExtraFlags = int.Parse(dataReader["ExtraFlags"].ToString());

                    it.Flags2 = (Flagsenum)it.Flags;

                    // Seriously...
                    if (it.AllowableClass == 32767 || it.AllowableClass == 2047)
                        it.AllowableClass = -1;

                    if (it.AllowableRace == 255)
                        it.AllowableRace = -1;

                    if (it.RandomProperty != 0)
                    {
                        MySqlCommand cmd2 = new MySqlCommand("SELECT * from " + ConfigurationManager.AppSettings["serverDB"] + ".item_enchantment_template et WHERE et.entry = " + it.RandomProperty, connection2);
                        MySqlDataReader dataReader2 = cmd2.ExecuteReader();

                        while (dataReader2.Read())
                        {
                            int id = int.Parse(dataReader2["ench"].ToString());
                            double chance = double.Parse(dataReader2["chance"].ToString());

                            if (Enchantment_list.ContainsKey(id))
                            {
                                Enchantment ench = new Enchantment(Enchantment_list[id])
                                {
                                    chance = chance
                                };
                                it.enchantments_ori.Add(ench);
                            }
                        }

                        dataReader2.Close();
                    }

                    if (!item_list.ContainsKey(it.entry))
                        item_list.Add(it.entry, it);
                }
                dataReader.Close();

                cmd = new MySqlCommand("SELECT l.entry, l.name_loc2, l.description_loc2 FROM locales_item l WHERE l.entry < " + MIN_ENTRY_SCALE, connection)
                {
                    CommandTimeout = connectionTimeout
                };
                dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    int entry = int.Parse(dataReader["entry"].ToString());
                    string name_loc2 = (string)(dataReader["name_loc2"].ToString());
                    string description_loc2 = (string)(dataReader["description_loc2"].ToString());

                    if (item_list.ContainsKey(entry))
                    {
                        item_list[entry].name_loc2 = name_loc2;
                        item_list[entry].description_loc2 = description_loc2;
                    }
                }

                dataReader.Close();
                connection.Close();
                connection2.Close();

                consumables_list = item_list.Values.Where(a => !forbidden_list.Contains(a.entry) && !a.Flags2.HasFlag(Flagsenum.Conjured) && !a.Flags2.HasFlag(Flagsenum.Lootable) && !a.Flags2.HasFlag(Flagsenum.Wrapped) && !a.Flags2.HasFlag(Flagsenum.Totem) && !a.Flags2.HasFlag(Flagsenum.Wrapper) && !a.Flags2.HasFlag(Flagsenum.Gifts) && !a.Flags2.HasFlag(Flagsenum.Charter) && !a.Flags2.HasFlag(Flagsenum.PvP) && !a.Flags2.HasFlag(Flagsenum.Unique) && !a.Flags2.HasFlag(Flagsenum.Throwable) && !a.Flags2.HasFlag(Flagsenum.Special) && (a._class == 0 && new[] { /*0,*/ 1, 2, 3, 4, 5, 6, 7 }.Contains(a.subclass)) && (a.spells_ori[0] != null || a.spells_ori[1] != null || a.spells_ori[2] != null || a.spells_ori[3] != null || a.spells_ori[4] != null)).OrderBy(a => a.entry).ToList();
                weapons_list = item_list.Values.Where(a => a.canScale() && !forbidden_list.Contains(a.entry) && (a._class == 2)).OrderBy(a => a.entry).ToList();
                armors_list = item_list.Values.Where(a => a.canScale() && !forbidden_list.Contains(a.entry) && (a._class == 4 && new[] { 0, 1, 2, 3, 4, 5, 6 }.Contains(a.subclass))).OrderBy(a => a.entry).ToList();
                junk_list = item_list.Values.Where(a => a._class == 15).OrderBy(a => a.entry).ToList();
                quest_list = item_list.Values.Where(a => !a.canScale() && a.startquest != 0).OrderBy(a => a.entry).ToList();

                foreach (Item it in consumables_list)
                    it.SetFood();

                groupBox1.Enabled = true;
                groupBox3.Enabled = true;

                if (garbageCollector)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

                BringToFront();
                Show();
            }
        }

        public string Between(ref string src, string start, string ended, bool del)
        {
            string ret = "";
            int idxStart = src.IndexOf(start);
            if (idxStart != -1)
            {
                idxStart += start.Length;
                int idxEnd = src.IndexOf(ended, idxStart);
                if (idxEnd != -1)
                {
                    ret = src.Substring(idxStart, idxEnd - idxStart);
                    if (del == true)
                        src = src.Replace(start + ret + ended, "");
                }
            }
            return ret;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Settings
            BonusUpgrade = checkUpgrade.Checked;
            BonusUpgradeValue = (int)numericUpDown1.Value;

            if (weapCheck.Checked)
                generateScaleGeneric(weapons_list, "weapons");
            if (armorCheck.Checked)
                generateScaleGeneric(armors_list, "armor");
            if (consuCheck.Checked)
                generateScaleFood();
            if (checkDBC.Checked)
                generateDBC();
            if (checkJunk.Checked)
                generateJunk();
            if (checkQUEST.Checked)
                generateQuest();

            CLEAN();
        }

        private void CLEAN()
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "out", "099_clean.sql")))
            {
                if (true)
                {
                    outputFile.WriteLine("-- CLEANING THE DATABASE");

                    if (weapCheck.Checked)
                    {
                        outputFile.WriteLine("DELETE FROM item_loot_scale;");
                        outputFile.WriteLine("DELETE FROM item_template WHERE entry > " + MIN_ENTRY_SCALE + ";");
                        outputFile.WriteLine("DELETE FROM locales_item WHERE entry > " + MIN_ENTRY_SCALE + ";");
                        outputFile.WriteLine("DELETE FROM item_enchantment_template WHERE entry > " + MIN_ENTRY_SCALE + ";");
                    }

                    if (consuCheck.Checked)
                    {
                        outputFile.WriteLine("");
                        outputFile.Write("DELETE FROM item_loot_scale WHERE entry IN (");
                        foreach (Item it in consumables_list)
                        {
                            outputFile.Write(it.entry);
                            if (it != consumables_list.Last())
                                outputFile.Write(",");
                        }
                        outputFile.Write(");");
                    }
                    outputFile.WriteLine();
                }
            }
        }

        public bool ContainsAny(string haystack, string[] needles)
        {
            foreach (string needle in needles)
                if (needle.Length > 1)
                    if (haystack.Contains(needle))
                        return true;

            return false;
        }

        readonly List<int> AURA_SAFE = new List<int>{ (int)AuraType.SPELL_AURA_NONE, (int)AuraType.SPELL_AURA_BIND_SIGHT, (int)AuraType.SPELL_AURA_MOD_POSSESS, (int)AuraType.SPELL_AURA_PERIODIC_DAMAGE,
    (int)AuraType.SPELL_AURA_DUMMY, (int)AuraType.SPELL_AURA_MOD_CHARM, (int)AuraType.SPELL_AURA_PERIODIC_HEAL, (int)AuraType.SPELL_AURA_MOD_DAMAGE_DONE,
    (int)AuraType.SPELL_AURA_MOD_DAMAGE_TAKEN, (int)AuraType.SPELL_AURA_DAMAGE_SHIELD, (int)AuraType.SPELL_AURA_OBS_MOD_HEALTH, (int)AuraType.SPELL_AURA_OBS_MOD_MANA,
    (int)AuraType.SPELL_AURA_MOD_RESISTANCE, (int)AuraType.SPELL_AURA_PERIODIC_TRIGGER_SPELL, (int)AuraType.SPELL_AURA_PERIODIC_ENERGIZE, (int)AuraType.SPELL_AURA_MOD_STAT,
    (int)AuraType.SPELL_AURA_MOD_SKILL, (int)AuraType.SPELL_AURA_MOD_INCREASE_HEALTH, (int)AuraType.SPELL_AURA_MOD_INCREASE_ENERGY, (int)AuraType.SPELL_AURA_46,
    (int)AuraType.SPELL_AURA_48, (int)AuraType.SPELL_AURA_PERIODIC_LEECH, (int)AuraType.SPELL_AURA_MOD_DAMAGE_DONE_CREATURE, (int)AuraType.SPELL_AURA_PERIODIC_HEALTH_FUNNEL,
    (int)AuraType.SPELL_AURA_PERIODIC_MANA_FUNNEL, (int)AuraType.SPELL_AURA_PERIODIC_MANA_LEECH, (int)AuraType.SPELL_AURA_MOD_STALKED, (int)AuraType.SPELL_AURA_SCHOOL_ABSORB,
    (int)AuraType.SPELL_AURA_MOD_POWER_COST_SCHOOL, (int)AuraType.SPELL_AURA_MOD_BASE_RESISTANCE, (int)AuraType.SPELL_AURA_MOD_REGEN, (int)AuraType.SPELL_AURA_MOD_POWER_REGEN,
    (int)AuraType.SPELL_AURA_CHANNEL_DEATH_ITEM, (int)AuraType.SPELL_AURA_MOD_DETECT_RANGE, (int)AuraType.SPELL_AURA_PREVENTS_FLEEING, (int)AuraType.SPELL_AURA_MOD_UNATTACKABLE,
    (int)AuraType.SPELL_AURA_INTERRUPT_REGEN, (int)AuraType.SPELL_AURA_GHOST, (int)AuraType.SPELL_AURA_SPELL_MAGNET, (int)AuraType.SPELL_AURA_MANA_SHIELD,
    (int)AuraType.SPELL_AURA_MOD_SKILL_TALENT, (int)AuraType.SPELL_AURA_MOD_ATTACK_POWER, (int)AuraType.SPELL_AURA_AURAS_VISIBLE, (int)AuraType.SPELL_AURA_MOD_MELEE_ATTACK_POWER_VERSUS,
    (int)AuraType.SPELL_AURA_MOD_TOTAL_THREAT, (int)AuraType.SPELL_AURA_WATER_WALK, (int)AuraType.SPELL_AURA_FEATHER_FALL, (int)AuraType.SPELL_AURA_HOVER,
    (int)AuraType.SPELL_AURA_ADD_FLAT_MODIFIER, (int)AuraType.SPELL_AURA_ADD_TARGET_TRIGGER, (int)AuraType.SPELL_AURA_ADD_CASTER_HIT_TRIGGER, (int)AuraType.SPELL_AURA_OVERRIDE_CLASS_SCRIPTS,
    (int)AuraType.SPELL_AURA_MOD_RANGED_DAMAGE_TAKEN, (int)AuraType.SPELL_AURA_MOD_HEALING, (int)AuraType.SPELL_AURA_MOD_REGEN_DURING_COMBAT, (int)AuraType.SPELL_AURA_MOD_MECHANIC_RESISTANCE,
    (int)AuraType.SPELL_AURA_SHARE_PET_TRACKING, (int)AuraType.SPELL_AURA_UNTRACKABLE, (int)AuraType.SPELL_AURA_EMPATHY, (int)AuraType.SPELL_AURA_MOD_TARGET_RESISTANCE,
    (int)AuraType.SPELL_AURA_MOD_RANGED_ATTACK_POWER, (int)AuraType.SPELL_AURA_MOD_MELEE_DAMAGE_TAKEN, (int)AuraType.SPELL_AURA_RANGED_ATTACK_POWER_ATTACKER_BONUS, (int)AuraType.SPELL_AURA_MOD_POSSESS_PET,
    (int)AuraType.SPELL_AURA_MOD_RANGED_ATTACK_POWER_VERSUS, (int)AuraType.SPELL_AURA_MOD_MANA_REGEN_INTERRUPT, (int)AuraType.SPELL_AURA_MOD_HEALING_DONE, (int)AuraType.SPELL_AURA_FORCE_REACTION,
    (int)AuraType.SPELL_AURA_MOD_RESISTANCE_EXCLUSIVE, (int)AuraType.SPELL_AURA_SAFE_FALL, (int)AuraType.SPELL_AURA_CHARISMA, (int)AuraType.SPELL_AURA_PERSUADED,
    (int)AuraType.SPELL_AURA_MECHANIC_IMMUNITY_MASK, (int)AuraType.SPELL_AURA_RESIST_PUSHBACK, (int)AuraType.SPELL_AURA_SPLIT_DAMAGE_FLAT, (int)AuraType.SPELL_AURA_PET_DAMAGE_MULTI,
    (int)AuraType.SPELL_AURA_MOD_SHIELD_BLOCKVALUE, (int)AuraType.SPELL_AURA_NO_PVP_CREDIT, (int)AuraType.SPELL_AURA_MOD_AOE_AVOIDANCE, (int)AuraType.SPELL_AURA_MOD_HEALTH_REGEN_IN_COMBAT,
    (int)AuraType.SPELL_AURA_POWER_BURN_MANA, (int)AuraType.SPELL_AURA_MOD_CRIT_DAMAGE_BONUS, (int)AuraType.SPELL_AURA_164, (int)AuraType.SPELL_AURA_MELEE_ATTACK_POWER_ATTACKER_BONUS,
    (int)AuraType.SPELL_AURA_MOD_DAMAGE_DONE_VERSUS, (int)AuraType.SPELL_AURA_DETECT_AMORE, (int)AuraType.SPELL_AURA_ALLOW_CHAMPION_SPELLS, (int)AuraType.SPELL_AURA_AOE_CHARM,
    (int)AuraType.SPELL_AURA_MOD_DEBUFF_RESISTANCE, (int)AuraType.SPELL_AURA_MOD_FLAT_SPELL_DAMAGE_VERSUS, (int)AuraType.SPELL_AURA_MOD_FLAT_SPELL_CRIT_DAMAGE_VERSUS };

        List<int> SPELL_EFFECT_RESTRICTED = new List<int> { (int)SpellEffects.SPELL_EFFECT_DUMMY }; // Do not scale those effects
        bool isEffectRestricted(int Effect)
        {
            return SPELL_EFFECT_RESTRICTED.Contains(Effect);
        }

        List<int> EFFECT_IGNORED = new List<int> { (int)SpellEffects.SPELL_EFFECT_NONE, (int)SpellEffects.SPELL_EFFECT_ENCHANT_ITEM, (int)SpellEffects.SPELL_EFFECT_ENCHANT_ITEM_TEMPORARY }; // Do not scale those effects
        bool isEffectMiscIgnored(int Effect)
        {
            // Where EffectMisc is different with a similar Effect and it's OKAY !
            return EFFECT_IGNORED.Contains(Effect);
        }

        private Item findSuitableFood(Item item, int expected_ilevel)
        {
            foreach (Item it in consumables_list.Where(a => a.entry != item.entry))
            {
                if (it.ItemLevel >= expected_ilevel && it.ItemLevel < expected_ilevel + 5)
                    if (it.stackable <= item.stackable)
                        if (it.requiredhonorrank == item.requiredhonorrank)
                            if (it.RequiredReputationRank == item.RequiredReputationRank)
                                if (it._class == item._class)
                                    if (it.subclass == item.subclass)
                                        if (it.bonding == item.bonding)
                                            if (it.InventoryType == item.InventoryType)
                                                if (it.AllowableClass == item.AllowableClass)
                                                    if (it.AllowableRace == item.AllowableRace)
                                                        if (it.GetFood() == item.GetFood())
                                                        {
                                                            bool canKeep = true;
                                                            for (int i = 0; i < 5; i++)
                                                            {
                                                                if (it.spells_ori[i] != null && item.spells_ori[i] != null)
                                                                {
                                                                    int effect1Aura = item.spells_ori[i].effect1Aura;
                                                                    int effect2Aura = item.spells_ori[i].effect2Aura;
                                                                    int effect3Aura = item.spells_ori[i].effect3Aura;

                                                                    int effect1id = item.spells_ori[i].effect1id;
                                                                    int effect2id = item.spells_ori[i].effect2id;
                                                                    int effect3id = item.spells_ori[i].effect3id;

                                                                    int effect1MiscValue = item.spells_ori[i].effect1MiscValue;
                                                                    int effect2MiscValue = item.spells_ori[i].effect2MiscValue;
                                                                    int effect3MiscValue = item.spells_ori[i].effect3MiscValue;

                                                                    if (it.spells_ori[i].effect1Aura != effect1Aura ||
                                                                        it.spells_ori[i].effect2Aura != effect2Aura ||
                                                                        it.spells_ori[i].effect3Aura != effect3Aura ||
                                                                        it.spells_ori[i].effect1id != effect1id ||
                                                                        it.spells_ori[i].effect2id != effect2id ||
                                                                        it.spells_ori[i].effect3id != effect3id ||
                                                                        // it.spells_ori[i].resistancesID != item.spells_ori[i].resistancesID ||
                                                                        (!isEffectMiscIgnored(effect1id) && it.spells_ori[i].effect1MiscValue != effect1MiscValue) ||
                                                                        (!isEffectMiscIgnored(effect2id) && it.spells_ori[i].effect2MiscValue != effect2MiscValue) ||
                                                                        (!isEffectMiscIgnored(effect3id) && it.spells_ori[i].effect3MiscValue != effect3MiscValue) ||
                                                                        isEffectRestricted(effect1id) ||
                                                                        isEffectRestricted(effect2id) ||
                                                                        isEffectRestricted(effect3id))
                                                                        canKeep = false;
                                                                }
                                                            }

                                                            if (canKeep)
                                                                return it;
                                                        }
            }

            return item;
        }

        public void generateQuest()
        {
            Console.Write("Processing startquest_list...");

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "out", "099_startquest.sql")))
            {
                var last = quest_list.Last();
                outputFile.Write("UPDATE item_template SET RequiredLevel = 0 WHERE entry IN (");
                foreach (Item item in quest_list)
                {
                    if (item.Equals(last))
                        outputFile.Write(item.entry + ");\n");
                    else
                        outputFile.Write(item.entry + ",");
                    Console.Write(".");
                }
            }
            Console.WriteLine();
        }

        public void generateScaleFood()
        {
            Console.Write("Processing consumables_list...");

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "out", "099_food.sql")))
            {
                foreach (Item it in item_list.Values)
                {
                    if (consumables_list.Contains(it) && it.ItemLevel > 0 && ((filter.Items.Count == 0) || filter.Items.Count != 0 && filter.Items.Contains(it.entry)))
                    {
                        Dictionary<int, int> tmp_list = new Dictionary<int, int>();
                        for (int ilevel = 10; ilevel <= 75; ilevel += 5)
                        {
                            Item item_replacement = findSuitableFood(it, ilevel);
                            int r_level = item_replacement.RequiredLevel > 0 ? item_replacement.RequiredLevel : item_replacement.ItemLevel - 10;

                            if (item_replacement.entry != it.entry && it.ItemLevel != item_replacement.ItemLevel)
                                if (!tmp_list.ContainsKey(r_level))
                                    tmp_list.Add(r_level, item_replacement.entry);
                        }

                        if (tmp_list.Count() > 0)
                        {
                            outputFile.WriteLine("-- REPLACEMENT FOR " + it.name + " [" + it.entry + "]");
                            outputFile.Write("REPLACE INTO item_loot_scale VALUES ");

                            var last = tmp_list.Last();
                            foreach (var item in tmp_list)
                            {
                                if (item.Equals(last))
                                    outputFile.Write("(" + it.entry + "," + item.Value + "," + item.Key + ");\n");
                                else
                                    outputFile.Write("(" + it.entry + "," + item.Value + "," + item.Key + "),");
                            }
                        }
                    }
                    Console.Write(".");
                }
            }
            Console.WriteLine();
        }

        public static int[] buyprice = new int[] { 0, 36, 71, 71, 88, 206, 380, 399, 538, 1041, 2264, 3429, 5109, 6267, 7377, 9375, 10699, 12471, 14208, 15429, 18197, 20859, 23269, 26712, 27500, 29185, 31948, 34712, 36528, 41695, 48302, 53949, 62051, 72886, 77373, 83183, 90316, 94951, 107665, 121033, 127320, 131048, 142936, 165169, 168534, 180695, 196443, 208389, 220206, 230000, 239377, 245000, 251259, 265320, 279382, 293444, 328092, 365580, 405649, 428242, 454890, 460000, 470000, 480000, 500000, 510000, 520000, 530000, 540000, 550000, 560000, 570000, 886627 };
        public static int GetPrice(int BuyPrice, int RequiredLevel, int pLevel)
        {
            double expected_buyprice = buyprice[RequiredLevel];
            double buyprice_ratio = BuyPrice / expected_buyprice;
            double expected_scaled_buyprice = buyprice[pLevel] * buyprice_ratio;

            return (int)(expected_scaled_buyprice);
        }

        static double MAX_SOCKET_DIF = 0.10;
        private static void ScaleSocketBonus(ref Item item, double coeffR)
        {
            socketBonus current_socket = item.socketBonus;

            if (current_socket == null)
                return;

            double s_current = current_socket.value;
            double lookup_value = Math.Max(s_current * coeffR, 1); // valeur recherchée

            double closest_diff = 99999;    // diff du dernier sort trouvé
            double closest_value = 1;       // valeur du dernier sort trouvé
            socketBonus closest_socket = new socketBonus();

            foreach (socketBonus s in socketBonus_list.Values.Where(a => a.family == current_socket.family))
            {
                double s_value = s.value;
                double diff_value = Math.Abs(s_value - lookup_value);
                double diff_test2 = diff_value / lookup_value;

                if ((s_value <= lookup_value && diff_value <= closest_diff) || diff_test2 <= MAX_SOCKET_DIF)
                {
                    closest_value = s_value;
                    closest_diff = diff_value;
                    closest_socket = new socketBonus(s);
                }

                if (closest_socket.id != 0)
                    item.socketBonus_new = closest_socket;
            }
        }

        static double MAX_RPROPERTY_DIF = 0.15;
        private static void ScaleRandomProperty(ref Item item, double coeffR)
        {
            double average_e = 0;

            foreach (Enchantment e in item.enchantments_ori)
                average_e += e.value[item.Quality <= (int)ItemQualities.ITEM_QUALITY_RARE ? 0 : 1];

            double average_value = Math.Max((average_e / item.enchantments_ori.Count()) * coeffR, 1);

            foreach (Enchantment e in item.enchantments_ori)
            {
                double current_e = e.value[item.Quality <= (int)ItemQualities.ITEM_QUALITY_RARE ? 0 : 1];
                double current_value = Math.Max(current_e * coeffR, 1); // valeur recherchée

                double lookup_value = current_value < average_value ? average_value - (current_value * MAX_RPROPERTY_DIF) : average_value + (current_value * MAX_RPROPERTY_DIF);

                double closest_diff = 99999;    // diff du dernier sort trouvé
                double closest_value = 1;       // valeur du dernier sort trouvé
                Enchantment closest_ench = new Enchantment();

                foreach (Enchantment l in Enchantment_list.Values.Where(a => a.family == e.family))
                {
                    double l_value = l.value[item.Quality <= (int)ItemQualities.ITEM_QUALITY_RARE ? 0 : 1];
                    double diff_value = Math.Abs(l_value - lookup_value);
                    double diff_test2 = diff_value / lookup_value;

                    if ((l_value <= lookup_value && diff_value <= closest_diff) || diff_test2 <= MAX_RPROPERTY_DIF)
                    {
                        closest_value = l_value;
                        closest_diff = diff_value;
                        closest_ench = new Enchantment(l);
                    }
                }

                if (closest_ench.id != 0)
                {
                    closest_ench.chance = e.chance;
                    closest_ench.ilevel = item.sItemLevel;
                    item.enchantments_new.Add(closest_ench);
                    // MessageBox.Show("Has '" + e.name + "' (+" + e_current + ")(" + e.chance + ")\n" + "Looking for '" + e.name + "' (+" + lookup_value + ")\n" + "Found '" + closest_ench.name + "' (+" + closest_value + ")(" + closest_ench.chance + ")");
                }
            }
        }

        public static int GetBlock(Item it)
        {
            double block = it.block;
            int it_class = it._class;
            int it_subclass = it.subclass;
            int it_quality = it.Quality;
            int it_ilevel = it.sItemLevel;

            if (block != 0)
            {
                if (it_class == 4)
                {
                    if (it_subclass == 6) // shields
                    {
                        block = -11.8776 + 2.5229 * it_ilevel - 0.141684 * it_ilevel * it_ilevel + 0.004191 * it_ilevel * it_ilevel * it_ilevel - 0.00005836963 * it_ilevel * it_ilevel * it_ilevel * it_ilevel + 0.00000038712964 * it_ilevel * it_ilevel * it_ilevel * it_ilevel * it_ilevel - 0.0000000009834832 * it_ilevel * it_ilevel * it_ilevel * it_ilevel * it_ilevel * it_ilevel;

                        switch (it_quality)
                        {
                            case (int)ItemQualities.ITEM_QUALITY_POOR:
                                block *= 0.76;
                                break;
                            case (int)ItemQualities.ITEM_QUALITY_NORMAL:
                                block *= 0.95;
                                break;
                            case (int)ItemQualities.ITEM_QUALITY_UNCOMMON:
                                // do nothing
                                break;
                            case (int)ItemQualities.ITEM_QUALITY_RARE:
                                if (it_ilevel < 65)
                                    block += 4;
                                else
                                    block += 5;
                                break;
                            case (int)ItemQualities.ITEM_QUALITY_EPIC:
                            case (int)ItemQualities.ITEM_QUALITY_LEGENDARY:
                            case (int)ItemQualities.ITEM_QUALITY_ARTIFACT:
                                block = 0.00214 * it_ilevel * it_ilevel + 0.897 * it_ilevel - 10;
                                break;
                        }
                    }
                }
            }

            return (int)block;
        }

        public static int SetArmorFactor(Item it, double expected_armor)
        {
            switch (it.InventoryType)
            {
                case 5:
                    expected_armor *= 1;
                    break;
                case 6:
                    expected_armor *= 0.5625;
                    break;
                case 1:
                    expected_armor *= 0.8125;
                    break;
                case 10:
                    expected_armor *= 0.625;
                    break;
                case 7:
                    expected_armor *= 0.875;
                    break;
                case 3:
                    expected_armor *= 0.75;
                    break;
                case 8:
                    expected_armor *= 0.6875;
                    break;
                case 16:
                    expected_armor *= 0.5;
                    break;
                case 9:
                    expected_armor *= 0.43;
                    break;
            }

            if (it.Quality > (int)ItemQualities.ITEM_QUALITY_RARE)
                expected_armor *= 1.25;

            return Convert.ToInt32(expected_armor);
        }

        public static int GetArmor(Item it, int it_ilevel)
        {
            double armor = it.armor;
            int it_class = it._class;
            int it_subclass = it.subclass;
            int it_quality = it.Quality;

            if (armor != 0)
            {
                if (it_class == 4)
                {
                    if (it_subclass == 1) //cloth
                    {
                        if (it_ilevel >= 10 && it_ilevel < 45)
                            armor = -9.52 + 3.82 * it_ilevel - 0.109 * it_ilevel * it_ilevel + 0.00137 * it_ilevel * it_ilevel * it_ilevel - 0.00000173 * it_ilevel * it_ilevel * it_ilevel * it_ilevel;
                        else if (it_ilevel >= 45 && it_ilevel <= 93)
                            armor = 1.22 * it_ilevel + 4.02;
                        else if (it_ilevel >= 93)
                            armor = 1.19 * it_ilevel + 5.1;

                        switch (it_quality)
                        {
                            case (int)ItemQualities.ITEM_QUALITY_POOR:
                                armor *= 0.883;
                                break;
                            case (int)ItemQualities.ITEM_QUALITY_NORMAL:
                                armor *= 0.94;
                                break;
                            case (int)ItemQualities.ITEM_QUALITY_UNCOMMON:
                                // do nothing
                                break;
                            case (int)ItemQualities.ITEM_QUALITY_RARE:
                                armor *= 1.10;
                                break;
                            case (int)ItemQualities.ITEM_QUALITY_EPIC:
                            case (int)ItemQualities.ITEM_QUALITY_LEGENDARY:
                            case (int)ItemQualities.ITEM_QUALITY_ARTIFACT:
                                armor *= (it_ilevel < 93 ? 1.20 : 1.375);
                                break;
                        }
                    }

                    if (it_subclass == 2) //leather
                    {
                        if (it_ilevel >= 10 && it_ilevel < 45)
                            armor = 7.07 + 7.03 * it_ilevel - 0.265 * it_ilevel * it_ilevel + 0.0053 * it_ilevel * it_ilevel * it_ilevel - 0.0000361 * it_ilevel * it_ilevel * it_ilevel * it_ilevel;
                        else if (it_ilevel >= 45 && it_ilevel <= 93)
                            armor = 2.15 * it_ilevel + 25.2;
                        else if (it_ilevel >= 93)
                            armor = 2.22 * it_ilevel + 10;

                        switch (it_quality)
                        {
                            case (int)ItemQualities.ITEM_QUALITY_POOR:
                                armor *= 0.883;
                                break;
                            case (int)ItemQualities.ITEM_QUALITY_NORMAL:
                                armor *= 0.94;
                                break;
                            case (int)ItemQualities.ITEM_QUALITY_UNCOMMON:
                                // do nothing
                                break;
                            case (int)ItemQualities.ITEM_QUALITY_RARE:
                                armor *= 1.10;
                                break;
                            case (int)ItemQualities.ITEM_QUALITY_EPIC:
                            case (int)ItemQualities.ITEM_QUALITY_LEGENDARY:
                            case (int)ItemQualities.ITEM_QUALITY_ARTIFACT:
                                armor *= (it_ilevel < 93 ? 1.20 : 1.375);
                                break;
                        }
                    }

                    if (it_subclass == 3) //mail
                    {
                        if (it_ilevel >= 10 && it_ilevel < 45)
                            armor = -59.6 + 24.9 * it_ilevel - 1.03 * it_ilevel * it_ilevel + 0.0205 * it_ilevel * it_ilevel * it_ilevel - 0.000146 * it_ilevel * it_ilevel * it_ilevel * it_ilevel;
                        else if (it_ilevel >= 45 && it_ilevel <= 93)
                            armor = 4.9 * it_ilevel + 28.5;
                        else if (it_ilevel >= 93)
                            armor = 4.9 * it_ilevel + 29;

                        switch (it_quality)
                        {
                            case (int)ItemQualities.ITEM_QUALITY_POOR:
                                armor *= 0.883;
                                break;
                            case (int)ItemQualities.ITEM_QUALITY_NORMAL:
                                armor *= 0.94;
                                break;
                            case (int)ItemQualities.ITEM_QUALITY_UNCOMMON:
                                // do nothing
                                break;
                            case (int)ItemQualities.ITEM_QUALITY_RARE:
                                armor *= 1.10;
                                break;
                            case (int)ItemQualities.ITEM_QUALITY_EPIC:
                            case (int)ItemQualities.ITEM_QUALITY_LEGENDARY:
                            case (int)ItemQualities.ITEM_QUALITY_ARTIFACT:
                                armor *= (it_ilevel < 93 ? 1.20 : 1.375);
                                break;
                        }
                    }

                    if (it_subclass == 4) //mail
                    {
                        if (it_ilevel >= 40 && it_ilevel < 45)
                            armor = 47.6 * it_ilevel - 1663;
                        else if (it_ilevel >= 45 && it_ilevel <= 93)
                            armor = 8.94 * it_ilevel + 33.5;
                        else if (it_ilevel >= 93)
                            armor = 9 * it_ilevel + 23;

                        switch (it_quality)
                        {
                            case (int)ItemQualities.ITEM_QUALITY_POOR:
                                armor *= 0.883;
                                break;
                            case (int)ItemQualities.ITEM_QUALITY_NORMAL:
                                armor *= 0.94;
                                break;
                            case (int)ItemQualities.ITEM_QUALITY_UNCOMMON:
                                // do nothing
                                break;
                            case (int)ItemQualities.ITEM_QUALITY_RARE:
                                armor *= 1.10;
                                break;
                            case (int)ItemQualities.ITEM_QUALITY_EPIC:
                            case (int)ItemQualities.ITEM_QUALITY_LEGENDARY:
                            case (int)ItemQualities.ITEM_QUALITY_ARTIFACT:
                                armor *= (it_ilevel < 93 ? 1.20 : 1.375);
                                break;
                        }
                    }

                    if (it_subclass == 6) // shields
                    {
                        if (it_ilevel >= 10 && it_ilevel < 14)
                            armor = 40 * it_ilevel - 166;
                        else if (it_ilevel >= 14 && it_ilevel <= 39)
                            armor = 16.7 * it_ilevel + 78;
                        else if (it_ilevel >= 40 && it_ilevel <= 44)
                            armor = 97 * it_ilevel - 2916;
                        else if (it_ilevel > 44)
                            armor = 28.3 * it_ilevel + 135;

                        switch (it_quality)
                        {
                            case (int)ItemQualities.ITEM_QUALITY_POOR:
                                armor *= 0.883;
                                break;
                            case (int)ItemQualities.ITEM_QUALITY_NORMAL:
                                armor *= 0.94;
                                break;
                            case (int)ItemQualities.ITEM_QUALITY_UNCOMMON:
                                // do nothing
                                break;
                            case (int)ItemQualities.ITEM_QUALITY_RARE:
                                armor *= 1.122;
                                break;
                            case (int)ItemQualities.ITEM_QUALITY_EPIC:
                                armor *= 1.25;
                                break;
                            case (int)ItemQualities.ITEM_QUALITY_LEGENDARY:
                            case (int)ItemQualities.ITEM_QUALITY_ARTIFACT:
                                if (it_ilevel >= 90 && it_ilevel <= 92)
                                    armor = 221 * it_ilevel - 16396;
                                else if (it_ilevel > 93)
                                    armor *= 1.436;
                                else
                                    armor *= 1.25;
                                break;
                        }
                    }
                }
            }

            armor = SetArmorFactor(it, armor);  // Allow the proper ammount of armor relative to the slot mod.
            return (int)armor;
        }

        public static double GetDps(Item it, int it_ilevel, bool Force1H = false)  //Variable Force1H est rajouté pour forcer la fonction à donner le DPS d'une arme à 1 main.(Utilisé pour le calcul du DPS sacrifice, même si l'arme est une arme 2H)
        {
            int it_class = it._class;
            int it_subclass = it.subclass;
            int it_quality = it.Quality;

            if (Force1H)
            {
                it_class = 2;
                it_subclass = 0;
            }

            double dps = 0;

            if (it_class == 2) // weapons
            {
                if (new[] { 0, 4, 7, 13, 15 }.Contains(it_subclass)) // one handed + fist + dagger OK
                {
                    if (it_quality == (int)ItemQualities.ITEM_QUALITY_POOR)
                    {
                        if (it_ilevel < 35)
                            dps = 0.513 * it_ilevel + 0.3;
                        else if (it_ilevel >= 35 && it_ilevel < 46)
                            dps = 0.84 * it_ilevel - 11.2;
                        else if (it_ilevel >= 46 && it_ilevel < 75)
                            dps = 0.6 * it_ilevel + 0.2;
                        else if (it_ilevel >= 75 && it_ilevel < 97) // tbc
                            dps = 0.3448 * it_ilevel + 16.7552;
                        else if (it_ilevel >= 97)
                            dps = 0.6333 * it_ilevel - 10.7;

                        dps *= 0.65;

                    }
                    else if (it_quality == (int)ItemQualities.ITEM_QUALITY_NORMAL)
                    {
                        if (it_ilevel < 35)
                            dps = 0.513 * it_ilevel + 0.3;
                        else if (it_ilevel >= 35 && it_ilevel < 46)
                            dps = 0.84 * it_ilevel - 11.2;
                        else if (it_ilevel >= 46 && it_ilevel < 75)
                            dps = 0.6 * it_ilevel + 0.2;
                        else if (it_ilevel >= 75 && it_ilevel < 97) // tbc
                            dps = 0.3448 * it_ilevel + 16.7552;
                        else if (it_ilevel >= 97)
                            dps = 0.6333 * it_ilevel - 10.7;

                        dps *= 0.90;
                    }
                    else if (it_quality == (int)ItemQualities.ITEM_QUALITY_UNCOMMON) // green
                    {
                        if (it_ilevel < 35)
                            dps = 0.513 * it_ilevel + 0.3;
                        else if (it_ilevel >= 35 && it_ilevel < 46)
                            dps = 0.84 * it_ilevel - 11.2;
                        else if (it_ilevel >= 46 && it_ilevel < 75)
                            dps = 0.6 * it_ilevel + 0.2;
                        else if (it_ilevel >= 75 && it_ilevel < 97) // tbc
                            dps = 0.3448 * it_ilevel + 16.7552;
                        else if (it_ilevel >= 97)
                            dps = 0.6333 * it_ilevel - 10.7;
                    }
                    else if (it_quality == (int)ItemQualities.ITEM_QUALITY_RARE) // blue
                    {
                        if (it_ilevel < 32)
                            dps = 0.518 * it_ilevel + 3.14;
                        else if (it_ilevel >= 32 && it_ilevel < 42)
                            dps = 0.88 * it_ilevel - 8.08;
                        else if (it_ilevel >= 42 && it_ilevel < 75)
                            dps = 0.613 * it_ilevel + 2.7;
                        else if (it_ilevel >= 75 && it_ilevel < 97) // tbc
                            dps = 0.4350 * it_ilevel + 15.8250;
                        else if (it_ilevel >= 97)
                            dps = 0.7488 * it_ilevel - 14.4905;
                    }
                    else if (it_quality >= (int)ItemQualities.ITEM_QUALITY_EPIC) // epic
                    {
                        if (it_ilevel < 92)
                            dps = 1.466769 + 1.092133 * it_ilevel - 0.0133254145 * it_ilevel * it_ilevel + 0.00011351298 * it_ilevel * it_ilevel * it_ilevel;
                        else if (it_ilevel >= 92 && it_ilevel < 138)
                            dps = 0.45 * it_ilevel + 36.1;
                        else if (it_ilevel >= 138)
                            dps = 0.6 * it_ilevel + 15.5;
                    }
                }
                else if (new[] { 1, 5, 8, 10, 17, 20 }.Contains(it_subclass)) // two handed + staff + spear + fishing pole OK
                {
                    if (it_quality == (int)ItemQualities.ITEM_QUALITY_POOR)
                    {
                        if (it_ilevel < 35)
                            dps = 0.513 * it_ilevel + 0.3;
                        else if (it_ilevel >= 35 && it_ilevel < 46)
                            dps = 0.84 * it_ilevel - 11.2;
                        else if (it_ilevel >= 46 && it_ilevel < 75)
                            dps = 0.6 * it_ilevel + 0.2;
                        else if (it_ilevel >= 75 && it_ilevel < 97) // tbc
                            dps = 0.3448 * it_ilevel + 16.7552;
                        else if (it_ilevel >= 97)
                            dps = 0.6333 * it_ilevel - 10.7;

                        dps *= 1.30 * 0.65;

                    }
                    else if (it_quality == (int)ItemQualities.ITEM_QUALITY_NORMAL)
                    {
                        if (it_ilevel < 35)
                            dps = 0.513 * it_ilevel + 0.3;
                        else if (it_ilevel >= 35 && it_ilevel < 46)
                            dps = 0.84 * it_ilevel - 11.2;
                        else if (it_ilevel >= 46 && it_ilevel < 75)
                            dps = 0.6 * it_ilevel + 0.2;
                        else if (it_ilevel >= 75 && it_ilevel < 97) // tbc
                            dps = 0.3448 * it_ilevel + 16.7552;
                        else if (it_ilevel >= 97)
                            dps = 0.6333 * it_ilevel - 10.7;

                        dps *= 1.30 * 0.90;
                    }
                    else if (it_quality == (int)ItemQualities.ITEM_QUALITY_UNCOMMON) // green
                    {
                        if (it_ilevel < 35)
                            dps = 0.513 * it_ilevel + 0.3;
                        else if (it_ilevel >= 35 && it_ilevel < 46)
                            dps = 0.84 * it_ilevel - 11.2;
                        else if (it_ilevel >= 46 && it_ilevel < 75)
                            dps = 0.6 * it_ilevel + 0.2;
                        else if (it_ilevel >= 75 && it_ilevel < 97) // tbc
                            dps = 0.3448 * it_ilevel + 16.7552;
                        else if (it_ilevel >= 97)
                            dps = 0.6333 * it_ilevel - 10.7;

                        dps *= 1.30;
                    }
                    else if (it_quality == (int)ItemQualities.ITEM_QUALITY_RARE) // blue
                    {
                        if (it_ilevel < 72)   //Vanilla
                            dps = 0.86 * it_ilevel;
                        else if (it_ilevel >= 72 && it_ilevel < 97) // tbc
                            dps = 1.3 * (0.4350 * it_ilevel + 15.8250);
                        else if (it_ilevel >= 97)
                            dps = 1.3 * (0.7488 * it_ilevel - 14.4905);
                    }
                    else if (it_quality >= (int)ItemQualities.ITEM_QUALITY_EPIC) // epic
                    {
                        if (it_ilevel < 92)
                            dps = 1.305 * (1.466769 + 1.092133 * it_ilevel - 0.0133254145 * it_ilevel * it_ilevel + 0.00011351298 * it_ilevel * it_ilevel * it_ilevel);
                        else if (it_ilevel >= 92 && it_ilevel < 138) //TBC
                            dps = 1.3 * (0.45 * it_ilevel + 36.1);
                        else if (it_ilevel >= 138)
                            dps = 1.3 * (0.6 * it_ilevel + 15.5);

                    }
                }
                else if (new[] { 2, 3, 18, 16 }.Contains(it_subclass)) // ranged (bow, gun, crossbow) & thrown (too many differents cases for thrown...)
                {
                    if (it_quality == (int)ItemQualities.ITEM_QUALITY_POOR)
                    {
                        if (it_ilevel < 37)
                            dps = 0.407 * it_ilevel - 0.142;
                        else if (it_ilevel >= 37 && it_ilevel < 45)
                            dps = 0.693 * it_ilevel - 10.4;
                        else if (it_ilevel >= 46 && it_ilevel < 75)
                            dps = 0.417 * it_ilevel + 2.75;
                        else if (it_ilevel >= 75) // tbc
                            dps = 0.5 * it_ilevel + 1.4;

                        dps *= 0.65;

                    }
                    else if (it_quality == (int)ItemQualities.ITEM_QUALITY_NORMAL)
                    {
                        if (it_ilevel < 37)
                            dps = 0.407 * it_ilevel - 0.142;
                        else if (it_ilevel >= 37 && it_ilevel < 45)
                            dps = 0.693 * it_ilevel - 10.4;
                        else if (it_ilevel >= 46 && it_ilevel < 75)
                            dps = 0.417 * it_ilevel + 2.75;
                        else if (it_ilevel >= 75) // tbc
                            dps = 0.5 * it_ilevel + 1.4;

                        dps *= 0.90;
                    }
                    else if (it_quality == (int)ItemQualities.ITEM_QUALITY_UNCOMMON) // green
                    {
                        if (it_ilevel < 37)
                            dps = 0.407 * it_ilevel - 0.142;
                        else if (it_ilevel >= 37 && it_ilevel < 45)
                            dps = 0.693 * it_ilevel - 10.4;
                        else if (it_ilevel >= 46 && it_ilevel < 79)
                            dps = 0.417 * it_ilevel + 2.75;
                        else if (it_ilevel >= 79) // tbc
                            dps = 0.5 * it_ilevel + 1.4;

                    }
                    else if (it_quality == (int)ItemQualities.ITEM_QUALITY_RARE) // blue 
                    {
                        if (it_ilevel < 30)
                            dps = 0.41 * it_ilevel + 2.33;
                        else if (it_ilevel >= 32 && it_ilevel < 41)
                            dps = 0.677 * it_ilevel - 5.74;
                        else if (it_ilevel >= 41 && it_ilevel < 75)
                            dps = 0.474 * it_ilevel + 2.43;
                        else if (it_ilevel >= 79) // tbc
                            dps = 0.58 * it_ilevel - 0.3;
                    }
                    else if (it_quality >= (int)ItemQualities.ITEM_QUALITY_EPIC) // epic
                    {
                        if (it_ilevel < 88)
                            dps = 2.38 + 0.426 * it_ilevel + 0.00807 * it_ilevel * it_ilevel - 0.000187 * it_ilevel * it_ilevel * it_ilevel + 0.00000135 * it_ilevel * it_ilevel * it_ilevel * it_ilevel;
                        else if (it_ilevel >= 88)
                            dps = 0.4047 * it_ilevel + 32.84;
                    }
                }
                // else if (new[] { 16 }.Contains(it_subclass)) // thrown
                // {

                // }
                else if (new[] { 19 }.Contains(it_subclass)) // wand OK
                {
                    if (it_quality == (int)ItemQualities.ITEM_QUALITY_POOR)
                    {
                        if (it_ilevel < 85)
                            dps = 0.372 + 0.668 * it_ilevel + 0.00338 * it_ilevel * it_ilevel;
                        else if (it_ilevel >= 85 && it_ilevel < 97) // tbc
                            dps = 1.77 * (0.3448 * it_ilevel + 16.7552);
                        else if (it_ilevel >= 97)
                            dps = 1.77 * (0.6333 * it_ilevel - 10.7);

                        dps *= 0.65;

                    }
                    else if (it_quality == (int)ItemQualities.ITEM_QUALITY_NORMAL)
                    {
                        if (it_ilevel < 85)
                            dps = 0.372 + 0.668 * it_ilevel + 0.00338 * it_ilevel * it_ilevel;
                        else if (it_ilevel >= 85 && it_ilevel < 97) // tbc
                            dps = 1.77 * (0.3448 * it_ilevel + 16.7552);
                        else if (it_ilevel >= 97)
                            dps = 1.77 * (0.6333 * it_ilevel - 10.7);

                        dps *= 0.90;
                    }
                    else if (it_quality == (int)ItemQualities.ITEM_QUALITY_UNCOMMON) // green
                    {
                        if (it_ilevel < 85)
                            dps = 0.372 + 0.668 * it_ilevel + 0.00338 * it_ilevel * it_ilevel;
                        else if (it_ilevel >= 85 && it_ilevel < 97) // tbc
                            dps = 1.77 * (0.3448 * it_ilevel + 16.7552);
                        else if (it_ilevel >= 97)
                            dps = 1.77 * (0.6333 * it_ilevel - 10.7);
                    }
                    else if (it_quality == (int)ItemQualities.ITEM_QUALITY_RARE) // blue
                    {
                        if (it_ilevel < 80)   //Vanilla
                            dps = 4.93 + 0.83 * it_ilevel - 0.00346 * it_ilevel * it_ilevel + 0.0000822 * it_ilevel * it_ilevel * it_ilevel;
                        else if (it_ilevel >= 80 && it_ilevel < 97) // tbc
                            dps = 1.8 * (0.4350 * it_ilevel + 15.8250);
                        else if (it_ilevel >= 97)
                            dps = 1.8 * (0.7488 * it_ilevel - 14.4905);
                    }
                    else if (it_quality >= (int)ItemQualities.ITEM_QUALITY_EPIC) // epic
                    {
                        if (it_ilevel < 60)
                            dps = 1.15 * (4.93 + 0.83 * it_ilevel - 0.00346 * it_ilevel * it_ilevel + 0.0000822 * it_ilevel * it_ilevel * it_ilevel);
                        else if (it_ilevel >= 60 && it_ilevel < 92)
                            dps = 76.7 - 1.63 * it_ilevel + 0.025 * it_ilevel * it_ilevel;
                        else if (it_ilevel >= 93 && it_ilevel < 138) //TBC
                            dps = 1.83 * (0.45 * it_ilevel + 36.1);
                        else if (it_ilevel >= 138)
                            dps = 1.83 * (0.6 * it_ilevel + 15.5);
                    }
                }
            }

            return (double)dps;
        }

        static double MAX_SPELL_DIF = 0.30;
        static public Spell findSuitableSpell(Dictionary<int, Spell> spell_shortlist, Spell mainspell, double coeffR, ref double closest_value)
        {
            double lookup_value = Math.Max(mainspell.GetPoints() * coeffR, 1);
            double closest_diff = 99999;
            Spell closest_spell = new Spell();

            foreach (Spell s in spell_shortlist.Values.Where(a => !a.spellname_loc0.ToLower().Contains("zz")))
            {
                double s_value = s.GetPoints();
                double diff_value = Math.Abs(s_value - lookup_value);
                double diff_test2 = diff_value / lookup_value;

                if ((s_value <= lookup_value && diff_value <= closest_diff) || diff_test2 <= MAX_SPELL_DIF)
                {
                    closest_value = s_value;
                    closest_diff = diff_value;
                    closest_spell = new Spell(s);
                }
            }

            if (closest_spell.spellID != 0)
            {
                closest_spell.spelltrigger = mainspell.spelltrigger;
                closest_spell.spellcharges = mainspell.spellcharges;
                closest_spell.spellppmRate = mainspell.spellppmRate;
                closest_spell.spellcooldown = mainspell.spellcooldown;
                closest_spell.spellcategory = mainspell.spellcategory;
                closest_spell.spellcategorycooldown = mainspell.spellcategorycooldown;

                return closest_spell;
            }

            return null;
        }

        public static float ToSingle(double value)
        {
            return (float)value;
        }

        private static void ScaleSpell(ref Item it, double coeffR)
        {
            foreach (Spell lookup_spell in it.spells_ori.Where(a => a != null))
            {
                if (!lookup_spell.IsHandled())
                {
                    if (Math.Abs(coeffR - 1) > 0.03)  //Pas de changement de spell si coeffR est proche de 1.
                    {

                        double lookup_value = Math.Max(lookup_spell.GetPoints(), 1);
                        double closest_value = 1;

                        Spell closest_spell = null;

                        if (lookup_spell.spelltrigger != 0)// On interdit le changement de spell pour les on-use spell.
                            closest_spell = findSuitableSpell(it.spell_shortlist, lookup_spell, coeffR, ref closest_value);

                        if (closest_spell == null)
                        {
                            closest_spell = new Spell(lookup_spell);
                            closest_value = lookup_value;
                        }

                        int spelltrigger = closest_spell.spelltrigger;

                        if (spelltrigger == 2) // on hit
                        {
                            double spellppmRate = lookup_spell.spellppmRate;

                            if (spellppmRate == 0)
                            {
                                int procChance = lookup_spell.procChance;
                                if (procChance != 101)
                                    spellppmRate = procChance * 60000.0d / it.delay; // (ms)
                                else
                                    spellppmRate = 0.2f;
                            }

                            double outppmRate = ToSingle(coeffR * spellppmRate * (lookup_value / closest_value));
                            closest_spell.spellppmRate = outppmRate;
                        }
                        else if (spelltrigger == 1) // on equip (handled by cmangos-tbc)
                        {
                            /* int spellcooldown = lookup_spell.spellcooldown; // cooldown stocké dans l'objet (ms)
                            if (spellcooldown > 0)
                                closest_spell.spellcooldown = (int)(spellcooldown / Math.Min(2, coeffR * (lookup_value / closest_value))); */
                        }
                        else if (spelltrigger == 0) // on use
                        {
                            int spellcooldown = lookup_spell.spellcooldown; // cooldown stocké dans l'objet (ms)
                            if (spellcooldown != -1)
                                closest_spell.spellcooldown = (int)(spellcooldown / Math.Min(2, coeffR * (lookup_value / closest_value)));
                        }

                        it.SetClassicSpell(closest_spell);
                    }
                    else { it.SetClassicSpell(lookup_spell); }
                }
            }
        }

        private static double getCoeffA(int ilvl, int quality)
        {
            if (quality < (int)ItemQualities.ITEM_QUALITY_RARE)
                return 2.0;
            if (quality == (int)ItemQualities.ITEM_QUALITY_RARE)
                return 1.8;
            if (quality > (int)ItemQualities.ITEM_QUALITY_RARE)
            {
                if (ilvl < 95)
                    return 1.7;
                else
                    return 1.2;
            }
            return 1.8;
        }

        private static double getCoeffB(int ilvl, int quality)
        {
            if (quality < (int)ItemQualities.ITEM_QUALITY_RARE)
                return 8.0;
            if (quality == (int)ItemQualities.ITEM_QUALITY_RARE)
                return 0.75;
            if (quality > (int)ItemQualities.ITEM_QUALITY_RARE)
            {
                if (ilvl < 95)
                    return -10.0;
                else
                    return 24.0;
            }
            return 8.0;
        }

        static double alpha = 1.705;

        private static double getCoeffR(int ilevel_in, int ilevel_out, int quality_in, int quality_out)
        {
            double origin_coeffA = getCoeffA(ilevel_in, quality_in);
            double origin_coeffB = getCoeffB(ilevel_in, quality_in);

            double it_coeffA1 = getCoeffA(ilevel_out, quality_out);
            double it_coeffB1 = getCoeffB(ilevel_out, quality_out);

            double coeffR1 = ((ilevel_out - it_coeffB1) / it_coeffA1) / ((ilevel_in - origin_coeffB) / origin_coeffA);  // coeffR en prennant en compte un eventuel quality bonus

            return double.IsInfinity(coeffR1) ? 1 : coeffR1;
        }

        public static void ScaleItem(ref Item it, Item it_ori)
        {
            double coeffR = getCoeffR(it_ori.ItemLevel, it.sItemLevel, it_ori.Quality, it.Quality);

            List<double> stats_redo = new List<double>(new double[Item.ITEM_MOD_MAX]);
            List<int> stats_sign = new List<int>(new int[Item.ITEM_MOD_MAX]);

            List<double> spells_values = new List<double>(new double[50]);
            List<double> spells_malus = new List<double>(new double[50]);
            List<double> spells_bonus = new List<double>(new double[50]);

            if (it._class == (int)ItemClass.ITEM_CLASS_ARMOR)
            {
                /* if (it.subclass == 3 && it.ItemLevel >= 45 && it.sItemLevel < 45)  // Mail converted to Leather below level 40 (ilvl 45)
                    it.subclass = 2; */

                if (it.subclass == (int)ItemSubclassArmor.ITEM_SUBCLASS_ARMOR_PLATE && it.ItemLevel >= 45 && it.sItemLevel < 45)
                    it.subclass = (int)ItemSubclassArmor.ITEM_SUBCLASS_ARMOR_MAIL;

                if (it.subclass == (int)ItemSubclassArmor.ITEM_SUBCLASS_ARMOR_MAIL && it.ItemLevel <= 45 && it.sItemLevel >= 45)
                    it.subclass = (int)ItemSubclassArmor.ITEM_SUBCLASS_ARMOR_PLATE;
            }

            // Doing work on armor
            double scaled_armor = GetArmor(it, it.sItemLevel);
            it.armor = (int)(scaled_armor + (it.bonus_armor * coeffR));
            it.block = GetBlock(it);

            // Doing work on damage
            double dps_scaled = GetDps(it, it.sItemLevel);
            double dps_scaledNoBQ = GetDps(it_ori, it.sItemLevel);
            double dps_expect = GetDps(it_ori, it.ItemLevel);
            double dps_origin = (it_ori.delay < 1) ? 0 : 1000.0 * (it_ori.damages_ori[0].max + it_ori.damages_ori[0].min) / (2 * (double)it_ori.delay);

            // DPS Sacrifice
            if (it._class == 2)
            {
                // DPS sacrifice : Start
                // Cette partie doit passer avant le pathing
                // Cette partie permet de déterminer si l'arme est une arme caster/healer/druid
                // Elle renvoie la valeur de DPS à appliquer sur l'arme ( double dps_sacrif )
                // Elle renvoie la valeur en +heal, +spell et/ou +feralAttackPower à ajouter sur l'arme. (WeaponSpell , WeaponHeal ou WeaponFeralAttPow).
                // Elle doit etre calculé avant tout scaling sur les stats car cette valeur ne suit pas une rescaling avec CoeffR. 
                // De plus, certaine armes ont trop de Spell ou Heal. Il faut donc dire au programme que ce montant est à séparer en deux parties : 
                //      Une partie correspondant au DPS sacrifice et donc calculé par la procedure qui suit.
                //      Une partie correspondant à un vrai bonus en Spell ou Heal et qui doit être rescalé avec CoeffR.
                // A la fin, les deux valeurs sont à additionner. (Cette gestion est un peu comparable au bonus armor).
                if (it_ori._class == 2) // weapons
                {
                    double dps_sacrif;
                    double Expected_OriWeaponSpell;
                    double Expected_OriWeaponHeal;
                    if (new[] { 1, 5, 8, 10, 17, 20 }.Contains(it_ori.subclass))  //2H weap    //dps_origin  < dps_expect && 
                    {

                        // Doing work on damage sacrifice
                        double dps_onehan;
                        if ((dps_origin + 1 < dps_expect && it.GetSpellValue("SPELLDMG", 0) != 0) || it.pathing_stat[Item.ITEM_MOD_CASTER_WEAP] == 1)
                        {
                            // Reduce weap DPS and Add Spell damage
                            dps_onehan = GetDps(it_ori, it.sItemLevel, true);
                            dps_sacrif = Math.Min(dps_scaled, 0.3 * dps_onehan + 41.5);

                            spells_bonus[(int)SpellType.SPELLDMG] = Math.Max(Math.Round(4 * (dps_scaled - dps_sacrif)), 0);

                            Expected_OriWeaponSpell = Math.Round(4 * (dps_expect - dps_origin));
                            spells_malus[(int)SpellType.SPELLDMG] = Math.Max(Expected_OriWeaponSpell, 0);
                            dps_scaled = dps_sacrif; // si l'objet posséde un dps sacrifice, on réécrit son dps_scaled
                        }

                        if ((dps_origin + 1 < dps_expect && it.GetSpellValue("HEAL", 0) != 0) || it.pathing_stat[Item.ITEM_MOD_HEALER_WEAP] == 1)
                        {
                            // Reduce weap DPS and Add Spell damage
                            dps_onehan = GetDps(it_ori, it.sItemLevel, true);
                            dps_sacrif = Math.Min(dps_scaled, 0.3 * dps_onehan + 41.5);
                            spells_bonus[(int)SpellType.HEAL] = Math.Max(Math.Round(7.5 * (dps_scaled - dps_sacrif)), 0);

                            Expected_OriWeaponHeal = Math.Round(7.5 * (dps_expect - dps_origin));
                            spells_malus[(int)SpellType.HEAL] = Math.Max(Expected_OriWeaponHeal, 0);
                            dps_scaled = dps_sacrif; // si l'objet posséde un dps sacrifice, on réécrit son dps_scaled
                        }

                        if ((dps_origin + 1 < dps_expect && it.GetSpellValue("ATTACKPWR_FERAL", 0) != 0) || it.pathing_stat[Item.ITEM_MOD_DRUID_WEAP] == 1)
                        {
                            // Reduce weap DPS and Add Spell damage
                            dps_onehan = GetDps(it_ori, it.sItemLevel, true);
                            dps_sacrif = Math.Min(dps_scaled, 0.3 * dps_onehan + 41.5);
                            spells_bonus[(int)SpellType.ATTACKPWR_FERAL] = Math.Max(Math.Round(18.37 * (dps_scaled - dps_sacrif) - 12.4843), 0);
                            dps_scaled = dps_sacrif; // si l'objet posséde un dps sacrifice, on réécrit son dps_scaled
                        }
                    }
                    else if (new[] { 0, 4, 7, 13, 15 }.Contains(it_ori.subclass))   //1H weap
                    {
                        // Reduce weap DPS and Add Spell damage
                        if ((dps_origin + 1 < dps_expect && it.GetSpellValue("SPELLDMG", 0) != 0) || it.pathing_stat[Item.ITEM_MOD_CASTER_WEAP] == 1)  //Attention, il faut rajouter une condition sur les sorts présents (spell / heal /feral attackpower)
                        {
                            dps_sacrif = Math.Min(dps_scaled, 41.5);
                            spells_bonus[(int)SpellType.SPELLDMG] = Math.Max(Math.Round(4 * (dps_scaled - dps_sacrif)), 0);

                            Expected_OriWeaponSpell = Math.Round(4 * (dps_expect - dps_origin));

                            spells_malus[(int)SpellType.SPELLDMG] = Math.Max(Expected_OriWeaponSpell, 0);
                            dps_scaled = dps_sacrif; // si l'objet posséde un dps sacrifice, on réécrit son dps_scaled
                        }

                        if ((dps_origin + 1 < dps_expect && it.GetSpellValue("HEAL", 0) != 0) || it.pathing_stat[Item.ITEM_MOD_HEALER_WEAP] == 1)  //Attention, il faut rajouter une condition sur les sorts présents (spell / heal /feral attackpower)
                        {
                            dps_sacrif = Math.Min(dps_scaled, 41.5);
                            spells_bonus[(int)SpellType.HEAL] = Math.Max(Math.Round(7.5 * (dps_scaled - dps_sacrif)), 0);

                            Expected_OriWeaponHeal = Math.Round(7.5 * (dps_expect - dps_origin));
                            spells_malus[(int)SpellType.HEAL] = Math.Max(Expected_OriWeaponHeal, 0);
                            dps_scaled = dps_sacrif; // si l'objet posséde un dps sacrifice, on réécrit son dps_scaled
                        }

                        if ((dps_origin + 1 < dps_expect && it.GetSpellValue("ATTACKPWR_FERAL", 0) != 0) || it.pathing_stat[Item.ITEM_MOD_DRUID_WEAP] == 1)  //Attention, il faut rajouter une condition sur les sorts présents (spell / heal /feral attackpower)
                        {
                            dps_sacrif = Math.Min(dps_scaled, 41.5);
                            spells_bonus[(int)SpellType.ATTACKPWR_FERAL] = Math.Max(Math.Round(18.37 * (dps_scaled - dps_sacrif) - 12.4843), 0);
                            dps_scaled = dps_sacrif; // si l'objet posséde un dps sacrifice, on réécrit son dps_scaled
                        }
                    }
                }
            }

            for (int i = 0; i < 5; i++)
            {
                Damage old_dmg = it.damages_ori[i];
                Damage new_dmg = new Damage(old_dmg.min, old_dmg.max, old_dmg.type);
                double reste = Math.Max(dps_scaled - dps_scaledNoBQ, 0);

                if (dps_expect != 0)
                {
                    new_dmg.min = (int)(new_dmg.min * (dps_scaledNoBQ + 0.3 * reste) / dps_expect);
                    new_dmg.max = (int)(new_dmg.max * (dps_scaledNoBQ + 0.3 * reste) / dps_expect);
                }

                new_dmg.min = double.IsNaN(new_dmg.min) ? 0 : (new_dmg.min);
                new_dmg.max = double.IsNaN(new_dmg.max) ? 0 : (new_dmg.max);
                it.damages[i] = new_dmg;
            }

            // Doing work on Misc Spells
            ScaleSpell(ref it, coeffR);

            // Doing work on Mods rating : NewRating = Rating * CoeffR
            for (int i = Item.ITEM_MOD_MANA; i < Item.ITEM_MOD_MAX; i++)
            {
                int mod_value = it.GetItemModValue(i);
                stats_sign[i] = Math.Sign(mod_value);
                mod_value = mod_value < 0 ? mod_value * -1 : mod_value;

                int s_mod_value = Convert.ToInt32(Math.Floor(mod_value * coeffR));
                it.SetItemModValue(i, s_mod_value);
                stats_redo[i] = mod_value * coeffR;
            }

            // Doing work on Handled Spells
            for (int SPELL_MOD_TYPE = 0; SPELL_MOD_TYPE < Item.SPELL_MOD_TYPES_MAX; SPELL_MOD_TYPE++)
            {
                for (int j = 0; j < Item.spell_types[SPELL_MOD_TYPE].Count; j++)
                {
                    string spell_type = Item.spell_types[SPELL_MOD_TYPE].ElementAt(j).Value.name;
                    int spell_id = Item.spell_types[SPELL_MOD_TYPE].ElementAt(j).Key;

                    double spell_value = Math.Max(it.GetSpellValue(spell_type, SPELL_MOD_TYPE) - spells_malus[spell_id], 0);
                    double s_spell_value = spell_value * coeffR;

                    spells_values[spell_id] = s_spell_value;
                }
            }

            // Doing work on Resistances
            for (int j = 0; j < Item.spell_types[Item.SPELL_MOD_TYPES_RESIST].Count; j++)
            {
                string ResName = Item.spell_types[Item.SPELL_MOD_TYPES_RESIST].ElementAt(j).Value.name;
                int ResValue = it.getItemResValue(ResName);
                it.SetItemResValue(ResName, (int)(ResValue * coeffR));
            }

            if (it.Quality > (int)ItemQualities.ITEM_QUALITY_NORMAL && it.Quality < (int)ItemQualities.ITEM_QUALITY_ARTIFACT && (it._class == 4 || it._class == 2))
            {
                // Doing work on Pathing
                double coeffK = 0;
                for (int i = Item.ITEM_MOD_AGILITY; i <= Item.ITEM_MOD_STAMINA; i++)
                    coeffK += Math.Pow(stats_redo[i] * (it.pathing_stat[i] == -1 ? 1 : 0), alpha);

                double Nadd = 0;
                for (int MOD_TYPE = 0; MOD_TYPE <= Item.SPELL_MOD_MIN; MOD_TYPE++)
                    for (int j = 0; j < Item.spell_types[MOD_TYPE].Count; j++)
                        Nadd += it.pathing_spell[j];

                double NAddMod = 0;

                for (int i = Item.ITEM_MOD_AGILITY; i < Item.ITEM_MOD_LAST; i++)
                    NAddMod += (it.pathing_stat[i] == 1 ? 1 : 0);

                double redoStat = 0.18 / Math.Pow(Nadd + NAddMod, alpha);

                double coeffP = 0;
                double coeffP2 = 0;
                for (int i = Item.ITEM_MOD_AGILITY; i <= Item.ITEM_MOD_STAMINA; i++)
                {
                    coeffP += Math.Pow(stats_redo[i] * (it.pathing_stat[i] == -1 ? (1 - redoStat) : 0), alpha);
                    coeffP2 += Math.Pow(stats_redo[i] * (it.pathing_stat[i] == -1 ? (1 - redoStat) : 1), alpha);
                }

                if (Nadd != 0)
                {
                    double Addbrut = (coeffK - coeffP) / Nadd;

                    // Doing work on Handled Spells
                    for (int SPELL_MOD_TYPE = 0; SPELL_MOD_TYPE < Item.SPELL_MOD_TYPES_MAX; SPELL_MOD_TYPE++)
                    {
                        for (int j = 0; j < Item.spell_types[SPELL_MOD_TYPE].Count; j++)
                        {
                            int indPathing = j + (SPELL_MOD_TYPE + (SPELL_MOD_TYPE == 0 ? 0 : 2)) * 10;     //Dirty fix to get the proper index of pathing_spell since pathing_spell and spell_type doesn't share the same structure.

                            int spell_id = Item.spell_types[SPELL_MOD_TYPE].ElementAt(j).Key;

                            double spell_weigth = Math.Pow(Item.spell_types[SPELL_MOD_TYPE].ElementAt(j).Value.weigth, alpha);

                            double spell_value = spells_values[spell_id];
                            double s_spell_value = Convert.ToInt32(Math.Pow(Math.Pow(spell_value, alpha) + ((it.pathing_spell[indPathing] == 1 ? 1 : 0) * Addbrut / spell_weigth), 1 / alpha));
                            spells_values[spell_id] = s_spell_value;
                            //it.SetSpellValue(spell_type, s_spell_value, SPELL_MOD_TYPE);
                        }
                    }

                    for (int i = Item.ITEM_MOD_AGILITY; i <= Item.ITEM_MOD_STAMINA; i++)
                        stats_redo[i] *= (it.pathing_stat[i] == -1 ? (1 - redoStat) : 1);
                }

                // Doing work on Handled Mods
                int TempLvl = Math.Max(it_ori.ItemLevel, 65);
                double t_coeffR = getCoeffR(it_ori.ItemLevel, TempLvl, it_ori.Quality, it_ori.Quality);
                double t_coeffK = coeffP2 * Math.Pow(t_coeffR / coeffR, alpha);   //P ou P2 ??
                double coeffKreduit = 0;

                for (int i = Item.ITEM_MOD_DEFENSE_SKILL_RATING; i < Item.ITEM_MOD_LAST; i++)
                {
                    if (it.pathing_stat[i] != 0)
                    {
                        double MOD_WEIGHT;
                        switch (i)
                        {
                            case Item.ITEM_MOD_CRIT_MELEE_RATING: MOD_WEIGHT = 14; break;
                            case Item.ITEM_MOD_CRIT_RANGED_RATING: MOD_WEIGHT = 14; break;
                            case Item.ITEM_MOD_CRIT_SPELL_RATING: MOD_WEIGHT = 14; break;
                            case Item.ITEM_MOD_HIT_MELEE_RATING: MOD_WEIGHT = 10; break;
                            case Item.ITEM_MOD_HIT_RANGED_RATING: MOD_WEIGHT = 10; break;
                            case Item.ITEM_MOD_HIT_SPELL_RATING: MOD_WEIGHT = 8; break;
                            case Item.ITEM_MOD_EXPERTISE_RATING: MOD_WEIGHT = 10; break;
                            case Item.ITEM_MOD_PARRY_RATING: MOD_WEIGHT = 15; break;
                            case Item.ITEM_MOD_BLOCK_RATING: MOD_WEIGHT = 5; break;
                            case Item.ITEM_MOD_DODGE_RATING: MOD_WEIGHT = 12; break;
                            case Item.ITEM_MOD_HASTE_MELEE_RATING: MOD_WEIGHT = 10; break;
                            case Item.ITEM_MOD_HASTE_RANGED_RATING: MOD_WEIGHT = 10; break;
                            case Item.ITEM_MOD_HASTE_SPELL_RATING: MOD_WEIGHT = 10; break;
                            default: MOD_WEIGHT = 0; break;
                        }

                        // A REVOIR : la valeur de la stat ajouter est trop élevé (elle devrait être égal au poid au lvl 60)
                        double redoStat2 = Math.Max(Math.Pow((1 - Math.Pow(MOD_WEIGHT, alpha) / t_coeffK), (1 / alpha)), 0.70);

                        if (double.IsNaN(redoStat2))
                            continue;

                        for (int j = Item.ITEM_MOD_AGILITY; j <= Item.ITEM_MOD_STAMINA; j++)
                        {
                            coeffKreduit += Math.Pow(stats_redo[j], alpha);
                            stats_redo[j] *= redoStat2;
                        }

                        int s_mod_value = Convert.ToInt32(Math.Pow(coeffKreduit * (1 - Math.Pow(redoStat2, alpha)), (1 / alpha)));
                        it.SetItemModValue(i, s_mod_value);
                    }
                }

                for (int i = Item.ITEM_MOD_AGILITY; i <= Item.ITEM_MOD_STAMINA; i++)
                {
                    int s_mod_value = Convert.ToInt32(stats_redo[i]);
                    it.SetItemModValue(i, s_mod_value);
                }
            }

            ScaleRandomProperty(ref it, coeffR);
            ScaleSocketBonus(ref it, coeffR);

            // Doing work on Mods : Restore sign if needed and apply mods
            for (int i = Item.ITEM_MOD_MANA; i < Item.ITEM_MOD_LAST; i++)
            {
                int mod_value = it.GetItemModValue(i) * stats_sign[i];

                switch (i)
                {
                    case Item.ITEM_MOD_STAMINA:
                        if (it.ItemLevel > 92 && it.sItemLevel < 92)
                            mod_value = Convert.ToInt32(Math.Floor(mod_value * 0.66)); // vanilla
                        else if (it.ItemLevel < 92 && it.sItemLevel > 92)
                            mod_value = Convert.ToInt32(Math.Floor(mod_value / 0.66)); // tbc
                        break;
                }

                it.SetItemModValue(i, mod_value);
            }

            // Doing work on Sockets
            // below reqlevel 60
            if (socketBonus_list.Count > 0 && (coeffR <= 0.85))
            {
                int socketMetaCount = it.socketColor_1 == 1 ? 1 : 0 + it.socketColor_2 == 1 ? 1 : 0 + it.socketColor_3 == 1 ? 1 : 0;
                int socketRedCount = it.socketColor_1 == 2 ? 1 : 0 + it.socketColor_2 == 2 ? 1 : 0 + it.socketColor_3 == 2 ? 1 : 0;
                int socketYellowCount = it.socketColor_1 == 4 ? 1 : 0 + it.socketColor_2 == 4 ? 1 : 0 + it.socketColor_3 == 4 ? 1 : 0;
                int socketBlueCount = it.socketColor_1 == 8 ? 1 : 0 + it.socketColor_2 == 8 ? 1 : 0 + it.socketColor_3 == 8 ? 1 : 0;

                int has_dodge = it.GetItemModValue(Item.ITEM_MOD_DODGE_RATING);
                int has_parry = it.GetItemModValue(Item.ITEM_MOD_PARRY_RATING);
                double has_heal = spells_values[(int)SpellType.HEAL];
                double has_spell = spells_values[(int)SpellType.SPELLDMG];
                double has_sppen = spells_values[(int)SpellType.SPELLPENETRATION];
                double has_atpw = spells_values[(int)SpellType.ATTACKPWR];
                double has_mp5 = spells_values[(int)SpellType.MP5];
                int has_stragi = (it.GetItemModValue(Item.ITEM_MOD_STRENGTH) > it.GetItemModValue(Item.ITEM_MOD_AGILITY) && it.GetItemModValue(Item.ITEM_MOD_AGILITY) != 0) ? it.GetItemModValue(Item.ITEM_MOD_STRENGTH) : 0;
                int has_agistr = (it.GetItemModValue(Item.ITEM_MOD_AGILITY) > it.GetItemModValue(Item.ITEM_MOD_STRENGTH) && it.GetItemModValue(Item.ITEM_MOD_STRENGTH) != 0) ? it.GetItemModValue(Item.ITEM_MOD_AGILITY) : 0;

                int has_spirit = it.GetItemModValue(Item.ITEM_MOD_SPIRIT);
                int has_stamina = it.GetItemModValue(Item.ITEM_MOD_STAMINA);
                int has_intellect = it.GetItemModValue(Item.ITEM_MOD_INTELLECT);

                int has_resilience = it.GetItemModValue(Item.ITEM_MOD_RESILIENCE_RATING);
                int has_def = it.GetItemModValue(Item.ITEM_MOD_DEFENSE_SKILL_RATING);
                int has_spellcrit = it.GetItemModValue(Item.ITEM_MOD_CRIT_SPELL_RATING);
                int has_spellhit = it.GetItemModValue(Item.ITEM_MOD_HIT_SPELL_RATING);
                int has_spellhaste = it.GetItemModValue(Item.ITEM_MOD_HASTE_SPELL_RATING);

                int has_crit = it.GetItemModValue(Item.ITEM_MOD_CRIT_RATING);
                int has_hit = it.GetItemModValue(Item.ITEM_MOD_HIT_RATING);

                if (socketRedCount > 0) // has red socket
                {
                    if (has_dodge != 0)
                        it.SetItemModValue(Item.ITEM_MOD_DODGE_RATING, Convert.ToInt32(Math.Floor(has_dodge + (8 * socketRedCount * coeffR))));
                    else if (has_parry != 0)
                        it.SetItemModValue(Item.ITEM_MOD_PARRY_RATING, Convert.ToInt32(Math.Floor(has_parry + (8 * socketRedCount * coeffR))));
                    else if (has_heal != 0)
                        spells_values[(int)SpellType.HEAL] = Convert.ToInt32(Math.Floor(has_heal + (18 * socketRedCount * coeffR)));
                    else if (has_spell != 0)
                        spells_values[(int)SpellType.SPELLDMG] = Convert.ToInt32(Math.Floor(has_spell + (9 * socketRedCount * coeffR)));
                    else if (has_stragi != 0)
                        it.SetItemModValue(Item.ITEM_MOD_STRENGTH, Convert.ToInt32(Math.Floor(has_stragi + (8 * socketRedCount * coeffR))));
                    else if (has_agistr != 0)
                        it.SetItemModValue(Item.ITEM_MOD_AGILITY, Convert.ToInt32(Math.Floor(has_agistr + (8 * socketRedCount * coeffR))));
                    else if (has_atpw != 0)
                        spells_values[(int)SpellType.ATTACKPWR] = Convert.ToInt32(Math.Floor(has_atpw + (16 * socketRedCount * coeffR)));
                }

                if (socketBlueCount > 0) // has blue socket
                {
                    if (has_sppen != 0)
                        spells_values[(int)SpellType.SPELLPENETRATION] = Convert.ToInt32(Math.Floor(has_sppen + (10 * socketBlueCount * coeffR)));
                    else if (has_mp5 != 0)
                        spells_values[(int)SpellType.SPELLPENETRATION] = Convert.ToInt32(Math.Floor(has_mp5 + (10 * socketBlueCount * coeffR)));
                    else if (has_spirit != 0)
                        it.SetItemModValue(Item.ITEM_MOD_SPIRIT, Convert.ToInt32(Math.Floor(has_spirit + (8 * socketBlueCount * coeffR))));
                    else if (has_stamina != 0)
                        it.SetItemModValue(Item.ITEM_MOD_STAMINA, Convert.ToInt32(Math.Floor(has_stamina + (12 * socketBlueCount * coeffR))));
                }

                if (socketYellowCount > 0) // has yellow socket
                {
                    if (has_resilience != 0)
                        it.SetItemModValue(Item.ITEM_MOD_RESILIENCE_RATING, Convert.ToInt32(Math.Floor(has_resilience + (8 * socketYellowCount * coeffR))));
                    else if (has_def != 0)
                        it.SetItemModValue(Item.ITEM_MOD_DEFENSE_SKILL_RATING, Convert.ToInt32(Math.Floor(has_def + (8 * socketYellowCount * coeffR))));
                    else if (has_spellcrit != 0)
                        it.SetItemModValue(Item.ITEM_MOD_CRIT_SPELL_RATING, Convert.ToInt32(Math.Floor(has_spellcrit + (8 * socketYellowCount * coeffR))));
                    else if (has_spellhit != 0)
                        it.SetItemModValue(Item.ITEM_MOD_HIT_SPELL_RATING, Convert.ToInt32(Math.Floor(has_spellhit + (8 * socketYellowCount * coeffR))));
                    else if (has_spellhaste != 0)
                        it.SetItemModValue(Item.ITEM_MOD_HASTE_SPELL_RATING, Convert.ToInt32(Math.Floor(has_spellhaste + (8 * socketYellowCount * coeffR))));
                    else if (has_crit != 0)
                        it.SetItemModValue(Item.ITEM_MOD_CRIT_RATING, Convert.ToInt32(Math.Floor(has_crit + (8 * socketYellowCount * coeffR))));
                    else if (has_hit != 0)
                        it.SetItemModValue(Item.ITEM_MOD_HIT_RATING, Convert.ToInt32(Math.Floor(has_hit + (8 * socketYellowCount * coeffR))));
                    else if (has_intellect != 0)
                        it.SetItemModValue(Item.ITEM_MOD_INTELLECT, Convert.ToInt32(Math.Floor(has_intellect + (8 * socketYellowCount * coeffR))));
                }

                if (socketMetaCount > 0) // has meta socket
                    it.SetItemModValue(Item.ITEM_MOD_STAMINA, Convert.ToInt32(Math.Floor(has_stamina + 18 * coeffR)));

                it.socketContent_1 = 0;
                it.socketContent_2 = 0;
                it.socketContent_3 = 0;
                it.socketColor_1 = 0;
                it.socketColor_2 = 0;
                it.socketColor_3 = 0;
                it.socketBonus_new = null;
            }

            // Doing work on Handled Spells
            for (int SPELL_MOD_TYPE = 0; SPELL_MOD_TYPE < Item.SPELL_MOD_TYPES_MAX; SPELL_MOD_TYPE++)
            {
                for (int j = 0; j < Item.spell_types[SPELL_MOD_TYPE].Count; j++)
                {
                    string spell_type = Item.spell_types[SPELL_MOD_TYPE].ElementAt(j).Value.name;
                    int spell_id = Item.spell_types[SPELL_MOD_TYPE].ElementAt(j).Key;

                    double spell_value = spells_values[spell_id];
                    spell_value += spells_bonus[spell_id];

                    it.SetSpellValue(spell_type, Convert.ToInt32(Math.Round(spell_value)), SPELL_MOD_TYPE);
                }
            }
        }

        public static int getScaledId(int entry, int BonusQuality, int level)
        {
            return (MIN_ENTRY_SCALE + entry * MAX_QUALITY_SCALE * MAX_ILEVEL_SCALE + BonusQuality * MAX_ILEVEL_SCALE + level);  //NEVER EVER CHANGE THIS FUNCTION.
        }

        public void generateDBC()
        {
            Console.Write("Processing dbc patch file...");

            UTF8Encoding UTF8NoPreamble = new UTF8Encoding(false);
            using (StreamWriter outputFile = new StreamWriter(new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "out", "patch_table.csv"), FileMode.Append), UTF8NoPreamble))
            {
                ///outputFile.WriteLine("itemID,ItemDisplayInfo,InventorySlotID,sheath");
                outputFile.WriteLine("long,long,long,long");

                MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT i.entry, i.displayid, i.InventoryType, i.sheath FROM item_template i LIMIT 99999999", connection)
                {
                    CommandTimeout = connectionTimeout
                };
                MySqlDataReader dataReader = cmd.ExecuteReader();

                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        int entry = int.Parse(dataReader["entry"].ToString());
                        int displayid = int.Parse(dataReader["displayid"].ToString());
                        int InventoryType = int.Parse(dataReader["InventoryType"].ToString());
                        int sheath = int.Parse(dataReader["sheath"].ToString());

                        outputFile.WriteLine(entry + "," + displayid + "," + InventoryType + "," + sheath);

                        Console.Write(".");
                    }
                }
                Console.WriteLine();
                dataReader.Close();
                connection.Close();
            }
        }

        public void generateJunk()
        {
            Console.Write("Processing junk_list...");

            foreach (Item item in junk_list.Where(a => (filter.Items.Count == 0) || filter.Items.Count != 0 && filter.Items.Contains(a.entry)))
            {
                List<Item> itemlist = new List<Item>();
                for (int ilevel = MIN_ILEVEL_SCALE; ilevel <= MAX_ILEVEL_SCALE; ilevel += 5)
                {
                    Item it = new Item(item);

                    it.name = it.name.Replace("\"", "\\\"");
                    it.description = it.description.Replace("\"", "\\\"");

                    if (it.name_loc2 != null)
                        it.name_loc2 = it.name_loc2.Replace("\"", "\\\"");
                    if (it.description_loc2 != null)
                        it.description_loc2 = it.description_loc2.Replace("\"", "\\\"");

                    it.sItemLevel = ilevel;

                    it.entry = getScaledId(item.entry, 0, ilevel);
                    it.BuyPrice = item.SellPrice != 0 ? GetPrice(item.BuyPrice, item.RequiredLevel, ilevel - 5) : 0;
                    it.SellPrice = it.BuyPrice / 5;

                    // Last step
                    it.Generate();
                    itemlist.Add(it);
                }

                UTF8Encoding UTF8NoPreamble = new UTF8Encoding(false);
                using (StreamWriter outputFile = new StreamWriter(new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "out", "099_junk.sql"), FileMode.Append), UTF8NoPreamble))
                {
                    Item last = itemlist.OrderBy(b => b.entry).Last();
                    outputFile.Write("REPLACE INTO item_template VALUES ");

                    foreach (Item it in itemlist)
                    {
                        outputFile.Write(it.Export());
                        if (it.Equals(last))
                            outputFile.Write(";\n");
                        else
                            outputFile.Write(",\n");
                    }

                    outputFile.Write("REPLACE INTO item_loot_scale VALUES ");
                    foreach (Item it in itemlist)
                    {
                        if (it.Equals(last))
                            outputFile.Write("(" + item.entry + ", " + it.entry + ", " + it.sItemLevel + ");\n");
                        else
                            outputFile.Write("(" + item.entry + ", " + it.entry + ", " + it.sItemLevel + "),");
                    }
                }
                Console.Write(".");
            }
            Console.WriteLine();
        }

        public static int ComputeRequiredLevel(Item it)
        {
            int RequiredLevel = it.sItemLevel - 5;
            int ilevel = it.sItemLevel;

            if (it.Quality <= (int)ItemQualities.ITEM_QUALITY_UNCOMMON) // green
            {
                if (ilevel <= 70)
                    RequiredLevel = ilevel - 5; // vanilla
                else
                    RequiredLevel = Math.Min((ilevel - 90) / 3 + 60, 70); // tbc
            }
            else if (it.Quality == (int)ItemQualities.ITEM_QUALITY_RARE) // blue
            {
                if (ilevel <= 80)
                    RequiredLevel = Math.Min(ilevel - 5, 60); // vanilla
                else
                    RequiredLevel = Math.Min((ilevel - 85) / 3 + 60, 70); // tbc
            }
            else if (it.Quality >= (int)ItemQualities.ITEM_QUALITY_EPIC) //purple/ orange / red
            {
                if (ilevel <= 93)
                    RequiredLevel = Math.Min(ilevel - 5, 60); // vanilla
                else
                    RequiredLevel = Math.Min((int)Math.Floor(10 + 0.6 * ilevel), 70); // tbc
            }

            return RequiredLevel;
        }

        public static void DoWork(object data)
        {
            threadcount++;

            Item item = item_list[(int)data];

            //On converti les sorts qui peuvent l'être en RATING (crit, hit etc.)
            item.ConvertSpellInModRating();

            if (item.patch <= (int)WowPatch.WOW_PATCH_112)  //Pas de Pathing pour les items TBC 
                if (!(item.ItemLevel >= 66 && item.ItemLevel <= 92 && item.Quality > (int)ItemQualities.ITEM_QUALITY_RARE))  //Pas de Pathing pour les items de raid vanila
                    item.GeneratePathing();

            if (item.Quality < (int)ItemQualities.ITEM_QUALITY_LEGENDARY) //Pas de changement de spell pour les items de qualité épic et sup (>=4)
                item.GenerateSpellShortList();

            // Work on bonus armor
            int expect_armor = GetArmor(item, item.ItemLevel);
            item.bonus_armor = Math.Max(item.armor - expect_armor, 0);

            int itBonusQuality = (BonusUpgrade ? BonusUpgradeValue : 0);

            if (item.Quality >= (int)ItemQualities.ITEM_QUALITY_EPIC)
                itBonusQuality = 0; // Do not create Legendary, Artifacts
            else if (item.Quality <= (int)ItemQualities.ITEM_QUALITY_NORMAL)
                itBonusQuality = 0; // Do not upgrade Poor, Common
            else if (item.Quality == (int)ItemQualities.ITEM_QUALITY_RARE)
                itBonusQuality = itBonusQuality == 2 ? 1 : itBonusQuality;

            // Main loops on bonus quality and ilvl
            item.wip_item_list.Clear();
            for (int BonusQuality = 0; BonusQuality <= itBonusQuality; BonusQuality++)
            {
                int itQuality = item.Quality + BonusQuality;
                List<Item> itemlist = new List<Item>();

                for (int pLevel = MIN_ILEVEL_SCALE; pLevel <= MAX_ILEVEL_SCALE; pLevel++)
                {
                    // temporary compute
                    string Name = item.name.Replace("\"", "\\\"");
                    string Description = item.description.Replace("\"", "\\\"");
                    int ItemLevel = item.GetIlvlFromLvl(itQuality, pLevel, BonusQuality);
                    int Entry = getScaledId(item.entry, BonusQuality, pLevel);
                    int BuyPrice = item.BuyPrice != 0 ? GetPrice(item.BuyPrice, item.RequiredLevel, pLevel) : 0;
                    int SellPrice = item.BuyPrice != 0 ? BuyPrice / 5 : 0;
                    int ItemSet = ItemLevel >= item.ItemLevel ? item.itemset : 0;

                    Item it = new Item(item)
                    {
                        entry = Entry,
                        Quality = itQuality,
                        RequiredLevel = pLevel,
                        name = Name,
                        description = Description,
                        sItemLevel = ItemLevel,
                        BuyPrice = BuyPrice,
                        SellPrice = SellPrice,
                        itemset = ItemSet
                    };

                    ScaleItem(ref it, item);
                    it.Generate();

                    itemlist.Add(it);
                }
                item.wip_item_list.Add(BonusQuality, itemlist);
            }

            //Work on writing item in files
            foreach (KeyValuePair<int, List<Item>> entry in item.wip_item_list)
            {
                int BonusUpgrade = entry.Key;
                List<Item> list = entry.Value;

                if (list.Count == 0)
                    continue;

                using (StreamWriter outputFile = new StreamWriter(new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "out", "item_template", item.entry + ".sql"), FileMode.Append), UTF8NoPreamble))
                {
                    Item first = list.OrderBy(b => b.entry).First();
                    Item last = list.OrderBy(b => b.entry).Last();

                    outputFile.Write("-- BonusUpgrade:" + BonusUpgrade + "\n");
                    outputFile.Write("REPLACE INTO item_template VALUES ");

                    foreach (Item it in list)
                    {
                        outputFile.Write(it.Export());
                        if (it.Equals(last))
                            outputFile.Write(";\n");
                        else
                            outputFile.Write(",\n");
                    }

                    if (item.enchantments_ori.Count > 0)
                    {
                        outputFile.Write("REPLACE INTO item_enchantment_template VALUES ");
                        foreach (Item it in list)
                        {
                            foreach (Enchantment ench in it.enchantments_new)
                            {
                                Enchantment last_ench = it.enchantments_new.Last();
                                if (it.Equals(last) && ench.Equals(last_ench))
                                    outputFile.Write("(" + it.entry + "," + ench.id + "," + ench.chance + ");\n");
                                else
                                    outputFile.Write("(" + it.entry + "," + ench.id + "," + ench.chance + "),");
                            }
                        }
                    }
                }
            }

            // memory leak ?
            item.wip_item_list.Clear();
            item.Dispose();

            Console.Write(".");
            threadcount--;
        }

        public static int threadcount = 0;
        public void generateScaleGeneric(List<Item> mylist, string list_name)
        {
            DateTime StartDateTime = DateTime.Now;

            IEnumerable<Item> subMylist = mylist.Where(a => (filter.Items.Count == 0) || (filter.Items.Count != 0 && filter.Items.Contains(a.entry)));
            Console.Write("Processing " + list_name + "_list...");

            foreach (Item item in subMylist)
            {
                while (threadcount >= maxThread)
                    Thread.Sleep(1000);

                Thread newThread = new Thread(DoWork);
                newThread.Start(item.entry);

                if (garbageCollector)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }

            while (threadcount > 0)
                Thread.Sleep(1000);

            Console.WriteLine();
        }

        private int GetEntryFilterValue()
        {
            int.TryParse(textBox1.Text, out int value);
            return value;
        }

        private void filter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                int value = filter.SelectedIndex;
                if (value != -1)
                    filter.Items.RemoveAt(value);
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int value = GetEntryFilterValue();
                if (value != 0 && !filter.Items.Contains(value))
                {
                    filter.Items.Add(value);
                    textBox1.Text = "";
                }
            }
        }

        private void checkUpgrade_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = checkUpgrade.Checked;
        }
    }


    enum ItemClass
    {
        ITEM_CLASS_CONSUMABLE = 0,
        ITEM_CLASS_CONTAINER = 1,
        ITEM_CLASS_WEAPON = 2,
        ITEM_CLASS_GEM = 3,
        ITEM_CLASS_ARMOR = 4,
        ITEM_CLASS_REAGENT = 5,
        ITEM_CLASS_PROJECTILE = 6,
        ITEM_CLASS_TRADE_GOODS = 7,
        ITEM_CLASS_GENERIC = 8,
        ITEM_CLASS_RECIPE = 9,
        ITEM_CLASS_MONEY = 10,
        ITEM_CLASS_QUIVER = 11,
        ITEM_CLASS_QUEST = 12,
        ITEM_CLASS_KEY = 13,
        ITEM_CLASS_PERMANENT = 14,
        ITEM_CLASS_MISC = 15
    };

    enum ItemSubclassConsumable
    {
        ITEM_SUBCLASS_CONSUMABLE = 0,
        ITEM_SUBCLASS_POTION = 1,
        ITEM_SUBCLASS_ELIXIR = 2,
        ITEM_SUBCLASS_FLASK = 3,
        ITEM_SUBCLASS_SCROLL = 4,
        ITEM_SUBCLASS_FOOD = 5,
        ITEM_SUBCLASS_ITEM_ENHANCEMENT = 6,
        ITEM_SUBCLASS_BANDAGE = 7,
        ITEM_SUBCLASS_CONSUMABLE_OTHER = 8
    };

    enum ItemSubclassContainer
    {
        ITEM_SUBCLASS_CONTAINER = 0,
        ITEM_SUBCLASS_SOUL_CONTAINER = 1,
        ITEM_SUBCLASS_HERB_CONTAINER = 2,
        ITEM_SUBCLASS_ENCHANTING_CONTAINER = 3,
        ITEM_SUBCLASS_ENGINEERING_CONTAINER = 4,
        ITEM_SUBCLASS_GEM_CONTAINER = 5,
        ITEM_SUBCLASS_MINING_CONTAINER = 6,
        ITEM_SUBCLASS_LEATHERWORKING_CONTAINER = 7
    };

    enum ItemSubclassWeapon
    {
        ITEM_SUBCLASS_WEAPON_AXE = 0,
        ITEM_SUBCLASS_WEAPON_AXE2 = 1,
        ITEM_SUBCLASS_WEAPON_BOW = 2,
        ITEM_SUBCLASS_WEAPON_GUN = 3,
        ITEM_SUBCLASS_WEAPON_MACE = 4,
        ITEM_SUBCLASS_WEAPON_MACE2 = 5,
        ITEM_SUBCLASS_WEAPON_POLEARM = 6,
        ITEM_SUBCLASS_WEAPON_SWORD = 7,
        ITEM_SUBCLASS_WEAPON_SWORD2 = 8,
        ITEM_SUBCLASS_WEAPON_obsolete = 9,
        ITEM_SUBCLASS_WEAPON_STAFF = 10,
        ITEM_SUBCLASS_WEAPON_EXOTIC = 11,
        ITEM_SUBCLASS_WEAPON_EXOTIC2 = 12,
        ITEM_SUBCLASS_WEAPON_FIST = 13,
        ITEM_SUBCLASS_WEAPON_MISC = 14,
        ITEM_SUBCLASS_WEAPON_DAGGER = 15,
        ITEM_SUBCLASS_WEAPON_THROWN = 16,
        ITEM_SUBCLASS_WEAPON_SPEAR = 17,
        ITEM_SUBCLASS_WEAPON_CROSSBOW = 18,
        ITEM_SUBCLASS_WEAPON_WAND = 19,
        ITEM_SUBCLASS_WEAPON_FISHING_POLE = 20
    };

    enum ItemSubclassGem
    {
        ITEM_SUBCLASS_GEM_RED = 0,
        ITEM_SUBCLASS_GEM_BLUE = 1,
        ITEM_SUBCLASS_GEM_YELLOW = 2,
        ITEM_SUBCLASS_GEM_PURPLE = 3,
        ITEM_SUBCLASS_GEM_GREEN = 4,
        ITEM_SUBCLASS_GEM_ORANGE = 5,
        ITEM_SUBCLASS_GEM_META = 6,
        ITEM_SUBCLASS_GEM_SIMPLE = 7,
        ITEM_SUBCLASS_GEM_PRISMATIC = 8
    };

    enum ItemSubclassArmor
    {
        ITEM_SUBCLASS_ARMOR_MISC = 0,
        ITEM_SUBCLASS_ARMOR_CLOTH = 1,
        ITEM_SUBCLASS_ARMOR_LEATHER = 2,
        ITEM_SUBCLASS_ARMOR_MAIL = 3,
        ITEM_SUBCLASS_ARMOR_PLATE = 4,
        ITEM_SUBCLASS_ARMOR_BUCKLER = 5,
        ITEM_SUBCLASS_ARMOR_SHIELD = 6,
        ITEM_SUBCLASS_ARMOR_LIBRAM = 7,
        ITEM_SUBCLASS_ARMOR_IDOL = 8,
        ITEM_SUBCLASS_ARMOR_TOTEM = 9
    };

    enum ItemSubclassReagent
    {
        ITEM_SUBCLASS_REAGENT = 0
    };

    enum ItemSubclassProjectile
    {
        ITEM_SUBCLASS_WAND = 0,        // ABS
        ITEM_SUBCLASS_BOLT = 1,        // ABS
        ITEM_SUBCLASS_ARROW = 2,
        ITEM_SUBCLASS_BULLET = 3,
        ITEM_SUBCLASS_THROWN = 4         // ABS
    };

    enum ItemSubclassTradeGoods
    {
        ITEM_SUBCLASS_TRADE_GOODS = 0,
        ITEM_SUBCLASS_PARTS = 1,
        ITEM_SUBCLASS_EXPLOSIVES = 2,
        ITEM_SUBCLASS_DEVICES = 3,
        ITEM_SUBCLASS_JEWELCRAFTING = 4,
        ITEM_SUBCLASS_CLOTH = 5,
        ITEM_SUBCLASS_LEATHER = 6,
        ITEM_SUBCLASS_METAL_STONE = 7,
        ITEM_SUBCLASS_MEAT = 8,
        ITEM_SUBCLASS_HERB = 9,
        ITEM_SUBCLASS_ELEMENTAL = 10,
        ITEM_SUBCLASS_TRADE_GOODS_OTHER = 11,
        ITEM_SUBCLASS_ENCHANTING = 12,
        ITEM_SUBCLASS_MATERIAL = 13        // Added in 2.4.2
    };

    enum ItemSubclassGeneric
    {
        ITEM_SUBCLASS_GENERIC = 0
    };

    enum ItemSubclassRecipe
    {
        ITEM_SUBCLASS_BOOK = 0,
        ITEM_SUBCLASS_LEATHERWORKING_PATTERN = 1,
        ITEM_SUBCLASS_TAILORING_PATTERN = 2,
        ITEM_SUBCLASS_ENGINEERING_SCHEMATIC = 3,
        ITEM_SUBCLASS_BLACKSMITHING = 4,
        ITEM_SUBCLASS_COOKING_RECIPE = 5,
        ITEM_SUBCLASS_ALCHEMY_RECIPE = 6,
        ITEM_SUBCLASS_FIRST_AID_MANUAL = 7,
        ITEM_SUBCLASS_ENCHANTING_FORMULA = 8,
        ITEM_SUBCLASS_FISHING_MANUAL = 9,
        ITEM_SUBCLASS_JEWELCRAFTING_RECIPE = 10
    };

    enum ItemSubclassMoney
    {
        ITEM_SUBCLASS_MONEY = 0
    };

    enum ItemSubclassQuiver
    {
        ITEM_SUBCLASS_QUIVER0 = 0,        // ABS
        ITEM_SUBCLASS_QUIVER1 = 1,        // ABS
        ITEM_SUBCLASS_QUIVER = 2,
        ITEM_SUBCLASS_AMMO_POUCH = 3
    };

    enum ItemSubclassQuest
    {
        ITEM_SUBCLASS_QUEST = 0
    };

    enum ItemSubclassKey
    {
        ITEM_SUBCLASS_KEY = 0,
        ITEM_SUBCLASS_LOCKPICK = 1
    };

    enum ItemSubclassPermanent
    {
        ITEM_SUBCLASS_PERMANENT = 0
    };

    enum ItemSubclassJunk
    {
        ITEM_SUBCLASS_JUNK = 0,
        ITEM_SUBCLASS_JUNK_REAGENT = 1,
        ITEM_SUBCLASS_JUNK_PET = 2,
        ITEM_SUBCLASS_JUNK_HOLIDAY = 3,
        ITEM_SUBCLASS_JUNK_OTHER = 4,
        ITEM_SUBCLASS_JUNK_MOUNT = 5
    };

    enum ItemSubclassGlyph
    {
        ITEM_SUBCLASS_GLYPH_WARRIOR = 1,
        ITEM_SUBCLASS_GLYPH_PALADIN = 2,
        ITEM_SUBCLASS_GLYPH_HUNTER = 3,
        ITEM_SUBCLASS_GLYPH_ROGUE = 4,
        ITEM_SUBCLASS_GLYPH_PRIEST = 5,
        ITEM_SUBCLASS_GLYPH_DEATH_KNIGHT = 6,
        ITEM_SUBCLASS_GLYPH_SHAMAN = 7,
        ITEM_SUBCLASS_GLYPH_MAGE = 8,
        ITEM_SUBCLASS_GLYPH_WARLOCK = 9,
        ITEM_SUBCLASS_GLYPH_DRUID = 11
    };

    public enum SpellType
    {
        HEAL = 0,
        ATTACKPWR = 1,
        ATTACKPWR_RANGED = 2,
        SPELLDMG = 3,
        MP5 = 4,
        HP5 = 5,
        DEFENSE = 6,
        BLOCK = 7,
        PARRY = 8,
        DODGE = 9,
        CRIT = 10,
        SPELLCRIT = 11,
        HIT = 12,
        SPELLHIT = 13,
        ARMOR = 14,
        SPELLPENETRATION = 15,
        WEAPONDAMAGE = 16,
        EXPERTISE = 17,
        ARMORPENETRATION = 18,
        ATTACKPWR_FERAL = 19,
        HASTE = 20
    }

    public enum WowPatch
    {
        WOW_PATCH_102 = 0,
        WOW_PATCH_103 = 1,
        WOW_PATCH_104 = 2,
        WOW_PATCH_105 = 3,
        WOW_PATCH_106 = 4,
        WOW_PATCH_107 = 5,
        WOW_PATCH_108 = 6,
        WOW_PATCH_109 = 7,
        WOW_PATCH_110 = 8,
        WOW_PATCH_111 = 9,
        WOW_PATCH_112 = 10,
        WOW_PATCH_203 = 11,
        WOW_PATCH_210 = 12,
        WOW_PATCH_220 = 13,
        WOW_PATCH_230 = 14,
        WOW_PATCH_240 = 15
    }

    public enum ItemQualities
    {
        ITEM_QUALITY_POOR = 0,                 // GREY
        ITEM_QUALITY_NORMAL = 1,                 // WHITE
        ITEM_QUALITY_UNCOMMON = 2,                 // GREEN
        ITEM_QUALITY_RARE = 3,                 // BLUE
        ITEM_QUALITY_EPIC = 4,                 // PURPLE
        ITEM_QUALITY_LEGENDARY = 5,                 // ORANGE
        ITEM_QUALITY_ARTIFACT = 6                  // LIGHT YELLOW
    };

    public enum Flagsenum
    {
        Soulbound = 1,
        Conjured = 2,
        Lootable = 4,
        Wrapped = 8,
        Totem = 32,
        Activatable = 64,
        Wrapper = 256,
        Gifts = 1024,
        Item = 2048,
        Charter = 8192,
        PvP = 32768,
        Unique = 524288,
        Throwable = 4194304,
        Special = 8388608
    }

    public enum SchoolMask
    {
        Physical = 1,
        Holy = 2,
        Fire = 4,
        Nature = 8,
        Frost = 16,
        Shadow = 32,
        Arcane = 64
    }

    enum AuraType
    {
        SPELL_AURA_NONE = 0,
        SPELL_AURA_BIND_SIGHT = 1,
        SPELL_AURA_MOD_POSSESS = 2,
        /**
         * The aura should do periodic damage, the function that handles
         * this is Aura::HandlePeriodicDamage, the amount is usually decided
         * by the Unit::SpellDamageBonusDone or Unit::MeleeDamageBonusDone
         * which increases/decreases the Modifier::m_amount
         */
        SPELL_AURA_PERIODIC_DAMAGE = 3,
        /**
         * Used by Aura::HandleAuraDummy
         */
        SPELL_AURA_DUMMY = 4,
        /**
         * Used by Aura::HandleModConfuse, will either confuse or unconfuse
         * the target depending on whether the apply flag is set
         */
        SPELL_AURA_MOD_CONFUSE = 5,
        SPELL_AURA_MOD_CHARM = 6,
        SPELL_AURA_MOD_FEAR = 7,
        /**
         * The aura will do periodic heals of a target, handled by
         * Aura::HandlePeriodicHeal, uses Unit::SpellHealingBonusDone
         * to calculate whether to increase or decrease Modifier::m_amount
         */
        SPELL_AURA_PERIODIC_HEAL = 8,
        /**
         * Changes the attackspeed, the Modifier::m_amount decides
         * how much we change in percent, ie, if the m_amount is
         * 50 the attackspeed will increase by 50%
         */
        SPELL_AURA_MOD_ATTACKSPEED = 9,
        /**
         * Modifies the threat that the Aura does in percent,
         * the Modifier::m_miscvalue decides which of the SpellSchools
         * it should affect threat for.
         * \see SpellSchoolMask
         */
        SPELL_AURA_MOD_THREAT = 10,
        /**
         * Just applies a taunt which will change the threat a mob has
         * Taken care of in Aura::HandleModThreat
         */
        SPELL_AURA_MOD_TAUNT = 11,
        /**
         * Stuns targets in different ways, taken care of in
         * Aura::HandleAuraModStun
         */
        SPELL_AURA_MOD_STUN = 12,
        /**
         * Changes the damage done by a weapon in any hand, the Modifier::m_miscvalue
         * will tell what school the damage is from, it's used as a bitmask
         * \see SpellSchoolMask
         */
        SPELL_AURA_MOD_DAMAGE_DONE = 13,
        /**
         * Not handled by the Aura class but instead this is implemented in
         * Unit::MeleeDamageBonusTaken and Unit::SpellBaseDamageBonusTaken
         */
        SPELL_AURA_MOD_DAMAGE_TAKEN = 14,
        /**
         * Not handled by the Aura class, implemented in Unit::DealMeleeDamage
         */
        SPELL_AURA_DAMAGE_SHIELD = 15,
        /**
         * Taken care of in Aura::HandleModStealth, take note that this
         * is not the same thing as invisibility
         */
        SPELL_AURA_MOD_STEALTH = 16,
        /**
         * Not handled by the Aura class, implemented in Unit::isVisibleForOrDetect
         * which does a lot of checks to determine whether the person is visible or not,
         * the SPELL_AURA_MOD_STEALTH seems to determine how in/visible ie a rogue is.
         */
        SPELL_AURA_MOD_STEALTH_DETECT = 17,
        /**
         * Handled by Aura::HandleInvisibility, the Modifier::m_miscvalue in the struct
         * seems to decide what kind of invisibility it is with a bitflag. the miscvalue
         * decides which bit is set, ie: 3 would make the 3rd bit be set.
         */
        SPELL_AURA_MOD_INVISIBILITY = 18,
        /**
         * Adds one of the kinds of detections to the possible detections.
         * As in SPEALL_AURA_MOD_INVISIBILITY the Modifier::m_miscvalue seems to decide
         * what kind of invisibility the Unit should be able to detect.
         */
        SPELL_AURA_MOD_INVISIBILITY_DETECTION = 19,
        SPELL_AURA_OBS_MOD_HEALTH = 20,                         // 20,21 unofficial
        SPELL_AURA_OBS_MOD_MANA = 21,
        /**
         * Handled by Aura::HandleAuraModResistance, changes the resistance for a unit
         * the field Modifier::m_miscvalue decides which kind of resistance that should
         * be changed, for possible values see SpellSchools.
         * \see SpellSchools
         */
        SPELL_AURA_MOD_RESISTANCE = 22,
        /**
         * Currently just sets Aura::m_isPeriodic to apply and has a special case
         * for Curse of the Plaguebringer.
         */
        SPELL_AURA_PERIODIC_TRIGGER_SPELL = 23,
        /**
         * Just sets Aura::m_isPeriodic to apply
         */
        SPELL_AURA_PERIODIC_ENERGIZE = 24,
        /**
         * Changes whether the target is pacified or not depending on the apply flag.
         * Pacify makes the target silenced and have all it's attack skill disabled.
         * See: http://www.wowhead.com/spell=6462/pacified
         */
        SPELL_AURA_MOD_PACIFY = 25,
        /**
         * Roots or unroots the target
         */
        SPELL_AURA_MOD_ROOT = 26,
        /**
         * Silences the target and stops and spell casts that should be stopped,
         * they have the flag SpellPreventionType::SPELL_PREVENTION_TYPE_SILENCE
         */
        SPELL_AURA_MOD_SILENCE = 27,
        SPELL_AURA_REFLECT_SPELLS = 28,
        SPELL_AURA_MOD_STAT = 29,
        SPELL_AURA_MOD_SKILL = 30,
        SPELL_AURA_MOD_INCREASE_SPEED = 31,
        SPELL_AURA_MOD_INCREASE_MOUNTED_SPEED = 32,
        SPELL_AURA_MOD_DECREASE_SPEED = 33,
        SPELL_AURA_MOD_INCREASE_HEALTH = 34,
        SPELL_AURA_MOD_INCREASE_ENERGY = 35,
        SPELL_AURA_MOD_SHAPESHIFT = 36,
        SPELL_AURA_EFFECT_IMMUNITY = 37,
        SPELL_AURA_STATE_IMMUNITY = 38,
        SPELL_AURA_SCHOOL_IMMUNITY = 39,
        SPELL_AURA_DAMAGE_IMMUNITY = 40,
        SPELL_AURA_DISPEL_IMMUNITY = 41,
        SPELL_AURA_PROC_TRIGGER_SPELL = 42,
        SPELL_AURA_PROC_TRIGGER_DAMAGE = 43,
        SPELL_AURA_TRACK_CREATURES = 44,
        SPELL_AURA_TRACK_RESOURCES = 45,
        SPELL_AURA_46 = 46,                                     // Ignore all Gear test spells
        SPELL_AURA_MOD_PARRY_PERCENT = 47,
        SPELL_AURA_48 = 48,                                     // One periodic spell
        SPELL_AURA_MOD_DODGE_PERCENT = 49,
        SPELL_AURA_MOD_BLOCK_SKILL = 50,
        SPELL_AURA_MOD_BLOCK_PERCENT = 51,
        SPELL_AURA_MOD_CRIT_PERCENT = 52,
        SPELL_AURA_PERIODIC_LEECH = 53,
        SPELL_AURA_MOD_HIT_CHANCE = 54,
        SPELL_AURA_MOD_SPELL_HIT_CHANCE = 55,
        SPELL_AURA_TRANSFORM = 56,
        SPELL_AURA_MOD_SPELL_CRIT_CHANCE = 57,
        SPELL_AURA_MOD_INCREASE_SWIM_SPEED = 58,
        SPELL_AURA_MOD_DAMAGE_DONE_CREATURE = 59,
        SPELL_AURA_MOD_PACIFY_SILENCE = 60,
        SPELL_AURA_MOD_SCALE = 61,
        SPELL_AURA_PERIODIC_HEALTH_FUNNEL = 62,
        SPELL_AURA_PERIODIC_MANA_FUNNEL = 63,
        SPELL_AURA_PERIODIC_MANA_LEECH = 64,
        SPELL_AURA_MOD_CASTING_SPEED_NOT_STACK = 65,
        SPELL_AURA_FEIGN_DEATH = 66,
        SPELL_AURA_MOD_DISARM = 67,
        SPELL_AURA_MOD_STALKED = 68,
        SPELL_AURA_SCHOOL_ABSORB = 69,
        SPELL_AURA_EXTRA_ATTACKS = 70,
        SPELL_AURA_MOD_SPELL_CRIT_CHANCE_SCHOOL = 71,
        SPELL_AURA_MOD_POWER_COST_SCHOOL_PCT = 72,
        SPELL_AURA_MOD_POWER_COST_SCHOOL = 73,
        SPELL_AURA_REFLECT_SPELLS_SCHOOL = 74,
        SPELL_AURA_MOD_LANGUAGE = 75,
        SPELL_AURA_FAR_SIGHT = 76,
        SPELL_AURA_MECHANIC_IMMUNITY = 77,
        SPELL_AURA_MOUNTED = 78,
        SPELL_AURA_MOD_DAMAGE_PERCENT_DONE = 79,
        SPELL_AURA_MOD_PERCENT_STAT = 80,
        SPELL_AURA_SPLIT_DAMAGE_PCT = 81,
        SPELL_AURA_WATER_BREATHING = 82,
        SPELL_AURA_MOD_BASE_RESISTANCE = 83,
        SPELL_AURA_MOD_REGEN = 84,
        SPELL_AURA_MOD_POWER_REGEN = 85,
        SPELL_AURA_CHANNEL_DEATH_ITEM = 86,
        SPELL_AURA_MOD_DAMAGE_PERCENT_TAKEN = 87,
        SPELL_AURA_MOD_HEALTH_REGEN_PERCENT = 88,
        SPELL_AURA_PERIODIC_DAMAGE_PERCENT = 89,
        SPELL_AURA_MOD_RESIST_CHANCE = 90,
        SPELL_AURA_MOD_DETECT_RANGE = 91,
        SPELL_AURA_PREVENTS_FLEEING = 92,
        SPELL_AURA_MOD_UNATTACKABLE = 93,
        SPELL_AURA_INTERRUPT_REGEN = 94,
        SPELL_AURA_GHOST = 95,
        SPELL_AURA_SPELL_MAGNET = 96,
        SPELL_AURA_MANA_SHIELD = 97,
        SPELL_AURA_MOD_SKILL_TALENT = 98,
        SPELL_AURA_MOD_ATTACK_POWER = 99,
        SPELL_AURA_AURAS_VISIBLE = 100,
        SPELL_AURA_MOD_RESISTANCE_PCT = 101,
        SPELL_AURA_MOD_MELEE_ATTACK_POWER_VERSUS = 102,
        SPELL_AURA_MOD_TOTAL_THREAT = 103,
        SPELL_AURA_WATER_WALK = 104,
        SPELL_AURA_FEATHER_FALL = 105,
        SPELL_AURA_HOVER = 106,
        SPELL_AURA_ADD_FLAT_MODIFIER = 107,
        SPELL_AURA_ADD_PCT_MODIFIER = 108,
        SPELL_AURA_ADD_TARGET_TRIGGER = 109,
        SPELL_AURA_MOD_POWER_REGEN_PERCENT = 110,
        SPELL_AURA_ADD_CASTER_HIT_TRIGGER = 111,
        SPELL_AURA_OVERRIDE_CLASS_SCRIPTS = 112,
        SPELL_AURA_MOD_RANGED_DAMAGE_TAKEN = 113,
        SPELL_AURA_MOD_RANGED_DAMAGE_TAKEN_PCT = 114,
        SPELL_AURA_MOD_HEALING = 115,
        SPELL_AURA_MOD_REGEN_DURING_COMBAT = 116,
        SPELL_AURA_MOD_MECHANIC_RESISTANCE = 117,
        SPELL_AURA_MOD_HEALING_PCT = 118,
        SPELL_AURA_SHARE_PET_TRACKING = 119,
        SPELL_AURA_UNTRACKABLE = 120,
        SPELL_AURA_EMPATHY = 121,
        SPELL_AURA_MOD_OFFHAND_DAMAGE_PCT = 122,
        SPELL_AURA_MOD_TARGET_RESISTANCE = 123,
        SPELL_AURA_MOD_RANGED_ATTACK_POWER = 124,
        SPELL_AURA_MOD_MELEE_DAMAGE_TAKEN = 125,
        SPELL_AURA_MOD_MELEE_DAMAGE_TAKEN_PCT = 126,
        SPELL_AURA_RANGED_ATTACK_POWER_ATTACKER_BONUS = 127,
        SPELL_AURA_MOD_POSSESS_PET = 128,
        SPELL_AURA_MOD_SPEED_ALWAYS = 129,
        SPELL_AURA_MOD_MOUNTED_SPEED_ALWAYS = 130,
        SPELL_AURA_MOD_RANGED_ATTACK_POWER_VERSUS = 131,
        SPELL_AURA_MOD_INCREASE_ENERGY_PERCENT = 132,
        SPELL_AURA_MOD_INCREASE_HEALTH_PERCENT = 133,
        SPELL_AURA_MOD_MANA_REGEN_INTERRUPT = 134,
        SPELL_AURA_MOD_HEALING_DONE = 135,
        SPELL_AURA_MOD_HEALING_DONE_PERCENT = 136,
        SPELL_AURA_MOD_TOTAL_STAT_PERCENTAGE = 137,
        SPELL_AURA_MOD_MELEE_HASTE = 138,
        SPELL_AURA_FORCE_REACTION = 139,
        SPELL_AURA_MOD_RANGED_HASTE = 140,
        SPELL_AURA_MOD_RANGED_AMMO_HASTE = 141,
        SPELL_AURA_MOD_BASE_RESISTANCE_PCT = 142,
        SPELL_AURA_MOD_RESISTANCE_EXCLUSIVE = 143,
        SPELL_AURA_SAFE_FALL = 144,
        SPELL_AURA_CHARISMA = 145,
        SPELL_AURA_PERSUADED = 146,
        SPELL_AURA_MECHANIC_IMMUNITY_MASK = 147,
        SPELL_AURA_RETAIN_COMBO_POINTS = 148,
        SPELL_AURA_RESIST_PUSHBACK = 149,                      //    Resist Pushback
        SPELL_AURA_MOD_SHIELD_BLOCKVALUE_PCT = 150,
        SPELL_AURA_TRACK_STEALTHED = 151,                      //    Track Stealthed
        SPELL_AURA_MOD_DETECTED_RANGE = 152,                    //    Mod Detected Range
        SPELL_AURA_SPLIT_DAMAGE_FLAT = 153,                     //    Split Damage Flat
        SPELL_AURA_MOD_STEALTH_LEVEL = 154,                     //    Stealth Level Modifier
        SPELL_AURA_MOD_WATER_BREATHING = 155,                   //    Mod Water Breathing
        SPELL_AURA_MOD_REPUTATION_GAIN = 156,                   //    Mod Reputation Gain
        SPELL_AURA_PET_DAMAGE_MULTI = 157,                      //    Mod Pet Damage
        SPELL_AURA_MOD_SHIELD_BLOCKVALUE = 158,
        SPELL_AURA_NO_PVP_CREDIT = 159,
        SPELL_AURA_MOD_AOE_AVOIDANCE = 160,
        SPELL_AURA_MOD_HEALTH_REGEN_IN_COMBAT = 161,
        SPELL_AURA_POWER_BURN_MANA = 162,
        SPELL_AURA_MOD_CRIT_DAMAGE_BONUS = 163,
        SPELL_AURA_164 = 164,
        SPELL_AURA_MELEE_ATTACK_POWER_ATTACKER_BONUS = 165,
        SPELL_AURA_MOD_ATTACK_POWER_PCT = 166,
        SPELL_AURA_MOD_RANGED_ATTACK_POWER_PCT = 167,
        SPELL_AURA_MOD_DAMAGE_DONE_VERSUS = 168,
        SPELL_AURA_MOD_CRIT_PERCENT_VERSUS = 169,
        SPELL_AURA_DETECT_AMORE = 170,
        SPELL_AURA_MOD_SPEED_NOT_STACK = 171,
        SPELL_AURA_MOD_MOUNTED_SPEED_NOT_STACK = 172,
        SPELL_AURA_ALLOW_CHAMPION_SPELLS = 173,
        SPELL_AURA_MOD_SPELL_DAMAGE_OF_STAT_PERCENT = 174,      // by defeult intelect, dependent from SPELL_AURA_MOD_SPELL_HEALING_OF_STAT_PERCENT
        SPELL_AURA_MOD_SPELL_HEALING_OF_STAT_PERCENT = 175,
        SPELL_AURA_SPIRIT_OF_REDEMPTION = 176,
        SPELL_AURA_AOE_CHARM = 177,
        SPELL_AURA_MOD_DEBUFF_RESISTANCE = 178,
        SPELL_AURA_MOD_ATTACKER_SPELL_CRIT_CHANCE = 179,
        SPELL_AURA_MOD_FLAT_SPELL_DAMAGE_VERSUS = 180,
        SPELL_AURA_MOD_FLAT_SPELL_CRIT_DAMAGE_VERSUS = 181,     // unused - possible flat spell crit damage versus
        SPELL_AURA_MOD_RESISTANCE_OF_STAT_PERCENT = 182,
        SPELL_AURA_MOD_CRITICAL_THREAT = 183,
        SPELL_AURA_MOD_ATTACKER_MELEE_HIT_CHANCE = 184,
        SPELL_AURA_MOD_ATTACKER_RANGED_HIT_CHANCE = 185,
        SPELL_AURA_MOD_ATTACKER_SPELL_HIT_CHANCE = 186,
        SPELL_AURA_MOD_ATTACKER_MELEE_CRIT_CHANCE = 187,
        SPELL_AURA_MOD_ATTACKER_RANGED_CRIT_CHANCE = 188,
        SPELL_AURA_MOD_RATING = 189,
        SPELL_AURA_MOD_FACTION_REPUTATION_GAIN = 190,
        SPELL_AURA_USE_NORMAL_MOVEMENT_SPEED = 191,
        SPELL_AURA_MOD_MELEE_RANGED_HASTE = 192,
        SPELL_AURA_HASTE_ALL = 193,
        SPELL_AURA_MOD_DEPRICATED_1 = 194,                     // not used now, old SPELL_AURA_MOD_SPELL_DAMAGE_OF_INTELLECT
        SPELL_AURA_MOD_DEPRICATED_2 = 195,                     // not used now, old SPELL_AURA_MOD_SPELL_HEALING_OF_INTELLECT
        SPELL_AURA_MOD_COOLDOWN = 196,                          // only 24818 Noxious Breath
        SPELL_AURA_MOD_ATTACKER_SPELL_AND_WEAPON_CRIT_CHANCE = 197,
        SPELL_AURA_MOD_ALL_WEAPON_SKILLS = 198,
        SPELL_AURA_MOD_INCREASES_SPELL_PCT_TO_HIT = 199,
        SPELL_AURA_MOD_XP_PCT = 200,
        SPELL_AURA_FLY = 201,
        SPELL_AURA_IGNORE_COMBAT_RESULT = 202,
        SPELL_AURA_MOD_ATTACKER_MELEE_CRIT_DAMAGE = 203,
        SPELL_AURA_MOD_ATTACKER_RANGED_CRIT_DAMAGE = 204,
        SPELL_AURA_MOD_ATTACKER_SPELL_CRIT_DAMAGE = 205,
        SPELL_AURA_MOD_FLIGHT_SPEED = 206,
        SPELL_AURA_MOD_FLIGHT_SPEED_MOUNTED = 207,
        SPELL_AURA_MOD_FLIGHT_SPEED_STACKING = 208,
        SPELL_AURA_MOD_FLIGHT_SPEED_MOUNTED_STACKING = 209,
        SPELL_AURA_MOD_FLIGHT_SPEED_NOT_STACKING = 210,
        SPELL_AURA_MOD_FLIGHT_SPEED_MOUNTED_NOT_STACKING = 211,
        SPELL_AURA_MOD_RANGED_ATTACK_POWER_OF_STAT_PERCENT = 212,
        SPELL_AURA_MOD_RAGE_FROM_DAMAGE_DEALT = 213,
        SPELL_AURA_214 = 214,
        SPELL_AURA_ARENA_PREPARATION = 215,
        SPELL_AURA_HASTE_SPELLS = 216,
        SPELL_AURA_217 = 217,
        SPELL_AURA_HASTE_RANGED = 218,
        SPELL_AURA_MOD_MANA_REGEN_FROM_STAT = 219,
        SPELL_AURA_MOD_RATING_FROM_STAT = 220,
        SPELL_AURA_DETAUNT = 221,
        SPELL_AURA_222 = 222,
        SPELL_AURA_223 = 223,
        SPELL_AURA_224 = 224,
        SPELL_AURA_PRAYER_OF_MENDING = 225,
        SPELL_AURA_PERIODIC_DUMMY = 226,
        SPELL_AURA_PERIODIC_TRIGGER_SPELL_WITH_VALUE = 227,
        SPELL_AURA_DETECT_STEALTH = 228,
        SPELL_AURA_MOD_AOE_DAMAGE_AVOIDANCE = 229,
        SPELL_AURA_230 = 230,
        SPELL_AURA_PROC_TRIGGER_SPELL_WITH_VALUE = 231,
        SPELL_AURA_MECHANIC_DURATION_MOD = 232,
        SPELL_AURA_233 = 233,
        SPELL_AURA_MECHANIC_DURATION_MOD_NOT_STACK = 234,
        SPELL_AURA_MOD_DISPEL_RESIST = 235,
        SPELL_AURA_236 = 236,
        SPELL_AURA_MOD_SPELL_DAMAGE_OF_ATTACK_POWER = 237,
        SPELL_AURA_MOD_SPELL_HEALING_OF_ATTACK_POWER = 238,
        SPELL_AURA_MOD_SCALE_2 = 239,
        SPELL_AURA_MOD_EXPERTISE = 240,
        SPELL_AURA_FORCE_MOVE_FORWARD = 241,
        SPELL_AURA_242 = 242,
        SPELL_AURA_FACTION_OVERRIDE = 243,
        SPELL_AURA_COMPREHEND_LANGUAGE = 244,
        SPELL_AURA_245 = 245,
        SPELL_AURA_246 = 246,
        SPELL_AURA_MIRROR_IMAGE = 247,
        SPELL_AURA_MOD_COMBAT_RESULT_CHANCE = 248,
        SPELL_AURA_249 = 249,
        SPELL_AURA_MOD_INCREASE_HEALTH_2 = 250,
        SPELL_AURA_MOD_ENEMY_DODGE = 251,
        SPELL_AURA_252 = 252,
        SPELL_AURA_253 = 253,
        SPELL_AURA_254 = 254,
        SPELL_AURA_255 = 255,
        SPELL_AURA_256 = 256,
        SPELL_AURA_257 = 257,
        SPELL_AURA_258 = 258,
        SPELL_AURA_259 = 259,
        SPELL_AURA_260 = 260,
        SPELL_AURA_PHASE = 261, // TODO: Implement for GameObjects, not needed for creatures in TBC
        TOTAL_AURAS = 262
    };

    enum SpellEffects
    {
        SPELL_EFFECT_NONE = 0,
        SPELL_EFFECT_INSTAKILL = 1,
        SPELL_EFFECT_SCHOOL_DAMAGE = 2,
        SPELL_EFFECT_DUMMY = 3,
        SPELL_EFFECT_PORTAL_TELEPORT = 4,
        SPELL_EFFECT_TELEPORT_UNITS = 5,
        SPELL_EFFECT_APPLY_AURA = 6,
        SPELL_EFFECT_ENVIRONMENTAL_DAMAGE = 7,
        SPELL_EFFECT_POWER_DRAIN = 8,
        SPELL_EFFECT_HEALTH_LEECH = 9,
        SPELL_EFFECT_HEAL = 10,
        SPELL_EFFECT_BIND = 11,
        SPELL_EFFECT_PORTAL = 12,
        SPELL_EFFECT_RITUAL_BASE = 13,
        SPELL_EFFECT_RITUAL_SPECIALIZE = 14,
        SPELL_EFFECT_RITUAL_ACTIVATE_PORTAL = 15,
        SPELL_EFFECT_QUEST_COMPLETE = 16,
        SPELL_EFFECT_WEAPON_DAMAGE_NOSCHOOL = 17,
        SPELL_EFFECT_RESURRECT = 18,
        SPELL_EFFECT_ADD_EXTRA_ATTACKS = 19,
        SPELL_EFFECT_DODGE = 20,
        SPELL_EFFECT_EVADE = 21,
        SPELL_EFFECT_PARRY = 22,
        SPELL_EFFECT_BLOCK = 23,
        SPELL_EFFECT_CREATE_ITEM = 24,
        SPELL_EFFECT_WEAPON = 25,
        SPELL_EFFECT_DEFENSE = 26,
        SPELL_EFFECT_PERSISTENT_AREA_AURA = 27,
        SPELL_EFFECT_SUMMON = 28,
        SPELL_EFFECT_LEAP = 29,
        SPELL_EFFECT_ENERGIZE = 30,
        SPELL_EFFECT_WEAPON_PERCENT_DAMAGE = 31,
        SPELL_EFFECT_TRIGGER_MISSILE = 32,
        SPELL_EFFECT_OPEN_LOCK = 33,
        SPELL_EFFECT_SUMMON_CHANGE_ITEM = 34,
        SPELL_EFFECT_APPLY_AREA_AURA_PARTY = 35,
        SPELL_EFFECT_LEARN_SPELL = 36,
        SPELL_EFFECT_SPELL_DEFENSE = 37,
        SPELL_EFFECT_DISPEL = 38,
        SPELL_EFFECT_LANGUAGE = 39,
        SPELL_EFFECT_DUAL_WIELD = 40,
        SPELL_EFFECT_41 = 41,            // old SPELL_EFFECT_SUMMON_WILD
        SPELL_EFFECT_42 = 42,            // old SPELL_EFFECT_SUMMON_GUARDIAN
        SPELL_EFFECT_TELEPORT_UNITS_FACE_CASTER = 43,
        SPELL_EFFECT_SKILL_STEP = 44,
        SPELL_EFFECT_UNDEFINED_45 = 45,
        SPELL_EFFECT_SPAWN = 46,
        SPELL_EFFECT_TRADE_SKILL = 47,
        SPELL_EFFECT_STEALTH = 48,
        SPELL_EFFECT_DETECT = 49,
        SPELL_EFFECT_TRANS_DOOR = 50,
        SPELL_EFFECT_FORCE_CRITICAL_HIT = 51,
        SPELL_EFFECT_GUARANTEE_HIT = 52,
        SPELL_EFFECT_ENCHANT_ITEM = 53,
        SPELL_EFFECT_ENCHANT_ITEM_TEMPORARY = 54,
        SPELL_EFFECT_TAMECREATURE = 55,
        SPELL_EFFECT_SUMMON_PET = 56,
        SPELL_EFFECT_LEARN_PET_SPELL = 57,
        SPELL_EFFECT_WEAPON_DAMAGE = 58,
        SPELL_EFFECT_OPEN_LOCK_ITEM = 59,
        SPELL_EFFECT_PROFICIENCY = 60,
        SPELL_EFFECT_SEND_EVENT = 61,
        SPELL_EFFECT_POWER_BURN = 62,
        SPELL_EFFECT_THREAT = 63,
        SPELL_EFFECT_TRIGGER_SPELL = 64,
        SPELL_EFFECT_HEALTH_FUNNEL = 65,
        SPELL_EFFECT_POWER_FUNNEL = 66,
        SPELL_EFFECT_HEAL_MAX_HEALTH = 67,
        SPELL_EFFECT_INTERRUPT_CAST = 68,
        SPELL_EFFECT_DISTRACT = 69,
        SPELL_EFFECT_PULL = 70,
        SPELL_EFFECT_PICKPOCKET = 71,
        SPELL_EFFECT_ADD_FARSIGHT = 72,
        SPELL_EFFECT_73 = 73,            // old SPELL_EFFECT_SUMMON_POSSESSED
        SPELL_EFFECT_74 = 74,            // old SPELL_EFFECT_SUMMON_TOTEM
        SPELL_EFFECT_HEAL_MECHANICAL = 75,
        SPELL_EFFECT_SUMMON_OBJECT_WILD = 76,
        SPELL_EFFECT_SCRIPT_EFFECT = 77,
        SPELL_EFFECT_ATTACK = 78,
        SPELL_EFFECT_SANCTUARY = 79,
        SPELL_EFFECT_ADD_COMBO_POINTS = 80,
        SPELL_EFFECT_CREATE_HOUSE = 81,
        SPELL_EFFECT_BIND_SIGHT = 82,
        SPELL_EFFECT_DUEL = 83,
        SPELL_EFFECT_STUCK = 84,
        SPELL_EFFECT_SUMMON_PLAYER = 85,
        SPELL_EFFECT_ACTIVATE_OBJECT = 86,
        SPELL_EFFECT_87 = 87,            // old SPELL_EFFECT_SUMMON_TOTEM_SLOT1
        SPELL_EFFECT_88 = 88,            // old SPELL_EFFECT_SUMMON_TOTEM_SLOT2
        SPELL_EFFECT_89 = 89,            // old SPELL_EFFECT_SUMMON_TOTEM_SLOT3
        SPELL_EFFECT_90 = 90,            // old SPELL_EFFECT_SUMMON_TOTEM_SLOT4
        SPELL_EFFECT_THREAT_ALL = 91,
        SPELL_EFFECT_ENCHANT_HELD_ITEM = 92,
        SPELL_EFFECT_93 = 93,            // old SPELL_EFFECT_SUMMON_PHANTASM
        SPELL_EFFECT_SELF_RESURRECT = 94,
        SPELL_EFFECT_SKINNING = 95,
        SPELL_EFFECT_CHARGE = 96,
        SPELL_EFFECT_97 = 97,            // old SPELL_EFFECT_SUMMON_CRITTER
        SPELL_EFFECT_KNOCK_BACK = 98,
        SPELL_EFFECT_DISENCHANT = 99,
        SPELL_EFFECT_INEBRIATE = 100,
        SPELL_EFFECT_FEED_PET = 101,
        SPELL_EFFECT_DISMISS_PET = 102,
        SPELL_EFFECT_REPUTATION = 103,
        SPELL_EFFECT_SUMMON_OBJECT_SLOT1 = 104,
        SPELL_EFFECT_SUMMON_OBJECT_SLOT2 = 105,
        SPELL_EFFECT_SUMMON_OBJECT_SLOT3 = 106,
        SPELL_EFFECT_SUMMON_OBJECT_SLOT4 = 107,
        SPELL_EFFECT_DISPEL_MECHANIC = 108,
        SPELL_EFFECT_SUMMON_DEAD_PET = 109,
        SPELL_EFFECT_DESTROY_ALL_TOTEMS = 110,
        SPELL_EFFECT_DURABILITY_DAMAGE = 111,
        SPELL_EFFECT_112 = 112,           // old SPELL_EFFECT_SUMMON_DEMON
        SPELL_EFFECT_RESURRECT_NEW = 113,
        SPELL_EFFECT_ATTACK_ME = 114,
        SPELL_EFFECT_DURABILITY_DAMAGE_PCT = 115,
        SPELL_EFFECT_SKIN_PLAYER_CORPSE = 116,
        SPELL_EFFECT_SPIRIT_HEAL = 117,
        SPELL_EFFECT_SKILL = 118,
        SPELL_EFFECT_APPLY_AREA_AURA_PET = 119,
        SPELL_EFFECT_TELEPORT_GRAVEYARD = 120,
        SPELL_EFFECT_NORMALIZED_WEAPON_DMG = 121,
        SPELL_EFFECT_122 = 122,
        SPELL_EFFECT_SEND_TAXI = 123,
        SPELL_EFFECT_PLAYER_PULL = 124,
        SPELL_EFFECT_MODIFY_THREAT_PERCENT = 125,
        SPELL_EFFECT_STEAL_BENEFICIAL_BUFF = 126,
        SPELL_EFFECT_PROSPECTING = 127,
        SPELL_EFFECT_APPLY_AREA_AURA_FRIEND = 128,
        SPELL_EFFECT_APPLY_AREA_AURA_ENEMY = 129,
        SPELL_EFFECT_REDIRECT_THREAT = 130,
        SPELL_EFFECT_PLAY_SOUND = 131,
        SPELL_EFFECT_PLAY_MUSIC = 132,
        SPELL_EFFECT_UNLEARN_SPECIALIZATION = 133,
        SPELL_EFFECT_KILL_CREDIT_GROUP = 134,
        SPELL_EFFECT_CALL_PET = 135,
        SPELL_EFFECT_HEAL_PCT = 136,
        SPELL_EFFECT_ENERGIZE_PCT = 137,
        SPELL_EFFECT_LEAP_BACK = 138,
        SPELL_EFFECT_CLEAR_QUEST = 139,
        SPELL_EFFECT_FORCE_CAST = 140,
        SPELL_EFFECT_FORCE_CAST_WITH_VALUE = 141,
        SPELL_EFFECT_TRIGGER_SPELL_WITH_VALUE = 142,
        SPELL_EFFECT_APPLY_AREA_AURA_OWNER = 143,
        SPELL_EFFECT_KNOCKBACK_FROM_POSITION = 144,
        SPELL_EFFECT_145 = 145,
        SPELL_EFFECT_146 = 146,
        SPELL_EFFECT_QUEST_FAIL = 147,
        SPELL_EFFECT_148 = 148,
        SPELL_EFFECT_CHARGE2 = 149,
        SPELL_EFFECT_150 = 150,
        SPELL_EFFECT_TRIGGER_SPELL_2 = 151,
        SPELL_EFFECT_152 = 152,
        SPELL_EFFECT_153 = 153,
        TOTAL_SPELL_EFFECTS = 154
    };
}