using System.Collections.Generic;
using Scenes.GameState.Scripts.Items;
using SuperTiled2Unity;
using UnityEngine;

namespace Scenes.GameState.Scripts.Factories {
    public class ItemsFactory { 
        static Dictionary<string, string> itemNames = new Dictionary<string, string>();
        internal BodyFactory bodyFactory;
        Dictionary<int, Item> container;
        private int _itemsCounter = 0;
        public int ItemsCounter {
            get => _itemsCounter;
            set => _itemsCounter = value;
        }

        public ItemsFactory(Dictionary<int, Item> container, BodyFactory factory) {
            bodyFactory = factory;
            this.container = container;
        }

        

        public Item GetItem(string itemId) {
            if (itemNames.Count == 0)
                init();
            if (itemId == null)
                return null;
            Item createdItem;
            switch (itemId) {
                // case "deagle_44" -> {
                //     Pistol gun = new Pistol(uid, itemId, getNameForID(itemId));
                //     gun.setData(unBox, bodyFactory, hud, gameStage, this);
                //     createdItem = gun;
                // }
                // case "pistol_magazine" -> {
                //     GunMagazine magaz = new GunMagazine(uid, itemId, getNameForID(itemId));
                //     magaz.setData(unBox, bodyFactory, hud, gameStage, this);
                //     magaz.addGunTypes("deagle_44");
                //     createdItem = magaz;
                // }
                // case "m4" -> {
                //     AutoRifle gun = new AutoRifle(uid, itemId, getNameForID(itemId));
                //     gun.setData(unBox, bodyFactory, hud, gameStage, this);
                //     createdItem = gun;
                // }
                // case "m4_magazine" -> {
                //     GunMagazine magaz = new GunMagazine(uid, itemId, getNameForID(itemId));
                //     magaz.setData(unBox, bodyFactory, hud, gameStage, this);
                //     magaz.addGunTypes("m4");
                //     magaz.setCapacity(30);
                //     magaz.setCurrentAmount(magaz.getCapacity());
                //     createdItem = magaz;
                // }
                // case "medkit" -> {
                //     Meds med = new Meds(uid, itemId, getNameForID(itemId));
                //     med.setData(unBox, bodyFactory, hud, gameStage, this);
                //     createdItem = med;
                // }
                // case "grenade" -> {
                //     Grenade item = new Grenade(uid, itemId, getNameForID(itemId));
                //     item.setData(unBox, bodyFactory, hud, gameStage, this);
                //     createdItem = item;
                // }
                default: 
                    Item item = ScriptableObject.CreateInstance<Item>();
                    item.Init(_itemsCounter, itemId, GetNameForID(itemId));
                    item.factory = this; 
                    createdItem = item;
                break;
            }
            AddToContainer(createdItem);
            return createdItem;
        }

        public void OnItemDispose(Item item) {
            container.Remove(item.uid);
        }

        private static string GetNameForID(string id){
            return itemNames.GetValueOrDefault(id, id);
        }

        private static void init(){
            itemNames.Add("deagle_44", "Deagle .44");
            itemNames.Add("10mm_fmj", "10mm FMJ ammo");
            itemNames.Add("beef", "Beef");
            itemNames.Add("watches", "Watches");
            itemNames.Add("shotgun_ammo", "Shotgun ammo");
            itemNames.Add("m4", "Auto rifle");
            itemNames.Add("m4_magazine", "Rifle magazine");
            itemNames.Add("pistol_magazine", "Pistol magazine");
            itemNames.Add("medkit", "Medkit");
            itemNames.Add("grenade", "Grenade");
        }

        private void AddToContainer(Item item) {
            container.Add(item.uid, item);
        }
    }
}