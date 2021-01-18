using System;

namespace Server.Items
{
    [FlipableAttribute(0x13B2, 0x13B1)]
    public class Bow : BaseRanged
    {
        public override int EffectId { get; set; } = 0xF42;

        public override Type AmmoType => typeof(Arrow);

        public override Item Ammo => new Arrow();

        public override int DefaultStrengthReq => 20;

        public override int DefaultMinDamage => 9;

        public override int DefaultMaxDamage => 41;

        public override int DefaultSpeed => 20;

        public override int DefaultMaxRange => 10;

        public override int InitMinHits => 31;

        public override int InitMaxHits => 60;

        public override WeaponAnimation DefaultAnimation => WeaponAnimation.ShootBow;


        [Constructible]
        public Bow() : base(0x13B2)
        {
            Weight = 6.0;
            Layer = Layer.TwoHanded;
        }

        [Constructible]
        public Bow(Serial serial) : base(serial)
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

            if (Math.Abs(Weight - 7.0) < 0.1)
                Weight = 6.0;
        }
    }
}