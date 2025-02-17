﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ProtoBuf;
using static SFML.Window.Keyboard;

namespace MMXOnline;

public partial class Player {
	public SpawnPoint? firstSpawn;
	public Input input;
	public Character? character;
	public Character lastCharacter;
	public bool ownedByLocalPlayer;
	public int? awakenedCurrencyEnd;
	public float fgMoveAmmo = 1920;
	public float fgMoveMaxAmmo = 1920;
	public bool isDefenderFavoredNonOwner;

	public bool isDefenderFavored {
		get {
			if (character != null && !character.ownedByLocalPlayer) {
				return isDefenderFavoredNonOwner;
			}
			if (Global.level?.server == null) {
				return false;
			}
			if (Global.serverClient == null) {
				return false;
			}
			if (Global.level.server.netcodeModel == NetcodeModel.FavorAttacker) {
				if (Global.serverClient?.isLagging() == true) {
					return true;
				}
				return (getPingOrStartPing() >= Global.level.server.netcodeModelPing);
			}
			return true;
		}
	}

	public string getDisplayPing() {
		int? pingOrStartPing = getPingOrStartPing();
		if (pingOrStartPing == null) return "?";
		return pingOrStartPing.Value.ToString();
	}
	public string getTeamDisplayPing() {
		int? pingOrStartPing = getPingOrStartPing();
		if (pingOrStartPing == null) {
			return "?";
		}
		return MathInt.Floor(pingOrStartPing.Value / 10f).ToString();
	}

	public int? getPingOrStartPing() {
		if (ping == null) {
			return serverPlayer?.startPing;
		}
		return ping.Value;
	}

	public Character preTransformedAxl;
	public bool isDisguisedAxl {
		get {
			return disguise != null;
		}
	}
	public List<Weapon> savedDNACoreWeapons = new List<Weapon>();
	public int axlBulletType;
	public List<bool> axlBulletTypeBought = new List<bool>() { true, false, false, false, false, false, false };
	public List<float> axlBulletTypeAmmo = new List<float>() { 0, 0, 0, 0, 0, 0, 0 };
	public List<float> axlBulletTypeLastAmmo = new List<float>() { 32, 32, 32, 32, 32, 32, 32 };
	public int lastDNACoreIndex = 4;
	public DNACore lastDNACore;
	
	public float zoomRange {
		get {
			if (character is Axl axl && (axl.isWhiteAxl() || axl.hyperAxlStillZoomed)) return 100000;
			return Global.viewScreenW * 2.5f;
		}
	}
	public RaycastHitData assassinHitPos;

	public bool canUpgradeXArmor() {
		return (realCharNum == 0);
	}

	public float adjustedZoomRange { get { return zoomRange - 40; } }

	public int getVileWeight(int? overrideLoadoutWeight = null) {
		if (overrideLoadoutWeight == null) {
			overrideLoadoutWeight = loadout.vileLoadout.getTotalWeight();
		}
		return overrideLoadoutWeight.Value;
	}

	public static Player stagePlayer = createStagePlayer();
	public static Player createStagePlayer() {
		return new Player(
			"Stage", 255, -1,
			new PlayerCharData() { charNum = -1 },
			false, true, GameMode.neutralAlliance,
			new Input(false),
			new ServerPlayer(
				"Stage", 255, false,
				-1, GameMode.neutralAlliance, "NULL", null, 0
			)
		);
	}

	public Point? lastDeathPos;
	public bool lastDeathWasVileMK2;
	public bool lastDeathWasVileMK5;
	public bool lastDeathWasSigmaHyper;
	public bool lastDeathWasXHyper;
	public const int zeroHyperCost = 10;
	public const int zBusterZeroHyperCost = 8;
	public const int AxlHyperCost = 10;
	public const int reviveVileCost = 5;
	public const int reviveSigmaCost = 10;
	public const int reviveXCost = 10;
	public const int goldenArmorCost = 5;
	public const int ultimateArmorCost = 10;
	public bool lastDeathCanRevive;
	public int vileFormToRespawnAs;
	public bool hyperSigmaRespawn;
	public bool hyperXRespawn;
	public float trainingDpsTotalDamage;
	public float trainingDpsStartTime;
	public bool showTrainingDps { get { return isAI && Global.serverClient == null && Global.level.isTraining(); } }

	public bool aiTakeover;
	public MaverickAIBehavior currentMaverickCommand;

	public bool isX { get { return charNum == (int)CharIds.X; } }
	public bool isZero { get { return charNum == (int)CharIds.Zero; } }
	public bool isVile { get { return charNum == (int)CharIds.Vile; } }
	public bool isAxl { get { return charNum == (int)CharIds.Axl; } }
	public bool isSigma { get { return charNum == (int)CharIds.Sigma; } }
	public bool isRock { get { return charNum == (int)CharIds.Rock; } }
	public bool isBlues { get { return charNum == (int)CharIds.Blues; } }
	public bool isBass { get { return charNum == (int)CharIds.Bass; } }


	public float healthBackup;

	public float health {
		get => (float)(character?.health ?? 0);
		set {
			if (character != null) {
				character.health = (decimal)value;
			}
		}
	}
	public float _maxHealth = 16;
	public float maxHealth {
		get {
			if (character != null) {
				_maxHealth = (float)character.maxHealth;
			}
			return _maxHealth;
		}
		set {
			if (character != null) {
				character.maxHealth = (decimal)value;
			}
			_maxHealth = value;
		}
	}

	public bool isDead {
		get {
			if (isSigma && currentMaverick != null) {
				return false;
			}
			if (character == null) return true;
			if (ownedByLocalPlayer && character.charState is Die) return true;
			else if (!ownedByLocalPlayer) {
				return health <= 0;
			}
			return false;
		}
	}
	public const float armorHealth = 16;
	public float respawnTime;
	public bool lockWeapon;
	public string[] randomTip;
	public int aiArmorPath;
	public float teamHealAmount;
	public List<CopyShotDamageEvent> copyShotDamageEvents = new List<CopyShotDamageEvent>();

	public bool scanned;
	public bool tagged;

	public List<int> aiArmorUpgradeOrder;
	public int aiArmorUpgradeIndex;
	public bool isAI;   //DO NOT USE THIS for determining if a player is a bot to non hosts in online matches, use isBot below
						//A bot is a subset of an AI; an AI that's in an online match
	public bool isBot {
		get {
			if (serverPlayer == null) {
				return isAI;
			}
			return serverPlayer.isBot;
		}
	}

	public bool isLocalAI {
		get { return isAI && Global.serverClient == null; }
	}

	public int realCharNum {
		get {
			if (isAxl || isDisguisedAxl) return 3;
			return charNum;
		}
	}

	public bool warpedInOnce;
	public bool spawnedOnce;

	public bool isMuted;

	// ETanks
	private Dictionary<int, List<ETank>> charETanks = new Dictionary<int, List<ETank>>() {
		{ (int)CharIds.X, new List<ETank>() },
		{ (int)CharIds.Zero, new List<ETank>() },
		{ (int)CharIds.Vile, new List<ETank>() },
		{ (int)CharIds.Axl, new List<ETank>() },
		{ (int)CharIds.Sigma, new List<ETank>() },
		{ (int)CharIds.PunchyZero, new List<ETank>() },
		{ (int)CharIds.BusterZero, new List<ETank>() },
		{ (int)CharIds.Rock, new List<ETank>() },
		{ (int)CharIds.Blues, new List<ETank>() },
		{ (int)CharIds.Bass, new List<ETank>() },
	};

	// WTanks

	public Dictionary<int, List<WTank>> charWTanks = new Dictionary<int, List<WTank>>() {
		{ (int)CharIds.Rock, new List<WTank>() },
		{ (int)CharIds.Blues, new List<WTank>() },
		{ (int)CharIds.Bass, new List<WTank>()},
	};

	// Blues exclusive tanks (L and M)

	public Dictionary<int, List<LTank>> charLTanks = new Dictionary<int, List<LTank>>() {
		{ (int)CharIds.Blues, new List<LTank>() },
	};

	public Dictionary<int, List<MTank>> charMTanks = new Dictionary<int, List<MTank>>() {
		{ (int)CharIds.Blues, new List<MTank>() },
	};

	// Heart tanks
	private Dictionary<int, ProtectedInt> charHeartTanks = new Dictionary<int, ProtectedInt>() {
		{ (int)CharIds.X, new() },
		{ (int)CharIds.Zero, new() },
		{ (int)CharIds.Vile, new() },
		{ (int)CharIds.Axl, new() },
		{ (int)CharIds.Sigma, new() },
		{ (int)CharIds.PunchyZero, new() },
		{ (int)CharIds.BusterZero, new() },
		{ (int)CharIds.Rock, new() },
		{ (int)CharIds.Blues, new() },
		{ (int)CharIds.Bass, new()}
	};
	

	// Getter functions.
	public List<ETank> ETanks {
		get { return charETanks[isDisguisedAxl ? 3 : charNum]; }
		set { charETanks[isDisguisedAxl ? 3 : charNum] = value; }
	}
	public List<WTank> wtanks {
		get { return charWTanks[isDisguisedAxl ? 3 : charNum];}
		set { charWTanks[isDisguisedAxl ? 3 : charNum] = value;}
	}
	public List<LTank> ltanks {
		get { return charLTanks[charNum]; }
		set { charLTanks[charNum] = value;}
	}
	public List<MTank> mtanks {
		get { return charMTanks[charNum]; }
		set { charMTanks[charNum] = value;}
	}
	public int heartTanks {
		get {
			if (!ownedByLocalPlayer) {
				return charHeartTanks[isDisguisedAxl ? 3 : charNum].unsafeVal;
			}
			return charHeartTanks[isDisguisedAxl ? 3 : charNum].value;
		}
		set {
			charHeartTanks[isDisguisedAxl ? 3 : charNum].value = value;
		}
	}

	// Currency
	public const int maxCharCurrencyId = 12;
	public static int curMul = Helpers.randomRange(2, 8);

	public ProtectedArrayInt charCurrency = new ProtectedArrayInt(maxCharCurrencyId);
	public int currency {
		get {
			if (!ownedByLocalPlayer) {
				return charCurrency.unsafeVal(isDisguisedAxl ? 3 : charNum);
			}
			return charCurrency[isDisguisedAxl ? 3 : charNum];
		}
		set {
			charCurrency[isDisguisedAxl ? 3 : charNum] = value;
		}
	}

	public bool isSpectator {
		get {
			if (Global.serverClient == null) return isOfflineSpectator;
			return serverPlayer.isSpectator;
		}
		set {
			if (Global.serverClient == null) isOfflineSpectator = value;
			else serverPlayer.isSpectator = value;
		}
	}
	private bool isOfflineSpectator;
	public bool is1v1Combatant;

	public void setSpectate(bool newSpectateValue) {
		if (Global.serverClient != null) {
			string msg = name + " now spectating.";
			if (newSpectateValue == false) msg = name + " stopped spectating.";
			Global.level.gameMode.chatMenu.addChatEntry(new ChatEntry(msg, "", null, true), sendRpc: true);
			RPC.makeSpectator.sendRpc(id, newSpectateValue);
		} else {
			isSpectator = newSpectateValue;
		}
	}

	public int selectedRAIndex = Global.quickStartMechNum ?? 0;
	public bool isSelectingRA() {
		if (character is Vile vile && vile.rideMenuWeapon.isMenuOpened) {
			return true;
		}
		return false;
	}

	public int selectedCommandIndex = 0;
	public bool isSelectingCommand() {
		if (weapon is MaverickWeapon mw && mw.isMenuOpened) {
			return true;
		}
		return false;
	}

	// Things needed to be synced to late joiners.
	// Note: these are not automatically applied,
	// you need to add code in Global.level.joinedLateSyncPlayers
	// and update PlayerSync class at top of this file
	public int kills;
	public int assists;
	public int deaths;
	public int getDeathScore() {
		if (Global.level.gameMode is Elimination ||
			Global.level.gameMode is TeamElimination
		) {
			return (Global.level.gameMode.playingTo - deaths);
		}
		return deaths;
	}
	public ushort curMaxNetId;
	public ushort curATransNetId;
	public bool warpedIn = false;
	public float readyTime;
	public const float maxReadyTime = 1.75f;
	public bool readyTextOver = false;
	public ServerPlayer serverPlayer;
	public LoadoutData loadout;
	public bool loadoutSet;
	public LoadoutData previousLoadout;
	public LoadoutData? atransLoadout;
	public AxlLoadout axlLoadout { get { return loadout.axlLoadout; } }

	public bool frozenCastlePurchased;
	public bool speedDevilPurchased;

	// Note: Every time you add an armor, add an "old" version and update DNA Core code appropriately
	public ushort armorFlag;
	public bool frozenCastle;
	public bool speedDevil;

	public Disguise? disguise;

	public int newAlliance;     // Not sure what this is useful for, seems like a pointless clone of alliance that needs to be kept in sync

	// Things that ServerPlayer already has
	public string name;
	public int id;
	public int alliance;    // Only set on spawn with data read from ServerPlayer alliance. The ServerPlayer alliance changes earlier on team change/autobalance
	public int charNum = 5;

	public int newCharNum;
	public int? delayedNewCharNum;
	public int? ping;
	public void syncFromServerPlayer(ServerPlayer serverPlayer) {
		if (!this.serverPlayer.isHost && serverPlayer.isHost) {
			promoteToHost();
		}
		this.serverPlayer = serverPlayer;
		name = serverPlayer.name;
		ping = serverPlayer.ping;

		kills = serverPlayer.kills;
		deaths = serverPlayer.deaths;

		if (ownedByLocalPlayer && serverPlayer.autobalanceAlliance != null &&
			newAlliance != serverPlayer.autobalanceAlliance.Value
		) {
			Global.level.gameMode.chatMenu.addChatEntry(
				new ChatEntry(
					name + " was autobalanced to " +
					GameMode.getTeamName(serverPlayer.autobalanceAlliance.Value), "", null, true
				), true);
			forceKill();
			currency += 5;
			Global.serverClient?.rpc(RPC.switchTeam, RPCSwitchTeam.getSendMessage(id, serverPlayer.autobalanceAlliance.Value));
			newAlliance = serverPlayer.autobalanceAlliance.Value;
		}
	}

	// Shaders
	public ShaderWrapper xPaletteShader = Helpers.cloneGenericPaletteShader("paletteTexture");
	public ShaderWrapper xStingPaletteShader = Helpers.cloneGenericPaletteShader("cStingPalette");
	public ShaderWrapper invisibleShader = Helpers.cloneShaderSafe("invisible");
	//public ShaderWrapper zeroPaletteShader = Helpers.cloneGenericPaletteShader("hyperZeroPalette");
	//public ShaderWrapper nightmareZeroShader = Helpers.cloneGenericPaletteShader("paletteViralZero");
	//public ShaderWrapper zeroAzPaletteShader = Helpers.cloneGenericPaletteShader("paletteAwakenedZero");
	//public ShaderWrapper axlPaletteShader = Helpers.cloneShaderSafe("hyperaxl");
	//public ShaderWrapper viralSigmaShader = Helpers.cloneShaderSafe("viralsigma");
	//public ShaderWrapper viralSigmaShader2 = Helpers.cloneShaderSafe("viralsigma");
	//public ShaderWrapper sigmaShieldShader = Helpers.cloneGenericPaletteShader("paletteSigma3Shield");
	public ShaderWrapper acidShader = Helpers.cloneShaderSafe("acid");
	public ShaderWrapper oilShader = Helpers.cloneShaderSafe("oil");
	public ShaderWrapper igShader = Helpers.cloneShaderSafe("freezeSlow");
	public ShaderWrapper infectedShader = Helpers.cloneShaderSafe("infected");
	//public ShaderWrapper frozenCastleShader = Helpers.cloneShaderSafe("frozenCastle");
	//public ShaderWrapper possessedShader = Helpers.cloneShaderSafe("possessed");
	//public ShaderWrapper vaccineShader = Helpers.cloneShaderSafe("vaccine");
	//public static ShaderWrapper darkHoldShader = Helpers.cloneShaderSafe("darkhold");
	public ShaderWrapper speedTrailShader = Helpers.cloneShaderSafe("speedTrail");

	// Maverick shaders.
	// Duplicated mavericks are not a thing so this should not be a problem.
	//public ShaderWrapper catfishChargeShader = Helpers.cloneGenericPaletteShader("paletteVoltCatfishCharge");
	//public ShaderWrapper gatorArmorShader = Helpers.cloneShaderSafe("wheelgEaten");
	//public ShaderWrapper spongeChargeShader = Helpers.cloneShaderSafe("wspongeCharge");

	// Projectile shaders.
	public ShaderWrapper timeSlowShader = Helpers.cloneShaderSafe("timeslow");
	//public ShaderWrapper darkHoldScreenShader = Helpers.cloneShaderSafe("darkHoldScreen");

	// New shaders.
	public ShaderWrapper burnStateShader = Helpers.cloneShaderSafe("burning");
	public ShaderWrapper evilEnergyShader = Helpers.cloneShaderSafe("evil_energy");
	public ShaderWrapper rockPaletteShader = Helpers.cloneGenericPaletteShader("rock_palette_texture");
	//public ShaderWrapper rockCharge1 = Helpers.cloneGenericPaletteShader("rock_charge_texture");
	//public ShaderWrapper rockCharge2 = Helpers.cloneGenericPaletteShader("rock_charge2_texture");
	public ShaderWrapper breakManShader = Helpers.cloneGenericPaletteShader("blues_hyperpalette");
	public ShaderWrapper bluesScarfShader = Helpers.cloneGenericPaletteShader("blues_palette_texture");
	public ShaderWrapper bassPaletteShader = Helpers.cloneGenericPaletteShader("bass_palette_texture");

	// Character specific data populated on RPC request
	public ushort? charNetId;
	public ushort? charRollingShieldNetId;
	public float charXPos;
	public float charYPos;
	public int charXDir;
	public Dictionary<int, int> charNumToKills = new Dictionary<int, int>() {
	};

	public int hyperChargeSlot;
	public int xArmor1v1;
	public float vileAmmo = 32;
	public float vileMaxAmmo = 32;
	public float sigmaAmmo = 32;
	public float sigmaMaxAmmo = 32;
	public int? maverick1v1;
	public bool maverick1v1Spawned;

	public float possessedTime;
	public const float maxPossessedTime = 12;
	public Player possesser;

	//Evil Energy stuff.
	public int pendingEvilEnergyStacks;
	public int evilEnergyStacks;
	public float evilEnergyTime;
	public float evilEnergyMaxTime = 1800;
	public float hpPerStack = 4;
	public List<GrenadeProj> grenades = new List<GrenadeProj>();
	public List<ChillPIceStatueProj> iceStatues = new List<ChillPIceStatueProj>();
	public List<WSpongeSpike> seeds = new List<WSpongeSpike>();
	public List<Actor> mechaniloids = new List<Actor>();

	public ExplodeDieEffect explodeDieEffect;
	public Character limboChar;
	public bool suicided;

	ushort savedArmorFlag;
	public bool[] headArmorsPurchased = new bool[] { false, false, false };
	public bool[] bodyArmorsPurchased = new bool[] { false, false, false };
	public bool[] armArmorsPurchased = new bool[] { false, false, false };
	public bool[] bootsArmorsPurchased = new bool[] { false, false, false };

	public float lastMashAmount;
	public int lastMashAmountSetFrame;

	public bool isNon1v1MaverickSigma() {
		return isSigma && maverick1v1 == null;
	}

	public bool is1v1MaverickX1() {
		return maverick1v1 <= 8;
	}

	public bool is1v1MaverickX2() {
		return maverick1v1 > 8 && maverick1v1 <= 17;
	}

	public bool is1v1MaverickX3() {
		return maverick1v1 > 17;
	}

	public bool is1v1MaverickFakeZero() {
		return maverick1v1 == 17;
	}

	public int getStartHeartTanks() {
		if (Global.level.isNon1v1Elimination() && Global.level.gameMode.playingTo < 3) {
			return 8;
		}
		if (Global.level.is1v1()) {
			return 8;
		}
		if (Global.level?.server?.customMatchSettings != null) {
			return Global.level.server.customMatchSettings.startHeartTanks;
		}

		return 0;
	}

	public int getStartHeartTanksForChar() {
		/*if (!Global.level.server.disableHtSt && Global.level?.server?.customMatchSettings == null && !Global.level.gameMode.isTeamMode) {
			int leaderKills = Global.level.getLeaderKills();
			if (leaderKills >= 32) return 8;
			if (leaderKills >= 28) return 7;
			if (leaderKills >= 24) return 6;
			if (leaderKills >= 20) return 5;
			if (leaderKills >= 16) return 4;
			if (leaderKills >= 12) return 3;
			if (leaderKills >= 8) return 2;
			if (leaderKills >= 4) return 1;
		}*/
		return 0;
	}

	public int getStartETanks() {
		if (Global.level?.server?.customMatchSettings != null) {
			return Global.level.server.customMatchSettings.startETanks;
		}

		return 0;
	}

	public int getStartWTanks() {
		if (Global.level?.server?.customMatchSettings != null) {
			return Global.level.server.customMatchSettings.startWTanks;
		}
		return 0;
	}

	public int getStartETanksForChar() {
		/*if (!Global.level.server.disableHtSt && Global.level?.server?.customMatchSettings == null && !Global.level.gameMode.isTeamMode) {
			int leaderKills = Global.level.getLeaderKills();
			if (leaderKills >= 32) return 4;
			if (leaderKills >= 24) return 3;
			if (leaderKills >= 16) return 2;
			if (leaderKills >= 8) return 1;
		}*/

		//return 0;
		return getStartETanks();
	}

	public int getSameCharNum() {
		if (Global.level?.server?.customMatchSettings != null) {
			if (Global.level.gameMode.isTeamMode && alliance == GameMode.redAlliance) {
				return Global.level.server.customMatchSettings.redSameCharNum;
			}
			return Global.level.server.customMatchSettings.sameCharNum;
		}
		return -1;
	}

	public Player(
		string name, int id, int charNum, PlayerCharData playerData,
		bool isAI, bool ownedByLocalPlayer, int alliance, Input input, ServerPlayer serverPlayer
	) {
		this.name = name;
		this.id = id;
		curMaxNetId = getFirstAvailableNetId();
		this.alliance = alliance;
		newAlliance = alliance;
		this.isAI = isAI;
		this.charNum = charNum;
		newCharNum = charNum;

		this.input = input;
		this.ownedByLocalPlayer = ownedByLocalPlayer;

		this.xArmor1v1 = playerData?.armorSet ?? 1;
		if (Global.level.is1v1() && isX) {
			legArmorNum = xArmor1v1;
			bodyArmorNum = xArmor1v1;
			helmetArmorNum = xArmor1v1;
			armArmorNum = xArmor1v1;
		}

		for (int i = 0; i < charCurrency.Length; i++) {
			charCurrency[i] = getStartCurrency();
		}
		foreach (var key in charHeartTanks.Keys) {
			int htCount = getStartHeartTanksForChar();
			int altHtCount = getStartHeartTanks();
			if (altHtCount > htCount) {
				htCount = altHtCount;
			}
			charHeartTanks[key].value = htCount;
		}
		foreach (var key in charETanks.Keys) {
			int stCount = key == charNum ? getStartETanksForChar() : getStartETanks();
			for (int i = 0; i < stCount; i++) {
				charETanks[key].Add(new ETank());
			}
		}

		foreach (var key in charWTanks.Keys) {
			int wtCount = getStartWTanks();
			for (int i=0; i < wtCount; i++) {
				charWTanks[key].Add(new WTank());
			}
		}

		maxHealth = getMaxHealth();
		if (charNum == (int)CharIds.Bass) maxHealth -= evilEnergyStacks * hpPerStack;
		health = maxHealth;

		aiArmorPath = new List<int>() { 1, 2, 3 }.GetRandomItem();
		aiArmorUpgradeOrder = new List<int>() { 0, 1, 2, 3 }.Shuffle();

		this.serverPlayer = serverPlayer;

		if (ownedByLocalPlayer && !isAI) {
			loadout = LoadoutData.createFromOptions(id);
			loadoutSet = true;
		} else {
			loadout = LoadoutData.createRandom(id);
			if (ownedByLocalPlayer) {
				loadoutSet = true;
			}
		}

		is1v1Combatant = !isSpectator;
		maxHealth = getMaxHealth();
	}

	public int getHeartTankModifier() {
		return Global.level.server?.customMatchSettings?.heartTankHp ?? 1;
	}

	public float getMaverickMaxHp() {
		if (!Global.level.is1v1() && isTagTeam()) {
			return getModifiedHealth(20) + (heartTanks * getHeartTankModifier());
		}
		return MathF.Ceiling(getModifiedHealth(24));
	}

	public bool hasAllItems() {
		return ETanks.Count >= 4 && heartTanks >= 8;
	}

	public static float getBaseHealth() {
		if (Global.level.server.customMatchSettings != null) {
			return Global.level.server.customMatchSettings.healthModifier;
		}
		return 16;
	}

	public static float getModifiedHealth(float health) {
		if (Global.level.server.customMatchSettings != null) {
			float retHp = getBaseHealth();
			float extraHP = health - 16;

			float hpMulitiplier = MathF.Ceiling(getBaseHealth() / 16);
			retHp += MathF.Ceiling(extraHP * hpMulitiplier);

			if (retHp < 1) {
				retHp = 1;
			}
			return retHp;
		}
		return health;
	}

	public float getDamageModifier() {
		if (Global.level.server.customMatchSettings != null) {
			/*if (Global.level.gameMode.isTeamMode && alliance == GameMode.redAlliance) {
				return Global.level.server.customMatchSettings.redDamageModifier;
			}*/
			return Global.level.server.customMatchSettings.damageModifier;
		}
		return 1;
	}

	public float getMaxHealth() {
		int baseHP = 28;

		if (isBlues) {
			baseHP = 14;
		} else if (isBass) {
			baseHP = 20;
		}
		// 1v1 is the only mode without possible heart tanks/sub tanks
		if (Global.level.is1v1()) {
			return getModifiedHealth(baseHP);
		}
		
		return MathF.Ceiling(baseHP);
	}

	public void creditHealing(float healAmount) {
		teamHealAmount += healAmount;
		if (teamHealAmount >= 16) {
			teamHealAmount = 0;
			currency++;
		}
	}

	public void applyLoadoutChange() {
		loadout = LoadoutData.createFromOptions(id);
		syncLoadout();
	}

	public void syncLoadout() {
		RPC.broadcastLoadout.sendRpc(this);
	}

	public int? teamAlliance {
		get {
			if (Global.level.gameMode.isTeamMode) {
				return alliance;
			}
			return null;
		}
	}

	public int getHudLifeSpriteIndex() {
		return charNum + (maverick1v1 ?? -1) + 1;
	}

	public const int netIdsPerPlayer = 2000;

	// The first net id this player could possibly own. This includes the "reserved" ones
	public ushort getStartNetId() {
		return (ushort)(Level.firstNormalNetId + (ushort)(id * netIdsPerPlayer));
	}

	// The character net id is always the first net id of the player
	//public ushort getCharActorNetId() {
	//return getStartNetId();
	//}

	public static int? getPlayerIdFromCharNetId(ushort charNetId) {
		int netIdInt = charNetId;
		int maxIdInt = Level.firstNormalNetId;
		int diff = (netIdInt - maxIdInt);
		if (diff < 0) {
			return null;
		}
		if (diff % netIdsPerPlayer != 0) {
			return null;
		}
		netIdInt -= maxIdInt;
		return netIdInt / netIdsPerPlayer;
	}

	// First available unreserved net id for general instantiation use of new objects
	public ushort getFirstAvailableNetId() {
		// +0 = Char
		// +1 = Ride armor (GM19 changed this is not a RA anymore)
		return (ushort)(getStartNetId() + 10);
	}

	public ushort getNextATransNetId() {
		if (curATransNetId < getStartNetId()) {
			curATransNetId = (ushort)(getStartNetId());
		}
		ushort retId = curATransNetId;
		curATransNetId++;
		if (curATransNetId >= getStartNetId() + 10) {
			curATransNetId = (ushort)(getStartNetId());
		}
		return retId;
	}

	// Usually, only the main player is allowed to get the next actor net id.
	// The exception is if you call setNextActorNetId() first. The assert checks for that in debug.
	public ushort getNextActorNetId(bool allowNonMainPlayer = false) {
		// Increase by 1 normall.
		int retId = curMaxNetId;
		curMaxNetId++;
		// Use this to avoid duplicates.
		if (ownedByLocalPlayer) {
			while (
				Global.level.actorsById.GetValueOrDefault(curMaxNetId) != null &&
				curMaxNetId <= getStartNetId() + netIdsPerPlayer
			) {
				// Overwrite if destroyed.
				if (Global.level.actorsById[curMaxNetId].destroyed) {
					break;
				}
				curMaxNetId++;
			}
		}
		if (curMaxNetId >= getStartNetId() + netIdsPerPlayer) {
			curMaxNetId = getFirstAvailableNetId();
		}
		return (ushort)retId;
	}

	public void setNextActorNetId(ushort curMaxNetId) {
		this.curMaxNetId = curMaxNetId;
	}

	public bool isCrouchHeld() {
		if (character == null) {
			return false;
		}
		if (character is not Axl || Options.main.axlAimMode == 2) {
			return input.isHeld(Control.Down, this);
		}
		if (input.isHeld(Control.AxlCrouch, this)) {
			return true;
		}
		if (!Options.main.axlSeparateAimDownAndCrouch) {
			return input.isHeld(Control.Down, this);
		}
		if (Options.main.axlAimMode == 1) {
			return input.isHeld(Control.Down, this) && !input.isHeld(Control.AimAngleDown, this);
		} else {
			return input.isHeld(Control.Down, this) && !input.isHeld(Control.AimDown, this);
		}
	}

	public void update() {
		readyTime += Global.spf;
		if (readyTime >= maxReadyTime) {
			readyTextOver = true;
		}
		if (Global.level.gameMode.isOver && aiTakeover) {
			aiTakeover = false;
			isAI = false;
			if (character != null) character.ai = null;
		}
		if (!Global.level.gameMode.isOver) {
			respawnTime -= Global.spf;
		}
		if (Global.canControlKillscore && Global.isOnFrame(30)) {
			RPC.updatePlayer.sendRpc(id, kills, deaths);
		}
		if (character == null && respawnTime <= 0 && eliminated()) {
			if (Global.serverClient != null && isMainPlayer) {
				RPC.makeSpectator.sendRpc(id, true);
			} else {
				isSpectator = true;
			}
			return;
		}
		// Evil Energy Timer.
		if (character != null && !character.destroyed && character is Bass) {
			Helpers.decrementFrames(ref evilEnergyTime);
		} 
		if (evilEnergyTime == 0 && evilEnergyStacks > 0) {
			evilEnergyTime = evilEnergyMaxTime;
			evilEnergyStacks--;
			maxHealth += hpPerStack;
			character?.playSound("heal");
		}
		// Never spawn a character if it already exists
		if (character == null && ownedByLocalPlayer) {
			if (!warpedInOnce && firstSpawn == null) {
				firstSpawn = Global.level.getSpawnPoint(this, true);
				Global.level.camX = MathF.Round(firstSpawn.pos.x) - Global.halfScreenW * Global.viewSize;
				Global.level.camY = MathF.Round(firstSpawn.getGroundY()) - Global.halfScreenH * Global.viewSize - 30;

				Global.level.camX = Helpers.clamp(Global.level.camX, 0, Global.level.width - Global.viewScreenW);
				Global.level.camY = Helpers.clamp(Global.level.camY, 0, Global.level.height - Global.viewScreenH);

				Global.level.computeCamPos(
					new Point(
						Global.level.camX + Global.halfScreenW * Global.viewSize,
						Global.level.camY + Global.halfScreenH * Global.viewSize
					),
					null
				);
			}
			if (shouldRespawn()) {
				ushort charNetId = getNextATransNetId();

				if (Global.level.gameMode is TeamDeathMatch or TeamElimination && warpedInOnce) {
					List<Player> spawnPoints = Global.level.players.FindAll(
						p => p.teamAlliance == teamAlliance && p.health > 0 && p.character != null
					);
					if (spawnPoints.Count != 0) {
						Character randomChar = spawnPoints[Helpers.randomRange(0, spawnPoints.Count - 1)].character!;
						Point warpInPos = Global.level.getGroundPosNoKillzone(
							randomChar.pos, Global.screenH
						) ?? randomChar.pos;
						spawnCharAtPoint(
							newCharNum, getCharSpawnData(newCharNum),
							warpInPos, randomChar.xDir, charNetId, true
						);
					} else {
						SpawnPoint spawnPoint = firstSpawn ?? Global.level.getSpawnPoint(this, false);
						firstSpawn = null;
						int spawnPointIndex = Global.level.spawnPoints.IndexOf(spawnPoint);
						spawnCharAtSpawnIndex(spawnPointIndex, charNetId, true);
					}
				}
				else {
					var spawnPoint = Global.level.getSpawnPoint(this, false);
					if (spawnPoint == null) return;
					int spawnPointIndex = Global.level.spawnPoints.IndexOf(spawnPoint);
					spawnCharAtSpawnIndex(spawnPointIndex, charNetId, true);
				}
			}
		}

		updateWeapons();
	}

	public bool eliminated() {
		if (Global.level.gameMode is Elimination || Global.level.gameMode is TeamElimination) {
			if (!isSpectator && (deaths >= Global.level.gameMode.playingTo || (Global.level.isNon1v1Elimination() && serverPlayer?.joinedLate == true))) {
				return true;
			}
		}
		return false;
	}

	public bool shouldRespawn() {
		if (character != null) return false;
		if (respawnTime > 0) return false;
		if (!ownedByLocalPlayer) return false;
		if (isSpectator) return false;
		if (eliminated()) return false;
		if (isAI) return true;
		if (Global.level.is1v1()) return true;
		if (!readyTextOver) return false;
		if (!spawnedOnce) {
			spawnedOnce = true;
			return true;
		}
		if (!Menu.inMenu && input.isPressedMenu(Control.MenuConfirm)) {
			return true;
		}
		if (respawnTime < -10) {
			return true;
		}
		return false;
	}

	public void spawnCharAtSpawnIndex(int spawnPointIndex, ushort charNetId, bool sendRpc) {
		if (Global.level.spawnPoints == null || spawnPointIndex >= Global.level.spawnPoints.Count) {
			return;
		}

		var spawnPoint = Global.level.spawnPoints[spawnPointIndex];

		spawnCharAtPoint(
			newCharNum, getCharSpawnData(newCharNum),
			new Point(spawnPoint.pos.x, spawnPoint.getGroundY()), spawnPoint.xDir, charNetId, sendRpc
		);
	}

	
	public byte[] getCharSpawnData(int charNum) {
		if (ownedByLocalPlayer) {
			if (!isAI) {
				applyLoadoutChange();
			}
			syncLoadout();
		}
		if (charNum == (int)CharIds.Rock) {
			return [
				(byte)loadout.rockLoadout.weapon1,
				(byte)loadout.rockLoadout.weapon2,
				(byte)loadout.rockLoadout.weapon3,
			];
		}
		if (charNum == (int)CharIds.Blues) {
			return [
				(byte)loadout.bluesLoadout.specialWeapon,
			];
		}
		if (charNum == (int)CharIds.Bass) {
			return [
				(byte)loadout.bassLoadout.weapon1,
				(byte)loadout.bassLoadout.weapon2,
				(byte)loadout.bassLoadout.weapon3
			];
		}
		return [];
	}

	public void spawnCharAtPoint(
		int spawnCharNum, byte[] extraData,
		Point pos, int xDir, ushort charNetId, bool sendRpc
	) {
		if (sendRpc) {
			RPC.spawnCharacter.sendRpc(spawnCharNum, extraData, pos, xDir, id, charNetId);
		}

		if (Global.level.gameMode.isTeamMode) {
			alliance = newAlliance;
		}

		if (character != null && charNetId == character.netId) {
			return;
		}
		// ONRESPAWN, SPAWN, RESPAWN, ON RESPAWN, ON SPAWN LOGIC, SPAWNLOGIC
		newCharNum = spawnCharNum;
		charNum = spawnCharNum;
		if (isMainPlayer) {
			previousLoadout = loadout;
			applyLoadoutChange();
		} else if (isAI && Global.level.isTraining()) {
			previousLoadout = loadout;
			applyLoadoutChange();
		}
		if (pendingEvilEnergyStacks > 0 && charNum == (int)CharIds.Bass) {
			evilEnergyStacks = pendingEvilEnergyStacks;
			pendingEvilEnergyStacks = 0;
			evilEnergyTime = evilEnergyMaxTime;
		}

		if (charNum == (int)CharIds.Rock) {
			loadout.rockLoadout.weapon1 = extraData[0];
			loadout.rockLoadout.weapon2 = extraData[1];
			loadout.rockLoadout.weapon3 = extraData[2];

			character = new Rock(
				this, pos.x, pos.y, xDir,
				false, charNetId, ownedByLocalPlayer
			);
		}
		else if (charNum == (int)CharIds.Blues) {
			loadout.bluesLoadout.specialWeapon = extraData[0];

			character = new Blues(
				this, pos.x, pos.y, xDir,
				false, charNetId, ownedByLocalPlayer
			);
		} else if (charNum == (int)CharIds.Bass) {
			loadout.bassLoadout.weapon1 = extraData[0];
			loadout.bassLoadout.weapon2 = extraData[1];
			loadout.bassLoadout.weapon3 = extraData[2];

			character = new Bass(
				this, pos.x, pos.y, xDir,
				false, charNetId, ownedByLocalPlayer
			);
		} else {
			throw new Exception("Error: Non-valid char ID: " + charNum);
		}
		// Do this once char has spawned and is not null.
		configureWeapons();
		lastCharacter = character;

		if (isAI) {
			character.addAI();
		}

		if (isCamPlayer) {
			Global.level.snapCamPos(character.getCamCenterPos(), null);
		}
		warpedIn = true;
	}

	public void startPossess(Player possesser, bool sendRpc = false) {
		possessedTime = maxPossessedTime;
		this.possesser = possesser;
		if (sendRpc) {
			//RPC.possess.sendRpc(possesser.id, id, false);
		}
	}

	public void possesseeUpdate() {
		if (Global.isOnFrameCycle(60) && character != null) {
			character.damageHistory.Add(new DamageEvent(possesser, 136, (int)ProjIds.Sigma2ViralPossess, true, Global.time));
		}

		float myMashValue = mashValue();
		possessedTime -= myMashValue;
		if (possessedTime < 0) {
			possessedTime = 0;
			//RPC.possess.sendRpc(0, id, true);
		}
	}

	public void possesserUpdate() {
		if (character == null || character.destroyed) return;

		// Held section
		input.possessedControlHeld[Control.Left] = Global.input.isHeld(Control.Left, Global.level.mainPlayer);
		input.possessedControlHeld[Control.Right] = Global.input.isHeld(Control.Right, Global.level.mainPlayer);
		input.possessedControlHeld[Control.Up] = Global.input.isHeld(Control.Up, Global.level.mainPlayer);
		input.possessedControlHeld[Control.Down] = Global.input.isHeld(Control.Down, Global.level.mainPlayer);
		input.possessedControlHeld[Control.Jump] = Global.input.isHeld(Control.Jump, Global.level.mainPlayer);
		input.possessedControlHeld[Control.Dash] = Global.input.isHeld(Control.Dash, Global.level.mainPlayer);
		input.possessedControlHeld[Control.Taunt] = Global.input.isHeld(Control.Taunt, Global.level.mainPlayer);

		byte inputHeldByte = Helpers.boolArrayToByte(new bool[] {
				input.possessedControlHeld[Control.Left],
				input.possessedControlHeld[Control.Right],
				input.possessedControlHeld[Control.Up],
				input.possessedControlHeld[Control.Down],
				input.possessedControlHeld[Control.Jump],
				input.possessedControlHeld[Control.Dash],
				input.possessedControlHeld[Control.Taunt],
				false,
		});

		// Pressed section
		input.possessedControlPressed[Control.Left] = Global.input.isPressed(Control.Left, Global.level.mainPlayer);
		input.possessedControlPressed[Control.Right] = Global.input.isPressed(Control.Right, Global.level.mainPlayer);
		input.possessedControlPressed[Control.Up] = Global.input.isPressed(Control.Up, Global.level.mainPlayer);
		input.possessedControlPressed[Control.Down] = Global.input.isPressed(Control.Down, Global.level.mainPlayer);
		input.possessedControlPressed[Control.Jump] = Global.input.isPressed(Control.Jump, Global.level.mainPlayer);
		input.possessedControlPressed[Control.Dash] = Global.input.isPressed(Control.Dash, Global.level.mainPlayer);
		input.possessedControlPressed[Control.Taunt] = Global.input.isPressed(Control.Taunt, Global.level.mainPlayer);

		byte inputPressedByte = Helpers.boolArrayToByte(new bool[] {
				input.possessedControlPressed[Control.Left],
				input.possessedControlPressed[Control.Right],
				input.possessedControlPressed[Control.Up],
				input.possessedControlPressed[Control.Down],
				input.possessedControlPressed[Control.Jump],
				input.possessedControlPressed[Control.Dash],
				input.possessedControlPressed[Control.Taunt],
				false,
		});

		//RPC.syncPossessInput.sendRpc(id, inputHeldByte, inputPressedByte);
	}

	public void unpossess(bool sendRpc = false) {
		possessedTime = 0;
		possesser = null;
		input.possessedControlHeld.Clear();
		input.possessedControlPressed.Clear();
		if (sendRpc) {
			//RPC.possess.sendRpc(0, id, true);
		}
	}

	public bool isPossessed() {
		return possessedTime > 0;
	}

	public bool canBePossessed() {
		if (character == null || character.destroyed) return false;
		if (character.isStatusImmune()) return false;
		if (character.flag != null) return false;
		if (possessedTime > 0) return false;
		return true;
	}

	public bool isMainPlayer {
		get { return Global.level.mainPlayer == this; }
	}

	public bool isCamPlayer {
		get { return this == Global.level.camPlayer; }
	}

	public bool hasArmor() {
		return bodyArmorNum > 0 || legArmorNum > 0 || armArmorNum > 0 || helmetArmorNum > 0;
	}

	public bool hasArmor(int version) {
		return bodyArmorNum == version || legArmorNum == version || armArmorNum == version || helmetArmorNum == version;
	}

	public bool hasAllArmor() {
		return bodyArmorNum > 0 && legArmorNum > 0 && armArmorNum > 0 && helmetArmorNum > 0;
	}

	public bool hasAllX3Armor() {
		return bodyArmorNum >= 3 && legArmorNum >= 3 && armArmorNum >= 3 && helmetArmorNum >= 3;
	}

	public void destroy() {
		character?.destroySelf();
		character = null;
		removeOwnedActors();
	}

	public void removeOwnedActors() {
		foreach (var go in Global.level.gameObjects) {
			if (go is Actor actor && actor.netOwner == this && actor.cleanUpOnPlayerLeave()) {
				actor.destroySelf();
			}
		}
	}

	public void removeOwnedGrenades() {
		for (int i = grenades.Count - 1; i >= 0; i--) {
			grenades[i].destroySelf();
		}
		grenades.Clear();
	}

	public void removeOwnedIceStatues() {
		for (int i = iceStatues.Count - 1; i >= 0; i--) {
			iceStatues[i].destroySelf();
		}
		iceStatues.Clear();
	}

	public void removeOwnedSeeds() {
		for (int i = seeds.Count - 1; i >= 0; i--) {
			seeds[i].destroySelf();
		}
		seeds.Clear();
	}

	public void removeOwnedMechaniloids() {
		for (int i = mechaniloids.Count - 1; i >= 0; i--) {
			mechaniloids[i].destroySelf();
		}
	}

	public int tankMechaniloidCount() {
		return mechaniloids.Count(m => m is Mechaniloid ml && ml.type == MechaniloidType.Tank);
	}

	public int hopperMechaniloidCount() {
		return mechaniloids.Count(m => m is Mechaniloid ml && ml.type == MechaniloidType.Hopper);
	}

	public int birdMechaniloidCount() {
		return mechaniloids.Count(m => m is BirdMechaniloidProj);
	}

	public int fishMechaniloidCount() {
		return mechaniloids.Count(m => m is Mechaniloid ml && ml.type == MechaniloidType.Fish);
	}

	public bool canControl {
		get {
			if (Global.level.gameMode.isOver) {
				return false;
			}
			if (!isAI && Menu.inChat) {
				return false;
			}
			if (!isAI && Menu.inMenu) {
				return false;
			}
			if (character != null && currentMaverick == null) {
				InRideArmor inRideArmor = character.charState as InRideArmor;
				if (inRideArmor != null &&
					(inRideArmor.frozenTime > 0 || inRideArmor.stunTime > 0 || inRideArmor.crystalizeTime > 0)
				) {
					return false;
				}
				if (character.charState is GenericStun) {
					return false;
				}
				if (character.charState is SniperAimAxl) {
					return false;
				}
				if (character.rideArmor?.rideArmorState is RADropIn) {
					return false;
				}
			}
			if (character is BaseSigma sigma && sigma.tagTeamSwapProgress > 0) {
				return false;
			}
			if (isPossessed()) {
				return false;
			}
			/*
			if (character?.charState?.isGrabbedState == true)
			{
				return false;
			}
			*/
			return true;
		}
	}

	public void awardCurrency(bool isKiller = true) {
		if (axlBulletType == (int)AxlBulletWeaponType.AncientGun && isAxl) return;

		// Check for stuff that cannot gain scraps.
		if (character is RagingChargeX or KaiserSigma or ViralSigma or WolfSigma) return;
		if (character?.rideArmor?.raNum == 4 && character.charState is InRideArmor) return;
		if (character is MegamanX mmx && mmx.hasUltimateArmor) {
			return;
		}

		int toAdd = isKiller ? 10 : 5;
		
		if (Global.level?.server?.customMatchSettings != null) {
			currency += Global.level.server.customMatchSettings.currencyGain * toAdd;
		} else {
			currency += toAdd;
		}
	}

	public int getStartCurrency() {
		if (Global.level.levelData.isTraining() || Global.anyQuickStart) {
			return 999;
		}
		if (Global.level?.server?.customMatchSettings != null) {
			return Global.level.server.customMatchSettings.startCurrency;
		}
		return 0;
	}

	public void onKillEffects(bool isAssist) {
		if (!ownedByLocalPlayer || character == null) {
			return; 
		}

		if (!isAssist) {
			if (character is Bass bass && bass.isSuperBass) {
				if (bass.evilEnergy[bass.phase - 1] < Bass.MaxEvilEnergy) {
					bass.evilEnergy[bass.phase - 1] += 7;
				} 
			}
		}
	}

	public int getRespawnTime() {
		if (Global.level.isTraining() || Global.level.isRace()) {
			return 2;
		}
		if (Global.level.gameMode is FFADeathMatch) {
			return 5;
		}
		if (Global.level?.server?.customMatchSettings != null) {
			return Global.level.server.customMatchSettings.respawnTime;
		} else {
			if (Global.level?.gameMode is ControlPoints && alliance == GameMode.redAlliance) {
				return 7;
			}
			if (Global.level?.gameMode is KingOfTheHill) {
				return 9;
			}
		}
		return 7;
	}

	public bool canReviveVile() {
		if (Global.level.isElimination() ||
			!lastDeathCanRevive ||
			newCharNum != 2 ||
			currency < reviveVileCost ||
			lastDeathWasVileMK5
		) {
			return false;
		}
		if (limboChar is not Vile vile || vile.summonedGoliath) {
			return false;
		}
		return true;
	}

	public bool canReviveSigma(out Point spawnPoint) {
		spawnPoint = Point.zero;

		if (Global.level.isHyper1v1() &&
			!lastDeathWasSigmaHyper &&
			limboChar != null && isSigma
			&& newCharNum == 4
		) {
			return true;
		}
		if (limboChar == null ||
			!lastDeathCanRevive ||
			!isSigma ||
			newCharNum != 4 ||
			currency < reviveSigmaCost ||
			lastDeathWasSigmaHyper
		) {
			return false;
		}
		return true;
	}

	public bool canReviveX() {
		return !Global.level.isElimination() && armorFlag == 0 && character?.charState is Die && lastDeathCanRevive && isX && newCharNum == 0 && currency >= reviveXCost && !lastDeathWasXHyper;
	}

	public bool canReviveBlues() {
		return ( 
			character is Blues blues &&
			!blues.isBreakMan &&
			blues.charState is Die dieState &&
			!dieState.respawnTimerOn &&
			lastDeathCanRevive && 
			currency >= Blues.reviveCost
		);
	}

	public void reviveVile(bool toMK5) {
		currency -= reviveVileCost;
		Vile vile = (limboChar as Vile);

		if (toMK5) {
			vileFormToRespawnAs = 2;
		} else if (vile.vileForm == 0) {
			vileFormToRespawnAs = 1;
		} else if (vile.vileForm == 1) {
			vileFormToRespawnAs = 2;
		}

		respawnTime = 0;
		character = limboChar;
		vile.alreadySummonedNewMech = false;
		character.visible = true;
		if (explodeDieEffect != null) {
			explodeDieEffect.destroySelf();
			explodeDieEffect = null;
		}
		limboChar = null;
		vile.rideMenuWeapon = new MechMenuWeapon(VileMechMenuType.All);
		character.changeState(new VileRevive(vileFormToRespawnAs == 2), true);
		RPC.playerToggle.sendRpc(id, vileFormToRespawnAs == 2 ? RPCToggleType.ReviveVileTo5 : RPCToggleType.ReviveVileTo2);
	}

	public void reviveVileNonOwner(bool toMK5) {
		Vile vile = (character as Vile);

		if (toMK5) {
			vileFormToRespawnAs = 2;
		} else {
			vileFormToRespawnAs = 1;
		}

		respawnTime = 0;
		vile.alreadySummonedNewMech = false;
		character.visible = true;
		if (explodeDieEffect != null) {
			explodeDieEffect.destroySelf();
			explodeDieEffect = null;
		}
		character.changeState(new VileRevive(toMK5), true);
	}

	public void reviveSigma(int form, Point spawnPoint) {
		currency -= reviveSigmaCost;
		hyperSigmaRespawn = true;
		respawnTime = 0;
		character = limboChar;
		limboChar = null;
		if (character.destroyed == false) {
			character.destroySelf();
		}
		ushort newNetId = getNextATransNetId();
		if (form == 0) {
			if (Global.level.is1v1()) {
				character.changePos(new Point(Global.level.width / 2, character.pos.y));
			}
			character.changeState(new WolfSigmaRevive(explodeDieEffect), true);
		} else if (form == 1) {
			explodeDieEffect.changeSprite("sigma2_revive");
			character.changeState(new ViralSigmaRevive(explodeDieEffect), true);
		} else {
			KaiserSigma kaiserSigma = new KaiserSigma(
				this, spawnPoint.x, spawnPoint.y, character.xDir, true,
				newNetId, true
			);
			character = kaiserSigma;
			//explodeDieEffect.changeSprite("sigma3_revive");
			if (Global.level.is1v1() && spawnPoint.isZero()) {
				var closestSpawn = Global.level.spawnPoints.OrderBy(
					s => s.pos.distanceTo(character.pos)
				).FirstOrDefault();
				spawnPoint = closestSpawn?.pos ?? new Point(Global.level.width / 2, character.pos.y);
			}
		}
		//RPC.reviveSigma.sendRpc(form, spawnPoint, id, newNetId);
	}

	public void reviveSigmaNonOwner(int form, Point spawnPoint, ushort sigmaNetId) {
		if (form >= 2) {
			character.destroySelf();
			KaiserSigma kaiserSigma = new KaiserSigma(
				this, spawnPoint.x, spawnPoint.y, character.xDir, true,
				sigmaNetId, false
			);
			character = kaiserSigma;

			character.changeSprite("kaisersigma_enter", true);
		}
	}

	public void reviveX() {
		currency -= reviveXCost;
		hyperXRespawn = true;
		respawnTime = 0;
		character.changeState(new XRevive(), true);
	}

	public void reviveXNonOwner() {
	}

	public void destroySigmaEffect() {
		ExplodeDieEffect.createFromActor(this, character, 25, 2, false);
	}

	public void destroyCharacter(bool sendRpc = false) {
		if (character == null) {
			return;
		}
		character.destroySelf();
		onCharacterDeath();
		if (sendRpc) {
			RPC.destroyCharacter.sendRpc(this, character);
		}
		character = null;
	}

	public bool destroyCharacter(ushort netId, bool createEffect = false) {
		if (character?.netId != netId) {
			return false;
		}
		character.destroySelf();
		onCharacterDeath();
		character = null;

		return true;
	}

	public void startDeathTimer() {
		respawnTime = getRespawnTime();
		randomTip = Tips.getRandomTip(charNum);
	}

	// Must be called on any character death
	public void onCharacterDeath() {
		delayedNewCharNum = null;
		suicided = false;
		unpossess();
		if (!ownedByLocalPlayer) {
			return;
		}
		if (delayedNewCharNum != null && Global.level.mainPlayer.charNum != delayedNewCharNum.Value) {
			Global.level.mainPlayer.newCharNum = delayedNewCharNum.Value;
			Global.serverClient?.rpc(RPC.switchCharacter, (byte)Global.level.mainPlayer.id, (byte)delayedNewCharNum.Value);
		}
		if (character == null) {
			return;
		}
		foreach (Weapon weapon in character.weapons) {
			if (weapon is MaverickWeapon mw && mw.maverick != null) {
				mw.maverick.changeState(new MExit(mw.maverick.pos, true), true);
			}
		}
	}

	public void maverick1v1Kill() {
		character?.applyDamage(1000, null, null, null, null);
		character?.destroySelf();
		character = null;
		respawnTime = getRespawnTime() * (suicided ? 2 : 1);
		suicided = false;
		randomTip = Tips.getRandomTip(charNum);
		maverick1v1Spawned = false;
	}

	public void forceKill() {
		if (maverick1v1 != null && Global.level.is1v1()) {
			//character?.applyDamage(null, null, 1000, null);
			currentMaverick?.applyDamage(1000, this, character, null, null);
			return;
		}

		if (currentMaverick != null && isTagTeam()) {
			destroyCharacter();
		} else {
			character?.applyDamage(1000, this, character, null, null);
		}
		foreach (var maverick in mavericks) {
			maverick.applyDamage(1000, this, character, null, null);
		}
	}

	public bool isGridModeEnabled() {
		if (isAxl || isDisguisedAxl) {
			if (Options.main.useMouseAim) return false;
			if (Global.level.is1v1()) return Options.main.gridModeAxl > 0;
			return Options.main.gridModeAxl > 1;
		} else if (isX) {
			if (Global.level.is1v1()) return Options.main.gridModeX > 0;
			return Options.main.gridModeX > 1;
		} else if (isRock) {
			if (Global.level.is1v1()) return Options.main.gridModeRock > 0;
			return Options.main.gridModeRock > 1;
		} else if (isBass) {
			if (Global.level.is1v1()) return Options.main.gridModeBass > 0;
			return Options.main.gridModeBass > 1;
		}

		return false;
	}

	public Point[] gridModePoints() {
		if (weapons.Count < 2) return null;
		if (weapons.Count == 2) return new Point[] { new Point(0, 0), new Point(-1, 0) };
		if (weapons.Count == 3) return new Point[] { new Point(0, 0), new Point(-1, 0), new Point(1, 0) };
		if (weapons.Count == 4) return new Point[] { new Point(0, 0), new Point(-1, 0), new Point(1, 0), new Point(0, -1) };
		if (weapons.Count == 5) return new Point[] { new Point(0, 0), new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1) };
		if (weapons.Count == 6) return new Point[] { new Point(0, 0), new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1), new Point(-1, -1) };
		if (weapons.Count == 7) return new Point[] { new Point(0, 0), new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1), new Point(-1, -1), new Point(1, -1) };
		if (weapons.Count == 8) return new Point[] { new Point(0, 0), new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1), new Point(-1, -1), new Point(1, -1), new Point(-1, 1) };
		if (weapons.Count == 9) return new Point[] { new Point(0, 0), new Point(-1, -1), new Point(0, -1), new Point(1, -1), new Point(-1, 0), new Point(1, 0), new Point(-1, 1), new Point(0, 1), new Point(1, 1) };
		if (weapons.Count >= 10) return new Point[] { new Point(0, 0), new Point(-1, -1), new Point(0, -1), new Point(1, -1), new Point(-1, 0), new Point(1, 0), new Point(-1, 1), new Point(0, 1), new Point(1, 1), new Point(2, 1) };
		return null;
	}

	// 0000 0000 0000 0000 [boots][body][helmet][arm]
	// 0000 = none, 0001 = x1, 0010 = x2, 0011 = x3, 1111 = chip
	public static int getArmorNum(int armorFlag, int armorIndex, bool isChipCheck) {
		List<string> bits = Convert.ToString(armorFlag, 2).Select(s => s.ToString()).ToList();
		while (bits.Count < 16) {
			bits.Insert(0, "0");
		}

		string bitStr = "";
		if (armorIndex == 0) bitStr = bits[0] + bits[1] + bits[2] + bits[3];
		if (armorIndex == 1) bitStr = bits[4] + bits[5] + bits[6] + bits[7];
		if (armorIndex == 2) bitStr = bits[8] + bits[9] + bits[10] + bits[11];
		if (armorIndex == 3) bitStr = bits[12] + bits[13] + bits[14] + bits[15];

		int retVal = Convert.ToInt32(bitStr, 2);
		if (retVal > 3 && !isChipCheck) retVal = 3;
		return retVal;
	}

	public void setArmorNum(int armorIndex, int val) {
		List<string> bits = Convert.ToString(armorFlag, 2).Select(s => s.ToString()).ToList();
		while (bits.Count < 16) {
			bits.Insert(0, "0");
		}

		List<string> valBits = Convert.ToString(val, 2).Select(s => s.ToString()).ToList();
		while (valBits.Count < 4) {
			valBits.Insert(0, "0");
		}

		int i = armorIndex * 4;
		bits[i] = valBits[0];
		bits[i + 1] = valBits[1];
		bits[i + 2] = valBits[2];
		bits[i + 3] = valBits[3];

		armorFlag = Convert.ToUInt16(string.Join("", bits), 2);
	}

	public int legArmorNum {
		get { return getArmorNum(armorFlag, 0, false); }
		set { setArmorNum(0, value); }
	}
	public int bodyArmorNum {
		get { return getArmorNum(armorFlag, 1, false); }
		set { setArmorNum(1, value); }
	}
	public int helmetArmorNum {
		get { return getArmorNum(armorFlag, 2, false); }
		set { setArmorNum(2, value); }
	}
	public int armArmorNum {
		get { return getArmorNum(armorFlag, 3, false); }
		set { setArmorNum(3, value); }
	}

	public bool hasBootsArmor(ArmorId armorId) { return legArmorNum == (int)armorId; }
	public bool hasBodyArmor(ArmorId armorId) { return bodyArmorNum == (int)armorId; }
	public bool hasHelmetArmor(ArmorId armorId) { return helmetArmorNum == (int)armorId; }
	public bool hasArmArmor(ArmorId armorId) { return armArmorNum == (int)armorId; }

	public bool hasBootsArmor(int xGame) { return legArmorNum == xGame; }
	public bool hasBodyArmor(int xGame) { return bodyArmorNum == xGame; }
	public bool hasHelmetArmor(int xGame) { return helmetArmorNum == xGame; }
	public bool hasArmArmor(int xGame) { return armArmorNum == xGame; }

	public bool isHeadArmorPurchased(int xGame) { return headArmorsPurchased[xGame - 1]; }
	public bool isBodyArmorPurchased(int xGame) { return bodyArmorsPurchased[xGame - 1]; }
	public bool isArmArmorPurchased(int xGame) { return armArmorsPurchased[xGame - 1]; }
	public bool isBootsArmorPurchased(int xGame) { return bootsArmorsPurchased[xGame - 1]; }

	public void setHeadArmorPurchased(int xGame) { headArmorsPurchased[xGame - 1] = true; }
	public void setBodyArmorPurchased(int xGame) { bodyArmorsPurchased[xGame - 1] = true; }
	public void setArmArmorPurchased(int xGame) { armArmorsPurchased[xGame - 1] = true; }
	public void setBootsArmorPurchased(int xGame) { bootsArmorsPurchased[xGame - 1] = true; }

	public bool hasAllArmorsPurchased() {
		for (int i = 0; i < 3; i++) {
			if (!headArmorsPurchased[i]) return false;
			if (!bodyArmorsPurchased[i]) return false;
			if (!armArmorsPurchased[i]) return false;
			if (!bootsArmorsPurchased[i]) return false;
		}
		return true;
	}

	public bool hasAnyArmorPurchased() {
		for (int i = 0; i < 3; i++) {
			if (headArmorsPurchased[i]) return true;
			if (bodyArmorsPurchased[i]) return true;
			if (armArmorsPurchased[i]) return true;
			if (bootsArmorsPurchased[i]) return true;
		}
		return false;
	}

	public bool hasAnyArmor() {
		return legArmorNum > 0 || armArmorNum > 0 || bodyArmorNum > 0 || helmetArmorNum > 0;
	}

	public void press(string inputMapping) {
		string keyboard = "keyboard";
		int? control = Control.controllerNameToMapping[keyboard].GetValueOrDefault(inputMapping);
		if (control == null) return;
		Key key = (Key)control;
		input.keyPressed[key] = !input.keyHeld.GetValueOrDefault(key);
		input.keyHeld[key] = true;
	}

	public void release(string inputMapping) {
		string keyboard = "keyboard";
		int? control = Control.controllerNameToMapping[keyboard].GetValueOrDefault(inputMapping);
		if (control == null) return;
		Key key = (Key)control;
		input.keyHeld[key] = false;
		input.keyPressed[key] = false;
	}

	public void clearAiInput() {
		input.keyHeld.Clear();
		input.keyPressed.Clear();
		if (character != null && character.ai.framesChargeHeld > 0) {
			press("shoot");
		}
		if (character != null) {
			if (character.ai.jumpTime > 0) {
				press("jump");
			} else {
				release("jump");
			}
		}
	}

	public bool dashPressed(out string dashControl) {
		dashControl = "";
		if (input.isPressed(Control.Dash, this)) {
			dashControl = Control.Dash;
			return true;
		} else if (!Options.main.disableDoubleDash) {
			if (input.isPressed(Control.Left, this) && input.checkDoubleTap(Control.Left)) {
				dashControl = Control.Left;
				return true;
			} else if (input.isPressed(Control.Right, this) && input.checkDoubleTap(Control.Right)) {
				dashControl = Control.Right;
				return true;
			}
		}
		return false;
	}

	public void promoteToHost() {
		if (this == Global.level.mainPlayer) {
			Global.serverClient.isHost = true;
			if (Global.level?.gameMode != null) {
				Global.level.gameMode.chatMenu.addChatEntry(new ChatEntry("You were promoted to host.", null, null, true));
			}
			if (Global.level?.redFlag != null) {
				Global.level.redFlag.takeOwnership();
				Global.level.redFlag.pedestal?.takeOwnership();
			}
			if (Global.level?.blueFlag != null) {
				Global.level.blueFlag.takeOwnership();
				Global.level.blueFlag.pedestal?.takeOwnership();
			}
			foreach (var cp in Global.level.controlPoints) {
				cp.takeOwnership();
			}
			Global.level?.hill?.takeOwnership();

			foreach (var player in Global.level.players) {
				if (player.serverPlayer.isBot) {
					player.ownedByLocalPlayer = true;
					player.isAI = true;
					player.character?.addAI();
					player.character?.takeOwnership();
				}
			}
		} else {
			Global.level.gameMode.chatMenu.addChatEntry(new ChatEntry(name + " promoted to host.", null, null, true));
		}
	}

	public void addKill() {
		if (Global.serverClient == null) {
			kills++;
		} else if (Global.canControlKillscore) {
			kills++;
			if (!charNumToKills.ContainsKey(realCharNum)) {
				charNumToKills[realCharNum] = 0;
			}
			charNumToKills[realCharNum]++;
			RPC.updatePlayer.sendRpc(id, kills, deaths);
		}
	}

	public void addAssist() {
		assists++;
	}

	public void addDeath(bool isSuicide) {
		if (isSigma && maverick1v1 == null && Global.level.isHyper1v1() && !lastDeathWasSigmaHyper) {
			return;
		}

		if (isSuicide) {
			kills--;
			currency -= 5;

			if (!charNumToKills.ContainsKey(realCharNum)) {
				charNumToKills[realCharNum] = 0;
			}
			charNumToKills[realCharNum]--;
		}

		if (Global.serverClient == null) {
			deaths++;
		} else if (Global.canControlKillscore) {
			deaths++;
			RPC.updatePlayer.sendRpc(id, kills, deaths);
		}
	}

	public float mashValue() {
		int mashCount = input.mashCount;
		if (isAI && character?.ai != null) {
			if (character.ai.mashType == 1) {
				mashCount = Helpers.randomRange(0, 6) == 0 ? 1 : 0;
			} else if (character.ai.mashType == 2) {
				mashCount = Helpers.randomRange(0, 3) == 0 ? 1 : 0;
			}
		}

		float healthPercent = 0.3333f + ((health / maxHealth) * 0.6666f);
		float mashAmount = (healthPercent * mashCount * 0.25f);

		if (Global.frameCount - lastMashAmountSetFrame > 10) {
			lastMashAmount = 0;
		}

		float prevLastMashAmount = lastMashAmount;
		lastMashAmount += mashAmount;
		if (mashAmount > 0 && prevLastMashAmount == 0) {
			lastMashAmountSetFrame = Global.frameCount;
		}

		return (Global.spf + mashAmount);
	}

	// Sigma helper functions

	public bool isSigma1AndSigma() {
		return isSigma1() && isSigma;
	}

	public bool isSigma2AndSigma() {
		return isSigma2() && isSigma;
	}

	public bool isSigma3AndSigma() {
		return isSigma3() && isSigma;
	}

	public bool isSigma1() {
		return loadout?.sigmaLoadout?.sigmaForm == 0;
	}

	public bool isSigma2() {
		return loadout?.sigmaLoadout?.sigmaForm == 1;
	}

	public bool isSigma3() {
		return loadout?.sigmaLoadout?.sigmaForm == 2;
	}

	public bool isSigma1Or3() {
		return isSigma1() || isSigma3();
	}

	public bool isWolfSigma() {
		return character is WolfSigma;
	}

	public bool isViralSigma() {
		return character is ViralSigma;
	}

	public bool isKaiserSigma() {
		return character is KaiserSigma;
	}

	public bool isKaiserViralSigma() {
		return character != null && character.sprite.name.StartsWith("kaisersigma_virus");
	}

	public bool isKaiserNonViralSigma() {
		return isKaiserSigma() && !isKaiserViralSigma();
	}

	public bool isSummoner() {
		return isAI || loadout?.sigmaLoadout != null && loadout.sigmaLoadout.commandMode == 0;
	}

	public bool isPuppeteer() {
		return !isAI && loadout?.sigmaLoadout != null && loadout.sigmaLoadout.commandMode == 1;
	}

	public bool isStriker() {
		return !isAI && loadout?.sigmaLoadout != null && loadout.sigmaLoadout.commandMode == 2;
	}

	public bool isTagTeam() {
		return !isAI && loadout?.sigmaLoadout != null && loadout.sigmaLoadout.commandMode == 3;
	}

	public bool isRefundableMode() {
		return isSummoner() || isPuppeteer() || isTagTeam();
	}

	public bool isAlivePuppeteer() {
		return isPuppeteer() && health > 0;
	}

	public bool isControllingPuppet() {
		return isSigma && isPuppeteer() && currentMaverick != null && weapon is MaverickWeapon;
	}

	public bool hasETankCapacity() {
		var Etanks = this.ETanks;
		for (int i = 0; i < Etanks.Count; i++) {
			if (Etanks[i].health < ETank.maxHealth) {
				return true;
			}
		} 
		return false;
	}

	public bool canUseEtank(ETank etank) {
		if (isDead) return false;
		if (character.healAmount > 0) return false;
		if (health <= 0 || health >= maxHealth) return false;
		if (etank.health <= 0) return false;
		if (character.charState is WarpOut) return false;
		if (character.charState.invincible) return false;
		if (character is MegamanX { stingActiveTime: >0 }) return false;
		// TODO: Add Wolf Check here.
		//if (character.isHyperSigmaBS.getValue()) return false;

		return true;
	}

	public bool canUseWTank(WTank wtank) {
		if (isDead) return false;
		if (character.charState is WarpOut) return false;
		if (character.charState.invincible) return false;
		//if (weaponToHeal.ammo >= weaponToHeal.maxAmmo) return false;
		//if (weapon.ammo == weapon.maxAmmo) return false;
		//if (weapon is not RockBuster) return true;
		// (weapon is not SARocketPunch) return true;
		
		return true;
	}

	public bool canUseLTank(LTank ltank) {
		if (character is Blues blues && character != null) {
			if (isDead) return false;
			if (blues?.charState is WarpOut) return false;
			if (blues != null && blues.charState.invincible) return false;
			if (blues?.charState is OverheatShutdown or
				OverheatShutdownStart) return false;
			if (health >= maxHealth && blues?.shieldHP >= blues?.shieldMaxHP && blues?.coreAmmo <= 0) return false;
			return true;
		} 
		return false;
	}

	public bool canUseMTank(MTank mtank) {
		if (isDead) return false;
		if (character.charState is WarpOut) return false;
		if (character.charState.invincible) return false;

		return true;
	}

	public void fillETank(float amount) {
		if (character?.healAmount > 0) return;
		var etanks = this.ETanks;
		for (int i = 0; i < etanks.Count; i++) {
			if (etanks[i].health < ETank.maxHealth) {
				etanks[i].health += amount;
				if (etanks[i].health >= ETank.maxHealth) {
					etanks[i].health = ETank.maxHealth;
					//if (isMainPlayer) Global.playSound("subtankFull");
				} else {
					if (isMainPlayer) Global.playSound("subtank_fill");
				}
				break;
			}
		}
	}

	public bool isUsingETank() {
		return character?.usedEtank != null;
	}

	public int getSpawnIndex(int spawnPointCount) {
		var nonSpecPlayers = Global.level.nonSpecPlayers();
		nonSpecPlayers = nonSpecPlayers.OrderBy(p => p.id).ToList();
		int index = nonSpecPlayers.IndexOf(this) % spawnPointCount;
		if (index < 0) {
			index = 0;
		}
		return index;
	}

	public void delayETank() {
		if (isMainPlayer) {
			UpgradeMenu.eTankDelay = UpgradeMenu.maxETankDelay;
		}
	}

	public void delayLTank() {
		if (isMainPlayer) {
			BluesUpgradeMenu.lTankDelay = BluesUpgradeMenu.maxLTankDelay;
		}
	}


	public void stopETankHeal() {
		if (character != null && character.eTankHealAmount > 0) character.eTankHealAmount = 0;
	}

	public void delayWTank() {
		if (isMainPlayer) {
			UpgradeMenu.wTankDelay = UpgradeMenu.maxWTankDelay;
		}
	}
}

[ProtoContract]
public class Disguise {
	[ProtoMember(1)]
	public string targetName { get; set; }

	public Disguise() { }

	public Disguise(string name) {
		targetName = name;
	}
}
