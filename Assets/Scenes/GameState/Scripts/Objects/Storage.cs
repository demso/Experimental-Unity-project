using System;
using System.Collections.Generic;
using Scenes.GameState.Scripts.Items;
using UnityEngine;

namespace Scenes.GameState.Scripts.Objects {
    public interface IStorage {
        string Name { get; }
        List<Item> GetInventoryItems();
        void SetInventoryItems(params Item[] items);
        void RemoveItem(Item item);
        void TakeItem(Item item);
        Vector2 GetPosition();
        public T GetItemOfType<T>() where T : Item {
            foreach (Item item in GetInventoryItems()) {
                if (item is T item1)
                    return item1;
            }
            return null;
        }

        public List<T> GetItemsOfType<T>() where T : Item {
            var tempStorage = new List<T>();
            foreach (Item item in GetInventoryItems()){
                if (item is T item1)
                    tempStorage.Add(item1);
            }

            return tempStorage.Count > 0 ? tempStorage : null;
        }
    }
}