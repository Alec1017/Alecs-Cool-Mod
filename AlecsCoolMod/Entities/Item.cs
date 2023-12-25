using LethalLib.Modules;
using System.Collections.Generic;
using UnityEngine;

namespace AlecsCoolMod.Entities
{
    
    public class ItemRegistry
    {
        // All items to add to the game
        public static List<ModdedItem> ModdedItems;

        // All item prefabs
        public static Dictionary<string, GameObject> Prefabs = new Dictionary<string, GameObject>();

        // Loads all item assets into the game
        public static void LoadItems(AssetBundle assets)
        {
            // Instantiate the item list
            ModdedItems = new List<ModdedItem>()
            {
                ModdedScrapItem.Add("Cube", "Cube.prefab", Levels.LevelTypes.All, 20)
            };

            foreach (var item in ModdedItems)
            {
                // get the item asset
                Item itemAsset = assets.LoadAsset<Item>(item.itemPath);

                // Store the item prefab in a dictionary
                Prefabs.Add(item.name, itemAsset.spawnPrefab);

                // Register the scrap item
                Items.RegisterScrap(itemAsset, ((ModdedScrapItem)item).rarity, ((ModdedScrapItem)item).levelType);
            }
        }
    }

    public class ModdedItem
    {
        // Name of the item.
        public string name = "";

        // Path to the item in the asset bundle
        public string itemPath = "";

        // Constructor for the generic item
        public ModdedItem(string name, string itemPath)
        {
            this.name = name;
            this.itemPath = itemPath;
        }

        // Creates an item and returns it
        public static ModdedItem Add(string name, string itemPath)
        {
            // Create an item
            ModdedItem item = new ModdedItem(name, itemPath);
            
            // Return the item
            return item;
        }
    }

    public class  ModdedScrapItem : ModdedItem
    {
        // Defines the planets that this item can be found
        public Levels.LevelTypes levelType = Levels.LevelTypes.All;
    
        // Rarity of the item
        public int rarity = 0;

        // Constructor for the generic scrap item
        public ModdedScrapItem(string name, string itemPath, Levels.LevelTypes levelType, int rarity) : base(name, itemPath) 
        {
            this.levelType = levelType;
            this.rarity = rarity;
        }

        // Creates a scrap item and returns it
        public static ModdedScrapItem Add(string name, string itemPath, Levels.LevelTypes levelType, int rarity)
        {
            // Create the scrap item
            ModdedScrapItem scrapItem = new ModdedScrapItem(name, itemPath, levelType, rarity);

            // Return the scrap item
            return scrapItem;
        }
       
    }
}
