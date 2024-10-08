﻿using System;

namespace MMXOnline;

public class PreLoadoutMenu : IMainMenu {
	public int selectY;
	public int[] optionPos = {
		50,
		70,
		90,
		110,
		130,
		150,
		170
	};
	public IMainMenu prevMenu;
	public string message;
	public Action yesAction;
	public bool inGame;
	public bool isAxl;
	public float startX = 150;

	public PreLoadoutMenu(IMainMenu prevMenu) {
		this.prevMenu = prevMenu;
		selectY = 0;
	}

	public void update() {
		Helpers.menuUpDown(ref selectY, 0, 2);
		if (Global.input.isPressedMenu(Control.MenuConfirm)) {
			/*if (selectY == 0) {
				Menu.change(new SelectWeaponMenu(this, false));
			}
			if (selectY == 1) {
				Menu.change(new SelectZeroWeaponMenu(this, false));
			}
			if (selectY == 2) {
				Menu.change(new SelectVileWeaponMenu(this, false));
			}
			if (selectY == 3) {
				Menu.change(new SelectAxlWeaponMenu(this, false));
			}
			if (selectY == 4) {
				Menu.change(new SelectSigmaWeaponMenu(this, false));
			}
			if (selectY == (int)CharIds.PunchyZero) {
				Menu.change(new SelectPunchyZeroWeaponMenu(this, false));
			}
			*/
			if (selectY == 0) {
				Menu.change(new SelectRockWeaponMenu(this, false));
			}
			if (selectY == 1) {
				Menu.change(new BluesWeaponMenu(this, false));
			}
			if (selectY == 2) {
				Menu.change(new BassWeaponMenu(this, false));
			}
		} else if (Global.input.isPressedMenu(Control.MenuBack)) {
			Menu.change(prevMenu);
		}
	}

	public void render() {
		if (!inGame) {
			DrawWrappers.DrawTextureHUD(Global.textures["menubackground"], 0, 0);
			//DrawWrappers.DrawTextureMenu(Global.textures["cursor"], 20, topLeft.y + ySpace + (selectArrowPosY * ySpace));
			Global.sprites["cursor"].drawToHUD(0, startX - 10, 53 + (selectY * 20));
		} else {
			DrawWrappers.DrawTextureHUD(Global.textures["pausemenu"], 0, 0);
			Global.sprites["cursor"].drawToHUD(0, startX - 10, 53 + (selectY * 20));
		}

		Fonts.drawText(FontType.BlueMenu, "SELECT CHARACTER LOADOUT", Global.screenW * 0.5f, 20, Alignment.Center);

		Fonts.drawText(FontType.Grey, "ROCKMAN LOADOUT", startX, optionPos[0], selected: selectY == 0);
		Fonts.drawText(FontType.Grey, "PROTOMAN LOADOUT", startX, optionPos[1], selected: selectY == 1);
		Fonts.drawText(FontType.Grey, "BASS LOADOUT", startX, optionPos[2], selected: selectY == 2);

		Fonts.drawTextEX(FontType.Grey, "[OK]: Choose, [BACK]: Back", Global.halfScreenW, 200, Alignment.Center);
	}
}
