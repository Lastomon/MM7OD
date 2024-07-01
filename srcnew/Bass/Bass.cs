using System;
using System.Collections.Generic;

namespace MMXOnline;

public class Bass : Character {
	float weaponCooldown;
	public CopyVisionClone? cVclone;
	public SpreadDrillProj? sDrill;
	public SpreadDrillMediumProj? sDrillM;

	public Bass(
		Player player, float x, float y, int xDir,
		bool isVisible, ushort? netId, bool ownedByLocalPlayer,
		bool isWarpIn = true
	) : base(
		player, x, y, xDir, isVisible, netId, ownedByLocalPlayer, isWarpIn, false, false
	) {
		charId = CharIds.Bass;
	}

	public override void update() {
		base.update();
		Helpers.decrementFrames(ref weaponCooldown);

		// Shoot controls.
		bool shootPressed;
		if (player.weapon.isStream) {
			shootPressed = player.input.isHeld(Control.Shoot, player);
		} else {
			shootPressed = player.input.isPressed(Control.Shoot, player);
		};
		if (shootPressed) {
			lastShootPressed = Global.frameCount;
		}
		player.changeWeaponControls();
	}

	public override bool attackCtrl() {
		int framesSinceLastShootPressed = Global.frameCount - lastShootPressed;
		if (framesSinceLastShootPressed <= 6) {
			if (weaponCooldown <= 0) {
				shoot();
				return true;
			}
		}
		return base.attackCtrl();
	}

	public void shoot() {
		player.weapon.shoot(this, 0);
		weaponCooldown = player.weapon.fireRateFrames;
		player.weapon.addAmmo(-player.weapon.getAmmoUsage(0), player);
		if (charState.attackCtrl) {
			changeState(new BassShoot());
		}
	}

	public int getShootYDir() {
		int multiplier = 2;
		if (player.input.getXDir(player) != 0) {
			multiplier = 1;
		}
		return player.input.getYDir(player) * multiplier;
	}

	public int getShootAngle() {
		int baseAngle = 0;
		if (xDir == -1) {
			baseAngle = 128;
		}
		return getShootYDir() * xDir * 32 + baseAngle;
	}

	public static List<Weapon> getAllWeapons() {
		return new List<Weapon>() {
			new BassBuster(),
			new IceWall(),
			new CopyVision(),
			new SpreadDrill(),
			new WaveBurner(),
			new RemoteMine(),
			new LightingBolt(),
			new TenguBlade(),
			new MagicCard(),
		};
	}

	public override string getSprite(string spriteName) {
		return "bass_" + spriteName;
	}

	public override bool canCrouch() {
		return false;
	}

	public override bool canMove() {
		if (shootAnimTime > 0 && grounded) {
			return false;
		}
		return base.canMove();
	}

	public override bool canShoot() {
		if (weaponCooldown > 0 ||
			charState is Dash
		) {
			return false;
		}
		return base.canShoot();
	}

	public override bool canAirJump() {
		return dashedInAir == 0;
	}

	public override bool canAirDash() {
		return false;
	}

	public override bool canWallClimb() {
		return false;
	}
}

