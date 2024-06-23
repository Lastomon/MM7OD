using System;
using System.Collections.Generic;

namespace MMXOnline;

public class GyroAttack : Weapon {
    public GyroAttack() : base() {
		displayName = "Gyro Attack";
		descriptionV2 = "Temp.";
		defaultAmmoUse = 2;

        index = (int)RockWeaponIds.GyroAttack;
        fireRateFrames = 60;

    }

    public override float getAmmoUsage(int chargeLevel) {
        return defaultAmmoUse;
    }

	public override void shoot(Character character, params int[] args) {
		base.shoot(character, args);
        Point shootPos = character.getShootPos();
        int xDir = character.getShootXDir();

        new GyroAttackProj(this, shootPos, xDir, character.player, character.player.getNextActorNetId(), true);
	}
}



public class GyroAttackProj : Projectile {

    bool changedDir;
    Player player;
    const float projSpeed = 180;

    public GyroAttackProj(Weapon weapon, Point pos, int xDir, Player player, ushort? netId, bool rpc = false) : 
    base(weapon, pos, xDir, projSpeed, 2, player, "gyro_attack_proj", 0, 0, netId, player.ownedByLocalPlayer) {
        maxTime = 1f;
        projId = (int)BluesProjIds.GyroAttack;
        this.player =  player;
        canBeLocal = false;
        
    }


    public override void update() {
        base.update();

        if (!changedDir) {
            if (player.input.isPressed(Control.Up, player)) {
                base.vel = new Point(0, -projSpeed);
                changedDir = true;
            }
            else if (player.input.isPressed(Control.Down, player)) {
                base.vel = new Point(0, projSpeed);
                changedDir = true;
            } 
        }
    }
}
