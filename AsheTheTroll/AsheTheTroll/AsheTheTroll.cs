﻿using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using Color = System.Drawing.Color;

namespace AsheTheTroll
{
    internal class AsheTheTroll
    {
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        private static AIHeroClient _target;

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static Spell.Active _q;
        private static Spell.Skillshot _w;
        private static Spell.Skillshot _e;
        private static Spell.Skillshot _r;
        public static Spell.Active Heal;

        public static float HealthPercent
        {
            get { return _Player.Health/_Player.MaxHealth*100; }
        }

        private static Item HealthPotion;
        private static Item CorruptingPotion;
        private static Item RefillablePotion;
        private static Item TotalBiscuit;
        private static Item HuntersPotion;
        public static Item Youmuu = new Item(ItemId.Youmuus_Ghostblade);
        public static Item Botrk = new Item(ItemId.Blade_of_the_Ruined_King);
        public static Item Cutlass = new Item(ItemId.Bilgewater_Cutlass);
        public static Item Tear = new Item(ItemId.Tear_of_the_Goddess);
        public static Item Qss = new Item(ItemId.Quicksilver_Sash);
        public static Item Simitar = new Item(ItemId.Mercurial_Scimitar);


        public static Menu Menu,
            ComboMenu,
            HarassMenu,
            JungleLaneMenu,
            MiscMenu,
            DrawMenu,
            ItemMenu,
            SkinMenu,
            AutoPotHealMenu,
            FleeMenu;





        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Ashe)
            {
                return;
            }


            _q = new Spell.Active(SpellSlot.Q);
            _w = new Spell.Skillshot(SpellSlot.W, 1200, SkillShotType.Linear, 0, int.MaxValue, 60);
            {
                _w.AllowedCollisionCount = 0;
            }
            _e = new Spell.Skillshot(SpellSlot.E, 15000, SkillShotType.Linear, 0, int.MaxValue, 0);
            _r = new Spell.Skillshot(SpellSlot.R, 15000, SkillShotType.Linear, 500, 1000, 250);
            _r.AllowedCollisionCount = int.MaxValue;
            var slot = _Player.GetSpellSlotFromName("summonerheal");
            if (slot != SpellSlot.Unknown)
            {
                Heal = new Spell.Active(slot, 600);
            }
            HealthPotion = new Item(2003, 0);
            TotalBiscuit = new Item(2010, 0);
            CorruptingPotion = new Item(2033, 0);
            RefillablePotion = new Item(2031, 0);
            HuntersPotion = new Item(2032, 0);

            Chat.Print(
                "<font color=\"#4dd5ea\" >MeLoDag Presents </font><font color=\"#ffffff\" >AsheTheToLL </font><font color=\"#4dd5ea\" >Kappa Kippo</font>");


            Menu = MainMenu.AddMenu("AsheTheTroll", "AsheTheTroll");

            ComboMenu = Menu.AddSubMenu("Combo Settings", "Combo");
            ComboMenu.Add("useQCombo", new CheckBox("Use Q"));
            ComboMenu.Add("useWCombo", new CheckBox("Use W"));
            ComboMenu.AddGroupLabel("R Logics");
            ComboMenu.Add("useRCombo", new CheckBox("Use R [Hp%]"));
            ComboMenu.Add("Hp", new Slider("Use R Enemy Health %", 45, 0, 100));
            ComboMenu.Add("useRComboENEMIES", new CheckBox("Use R[Count Enemy]"));
            ComboMenu.Add("Rcount", new Slider("R When Enemies >= ", 2, 1, 5));
            ComboMenu.Add("useRComboFinisher", new CheckBox("Use R [FinisherMode]"));
            ComboMenu.Add("ForceR",
                new KeyBind("Force R On Target Selector", false, KeyBind.BindTypes.HoldActive, "T".ToCharArray()[0]));
            ComboMenu.Add("useRRange", new Slider("Use Ulty Max Range", 1000, 500, 2000));

            HarassMenu = Menu.AddSubMenu("Harass Settings", "Harass");
            HarassMenu.Add("useQHarass", new CheckBox("Use Q"));
            HarassMenu.Add("useWHarass", new CheckBox("Use W"));
            HarassMenu.Add("useWHarassMana", new Slider("W Mana > %", 70, 0, 100));
            HarassMenu.AddLabel("AutoHarass");
            HarassMenu.Add("autoWHarass", new CheckBox("Auto W for Harass", false));
            HarassMenu.Add("autoWHarassMana", new Slider("W Mana > %", 70, 0, 100));

            JungleLaneMenu = Menu.AddSubMenu("Lane Clear Settings", "FarmSettings");
            JungleLaneMenu.AddLabel("Lane Clear");
            JungleLaneMenu.Add("useWFarm", new CheckBox("Use W"));
            JungleLaneMenu.Add("useWManalane", new Slider("W Mana > %", 70, 0, 100));
            JungleLaneMenu.AddLabel("Jungle Clear");
            // JungleLaneMenu.Add("useQJungle", new CheckBox("Use Q"));
            JungleLaneMenu.Add("useWJungle", new CheckBox("Use W"));
            JungleLaneMenu.Add("useWMana", new Slider("W Mana > %", 70, 0, 100));

            FleeMenu = Menu.AddSubMenu("Flee Settings", "FleeSettings");
            FleeMenu.Add("FleeQ", new CheckBox("Use W"));

            MiscMenu = Menu.AddSubMenu("Misc Settings", "MiscSettings");
            MiscMenu.AddGroupLabel("Gapcloser & Interrupter settings");
            MiscMenu.Add("gapcloser", new CheckBox("Auto W for Gapcloser"));
            MiscMenu.Add("interrupter", new CheckBox("Auto R for Interrupter"));
            MiscMenu.AddGroupLabel("Auto use skils CC Enemy");
            MiscMenu.Add("CCE", new CheckBox("Auto W on Enemy CC"));
            MiscMenu.AddGroupLabel("Ks Settings");
            MiscMenu.Add("UseWks", new CheckBox("Use W ks"));
            MiscMenu.Add("UseRks", new CheckBox("Use R ks"));
            MiscMenu.Add("UseRksRange", new Slider("Use Ulty Max Range[KS]", 1000, 500, 2000));

            AutoPotHealMenu = Menu.AddSubMenu("Potion & HeaL", "Potion & HeaL");
            AutoPotHealMenu.AddGroupLabel("Auto pot usage");
            AutoPotHealMenu.Add("potion", new CheckBox("Use potions"));
            AutoPotHealMenu.Add("potionminHP", new Slider("Minimum Health % to use potion", 40));
            AutoPotHealMenu.Add("potionMinMP", new Slider("Minimum Mana % to use potion", 20));
            AutoPotHealMenu.AddGroupLabel("AUto Heal Usage");
            AutoPotHealMenu.Add("UseHeal", new CheckBox("Use Heal"));
            AutoPotHealMenu.Add("useHealHP", new Slider("Minimum Health % to use Heal", 20));

            ItemMenu = Menu.AddSubMenu("Item Settings", "ItemMenuettings");
            ItemMenu.Add("useBOTRK", new CheckBox("Use BOTRK"));
            ItemMenu.Add("useBotrkMyHP", new Slider("My Health < ", 60, 1, 100));
            ItemMenu.Add("useBotrkEnemyHP", new Slider("Enemy Health < ", 60, 1, 100));
            ItemMenu.Add("useYoumu", new CheckBox("Use Youmu"));
            ItemMenu.AddGroupLabel("QQs Settings");
            ItemMenu.Add("useQSS", new CheckBox("Use QSS"));
            ItemMenu.Add("Qssmode", new ComboBox(" ", 0, "Auto", "Combo"));
            ItemMenu.Add("Stun", new CheckBox("Stun", true));
            ItemMenu.Add("Blind", new CheckBox("Blind", true));
            ItemMenu.Add("Charm", new CheckBox("Charm", true));
            ItemMenu.Add("Suppression", new CheckBox("Suppression", true));
            ItemMenu.Add("Polymorph", new CheckBox("Polymorph", true));
            ItemMenu.Add("Fear", new CheckBox("Fear", true));
            ItemMenu.Add("Taunt", new CheckBox("Taunt", true));
            ItemMenu.Add("Silence", new CheckBox("Silence", false));
            ItemMenu.Add("QssDelay", new Slider("Use QSS Delay(ms)", 250, 0, 1000));
            ItemMenu.AddGroupLabel("Qqs Utly");
            ItemMenu.Add("ZedUlt", new CheckBox("Zed R", true));
            ItemMenu.Add("VladUlt", new CheckBox("Vladimir R", true));
            ItemMenu.Add("FizzUlt", new CheckBox("Fizz R", true));
            ItemMenu.Add("MordUlt", new CheckBox("Mordekaiser R", true));
            ItemMenu.Add("PoppyUlt", new CheckBox("Poppy R", true));
            ItemMenu.Add("QssUltDelay", new Slider("Use QSS Delay(ms) for Ult", 250, 0, 1000));

            SkinMenu = Menu.AddSubMenu("Skin Changer", "SkinChanger");
            SkinMenu.Add("checkSkin", new CheckBox("Use Skin Changer"));
            SkinMenu.Add("skin.Id", new Slider("Skin", 1, 0, 8));

            DrawMenu = Menu.AddSubMenu("Drawing Settings");
            DrawMenu.Add("drawRange", new CheckBox("Draw Q Range"));
            DrawMenu.Add("drawW", new CheckBox("Draw W Range"));
            DrawMenu.Add("drawR", new CheckBox("Draw R Range"));
            DrawMenu.Add("ShowStatus", new CheckBox("ShowStatus"));
            DrawMenu.Add("AutoHarass", new CheckBox("AutoHarass"));

            Game.OnTick += Game_OnTick;
            Game.OnUpdate += OnGameUpdate;
            Obj_AI_Base.OnBuffGain += OnBuffGain;
            Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Drawing.OnDraw += Drawing_OnDraw;

        }



        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs e)
        {
            if (MiscMenu["interrupter"].Cast<CheckBox>().CurrentValue && sender.IsEnemy &&
                e.DangerLevel == EloBuddy.SDK.Enumerations.DangerLevel.High && sender.IsValidTarget(850))
            {
                _r.Cast(sender);
            }
        }

        private static
            void OnGameUpdate(EventArgs args)
        {
            Orbwalker.ForcedTarget = null;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
                UseQ();
                ItemUsage();
                AUtoheal();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                WaveClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();
            }

            Ks();
            Auto();
            AutoW();
            UseRTarget();
            AutoPot();
        }

        private static void AUtoheal()
        {
            if (Heal != null && AutoPotHealMenu["UseHeal"].Cast<CheckBox>().CurrentValue && Heal.IsReady() &&
                HealthPercent <= AutoPotHealMenu["useHealHP"].Cast<Slider>().CurrentValue
                && _Player.CountEnemiesInRange(600) > 0 && Heal.IsReady())
            {
                Heal.Cast();
                Chat.Print("<font color=\"#fffffff\" > Use HeaL Kappa kippo</font>");
            }
        }

        private static
            void AutoPot()
        {
            if (AutoPotHealMenu["potion"].Cast<CheckBox>().CurrentValue && !Player.Instance.IsInShopRange() &&
                Player.Instance.HealthPercent <= AutoPotHealMenu["potionminHP"].Cast<Slider>().CurrentValue &&
                !(Player.Instance.HasBuff("RegenerationPotion") || Player.Instance.HasBuff("ItemCrystalFlaskJungle") ||
                  Player.Instance.HasBuff("ItemMiniRegenPotion") || Player.Instance.HasBuff("ItemCrystalFlask") ||
                  Player.Instance.HasBuff("ItemDarkCrystalFlask")))
            {
                if (Item.HasItem(HealthPotion.Id) && Item.CanUseItem(HealthPotion.Id))
                {
                    HealthPotion.Cast();
                    Chat.Print("<font color=\"#fffffff\" > Use Pot Kappa kippo</font>");
                    return;
                }
                if (Item.HasItem(TotalBiscuit.Id) && Item.CanUseItem(TotalBiscuit.Id))
                {
                    TotalBiscuit.Cast();
                    Chat.Print("<font color=\"#fffffff\" > Use Pot Kappa kippo</font>");
                    return;
                }
                if (Item.HasItem(RefillablePotion.Id) && Item.CanUseItem(RefillablePotion.Id))
                {
                    RefillablePotion.Cast();
                    Chat.Print("<font color=\"#fffffff\" > Use Pot Kappa kippo</font>");
                    return;
                }
                if (Item.HasItem(CorruptingPotion.Id) && Item.CanUseItem(CorruptingPotion.Id))
                {
                    CorruptingPotion.Cast();
                    Chat.Print("<font color=\"#fffffff\" > Use Pot Kappa kippo</font>");
                    return;
                }
            }
            if (Player.Instance.ManaPercent <= AutoPotHealMenu["potionMinMP"].Cast<Slider>().CurrentValue &&
                !(Player.Instance.HasBuff("RegenerationPotion") || Player.Instance.HasBuff("ItemMiniRegenPotion") ||
                  Player.Instance.HasBuff("ItemCrystalFlask") || Player.Instance.HasBuff("ItemDarkCrystalFlask")))
            {
                if (Item.HasItem(CorruptingPotion.Id) && Item.CanUseItem(CorruptingPotion.Id))
                {
                    CorruptingPotion.Cast();
                    Chat.Print("<font color=\"#fffffff\" > Use Pot Kappa kippo</font>");
                }
            }
        }

        //Gredit GuTenTak
        public static
            void ItemUsage()
        {
            var target = TargetSelector.GetTarget(550, DamageType.Physical); // 550 = Botrk.Range


            if (ItemMenu["useYoumu"].Cast<CheckBox>().CurrentValue && Youmuu.IsOwned() && Youmuu.IsReady())
            {
                if (ObjectManager.Player.CountEnemiesInRange(1500) == 1)
                {
                    Youmuu.Cast();
                }
            }
            if (target != null)
            {
                if (ItemMenu["useBOTRK"].Cast<CheckBox>().CurrentValue && Item.HasItem(Cutlass.Id) &&
                    Item.CanUseItem(Cutlass.Id) &&
                    Player.Instance.HealthPercent < ItemMenu["useBotrkMyHP"].Cast<Slider>().CurrentValue &&
                    target.HealthPercent < ItemMenu["useBotrkEnemyHP"].Cast<Slider>().CurrentValue)
                {
                    Item.UseItem(Cutlass.Id, target);
                }
                if (ItemMenu["useBOTRK"].Cast<CheckBox>().CurrentValue && Item.HasItem(Botrk.Id) &&
                    Item.CanUseItem(Botrk.Id) &&
                    Player.Instance.HealthPercent < ItemMenu["useBotrkMyHP"].Cast<Slider>().CurrentValue &&
                    target.HealthPercent < ItemMenu["useBotrkEnemyHP"].Cast<Slider>().CurrentValue)
                {
                    Botrk.Cast(target);
                }
            }
        }

        private static void OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!sender.IsMe) return;
            var type = args.Buff.Type;
            var duration = args.Buff.EndTime - Game.Time;
            var Name = args.Buff.Name.ToLower();

            if (ItemMenu["Qssmode"].Cast<ComboBox>().CurrentValue == 0)
            {
                if (type == BuffType.Taunt && ItemMenu["Taunt"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Stun && ItemMenu["Stun"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Snare && ItemMenu["Snare"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Polymorph && ItemMenu["Polymorph"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Blind && ItemMenu["Blind"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Flee && ItemMenu["Fear"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Charm && ItemMenu["Charm"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Suppression && ItemMenu["Suppression"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Silence && ItemMenu["Silence"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (Name == "zedrdeathmark" && ItemMenu["ZedUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "vladimirhemoplague" && ItemMenu["VladUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "fizzmarinerdoom" && ItemMenu["FizzUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "mordekaiserchildrenofthegrave" && ItemMenu["MordUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "poppydiplomaticimmunity" && ItemMenu["PoppyUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
            }
            if (ItemMenu["Qssmode"].Cast<ComboBox>().CurrentValue == 1 &&
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (type == BuffType.Taunt && ItemMenu["Taunt"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Stun && ItemMenu["Stun"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Snare && ItemMenu["Snare"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Polymorph && ItemMenu["Polymorph"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Blind && ItemMenu["Blind"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Flee && ItemMenu["Fear"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Charm && ItemMenu["Charm"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Suppression && ItemMenu["Suppression"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Silence && ItemMenu["Silence"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (Name == "zedrdeathmark" && ItemMenu["ZedUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "vladimirhemoplague" && ItemMenu["VladUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "fizzmarinerdoom" && ItemMenu["FizzUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "mordekaiserchildrenofthegrave" && ItemMenu["MordUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "poppydiplomaticimmunity" && ItemMenu["PoppyUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
            }
        }

        private static void DoQSS()
        {
            if (ItemMenu["useQSS"].Cast<CheckBox>().CurrentValue && Qss.IsOwned() && Qss.IsReady() &&
                ObjectManager.Player.CountEnemiesInRange(1800) > 0)
            {
                Core.DelayAction(() => Qss.Cast(), ItemMenu["QssDelay"].Cast<Slider>().CurrentValue);
            }
            if (Simitar.IsOwned() && Simitar.IsReady() && ObjectManager.Player.CountEnemiesInRange(1800) > 0)
            {
                Core.DelayAction(() => Simitar.Cast(), ItemMenu["QssDelay"].Cast<Slider>().CurrentValue);
            }
        }

        private static void UltQSS()
        {
            if (ItemMenu["useQSS"].Cast<CheckBox>().CurrentValue && Qss.IsOwned() && Qss.IsReady())
            {
                Core.DelayAction(() => Qss.Cast(), ItemMenu["QssUltDelay"].Cast<Slider>().CurrentValue);
            }
            if (Simitar.IsOwned() && Simitar.IsReady())
            {
                Core.DelayAction(() => Simitar.Cast(), ItemMenu["QssUltDelay"].Cast<Slider>().CurrentValue);
            }
        }

        private static void UseQ()
        {
            if (ComboMenu["useQCombo"].Cast<CheckBox>().CurrentValue && _q.IsReady())
            {
                if (Player.Instance.CountEnemiesInRange(600) == 1)
                {
                    foreach (var a in Player.Instance.Buffs)
                        if (a.Name == "asheqcastready" && a.Count == 4)
                        {
                            _q.Cast();
                        }
                }
            }
        }

        private static void Combo()
        {
            var rCount = ComboMenu["Rcount"].Cast<Slider>().CurrentValue;
            var comboR = ComboMenu["useRComboENEMIES"].Cast<CheckBox>().CurrentValue;
            var useR = ComboMenu["useRcombo"].Cast<CheckBox>().CurrentValue;
            var useW = ComboMenu["useWCombo"].Cast<CheckBox>().CurrentValue;
            var hp = ComboMenu["Hp"].Cast<Slider>().CurrentValue;
            var ultyFinisher = ComboMenu["useRComboFinisher"].Cast<CheckBox>().CurrentValue;
            var targetR = TargetSelector.GetTarget(_r.Range, DamageType.Magical);
            var target = TargetSelector.GetTarget(_q.Range, DamageType.Magical);
            var distance = ComboMenu["useRRange"].Cast<Slider>().CurrentValue;

            if (target == null || !target.IsValidTarget()) return;


            if (_w.IsReady() && useW)
            {
                var predW = _w.GetPrediction(target);
                if (predW.HitChance >= HitChance.Immobile)
                {
                    _w.Cast(predW.CastPosition);
                }
                else if (predW.HitChance >= HitChance.High)
                {
                    _w.Cast(predW.CastPosition);
                }
                if (_r.IsReady() && useR && targetR.Distance(_Player) <= distance &&
                    _r.GetPrediction(targetR).HitChance >= HitChance.High && target.HealthPercent <= hp)
                {
                    _r.Cast(_r.GetPrediction(targetR).CastPosition);
                }

                if (comboR && _Player.CountEnemiesInRange(_Player.AttackRange) >= rCount && _r.IsReady()
                    && targetR.Distance(_Player) <= distance && targetR != null &&
                    _r.GetPrediction(targetR).HitChance >= HitChance.High)
                {
                    _r.Cast(_r.GetPrediction(targetR).CastPosition);

                }
                if (_r.IsReady() && ultyFinisher && targetR.Distance(_Player) <= distance &&
                    RDamage(targetR) >= targetR.Health)
                {
                    _r.Cast(targetR);
                }
            }
        }


        public static
            void UseRTarget()
        {
            var target = TargetSelector.GetTarget(_r.Range, DamageType.Magical);
            if (target != null &&
                (ComboMenu["ForceR"].Cast<KeyBind>().CurrentValue && _r.IsReady() && target.IsValid &&
                 !Player.HasBuff("AsheR"))) _r.Cast(target.Position);
        }


        private static void Game_OnTick(EventArgs args)
        {
            if (CheckSkin())
            {
                Player.SetSkinId(SkinId());
            }
        }

        public static int SkinId()
        {
            return SkinMenu["skin.Id"].Cast<Slider>().CurrentValue;
        }

        public static bool CheckSkin()
        {
            return SkinMenu["checkSkin"].Cast<CheckBox>().CurrentValue;
        }

        public static void Auto()
        {
            var eonCc = MiscMenu["CCE"].Cast<CheckBox>().CurrentValue;
            if (eonCc)
            {
                foreach (var enemy in EntityManager.Heroes.Enemies)
                {
                    if (enemy.Distance(Player.Instance) < _w.Range &&
                        (enemy.HasBuffOfType(BuffType.Stun)
                         || enemy.HasBuffOfType(BuffType.Snare)
                         || enemy.HasBuffOfType(BuffType.Suppression)
                         || enemy.HasBuffOfType(BuffType.Fear)
                         || enemy.HasBuffOfType(BuffType.Knockup)))
                    {
                        _w.Cast(enemy);
                    }
                }
            }
        }


        public static void AutoW()
        {
            var targetW = TargetSelector.GetTarget(_w.Range, DamageType.Physical);
            if (HarassMenu["autoWHarass"].Cast<CheckBox>().CurrentValue &&
                _w.IsReady() && targetW.IsValidTarget(_w.Range) &&
                Player.Instance.ManaPercent > HarassMenu["autoWHarassMana"].Cast<Slider>().CurrentValue)
            {
                _w.Cast(targetW);
            }
        }

        public static void Flee()
        {
            var targetW = TargetSelector.GetTarget(_w.Range, DamageType.Physical);
            var FleeQ = FleeMenu["FleeQ"].Cast<CheckBox>().CurrentValue;

            if (FleeQ && _w.IsReady() && targetW.IsValidTarget(_w.Range))
            {
                _w.Cast(targetW);
            }
        }

        public static void JungleClear()
        {
            var useW = JungleLaneMenu["useWJungle"].Cast<CheckBox>().CurrentValue;
            var junglemana = JungleLaneMenu["useWMana"].Cast<Slider>().CurrentValue;

            if (Orbwalker.IsAutoAttacking) return;
            {
                if (useW && Player.Instance.ManaPercent > junglemana)
                {
                    var minions =
                        EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, _w.Range)
                            .Where(t => !t.IsDead && t.IsValid && !t.IsInvulnerable);
                    if (minions.Count() > 0)
                    {
                        _w.Cast(minions.First());
                    }
                }
            }
        }

        public static void WaveClear()
        {
            var useW = JungleLaneMenu["useWFarm"].Cast<CheckBox>().CurrentValue;
            var junglemana = JungleLaneMenu["useWManalane"].Cast<Slider>().CurrentValue;


            if (Orbwalker.IsAutoAttacking) return;

            if (useW && Player.Instance.ManaPercent > junglemana)
            {
                var minions =
                    EntityManager.MinionsAndMonsters.EnemyMinions.Where(
                        t =>
                            t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable &&
                            t.IsInRange(Player.Instance.Position, _w.Range));
                foreach (var m in minions)
                {
                    if (
                        _w.GetPrediction(m)
                            .CollisionObjects.Where(t => t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable)
                            .Count() >= 0)
                    {
                        _w.Cast(m);
                        break;
                    }
                }
            }
        }


        public static
            void Harass()
        {
            var targetW = TargetSelector.GetTarget(_w.Range, DamageType.Physical);
            var target = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
            var wmana = HarassMenu["useWHarassMana"].Cast<Slider>().CurrentValue;
            var wharass = HarassMenu["useWHarass"].Cast<CheckBox>().CurrentValue;
            var useQharass = HarassMenu["useQHarass"].Cast<CheckBox>().CurrentValue;

            Orbwalker.ForcedTarget = null;

            if (Orbwalker.IsAutoAttacking) return;

            if (targetW != null)
            {

                if (wharass && _w.IsReady() &&
                    target.Distance(_Player) > _Player.AttackRange &&
                    targetW.IsValidTarget(_w.Range) && Player.Instance.ManaPercent > wmana)
                {
                    _w.Cast(targetW);
                }
            }

            if (target != null)
            {
                if (useQharass && _q.IsReady())
                {
                    if (target.Distance(_Player) <= Player.Instance.AttackRange)
                    {
                        _q.Cast();
                    }
                }
            }
        }

        public static void Gapcloser_OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (MiscMenu["gapcloser"].Cast<CheckBox>().CurrentValue && sender.IsEnemy &&
                e.End.Distance(_Player) < 1200)
            {
                _w.Cast(e.End);
            }

        }

        public static void Ks()
        {
            var distance = MiscMenu["UseRksRange"].Cast<Slider>().CurrentValue;
            var usewks = MiscMenu["UseWks"].Cast<CheckBox>().CurrentValue;
            var useRks = MiscMenu["UseRks"].Cast<CheckBox>().CurrentValue;
            foreach (
                var enemy in
                    EntityManager.Heroes.Enemies.Where(
                        e => e.Distance(_Player) <= _w.Range && e.IsValidTarget() && !e.IsInvulnerable))
            {

                if (usewks && _w.IsReady() &&
                    WDamage(enemy) >= enemy.Health && enemy.Distance(_Player) <= _w.Range)
                {
                    _w.Cast(enemy);
                    Chat.Print("<font color=\"#fffffff\" > Use W Free Kill</font>");
                }
                if (useRks && _r.IsReady() && RDamage(enemy) >= enemy.Health &&
                    enemy.Distance(_Player) <= distance)
                {
                    _r.Cast(enemy);
                    Chat.Print("<font color=\"#fffffff\" > Use Ulty Free Kill</font>");
                }
            }
        }

        public static
            int WDamage(Obj_AI_Base target)
        {
            return
                (int)
                    (new[] {10, 60, 110, 160, 210}[_w.Level - 1] +
                     1.4*(_Player.TotalAttackDamage));
        }

        public static float RDamage(Obj_AI_Base target)
        {

            if (!Player.GetSpell(SpellSlot.R).IsLearned) return 0;
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                (float) new double[] {250, 425, 600}[_r.Level - 1] + 1*Player.Instance.FlatMagicDamageMod);

        }

        private static void Drawing_OnDraw(EventArgs args)
        {


            {
                if (DrawMenu["drawRange"].Cast<CheckBox>().CurrentValue)
                {
                    if (_q.IsReady()) new Circle {Color = Color.Aqua, Radius = _q.Range}.Draw(_Player.Position);
                    else if (_q.IsOnCooldown)
                        new Circle {Color = Color.Gray, Radius = _q.Range}.Draw(_Player.Position);
                }

                if (DrawMenu["drawW"].Cast<CheckBox>().CurrentValue)
                {
                    if (_w.IsReady()) new Circle {Color = Color.Aqua, Radius = _w.Range}.Draw(_Player.Position);
                    else if (_w.IsOnCooldown)
                        new Circle {Color = Color.Gray, Radius = _w.Range}.Draw(_Player.Position);
                }

                if (DrawMenu["drawR"].Cast<CheckBox>().CurrentValue)
                {
                    if (_r.IsReady()) new Circle {Color = Color.Aqua, Radius = _r.Range}.Draw(_Player.Position);
                    else if (_r.IsOnCooldown)
                        new Circle {Color = Color.Gray, Radius = _r.Range}.Draw(_Player.Position);
                }
            }
        }
    }
}
            





   