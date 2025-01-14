using ZuluContent.Zulu.Items;

namespace Server.Items
{
    [Serializable(0, false)]
    public partial class DarknessShield : BaseShield, IGMItem
    {
        public override int InitMinHits => 100;

        public override int InitMaxHits => 100;

        public override int ArmorBase => 45;

        public override int DefaultStrReq => 70;
        
        public override bool AllowEquippedCast(Mobile from) => true;

        public override string DefaultName => "Shield of Darkness";

        [Constructible]
        public DarknessShield() : base(0x1B7B)
        {
            Hue = 1157;
            Identified = false;
            Weight = 6.0;
        }
    }
}