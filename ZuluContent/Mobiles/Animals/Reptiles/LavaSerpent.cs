using Server.Items;
using Server.Scripts.Engines.Loot;

namespace Server.Mobiles
{
    [CorpseName( "a lava serpent corpse" )]
	[TypeAlias( "Server.Mobiles.Lavaserpant" )]
	public class LavaSerpent : BaseCreature
	{

		[Constructible]
public LavaSerpent() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a lava serpent";
			Body = 90;
			BaseSoundID = 219;

			SetStr( 386, 415 );
			SetDex( 56, 80 );
			SetInt( 66, 85 );

			SetHits( 232, 249 );
			SetMana( 0 );

			SetDamage( 10, 22 );

			SetSkill( SkillName.MagicResist, 25.3, 70.0 );
			SetSkill( SkillName.Tactics, 65.1, 70.0 );
			SetSkill( SkillName.Wrestling, 60.1, 80.0 );

			Fame = 4500;
			Karma = -4500;

			VirtualArmor = 40;

			PackItem( new SulfurousAsh( 3 ) );
			PackItem( new Bone() );
			// TODO: body parts, armour
		}

		public override void GenerateLoot()
		{
			AddLoot(LootTable.Table1);
		}

		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override int Meat{ get{ return 4; } }
		public override int Hides{ get{ return 15; } }
		public override HideType HideType{ get{ return HideType.Regular; } }

		[Constructible]
public LavaSerpent(Serial serial) : base(serial)
		{
		}

		public override void Serialize(IGenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(IGenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			if ( BaseSoundID == -1 )
				BaseSoundID = 219;
		}
	}
}
