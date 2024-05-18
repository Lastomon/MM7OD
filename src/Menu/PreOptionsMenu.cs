﻿using System;

namespace MMXOnline;

public class PreOptionsMenu : IMainMenu {
	public int selectY;
	public int[] optionPos = new int[9];
	public int lineH = 10;
	public IMainMenu prevMenu;
	public string message;
	public Action yesAction;
	public bool inGame;
	public bool isAxl;
	public float startX = 32;

	public PreOptionsMenu(IMainMenu prevMenu, bool inGame) {
		this.prevMenu = prevMenu;
		this.inGame = inGame;
		for (int i = 0; i < optionPos.Length; i++) {
			optionPos[i] = 35 + lineH * i;
		}
	}

	public void update() {
		Helpers.menuUpDown(ref selectY, 0, 3);
		if (Global.input.isPressedMenu(Control.MenuConfirm)) {
			int? charNum = null;
			if (selectY == 3) charNum = 5;


			Menu.change(new OptionsMenu(this, inGame, charNum, selectY));
		} else if (Global.input.isPressedMenu(Control.MenuBack)) {
			Menu.change(prevMenu);
		}
	}

	public void render() {
		if (!inGame) {
			DrawWrappers.DrawTextureHUD(Global.textures["settingsmenu"], 0, 0);
			//DrawWrappers.DrawTextureMenu(
			//Global.textures["cursor"], 20, topLeft.y + ySpace + (selectArrowPosY * ySpace)
			//);
		} else {
			DrawWrappers.DrawTextureHUD(Global.textures["pausemenu"], 0, 0);
		}
		Global.sprites["cursor"].drawToHUD(0, startX - 5, 39 + (selectY * lineH));
		FontType tileFont = FontType.BlueMenu;
		FontType menuFont = FontType.Grey;
		if (inGame) {
			tileFont = FontType.BlueMenu;
			menuFont = FontType.Grey;
		}

		Fonts.drawText(tileFont, "SETTINGS", Global.screenW * 0.5f, 20, Alignment.Center);

		Fonts.drawText(menuFont, "GENERAL SETTINGS", startX, optionPos[0], selected: selectY == 0);
		Fonts.drawText(menuFont, "GAMEPLAY SETTINGS", startX, optionPos[1], selected: selectY == 1);
		Fonts.drawText(menuFont, "GRAPHICS SETTINGS", startX, optionPos[2], selected: selectY == 2);
		//Fonts.drawText(menuFont, "X settings", startX, optionPos[3], selected: selectY == 3);
		//Fonts.drawText(menuFont, "Zero settings", startX, optionPos[4], selected: selectY == 4);
		//Fonts.drawText(menuFont, "Vile settings", startX, optionPos[5], selected: selectY == 5);
		//Fonts.drawText(menuFont, "Axl settings", startX, optionPos[6], selected: selectY == 6);
		//Fonts.drawText(menuFont, "Sigma settings", startX, optionPos[7], selected: selectY == 7);
		Fonts.drawText(menuFont, "ROCKMAN SETTINGS", startX, optionPos[3], selected: selectY == 3);

		Fonts.drawTextEX(FontType.Grey, "[OK]: Choose, [BACK]: Back", Global.halfScreenW, 198, Alignment.Center);
	}
}
