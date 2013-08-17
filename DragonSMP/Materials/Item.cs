namespace DragonSpire
{
	public abstract class Item : Material
	{
		/// <summary>
		/// Whether or not this item can be stacked.
		/// We don't need to make this abstract since we dynamically set the value based upon MaximumStack.
		/// </summary>
		public bool isStackable { get { return (MaximumStack > 1); } }
		/// <summary>
		/// The maximum amount that can be in one stack of this item.
		/// </summary>
		public abstract byte MaximumStack { get; }

		/// <summary>
		/// Whether or not you can enchant this item.
		/// </summary>
		public virtual bool isEnchantable { get { return false; } }
		/// <summary>
		/// Whether or not this item is repairable
		/// </summary>
		public virtual bool isRepairable { get { return false; } }
		/// <summary>
		/// Whether or not this item is consumable
		/// </summary>
		public virtual bool isConsummable { get { return false; } }

		/// <summary>
		/// The default metadata or damage of this item
		/// </summary>
		public virtual byte defaultMeta { get { return 0; } }

		/// <summary>
		/// The amount of modification this item does to the rate the player digs (this is multiplied into the dig speed)
		/// </summary>
		public virtual float digModifier { get { return 1; } }
		/// <summary>
		/// The amount of modification this item does to the rate the player mines (this is multiplied into the mine speed)
		/// </summary>
		public virtual float mineModifier { get { return 1; } }
		/// <summary>
		/// The amount of modification this item does to the rate the player chops (this is multiplied into the chop speed)
		/// </summary>
		public virtual float chopModifier { get { return 1; } }
		/// <summary>
		/// The amount of modification this item does to the rate the player deals damage (this is multiplied into the attack rate)
		/// </summary>
		public virtual float attackModifier { get { return 1; } }

		public virtual void OnRightClickAir() { }
		public virtual void OnRightClickEntity() { }
		public virtual bool OnRightClickBlock() { return false; }

		public virtual void OnLeftClickAir() { }
		public virtual void OnLeftClickEntity() { }
		public virtual void OnLeftClickBlock() { }

		public virtual void OnConsume() { }
	}
}