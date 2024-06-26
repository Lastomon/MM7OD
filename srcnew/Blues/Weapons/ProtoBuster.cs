using System;
using System.Collections.Generic;

namespace MMXOnline;

public class ProtoBuster : Weapon {

	public static ProtoBuster netWeapon = new();
	public ProtoBuster() : base() {

	}
}

public class ProtoBusterProj : Projectile {
	public ProtoBusterProj(
		Point pos, int xDir, Player player,
		ushort? netId, Point? vel = null, bool rpc = false
	) : base(
		ProtoBuster.netWeapon, pos, xDir, 250, 1, player,
		"proto_buster_proj", 0, 0, netId, player.ownedByLocalPlayer
	) {
		maxTime = 0.425f;
		projId = (int)BluesProjIds.Lemon;
		fadeSprite = "proto_buster_proj_fade";

		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new ProtoBusterProj(
			args.pos, args.xDir, args.player, args.netId
		);
	}
	public override void update() {
		base.update();
		if (reflectCount == 0 && System.MathF.Abs(vel.x) < 300) {
			vel.x += Global.spf * xDir * 900f;
			if (System.MathF.Abs(vel.x) >= 300) {
				vel.x = (float)xDir * 300;
			}
		}
	}

	public override void onReflect() {
		vel.x = 300;
		base.onReflect();
	}
}

public class ProtoBusterAngledProj : Projectile {
	public ProtoBusterAngledProj(
		Point pos, float byteAngle, Player player, ushort? netId, bool rpc = false
	) : base(
		ProtoBuster.netWeapon, pos, 1, 0, 1, player,
		"proto_buster_proj", 0, 0, netId, player.ownedByLocalPlayer
	) {
		byteAngle = byteAngle % 256;
		fadeSprite = "proto_buster_proj_fade";
		maxTime = 0.425f;
		projId = (int)BluesProjIds.LemonAngled;
		vel = 300 * Point.createFromByteAngle(byteAngle);

		if (byteAngle >= 64 && byteAngle <= 192) {
			xDir = -1;
		}
		if (rpc) {
			rpcCreateAngle(pos, player, netId, byteAngle);
		}
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new ProtoBusterAngledProj(
			args.pos, args.byteAngle, args.player, args.netId
		);
	}

	public override void onReflect() {
		xDir *= -1;
		vel.x *= -1;
		vel.y *= -1;
		time = 0;
	}
}

public class ProtoBusterChargedProj : Projectile {
	public ProtoBusterChargedProj(
		Point pos, int xDir, Player player,
		ushort? netId, bool rpc = false
	) : base(
		ProtoBuster.netWeapon, pos, xDir, 325, 3, player,
		"proto_chargeshot_proj", Global.halfFlinch, 0.5f, netId, player.ownedByLocalPlayer
	) {
		fadeSprite = "proto_chargeshot_proj_fade";
		fadeOnAutoDestroy = true;
		maxTime = 0.475f;
		projId = (int)BluesProjIds.ChargedBuster;

		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}


	public static Projectile rpcInvoke(ProjParameters args) {
		return new ProtoBusterChargedProj(
			args.pos, args.xDir, args.player, args.netId
		);
	}
}

public class ProtoBusterLv3Proj : Projectile {
	public ProtoBusterLv3Proj(
		Point pos, int xDir, Player player,
		ushort? netId, bool rpc = false
	) : base(
		ProtoBuster.netWeapon, pos, xDir, 325, 4, player,
		"rock_buster2_proj", Global.defFlinch, 0.5f, netId, player.ownedByLocalPlayer
	) {
		fadeSprite = "rock_buster2_fade";
		fadeOnAutoDestroy = true;
		maxTime = 0.5f;
		projId = (int)BluesProjIds.ChargedBuster;
		addRenderEffect(RenderEffectType.ChargeOrange, 0, 100);

		if (rpc) {
			rpcCreate(pos, player, netId, xDir);
		}
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new ProtoBusterChargedProj(
			args.pos, args.xDir, args.player, args.netId
		);
	}
}
