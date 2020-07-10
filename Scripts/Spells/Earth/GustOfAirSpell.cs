using System;
using System.Collections;
using Server.Engines.Magic;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Spells.Earth
{
    public class GustOfAirSpell : AbstractEarthSpell, IMobileTargeted
    {
        public override SpellInfo GetSpellInfo() => m_Info;
        
        private static SpellInfo m_Info = new SpellInfo(
                "Gust Of Air", "Gusto Do Ar",
                230,
                9022,
                typeof(BatWing),
                typeof(Brimstone),
                typeof(VialOfBlood));


        public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds(0); } }

        public override double RequiredSkill { get { return 100.0; } }
        public override int RequiredMana { get { return 15; } }

        public GustOfAirSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {
            Caster.Target = new MobileTarget(this, 10, TargetFlags.Harmful);
        }

        public void OnTargetFinished(Mobile from)
        {
            FinishSequence();
        }

        public void OnTarget(Mobile from, Mobile m)
        {
            if (!Caster.CanSee(m))
            {
                // Seems like this should be responsibility of the targetting system.  --daleron
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
                goto Return;
            }

            if (!CheckHSequence(m))
            {
                goto Return;
            }

            SpellHelper.Turn(Caster, m);

            // TODO: Push player in random direction

            // Do the effects
            Caster.MovingParticles(m, 0x379F, 7, 0, false, true, 3043, 4043, 0x211);
            Caster.PlaySound(0x20A);

            // 40 damage average at 130 skill
            double avg = 4 * Caster.Skills[DamageSkill].Value / 13;
            double damage = Utility.Dice(3, 6, (int)avg); //i.e. 3d6 + 30, clusters about 40, ranges 33 to 48

            if (Core.Debug)
            {
                Console.WriteLine("randomgaussian: {0} ", damage);
            }

            //m.Damage((int)damage, Caster, ElementalType.Air);
            SpellHelper.Damage(this, TimeSpan.Zero, m, Caster, damage, ElementalType.Air);

        Return:
            FinishSequence();
        }
    }
}
