using System.Collections.Generic;
using System.Linq;
using MessagePack;
using Server;
using Server.Items;
using Server.Mobiles;
using ZuluContent.Zulu.Engines.Magic.Enums;

namespace ZuluContent.Zulu.Engines.Magic.Enchantments
{
    [MessagePackObject]
    public class SlayerHit : Enchantment<SlayerHitInfo>
    {
        [IgnoreMember] public override string AffixName => EnchantmentInfo.GetName((int) Type, Cursed);

        [IgnoreMember]
        public override EnchantmentInfo Info => SlayerHitInfo.Variants[Type];

        [Key(1)] public CreatureType Type { get; set; } = CreatureType.None;

        [CallPriority(1)]
        public override bool GetShouldDye() => true;

        public override void OnArmorHit(Mobile attacker, Mobile defender, BaseWeapon weapon, BaseArmor armor,
            ref double damage)
        {
            if (attacker is BaseCreature creature && creature.CreatureType == Type)
                damage /= 2;
        }

        public override void OnMeleeHit(Mobile attacker, Mobile defender, BaseWeapon weapon, ref int damage)
        {
            if (defender is BaseCreature creature && creature.CreatureType == Type)
                damage *= 2;
        }
    }

    public class SlayerHitInfo : EnchantmentInfo
    {
        private static readonly SlayerHitInfo DefaultVariant = new();

        public override string Description { get; protected set; } = "Creature Slayer";
        public override EnchantNameType Place { get; protected set; } = EnchantNameType.Suffix;

        public override int Hue { get; protected set; } = 0;
        public override int CursedHue { get; protected set; } = 0;

        public static readonly IReadOnlyDictionary<CreatureType, SlayerHitInfo> Variants =
            new Dictionary<CreatureType, SlayerHitInfo>
            {
                [CreatureType.None] = DefaultVariant,
                [CreatureType.Slime] = DefaultVariant,
                [CreatureType.Ratkin] = DefaultVariant,
                [CreatureType.Plant] = DefaultVariant,
                [CreatureType.Beholder] = DefaultVariant,
                [CreatureType.Terathan] = DefaultVariant,
                [CreatureType.Ophidian] = DefaultVariant,
                [CreatureType.Animated] = DefaultVariant,
                [CreatureType.Gargoyle] = DefaultVariant,
                [CreatureType.Giantkin] = DefaultVariant,
                [CreatureType.Daemon] = DefaultVariant,
                [CreatureType.Animal] = new()
                {
                    Hue = 871,
                    CursedHue = 867
                },
                [CreatureType.Orc] = new()
                {
                    Hue = 845,
                    CursedHue = 842
                },
                [CreatureType.Troll] = new()
                {
                    Hue = 816,
                    CursedHue = 812
                },
                [CreatureType.Elemental] = new()
                {
                    Hue = 901,
                    CursedHue = 897
                },
                [CreatureType.Undead] = new()
                {
                    Hue = 856,
                    CursedHue = 852
                },
                [CreatureType.Dragonkin] = new()
                {
                    Hue = 641,
                    CursedHue = 631
                },
                [CreatureType.Human] = new()
                {
                    Hue = 1002,
                    CursedHue = 1020
                },
            };

        // When LINQ goes wrong...
        public override string[,] Names { get; protected set; } =
        {
            // These are in order of (int)CreatureType
            {string.Empty, string.Empty},
            {"Slime Slayer", "Slime Protector"},
            {"Ratkin Slayer", "Ratkin Protector"},
            {"Plant Slayer", "Plant Protector"},
            {"Animal Slayer", "Animal Protector"},
            {"Beholder Slayer", "Beholder Protector"},
            {"Orc Slayer", "Orc Protector"},
            {"Terathan Slayer", "Terathan Protector"},
            {"Ophidian Slayer", "Ophidian Protector"},
            {"Bewitched Slayer", "Bewitched Protector"},
            {"Gargoyle Slayer", "Gargoyle Protector"},
            {"Troll Slayer", "Troll Protector"},
            {"Giant Slayer", "Giant Protector"},
            {"Elemental Slayer", "Elemental Protector"},
            {"Silver", "Undead Protector"},
            {"Holy", "Unholy"},
            {"Dragon Slayer", "Dragon Protector"},
            {"Assassin's", "Peacekeeping"},
        };
    }
}