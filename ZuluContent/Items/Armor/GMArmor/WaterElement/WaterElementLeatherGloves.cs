using ZuluContent.Zulu.Items;

namespace Server.Items
{
    [Serializable(0, false)]
    [Flipable]
    public partial class WaterElementLeatherGloves : BaseArmor, IGMItem
    {
        public override int InitMinHits => 70;

        public override int InitMaxHits => 70;

        public override int ArmorBase => 40;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Leather;
        
        public override string DefaultName => "Leather Gloves of the Water Element";

        [Constructible]
        public WaterElementLeatherGloves() : base(0x13C6)
        {
            Hue = 1167;
            Weight = 1.0;
        }
    }
}