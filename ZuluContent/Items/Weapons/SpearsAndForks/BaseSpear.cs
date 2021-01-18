using System;

namespace Server.Items
{
    public abstract class BaseSpear : BaseMeleeWeapon
    {
        public override int DefaultHitSound => 0x23C;

        public override int DefaultMissSound => 0x238;

        public override SkillName DefaultSkill => SkillName.Fencing;

        public override WeaponType DefaultWeaponType => WeaponType.Piercing;

        public override WeaponAnimation DefaultAnimation => WeaponAnimation.Pierce2H;

        public BaseSpear(int itemId) : base(itemId)
        {
        }

        public BaseSpear(Serial serial) : base(serial)
        {
        }

        public override void Serialize(IGenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int) 0); // version
        }

        public override void Deserialize(IGenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            base.OnHit(attacker, defender, damageBonus);

            if (Layer == Layer.TwoHanded && attacker.Skills[SkillName.Anatomy].Value / 400.0 >= Utility.RandomDouble())
            {
                defender.SendMessage("You receive a paralyzing blow!"); // Is this not localized?
                defender.Freeze(TimeSpan.FromSeconds(2.0));

                attacker.SendMessage("You deliver a paralyzing blow!"); // Is this not localized?
                attacker.PlaySound(0x11C);
            }
        }
    }
}