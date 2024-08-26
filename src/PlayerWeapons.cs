﻿using System;
using System.Collections.Generic;
using System.Linq;
using static SFML.Window.Keyboard;

namespace MMXOnline;

public partial class Player {
	public List<Weapon> weapons = new();
	public List<Weapon> oldWeapons = new();

	public Weapon nonOwnerWeapon;

	public int prevWeaponSlot;
	private int _weaponSlot;
	public int weaponSlot {
		get {
			return _weaponSlot;
		}
		set {
			if (_weaponSlot != value) {
				prevWeaponSlot = _weaponSlot;
				_weaponSlot = value;
			}
		}
	}

	public Weapon weapon {
		get {
			if (ownedByLocalPlayer) {
				if (weapons.InRange(weaponSlot)) {
					return weapons[weaponSlot];
				}
				return new Weapon();
			}
			return nonOwnerWeapon ?? new Weapon();
		}
	}

	public Weapon lastHudWeapon = null;

	public AxlWeapon? axlWeapon {
		get {
			return weapon as AxlWeapon;
		}
	}

	public MaverickWeapon? maverickWeapon {
		get { return weapon as MaverickWeapon; }
	}

	public List<Maverick> mavericks {
		get {
			var mavericks = new List<Maverick>();
			foreach (var weapon in weapons) {
				if (weapon is MaverickWeapon mw && mw.maverick != null) {
					mavericks.Add(mw.maverick);
				}
			}
			return mavericks;
		}
	}

	public MaverickWeapon? currentMaverickWeapon {
		get {
			foreach (var weapon in weapons) {
				if (weapon is MaverickWeapon mw && mw.maverick != null && mw.maverick == currentMaverick) {
					return mw;
				}
			}
			return null;
		}
	}

	public Maverick? currentMaverick {
		get {
			var mw = weapons.FirstOrDefault(
				w => w is MaverickWeapon mw && mw.maverick?.aiBehavior == MaverickAIBehavior.Control
			);
			return (mw as MaverickWeapon)?.maverick;
		}
	}

	public bool shouldBlockMechSlotScroll() {
		if (character is Vile { isVileMK5: true, startRideArmor: not null }) {
			return false;
		}
		return Options.main.blockMechSlotScroll;
		
	}

	public bool gridModeHeld;
	public Point gridModePos = new Point();
	public void changeWeaponControls() {
		if (character == null) return;
		if (!character.canChangeWeapons()) return;

		if (isGridModeEnabled() && weapons.Count > 1) {
			if (input.isWeaponLeftOrRightHeld(this)) {
				gridModeHeld = true;
				if (input.isPressedMenu(Control.Up)) gridModePos.y--;
				else if (input.isPressedMenu(Control.Down)) gridModePos.y++;

				if (input.isPressedMenu(Control.Left)) gridModePos.x--;
				else if (input.isPressedMenu(Control.Right)) gridModePos.x++;

				if (gridModePos.y < -1) gridModePos.y = -1;
				if (gridModePos.y > 1) gridModePos.y = 1;
				if (gridModePos.x < -1) gridModePos.y = -1;
				if (gridModePos.x > 1) gridModePos.x = 1;

				var gridPoints = gridModePoints();

				for (int i = 0; i < weapons.Count; i++) {
					if (i >= gridPoints.Length) break;
					var gridPoint = gridPoints[i];
					if (gridModePos.x == gridPoint.x && gridModePos.y == gridPoint.y && weapons.Count >= i + 1) {
						changeWeaponSlot(i);
					}
				}
			} else {
				gridModeHeld = false;
				gridModePos = new Point();
			}
			return;
		}

		if ((isAxl || isDisguisedAxl) && isMainPlayer) {
			if (Input.mouseScrollUp) {
				weaponLeft();
				return;
			} else if (Input.mouseScrollDown) {
				weaponRight();
				return;
			}
		}

		if (input.isPressed(Control.WeaponLeft, this)) {
			if (isDisguisedAxl && isZero && input.isHeld(Control.Down, this)) return;
			weaponLeft();
		} else if (input.isPressed(Control.WeaponRight, this)) {
			if (isDisguisedAxl && isZero && input.isHeld(Control.Down, this)) return;
			weaponRight();
		} else if (character != null && !Control.isNumberBound(realCharNum, Options.main.axlAimMode)) {
			if (isVile && weapon is MechMenuWeapon mmw &&
				character.startRideArmor == null &&
				shouldBlockMechSlotScroll()
			) {
				if (input.isPressed(Key.Num1, canControl)) {
					selectedRAIndex = 0;
					character.onMechSlotSelect(mmw);
				} else if (input.isPressed(Key.Num2, canControl)) {
					selectedRAIndex = 1;
					character.onMechSlotSelect(mmw);
				} else if (input.isPressed(Key.Num3, canControl)) {
					selectedRAIndex = 2;
					character.onMechSlotSelect(mmw);
				} else if (input.isPressed(Key.Num4, canControl)) {
					selectedRAIndex = 3;
					character.onMechSlotSelect(mmw);
				} else if (input.isPressed(Key.Num5, canControl)) {
					selectedRAIndex = 4;
					character.onMechSlotSelect(mmw);
				}
			} else {
				if (input.isPressed(Key.Num1, canControl) && weapons.Count >= 1) {
					changeWeaponSlot(0);
					if (isVile && weapon is MechMenuWeapon mmw2 && shouldBlockMechSlotScroll()) {
						character.onMechSlotSelect(mmw2);
					}
				} else if (input.isPressed(Key.Num2, canControl) && weapons.Count >= 2) {
					changeWeaponSlot(1);
					if (isVile && weapon is MechMenuWeapon mmw2 && shouldBlockMechSlotScroll()) {
						character.onMechSlotSelect(mmw2);
					}
				} else if (input.isPressed(Key.Num3, canControl) && weapons.Count >= 3) {
					changeWeaponSlot(2);
					if (isVile && weapon is MechMenuWeapon mmw2 && shouldBlockMechSlotScroll()) {
						character.onMechSlotSelect(mmw2);
					}
				} else if (input.isPressed(Key.Num4, canControl) && weapons.Count >= 4) { changeWeaponSlot(3); } else if (input.isPressed(Key.Num5, canControl) && weapons.Count >= 5) { changeWeaponSlot(4); } else if (input.isPressed(Key.Num6, canControl) && weapons.Count >= 6) { changeWeaponSlot(5); } else if (input.isPressed(Key.Num7, canControl) && weapons.Count >= 7) { changeWeaponSlot(6); } else if (input.isPressed(Key.Num8, canControl) && weapons.Count >= 8) { changeWeaponSlot(7); } else if (input.isPressed(Key.Num9, canControl) && weapons.Count >= 9) { changeWeaponSlot(8); } else if (input.isPressed(Key.Num0, canControl) && weapons.Count >= 10) { changeWeaponSlot(9); }
			}
		}
	}

	public void changeWeaponFromWi(int weaponIndex) {
		nonOwnerWeapon = weapons.FirstOrDefault(w => w.index == weaponIndex) ?? nonOwnerWeapon;
	}

	public void changeToSigmaSlot() {
		for (int i = 0; i < weapons.Count; i++) {
			if (weapons[i] is SigmaMenuWeapon) {
				changeWeaponSlot(i);
				return;
			}
		}
	}

	public void removeWeaponSlot(int index) {
		if (index < 0 || index >= weapons.Count) return;
		if (index < weaponSlot && weaponSlot > 0) weaponSlot--;
		for (int i = weapons.Count - 1; i >= 0; i--) {
			if (i == index) {
				weapons.RemoveAt(i);
				return;
			}
		}
	}

	public void changeWeaponSlot(int newWeaponSlot) {
		if (weaponSlot == newWeaponSlot) return;
		if (isDead) return;
		if (!weapons.InRange(newWeaponSlot)) return;
		if (weapons[newWeaponSlot].index == (int)WeaponIds.MechMenuWeapon) {
			selectedRAIndex = 0;
		}

		if (isX) {
			character.stingChargeTime = 0;
		}

		Weapon oldWeapon = weapon;
		if (oldWeapon is MechMenuWeapon mmw) {
			mmw.isMenuOpened = false;
		}

		weaponSlot = newWeaponSlot;
		Weapon newWeapon = weapon;

		if (newWeapon is MaverickWeapon mw) {
			mw.selCommandIndex = 1;
			mw.selCommandIndexX = 1;
			mw.isMenuOpened = false;
		}

		if (isX) {
			if (character.getChargeLevel() >= 2) {
				newWeapon.shootTime = 0;
			} else {
				// Switching from laggy move (like tornado) to a fast one
				if (oldWeapon.switchCooldown != null && oldWeapon.shootTime > 0) {
					newWeapon.shootTime = Math.Max(newWeapon.shootTime, oldWeapon.switchCooldown.Value);
				} else {
					newWeapon.shootTime = Math.Max(newWeapon.shootTime, oldWeapon.shootTime);
				}
				/*
				if (newWeapon is NovaStrike ns) {
					ns.shootTime = 0;
				}
				*/
			}
		}

		if (character is Axl axl) {
			if (oldWeapon is AxlWeapon) {
				axl.axlSwapTime = axl.switchTime;
				axl.axlAltSwapTime = axl.altSwitchTime;
			}
			if (axl.isZooming()) {
				axl.zoomOut();
			}
		}

		if (isRock) {
			if (character.getChargeLevel() >= 2) {
				newWeapon.shootTime = 0;
			} else {
				// Switching from laggy move (like tornado) to a fast one
				if (oldWeapon.switchCooldown != null && oldWeapon.shootTime > 0) {
					newWeapon.shootTime = Math.Max(newWeapon.shootTime, oldWeapon.switchCooldown.Value);
				} else {
					newWeapon.shootTime = Math.Max(newWeapon.shootTime, oldWeapon.shootTime);
				}
			}
		}
	}

	public void weaponLeft() {
		int ws = weaponSlot - 1;
label:
		if (ws < 0) {
			ws = weapons.Count - 1;
			if (shouldBlockMechSlotScroll() && isVile && weapons.ElementAtOrDefault(ws) is MechMenuWeapon) {
				ws--;
				if (ws < 0) ws = 0;
			}
		}
		if ((weapons.ElementAtOrDefault(ws) is RushWeapon && Options.main.rushSpecial) || (weapons.ElementAtOrDefault(ws) is NovaStrike && Options.main.novaStrikeSpecial)) {
			ws--;
			goto label;
		}
		changeWeaponSlot(ws);
	}

	public void weaponRight() {
		int ws = weaponSlot + 1;
label:
		int max = weapons.Count;
		if (shouldBlockMechSlotScroll() && isVile && weapons.ElementAtOrDefault(max - 1) is MechMenuWeapon) {
			max--;
		}
		if (ws >= max) {
			ws = 0;
		}
		if ((weapons.ElementAtOrDefault(ws) is RushWeapon && Options.main.rushSpecial) || (weapons.ElementAtOrDefault(ws) is NovaStrike && Options.main.novaStrikeSpecial)) {
			ws++;
			goto label;
		}
		changeWeaponSlot(ws);
	}

	public void clearSigmaWeapons() {
		preSigmaReviveWeapons = new List<Weapon>(weapons);
		weapons.Clear();
	}

	public List<Weapon>? preSigmaReviveWeapons;
	public void configureWeapons() {
		weapons = new List<Weapon>();

		if (ownedByLocalPlayer) {
			if (isRock) {
				Rock rock = character as Rock;
				if (Global.level.isTraining() && !Global.level.server.useLoadout) {
					weapons = Weapon.getAllRockWeapons().Select(w => w.clone()).ToList();
				}  else if (!Global.level.is1v1() && !Global.level.isTraining() && Options.main.useRandomRockLoadout) {
					weapons = getRandomRockLoadout().Select(wep => wep.clone()).ToList();
				} else if (Global.level.is1v1()) {
					weapons = Weapon.getAllMM7Weapons().Select(w => w.clone()).ToList();
				}
				else {
					weapons = loadout.rockLoadout.getWeaponsFromLoadout(this);
					//weapons.Add(loadout.rockLoadout.getRushFromLoadout(this));
				}
			} else if (isBass) {
				//weapons = Bass.getAllWeapons().Select(w => w.clone()).ToList();
			}
		}
	}

	private Weapon getAxlBullet(int axlBulletType) {
		if (axlBulletType == (int)AxlBulletWeaponType.DoubleBullets) {
			return new DoubleBullet();
		}
		return new AxlBullet((AxlBulletWeaponType)axlBulletType);
	}

	public Weapon getAxlBulletWeapon() {
		return getAxlBulletWeapon(axlBulletType);
	}

	public Weapon getAxlBulletWeapon(int type) {
		if (type == (int)AxlBulletWeaponType.DoubleBullets) {
			return new DoubleBullet();
		} else {
			return new AxlBullet((AxlBulletWeaponType)type);
		}
	}

	public int getLastWeaponIndex() {
		int miscSlots = 0;
		if (weapons.Any(w => w is GigaCrush)) miscSlots++;
		if (weapons.Any(w => w is HyperBuster)) miscSlots++;
		if (weapons.Any(w => w is NovaStrike)) miscSlots++;
		return weapons.Count - miscSlots;
	}

	public void addGigaCrush() {
		if (!weapons.Any(w => w is GigaCrush)) {
			weapons.Add(new GigaCrush());
		}
	}

	public void addHyperCharge() {
		if (!weapons.Any(w => w is HyperBuster)) {
			weapons.Insert(getLastWeaponIndex(), new HyperBuster());
		}
	}

	public void addNovaStrike() {
		if (!weapons.Any(w => w is NovaStrike)) {
			weapons.Add(new NovaStrike(this));
		}
	}

	public void removeNovaStrike() {
		if (weapon is NovaStrike) {
			weaponSlot = 0;
		}
		weapons.RemoveAll(w => w is NovaStrike);
	}

	public void removeHyperCharge() {
		if (weapon is HyperBuster) {
			weaponSlot = 0;
		}
		weapons.RemoveAll(w => w is HyperBuster);
	}

	public void removeGigaCrush() {
		if (weapon is GigaCrush) {
			weaponSlot = 0;
		}
		weapons.RemoveAll(w => w is GigaCrush);
	}

	public void addSARocketPunch() {
		if (!weapons.Any(w => w is SARocketPunch)) {
			weapons.Add(new SARocketPunch());
		}
	}

	public void removeSARocketPunch() {
		if (weapon is SARocketPunch) {
			weaponSlot = 0;
		}
		weapons.RemoveAll(w => w is SARocketPunch);
	}

	public void removeWeaponsButBuster() {
		weaponSlot = 0;
		weapons.RemoveAll(w => w is not RockBuster);
		if (!weapons.Any(w => w is RockBuster)) weapons.Add(new RockBuster());
	}

	public void updateWeapons() {
		foreach (var weapon in weapons) {
			weapon.update();
			if (character != null && health > 0) {
				bool alwaysOn = false;
				if (weapon is GigaCrush && Options.main.gigaCrushSpecial ||
					weapon is NovaStrike && Options.main.novaStrikeSpecial
				) {
					alwaysOn = true;
				}
				weapon.charLinkedUpdate(character, alwaysOn);
			}
		}
	}

	//-------------------RANDOM LOADOUT FEATURE (VIA SETTINGS)--------------------------------

	public static List<Weapon> getRandomXLoadout() {
		Random slot0 = new Random(), slot1 = new Random(), slot2 = new Random();
		bool hasRepeatedWeapons = true;
		int[] weaponIndexes = new int[3];
		var indices = new List<byte>();

		while (hasRepeatedWeapons) {
			weaponIndexes[0] = slot0.Next(0, 25);
			weaponIndexes[1] = slot1.Next(0, 25);
			weaponIndexes[2] = slot2.Next(0, 25);

			if (weaponIndexes[0] != weaponIndexes[1] 
				&& weaponIndexes[0] != weaponIndexes[2] 
				&& weaponIndexes[1] != weaponIndexes[2]) hasRepeatedWeapons = false;
		}

		indices.Add((byte)weaponIndexes[0]);
		indices.Add((byte)weaponIndexes[1]);
		indices.Add((byte)weaponIndexes[2]);

		return indices.Select(index => {
			return Weapon.getAllXWeapons().Find(w => w.index == index).clone();
		}).ToList();;
	}

	public static List<Weapon> getRandomRockLoadout() {
		Random slot0 = new Random(), slot1 = new Random(), slot2 = new Random();
		bool hasRepeatedWeapons = true;
		int[] weaponIndexes = new int[3];
		var indices = new List<byte>();

		while (hasRepeatedWeapons) {
			weaponIndexes[0] = slot0.Next(0, 9);
			weaponIndexes[1] = slot1.Next(0, 9);
			weaponIndexes[2] = slot2.Next(0, 9);

			if (weaponIndexes[0] != weaponIndexes[1] 
				&& weaponIndexes[0] != weaponIndexes[2] 
				&& weaponIndexes[1] != weaponIndexes[2]) hasRepeatedWeapons = false;
		}

		indices.Add((byte)weaponIndexes[0]);
		indices.Add((byte)weaponIndexes[1]);
		indices.Add((byte)weaponIndexes[2]);

		return indices.Select(index => {
			return Weapon.getAllRockWeapons().Find(w => w.index == index).clone();
		}).ToList();;
	}
}
