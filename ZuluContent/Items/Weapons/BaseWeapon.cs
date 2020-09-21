using System;
using Server.Network;
using Server.Mobiles;
using Server.Engines.Craft;
using System.Collections.Generic;
using Scripts.Engines.Magic;
using ZuluContent.Zulu.Engines.Magic;

namespace Server.Items
{
    public interface ISlayer
    {
        SlayerName Slayer { get; set; }
        SlayerName Slayer2 { get; set; }
    }

    public abstract class BaseWeapon : Item, IWeapon, ICraftable, ISlayer, IDurability, IMagicEquipItem
    {
        /* Weapon internals work differently now (Mar 13 2003)
         *
         * The attributes defined below default to -1.
         * If the value is -1, the corresponding virtual 'Aos/Old' property is used.
         * If not, the attribute value itself is used. Here's the list:
         *  - MinDamage
         *  - MaxDamage
         *  - Speed
         *  - HitSound
         *  - MissSound
         *  - StrRequirement, DexRequirement, IntRequirement
         *  - WeaponType
         *  - WeaponAnimation
         *  - MaxRange
         */

        #region Var declarations

        // Instance values. These values are unique to each weapon.
        private int m_Hits;

        // private SkillMod m_SkillMod, m_MageMod;
        private CraftResource m_Resource;

        // Overridable values. These values are provided to override the defaults which get defined in the individual weapon scripts.
        private int m_StrReq, m_DexReq, m_IntReq;
        private int m_MinDamage, m_MaxDamage;
        private int m_HitSound, m_MissSound;
        private float m_Speed;
        private int m_MaxRange;
        private SkillName m_Skill;
        private WeaponType m_Type;
        private WeaponAnimation m_Animation;

        #endregion

        #region Magical Properties
        private MagicalProperties m_MagicProps;

        public MagicalProperties MagicProps
        {
            get => m_MagicProps ??= new MagicalProperties(this);
        }

        #endregion

        #region Virtual Properties

        public virtual int DefaultHitSound => 0;
        public virtual int DefaultMissSound => 0;
        public virtual SkillName DefaultSkill => SkillName.Swords;
        public virtual WeaponType DefaultWeaponType => WeaponType.Slashing;
        public virtual WeaponAnimation DefaultAnimation => WeaponAnimation.Slash1H;
        public virtual int DefaultStrengthReq => 0;
        public virtual int DefaultDexterityReq => 0;
        public virtual int DefaultIntelligenceReq => 0;
        public virtual int DefaultMinDamage => 0;
        public virtual int DefaultMaxDamage => 0;
        public virtual int DefaultSpeed => 0;
        public virtual int DefaultMaxRange => 1;
        public bool CanFortify { get; } = true;

        public virtual int InitMinHits => 0;
        public virtual int InitMaxHits => 0;
        public virtual SkillName AccuracySkill => SkillName.Tactics;

        public virtual int VirtualDamageBonus => 0;

        #endregion

        #region Getters & Setters

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Identified
        {
            get => MagicProps.GetAttr<bool>(MagicProp.Identified);
            set => MagicProps.SetAttr(MagicProp.Identified, value);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HitPoints
        {
            get { return m_Hits; }
            set
            {
                if (m_Hits == value)
                    return;

                if (value > MaxHitPoints)
                    value = MaxHitPoints;

                m_Hits = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxHitPoints { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PoisonCharges { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Poison Poison { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponQuality Quality
        {
            get => MagicProps.GetAttr<WeaponQuality>();
            set
            {
                UnscaleDurability();
                MagicProps.SetAttr(value);
                ScaleDurability();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SlayerName Slayer { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SlayerName Slayer2 { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get { return m_Resource; }
            set
            {
                UnscaleDurability();
                m_Resource = value;
                Hue = CraftResources.GetHue(m_Resource);
                ScaleDurability();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponDamageLevel DamageLevel
        {
            get => MagicProps.GetAttr<WeaponDamageLevel>();
            set => MagicProps.SetAttr(value);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponDurabilityLevel DurabilityLevel
        {
            get => MagicProps.GetAttr<WeaponDurabilityLevel>();
            set
            {
                UnscaleDurability();
                MagicProps.SetAttr(value);
                ScaleDurability();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayerConstructed
        {
            get => MagicProps.GetAttr<bool>(MagicProp.PlayerConstructed);
            set => MagicProps.SetAttr(MagicProp.PlayerConstructed, value);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxRange
        {
            get { return m_MaxRange == -1 ? DefaultMaxRange : m_MaxRange; }
            set { m_MaxRange = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponAnimation Animation
        {
            get { return m_Animation == default ? DefaultAnimation : m_Animation; }
            set { m_Animation = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponType Type
        {
            get { return m_Type == (WeaponType) (-1) ? DefaultWeaponType : m_Type; }
            set { m_Type = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName Skill
        {
            get { return m_Skill == (SkillName) (-1) ? DefaultSkill : m_Skill; }
            set { m_Skill = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HitSound
        {
            get { return m_HitSound == -1 ? DefaultHitSound : m_HitSound; }
            set { m_HitSound = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MissSound
        {
            get { return m_MissSound == -1 ? DefaultMissSound : m_MissSound; }
            set { m_MissSound = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MinDamage
        {
            get { return m_MinDamage == -1 ? DefaultMinDamage : m_MinDamage; }
            set { m_MinDamage = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxDamage
        {
            get { return m_MaxDamage == -1 ? DefaultMaxDamage : m_MaxDamage; }
            set { m_MaxDamage = value; }
        }

        public virtual bool UseSkillMod
        {
            get { return true; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public float Speed
        {
            get
            {
                if (m_Speed > 0)
                    return m_Speed;

                return DefaultSpeed;
            }
            set { m_Speed = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int StrRequirement
        {
            get { return m_StrReq == -1 ? DefaultStrengthReq : m_StrReq; }
            set { m_StrReq = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DexRequirement
        {
            get { return m_DexReq == -1 ? DefaultDexterityReq : m_DexReq; }
            set { m_DexReq = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int IntRequirement
        {
            get { return m_IntReq == -1 ? DefaultIntelligenceReq : m_IntReq; }
            set { m_IntReq = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponAccuracyLevel AccuracyLevel
        {
            get => MagicProps.GetAttr<WeaponAccuracyLevel>();
            set
            {
                if (AccuracyLevel != value)
                {
                    MagicProps.SetAttr(value);

                    if (UseSkillMod)
                    {
                        if (AccuracyLevel == default && MagicProps.TryGetMod(Skill, out MagicSkillMod mod))
                            MagicProps.RemoveMod(mod);
                        else
                            MagicProps.AddMod(new MagicSkillMod(Skill, (int) value * 5));
                    }
                }
            }
        }

        #endregion

        public virtual void UnscaleDurability()
        {
            int scale = 100 + GetDurabilityBonus();

            m_Hits = (m_Hits * 100 + (scale - 1)) / scale;
            MaxHitPoints = (MaxHitPoints * 100 + (scale - 1)) / scale;
        }

        public virtual void ScaleDurability()
        {
            int scale = 100 + GetDurabilityBonus();

            m_Hits = (m_Hits * scale + 99) / 100;
            MaxHitPoints = (MaxHitPoints * scale + 99) / 100;
        }

        public int GetDurabilityBonus()
        {
            var bonus = DurabilityLevel switch
            {
                WeaponDurabilityLevel.Durable => 20,
                WeaponDurabilityLevel.Substantial => 50,
                WeaponDurabilityLevel.Massive => 70,
                WeaponDurabilityLevel.Fortified => 100,
                WeaponDurabilityLevel.Indestructible => 120,
                _ => 0
            };

            var exceptionalBonus = Quality == WeaponQuality.Exceptional ? 20 : 0;
            return bonus + exceptionalBonus;
        }

        private class ResetEquipTimer : Timer
        {
            private Mobile m_Mobile;

            public ResetEquipTimer(Mobile m, TimeSpan duration) : base(duration)
            {
                m_Mobile = m;
            }

            protected override void OnTick()
            {
                m_Mobile.EndAction(typeof(BaseWeapon));
            }
        }

        public override bool CheckConflictingLayer(Mobile m, Item item, Layer layer)
        {
            if (base.CheckConflictingLayer(m, item, layer))
                return true;

            switch (Layer)
            {
                case Layer.TwoHanded when layer == Layer.OneHanded:
                    m.SendLocalizedMessage(500214); // You already have something in both hands.
                    return true;
                case Layer.OneHanded
                    when layer == Layer.TwoHanded && !(item is BaseShield) && !(item is BaseEquipableLight):
                    m.SendLocalizedMessage(500215); // You can only wield one weapon at a time.
                    return true;
                default:
                    return false;
            }
        }

        public virtual Race RequiredRace
        {
            get { return null; }
        } //On OSI, there are no weapons with race requirements, this is for custom stuff

        public override bool CanEquip(Mobile from)
        {
            if (RequiredRace != null && from.Race != RequiredRace)
            {
                if (RequiredRace == Race.Elf)
                    from.SendLocalizedMessage(1072203); // Only Elves may use this.
                else
                    from.SendMessage("Only {0} may use this.", RequiredRace.PluralName);

                return false;
            }

            if (from.Dex < DexRequirement)
            {
                from.SendMessage("You are not nimble enough to equip that.");
                return false;
            }

            if (from.Str < StrRequirement)
            {
                from.SendLocalizedMessage(500213); // You are not strong enough to equip that.
                return false;
            }

            if (from.Int < IntRequirement)
            {
                from.SendMessage("You are not smart enough to equip that.");
                return false;
            }

            return from.CanBeginAction<BaseWeapon>() && base.CanEquip(from);
        }

        public override bool OnEquip(Mobile from)
        {
            from.NextCombatTime = Core.TickCount + (int) GetDelay(from).TotalMilliseconds;

            if (AccuracyLevel != WeaponAccuracyLevel.Regular)
            {
                MagicProps.AddMod(new MagicSkillMod(AccuracySkill, (int) AccuracyLevel * 5)
                {
                    Owner = from
                });
            }

            return true;
        }

        public override void OnAdded(IEntity parent)
        {
            base.OnAdded(parent);

            if (parent is Mobile from)
            {
                MagicProps.OnMobileEquip();
                from.CheckStatTimers();
                from.Delta(MobileDelta.WeaponDamage);
            }
        }

        public override void OnRemoved(IEntity parent)
        {
            if (parent is Mobile m)
            {
                if (m.Weapon is BaseWeapon weapon)
                    m.NextCombatTime = Core.TickCount + (int) weapon.GetDelay(m).TotalMilliseconds;
                
                MagicProps.OnMobileRemoved();
                m.CheckStatTimers();
                m.Delta(MobileDelta.WeaponDamage);
            }
        }

        public virtual SkillName GetUsedSkill(Mobile m, bool checkSkillAttrs)
        {
            SkillName sk = Skill;

            if (sk != SkillName.Wrestling && !m.Player && !m.Body.IsHuman &&
                m.Skills[SkillName.Wrestling].Value > m.Skills[sk].Value)
                sk = SkillName.Wrestling;

            return sk;
        }

        public virtual double GetAttackSkillValue(Mobile attacker, Mobile defender)
        {
            return attacker.Skills[GetUsedSkill(attacker, true)].Value;
        }

        public virtual double GetDefendSkillValue(Mobile attacker, Mobile defender)
        {
            return defender.Skills[GetUsedSkill(defender, true)].Value;
        }

        public virtual bool CheckHit(Mobile attacker, Mobile defender)
        {
            BaseWeapon atkWeapon = attacker.Weapon as BaseWeapon;
            BaseWeapon defWeapon = defender.Weapon as BaseWeapon;

            Skill atkSkill = attacker.Skills[atkWeapon.Skill];
            Skill defSkill = defender.Skills[defWeapon.Skill];

            double atkValue = atkWeapon.GetAttackSkillValue(attacker, defender);
            double defValue = defWeapon.GetDefendSkillValue(attacker, defender);

            double ourValue, theirValue;

            int bonus = GetHitChanceBonus();

            if (atkValue <= -50.0)
                atkValue = -49.9;

            if (defValue <= -50.0)
                defValue = -49.9;

            ourValue = atkValue + 50.0;
            theirValue = defValue + 50.0;

            double chance = ourValue / (theirValue * 2.0);

            chance *= 1.0 + (double) bonus / 100;

            return attacker.CheckSkill(atkSkill.SkillName, chance);
        }

        public virtual TimeSpan GetDelay(Mobile m)
        {
            var speed = Speed;

            if (speed <= 0)
                return TimeSpan.FromSeconds(10);

            var stamina = Math.Clamp(m.Stam, 0, m.Dex);
            double delayInSeconds = 15000.0 / ((stamina + 100) * speed);
            delayInSeconds = Math.Clamp(delayInSeconds, 1.25, 7.0);

            return TimeSpan.FromSeconds(delayInSeconds);
        }

        public virtual void OnBeforeSwing(Mobile attacker, Mobile defender)
        {
        }

        public virtual TimeSpan OnSwing(Mobile attacker, Mobile defender)
        {
            return OnSwing(attacker, defender, 1.0);
        }

        public virtual TimeSpan OnSwing(Mobile attacker, Mobile defender, double damageBonus)
        {
            if (attacker.HarmfulCheck(defender))
            {
                attacker.DisruptiveAction();

                if (attacker.NetState != null)
                    attacker.Send(new Swing(attacker.Serial, defender.Serial));


                if (CheckHit(attacker, defender))
                {
                    if (attacker is BaseCreature bc)
                    {
                        var ab = bc.GetWeaponAbility();

                        if (ab != null)
                            if (bc.WeaponAbilityChance >= Utility.RandomDouble())
                                ab.OnHit(attacker, defender, damageBonus);
                    }

                    OnHit(attacker, defender, damageBonus);
                }
                else
                {
                    OnMiss(attacker, defender);
                }
            }

            return GetDelay(attacker);
        }

        #region Sounds

        public virtual int GetHitAttackSound(Mobile attacker, Mobile defender)
        {
            int sound = attacker.GetAttackSound();

            if (sound == -1)
                sound = HitSound;

            return sound;
        }

        public virtual int GetHitDefendSound(Mobile attacker, Mobile defender)
        {
            return defender.GetHurtSound();
        }

        public virtual int GetMissAttackSound(Mobile attacker, Mobile defender)
        {
            if (attacker.GetAttackSound() == -1)
                return MissSound;
            else
                return -1;
        }

        public virtual int GetMissDefendSound(Mobile attacker, Mobile defender)
        {
            return -1;
        }

        #endregion

        public virtual int AbsorbDamage(Mobile attacker, Mobile defender, int damage)
        {
            if (defender.FindItemOnLayer(Layer.TwoHanded) is BaseShield shield)
                damage = shield.OnHit(this, damage);

            double chance = Utility.RandomDouble();

            Item armorItem;

            if (chance < 0.07)
                armorItem = defender.NeckArmor;
            else if (chance < 0.14)
                armorItem = defender.HandArmor;
            else if (chance < 0.28)
                armorItem = defender.ArmsArmor;
            else if (chance < 0.43)
                armorItem = defender.HeadArmor;
            else if (chance < 0.65)
                armorItem = defender.LegsArmor;
            else
                armorItem = defender.ChestArmor;

            if (armorItem is IWearableDurability armor)
                damage = armor.OnHit(this, damage);

            int virtualArmor = defender.VirtualArmor + defender.VirtualArmorMod;

            if (virtualArmor > 0)
            {
                double scalar;

                if (chance < 0.14)
                    scalar = 0.07;
                else if (chance < 0.28)
                    scalar = 0.14;
                else if (chance < 0.43)
                    scalar = 0.15;
                else if (chance < 0.65)
                    scalar = 0.22;
                else
                    scalar = 0.35;

                int from = (int) (virtualArmor * scalar) / 2;
                int to = (int) (virtualArmor * scalar);

                damage -= Utility.Random(from, to - from + 1);
            }

            return damage;
        }

        public void OnHit(Mobile attacker, Mobile defender)
        {
            OnHit(attacker, defender, 1.0);
        }

        public virtual void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            PlaySwingAnimation(attacker);
            PlayHurtAnimation(defender);

            attacker.PlaySound(GetHitAttackSound(attacker, defender));
            defender.PlaySound(GetHitDefendSound(attacker, defender));

            int damage = ComputeDamage(attacker, defender);

            damage = AbsorbDamage(attacker, defender, damage);

            if (damage < 1)
                damage = 1;

            AddBlood(attacker, defender, damage);

            defender.Damage(damage, attacker);

            if (MaxHitPoints > 0 && (MaxRange <= 1 && defender is Slime || Utility.Random(25) == 0)
            ) // Stratics says 50% chance, seems more like 4%..
            {
                if (MaxRange <= 1 && defender is Slime)
                    attacker.LocalOverheadMessage(MessageType.Regular, 0x3B2,
                        500263); // *Acid blood scars your weapon!*

                if (m_Hits > 0)
                {
                    --HitPoints;
                }
                else if (MaxHitPoints > 1)
                {
                    --MaxHitPoints;

                    if (Parent is Mobile)
                        ((Mobile) Parent).LocalOverheadMessage(MessageType.Regular, 0x3B2,
                            1061121); // Your equipment is severely damaged.
                }
                else
                {
                    Delete();
                }
            }

            if (attacker is BaseCreature attackingCreature)
                attackingCreature.OnGaveMeleeAttack(defender);

            if (defender is BaseCreature defendingCreature)
                defendingCreature.OnGotMeleeAttack(attacker);
        }

        public virtual CheckSlayerResult CheckSlayers(Mobile attacker, Mobile defender)
        {
            BaseWeapon atkWeapon = attacker.Weapon as BaseWeapon;
            SlayerEntry atkSlayer = SlayerGroup.GetEntryByName(atkWeapon.Slayer);
            SlayerEntry atkSlayer2 = SlayerGroup.GetEntryByName(atkWeapon.Slayer2);

            if (atkSlayer != null && atkSlayer.Slays(defender) || atkSlayer2 != null && atkSlayer2.Slays(defender))
                return CheckSlayerResult.Slayer;

            ISlayer defISlayer = Spellbook.FindEquippedSpellbook(defender);

            if (defISlayer == null)
                defISlayer = defender.Weapon as ISlayer;

            if (defISlayer != null)
            {
                SlayerEntry defSlayer = SlayerGroup.GetEntryByName(defISlayer.Slayer);
                SlayerEntry defSlayer2 = SlayerGroup.GetEntryByName(defISlayer.Slayer2);

                if (defSlayer != null && defSlayer.Group.OppositionSuperSlays(attacker) ||
                    defSlayer2 != null && defSlayer2.Group.OppositionSuperSlays(attacker))
                    return CheckSlayerResult.Opposition;
            }

            return CheckSlayerResult.None;
        }

        public virtual void AddBlood(Mobile attacker, Mobile defender, int damage)
        {
            if (damage > 0)
            {
                new Blood().MoveToWorld(defender.Location, defender.Map);

                int extraBlood = Utility.RandomMinMax(0, 1);

                for (int i = 0; i < extraBlood; i++)
                {
                    new Blood().MoveToWorld(new Point3D(
                        defender.X + Utility.RandomMinMax(-1, 1),
                        defender.Y + Utility.RandomMinMax(-1, 1),
                        defender.Z), defender.Map);
                }
            }
        }

        public virtual void OnMiss(Mobile attacker, Mobile defender)
        {
            PlaySwingAnimation(attacker);
            attacker.PlaySound(GetMissAttackSound(attacker, defender));
            defender.PlaySound(GetMissDefendSound(attacker, defender));
        }

        public virtual void GetBaseDamageRange(Mobile attacker, out int min, out int max)
        {
            if (attacker is BaseCreature)
            {
                BaseCreature c = (BaseCreature) attacker;

                if (c.DamageMin >= 0)
                {
                    min = c.DamageMin;
                    max = c.DamageMax;
                    return;
                }

                if (this is Fists && !attacker.Body.IsHuman)
                {
                    min = attacker.Str / 28;
                    max = attacker.Str / 28;
                    return;
                }
            }

            min = MinDamage;
            max = MaxDamage;
        }

        public virtual double GetBaseDamage(Mobile attacker)
        {
            int min, max;

            GetBaseDamageRange(attacker, out min, out max);

            int damage = Utility.RandomMinMax(min, max);

            /* Apply damage level offset
             * : Regular : 0
             * : Ruin    : 1
             * : Might   : 3
             * : Force   : 5
             * : Power   : 7
             * : Vanq    : 9
             */
            if (DamageLevel != WeaponDamageLevel.Regular)
                damage += 2 * (int) DamageLevel - 1;

            return damage;
        }

        public virtual double GetBonus(double value, double scalar, double threshold, double offset)
        {
            double bonus = value * scalar;

            if (value >= threshold)
                bonus += offset;

            return bonus / 100;
        }

        public virtual int GetHitChanceBonus()
        {
            return 0;
        }

        public virtual int GetDamageBonus()
        {
            var qualityBonus = Quality switch
            {
                WeaponQuality.Low => -20,
                WeaponQuality.Exceptional => 20,
                _ => 0
            };

            var damageBonus = DamageLevel switch
            {
                WeaponDamageLevel.Ruin => 15,
                WeaponDamageLevel.Might => 20,
                WeaponDamageLevel.Force => 25,
                WeaponDamageLevel.Power => 30,
                WeaponDamageLevel.Vanquishing => 35,
                WeaponDamageLevel.Devastation => 40,
                _ => 0
            };

            return VirtualDamageBonus + qualityBonus + damageBonus;
        }

        public virtual void GetStatusDamage(Mobile from, out int min, out int max)
        {
            int baseMin, baseMax;

            GetBaseDamageRange(from, out baseMin, out baseMax);

            min = Math.Max((int) ScaleDamageOld(from, baseMin, false), 1);
            max = Math.Max((int) ScaleDamageOld(from, baseMax, false), 1);
        }

        public virtual double ScaleDamageOld(Mobile attacker, double damage, bool checkSkills)
        {
            if (checkSkills)
            {
                attacker.CheckSkill(SkillName.Tactics, 0.0,
                    attacker.Skills[SkillName.Tactics].Cap); // Passively check tactics for gain
                attacker.CheckSkill(SkillName.Anatomy, 0.0,
                    attacker.Skills[SkillName.Anatomy].Cap); // Passively check Anatomy for gain

                if (Type == WeaponType.Axe)
                    attacker.CheckSkill(SkillName.Lumberjacking, 0.0, 100.0); // Passively check Lumberjacking for gain
            }

            /* Compute tactics modifier
             * :   0.0 = 50% loss
             * :  50.0 = unchanged
             * : 100.0 = 50% bonus
             */
            damage += damage * ((attacker.Skills[SkillName.Tactics].Value - 50.0) / 100.0);


            /* Compute strength modifier
             * : 1% bonus for every 5 strength
             */
            double modifiers = attacker.Str / 5.0 / 100.0;

            /* Compute anatomy modifier
             * : 1% bonus for every 5 points of anatomy
             * : +10% bonus at Grandmaster or higher
             */
            double anatomyValue = attacker.Skills[SkillName.Anatomy].Value;
            modifiers += anatomyValue / 5.0 / 100.0;

            if (anatomyValue >= 100.0)
                modifiers += 0.1;

            /* Compute lumberjacking bonus
             * : 1% bonus for every 5 points of lumberjacking
             * : +10% bonus at Grandmaster or higher
             */
            if (Type == WeaponType.Axe)
            {
                double lumberValue = attacker.Skills[SkillName.Lumberjacking].Value;

                modifiers += lumberValue / 5.0 / 100.0;

                if (lumberValue >= 100.0)
                    modifiers += 0.1;
            }

            // New quality bonus:
            if (Quality != WeaponQuality.Regular)
                modifiers += ((int) Quality - 1) * 0.2;

            // Virtual damage bonus:
            if (VirtualDamageBonus != 0)
                modifiers += VirtualDamageBonus / 100.0;

            // Apply bonuses
            damage += damage * modifiers;

            return (int) damage;
        }

        public virtual int ComputeDamage(Mobile attacker, Mobile defender)
        {
            int damage = (int) ScaleDamageOld(attacker, GetBaseDamage(attacker), true);

            // pre-AOS, halve damage if the defender is a player or the attacker is not a player
            if (defender is PlayerMobile || !(attacker is PlayerMobile))
                damage = (int) (damage / 2.0);

            return damage;
        }

        public virtual void PlayHurtAnimation(Mobile from)
        {
            int action;
            int frames;

            switch (from.Body.Type)
            {
                case BodyType.Sea:
                case BodyType.Animal:
                {
                    action = 7;
                    frames = 5;
                    break;
                }
                case BodyType.Monster:
                {
                    action = 10;
                    frames = 4;
                    break;
                }
                case BodyType.Human:
                {
                    action = 20;
                    frames = 5;
                    break;
                }
                default: return;
            }

            if (from.Mounted)
                return;

            from.Animate(action, frames, 1, true, false, 0);
        }

        public virtual void PlaySwingAnimation(Mobile from)
        {
            int action;

            switch (from.Body.Type)
            {
                case BodyType.Sea:
                case BodyType.Animal:
                {
                    action = Utility.Random(5, 2);
                    break;
                }
                case BodyType.Monster:
                {
                    switch (Animation)
                    {
                        default:
                        case WeaponAnimation.Wrestle:
                        case WeaponAnimation.Bash1H:
                        case WeaponAnimation.Pierce1H:
                        case WeaponAnimation.Slash1H:
                        case WeaponAnimation.Bash2H:
                        case WeaponAnimation.Pierce2H:
                        case WeaponAnimation.Slash2H:
                            action = Utility.Random(4, 3);
                            break;
                        case WeaponAnimation.ShootBow: return; // 7
                        case WeaponAnimation.ShootXBow: return; // 8
                    }

                    break;
                }
                case BodyType.Human:
                {
                    if (!from.Mounted)
                    {
                        action = (int) Animation;
                    }
                    else
                    {
                        switch (Animation)
                        {
                            default:
                            case WeaponAnimation.Wrestle:
                            case WeaponAnimation.Bash1H:
                            case WeaponAnimation.Pierce1H:
                            case WeaponAnimation.Slash1H:
                                action = 26;
                                break;
                            case WeaponAnimation.Bash2H:
                            case WeaponAnimation.Pierce2H:
                            case WeaponAnimation.Slash2H:
                                action = 29;
                                break;
                            case WeaponAnimation.ShootBow:
                                action = 27;
                                break;
                            case WeaponAnimation.ShootXBow:
                                action = 28;
                                break;
                        }
                    }

                    break;
                }
                default: return;
            }

            from.Animate(action, 7, 1, true, false, 0);
        }

        #region Serialization/Deserialization

        private static bool SetSaveFlag(ref SaveFlag flags, SaveFlag toSet, bool setIf)
        {
            if (setIf)
                flags |= toSet;

            return setIf;
        }

        private static bool GetSaveFlag(SaveFlag flags, SaveFlag toGet)
        {
            return (flags & toGet) != 0;
        }

        public override void Serialize(IGenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int) 10); // version

            SaveFlag flags = SaveFlag.None;
            
            SetSaveFlag(ref flags, SaveFlag.NewMagicalProperties, true);

            // Legacy property saving
            if (!GetSaveFlag(flags, SaveFlag.NewMagicalProperties))
            {
                SetSaveFlag(ref flags, SaveFlag.DamageLevel, DamageLevel != WeaponDamageLevel.Regular);
                SetSaveFlag(ref flags, SaveFlag.AccuracyLevel, AccuracyLevel != WeaponAccuracyLevel.Regular);
                SetSaveFlag(ref flags, SaveFlag.DurabilityLevel, DurabilityLevel != WeaponDurabilityLevel.Regular);
                SetSaveFlag(ref flags, SaveFlag.Quality, Quality != WeaponQuality.Regular);
                SetSaveFlag(ref flags, SaveFlag.Identified, Identified);
                SetSaveFlag(ref flags, SaveFlag.PlayerConstructed, PlayerConstructed);
            }

            SetSaveFlag(ref flags, SaveFlag.Hits, m_Hits != 0);
            SetSaveFlag(ref flags, SaveFlag.MaxHits, MaxHitPoints != 0);
            SetSaveFlag(ref flags, SaveFlag.Slayer, Slayer != SlayerName.None);
            SetSaveFlag(ref flags, SaveFlag.Poison, Poison != null);
            SetSaveFlag(ref flags, SaveFlag.PoisonCharges, PoisonCharges != 0);
            SetSaveFlag(ref flags, SaveFlag.Crafter, Crafter != null);
            SetSaveFlag(ref flags, SaveFlag.StrReq, m_StrReq != -1);
            SetSaveFlag(ref flags, SaveFlag.DexReq, m_DexReq != -1);
            SetSaveFlag(ref flags, SaveFlag.IntReq, m_IntReq != -1);
            SetSaveFlag(ref flags, SaveFlag.MinDamage, m_MinDamage != -1);
            SetSaveFlag(ref flags, SaveFlag.MaxDamage, m_MaxDamage != -1);
            SetSaveFlag(ref flags, SaveFlag.HitSound, m_HitSound != -1);
            SetSaveFlag(ref flags, SaveFlag.MissSound, m_MissSound != -1);
            SetSaveFlag(ref flags, SaveFlag.Speed, m_Speed > -1);
            SetSaveFlag(ref flags, SaveFlag.MaxRange, m_MaxRange != -1);
            SetSaveFlag(ref flags, SaveFlag.Skill, m_Skill != (SkillName) (-1));
            SetSaveFlag(ref flags, SaveFlag.Type, m_Type != (WeaponType) (-1));
            SetSaveFlag(ref flags, SaveFlag.Animation, m_Animation != (WeaponAnimation) (-1));
            SetSaveFlag(ref flags, SaveFlag.Resource, m_Resource != CraftResource.Iron);
            SetSaveFlag(ref flags, SaveFlag.Slayer2, Slayer2 != SlayerName.None);
            
            writer.Write((int) flags);
            
            if (GetSaveFlag(flags, SaveFlag.NewMagicalProperties))
                MagicProps.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.DamageLevel))
                writer.Write((int)DamageLevel);

            if (GetSaveFlag(flags, SaveFlag.AccuracyLevel))
                writer.Write((int)AccuracyLevel);

            if (GetSaveFlag(flags, SaveFlag.DurabilityLevel))
                writer.Write((int)DurabilityLevel);

            if (GetSaveFlag(flags, SaveFlag.Quality))
                writer.Write((int)Quality);

            if (GetSaveFlag(flags, SaveFlag.Hits))
                writer.Write((int) m_Hits);

            if (GetSaveFlag(flags, SaveFlag.MaxHits))
                writer.Write((int) MaxHitPoints);

            if (GetSaveFlag(flags, SaveFlag.Slayer))
                writer.Write((int) Slayer);

            if (GetSaveFlag(flags, SaveFlag.Poison))
                Poison.Serialize(Poison, writer);

            if (GetSaveFlag(flags, SaveFlag.PoisonCharges))
                writer.Write((int) PoisonCharges);

            if (GetSaveFlag(flags, SaveFlag.Crafter))
                writer.Write((Mobile) Crafter);
            
            if (GetSaveFlag(flags, SaveFlag.Identified))
                writer.Write((bool)Identified);

            if (GetSaveFlag(flags, SaveFlag.StrReq))
                writer.Write((int) m_StrReq);

            if (GetSaveFlag(flags, SaveFlag.DexReq))
                writer.Write((int) m_DexReq);

            if (GetSaveFlag(flags, SaveFlag.IntReq))
                writer.Write((int) m_IntReq);

            if (GetSaveFlag(flags, SaveFlag.MinDamage))
                writer.Write((int) m_MinDamage);

            if (GetSaveFlag(flags, SaveFlag.MaxDamage))
                writer.Write((int) m_MaxDamage);

            if (GetSaveFlag(flags, SaveFlag.HitSound))
                writer.Write((int) m_HitSound);

            if (GetSaveFlag(flags, SaveFlag.MissSound))
                writer.Write((int) m_MissSound);

            if (GetSaveFlag(flags, SaveFlag.Speed))
                writer.Write((float) m_Speed);

            if (GetSaveFlag(flags, SaveFlag.MaxRange))
                writer.Write((int) m_MaxRange);

            if (GetSaveFlag(flags, SaveFlag.Skill))
                writer.Write((int) m_Skill);

            if (GetSaveFlag(flags, SaveFlag.Type))
                writer.Write((int) m_Type);

            if (GetSaveFlag(flags, SaveFlag.Animation))
                writer.Write((int) m_Animation);

            if (GetSaveFlag(flags, SaveFlag.Resource))
                writer.Write((int) m_Resource);
            
            if (GetSaveFlag(flags, SaveFlag.PlayerConstructed))
                writer.Write((bool)PlayerConstructed);

            if (GetSaveFlag(flags, SaveFlag.Slayer2))
                writer.Write((int) Slayer2);
        }

        [Flags]
        private enum SaveFlag
        {
            None = 0x00000000,
            DamageLevel = 0x00000001,
            AccuracyLevel = 0x00000002,
            DurabilityLevel = 0x00000004,
            Quality = 0x00000008,
            Hits = 0x00000010,
            MaxHits = 0x00000020,
            Slayer = 0x00000040,
            Poison = 0x00000080,
            PoisonCharges = 0x00000100,
            Crafter = 0x00000200,
            Identified = 0x00000400,
            StrReq = 0x00000800,
            DexReq = 0x00001000,
            IntReq = 0x00002000,
            MinDamage = 0x00004000,
            MaxDamage = 0x00008000,
            HitSound = 0x00010000,
            MissSound = 0x00020000,
            Speed = 0x00040000,
            MaxRange = 0x00080000,
            Skill = 0x00100000,
            Type = 0x00200000,
            Animation = 0x00400000,
            Resource = 0x00800000,
            xAttributes = 0x01000000,
            xWeaponAttributes = 0x02000000,
            PlayerConstructed = 0x04000000,
            SkillBonuses = 0x08000000,
            Slayer2 = 0x10000000,
            ElementalDamages = 0x20000000,
            NewMagicalProperties = 0x40000000
        }

        public override void Deserialize(IGenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            
            SaveFlag flags = (SaveFlag) reader.ReadInt();

            switch (version)
            {
                case 10:
                    if (GetSaveFlag(flags, SaveFlag.NewMagicalProperties))
                        m_MagicProps = MagicalProperties.Deserialize(reader, this);
                    goto case 9;
                case 9:
                case 8:
                case 7:
                case 6:
                case 5:
                {
                    if (GetSaveFlag(flags, SaveFlag.DamageLevel))
                    {
                        DamageLevel = (WeaponDamageLevel) reader.ReadInt();

                        if (DamageLevel > WeaponDamageLevel.Vanquishing)
                            DamageLevel = WeaponDamageLevel.Ruin;
                    }

                    if (GetSaveFlag(flags, SaveFlag.AccuracyLevel))
                    {
                        AccuracyLevel = (WeaponAccuracyLevel) reader.ReadInt();

                        if (AccuracyLevel > WeaponAccuracyLevel.Supremely)
                            AccuracyLevel = WeaponAccuracyLevel.Accurate;
                    }

                    if (GetSaveFlag(flags, SaveFlag.DurabilityLevel))
                    {
                        DurabilityLevel = (WeaponDurabilityLevel) reader.ReadInt();

                        if (DurabilityLevel > WeaponDurabilityLevel.Indestructible)
                            DurabilityLevel = WeaponDurabilityLevel.Durable;
                    }

                    if (GetSaveFlag(flags, SaveFlag.Quality))
                        Quality = (WeaponQuality) reader.ReadInt();
                    else
                        Quality = WeaponQuality.Regular;

                    if (GetSaveFlag(flags, SaveFlag.Hits))
                        m_Hits = reader.ReadInt();

                    if (GetSaveFlag(flags, SaveFlag.MaxHits))
                        MaxHitPoints = reader.ReadInt();

                    if (GetSaveFlag(flags, SaveFlag.Slayer))
                        Slayer = (SlayerName) reader.ReadInt();

                    if (GetSaveFlag(flags, SaveFlag.Poison))
                        Poison = Poison.Deserialize(reader);

                    if (GetSaveFlag(flags, SaveFlag.PoisonCharges))
                        PoisonCharges = reader.ReadInt();

                    if (GetSaveFlag(flags, SaveFlag.Crafter))
                        Crafter = reader.ReadMobile();

                    if (GetSaveFlag(flags, SaveFlag.Identified))
                        Identified = version >= 6 || reader.ReadBool();

                    if (GetSaveFlag(flags, SaveFlag.StrReq))
                        m_StrReq = reader.ReadInt();
                    else
                        m_StrReq = -1;

                    if (GetSaveFlag(flags, SaveFlag.DexReq))
                        m_DexReq = reader.ReadInt();
                    else
                        m_DexReq = -1;

                    if (GetSaveFlag(flags, SaveFlag.IntReq))
                        m_IntReq = reader.ReadInt();
                    else
                        m_IntReq = -1;

                    if (GetSaveFlag(flags, SaveFlag.MinDamage))
                        m_MinDamage = reader.ReadInt();
                    else
                        m_MinDamage = -1;

                    if (GetSaveFlag(flags, SaveFlag.MaxDamage))
                        m_MaxDamage = reader.ReadInt();
                    else
                        m_MaxDamage = -1;

                    if (GetSaveFlag(flags, SaveFlag.HitSound))
                        m_HitSound = reader.ReadInt();
                    else
                        m_HitSound = -1;

                    if (GetSaveFlag(flags, SaveFlag.MissSound))
                        m_MissSound = reader.ReadInt();
                    else
                        m_MissSound = -1;

                    if (GetSaveFlag(flags, SaveFlag.Speed))
                    {
                        if (version < 9)
                            m_Speed = reader.ReadInt();
                        else
                            m_Speed = reader.ReadFloat();
                    }
                    else
                        m_Speed = -1;

                    if (GetSaveFlag(flags, SaveFlag.MaxRange))
                        m_MaxRange = reader.ReadInt();
                    else
                        m_MaxRange = -1;

                    if (GetSaveFlag(flags, SaveFlag.Skill))
                        m_Skill = (SkillName) reader.ReadInt();
                    else
                        m_Skill = (SkillName) (-1);

                    if (GetSaveFlag(flags, SaveFlag.Type))
                        m_Type = (WeaponType) reader.ReadInt();
                    else
                        m_Type = (WeaponType) (-1);

                    if (GetSaveFlag(flags, SaveFlag.Animation))
                        m_Animation = (WeaponAnimation) reader.ReadInt();
                    else
                        m_Animation = (WeaponAnimation) (-1);

                    if (GetSaveFlag(flags, SaveFlag.Resource))
                        m_Resource = (CraftResource) reader.ReadInt();
                    else
                        m_Resource = CraftResource.Iron;
                    
                    if (GetSaveFlag(flags, SaveFlag.PlayerConstructed))
                        PlayerConstructed = true;

                    if (GetSaveFlag(flags, SaveFlag.Slayer2))
                        Slayer2 = (SlayerName) reader.ReadInt();

                    break;
                }
                case 4:
                {
                    Slayer = (SlayerName) reader.ReadInt();

                    goto case 3;
                }
                case 3:
                {
                    m_StrReq = reader.ReadInt();
                    m_DexReq = reader.ReadInt();
                    m_IntReq = reader.ReadInt();

                    goto case 2;
                }
                case 2:
                {
                    Identified = reader.ReadBool();

                    goto case 1;
                }
                case 1:
                {
                    m_MaxRange = reader.ReadInt();

                    goto case 0;
                }
                case 0:
                {
                    if (version == 0)
                        m_MaxRange = 1; // default

                    if (version < 5)
                    {
                        m_Resource = CraftResource.Iron;
                    }

                    m_MinDamage = reader.ReadInt();
                    m_MaxDamage = reader.ReadInt();

                    m_Speed = reader.ReadInt();

                    m_HitSound = reader.ReadInt();
                    m_MissSound = reader.ReadInt();

                    m_Skill = (SkillName) reader.ReadInt();
                    m_Type = (WeaponType) reader.ReadInt();
                    m_Animation = (WeaponAnimation) reader.ReadInt();
                    DamageLevel = (WeaponDamageLevel) reader.ReadInt();
                    AccuracyLevel = (WeaponAccuracyLevel) reader.ReadInt();
                    DurabilityLevel = (WeaponDurabilityLevel) reader.ReadInt();
                    Quality = (WeaponQuality) reader.ReadInt();

                    Crafter = reader.ReadMobile();

                    Poison = Poison.Deserialize(reader);
                    PoisonCharges = reader.ReadInt();

                    if (m_StrReq == DefaultStrengthReq)
                        m_StrReq = -1;

                    if (m_DexReq == DefaultDexterityReq)
                        m_DexReq = -1;

                    if (m_IntReq == DefaultIntelligenceReq)
                        m_IntReq = -1;

                    if (m_MinDamage == DefaultMinDamage)
                        m_MinDamage = -1;

                    if (m_MaxDamage == DefaultMaxDamage)
                        m_MaxDamage = -1;

                    if (m_HitSound == DefaultHitSound)
                        m_HitSound = -1;

                    if (m_MissSound == DefaultMissSound)
                        m_MissSound = -1;

                    if (Math.Abs(m_Speed - DefaultSpeed) < 0.1)
                        m_Speed = -1;

                    if (m_MaxRange == DefaultMaxRange)
                        m_MaxRange = -1;

                    if (m_Skill == DefaultSkill)
                        m_Skill = (SkillName) (-1);

                    if (m_Type == DefaultWeaponType)
                        m_Type = (WeaponType) (-1);

                    if (m_Animation == DefaultAnimation)
                        m_Animation = (WeaponAnimation) (-1);

                    break;
                }
            }

            if (Parent is Mobile)
                ((Mobile) Parent).CheckStatTimers();

            if (m_Hits <= 0 && MaxHitPoints <= 0)
            {
                m_Hits = MaxHitPoints = Utility.RandomMinMax(InitMinHits, InitMaxHits);
            }

            if (version < 6)
                PlayerConstructed = true; // we don't know, so, assume it's crafted
        }

        #endregion

        public BaseWeapon(int itemID) : base(itemID)
        {
            Layer = (Layer) ItemData.Quality;

            Quality = WeaponQuality.Regular;
            m_StrReq = -1;
            m_DexReq = -1;
            m_IntReq = -1;
            m_MinDamage = -1;
            m_MaxDamage = -1;
            m_HitSound = -1;
            m_MissSound = -1;
            m_Speed = -1;
            m_MaxRange = -1;
            m_Skill = (SkillName) (-1);
            m_Type = (WeaponType) (-1);
            m_Animation = (WeaponAnimation) (-1);

            m_Hits = MaxHitPoints = Utility.RandomMinMax(InitMinHits, InitMaxHits);

            m_Resource = CraftResource.Iron;
        }

        public BaseWeapon(Serial serial) : base(serial)
        {
        }

        private string GetNameString()
        {
            string name = Name;

            if (name == null)
                name = $"#{LabelNumber}";

            return name;
        }

        [Hue, CommandProperty(AccessLevel.GameMaster)]
        public override int Hue
        {
            get { return base.Hue; }
            set { base.Hue = value; }
        }

        public override bool AllowEquippedCast(Mobile from)
        {
            return base.AllowEquippedCast(@from);
        }

        public override void OnSingleClick(Mobile from)
        {
            List<EquipInfoAttribute> attrs = new List<EquipInfoAttribute>();

            if (DisplayLootType)
            {
                if (LootType == LootType.Blessed)
                    attrs.Add(new EquipInfoAttribute(1038021)); // blessed
                else if (LootType == LootType.Cursed)
                    attrs.Add(new EquipInfoAttribute(1049643)); // cursed
            }

            if (Quality == WeaponQuality.Exceptional)
                attrs.Add(new EquipInfoAttribute(1018305 - (int) Quality));

            if (Identified || from.AccessLevel >= AccessLevel.GameMaster)
            {
                if (Slayer != SlayerName.None)
                {
                    SlayerEntry entry = SlayerGroup.GetEntryByName(Slayer);
                    if (entry != null)
                        attrs.Add(new EquipInfoAttribute(entry.Title));
                }

                if (Slayer2 != SlayerName.None)
                {
                    SlayerEntry entry = SlayerGroup.GetEntryByName(Slayer2);
                    if (entry != null)
                        attrs.Add(new EquipInfoAttribute(entry.Title));
                }

                if (DurabilityLevel != WeaponDurabilityLevel.Regular)
                    attrs.Add(new EquipInfoAttribute(1038000 + (int) DurabilityLevel));

                if (DamageLevel != WeaponDamageLevel.Regular)
                    attrs.Add(new EquipInfoAttribute(1038015 + (int) DamageLevel));

                if (AccuracyLevel != WeaponAccuracyLevel.Regular)
                    attrs.Add(new EquipInfoAttribute(1038010 + (int) AccuracyLevel));
            }
            else if (Slayer != SlayerName.None || Slayer2 != SlayerName.None ||
                     DurabilityLevel != WeaponDurabilityLevel.Regular || DamageLevel != WeaponDamageLevel.Regular ||
                     AccuracyLevel != WeaponAccuracyLevel.Regular)
                attrs.Add(new EquipInfoAttribute(1038000)); // Unidentified

            if (Poison != null && PoisonCharges > 0)
                attrs.Add(new EquipInfoAttribute(1017383, PoisonCharges));

            int number;

            if (Name == null)
            {
                number = LabelNumber;
            }
            else
            {
                LabelTo(from, Name);
                number = 1041000;
            }

            if (attrs.Count == 0 && Crafter == null && Name != null)
                return;

            EquipmentInfo eqInfo = new EquipmentInfo(number, Crafter, false, attrs.ToArray());

            from.Send(new DisplayEquipmentInfo(this, eqInfo));
        }

        public static BaseWeapon Fists { get; set; }

        #region ICraftable Members

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes,
            BaseTool tool, CraftItem craftItem, int resHue)
        {
            Quality = (WeaponQuality) quality;

            if (makersMark)
                Crafter = from;

            PlayerConstructed = true;

            return quality;
        }

        #endregion
    }

    public enum CheckSlayerResult
    {
        None,
        Slayer,
        Opposition
    }
}