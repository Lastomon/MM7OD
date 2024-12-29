namespace MMXOnline;

public class LTank {

	public const int maxAmmo = 28;
	public float ammo = 28;
	public bool inUse;

	public LTank() {

	}
	public void use(Player player, Character character) {
		Blues? blues = character as Blues;
		blues.isUsingLTank = true;

		blues?.addETankHealth(player.maxHealth);
		blues?.healCore(blues.coreMaxAmmo);
		blues?.healShield(blues.shieldMaxHP);
	}
}