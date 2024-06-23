using System;
using System.Collections.Generic;

namespace MMXOnline;

public class SparkShock : Weapon {
	public static SparkShock netWeapon = new();

	public SparkShock() : base() {
		displayName = "Spark Shock";
		descriptionV2 = "Generates an energy ball that can short-circuit" + "\n" +
						"electronic components, temporarily paralyzing enemies.";
		defaultAmmoUse = 3;

		index = (int)RockWeaponIds.SparkShock;
		fireRateFrames = 60;
	}

	public override float getAmmoUsage(int chargeLevel) {
		return defaultAmmoUse;
	}

	public override void shoot(Character character, params int[] args) {
		base.shoot(character, args);
        Point shootPos = character.getShootPos();
        int xDir = character.getShootXDir();
        new SparkShockProj(shootPos, xDir, character.player, character.player.getNextActorNetId(), true);
        character.playSound("spark_shock", sendRpc: true);
	}
}

public class SparkShockProj : Projectile {
	public SparkShockProj(
		Point pos, int xDir,
		Player player, ushort? netId, bool rpc = false
	) : base(
		SparkShock.netWeapon, pos, xDir, 3 * 60, 2, player, "spark_shock_proj",
		0, 0, netId, player.ownedByLocalPlayer
	) {
		maxTime = 0.75f;
		projId = (int)BluesProjIds.SparkShock;
	}

	public override void update() {
		base.update();
	}
}
