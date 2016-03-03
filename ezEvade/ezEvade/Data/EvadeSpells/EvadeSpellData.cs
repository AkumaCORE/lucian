﻿using System.Text;
using ezEvade.Data.Spells;
using EloBuddy;

namespace ezEvade.Data.EvadeSpells
{
    public delegate bool UseSpellFunc(EvadeSpellData evadeSpell, bool process = true);

    public enum CastType
    {
        Position,
        Target,
        Self,
    }

    public enum SpellTargets
    {
        AllyMinions,
        EnemyMinions,

        AllyChampions,
        EnemyChampions,

        Targetables,
    }

    public enum EvadeType
    {
        Blink,
        Dash,
        Invulnerability,
        MovementSpeedBuff,
        Shield,
        SpellShield,
        WindWall,
    }

    public class EvadeSpellData
    {
        public string CharName;
        public SpellSlot SpellKey = SpellSlot.Q;
        public SpellDangerLevel Dangerlevel = SpellDangerLevel.Low;
        public string SpellName;
        public string Name;
        public bool CheckSpellName = false;
        public float SpellDelay = 250;
        public float Range;
        public float Speed = 0;
        public bool FixedRange = false;
        public EvadeType EvadeType;
        public bool IsReversed = false;
        public bool BehindTarget = false;
        public bool InfrontTarget = false;
        public bool IsSummonerSpell = false;
        public bool IsItem = false;
        public ItemId ItemId = 0;
        public CastType CastType = CastType.Position;
        public SpellTargets[] SpellTargets = { };
        public UseSpellFunc UseSpellFunc = null;
        public bool IsSpecial = false;

        public EvadeSpellData()
        {

        }

        public EvadeSpellData(
            string charName,
            string name,
            SpellSlot spellKey,
            EvadeType evadeType,
            SpellDangerLevel dangerlevel
            )
        {
            CharName = charName;
            Name = name;
            SpellKey = spellKey;
            EvadeType = evadeType;
            Dangerlevel = dangerlevel;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Name " + SpellName);
            sb.Append(" DangerLevel: " + Dangerlevel);
            sb.Append(" EvadeType: " + EvadeType);
            sb.Append("Range: " + Range);
            return sb.ToString();
        }
    }
}
