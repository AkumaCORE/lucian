﻿using System;
using System.Collections.Generic;
using System.Linq;
using ezEvade.Data;
using ezEvade.Data.Spells;
using ezEvade.Draw;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SpellData = ezEvade.Data.Spells.SpellData;

namespace ezEvade.Config
{
    public enum ConfigDataType
    {
        Data, Spells, KeyBind, EvadeSpell
    }

    public class ConfigValueChangedArgs : EventArgs
    {
        public string Key { get; set; }
        public object Value { get; set; }

        public ConfigValueChangedArgs(string key, object value)
        {
            Key = key;
            Value = value;
        }
    }
    public static class Properties
    {
        public delegate void ConfigValueChangedHandler(ConfigValueChangedArgs args);
        public static event ConfigValueChangedHandler OnConfigValueChanged;


        public static readonly Dictionary<string, object> Data = new Dictionary<string, object>();

        public static readonly Dictionary<string, SpellConfig> Spells = new Dictionary<string, SpellConfig>();

        public static readonly Dictionary<string, EvadeSpellConfig> EvadeSpells = new Dictionary<string, EvadeSpellConfig>();

        public static readonly Dictionary<string, KeyBind> Keys = new Dictionary<string, KeyBind>();

        public static SpellConfig GetSpellConfig(this SpellData spell, SpellConfigControl control)
        {
            return new SpellConfig
            {
                DangerLevel = spell.Dangerlevel,
                Dodge = true,
                Draw = true,
                Radius = spell.Radius,
                PlayerName = spell.CharName
            };
        }

        public static T GetData<T>(string key)
        {
            if (Data.Any(i => i.Key == key))
            {
                if(Data[key] is T)
                    return (T) Data[key];
                else 
                    Debug.DrawTopLeft("Tryed To Access key with wrong type: " + key);
            }
            return default(T);
        }
        public static SpellConfig GetSpell(string key)
        {
            if (Spells.Any(i => i.Key == key))
            {
                //Debug.DrawTopLeft("Found Spell at key: " + key + " = " + Spells[key]);
                return Spells[key];
            }
            if (GetData<bool>("EnableSpellTester"))
                if (SpellDatabase.Spells.Any(x => x.SpellName == key))
                {
                    var spellfromdb = SpellDatabase.Spells.First(x => x.SpellName == key);
                    return new SpellConfig { DangerLevel = spellfromdb.Dangerlevel, Radius = spellfromdb.Radius, Dodge = true, Draw = true, EvadeSpellMode = SpellModes.Always};
                }
            Debug.DrawTopLeft("*Spell: " + key + " Not Found, Returning: DO NOT DODGE");
            return new SpellConfig { DangerLevel = SpellDangerLevel.Normal, Dodge = false, Draw = true, EvadeSpellMode = SpellModes.Undodgeable, Radius = 20 };

        }
        public static EvadeSpellConfig GetEvadeSpell(string key)
        {
            if (EvadeSpells.Any(i => i.Key == key))
            {
                return EvadeSpells[key];
            }
            Debug.DrawTopLeft(" * Evade Spell: " + key + " Not Found, Returning: DO NOT USE");
            return new EvadeSpellConfig { DangerLevel = SpellDangerLevel.Low, SpellMode = SpellModes.Undodgeable, Use = false};
        }
        public static void SetData(string key, object value, bool raiseEvent = true)
        {
            if (Data.Any(i => i.Key == key))
            {
                Data[key] = value;
                return;
            }
            Data.Add(key, value);
            if (raiseEvent && OnConfigValueChanged != null)
                OnConfigValueChanged.Invoke(new ConfigValueChangedArgs(key, value));
        }
        public static void SetSpell(string id, SpellConfig value, bool raiseEvent = true)
        {
            if (Spells.Any(i => i.Key == id))
            {
                Spells[id] = value;
                return;
            }
            Spells.Add(id, value);
            if (raiseEvent && OnConfigValueChanged != null)
                OnConfigValueChanged.Invoke(new ConfigValueChangedArgs(id, value));
        }
        public static void SetEvadeSpell(string key, EvadeSpellConfig value, bool raiseEvent = true)
        {
            if (EvadeSpells.Any(i => i.Key == key))
            {
                EvadeSpells[key] = value;
                return;
            }
            EvadeSpells.Add(key, value);
            if (raiseEvent && OnConfigValueChanged != null)
                OnConfigValueChanged.Invoke(new ConfigValueChangedArgs(key, value));
        }

        public static void SetKey(string key, KeyBind value, bool raiseEvent = true)
        {
            if (Keys.Any(i => i.Key == key))
            {
                Keys[key] = value;
                return;
            }
            Keys.Add(key, value);
        }
    }

    public class EvadeSpellConfig
    {
        public bool Use { get; set; }
        public SpellDangerLevel DangerLevel { get; set; }
        public SpellModes SpellMode { get; set; }

        public override string ToString()
        {
            return string.Format("Use: {0} DangerLevel: {1} SpellMode: {2}", Use, DangerLevel, SpellMode);
        }
    }

    public class SpellConfig
    {
        public bool Dodge { get; set; }
        public bool Draw { get; set; }
        public float Radius { get; set; }
        public SpellDangerLevel DangerLevel { get; set; }
        public SpellModes EvadeSpellMode { get; set; }
        public string PlayerName { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}:{3}:{4}:{5}", Dodge, Draw, Radius, DangerLevel, EvadeSpellMode, PlayerName);
        }
    }
}
