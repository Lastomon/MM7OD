﻿namespace MMXOnline;

// Higher values are drawn above
public class ZIndex {
	public const long Default = 0;
	public const long Actor = -500000;
	public const long MainPlayer = -1000000;
	public const long Character = -2000000;
	public const long Background = -3000000;
	public const long Backwall = -4000000;
	public const long Parallax = -5000000;
	public const long Foreground = 2000000;
	public const long HUD = 1000000;
	public const long AboveFont = 2000000;
}

public enum Alignment {
	Left,
	Center,
	Right
}

public enum VAlignment {
	Top,
	Center,
	Bottom
}

// Clamping movement outside map bounds
public enum MoveClampMode {
	None,
	InnerBox,
	OuterBox
}

// Text Categories
public enum TCat {
	Default,
	Option,
	OptionNoSplit,
	Title,
	HUD,
	HUDColored,
	HUDBig,
	BotHelp,
	Chat,
	Custom,
}

public enum WeaponIds {
	// DO NOT change the order of these X weapons
	Buster = 500,
	Torpedo,
	Sting,
	RollingShield,
	FireWave,
	Tornado,
	ElectricSpark,
	Boomerang,
	ShotgunIce,

	CrystalHunter,
	BubbleSplash,
	SilkShot,
	SpinWheel,
	SonicSlicer,
	StrikeChain,
	MagnetMine,
	SpeedBurner,
	AcidBurst,
	ParasiticBomb,
	TriadThunder,
	SpinningBlade,
	RaySplasher,
	GravityWell,
	FrostShield,
	TunnelFang,
	ShotgunIceAlt,
	GigaCrush,
	ItemTracer,
	XSaber,
	HyperBuster,
	Hadouken,
	Shoryuken,
	Headbutt,
	UPGrab,
	UPPunch,
	UPParry,
	NovaStrike,
	X6Saber,
	

	// DO NOT CHANGE THE ORDER OF THESE SIGMA WEAPONS
	// Adrian: estoy arreglando tu chingadera mamawebo
	// edit: gud ending si lo arreglé
	Sigma,
	ChillPenguin,
	SparkMandrill,
	ArmoredArmadillo,
	LaunchOctopus,
	BoomerangKuwanger,
	StingChameleon,
	StormEagle,
	FlameMammoth,
	Velguarder,
	WireSponge,
	WheelGator,
	BubbleCrab,
	FlameStag,
	MorphMoth,
	MagnaCentipede,
	CrystalSnail,
	OverdriveOstrich,
	FakeZero,
	BlizzardBuffalo,
	ToxicSeahorse,
	TunnelRhino,
	VoltCatfish,
	CrushCrawfish,
	NeonTiger,
	GravityBeetle,
	BlastHornet,
	DrDoppler,

	ZSaber,
	ZSaberProjSwing,
	Raijingeki,
	Raijingeki2,
	Ryuenjin,
	Hyouretsuzan,
	Rakukojin,
	QuakeBlazer,
	Rakuhouha,
	Rekkoha,
	CFlasher,
	Shippuuga,
	EBlade,
	Rising,
	Gemnu,
	ShinMessenkou,
	Shingetsurin,
	AwakenedAura,
	Suiretsusen,
	TBreaker,
	FSplasher,
	Hyoroga,
	Kuuenzan,
	KKnuckle,
	KKnuckleParry,
	ZeroShoryuken,
	MegaPunchWeapon,
	DropKick,
	DarkHold,

	MechMenuWeapon,
	MechBuster,
	MechPunch,
	MechKangarooPunch,
	MechGoliathPunch,
	MechDevilBearPunch,
	MechGoliathStomp,
	MechStomp,
	MechKangarooStomp,
	MechHawkStomp,
	MechFrogStomp,
	MechDevilBearStomp,
	MechChainCharge,
	MechChain,
	MechMissile,
	MechTorpedo,
	ElectricShock,
	MK2StunShot,
	Vulcan,
	RocketPunch,
	FrontRunner,
	VileBomb,
	VileEBomb,
	Napalm,
	VileCutter,
	VileLaser,
	VileMK2Grab,
	NecroBurst,
	RisingSpecter,
	VileFlamethrower,
	StraightNightmare,
	HexaInvolute,

	AxlBullet,
	BlastLauncher,
	RayGun,
	DoubleBullet,
	AssassinBullet,
	Undisguise,
	BlackArrow,
	SpiralMagnum,
	BoundBlaster,
	PlasmaGun,
	IceGattling,
	FlameBurner,
	MetteurCrash,
	BeastHunter,
	MachineBullets,
	RevolverBarrel,
	AncientGun,

	SigmaSlash,
	SigmaBall,
	SigmaWolfHead,
	SigmaWolfHand,
	SigmaWolfHandBeam,
	ChillPIceShot,
	ChillPIceBlow,
	ChillPIcePenguin,
	ChillPBlizzard,
	ChillPSlide,
	SparkMPunch,
	SparkMStomp,
	SparkMSpark,
	ArmoredARoll,
	ArmoredAChargeRelease,
	ArmoredAProj,
	LaunchOMissile,
	LaunchOMelee,
	LaunchOHomingTorpedo,
	LaunchOWhirlpool,
	LaunchODrain,
	BoomerangKBoomerang,
	BoomerangKDeadLift,
	StingCSting,
	StingCTongue,
	StingCSpike,
	StormETornado,
	StormEDive,
	StormEBird,
	StormEGust,
	StormEEgg,
	FlameMFireball,
	FlameMStomp,
	FlameMOil,
	FlameMOilFire,
	VelGFire,
	VelGIce,
	VelGMelee,
	Sigma2Claw,
	Sigma2Ball,
	Sigma2Ball2,
	Sigma2ViralTackle,
	Sigma2ViralBeam,
	Sigma2Mechaniloid,
	ChillPGeneric,
	SparkMGeneric,
	ArmoredAGeneric,
	LaunchOGeneric,
	BoomerangKGeneric,
	StingCGeneric,
	StormEGeneric,
	FlameMGeneric,
	VelGGeneric,
	WSpongeGeneric,
	WSpongeStrikeChain,
	WheelGGeneric,
	BCrabGeneric,
	FStagGeneric,
	MorphMCGeneric,
	MorphMGeneric,
	MagnaCGeneric,
	CSnailGeneric,
	OverdriveOGeneric,
	FakeZeroGeneric,
	BBuffaloGeneric,
	TSeahorseGeneric,
	TunnelRGeneric,
	VoltCGeneric,
	CrushCGeneric,
	NeonTGeneric,
	GBeetleGeneric,
	BHornetGeneric,
	DrDopplerGeneric,
	Sigma3Shield,
	Sigma3Fire,
	Sigma3KaiserMissile,
	Sigma3KaiserMine,
	Sigma3KaiserBeam,
	Sigma3KaiserStomp,
	RideChaserGun,
	RideChaserHit,
	DNACore,
}

public enum RockWeaponIds {
	MegaBuster,
	FreezeCracker,
	ThunderBolt,
	JunkShield,
	ScorchWheel,
	SlashClaw,
	NoiseCrush,
	DangerWrap,
	WildCoil,
	RushSearch,
	RushJet,
	RushCoil,
	SARocketPunch,
	SAArrowSlash,
	LegBreaker,
	GeminiLaser,
	HardKnuckle,
	SearchSnake,
	SparkShock,
	PowerStone,
	WaterWave,
	GyroAttack,
	StarCrash,
	NeedleCannon,
}
public enum BluesProjIds  {
	Lemon = 600,
	LemonAngled,
	BusterLV2,
	BusterLV3,
	BusterLV4,
	GeminiLaser,
	HardKnuckle,
	SearchSnake,
	SparkShock,
	PowerStone,
	GyroAttack,
	StarCrash,
	ShieldBlock,
	NeedleCannon,
	BigBangStrike,
	BigBangStrikeExplosion,
}

public enum RockProjIds  {
	RockBuster = 500,
	RockBusterMid,
	RockBusterCharged,
	FreezeCracker,
	FreezeCrackerPiece,
	ThunderBolt,
	ThunderBoltSplit,
	JunkShield,
	JunkShieldPiece,
	ScorchWheelSpawn,
	ScorchWheel,
	ScorchWheelLoop,
	ScorchWheelMove,
	ScorchWheelUnderwater,
	ScorchWheelBurn,
	SlashClaw,
	NoiseCrush,
	NoiseCrushCharged,
	DangerWrap,
	DangerWrapMine,
	DangerWrapExplosion,
	DangerWrapBubbleExplosion,
	WildCoil,
	WildCoilCharged,
	SARocketPunch,
	SAArrowSlash,
}

public enum ProjIds {

	// Adrián: Some of Rock projectiles IDs have to be here because GM moment
	Buster,
	Buster2,
	Buster3,
	Buster4,
	BusterUnpo,
	BusterX3Proj2,
	BusterX3Plasma,
	BusterX3PlasmaHit,
	Torpedo,
	TorpedoCharged,
	MechTorpedo,
	Sting,
	StingDiag,
	RollingShield,
	RollingShieldCharged,
	FireWave,
	FireWaveChargedStart,
	FireWaveCharged,
	Tornado,
	TornadoCharged,
	ElectricSpark,
	ElectricSparkCharged,
	Boomerang,
	BoomerangCharged,
	ShotgunIce,
	ShotgunIceSled,
	ShotgunIceCharged,
	CrystalHunter,
	CrystalHunterDash,
	BubbleSplash,
	BubbleSplashCharged,
	SilkShot,
	SilkShotShrapnel,
	SilkShotChargedLv2,
	SilkShotCharged,
	SpinWheel,
	SpinWheelChargedStart,
	SpinWheelCharged,
	SonicSlicer,
	SonicSlicerChargedStart,
	SonicSlicerCharged,
	StrikeChain,
	MagnetMine,
	MagnetMineCharged,
	SpeedBurner,
	SpeedBurnerWater,
	SpeedBurnerTrail,
	SpeedBurnerCharged,
	SpeedBurnerRecoil,
	AcidBurst,
	AcidBurstSmall,
	AcidBurstCharged,
	AcidBurstPoison,
	ParasiticBomb,
	ParasiticBombExplode,
	ParasiticBombCharged,
	TriadThunder,
	TriadThunderBall,
	TriadThunderBeam,
	TriadThunderCharged,
	TriadThunderQuake,
	SpinningBlade,
	SpinningBladeCharged,
	RaySplasher,
	RaySplasherChargedProj,
	RaySplasherTurret,
	RaySplasherHurtSelf,
	GravityWell,
	GravityWellCharged,
	FrostShield,
	FrostShieldAir,
	FrostShieldGround,
	FrostShieldChargedGrounded,
	FrostShieldChargedPlatform,
	TunnelFang,
	TunnelFang2,
	TunnelFangCharged,
	GigaCrush,
	ItemTracer,
	HyperBuster,
	Hadouken,
	Shoryuken,
	XSaber,
	XSaberProj,
	Headbutt,
	UPGrab,
	UPPunch,
	UPParryBlock,
	UPParryMelee,
	UPParryProj,
	NovaStrike,
	X6Saber,

	ZBuster,
	ZSaber,
	ZSaber1,
	ZSaber2,
	ZSaber3,
	ZSaberProj,
	ZSaberDash,
	ZSaberCrouch,
	ZSaberAir,
	ZSaberRollingSlash,
	ZSaberLadder,
	ZSaberslide,
	ZSaberProjSwing,
	SwordBlock,
	Ryuenjin,
	Denjin,
	RisingFang,
	Raijingeki,
	Raijingeki2,
	Hyouretsuzan,
	Hyouretsuzan2,
	Rakukojin,
	QuakeBlazer,
	QuakeBlazerFlame,
	Shippuuga,
	Gemnu,
	Rakuhouha,
	Rekkoha,
	CFlasher,
	ShinMessenkou,
	ZBuster2,
	ZBuster3,
	ZBuster4,
	Shingetsurin,
	AwakenedAura,
	FSplasher,
	HyorogaProj,
	HyorogaSwing,
	Suiretsusan,
	SuiretsusanProj,
	TBreaker,
	TBreakerProj,

	ElectricShock,
	MK2StunShot,
	VileMissile,
	PopcornDemon,
	PopcornDemonSplit,
	Vulcan,
	DistanceNeedler,
	BuckshotDance,
	RocketPunch,
	SpoiledBrat,
	InfinityGig,
	MechPunch,
	MechKangarooPunch,
	MechGoliathPunch,
	MechDevilBearPunch,
	MechStomp,
	MechFrogGroundPound,
	MechFrogStompShockwave,
	FrontRunner,
	LongshotGizmo,
	FatBoy,
	MechChain,
	MechMissile,
	MechBuster,
	MechBuster2,
	VileBomb,
	VileBombSplit,
	VileEBombStart,
	VileEBomb,
	PeaceOutRoller,
	NapalmGrenade,
	NapalmGrenade2,
	NapalmGrenadeSplashHit,
	NapalmSplashHit,
	Napalm,
	Napalm2,
	Napalm2Wall,
	Napalm2Flame,
	VileMK2Grab,
	NecroBurst,
	NecroBurstShrapnel,
	RisingSpecter,
	StraightNightmare,
	HexaInvolute,
	QuickHomesick,
	ParasiteSword,
	MaroonedTomahawk,
	WildHorseKick,
	SeaDragonRage,
	DragonsWrath,

	AxlBullet,
	MetteurCrash,
	BeastKiller,
	MachineBullets,
	RevolverBarrel,
	AncientGun,
	CopyShot,
	AssassinBullet,
	AssassinBulletQuick,
	BlastLauncher,
	BlastLauncherSplash,
	GreenSpinner,
	GreenSpinnerSplash,
	RayGun,
	RayGun2,
	SplashLaser,
	DoubleBullet,
	BlackArrow,
	WindCutter,
	SpiralMagnum,
	SpiralMagnumScoped,
	SniperMissile,
	SniperMissileBlast,
	BoundBlaster,
	BoundBlaster2,
	MovingWheel,
	PlasmaGun,
	PlasmaGun2,
	PlasmaGun2Hyper,
	VoltTornado,
	VoltTornadoHyper,
	IceGattling,
	IceGattlingHeadshot,
	IceGattlingHyper,
	GaeaShield,
	FlameBurner,
	FlameBurner2,
	FlameBurnerHyper,
	CircleBlaze,
	CircleBlazeExplosion,

	SigmaSlash,
	SigmaSwordBlock,
	SigmaBall,
	SigmaWolfHeadFlameProj,
	SigmaWolfHeadBallProj,
	SigmaHand,
	SigmaHandElecBeam,
	ChillPIceShot,
	ChillPIceBlow,
	ChillPIcePenguin,
	ChillPBlizzard,
	ChillPSlide,
	SparkMPunch,
	SparkMStomp,
	SparkMSpark,
	ArmoredAProj,
	ArmoredARoll,
	ArmoredAChargeRelease,
	LaunchOMissle,
	LaunchOTorpedo,
	LaunchOWhirlpool,
	LaunchODrain,
	BoomerangKBoomerang,
	BoomerangKDeadLift,
	StingCSting,
	StingCTongue,
	StingCSpike,
	StormEEgg,
	StormEDive,
	StormETornado,
	StormEGust,
	StormEBird,
	FlameMFireball,
	FlameMStomp,
	FlameMStompShockwave,
	FlameMOil,
	FlameMOilSpill,
	FlameMOilFire,
	VelGFire,
	VelGIce,
	VelGMelee,
	Sigma2Ball,
	Sigma2Ball2,
	Sigma2Claw,
	Sigma2Claw2,
	Sigma2ClawDash,
	Sigma2ClawJump,
	Sigma2UpDownClaw,
	Sigma2ViralTackle,
	Sigma2ViralBeam,
	Sigma2ViralProj,
	Sigma2TankProj,
	Sigma2BirdProj,
	Sigma2HopperDrill,
	Sigma2ViralPossess,
	WSpongeSeed,
	WSpongeSpike,
	WSpongeChainSpin,
	WSpongeChain,
	WSpongeUpChain,
	WSpongeLightning,
	WheelGSpin,
	WheelGSpinWheel,
	WheelGSpit,
	WheelGBite,
	WheelGEat,
	WheelGGrab,
	WheelGUpBite,
	WheelGStomp,
	BCrabClaw,
	BCrabBubbleSplash,
	BCrabBubbleShield,
	BCrabCrabling,
	BCrabCrablingBubble,
	FStagDashCharge,
	FStagDash,
	FStagDashTrail,
	FStagFireball,
	FStagUppercut,
	MorphMMelee,
	MorphMBeam,
	MorphMPowder,
	MorphMCSpin,
	MorphMCSwing,
	MorphMCThread,
	MorphMCScrap,
	MagnaCMelee,
	MagnaCShuriken,
	MagnaCMagnetMine,
	MagnaCMagnetPull,
	MagnaCTail,
	CSnailMelee,
	CSnailCrystalHunter,
	CSnailSpecial,
	OverdriveOMelee,
	OverdriveOSonicSlicer,
	OverdriveOSonicSlicerUp,
	FakeZeroMelee,
	FakeZeroBuster,
	FakeZeroBuster2,
	FakeZeroSwordBeam,
	FakeZeroGroundPunch,
	BBuffaloIceProj,
	BBuffaloIceProjGround,
	BBuffaloBeam,
	BBuffaloDrag,
	BBuffaloCrash,
	BBuffaloStomp,
	TSeahorseAcid1,
	TSeahorseAcid2,
	TSeahorsePuddle,
	TSeahorseEmerge,
	TunnelRTornadoFang,
	TunnelRTornadoFang2,
	TunnelRTornadoFangDiag,
	TunnelRDash,
	TunnelRStomp,
	VoltCBall,
	VoltCTriadThunder,
	VoltCSuck,
	VoltCUpBeam,
	VoltCUpBeam2,
	VoltCCharge,
	VoltCBarrier,
	VoltCSparkle,
	VoltCStomp,
	CrushCProj,
	CrushCArmProj,
	CrushCGrab,
	CrushCGrabAttack,
	NeonTRaySplasher,
	NeonTClaw,
	NeonTClaw2,
	NeonTClawAir,
	NeonTClawDash,
	NeonTClawWall,
	NeonTSpecial,
	GBeetleBall,
	GBeetleGravityWell,
	GBeetleLift,
	GBeetleLiftCrash,
	GBeetleStomp,
	BHornetBee,
	BHornetHomingBee,
	BHornetSting,
	BHornetCursor,
	DrDopplerBall,
	DrDopplerBall2,
	DrDopplerDash,
	DrDopplerDashWater,
	DrDopplerAbsorb,
	Sigma3Shield,
	Sigma3ShieldBlock,
	Sigma3Fire,
	Sigma3KaiserMissile,
	Sigma3KaiserMine,
	Sigma3KaiserBeam,
	Sigma3KaiserStomp,
	Sigma3KaiserSuit,
	MaverickContactDamage,
	Burn,
	SigmaVirus,
	RideChaserProj,
	RideChaserHit,
	RideChaserCrash,
	DarkHold,
	// PunchyZeroStuff
	PZeroPunch,
	PZeroPunch2,
	PZeroAirKick,
	PZeroSenpuukyaku, // Spin kick
	PZeroShoryuken, // Air Upercut
	PZeroEnkoukyaku, // Drop Kick
	PZeroYoudantotsu, // Strong Punch
	PZeroParryStart,
	PZeroParryAttack,

	// Dark Zero buster.
	DZBuster,
	DZBuster2,
	DZBuster3,
	DZBuster4,
	DZMelee,
	DZHadangeki,
	
	//Rock stuff
	SlashClaw,
	ScorchWheelUnderwater,
	LegBreaker,
	ProtoStrike,
	ProtoStrikeLvl2,

	// Special damage types.
	SelfDmg = 30000,

	// Enviroment effect shenanigans.
	KillZone = 32000,

	// Close to the int16 max value.
	// Do not add things bellow this.
	SelfDestruct = 32700,

	
}

public enum WeaponSlotsIds
{
	
	//X Slots
	Buster,
	HomingTorpedo,
	ChameleonSting,
	RollingShield,
	FireWave,
	StormTornado,
	ElectricSpark,
	BoomerangCutter,
	ShotgunIce,
	CrystalHunter,
	BubbleSplash,
	SilkShot,
	SpinWheel,
	SonicSlicer,
	StrikeChain,
	MagnetMine,
	SpeedBurner,
	AcidBurst,
	ParasiticBomb,
	TriadThunder,
	SpinningBlade,
	RaySplasher,
	GravityWell,
	FrostShield,
	TunnelFang,
	ShotgunIceAlt,
	GigaCrush = 57,
	HyperBuster,
	Hadouken,
	Shoryuken,
	NovaStrike,
	FalconGigaAttack,
	StockBuster,
	XSaber,


	//Zero Slots
	Rakuhouha,
	CFlasher,
	Rekkoha,
	ShinMessenkou,
	DarkHold,


	//Vile Slots
	FrontRunner = 72,
	LongshotGizmo,
	FatBoy,
	CherryBlast,
	DistanceNeedler,
	BuckshotDance,
	HumerusCrush,
	PopcornDemon,
	ElectricShock,
	GoGetterRight,
	InfinityGig,
	SpoiledBrat,
	ParasiteSword,
	QuickHomesick,
	MaroonedTomahawk,
	WildHorseKick,
	DragonsWrath,
	SeaDragonRage,
	RumblingBang,
	FlameRound,
	SplashHit,
	ExplosiveRound,
	SpreadShot,
	PeaceOutRoller,
	DashModule,
	RideArmorSlot = 98,
	BlackBearRide,
	KangarroRide,
	HawkRide,
	FrogRide,
	RabbitRide,
	GoliathRide,
	DevilBearRide,


	//Axl Slots
	AxlBullet,
	MetteurCrash,
	MachineBullets,
	RevolverBarrel,
	AncientGun,
	DoubleBullet,
	BeastHunter,
	BoundBlaster,
	RayGun,
	SpiralMagnum,
	GLauncher,
	IceGattling = 118,
	FlameBurner,
	BlackArrow,
	PlasmaGun,
	AxlUndisguise = 142,
	AssassinBullet,
	WhiteAxl,
	StealthAxl,


	//Sigma Slots
	ChillPenguin,
	SparkMandrill,
	ArmoredArmadillo,
	LaunchOctopus,
	BoomerKuwanger,
	StingChameleon,
	StormEagle,
	FlameMammoth,
	Velguarder,
	WireSponge,
	WheelGator,
	BubbleCrab,
	FlameStag,
	MorphMoth,
	MorphMothCocoon,
	MagnaCentipede,
	CrystalSnail,
	OverdriveOstrich,
	FakeZero,
	BlizzardBuffalo,
	ToxicSeahorse,
	TunnelRhino,
	VoltCatfish,
	CrushCrawfish,
	NeonTiger,
	GravityBeetle,
	BlastHornet,
	DrDoppler,
	WolfSigmaHead = 201,
	WolfSigmaHand,
	NeoSigmaBall,
	VirusSigma,
	CommanderSigmaBall,
	MechaniloidTiranos,
	MechaniloidScrewdriver,
	MechaniloidBird,
	MechaniloidFish,
	

	//Characters Slots
	XSlot,
	ZeroSlot,
	VileSlot,
	AxlSlot,
	SigmaSlot,

}

public enum RockWeaponSlotIds
{
	MegaBuster,
	FreezeCracker,
	ThunderBolt,
	JunkShield,
	ScorchWheel,
	SlashClaw,
	NoiseCrush,
	DangerWrap,
	WildCoil,
	RushSearch,
	RushJet,
	RushCoil,
	SARocketPunch,
	ArrowSlash,
	LegBreaker,
}

public enum WeaponBarsIds
{	
	// X Bars
	Buster,
	HomingTorpedo,
	ChameleonSting,
	RollingShield,
	FireWave,
	StormTornado,
	ElectricSpark,
	BoomerangCutter,
	ShotgunIce,
	CrystalHunter,
	BubbleSplash,
	SilkShot,
	SpinWheel,
	SonicSlicer,
	StrikeChain,
	MagnetMine,
	SpeedBurner,
	AcidBurst,
	ParasiticBomb,
	TriadThunder,
	SpinningBlade,
	RaySplasher,
	GravityWell,
	FrostShield,
	TunnelFang,
	ShotgunIceAlt,
	GigaCrush = 57,
	HyperBuster,
	NovaStrike,
	FalconGigaAttack,
	

	//Zero Bars
	Rakuhouha,
	CFlasher,
	Rekkoha,
	ShinMessenkou,
	DarkHold,


	//Vile Bar
	EBar = 68,


	//Axl Bars
	AxlBullet,
	MetteurCrash,
	MachineBullets,
	RevolverBarrel,
	AncientGun,
	DoubleBullet,
	BeastHunter,
	BoundBlaster,
	RayGun,
	SpiralMagnum,
	GLauncher,
	IceGattling = 81,
	FlameBurner,
	BlackArrow,
	PlasmaGun,
	

	//Sigma Bars
	SigmaBall = 105,
	Sigma2Ball,
	SigmaVirus,
	SparkMSpark,
	ArmoredARoll,
	LaunchOTorpedo,
	BoomerKTeleport,
	StingCCloak,
	StormEFlight,
	BCrabBubbleShield,
	MorphMFlight,
	MorphMCScrap,
	MagnaCTeleport,
	CSnailShell,
	FakeZeroBuster,
	TSeahorseEmerge,
	VoltCCharge,
	BHornetFlight,
	DrDopplerAbsorb,
	
}

public enum RockWeaponBarIds
{
	MegaBuster,
	FreezeCracker,
	ThunderBolt,
	JunkShield,
	ScorchWheel,
	SlashClaw,
	NoiseCrush,
	DangerWrap,
	WildCoil,
	RushSearch,
	RushJet,
	RushCoil,
}
