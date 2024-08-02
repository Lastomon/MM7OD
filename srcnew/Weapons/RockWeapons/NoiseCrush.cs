using System;
using System.Collections.Generic;

namespace MMXOnline;

public class NoiseCrush : Weapon {

	public static NoiseCrush netWeapon = new NoiseCrush();

	public NoiseCrush() : base() {
		index = (int)RockWeaponIds.NoiseCrush;
		weaponSlotIndex = (int)RockWeaponSlotIds.NoiseCrush;
		weaponBarBaseIndex = (int)RockWeaponBarIds.NoiseCrush;
		weaponBarIndex = weaponBarBaseIndex;
		killFeedIndex = 0;
		maxAmmo = 14;
		ammo = maxAmmo;
		rateOfFire = 0.5f;
		//shootSounds = new List<string>() {"noise_crush", "noise_crush", "", ""};
		description = new string[] { "Weak projectile that bounces on walls.", "Catch it to get a stronger shot." };
	}

	public override bool canShoot(int chargeLevel, Player player) {
		Rock? rock = player.character as Rock ?? throw new NullReferenceException();
		if (rock.hasChargedNoiseCrush) return true;
		return base.canShoot(chargeLevel, player);
	}


	public override void getProjectile(Point pos, int xDir, Player player, float chargeLevel, ushort netProjId) {
		if (player.character.ownedByLocalPlayer) {
			if (player.character is Rock rock) {

				if (chargeLevel >= 2 && rock.hasChargedNoiseCrush) {
					player.character.playSound("noise_crush", sendRpc: true);
					new NoiseCrushChargedProj(pos, xDir, player, 0, netProjId, true);
					new NoiseCrushChargedProj(new Point(pos.x - (6 * xDir), pos.y), xDir, player, 0, netProjId, true);
					new NoiseCrushChargedProj(new Point(pos.x - (12 * xDir), pos.y), xDir, player, 1, netProjId, true);
					new NoiseCrushChargedProj(new Point(pos.x - (18 * xDir), pos.y), xDir, player, 2, netProjId, true);
					new NoiseCrushChargedProj(new Point(pos.x - (24 * xDir), pos.y), xDir, player, 3, netProjId, true);
					rock.hasChargedNoiseCrush = false;
					rock.noiseCrushAnimTime = 0;
				} else {
					player.setNextActorNetId(netProjId);
					new NoiseCrushProj(pos, xDir, player, 0, player.getNextActorNetId(true), true, true);
					new NoiseCrushProj(new Point(pos.x - (4 * xDir), pos.y), xDir, player, 0, player.getNextActorNetId(true), rpc: true);
					new NoiseCrushProj(new Point(pos.x - (8 * xDir), pos.y), xDir, player, 1, player.getNextActorNetId(true), rpc: true);
					new NoiseCrushProj(new Point(pos.x - (12 * xDir), pos.y), xDir, player, 1, player.getNextActorNetId(true), rpc: true);
					new NoiseCrushProj(new Point(pos.x - (16 * xDir), pos.y), xDir, player, 2, player.getNextActorNetId(true), rpc: true);
					player.character.playSound("noise_crush", sendRpc: true);
				}
			}
		}
	}

	public override void shoot(Character character, params int[] args) {
		base.shoot(character, args);
		Point shootPos = character.getShootPos();
		int xDir = character.getShootXDir();
		Player player = character.player;
		int chargeLevel = args[0];

		if (player.character is Rock rock) {

				if (chargeLevel >= 2 && rock.hasChargedNoiseCrush) {
					character.playSound("noise_crush_charged");
					new NoiseCrushChargedProj(shootPos, xDir, player, 0, player.getNextActorNetId(), true);
					new NoiseCrushChargedProj(shootPos.addxy(6 * xDir, 0), xDir, player, 0, player.getNextActorNetId(), true);
					new NoiseCrushChargedProj(shootPos.addxy(12 * xDir, 0), xDir, player, 1, player.getNextActorNetId(), true);
					new NoiseCrushChargedProj(shootPos.addxy(18 * xDir, 0), xDir, player, 2, player.getNextActorNetId(), true);
					new NoiseCrushChargedProj(shootPos.addxy(24 * xDir, 0), xDir, player, 3, player.getNextActorNetId(), true);
					rock.hasChargedNoiseCrush = false;
					rock.noiseCrushAnimTime = 0;
				} else {
					new NoiseCrushProj(shootPos, xDir, player, 0, player.getNextActorNetId(), true, true);
					new NoiseCrushProj(shootPos.addxy(4 * xDir, 0), xDir, player, 0, player.getNextActorNetId(true), rpc: true);
					new NoiseCrushProj(shootPos.addxy(8 * xDir, 0), xDir, player, 1, player.getNextActorNetId(true), rpc: true);
					new NoiseCrushProj(shootPos.addxy(12 * xDir, 0), xDir, player, 1, player.getNextActorNetId(true), rpc: true);
					new NoiseCrushProj(shootPos.addxy(16 * xDir, 0), xDir, player, 2, player.getNextActorNetId(true), rpc: true);
					character.playSound("noise_crush", sendRpc: true);
				}
			}
	}
}


public class NoiseCrushProj : Projectile {

	public int type;
	public int bounces = 0;
	public bool isMain;

	public NoiseCrushProj(
		Point pos, int xDir, Player player, 
		int type, ushort netProjId,
		bool isMain = false, bool rpc = false
	) : base(
		NoiseCrush.netWeapon, pos, xDir, 240, 1,
		player, "noise_crush_top", 0, 0.2f,
		netProjId, player.ownedByLocalPlayer
	) {

		projId = (int)RockProjIds.NoiseCrush;
		maxTime = 0.75f;
		this.type = type;
		this.isMain = isMain;
		fadeSprite = "rock_buster_fade";
		canBeLocal = false;

		if (type == 1) changeSprite("noise_crush_middle", true);
		else if (type == 2) {
			changeSprite("noise_crush_bottom", true);
		}
		if (rpc) {
			byte[] extraArgs = new byte[] { (byte)type };

			rpcCreate(pos, player, netProjId, xDir, extraArgs);
		}
	}

	public static Projectile rpcInvoke(ProjParameters arg) {
		return new NoiseCrushProj(
			arg.pos, arg.xDir, arg.player, arg.extraData[0], arg.netId
		);
	}

	public override void onHitWall(CollideData other) {
		base.onHitWall(other);

		if (bounces < 4) {

			if (other.isSideWallHit()) {
				vel.x *= -1;
				xDir *= -1;
				incPos(new Point(5 * MathF.Sign(vel.x), 0));
				bounces++;
				time = 0;
			}
		}
	}
}


public class NoiseCrushChargedProj : Projectile {

	public int type;

	public NoiseCrushChargedProj(
		Point pos, int xDir, Player player,
		int type, ushort netProjId, bool rpc = false
	) : base(
		NoiseCrush.netWeapon, pos, xDir, 240, 3,
		player, "noise_crush_charged_top", 0, 0.33f,
		netProjId, player.ownedByLocalPlayer
	) {

		projId = (int)RockProjIds.NoiseCrushCharged;
		maxTime = 1f;
		this.type = type;

		if (type == 1) changeSprite("noise_crush_charged_middle", true);
		else if (type == 2) changeSprite("noise_crush_charged_middle2", true);
		else if (type == 3) {
			changeSprite("noise_crush_charged_bottom", true);
		}
		
		if (rpc) {
			byte[] extraArgs = new byte[] { (byte)type };

			rpcCreate(pos, player, netProjId, xDir, extraArgs);
		}
	}

	public static Projectile rpcInvoke(ProjParameters arg) {
		return new NoiseCrushChargedProj(
			arg.pos, arg.xDir, arg.player, arg.extraData[0], arg.netId
		);
	}
}
