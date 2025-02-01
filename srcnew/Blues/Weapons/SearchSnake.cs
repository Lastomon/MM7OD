using System;
using System.Collections.Generic;

namespace MMXOnline;

public class SearchSnake : Weapon {
	public static SearchSnake netWeapon = new();

	public SearchSnake() : base() {
		displayName = "SEARCH SNAKE";
		descriptionV2 = "Releases snake-like missiles\nthat crawl across surfaces.";
		defaultAmmoUse = 2;

		index = (int)BluesWeaponIds.SearchSnake;
		fireRate = 35;
	}

	public override float getAmmoUsage(int chargeLevel) {
		return defaultAmmoUse;
	}

	public override void shoot(Character character, params int[] args) {
		base.shoot(character, args);
		Blues blues = character as Blues ?? throw new NullReferenceException();
		Point shootPos = blues.getShootPos();
		int xDir = blues.getShootXDir();
		new SearchSnakeProj(blues, shootPos, xDir, blues.player.getNextActorNetId(), true);
		character.playSound("buster", sendRpc: true);
	}
}
public class SearchSnakeProj : Projectile {
	bool groundedOnce;
	bool startAngleDrawing;
	float projSpeed = 120;

	public SearchSnakeProj(
		Actor owner, Point pos, int xDir, ushort? netProjId, bool rpc = false, Player? altPlayer = null
	) : base(
		pos, xDir, owner, "search_snake_proj_air", netProjId, altPlayer
	) {
		projId = (int)BluesProjIds.SearchSnake;
		wallCrawlSpeed = projSpeed * 0.75f;
		destroyOnHit = true;
		fadeSprite = "generic_explosion";
		fadeOnAutoDestroy = true;
		useGravity = true;
		maxTime = 2;

		vel.x = 120 * xDir;
		damager.damage = 2;
		damager.hitCooldown = 0.5f;

		if (rpc) {
			rpcCreate(pos, owner, ownerPlayer, netProjId, xDir);
		}
		canBeLocal = false;
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new SearchSnakeProj(
			args.owner, args.pos, args.xDir, args.netId, altPlayer: args.player
		);
	}

	public override void update() {
		base.update();
		if (!groundedOnce) {
			if (grounded) {
				vel.x = 0;
				vel.y = 0;
				setupWallCrawl(new Point(xDir, 1));
				changeSprite("search_snake_proj", true);
				groundedOnce = true;
				useGravity = false;
			}
			return;
		}
		updateWallCrawl();
		if (groundedOnce) updateWallCrawl();

		if (!startAngleDrawing) {
			startAngleDrawing = true;
			return;
		}
	}

	public override void onHitWall(CollideData other) {
		base.onHitWall(other);
		if (!groundedOnce) {
			useGravity = false;
			stopMoving();
			changeSprite("search_snake_proj", true);
			setupWallCrawl(new Point(xDir, -1));
			groundedOnce = true;
		}
	}

	public override void render(float x, float y) {
		base.render(x, y);
		if (startAngleDrawing) {
			byteAngle = deltaPos.byteAngle;
			if (xDir < 0) byteAngle = -byteAngle + 128;

			if (deltaPos.y < 0 && xDir < 0) byteAngle += 128;
			if (xDir < 0) byteAngle += 128;
		}
	}

	
	public override List<byte> getCustomActorNetData() {
		List<byte> customData = base.getCustomActorNetData() ?? new();
		
		if (byteAngle != null) customData.Add((byte)byteAngle.Value);

		return customData;
	}

	public override void updateCustomActorNetData(byte[] data) {
		base.updateCustomActorNetData(data);

		byteAngle = data[0];
	}
}
