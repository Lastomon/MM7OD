using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;

namespace MMXOnline;

public class BassShoot : CharState {
	Bass bass = null!;

	public BassShoot() : base("not_a_real_sprite") {
		attackCtrl = true;
		airMove = true;
		useDashJumpSpeed = true;
		canJump = true;
		canStopJump = true;
		//airSpriteReset = true;
	}

	public override void update() {
		base.update();
		if (player.dashPressed(out string dashControl) && character.grounded) {
			if (bass.canUseTBladeDash()) {
				bass.changeState(new TenguBladeDash(), true);
			} else {
				bass.changeState(new Dash(dashControl), true);
			}
			return;
		}
		if (stateFrames >= 16) {
			bass.changeToIdleOrFall();
			return;
		}
	}

	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		bass = character as Bass ?? throw new NullReferenceException();

		sprite = getShootSprite(bass.getShootYDir(), bass.currentWeapon ?? throw new NullReferenceException());
		landSprite = sprite;
		airSprite = "jump_" + sprite;
		//fallSprite = "fall_" + sprite;

		if (!bass.grounded || bass.vel.y < 0) {
			string tempSprite = airSprite;
			/* if (bass.vel.y >= 0) {
				tempSprite = fallSprite;
			} */
			if (bass.sprite.name != bass.getSprite("tempSprite")) {
				bass.changeSpriteFromName(tempSprite, false);
			}
		} else {
			bass.changeSpriteFromName(sprite, true);
			bass.sprite.restart();
		}
	}

	public static string getShootSprite(int dir, Weapon wep) {
		if (wep is not BassBuster
			and not MagicCard
			and not WaveBurner
			and not RemoteMine
		) {
			return "shoot";
		}
		if (wep is RemoteMine && dir == -2) {
			dir = -1;
		}
		if (wep is MagicCard && dir == -1) {
			dir = -2;
		}
		return dir switch {
			-2 => "shoot_up",
			-1 => "shoot_up_diag",
			0 => "shoot",
			1 or 2 => "shoot_down_diag",
			_ => "shoot"
		};
	}
}


public class BassShootLadder : CharState {

	Bass bass = null!;
	public Ladder ladder;
	float midX; 
	public BassShootLadder(Ladder ladder) : base("spritent") {
		normalCtrl = false;
		attackCtrl = true;
		canJump = true;
		canStopJump = true;
		this.ladder = ladder;
	}

	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		bass = character as Bass ?? throw new NullReferenceException();
		bass.useGravity = false;

		sprite = getShootSprite(bass.getShootYDir(), bass.currentWeapon ?? throw new NullReferenceException());
 
		bass.changeSpriteFromName(sprite, true);
		bass.sprite.restart();
	}

	public override void update() {
		base.update();
	
		if (stateFrames >= 16) {
			float midX = ladder.collider.shape.getRect().center().x;
			character.changeState(new LadderClimb(ladder, midX), true);
			return;
		}
	}

	public static string getShootSprite(int dir, Weapon wep) {
		if (wep is not BassBuster &&
			wep is not MagicCard) return "ladder_shoot";

		else if (wep is MagicCard) {
			if (dir < 0) return "ladder_shoot_up";
			return "ladder_shoot";
		}

		return dir switch {
			-2 => "ladder_shoot_up",
			-1 => "ladder_shoot_up_diag",
			0 => "ladder_shoot",
			1 or 2 => "ladder_shoot_down_diag",
			_ => "ladder_shoot"
		};
	}
}


public class DashEnd : CharState {
	public DashEnd() : base("dash_end", "") {
		normalCtrl = true;
		attackCtrl = true;
	}

	public override void update() {
		base.update();

		float inputXDir = player.input.getInputDir(player).x;

		if (character.isAnimOver() || inputXDir != 0) {
			character.changeToIdleOrFall();
			return;
		}
	}
}


public class SuperBassStart : CharState {

	Bass bass = null!;
	Anim? treble;
	Anim? aura;
	int phase;
	Point headPos;
	bool jumped;
	bool drawSquare;
	float s; // Square size
	float a; // Square rotation
	float t; // Square transparency
	float tIncrease = 6;
	int endTime;

	public SuperBassStart() : base("lbolt") {
		normalCtrl = false;
		attackCtrl = false;
		useGravity = false;
	}

	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		character.stopMoving();
		headPos = character.pos.addxy(0, -32);
		bass = character as Bass ?? throw new NullReferenceException();
		bass.frameSpeed = 0;

		//Spawns Treble.
		Point spawnPos = character.pos.addxy(character.xDir * -32, -200);
		treble = new Anim(
			spawnPos, "treble_warp_beam", character.xDir, null, false, true) 
			{ vel = new Point(0, 480), zIndex = ZIndex.Character + 10 };

		character.playSound("warpin", sendRpc: true);
	}

	public override void onExit(CharState newState) {
		base.onExit(newState);
		aura?.destroySelf();
	}

	public override void update() {
		base.update();

		if (drawSquare) {
			s += 0.75f;
			a += 4;
			t += tIncrease;
			if (t > 255) t = 255;
			else if (t < 0) t = 0;
		}
		if (phase == 6) endTime++;

		if (treble == null) return;

		switch (phase) {
			//Lands
			case 0:
				if (treble.pos.y >= bass.pos.y) {
					treble.stopMoving();
					treble.changeSprite("treble_warp_in", true);
					phase = 1;	
				}
				break;
			//guau guau
			case 1:
				if (treble.isAnimOver()) {
					treble.changeSprite("treble_bark", true);
					character.playSound("treble", sendRpc: true);
					phase = 2;
				}
				break;
			//Jumps
			case 2:
				if (treble.isAnimOver()) {
						treble.stopMoving();
						treble.changeSprite("treble_jump", true);
						Point landPos = character.getShootPos();
						treble.move(treble.pos.directionToNorm(landPos).times(300));
						headPos = landPos;
						if (treble.pos.distanceTo(landPos) < 10) phase = 3;
					}
				break;
			//Transforms and starts drawing the square
			case 3:				
				treble.stopMoving();
				treble.useGravity = false;
				treble.changeSprite("treble_transform", true);
				character.playSound("treble_boost_activate");
				phase = 4;
				drawSquare = true;
				
				break;
			//Despawns treble and uses hypermode
			case 4:
				if (s >= 70) {
					tIncrease = -12.75f;
					treble.destroySelf();
					bass.frameSpeed = 1;
					bass.setSuperBass();
					bass.changeSpriteFromName("enter", true);

					aura = new Anim(
						bass.pos, "sbass_aura", bass.xDir, null, false, true
					) { zIndex = ZIndex.Character - 10 };

					phase = 5;
				}
				break;
			//Stops drawing the square
			case 5:
				if (s >= 90) {
					drawSquare = false;
					new SuperBassPilar(character.pos);
					character.playSound("super_bass_aura", sendRpc: true);
					phase = 6;
				} 
				break;
		}

		if (endTime >= 60) character.changeToIdleOrFall();
	}

	public override void render(float x, float y) {
		base.render(x,y);
		if (!drawSquare) return;

		Point center = headPos;
		Point[] points = new Point[4];
		Color color = new Color(255,255,255, (byte)t);

		for (int i = 0; i < 4; i++) {
			points[i] = center.add(
				Point.createFromByteAngle(a + (i * 64)).times(s)
			);
		}

		DrawWrappers.DrawPolygon(points.ToList(), color, true, ZIndex.Foreground + 10);
	}
}


public class SuperBassPilar : Effect {

	float time;
	float maxTime = 40;

	public SuperBassPilar(Point pos) : base(pos) {}
	
	public override void update() {
		// Because we do not have this built-in for the Effect class.
		if (time >= maxTime) {
			destroySelf();
		}
		// Time added at the end of update because we count from 0.
		time += Global.speedMul;
	}

	public override void render(float x, float y) {
		float progress = time / maxTime;
		float size = 300 * progress;
		float t = 255 * (1 - progress); 
		Color color = new Color(255,255,255, (byte)t);

		DrawWrappers.DrawRect(
			pos.x - size, pos.y - 200, pos.x + size, pos.y + 200,
			true, color, 1, ZIndex.Character + 10
		);
	}
}


public class EnergyCharge : CharState {

	Bass bass = null!;
	Anim? aura;

	public EnergyCharge() : base("enter") {
		normalCtrl = false;
		attackCtrl = true;
	}

	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		bass = character as Bass ?? throw new NullReferenceException();
		aura = new Anim(
			bass.pos, "sbass_aura", bass.xDir, null, false, true
		) { zIndex = ZIndex.Character - 10 };
		bass.stopMoving();
		bass.gravityModifier = 0.1f;
	}

	public override void onExit(CharState newState) {
		base.onExit(newState);
		aura?.destroySelf();
		bass.gravityModifier = 1;
	}

	public override void update() {
		base.update();

		if (!player.input.isHeld(Control.Special2, player)) {
			bass.changeToIdleOrFall();
			return;
		}

		aura?.changePos(bass.pos);

		if (stateFrames % 15 == 0 && stateFrames > 0) {
			bass.playSound("heal");
			bass.evilEnergy[bass.phase - 1]++;
			if (bass.evilEnergy[bass.phase - 1] >= Bass.MaxEvilEnergy) {
				bass.nextPhase(bass.phase + 1);
				bass.changeState(new EnergyIncrease());
			}
		}	
	}
}


public class EnergyIncrease : CharState {
	public EnergyIncrease() : base("enter") {
		normalCtrl = false;
		attackCtrl = false;
		useGravity = false;
	}

	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		character.stopMoving();
		character.gravityModifier = 0.1f;
	}

	public override void update() {
		base.update();

		if (stateFrames >= 30) character.changeToIdleOrFall();
	}

	public override void onExit(CharState newState) {
		base.onExit(newState);
		character.gravityModifier = 1;
	}
}


public class BassFly : CharState {

	public Point flyVel;
	float flyVelAcc = 500;
	float flyVelMaxSpeed = 200;
	public float fallY;
	Bass bass = null!;

	public BassFly() : base("fly", "fly_shoot") {
		exitOnLanding = true;
		normalCtrl = true;
		attackCtrl = true;
		useGravity = false;
	}

	public override void update() {
		base.update();
		if (player == null) return;

		if (character.flag != null) {
			character.changeToIdleOrFall();
			return;
		}

		if (Global.level.checkTerrainCollisionOnce(character, 0, -character.getYMod()) != null && character.vel.y * character.getYMod() < 0) {
			character.vel.y = 0;
		}

		Point move = getFlightMove();

		if (move.magnitude > 0) {
			character.move(move);
		}

		if (character.isUnderwater()) {
			character.frameIndex = 0;
			character.frameSpeed = 0;
		}

		bass.flyTime += getFlyConsume();
		
		if (bass.flyTime >= Bass.MaxFlyTime) character.changeToIdleOrFall();
	}

	public float getFlyConsume() {
		Point inputDir = bass.isSoftLocked() ? Point.zero : player.input.getInputDir(player);

		if (inputDir.y == -1) return 1.5f;
		return 1;
	}

	public Point getFlightMove() {
		bool isSoftLocked = character.isSoftLocked();

		var inputDir = isSoftLocked ? Point.zero : player.input.getInputDir(player);

		if (inputDir.x > 0) character.xDir = 1;
		if (inputDir.x < 0) character.xDir = -1;

		if (inputDir.isZero()) {
			flyVel = Point.lerp(flyVel, Point.zero, Global.spf * 5f);
		} else {
			float ang = flyVel.angleWith(inputDir);
			float modifier = Math.Clamp(ang / 90f, 1, 2);

			flyVel.inc(inputDir.times(Global.spf * flyVelAcc * modifier));
			if (flyVel.magnitude > flyVelMaxSpeed) {
				flyVel = flyVel.normalize().times(flyVelMaxSpeed);
			}
		}

		var hit = character.checkCollision(flyVel.x * Global.spf, flyVel.y * Global.spf);
		if (hit != null && !hit.isGroundHit()) {
			flyVel = flyVel.subtract(flyVel.project(hit.getNormalSafe()));
		}

		return flyVel.addxy(0, fallY);
	}

	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		bass = character as Bass ?? throw new NullReferenceException();

		float flyVelX = 0;
		if (character.isDashing && character.deltaPos.x != 0) {
			flyVelX = character.xDir * character.getDashSpeed() * 0.5f;
		} else if (character.deltaPos.x != 0) {
			flyVelX = character.xDir * character.getRunSpeed() * 0.5f;
		}

		float flyVelY = 0;
		if (character.vel.y < 0) {
			flyVelY = character.vel.y;
		}

		flyVel = new Point(flyVelX, flyVelY);
		if (flyVel.magnitude > flyVelMaxSpeed) flyVel = flyVel.normalize().times(flyVelMaxSpeed);

		if (character.vel.y > 0) {
			fallY = character.vel.y;
		}

		character.isDashing = false;
		character.stopMoving();
	}

	public override void onExit(CharState newState) {
		base.onExit(newState);
		character.stopMoving();
	}
}


public class BassKick : CharState {

	bool jumped;
	int airTime;

	public BassKick() : base("kick") {
		normalCtrl = false;
		attackCtrl = false;
	}

	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		character.stopMovingWeak();
		character.vel.x = character.xDir * 120;
	}

	public override void update() {
		base.update();

		if (!jumped) {

			if (character.frameIndex >= 2) {
				jumped = true;
				character.vel.y = -character.getJumpPower();
			} 
		}

		character.vel.x = Helpers.lerp(character.vel.x, 0, Global.spf * 5);

		if (jumped) {
			if (character.vel.y > 0) {
				character.stopMoving();
				character.useGravity = false;
			}

			airTime++;
		}

		if (airTime >= 40) {
			character.useGravity = true;
			character.changeToIdleOrFall();
		}
	}
}


public class SonicCrusher : CharState {

	Bass bass = null!;
	float? speed;
	float? oldSpeed;

	public SonicCrusher(float? oldSpeed = null) : base("soniccrusher") {
		normalCtrl = false;
		attackCtrl = true;
		useGravity = false;
		this.oldSpeed = oldSpeed;
	}

	public override void onEnter(CharState oldState) {
		base.onEnter(oldState);
		bass = character as Bass ?? throw new NullReferenceException();
		float full = Bass.MaxFlyTime;
		float half = full / 2;

		if (
			oldSpeed != null && oldSpeed > speed && 
			bass.flyTime < half
		) {
			speed = oldSpeed;
		} else if (bass.flyTime < half) {
			speed = 180;
		} else if (bass.flyTime >= half && bass.flyTime < full) {
			speed = 120;
		} else {
			speed = 60;
		}
	}

	public override void update () {
		base.update();

		if (speed != null) character.move(new Point(character.xDir * speed.Value, 0));

		float depth = 24;
		if (character.checkCollision(0, depth) != null && stateFrames % 6 == 0) {
			Point offset = new Point(character.xDir * -16, 0);
			var groundPos = Global.level.getGroundPosNoKillzone(character.pos.add(offset), depth);

			 if (groundPos != null) {
				new Anim(
					groundPos.Value,
					"dust", character.xDir, null, true, true
				) { vel = new Point(0, -60) };
			}
		} 

		bass.flyTime += 1.25f;
		if (stateFrames >= 32) character.changeToIdleOrFall();
	}
}
