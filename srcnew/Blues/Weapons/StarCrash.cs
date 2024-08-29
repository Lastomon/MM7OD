using System;
using System.Collections.Generic;

namespace MMXOnline;

public class StarCrash : Weapon {
	public static StarCrash netWeapon = new();
	public StarCrashProj? activeProj;

	public StarCrash() : base() {
		displayName = "STAR CRASH";
		descriptionV2 = "Creates a star-shaped energy barrier\nthat reduces gravity.";
		decimal coreCooldown = 20;
		ammoUseText = (1 / coreCooldown * 60).ToGBString() + " per second";

		index = (int)BluesWeaponIds.StarCrash;
		fireRateFrames = 60;
		hasCustomAnim = true;
	}

	public override float getAmmoUsage(int chargeLevel) {
		if (activeProj?.destroyed == false) {
			return 4;
		}
		return 0;
	}

	public override void shoot(Character character, params int[] args) {
		base.shoot(character, args);
		Blues blues = character as Blues ?? throw new NullReferenceException();

		if (activeProj?.destroyed == false || blues.starCrash != null || blues.starCrashActive) {
			blues.destroyStarCrash();
			activeProj?.destroySelf();
			activeProj = null;
		} else {
			activeProj = new StarCrashProj(
				character.getCenterPos(), character.xDir, character.player,
				character.player.getNextActorNetId(), true
			);
			blues.starCrash = activeProj;
			blues.starCrashActive = true;
			blues.gravityModifier = 0.625f;
			shootCooldown = 0;
		}
	}
}

public class StarCrashProj : Projectile {
	Blues blues = null!;
	float starAngle;
	int radius = 30;
	int coreCooldown = 50;
	int frameCount;
	int starFrame;
	List<Sprite> stars = new();

	public StarCrashProj(
		Point pos, int xDir,
		Player player, ushort? netId, bool rpc = false
	) : base(
		StarCrash.netWeapon, pos, xDir, 0, 0, player, "empty",
		0, 0, netId, player.ownedByLocalPlayer
	) {
		projId = (int)BluesProjIds.StarCrash;
		blues = player.character as Blues ?? throw new NullReferenceException();
		canBeLocal = false;

		for(int i = 0; i < 3; i++) {
			Sprite star = new Sprite("star_crash");
			stars.Add(star);
		}

		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new StarCrashProj(
			args.pos, args.xDir, args.player, args.netId
		);
	}

	public override void update() {
		base.update();
		starAngle += 4;
		if (starAngle >= 360) starAngle -= 360;
		/*if (blues == null) {
			if (ownedByLocalPlayer) {
				destroySelf();
			}
			return;
		}*/
		// Sync poses with protoman.
		if (blues != null) {
			pos = blues.getCenterPos().round();
			xDir = blues.xDir;
		} 

		frameCount++;
		if (frameCount > 4) {
			frameCount = 0;
			starFrame++;
		}

		// Local player ends here.
		/*if (!ownedByLocalPlayer) {
			return;
		}*/
		// Destroy if not linked with Protoman anymore.
		if (blues == null || blues.destroyed || blues.starCrash != this || !blues.starCrashActive || blues.overheating) {
			destroySelf();
		}
		// Ammo reduction.
		if (coreCooldown <= 0) {
			coreCooldown = 20;
			blues?.addCoreAmmo(1);
		} else {
			coreCooldown--;
		}
	}

	public override void render(float x, float y) {
		base.render(x,y);
		Point center = blues.getCenterPos();
		
		for (int i = 0; i < 3; i++) {
			float extraAngle = (starAngle + i * 120) % 360;
			float xPlus = pos.x + (Helpers.cosd(extraAngle) * radius);
			float yPlus = pos.y + (Helpers.sind(extraAngle) * radius);

			stars[i].draw(
				starFrame % 4, center.x + xPlus, center.y + yPlus, 
				xDir, yDir, getRenderEffectSet(), 1, 1, 1, zIndex
			);
		}
		
	}

	public override void onDestroy() {
		base.onDestroy();
		blues?.destroyStarCrash();

		for (int i = 0; i < 3; i++) {
			float extraAngle = (starAngle + i * 120) % 360;
			float xPlus = pos.x + (Helpers.cosd(extraAngle) * radius);
			float yPlus = pos.y + (Helpers.sind(extraAngle) * radius);

			new Anim(new Point(xPlus, yPlus), "star_crash_fade", xDir, damager.owner.getNextActorNetId(), true, true);
		}
	}
}
