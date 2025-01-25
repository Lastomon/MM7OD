﻿using System;

namespace MMXOnline;

public class TunnelRhino : Maverick {
	public static Weapon getWeapon() { return new Weapon(WeaponIds.TunnelRGeneric, 153); }
	public static Weapon getMeleeWeapon(Player player) { return new Weapon(WeaponIds.TunnelRGeneric, 153); }

	public Weapon meleeWeapon;
	public TunnelRhino(
		Player player, Point pos, Point destPos, int xDir, ushort? netId, bool ownedByLocalPlayer, bool sendRpc = false
	) : base(
		player, pos, destPos, xDir, netId, ownedByLocalPlayer
	) {
		stateCooldowns.Add(typeof(TunnelRShootState), new MaverickStateCooldown(true, false, 0.75f));
		stateCooldowns.Add(typeof(TunnelRShoot2State), new MaverickStateCooldown(true, false, 0.75f));
		stateCooldowns.Add(typeof(TunnelRDashState), new MaverickStateCooldown(false, false, 1f));

		weapon = getWeapon();
		meleeWeapon = getMeleeWeapon(player);

		spriteFrameToSounds["tunnelr_run/3"] = "walkStomp";
		spriteFrameToSounds["tunnelr_run/11"] = "walkStomp";

		awardWeaponId = WeaponIds.TornadoFang;
		weakWeaponId = WeaponIds.AcidBurst;
		weakMaverickWeaponId = WeaponIds.ToxicSeahorse;

		netActorCreateId = NetActorCreateId.TunnelRhino;
		netOwner = player;
		if (sendRpc) {
			createActorRpc(player.id);
		}

		armorClass = ArmorClass.Heavy;
		canStomp = true;
	}

	public override void update() {
		base.update();
		if (aiBehavior == MaverickAIBehavior.Control) {
			if (state is MIdle or MRun or MLand) {
				if (input.isPressed(Control.Shoot, player)) {
					changeState(new TunnelRShootState(false));
				} else if (input.isPressed(Control.Special1, player)) {
					changeState(new TunnelRShoot2State());
				} else if (input.isPressed(Control.Dash, player)) {
					changeState(new TunnelRDashState());
				}
			}
		}
	}

	public override float getRunSpeed() {
		return 75;
	}

	public override string getMaverickPrefix() {
		return "tunnelr";
	}

	public override MaverickState getRandomAttackState() {
		return aiAttackStates().GetRandomItem();
	}

	public override MaverickState[] aiAttackStates() {
		return new MaverickState[]
		{
				new TunnelRShootState(false),
				new TunnelRShoot2State(),
				new TunnelRDashState(),
		};
	}

	// Melee IDs for attacks.
	public enum MeleeIds {
		None = -1,
		Dash,
		Fall,
	}

	// This can run on both owners and non-owners. So data used must be in sync.
	public override int getHitboxMeleeId(Collider hitbox) {
		return (int)(sprite.name switch {
			"tunnelr_dash" => MeleeIds.Dash,
			"tunnelr_fall" => MeleeIds.Fall,
			_ => MeleeIds.None
		});
	}

	// This can be called from a RPC, so make sure there is no character conditionals here.
	public override Projectile? getMeleeProjById(int id, Point pos, bool addToLevel = true) {
		return (MeleeIds)id switch {
			MeleeIds.Dash => new GenericMeleeProj(
				weapon, pos, ProjIds.TunnelRDash, player,
				4, Global.defFlinch, addToLevel: addToLevel
			),
			MeleeIds.Fall => new GenericMeleeProj(
				weapon, pos, ProjIds.TunnelRStomp, player,
				4, Global.defFlinch, addToLevel: addToLevel
			),
			_ => null
		};
	}

	public override void updateProjFromHitbox(Projectile proj) {
		if (proj.projId == (int)ProjIds.TunnelRStomp) {
			float damage = 1 + Helpers.clamp(MathF.Floor(deltaPos.y * 0.6125f), 0, 4);
			proj.damager.damage = damage;
		}
	}

}

public class TunnelRTornadoFang : Projectile {
	int state = 0;
	float stateTime = 0;
	int type;
	float sparksCooldown;
	public TunnelRTornadoFang(
		Weapon weapon, Point pos, int xDir, int type, Player player, ushort netProjId, bool sendRpc = false
	) : base(
		weapon, pos, xDir, 100, 1, player, "tunnelr_proj_drillbig", 0, 0.25f, netProjId, player.ownedByLocalPlayer
	) {
		maxTime = 1.5f;
		projId = (int)ProjIds.TunnelRTornadoFang;
		destroyOnHit = false;
		this.type = type;
		if (type != 0) {
			vel.x = 0;
			vel.y = (type == 1 ? -100 : 100);
			projId = (int)ProjIds.TunnelRTornadoFang2;
		}

		if (sendRpc) {
			rpcCreate(pos, player, netProjId, xDir, (byte)type);
		}
	}

	public override void update() {
		base.update();
		Helpers.decrementTime(ref sparksCooldown);

		if (state == 0) {
			if (type == 0) {
				if (stateTime > 0.15f) {
					vel.x = 0;
				}
			} else if (type == 1 || type == 2) {
				if (stateTime > 0.15f) {
					vel.y = 0;
				}
				if (stateTime > 0.15f && stateTime < 0.3f) vel.x = 100 * xDir;
				else vel.x = 0;
			}
			stateTime += Global.spf;
			if (stateTime >= 0.75f) {
				state = 1;
			}
		} else if (state == 1) {
			vel.x += Global.spf * 500 * xDir;
			if (Math.Abs(vel.x) > 350) vel.x = 350 * xDir;
		}
	}

	public override void onHitDamagable(IDamagable damagable) {
		base.onHitDamagable(damagable);
		vel.x = 4 * xDir;

		if (damagable is not CrackedWall) {
			time -= Global.spf;
			if (time < 0) time = 0;
		}

		if (sparksCooldown == 0) {
			//playSound("tunnelrDrill");
			var sparks = new Anim(pos, "tunnelfang_sparks", xDir, null, true);
			sparks.setzIndex(zIndex + 100);
			sparksCooldown = 0.25f;
		}
		var chr = damagable as Character;
		if (chr != null && chr.ownedByLocalPlayer && !chr.isSlowImmune()) {
			chr.vel = Point.lerp(chr.vel, Point.zero, Global.spf * 10);
			chr.slowdownTime = 0.25f;
		}
	}
}

public class TunnelRShootState : MaverickState {
	bool shotOnce;
	bool isSecond;
	public TunnelRShootState(bool isSecond) : base("shoot1") {
		this.isSecond = isSecond;
		exitOnAnimEnd = true;
		canEnterSelf = true;
	}

	public override void update() {
		base.update();

		Point? shootPos = maverick.getFirstPOI();
		if (!shotOnce && shootPos != null) {
			shotOnce = true;
			//maverick.playSound("tunnelrShoot", sendRpc: true);
			if (!isSecond) {
				new TunnelRTornadoFang(maverick.weapon, shootPos.Value, maverick.xDir, 0, player, player.getNextActorNetId(), sendRpc: true);
			} else {
				new TunnelRTornadoFang(maverick.weapon, shootPos.Value, maverick.xDir, 1, player, player.getNextActorNetId(), sendRpc: true);
				new TunnelRTornadoFang(maverick.weapon, shootPos.Value, maverick.xDir, 2, player, player.getNextActorNetId(), sendRpc: true);
			}
		}

		if (!isSecond && (input.isPressed(Control.Shoot, player) || isAI) && maverick.frameIndex > 6) {
			maverick.sprite.restart();
			maverick.changeState(new TunnelRShootState(true), true);
		}
	}
}

public class TunnelRTornadoFangDiag : Projectile {
	public TunnelRTornadoFangDiag(
		Weapon weapon, Point pos, int xDir, Player player, ushort netProjId, bool sendRpc = false
	) : base(
		weapon, pos, xDir, 0, 3, player, "tunnelr_proj_drill", Global.halfFlinch, 0.25f, netProjId, player.ownedByLocalPlayer
	) {
		maxTime = 1.5f;
		projId = (int)ProjIds.TunnelRTornadoFangDiag;
		destroyOnHit = false;
		vel = new Point(xDir * 150, -150);

		if (sendRpc) {
			rpcCreate(pos, player, netProjId, xDir);
		}
	}
}


public class TunnelRShoot2State : MaverickState {
	bool shotOnce;
	bool shotOnce2;
	bool shotOnce3;
	public TunnelRShoot2State() : base("shoot3") {
		exitOnAnimEnd = true;
	}

	public override void update() {
		base.update();

		Point? shootPos = maverick.getFirstPOI("drillbig");
		if (!shotOnce && shootPos != null) {
			shotOnce = true;
			new TunnelRTornadoFang(
				maverick.weapon, shootPos.Value, maverick.xDir, 0,
				player, player.getNextActorNetId(), sendRpc: true
			);
			//maverick.playSound("tunnelrShoot", sendRpc: true);
		}

		Point? shootPos2 = maverick.getFirstPOI("drillfront");
		if (!shotOnce2 && shootPos2 != null) {
			shotOnce2 = true;
			new TunnelRTornadoFangDiag(
				maverick.weapon, shootPos2.Value, maverick.xDir * -1,
				player, player.getNextActorNetId(), sendRpc: true);
		}

		Point? shootPos3 = maverick.getFirstPOI("drillback");
		if (!shotOnce3 && shootPos3 != null) {
			shotOnce3 = true;
			new TunnelRTornadoFangDiag(
				maverick.weapon, shootPos3.Value, maverick.xDir,
				player, player.getNextActorNetId(), sendRpc: true
			);
		}
	}
}

public class TunnelRDashState : MaverickState {
	float dustTime;
	public TunnelRDashState() : base("dash", "dash_start") {
	}

	public override void update() {
		base.update();
		if (inTransition()) return;

		dustTime += Global.spf;
		if (dustTime > 0.05f) {
			dustTime = 0;
			new Anim(
				maverick.pos.addxy(-maverick.xDir * 10, 0), "dust", 1,
				player.getNextActorNetId(), true, sendRpc: true
			) {
				vel = new Point(0, -50)
			};
		}

		var move = new Point(250 * maverick.xDir, 0);
		var hitGround = Global.level.checkTerrainCollisionOnce(maverick, move.x * Global.spf * 5, 20);
		if (hitGround == null) {
			maverick.changeState(new MIdle());
			return;
		}

		var hitWall = Global.level.checkTerrainCollisionOnce(maverick, move.x * Global.spf * 2, -5);
		if (hitWall?.isSideWallHit() == true) {
			//maverick.playSound("crash", sendRpc: true);
			maverick.shakeCamera(sendRpc: true);
			maverick.changeToIdleOrFall();
			return;
		}

		maverick.move(move);

		if (isHoldStateOver(0.5f, 1.5f, 1f, Control.Dash)) {
			maverick.changeToIdleOrFall();
			return;
		}
	}
}
