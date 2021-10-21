using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : Building {
	protected struct ItemProbability {
		public System.Type itemType;
		public float probability; // 0 = 0%; 1 = 100%
		public ItemProbability (System.Type itemType, float probability) {
			this.itemType = itemType;
			this.probability = probability;
		}
	}

	public ushort shopKeeperID = 0;
	public Character shopkeeper {
		get {
			return Game.current.GetCharacterByID (shopKeeperID);
		}
	}
	public List<Item> inventory = new List<Item> ();
	public int gold = 0;

	Vector2Int shopKeeperLocation = Vector2Int.zero;

	public Shop (int x, int y) : base (x, y) {
		
	}

	//allows the player to buy or sell the given item
	public void Buy (Item i) {
		Game.current.playerCharacter.inventory.gold -= i.cost;
		gold += i.cost;
		Game.current.playerCharacter.inventory.Add (i);
		inventory.Remove (i);
	}
	public virtual void Buy (int i) {
		Game.current.playerCharacter.inventory.gold -= inventory [i].cost;
		gold += inventory [i].cost;
		Game.current.playerCharacter.inventory.items.Add (inventory [i]);
		inventory.RemoveAt (i);
	}
	public virtual void Sell (Item i) {
		Game.current.playerCharacter.inventory.gold += i.cost;
		gold -= i.cost;
		inventory.Add (i);
		Game.current.playerCharacter.inventory.Remove (i);
	}
	public virtual void Sell (int i) {
		Game.current.playerCharacter.inventory.gold += Game.current.playerCharacter.inventory.items [i].cost;
		gold -= Game.current.playerCharacter.inventory.items [i].cost;
		inventory.Add (Game.current.playerCharacter.inventory.items [i]);
		Game.current.playerCharacter.inventory.items.RemoveAt (i);
	}

	protected override void GenerateBuilding () {
		GenerateEmpty ();
		PlaceCounter ();
		GenerateShopKeeper ();
		GenerateShop ();
		GenerateInventory ();
	}

	protected virtual void GenerateShop () {

	}

	protected virtual void GenerateInventory () {

	}

	void PlaceCounter () {
		//pick a random side for the counter to stem from
		//0 = left; 1 = right
		int side = Random.Range (0, 2);
		int inc = (side == 0) ? 1 : -1;
		if (size.x - 2 <= 4) {
			for (int x = side * (size.x - 3) + 1; (side == 0) ? x < size.x - 1 : x > 1; x += inc) {
				tiles [x, size.y - 3].underType = Tile.UnderType.wall_stone_red_top_bottom;
			}
		} else {
			for (int x = side * (size.x - 3) + 1; (side == 0) ? x <= 4 : x >= size.x - 5; x += inc) {
				tiles [x, size.y - 3].underType = Tile.UnderType.wall_stone_red_top_bottom;
			}
		}
		shopKeeperLocation = new Vector2Int ((side == 0) ? 2 : size.x - 3, size.y - 2);
	}

	void GenerateShopKeeper () {
		Character c = Character.CreateCharacter (Character.Race.man, Character.Alignment.good, typeof(ShopKeeperAI));
		c.mapID = Game.current.map.id;
		c.position = position + shopKeeperLocation;
		c.name = "Shop Keeper";

		c.characterInfo.Add ("townID", (object)townID);
		c.characterInfo.Add ("shopID", (object)id);
		Game.current.AddCharacter (c);
		shopKeeperID = c.id;
	}
}