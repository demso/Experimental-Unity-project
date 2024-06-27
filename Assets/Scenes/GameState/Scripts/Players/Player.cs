using System.Collections.Generic;
using Scenes.GameState.Scripts.Entities;
using Scenes.GameState.Scripts.Items;
using Scenes.GameState.Scripts.Objects;
using UnityEngine;

namespace Scenes.GameState.Scripts.Players {
    public class Player : Entity, Storage {
        public List<Item> GetInventoryItems() {
            throw new System.NotImplementedException();
        }

        public void SetInventoryItems(params Item[] items) {
            throw new System.NotImplementedException();
        }

        public void RemoveItem(Item item) {
            throw new System.NotImplementedException();
        }

        public void TakeItem(Item item) {
            throw new System.NotImplementedException();
        }

        public Vector2 GetPosition() {
            throw new System.NotImplementedException();
        }

        public string GetName() {
            throw new System.NotImplementedException();
        }
    }
}

