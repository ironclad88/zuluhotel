namespace Server.Items
{
    [FlipableAttribute( 0xE89, 0xE8a )]
	public class QuarterStaff : BaseStaff
	{
		public override int DefaultStrengthReq{ get{ return 30; } }
		public override int DefaultMinDamage{ get{ return 8; } }
		public override int DefaultMaxDamage{ get{ return 28; } }
		public override int DefaultSpeed{ get{ return 48; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 60; } }

		[Constructable]
		public QuarterStaff() : base( 0xE89 )
		{
			Weight = 4.0;
		}

		public QuarterStaff( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}