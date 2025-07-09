using Scenes.GameState.Scripts.Entities;
using Scenes.GameState.Scripts.Objects;
using Scenes.GameState.Scripts.Players;
using Scenes.GameState.Scripts.Tilemap_Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scenes.GameState.Scripts.Items {
    public class ItemComponent : MonoBehaviour, IInteractable
    {
        private Item _item;

        public Item Item {
            get => _item;
            set {
                _item = value;
                itemID = _item.uid;
            }
        }

        public int itemID;

        public void Interact(Entity entity) {
            if (entity is Player player)
                Item.OnInteraction(player);
        }
    }
}