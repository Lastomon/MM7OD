﻿using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Graphics.Glsl;

namespace MMXOnline;

public class CopyVision : Weapon {
	public static CopyVision netWeapon = new();

	public CopyVision() : base() {
		weaponSlotIndex = (int)BassWeaponIds.CopyVision;
		weaponBarBaseIndex = (int)BassWeaponIds.CopyVision;
		index = (int)BassWeaponIds.CopyVision;
		weaponBarIndex = (int)BassWeaponIds.CopyVision;
		killFeedIndex = 0;
		maxAmmo = 7;
		ammo = maxAmmo;
		switchCooldown = 0.75f; //gambiarrita
		rateOfFire = 2f;

		descriptionV2 = (
			"Create a clone that attack automatically." + "\n" +
			"Can only have one clone at once."
		);
	}

	public override void shoot(Character character, params int[] args) {
		Point shootPos = character.getShootPos();
		Player player = character.player;

		new CopyVisionClone(shootPos, player, character.xDir, character.player.getNextActorNetId(), true);

	}
	public override bool canShoot(int chargeLevel, Player player) {
		if (!base.canShoot(chargeLevel, player)) {
			return false;
		}
		if (player.character is Bass { cVclone: not null }) {
			return false;
		}
		return true;
	}
}
public class CopyVisionLemon : Projectile {
	public CopyVisionLemon(
		Point pos, int xDir, Player player, ushort netProjId, bool rpc = false
	) : base(
		CopyVision.netWeapon, pos, xDir, 240, 1, player, "copy_vision_lemon",
		0, 0.075f, netProjId, player.ownedByLocalPlayer
	) {
		maxTime = 0.525f;
		fadeSprite = "copy_vision_lemon_fade";
		projId = (int)BassProjIds.BassLemon;
	}
}

public class CopyVisionClone : Actor {
	int state = 0;
	float cloneShootTime;
	float cloneTime;

	// Define the rateOfFire of the clone.
	float rateOfFire = 9;
	Bass? bass;

	public CopyVisionClone(
		Point pos, Player player, int xDir, ushort netId, bool ownedByLocalPlayer, bool rpc = false
	) : base("copy_vision_start", pos, netId, ownedByLocalPlayer, false
	) {
		bass = player.character as Bass;
		if (ownedByLocalPlayer && bass != null) {
			bass.cVclone = this;
		}
		useGravity = false;
		this.xDir = xDir;
		netOwner = player;
		netActorCreateId = NetActorCreateId.RaySplasherTurret;
		if (rpc) {
			createActorRpc(player.id);
		}
	}



	public override void update() {
		base.update();
		if (!ownedByLocalPlayer) return;
		cloneTime += Global.spf;
		if (ownedByLocalPlayer && netOwner.weapon is CopyVision) {
			if (netOwner.input.isPressed(Control.Shoot, netOwner)) { destroySelf(); }
		}
		if (isAnimOver()) {
			state = 1;
		}
		if (state == 1) {
			changeSprite("copy_vision_clone", true);
			cloneShootTime += Global.speedMul;
			if (cloneShootTime > rateOfFire) {
				Point? shootPos = getFirstPOI();
				if (shootPos != null) {
					new CopyVisionLemon(shootPos.Value, xDir, netOwner, netOwner.getNextActorNetId(), rpc: true);
					cloneShootTime = 0;
				}
			}
		}
		if (cloneTime >= 120) {
			destroySelf();
		}
	}
	public override void onDestroy() {
		base.onDestroy();
		new Anim(
				pos.clone(), "copy_vision_exit", xDir,
				netOwner.getNextActorNetId(), true, sendRpc: true
			);
		if (ownedByLocalPlayer && bass != null) {
			bass.cVclone = null!;
		}
	}

}