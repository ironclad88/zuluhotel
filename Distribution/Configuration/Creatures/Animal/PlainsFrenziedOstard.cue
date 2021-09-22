package Animal

PlainsFrenziedOstard: {
	Name:                 "a plains frenzied ostard"
	CorpseNameOverride:   "corpse of a plains frenzied ostard"
	BaseType:             "Server.Mobiles.BaseCreatureTemplate"
	Str:                  130
	Int:                  35
	Dex:                  180
	BaseSoundID:          629
	Body:                 218
	CreatureType:         "Animal"
	VirtualArmor:         10
	HideType:             "Ostard"
	Hides:                4
	HitsMax:              130
	Hue:                  53
	ManaMaxSeed:          0
	MinTameSkill:         80
	ProvokeSkillOverride: 110
	StamMaxSeed:          50
	Tamable:              true
	Skills: {
		Parry:       80
		MagicResist: 70
		Tactics:     100
		Wrestling:   140
	}
	Attack: {
		Speed: 25
		Damage: {
			Min: 10
			Max: 45
		}
		HitSound:  595
		MissSound: 597
	}
}