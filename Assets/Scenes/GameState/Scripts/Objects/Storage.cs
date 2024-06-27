using System;
using System.Collections.Generic;
using Scenes.GameState.Scripts.Items;
using UnityEngine;

namespace Scenes.GameState.Scripts.Objects {
    public interface Storage {
        List<Item> GetInventoryItems();
        void SetInventoryItems(params Item[] items);
        void RemoveItem(Item item);
        void TakeItem(Item item);
        Vector2 GetPosition();
        String GetName();
    }
}