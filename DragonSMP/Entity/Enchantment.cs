using System.Collections.Generic;

//May be removed later.
//Reason for this setup, to check what items this enchantment can go on.
namespace DragonSpire
{
	#region Enchantment IDs
	public enum VanillaEnchantmentType : byte
	{
		Protection = 0,
		FireProtection = 1,
		FeatherFalling = 2,
		BlastProtection = 3,
		ProjectileProtection = 4,
		Respiration = 5,
		AquaAffinity = 6,
		Thorns = 7,
		//8 - 15 unused
		Sharpness = 16,
		Smite = 17,
		BaneArthropods = 18,
		Knockback = 19,
		FireAspect = 20,
		Looting = 21,
		//22 - 31 unused
		Efficiency = 32,
		SilkTouch = 33,
		Unbreaking = 34,
		Fortune = 35,
		//36 - 47 unused
		Power = 48,
		Punch = 49,
		Flame = 50,
		Infinity = 51,
	}
	#endregion

	public class Enchantment
	{
		//Dictionary containing all Enchanment ids and an array of item ids that the enchantment can be applied to.
		internal static Dictionary<byte, short[]> Enchantments = new Dictionary<byte, short[]>();

		byte ID;
		short Level;

		public Enchantment(byte id)
		{
			ID = id;
			Level = 1;
		}

		public Enchantment(byte id, short level)
		{
			ID = id;
			Level = level;
		}
	}
}