package Plant

DarkStrangler: {
	Name:               "a Dark Strangler"
	CorpseNameOverride: "corpse of a Dark Strangler"
	BaseType:           "Server.Mobiles.BaseCreatureTemplate"
	Str:                400
	Int:                45
	Dex:                200
	AiType:             "AI_Archer"
	AlwaysMurderer:     true
	BaseSoundID:        684
	Body:               8
	CreatureType:       "Plant"
	VirtualArmor:       40
	FightMode:          "Closest"
	FightRange:         7
	HitPoison:          "Regular"
	HitsMax:            400
	Hue:                1285
	LootItemChance:     50
	LootItemLevel:      4
	LootTable:          "128"
	ManaMaxSeed:        35
	StamMaxSeed:        95
	Resistances: Earth: 100
	Skills: {
		Parry:       90
		Hiding:      125
		Tactics:     110
		MagicResist: 75
		Archery:     150
	}
	Attack: {
		Speed: 20
		Damage: {
			Min: 16
			Max: 46
		}
		HitSound:  458
		MissSound: 457
	}
	Equipment: [{
		ItemType: "Server.Items.Bow"
		Name:     "Dark Strangler Weapon"
		Hue:      1171
	}, {
		ItemType:    "Server.Items.HeaterShield"
		Name:        "Shield AR30"
		ArmorRating: 30
	}]
}