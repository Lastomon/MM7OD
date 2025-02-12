﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Converters;

namespace MMXOnline;

public class IceWall : Weapon {
	public static IceWall netWeapon = new();
	public IceWallProj wall = null!;

	public IceWall() : base() {
		index = (int)BassWeaponIds.IceWall;
		displayName = "ICE WALL";
		maxAmmo = 14;
		ammo = maxAmmo;
		weaponSlotIndex = index;
		weaponBarBaseIndex = index;
		weaponBarIndex = index;
		fireRate = 30;
	}

	public override bool canShoot(int chargeLevel, Character character) {
		return base.canShoot(chargeLevel, character) && (wall == null || wall?.destroyed == true);
	}

	public override void shoot(Character character, params int[] args) {
		base.shoot(character, args);
		Bass bass = character as Bass ?? throw new NullReferenceException();
		Point shootPos = character.getShootPos().addxy(0, 2);
		Player player = character.player;

		wall = new IceWallProj(bass, shootPos, bass.getShootXDir(), player.getNextActorNetId(), true);
		bass.playSound("icewall", true);
	}
}


public class IceWallStart : Anim {
	Player player;

	public IceWallStart(
		Point pos, int xDir, ushort? netId, Player player,
		bool sendRpc = false, bool ownedByLocalPlayer = true
	) : base(
		pos, "ice_wall_spawn", xDir, netId, true, 
		sendRpc, ownedByLocalPlayer, player.character
	) {
		this.player = player;
	}

	public override void onDestroy() {
		base.onDestroy();
		if (ownedByLocalPlayer) {
			//new IceWallProj(pos, xDir, player, player.getNextActorNetId(), true);
		}
	}
}
	
public class IceWallProj : Projectile, IDamagable {
	float lastDeltaX = 0;
	float maxSpeed = 250;
	int bounces;
	bool startedMoving;
	List<Character> chrs = new();
	Player player;
	Collider? terrainCollider;
	float health = 2;

	public IceWallProj(
		Actor owner, Point pos, int xDir, ushort? netId, 
		bool rpc = false, Player? altPlayer = null
	) : base(
		pos, xDir, owner, "ice_wall_proj", netId, altPlayer
	) {
		projId = (int)BassProjIds.IceWall;
		damager.damage = 2;
		damager.flinch = Global.halfFlinch;
		damager.hitCooldown = 140;

		fadeSprite = "ice_wall_fade";
		fadeSound = "freezebreak2";
		fadeOnAutoDestroy = true;
		useGravity = true;
		canBeLocal = false;
		base.xDir = xDir;
		this.player = ownerPlayer;
		isSolidWall = true;
		maxTime = 2f;
		destroyOnHit = false;
		splashable = true;
		Global.level.modifyObjectGridGroups(this, isActor: true, isTerrain: true);
		selectiveSolididyFunc = selectiveSolidity;

		if (rpc) rpcCreate(pos, owner, ownerPlayer, netId, xDir);
	}

	public static Projectile rpcInvoke(ProjParameters arg) {
		return new IceWallProj(
			arg.owner, arg.pos, arg.xDir, arg.netId, altPlayer: arg.player
		);
	}
	
	public override void update() {
		base.update();
		if (deltaPos.x != 0) {
			lastDeltaX = deltaPos.x;
		}
		if (!ownedByLocalPlayer) {
			return;
		}

		if (startedMoving && Math.Abs(vel.x) < maxSpeed) {
			vel.x += xDir * 0.1f * 60f;
			if (Math.Abs(vel.x) > maxSpeed) vel.x = maxSpeed * xDir;
		}

		if (isUnderwater()) {
			grounded = false;
			gravityModifier = -1;
			if (Math.Abs(vel.y) > Physics.MaxUnderwaterFallSpeed * 0.5f) {
				vel.y = Physics.MaxUnderwaterFallSpeed * 0.5f * gravityModifier;
			}
		} else gravityModifier = 1;

		if (bounces >= 3) {
			destroySelf();
		}
	}

	public bool selectiveSolidity(GameObject other) {
		if (other is not Character chara) {
			return false;
		}
		// Fully solid for enemies.
		if ((chara.player == damager.owner || chara.player.alliance != damager.owner.alliance) &&
			chara.charState is not LadderClimb
		) {
			return true;
		}
		// Platform-like behaviour for allies.
		if (chara.pos.y <= getTopY() + 16) {
			return true;
		}
		return false;
	}

	public override void onCollision(CollideData other) {
		base.onCollision(other);
		// Wall hit.
		if (other.gameObject is Wall) {
			if (other.isSideWallHit()) {
				xDir *= -1;
				vel.x *= -1;
				pos.y += xDir;
				playSound("ding");
				bounces++;
			}
			return;
		}
		// Hit enemy.
		if (other.gameObject is not Actor actor || !actor.ownedByLocalPlayer || actor is not Character) {
			return;
		}
		Character? ownChar = damager.owner?.character;
		// Movement start.
		if (other.gameObject == ownChar && !startedMoving) {
			if (ownChar.pos.y >= getTopY() + 10 && ownChar.charState is Dash or Run or TenguBladeDash) {
				startedMoving = true;
				xDir = MathF.Sign(pos.x - ownChar.pos.x) >= 0 ? 1 : -1;
				vel.x = xDir * 30;
			}
		}
	}

	public void applyDamage(float damage, Player owner, Actor? actor, int? weaponIndex, int? projId) {
		health -= damage;
		if (health <= 0) {
			destroySelf();
		}
	}
	public bool canBeDamaged(int damagerAlliance, int? damagerPlayerId, int? projId) {
		return health > 0 && damagerAlliance != damager.owner.alliance && projId == (int)BassProjIds.IceWall;
	}
	public bool isInvincible(Player attacker, int? projId) => false;
	public bool canBeHealed(int healerAlliance) => false;
	public void heal(Player healer, float healAmount, bool allowStacking = true, bool drawHealText = true) { }
	public bool isPlayableDamagable() => false;
}
