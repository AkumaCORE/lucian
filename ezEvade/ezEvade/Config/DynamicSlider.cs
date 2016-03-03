﻿using System;
using ezEvade.Data;
using ezEvade.Data.Spells;
using ezEvade.Draw;
using EloBuddy.SDK.Menu.Values;

namespace ezEvade.Config
{
    public enum SpellConfigProperty
    {
        Dodge, Draw, Radius, DangerLevel, SpellMode, UseEvadeSpell, None, 
    }
    public class DynamicSlider
    {
        public Slider Slider;
        private readonly ConfigDataType _type;
        private readonly string _configKey;
        private bool _isBasedOnSpell;
        private SpellConfigProperty _spellProperty;

        public DynamicSlider(ConfigDataType configDataType, string key, string displayName, int defaultValue, int minValue, int maxValue)
        {
            _type = configDataType;
            _configKey = key;
            DynamicSliderInit(displayName, defaultValue, minValue, maxValue, false, SpellConfigProperty.None);
        }

        public DynamicSlider(ConfigDataType configDataType, string key, string displayName, int defaultValue,
            int minValue, int maxValue, bool isBasedOnSpell, SpellConfigProperty property)
        {
            _type = configDataType;
            _configKey = key;
            DynamicSliderInit(displayName, defaultValue, minValue, maxValue, isBasedOnSpell, property);
        }

        public void DynamicSliderInit(string displayName, int defaultValue,
            int minValue, int maxValue, bool isBasedOnSpell, SpellConfigProperty property)
        {
            _spellProperty = property;
            _isBasedOnSpell = isBasedOnSpell;
            Slider = new Slider(displayName, defaultValue, minValue, maxValue);
            switch (_type)
            {
                case ConfigDataType.Data:
                    Properties.SetData(_configKey, defaultValue);
                    break;
            }
            Slider.OnValueChange += Slider_OnValueChange;
            //Properties.OnConfigValueChanged += Config_OnConfigValueChanged;
        }


        //private void Config_OnConfigValueChanged(ConfigValueChangedArgs args)
        //{
        //    if (args.Key == _configKey && args.Type == _type && (!_isBasedOnSpell || _isBasedOnSpell && _spellProperty == args.Property))
        //    {
        //        Slider.CurrentValue = (int) args.Value;
        //    }
        //}

        private void Slider_OnValueChange(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
        {
            Debug.DrawTopLeft(_type + ": " + _spellProperty + ": " + _configKey + ": " + "Value Changed To " + sender.CurrentValue);
            switch (_type)
            {
                case ConfigDataType.Data:
                    Properties.SetData(_configKey, sender.CurrentValue, false);
                    break;
                case ConfigDataType.Spells:
                    if (_isBasedOnSpell)
                    {
                        var spell = Properties.GetSpell(_configKey);
                        switch (_spellProperty)
                        {
                            case SpellConfigProperty.Radius:
                                spell.Radius = sender.CurrentValue;
                                break;
                            case SpellConfigProperty.DangerLevel:
                                spell.DangerLevel = (SpellDangerLevel) sender.CurrentValue;
                                break;
                            case SpellConfigProperty.SpellMode:
                                spell.EvadeSpellMode = (SpellModes) sender.CurrentValue;
                                break;
                            default:
                                return;
                        }
                        Properties.SetSpell(_configKey, spell);
                    }
                    break;
                case ConfigDataType.EvadeSpell:
                    if (_isBasedOnSpell)
                    {
                        var spell = Properties.GetEvadeSpell(_configKey);
                        switch (_spellProperty)
                        {
                            case SpellConfigProperty.DangerLevel:
                                spell.DangerLevel = (SpellDangerLevel) sender.CurrentValue;
                                break;
                            case SpellConfigProperty.SpellMode:
                                spell.SpellMode = (SpellModes) sender.CurrentValue;
                                break;
                            default:
                                return;
                        }
                        Properties.SetEvadeSpell(_configKey, spell);
                    }
                    break;
            }
        }
    }
}