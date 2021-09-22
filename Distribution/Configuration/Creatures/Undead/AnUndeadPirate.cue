package Undead

AnUndeadPirate: {
	Name:               "<random>, an undead pirate"
	CorpseNameOverride: "corpse of <random>, an undead pirate"
	BaseType:           "Server.Mobiles.BaseCreatureTemplate"
	Str:                110
	Int:                20
	Dex:                110
	AlwaysMurderer:     true
	CreatureType:       "Undead"
	VirtualArmor:       25
	FightMode:          "Aggressor"
	HitsMax:            110
	LootItemChance:     10
	LootItemLevel:      1
	LootTable:          "47"
	ManaMaxSeed:        100
	StamMaxSeed:        100
	Resistances: Poison: 6
	Skills: {
		Tactics:     100
		MagicResist: 30
	}
	Attack: {
		Damage: {
			Min: 4
			Max: 16
		}
	}
}