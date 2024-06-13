﻿using System.Collections.Generic;
using System.Linq;

namespace MMXOnline;

public class AddBotMenu : IMainMenu {
	public int selectArrowPosY;
	public IMainMenu previous;

	public static int botCharNum = 5;
	public static int botTeamNum = -1;

	public List<Point> optionPoses = new List<Point>()
	{
			new Point(30, 100),
			new Point(30, 120),
		};
	public int ySep = 10;

	public AddBotMenu(IMainMenu mainMenu) {
		previous = mainMenu;
	}

	public void update() {
		Helpers.menuUpDown(ref selectArrowPosY, 5, 5);

		if (selectArrowPosY == 0) {
			if (Global.input.isPressedMenu(Control.MenuLeft)) {
				botCharNum--;
				if (botCharNum < 5) botCharNum = 5;
			} else if (Global.input.isPressedMenu(Control.MenuRight)) {
				botCharNum++;
				if (botCharNum >= 5) botCharNum = 5;
			}
		}
		if (selectArrowPosY == 1 && teamOptionEnabled()) {
			if (Global.input.isPressedMenu(Control.MenuLeft)) {
				botTeamNum--;
				if (botTeamNum < -1) botTeamNum = -1;
			} else if (Global.input.isPressedMenu(Control.MenuRight)) {
				botTeamNum++;
				if (botTeamNum > 1) botTeamNum = 1;
			}
		}

		if (Global.input.isPressedMenu(Control.MenuConfirm)) {
			if (Global.serverClient != null) {
				RPC.addBot.sendRpc(botCharNum, botTeamNum);
			} else {
				int id = 0;
				int charNum = botCharNum;
				int alliance = botTeamNum;
				if (charNum == -1) charNum = 5;
				if (alliance == -1) {
					if (Global.level.gameMode.isTeamMode) {
						alliance = Server.getMatchInitAutobalanceTeam(Global.level.players, Global.level.teamNum);
					}
				}

				for (int i = 0; i < Global.level.players.Count + 1; i++) {
					if (!Global.level.players.Any(p => p.id == i)) {
						id = i;
						if (!Global.level.gameMode.isTeamMode) {
							alliance = id;
						}
						break;
					}
				}

				var cpu = new Player("CPU" + id.ToString(), id, 6, null, true, true, alliance, new Input(true), null);
				Global.level.players.Add(cpu);
				Global.level.gameMode.chatMenu.addChatEntry(new ChatEntry("Added bot " + cpu.name + ".", null, null, true));
			}
			Menu.change(previous);
		} else if (Global.input.isPressedMenu(Control.MenuBack)) {
			Menu.change(previous);
		}
	}

	public void render() {
		DrawWrappers.DrawTextureHUD(Global.textures["pausemenu"], 0, 0);
		Global.sprites["cursor"].drawToHUD(0, 15, optionPoses[selectArrowPosY].y + 5);

		Fonts.drawText(FontType.BlueMenu, "Add Bot", Global.halfScreenW, 15, Alignment.Center);


		string botTeamStr = "(Auto)";
		if (botTeamNum == GameMode.blueAlliance) botTeamStr = "Blue";
		else if (botTeamNum == GameMode.redAlliance) botTeamStr = "Red";

		string botCharStr = "(Random)";
		if (botCharNum == 0) botCharStr = "Mega Man X";
		else if (botCharNum == 1) botCharStr = "Zero";
		else if (botCharNum == 2) botCharStr = "Vile";
		else if (botCharNum == 3) botCharStr = "Axl";
		else if (botCharNum == 4) botCharStr = "Sigma";
		else if (botCharNum == 5) botCharStr = "Rockman";

		Fonts.drawText(
			FontType.Grey, "CHARACTER: " + botCharStr,
			optionPoses[0].x, optionPoses[0].y, selected: selectArrowPosY == 0
		);
		Fonts.drawText(
			teamOptionEnabled() ? FontType.Grey : FontType.Grey, "TEAM: " + botTeamStr,
			optionPoses[1].x, optionPoses[1].y,
			selected: selectArrowPosY == 1
		);

		Fonts.drawTextEX(
			FontType.Grey, "Left/Right: Change, [OK]: Add, [BACK]: Back",
			Global.screenW * 0.5f, 200, Alignment.Center
		);
	}

	public bool teamOptionEnabled() {
		if (!Global.level.gameMode.isTeamMode) return false;
		if (Global.serverClient == null) return true;
		if (Global.level?.server != null && Global.level.server.hidden) return true;
		return false;
	}
}
