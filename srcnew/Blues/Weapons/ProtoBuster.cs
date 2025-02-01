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
		Actor owner, Point pos, int xDir, ushort? netId, 
		Point? vel = null, bool rpc = false, Player? altPlayer = null
	) : base(
		pos, xDir, owner, "proto_buster_proj", netId, altPlayer
	) {
		maxTime = 0.425f;
		projId = (int)BluesProjIds.Lemon;
		fadeSprite = "proto_buster_proj_fade";

		base.vel.x = 250 * xDir;
		damager.damage = 1;

		if (rpc) {
			rpcCreate(pos, owner, ownerPlayer, netId, xDir);
		}
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new ProtoBusterProj(
			args.owner, args.pos, args.xDir, args.netId, altPlayer: args.player
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
		Actor owner, Point pos, float byteAngle, int type, 
		ushort? netId, bool rpc = false, Player? altPlayer = null
	) : base(
		pos, 1, owner, "proto_midcharge_proj", netId, altPlayer
	) {
		byteAngle = byteAngle % 256;
		fadeSprite = "proto_midcharge_proj_fade";
		fadeOnAutoDestroy = true;
		maxTime = 0.425f;
		projId = (int)BluesProjIds.LemonAngled;
		vel = 300 * Point.createFromByteAngle(byteAngle);

		damager.damage = 2;
		damager.hitCooldown = 1;

		if (byteAngle > 64 && byteAngle < 192) {
			xDir = -1;
			this.byteAngle = byteAngle - 128;
		} else {
			this.byteAngle = byteAngle;
		}

		if (type == 0) {
			changeSprite("proto_buster_proj", true);
			fadeSprite = "proto_buster_proj_fade";
			damager.damage = 1;
		} else if (type == 2) {
			damager.flinch = Global.miniFlinch;
			changeSprite("proto_chargeshot_yellow_proj", true);
			fadeSprite = "proto_chargeshot_yellow_proj_fade";
		}

		if (rpc) {
			rpcCreateByteAngle(pos, ownerPlayer, netId, byteAngle, (byte)type);
		}
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new ProtoBusterAngledProj(
			args.owner, args.pos, args.byteAngle, args.extraData[0], 
			args.netId, altPlayer: args.player
		);
	}

	public override void onReflect() {
		xDir *= -1;
		vel.x *= -1;
		vel.y *= -1;
		time = 0;
	}
}

public class ProtoBusterOverdriveProj : Projectile {
	public ProtoBusterOverdriveProj(
		Actor owner, Point pos, int xDir, ushort? netId, 
		Point? vel = null, bool rpc = false, Player? altPlayer = null
	) : base(
		pos, xDir, owner, "proto_midcharge_proj", netId, altPlayer
	) {
		maxTime = 0.425f;
		projId = (int)BluesProjIds.LemonOverdrive;
		fadeSprite = "proto_midcharge_proj_fade";
		fadeOnAutoDestroy = true;

		base.vel.x = 250 * xDir;
		damager.damage = 1;
		damager.flinch = Global.miniFlinch;

		if (rpc) {
			rpcCreate(pos, owner, ownerPlayer, netId, xDir);
		}
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new ProtoBusterProj(
			args.owner, args.pos, args.xDir, args.netId, altPlayer: args.player
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

public class ProtoBusterLv2Proj : Projectile {
	public ProtoBusterLv2Proj(
		Actor owner, int type, Point pos, int xDir, 
		ushort? netId, bool rpc = false, Player? altPlayer = null
	) : base(
		pos, xDir, owner, "rock_buster1_proj", netId, altPlayer
	) {
		fadeSprite = "rock_buster1_fade";
		fadeOnAutoDestroy = true;
		maxTime = 0.4125f;
		projId = (int)BluesProjIds.BusterLV2;

		vel.x = 325 * xDir;
		damager.damage = 2;
		damager.flinch = Global.miniFlinch;
		damager.hitCooldown = 0.5f;

		if (type == 1) {
			damager.flinch = Global.halfFlinch;
			changeSprite("rock_buster2_proj", true);
			fadeSprite = "rock_buster2_fade";
		}

		if (rpc) {
			byte[] extraArgs = new byte[] { (byte)type };

			rpcCreate(pos, owner, ownerPlayer, netId, xDir, extraArgs);
		}
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new ProtoBusterLv2Proj(
			args.owner, args.extraData[0], args.pos, args.xDir, args.netId, altPlayer: args.player
		);
	}
}


public class ProtoBusterLv3Proj : Projectile {
	public ProtoBusterLv3Proj(
		Actor owner, int type, Point pos, int xDir, ushort? netId, 
		bool rpc = false, Player? altPlayer = null
	) : base(
		pos, xDir, owner, "proto_chargeshot_purple_proj", netId, altPlayer
	) {
		fadeSprite = "proto_chargeshot_purple_proj_fade";
		fadeOnAutoDestroy = true;
		maxTime = 0.45f;
		projId = (int)BluesProjIds.BusterLV4;

		vel.x = 325 * xDir;
		damager.damage = 3;
		damager.flinch = Global.halfFlinch;
		damager.hitCooldown = 0.5f;

		if (type == 1) {
			damager.flinch = Global.defFlinch;
		}

		if (rpc) {
			byte[] extraArgs = new byte[] { (byte)type };

			rpcCreate(pos, owner, ownerPlayer, netId, xDir, extraArgs);
		}
	}


	public static Projectile rpcInvoke(ProjParameters args) {
		return new ProtoBusterLv3Proj(
			args.owner, args.extraData[0], args.pos, args.xDir, args.netId, altPlayer: args.player
		);
	}
}

public class ProtoBusterLv4Proj : Projectile {
	public ProtoBusterLv4Proj(
		Actor owner, int type, Point pos, int xDir, ushort? netId, 
		bool rpc = false, Player? altPlayer = null
	) : base(
		pos, xDir, owner, "proto_chargeshot_proj", netId, altPlayer
	) {
		fadeSprite = "proto_chargeshot_proj_fade";
		fadeOnAutoDestroy = true;
		maxTime = 0.5f;
		projId = (int)BluesProjIds.BusterLV4;

		vel.x = 325 * xDir;
		damager.damage = 4;
		damager.flinch = Global.defFlinch;
		damager.hitCooldown = 0.5f;

		if (type == 1) {
			damager.flinch = Global.superFlinch;
		}

		if (rpc) {
			byte[] extraArgs = new byte[] { (byte)type };

			rpcCreate(pos, owner, ownerPlayer, netId, xDir, extraArgs);
		}
	}

	public static Projectile rpcInvoke(ProjParameters args) {
		return new ProtoBusterLv4Proj(
			args.owner, args.extraData[0], args.pos, args.xDir, args.netId, altPlayer: args.player
		);
	}
}
