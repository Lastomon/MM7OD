using System;
using System.Collections.Generic;

namespace MMXOnline;

public class Met : NeutralEnemy {
	public Met(
		Point pos, ushort netId, bool isLocal, bool addToLevel = true
	) : base(
		pos, netId, isLocal, !addToLevel
	) {

	}

	public override string getSprite(string spriteName) {
		return "met_" + spriteName;
	}

	public override void update() {
		base.update();

		if (state is NeIdle) invincibleFlag = true;
	}
}
