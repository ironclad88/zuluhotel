

using System;
using System.Collections.Generic;
using Server;

using Server.Misc;
using Server.Items;
using static Server.Mobiles.CreatureProp;
using Server.Engines.Magic;
using Server.Engines.Harvest;

namespace Server.Mobiles
{
    public class UndeadFlayer : BaseCreature
    {
        static UndeadFlayer() => CreatureProperties.Register<UndeadFlayer>(new CreatureProperties
        {
            // cast_pct = 25,
            // CProp_HealingIncrease = i2,
            // CProp_NecroProtection = i8,
            // CProp_PermMagicImmunity = i8,
            // DataElementId = undeadflayer,
            // DataElementType = NpcTemplate,
            // dstart = 10,
            // Equip = undeadflayer,
            // Graphic = 0x0ec4 /* Weapon */,
            // Hitscript = :combat:manadrainscript /* Weapon */,
            // HitSound = 0x16D /* Weapon */,
            // hostile = 1,
            // lootgroup = 35,
            // MagicItemChance = 100,
            // MagicItemChance_0 = 65,
            // Magicitemlevel = 6,
            // MagicItemLevel_0 = 6,
            // MissSound = 0x239 /* Weapon */,
            // num_casts = 5,
            // script = spellkillpcs,
            // speech = 35,
            // Speed = 50 /* Weapon */,
            // spell = poison,
            // spell_0 = flamestrike,
            // spell_1 = ebolt,
            // spell_10 = abyssalflame,
            // spell_11 = gheal,
            // spell_2 = lightning,
            // spell_3 = explosion,
            // spell_4 = wraithbreath,
            // spell_5 = wyvernstrike,
            // spell_6 = plague,
            // spell_7 = decayingray,
            // spell_8 = darkness,
            // spell_9 = kill,
            // TrueColor = 1172,
            // virtue = 4,
            ActiveSpeed = 0.2,
            AiType = AIType.AI_Mage /* spellkillpcs */,
            AlwaysMurderer = true,
            Body = 0x18,
            CorpseNameOverride = "corpse of an Undead Flayer",
            CreatureType = CreatureType.Undead,
            DamageMax = 60,
            DamageMin = 10,
            Dex = 310,
            Female = false,
            FightMode = FightMode.Closest,
            FightRange = 1,
            Hides = 3,
            HideType = HideType.Liche,
            HitsMax = 1200,
            Hue = 1172,
            Int = 2750,
            ManaMaxSeed = 2750,
            Name = "an Undead Flayer",
            PassiveSpeed = 0.4,
            PerceptionRange = 10,
            PreferredSpells = new List<Type>
            {
                typeof(Spells.Third.PoisonSpell),
                typeof(Spells.Sixth.EnergyBoltSpell),
                typeof(Spells.Fourth.LightningSpell),
                typeof(Spells.Sixth.ExplosionSpell),
                typeof(Spells.Necromancy.WraithBreathSpell),
                typeof(Spells.Necromancy.WyvernStrikeSpell),
                typeof(Spells.Necromancy.PlagueSpell),
                typeof(Spells.Necromancy.DecayingRaySpell),
                typeof(Spells.Necromancy.DarknessSpell),
                typeof(Spells.Necromancy.WyvernStrikeSpell),
                typeof(Spells.Necromancy.AbyssalFlameSpell),
                typeof(Spells.Fourth.GreaterHealSpell),
            },
            Resistances = new Dictionary<ElementalType, CreatureProp>
            {
                { ElementalType.Poison, 100 },
            },
            SaySpellMantra = true,
            Skills = new Dictionary<SkillName, CreatureProp>
            {
                { SkillName.Magery, 175 },
                { SkillName.MagicResist, 150 },
                { SkillName.Tactics, 150 },
                { SkillName.Macing, 160 },
                { SkillName.DetectHidden, 200 },
            },
            StamMaxSeed = 50,
            Str = 1200,
            VirtualArmor = 45,
  
        });

        [Constructable]
        public UndeadFlayer() : base(CreatureProperties.Get<UndeadFlayer>())
        {
            // Add customization here

            AddItem(new SkinningKnife
            {
                Movable = false,
                Name = "Undead Flayer Weapon",
                Speed = 50,
                MaxHitPoints = 250,
                HitPoints = 250,
                HitSound = 0x16D,
                MissSound = 0x239,
            });
  
  
        }

        public UndeadFlayer(Serial serial) : base(serial) {}

  

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int) 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}