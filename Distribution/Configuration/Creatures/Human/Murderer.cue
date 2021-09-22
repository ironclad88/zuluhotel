package Human

Murderer: {
	Name:                 "a murderer"
	CorpseNameOverride:   "corpse of a murderer"

	Str:                  650
	Int:                  105
	Dex:                  200
	CreatureType:         "Human"
	HitPoison:            "Regular"
	HitsMax:              160
	LootItemChance:       1
	LootTable:            "47"
	ManaMaxSeed:          95
	ProvokeSkillOverride: 104
	StamMaxSeed:          50
	Skills: {
		Tactics:     85
		Fencing:     100
		Hiding:      100
		MagicResist: 50
	}
	Attack: {
		Damage: {
			Min: 23
			Max: 29
		}
		HitSound:  571
		MissSound: 569
	}
	Equipment: [{
		ItemType: "SShortHair"
		Hue:      1
	}, {
		ItemType: "SDagger"
		Name:     "Murderer's Dagger"
	}]
}
